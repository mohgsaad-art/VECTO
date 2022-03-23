using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

/**
 * Record run-data as rows with key-value pairs into files with consecutive bson docs.
 *
 * ## Instructions
 *
 * - Check test-cases.
 * - Observe the API order: `Open [ --> Record(header-kv) ] --> NewRow [ --> Record ] --> Close
 * - Will scream if out-file already exists
 * - Set `MatRec.Config.Disabled = true;` to stop all activity of this machinery
 * - Read `Store()` for a tip on how to read the result file from Python
 *   without blocking its deletion.
 *
 * ## Infos
 *
 * - [Feature Guide](https://app.clickup.com/2189764/v/dc/22ue4-596/22ue4-526)
 * - See MR !10 & !22
 * - See MatRec.MATREC_VER field for history of changes/version.
 *
 * ## Version Changes:
 *
 * - `matrec_ver: 0.2.0`: MR#22, OOP redesigned, last empty header, +`batch` number, interpolate threadId
 * - `matrec_ver: 0.1.0`: MR#22, fixed repeated stores & zero batches
 * - `matrec_ver: 0.0.0`: MR#22, fixed
 * - `file_version: 0.0.0`:MR#10, initial broken impl
 *
 * ## Sample Output
 *
 * Represented in YAML, where `---` denotes the start of a new BSON-doc:
 *
 *     ```
 *     ---
 *     header:
 *         matrec_ver: 0.2.0
 *         vecto_ver: 1.2.3
 *         batch: 0
 *         row0: 0
 *         h1: h2
 *     rows:
 *         - k1: v1
 *           k2: v2
 *         - pi: 3.14
 *     ---
 *     header:
 *         batch: 1
 *         row0: 2
 *     rows:
 *         - k3: v3
 *     ---
 *     header:
 *         batch: 2
 *         row0: 3
 *     rows: []
 *     ```
 *
 */
namespace TUGraz.VectoCommon.Utils.MatrixRecorder {
    /// <summary>Stuff read only once from env-vars.</summary>
    public class MatRecConfig {
        public const string DISABLED_ENV_VAR_NAME = "MATREC_DISABLED";
        public const string STORE_INTERVAL_ENV_VAR_NAME = "MATREC_STORE_INTERVAL_SEC";
        public const int DEFAULT_STORE_INTERVAL_SEC = 17;

        public bool Disabled;
        /// Vecto-version to add in the header (public for TCs).
        public readonly string VectoVersionId;
        public int BatchStoreIntervalSec;

        public MatRecConfig() {
            var enabledEnvVar = Environment.GetEnvironmentVariable(DISABLED_ENV_VAR_NAME);
            if (!bool.TryParse(enabledEnvVar, out Disabled)) {
                Disabled = false;
            }

            var intervalEnvVar = Environment.GetEnvironmentVariable(STORE_INTERVAL_ENV_VAR_NAME);
            BatchStoreIntervalSec = intervalEnvVar is null ? DEFAULT_STORE_INTERVAL_SEC : int.Parse(intervalEnvVar);

            var dllName = "VectoCommon.dll";
            var assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var commonDll = Assembly.LoadFile(
                Path.Combine(assemblyDirectory, dllName)
            ).GetName();
            VectoVersionId = commonDll.Version.ToString();
        }
    }

    /// <summary>Collects the rows in batches, to be stored in a bson-doc.</summary>
    public class BsonBatch {
        /// The seek-offset of the proper last "empty" batch written,
        /// used to detect if existing file has closed normally.
        public const int LAST_BATCH_BYTES_OFFSET = -38;
        /// How many output files to backup when attempting to overwrite them
        /// before starting to erase them.

        private readonly Logger log = LogManager.GetCurrentClassLogger();
        public readonly MatRecConfig Config;
        // Filepath, assigned when `Open()` is called.
        public readonly string Filepath;

        /// Header dictionary, assigned when `Open()` is called,
        /// and then becoming *light*.
        internal readonly Dictionary<string, object> Header;

        /// List to append completed lines.
        private readonly List<Dictionary<string, object>> PendingRows = new List<Dictionary<string, object>>();

        /// The row where new column-values are appended on `Record()`,
        /// assigned on the 1st call of `NewRow().
        internal Dictionary<string, object> CurrentRow;

        // The 0-based batch index.
        private int Batch = 0;
        // The 0-based index of batch's first-row.
        private int Row0 = 0;

        // Updated when any pending rows are written to disk.
        private readonly Stopwatch StoreReminder = new Stopwatch();

        public BsonBatch(MatRecConfig config, string filepath, Dictionary<string, object> header) {
            Config = config;
            Filepath = interpolateFilepath(filepath);
            Header = header;
            StoreReminder.Start();
        }

        private static string interpolateFilepath(string filepath) {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString("x");
            return filepath.Replace("{thread}", threadId);
        }

        /// Called before creating & appending my out-file,
        /// to ensure that there is no other MatRec using this file,
        /// throws `ArgumentException` otherwise.
        public static void EnsureFileMissingOrFinalized(string filepath) {
            try {
                using (FileStream fs = new FileStream(
                        filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (BsonBinaryReader reader = new BsonBinaryReader(fs)) {
                    // Attempt to parse the last empty doc.
                    //
                    fs.Seek(LAST_BATCH_BYTES_OFFSET, SeekOrigin.End);
                    BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(reader);

                    // Require last empty doc's header as expected (with 'rows' possibly missing).
                    //
                    var header = (IDictionary<string, object>)document["header"];
                    var batch = (int)header["batch"];
                    var row0 = (int)header["row0"];
                }
            } catch (FileNotFoundException) {
                return;  // Can create a new file immediately, nothing blocking its way.
            } catch (Exception ex) {
                throw new ArgumentException($@"Out-file '{filepath}' already exists and is not BSON or has not been closed properly.
  tip: if the Recorder that generated it crashed, you may have to delete/move it manually.", "filepath", ex);
            }
        }

        /// <summary>
        /// Store header and accumulated pendingRows (even if empty).
        /// <return>the number of rows stored</return>
        ///
        /// TIP: from python, open the file for reading without blocking deletes,
        /// folowi this [SO recipe](https://stackoverflow.com/a/18169890/548792).
        /// </summary>
        private int Store() {
            // if (pendingRows is null || mrd.currentRow != null)  // Enable it when developing this util.
            //     throw new InvalidOperationException(
            //         "Forgot user-check for calling Open() or committing currentRow?"
            //     );
            var filepath = this.Filepath;
            var header = this.Header;
            var pendingRows = this.PendingRows;
            var nrows = this.PendingRows.Count();

            header["batch"] = Batch;
            header["row0"] = Row0;

            var root = new Dictionary<string, object>()
            {
                { "header", header },
                { "rows", pendingRows }
            };

            bool createNewFile = Batch == 0;
            if (createNewFile) {
                EnsureFileMissingOrFinalized(filepath);  // Will scream if invalid/unfinished file exists.
            }
            var fileAccess = createNewFile ? FileMode.Create : FileMode.Append;
            BsonDocument bsonDoc = new BsonDocument(root);
            var bsonBinary = bsonDoc.ToBson();

            using (FileStream fs = new FileStream(
                    filepath, fileAccess, FileAccess.Write, FileShare.Read))
            using (BsonBinaryWriter writer = new BsonBinaryWriter(fs)) {
                fs.Write(bsonBinary, 0, bsonBinary.Length);
            }
            var rowsStored = pendingRows.Count;
            log.Info("Written {0} bytes for {1} rows in batch {2}:{3} --> '{4}'.",
                    rowsStored, bsonBinary.Length, Batch, Row0, filepath);

            header.Clear();
            Batch += 1;
            Row0 += pendingRows.Count;
            pendingRows.Clear();  // Subsequent docs have *light* headers.

            return nrows;
        }

        /// <summary>
        /// Store batch of pending rows based based on time/size, reportin any failures.
        ///
        /// If forced, always stores (just a header if no pendingRows exist).
        /// </summary>
        internal int CommitRowAndPotentiallyStore(bool forceStore = false) {
            int nrows = -2;  // Sentinel
            var pendingRows = this.PendingRows;
            var filepath = this.Filepath;

            var crow = this.CurrentRow;
            if (crow != null && crow.Any()) {  // Don't produce an empty-row.
                pendingRows.Add(crow);
            }
            this.CurrentRow = new Dictionary<string, object>();

            bool isTimeToStore = StoreReminder.Elapsed.TotalSeconds > Config.BatchStoreIntervalSec;
            if (forceStore || isTimeToStore) {
                StoreReminder.Restart();  // Restart regardless of any failures.
            }

            bool isAnyRowsPending = pendingRows.Any();
            if (forceStore || isAnyRowsPending && isTimeToStore) {
                try {
                    nrows = Store();
                } catch (IOException ex) {
                    if (forceStore) { // Scream, since Close() really ordered to store batch.
                        throw;
                    }
                    log.Warn("Soft-failure storing {0} rows in batch {1}:{2} --> '{3}' due to: {4}",
                            nrows, Batch, Row0, filepath, ex);
                    nrows = -1;
                }
            } else if (!isAnyRowsPending)
                log.Warn("Skipped writting empty batch {0}:{1} --> '{2}'.", Batch, Row0, filepath);

            return nrows;
        }
    }

    /// <summary>The public API of Matrix Recorder.</summary>
    public static class MatRec {
        /// For past version-changes, see the comment at the top of this source-file.
        public const string MATREC_VER = "0.2.0";

        private static MatRecConfig _Config;
        public static MatRecConfig Config {
            get {
                if (_Config is null)
                    _Config = new MatRecConfig();
                return _Config;
            }
            set {  // Just For TCs.
                _Config = value;
            }
        }

        /// <summary>The actual worker doing all the work</summary>
        static readonly ThreadLocal<BsonBatch> MatRecThreadLocal = new ThreadLocal<BsonBatch>();
        static BsonBatch MatRecData {
            get {
                var batch = MatRecThreadLocal.Value;
                if (batch is null) {
                    throw new InvalidOperationException("Forgot calling Open() from Open->NewRow->Record->Close.");
                }
                return batch;
            }
        }

        /// <summary>
        /// Enable recording of key-value pairs in rows for this thread,
        /// to be stored later, on regular intervals.
        ///
        /// Atention: better not call it twice or it will override your file.
        /// </summary>
        public static void Open(string filepath) {
            var conf = Config;
            if (conf.Disabled) {
                return;
            }

            MatRecThreadLocal.Value = new BsonBatch(
                    conf, filepath, new Dictionary<string, object>
                    {
                            // Keep file_version first to the top, to facilitate parsers.
                            { "matrec_ver", MATREC_VER },
                            { "vecto_ver", conf.VectoVersionId },
                            { "batch", -1 },
                            { "row0", -1 },
                    }
            );
        }

        /// <summary>
        /// Write pending pendingRows into file:
        /// 1. Append current row to pendingRows
        /// 2. Create a document with header and pendingRows
        /// 3. Write document into the file
        /// 4. Clear the thread local data
        /// Scream if  `Open()` has not been called.
        /// </summary>
        public static void Close() {
            if (Config.Disabled) {
                return;
            }

            BsonBatch mrd = MatRecData;
            int rowsStored = mrd.CommitRowAndPotentiallyStore(true);
            if (rowsStored > 0)
                // Store a last header, to facilitate parser calculate nrows & nbatches.
                mrd.CommitRowAndPotentiallyStore(true);

            MatRecThreadLocal.Value = null;
        }

        /// <summary>
        /// Send any old row to the sink and initiate a new one.
        /// Create a new underlying *dict* to store values, and append any given values in `index`.
        /// Scream if  `Open()` has not been called.
        /// </summary>
        /// <param name="key">key to be recorded; if null no column is written</param>
        /// <param name="value">value to be recorded; if null no column is written</param>
        public static void NewRow(string key, object value) {
            if (Config.Disabled) {
                return;
            }

            BsonBatch mrd = MatRecData;
            mrd.CommitRowAndPotentiallyStore();
            mrd.CurrentRow[key] = value;
        }

        /// <summary>
        /// Send any old row to the sink and initiate a new one.
        /// Create a new underlying *dict* to store values, and append any given values in `index`.
        /// Scream if  `Open()` has not been called.
        /// </summary>
        /// <param name="values">List of key value pairs to be updated in current row</param>
        public static void NewRow(IEnumerable<KeyValuePair<string, object>> values = null) {
            if (Config.Disabled) {
                return;
            }

            BsonBatch mrd = MatRecData;
            mrd.CommitRowAndPotentiallyStore();
            var crow = mrd.CurrentRow;
            if (values != null) {
                foreach (var item in values) {
                    crow[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// Record a new key-value pair.
        /// Append the pair into the *dict* representing the row.
        /// Any pre-existing value for some key is overwritten.
        ///
        /// The 1st time before `NewRow()` has ever been called,
        /// any key-value pair is recorder into header.
        /// </summary>
        /// <param name="key">a key to be inserted into header or rows</param>
        /// <param name="value">a value to be inserted into header or rows</param>
        public static void Record(string key, object value) {
            if (Config.Disabled) {
                return;
            }

            BsonBatch mrd = MatRecData;
            var destination = (mrd.CurrentRow is null) ? mrd.Header : mrd.CurrentRow;
            destination[key] = value;
        }

        /// <summary>
        /// En-mass `Record()` key-value pairs.
        ///
        /// The 1st time before `NewRow()` has ever been called,
        /// any items are recorder into header.
        /// </summary>
        /// <param name="values">key-value pairs to insert into header or rows.</param>
        public static void Record(IEnumerable<KeyValuePair<string, object>> values) {
            if (Config.Disabled) {
                return;
            }

            BsonBatch mrd = MatRecData;
            var destination = (mrd.CurrentRow is null) ? mrd.Header : mrd.CurrentRow;
            foreach (var item in values) {
                destination[item.Key] = item.Value;
            }
        }
    }
}

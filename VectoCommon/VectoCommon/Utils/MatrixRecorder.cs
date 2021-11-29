using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace TUGraz.VectoCommon.Utils.MatrixRecorder
{
    public class MatRecData
    {
        // Filename, stored when Open is called
        public string filename;

        // Header dictionary, stored when Open is called
        public Dictionary<string, object> headerDict;

        // List to append completed lines
        public List<Dictionary<string, object>> pendingRows;

        // The row where new column-values are appended on Record().
        public Dictionary<string, object> currentRow;

        // The 0-based row where new column-values are appended on Record().
        public int row0;

        // Updated when any pending rows are written to disk.
        public Stopwatch writeReminder;

        /// <summary>
        /// Clear Thread local storage
        /// </summary>
        public void Clear()
        {
            currentRow = null;
            row0 = 0;
            pendingRows = null;
            headerDict = null;
            filename = null;
            writeReminder.Stop();
            writeReminder.Reset();
        }
    }

    /// <summary>The public API of Matrix Recorder.</summary>
    public static class MatRec
    {
        public const string STORE_INTERVAL_ENV_VAR_NAME = "MATREC_STORE_INTERVAL_SEC";
        public const int DEFAULT_STORE_INTERVAL_SEC = 17;

        /// <summary>
        /// Thread local data initialization
        /// </summary>
        private static ThreadLocal<MatRecData> MatRecThreadLocal =
            new ThreadLocal<MatRecData>(() => new MatRecData());

        private static string _VectoVersionId;
        // Vecto-version to add in the header (public for TCs).
        public static string VectoVersionId
        {
            get
            {
                if (_VectoVersionId is null)
                {
                    var dllName = "VectoCommon.dll";
                    var assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var commonDll = Assembly.LoadFile(
                        Path.Combine(assemblyDirectory, dllName)
                    ).GetName();

                    _VectoVersionId = commonDll.Version.ToString();
                }

                return _VectoVersionId;
            }
        }

        /// <summary>
        /// Prepare the thread local data with needed properties for recording pendingRows and storing them later.
        ///
        /// Atention: better not call it twice or it will override your file.
        /// </summary>
        /// <param name="header">Metadata like cycle info, the file version etc.</param>
        public static void Open(string filepath, IEnumerable<KeyValuePair<string, object>> header = null)
        {
            MatRecData mrd = MatRecThreadLocal.Value;
            mrd.filename = filepath;

            Dictionary<string, object> headerDict = new Dictionary<string, object>
            {
                // Keep file_version first to the top, to facilitate parsers.
                { "matrec_ver", "0.0.0" },
                { "vecto_ver", VectoVersionId },
            };

            if (header != null)
            {
                foreach (var item in header)
                {
                    headerDict.Add(item.Key, item.Value);
                }
            };

            mrd.headerDict = headerDict;
            mrd.currentRow = null;
            mrd.row0 = 0;
            mrd.pendingRows = new List<Dictionary<string, object>>();
            mrd.writeReminder = new Stopwatch();
            mrd.writeReminder.Start();
        }

        /// <summary>
        /// Store header & accumulated pendingRows
        /// </summary>
        private static void StoreCurrentDoc()
        {
            MatRecData mrd = MatRecThreadLocal.Value;
            var pendingRows = mrd.pendingRows;

            string filename = mrd.filename;
            var row0 = mrd.row0;

            if (pendingRows == null || !pendingRows.Any())
            {
                // FIXME: use proper logging
                Debug.Write($"Matrec skipped writting file({filename}) with empty pendingRows@{row0}.");
                return;
            }

            Dictionary<string, object> header = mrd.headerDict;
            header["row0"] = row0;

            Dictionary<string, object> root = new Dictionary<string, object>()
            {
                ["header"] = header,
                ["rows"] = pendingRows
            };

            bool isFirstDoc = row0 == 0;
            var fileAccess = isFirstDoc ? FileMode.Create : FileMode.Append;
            BsonDocument bsonDoc = new BsonDocument(root);
            var bsonBinary = bsonDoc.ToBson();

            using (FileStream fs = new FileStream(mrd.filename, fileAccess, FileAccess.Write, FileShare.Write))
            using (BsonBinaryWriter writer = new BsonBinaryWriter(fs))
                fs.Write(bsonBinary, 0, bsonBinary.Length);

            mrd.headerDict = new Dictionary<string, object>();  // subsequent docs have light headers.
            mrd.row0 += pendingRows.Count;
            pendingRows.Clear();
        }

        /// <summary>
        /// Write pending pendingRows into file:
        /// 1. Append current row to pendingRows
        /// 2. Create a document with header and pendingRows
        /// 3. Write document into the file
        /// 4. Clear the thread local data
        /// Scream if  `Open()` has not been called.
        /// </summary>
        public static void Close()
        {
            MatRecData mrd = MatRecThreadLocal.Value;
            if (mrd.pendingRows is null)
                throw new InvalidOperationException("Forgotten Open() from Open->NewRow->Record->Close.");

            if (mrd.currentRow != null)
                mrd.pendingRows.Add(mrd.currentRow);

            StoreCurrentDoc();

            mrd.Clear();
        }

        /// <summary>
        /// Send any old row to the sink and initiate a new one.
        /// Create a new underlying *dict* to store values, and append any given values in `index`.
        /// Scream if  `Open()` has not been called.
        /// </summary>
        /// <param name="key">key to be recorded; if null no column is written</param>
        /// <param name="value">value to be recorded; if null no column is written</param>
        public static void NewRow(string key = null, object value = null)
        {
            MatRecData mrd = MatRecThreadLocal.Value;
            if (mrd.pendingRows is null)
                throw new InvalidOperationException("Forgotten Open() from Open->NewRow->Record->Close.");

            if (mrd.currentRow != null)
                mrd.pendingRows.Add(mrd.currentRow);

            var intervalEnvVar = Environment.GetEnvironmentVariable(STORE_INTERVAL_ENV_VAR_NAME);
            var docStoreInterval = intervalEnvVar is null ? DEFAULT_STORE_INTERVAL_SEC : int.Parse(intervalEnvVar);
            if (mrd.writeReminder.Elapsed.TotalSeconds > docStoreInterval)
            {
                try
                {
                    StoreCurrentDoc();
                } catch (IOException ex)
                {
                    // FIXME: use proper logging
                    Debug.Write($"Matrec failed storing batch row0({mrd.row0})@({mrd.filename}) due to: {ex}");
                }
            }

            mrd.currentRow = new Dictionary<string, object>();
            if (key != null)
                mrd.currentRow[key] = value;
        }

        /// <summary>
        /// Send any old row to the sink and initiate a new one.
        /// Create a new underlying *dict* to store values, and append any given values in `index`.
        /// Scream if  `Open()` has not been called.
        /// </summary>
        /// <param name="index">List of key value pairs to be updated in current row</param>
        public static void NewRow(IEnumerable<KeyValuePair<string, object>> index)
        {
            MatRecData mrd = MatRecThreadLocal.Value;
            NewRow(null, null);
            mrd.currentRow = new Dictionary<string, object>();
            foreach (var item in index)
                mrd.currentRow[item.Key] = item.Value;
        }

        /// <summary>
        /// Record a new key-value pair. 
        /// Append the pair into the *dict* representing the row.
        /// Any pre-existing value for some key is overwritten.
        /// Scream if NewRow() has not been called after `Open()`.
        /// </summary>
        /// <param name="key">key to be recorded</param>
        /// <param name="value">value to be recorded</param>
        public static void Record(string key, object value)
        {
            MatRecData mrd = MatRecThreadLocal.Value;
            var row = mrd.currentRow;
            if (row is null)
                throw new InvalidOperationException("Forgotten NewRow() from Open->NewRow->Record->Close.");

            row[key] = value;
        }

        /// <summary>
        /// Send any old row to the sink and initiate a new one.
        /// Create a new underlying *dict* to store values, and append any given values in `index`.
        /// Check if the recorder has not used `Open()` yet.
        /// </summary>
        /// <param name="key">key to be recorded; if null no column is written</param>
        /// <param name="value">value to be recorded; if null no column is written</param>
        public static void NewRow(string key = null, object value = null)
        {
            Debug.Assert(ThreadLocalData.pendingRows != null, "pendingRows != null");

            if (ThreadLocalData.currentRow != null)
            {
                ThreadLocalData.pendingRows.Add(ThreadLocalData.currentRow);
            }
            ThreadLocalData.currentRowIndex++;

            var docStoreInterval = int.Parse(Environment.GetEnvironmentVariable("DocStoreInterval"));
            if (ThreadLocalData.writeReminder.Elapsed.TotalSeconds > docStoreInterval)
            {
                StoreCurrentDoc();
            }

            ThreadLocalData.currentRow = new Dictionary<string, object>();
            if (key != null)
            {
                ThreadLocalData.currentRow[key] = value;
            }
        }

        /// <summary>
        /// En-mass `Record()` key-value pairs
        /// Scream if NewRow() has not been called after `Open()`.
        /// </summary>
        /// <param name="values">list of key value pairs to be recorded</param>
        public static void Record(IEnumerable<KeyValuePair<string, object>> values)
        {
            MatRecData mrd = MatRecThreadLocal.Value;
            var row = mrd.currentRow;
            if (row is null)
                throw new InvalidOperationException("Forgotten NewRow() from Open->NewRow->Record->Close.");

            foreach (var item in values)
                row[item.Key] = item.Value;
        }
    }
}

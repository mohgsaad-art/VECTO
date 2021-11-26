using MongoDB.Bson;
using MongoDB.Bson.IO;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCommon.Utils.MatrixRecorder
{
    public class ThreadLocalData
    {
        // Filename, stored when Open is called
        public string filename;

        // Header dictionary, stored when Open is called
        public Dictionary<string, object> headerDict;

        // List to append completed lines
        public List<Dictionary<string, object>> pendingRows;

        // The row where new column-values are appended on Record().
        public Dictionary<string, object> currentRow;

        // The row where new column-values are appended on Record().
        public int currentRowIndex;

        // Updated when any pending rows are written to disk.
        public Stopwatch writeReminder;

        /// <summary>
        /// Clear Thread local storage
        /// </summary>
        public void Clear()
        {
            currentRow = null;
            currentRowIndex = -1;
            pendingRows = new List<Dictionary<string, object>>();
            headerDict = new Dictionary<string, object>
            {
                // Keep file_version first to the top, to facilitate parsers.
                { "file_version", "0.0.0" },
                { "vecto_version", GetVectoCommonVersionInformation() },
                { "name", "matrixrecorder" }
            }; ;
            filename = null;
            writeReminder.Stop();
            writeReminder.Reset();
        }

        private string GetVectoCommonVersionInformation()
        {
            return "debug version";
        }
    }

    public static class MatrixRecorder
    {
        /// <summary>
        /// Thread local data initialization
        /// </summary>
        private static ThreadLocal<ThreadLocalData> _ThreadLocalData =
            new ThreadLocal<ThreadLocalData>(() => new ThreadLocalData());

        /// <summary>
        /// Absolute time recording
        /// </summary>
        public static double AbsTime;

        /// <summary>
        /// Thread local data
        /// </summary>
        private static ThreadLocalData ThreadLocalData
        {
            get { return _ThreadLocalData.Value; }
            set { _ThreadLocalData.Value = value; }
        }
        
        // vecto common dll name, used for retrieving the version
        private static readonly string commonDllName = "VectoCommon.dll";

        /// <summary>
        /// Prepare the thread local data with needed properties for recording pendingRows and storing them later.
        /// </summary>
        /// <param name="header">Metadata like cycle info, the file version etc.</param>
        public static void Open(string filepath, IEnumerable<KeyValuePair<string, object>> header)
        {
            ThreadLocalData.filename = filepath;

            Dictionary<string, object> headerDict = new Dictionary<string, object>
            {
                // Keep file_version first to the top, to facilitate parsers.
                { "file_version", "0.0.0" },
                { "vecto_version", GetVectoCommonVersionInformation() },
                { "name", "matrixrecorder" }
            };

            if (header != null)
            {
                foreach (var item in header)
                {
                    headerDict.Add(item.Key, item.Value);
                }
            }

            ThreadLocalData.headerDict = headerDict;
            ThreadLocalData.currentRow = null;
            ThreadLocalData.currentRowIndex = -1;
            ThreadLocalData.pendingRows = new List<Dictionary<string, object>>();
            ThreadLocalData.writeReminder = new Stopwatch();
            ThreadLocalData.writeReminder.Start();
        }

        /// <summary>
        /// Store header & accumulated pendingRows
        /// </summary>
        private static void StoreCurrentDoc()
        {
            var pendingRows = ThreadLocalData.pendingRows;

            string filename = ThreadLocalData.filename;
            var row0 = ThreadLocalData.currentRowIndex + 1;

            if (pendingRows == null || !pendingRows.Any())
            {
                Debug.Write("$Skipped writting file({ filename}) with empty pendingRows@{row0}.");
                return;
            }
            if (ThreadLocalData.headerDict == null)
            {
                ThreadLocalData.headerDict = new Dictionary<string, object>
                    {
                        // Keep file_version first to the top, to facilitate parsers.
                        { "file_version", "0.0.0" },
                        { "vecto_version", GetVectoCommonVersionInformation() },
                        { "name", "matrixrecorder" }
                    };
            }
            Dictionary<string, object> header = ThreadLocalData.headerDict;
            header["row0"] = row0;

            Dictionary<string, object> root = new Dictionary<string, object>()
            {
                ["header"] = header,
                ["rows"] = pendingRows
            };

            bool isFirstDoc = row0 == 0;
            //BsonDocument bsonDoc = new BsonDocument(root.ToBsonDocument());

            BsonDocument bsonDoc = new BsonDocument(root);
            
            //BsonDocument bsonDoc = BsonDocument.Create(root);

            var bsonBinary = bsonDoc.ToBson();

            if ( ThreadLocalData.filename == null )
            {
                ThreadLocalData.filename = Directory.GetCurrentDirectory() + "\\Logs\\Matrec" + DateTime.Now.ToString("MMddhhmmss");
            }
            using (FileStream fs = new FileStream(
                        ThreadLocalData.filename,
                        isFirstDoc ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Write)
                   )
            using (BsonBinaryWriter writer = new BsonBinaryWriter(fs))
            {
                fs.Write(bsonBinary, 0, bsonBinary.Length);
            }

            ThreadLocalData.pendingRows.Clear();
            if (ThreadLocalData.writeReminder == null)
            {
                ThreadLocalData.writeReminder = new Stopwatch();
            }
            ThreadLocalData.writeReminder.Reset();
        }

        /// <summary>
        /// Write pending pendingRows into file:
        /// 1. Append current row to pendingRows
        /// 2. Create a document with header and pendingRows
        /// 3. Write document into the file
        /// 4. Clear the thread local data
        /// </summary>
        public static void Close()
        {
            //Debug.Assert(ThreadLocalData.pendingRows != null, "pendingRows != null");

            if (ThreadLocalData.currentRow != null)
            {
                if (ThreadLocalData.pendingRows == null)
                {
                    ThreadLocalData.pendingRows = new List<Dictionary<string, object>>();
                }
                ThreadLocalData.pendingRows.Add(ThreadLocalData.currentRow);
                ThreadLocalData.currentRowIndex++;
            }

            StoreCurrentDoc();

            ThreadLocalData.Clear();
        }

        /// <summary>
        /// Send any old row to the sink and initiate a new one.
        /// Create a new underlying *dict* to store values, and append any given values in `index`.
        /// Check if the recorder has not used `Open()` yet.
        /// </summary>
        /// <param name="index">List of key value pairs to be updated in current row</param>
        public static void NewRow(IEnumerable<KeyValuePair<string, object>> index = null)
        {
            NewRow(null, null);
            ThreadLocalData.currentRow = new Dictionary<string, object>();
            if (index != null)
            {
                foreach (var item in index)
                {
                     ThreadLocalData.currentRow[item.Key] = item.Value;
                }
                ThreadLocalData.currentRow["AbsTime"] = AbsTime;
            }
        }

        /// <summary>
        /// Record a new key-value pair. 
        /// Append the pair into the *dict* representing the row.
        /// Any pre-existing value for some key is overwritten.
        /// Check if recorder has not used `Open()` yet.
        /// </summary>
        /// <param name="key">key to be recorded</param>
        /// <param name="value">value to be recorded</param>
        public static void Record(string key, object value)
        {
            // NRE error if NewRow() has not been called
            var row = ThreadLocalData.currentRow;

            Debug.Assert(row != null, "row != null");

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
            try
            {
                //Debug.Assert(ThreadLocalData.pendingRows != null, "pendingRows != null");

                if (ThreadLocalData.currentRow != null)
                {
                    ThreadLocalData.pendingRows.Add(ThreadLocalData.currentRow);
                    ThreadLocalData.currentRowIndex++;
                }

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
            catch (Exception ex) {
                Logger _log;
                _log = NLog.LogManager.GetLogger("Matrix Recorder");
                _log.Debug(ex.ToString());
            }

        }

        /// <summary>
        /// En-mass `Record()` key-value pairs
        /// </summary>
        /// <param name="values">list of key value pairs to be recorded</param>
        public static void Record(IEnumerable<KeyValuePair<string, object>> values)
        {
            //NRE error if NewRow() has not been called
            var row = ThreadLocalData.currentRow;
            Debug.Assert(row != null, "row != null");

            foreach (var item in values)
            {
                row[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// Get the version from vecto common
        /// </summary>
        private static string GetVectoCommonVersionInformation()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var commonDll = Assembly.LoadFile(Path.Combine(currentDirectory, commonDllName))
              .GetName();

            return commonDll.Version.ToString();
        }

        /// <summary>
        /// Update absolute time
        /// </summary>
        public static void UpdateAbsTime(double absTime)
        {
            AbsTime = absTime;
        }
    }
}

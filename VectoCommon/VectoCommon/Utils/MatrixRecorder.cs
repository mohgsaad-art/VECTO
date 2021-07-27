using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MatrixRecorder
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
            pendingRows = null;
            headerDict = null;
            filename = null;
            writeReminder.Stop();
            writeReminder.Reset();
        }
    }
    
    public static class MatrixRecorder 
    {
        private static readonly long docStoreIntervalSec = 1;

        private static ThreadLocal<ThreadLocalData> _ThreadLocalData =
            new ThreadLocal<ThreadLocalData>(() => new ThreadLocalData());

        private static ThreadLocalData ThreadLocalData
        {
            get { return _ThreadLocalData.Value; }
            set { _ThreadLocalData.Value = value; }
        }

        /// <summary>
        /// Prepare the thread local data with needed properties for recording pendingRows and storing them later.
        /// </summary>
        /// <param name="header">Metadata like cycle info, the file version etc.</param>
        public static void Open(string filepath, IEnumerable<KeyValuePair<string, object>> header)
        {
            ThreadLocalData.filename = filepath;

            // the order needs to be kept, file_version pair should be first            
            Dictionary<string, object> headerDict = new Dictionary<string, object>
            {
                { "file_version", "0.0.0" },
                //{ "vecto_version", VectoSimulationCore.VersionNumber },
                { "name", "matrixrecorder" }
            };

            foreach (var item in header)
            {
                headerDict.Add(item.Key, item.Value);
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
            var row0 = ThreadLocalData.currentRowIndex;
            if (pendingRows == null || !pendingRows.Any())
            {
                //Log.Info($"Skipped writting file({ filename}) with empty pendingRows@{row0}.");
                return;
            }

            Dictionary<string, object> header = ThreadLocalData.headerDict;
            header["row0"] = row0;

            Dictionary<string, object> root = new Dictionary<string, object>()
            {
                ["header"] = header,
                ["rows"] = pendingRows
            };

            var doc = new BsonDocument(root);
            using (FileStream fs = new FileStream(ThreadLocalData.filename,
                FileMode.Append, FileAccess.Write, FileShare.Read))
                using (BsonBinaryWriter writer = new BsonBinaryWriter(fs))
                {
                    BsonSerializer.Serialize(writer, root);
                }

            ThreadLocalData.pendingRows.Clear();
            ThreadLocalData.writeReminder.Reset();
        }

        /// <summary>
        /// Write pending pendingRows into file:
        /// 1. Append current row to pendingRows
        /// 2. Create a document with header and pendingRows
        /// 3. Write document into the file
        /// 4. Clear the tl
        /// </summary>
        public static void Close()
        {
            if (ThreadLocalData.currentRow != null && ThreadLocalData.pendingRows != null)
            {
                ThreadLocalData.pendingRows.Add(ThreadLocalData.currentRow);
            }

            StoreCurrentDoc();

            ThreadLocalData.Clear();
        }

        /// <summary>
        /// Send any old row to the sink and initiate a new one.
        /// Create a new underlying *dict* to store values, and append any given values in `index`.
        /// Check if recorder has not `Open()` yet.
        /// </summary>
        /// <param name="index">List of key value pairs to be updated in current row</param>
        public static void NewRow(IEnumerable<KeyValuePair<string, object>> index = null)
        {
            if (ThreadLocalData.currentRow != null && ThreadLocalData.pendingRows != null)
            {
                ThreadLocalData.pendingRows.Add(ThreadLocalData.currentRow);
                ThreadLocalData.currentRowIndex++;
            }
            
            if (ThreadLocalData.writeReminder.Elapsed.TotalSeconds > docStoreIntervalSec)
            {
                StoreCurrentDoc();
            }

            ThreadLocalData.currentRow = new Dictionary<string, object>();
            if (index != null)
            {
                foreach (var item in index)
                {
                    ThreadLocalData.currentRow[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// Record a new key-value pair. 
        /// Append the pair into the *dict* representing the row.
        /// Any pre-existing value for some key is overwritten.
        /// Check if recorder has not `Open()` yet.
        /// </summary>
        /// <param name="key">key to be recorded</param>
        /// <param name="value">value to be recorded</param>
        public static void Record(string key, object value)
        {
            //NRE error if NewRow() has not been called
            var row = ThreadLocalData.currentRow;

            row[key] = value;
        }

        /// <summary>
        /// En-mass `Record()` key-value pairs
        /// </summary>
        /// <param name="values">list of key value pairs to be recorded</param>
        public static void Record(IEnumerable<KeyValuePair<string, object>> values)
        {
            //NRE error if NewRow() has not been called
            var row = ThreadLocalData.currentRow;

            foreach (var item in values)
            {
                row[item.Key] = item.Value;
            }
        }

        private static void ShowVersionInformation()
        {
            var core = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VectoCore.dll"))
              .GetName();
            //Console.WriteLine(string.Format(@"CoreLibrary: {0}", core.Version));
        }
    
    }
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MatrixRecorder
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MatrixRecorderTests
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.SetEnvironmentVariable("DocStoreInterval", "33");
         
            File.Delete(Path.Combine(Path.GetTempPath(), "out1.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out2.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out3.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out4.bson"));
        }

        [TestCase]
        public static void BSON_MatrixRecorder_Basic()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out1.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key 1", "record val 1");
            
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key 2", "record val 2");

            MatrixRecorder.Close();
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out2.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(60000);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed_two_bson_doc_subseq_first_without_close()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out3.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(60000);

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out3.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();
        }

        [TestCase]
        public static void BSON_MatrixRecorder_two_bson_doc_subseq()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out4.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            MatrixRecorder.Close();

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out4.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();
        }
    }

}

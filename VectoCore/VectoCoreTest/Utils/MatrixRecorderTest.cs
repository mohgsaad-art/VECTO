using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCommon.Utils.MatrixRecorder
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MatrixRecorderTests
    {
        private static void CheckOutput(string outpath, string expectedJsonStr)
        {

            using (FileStream fs = new FileStream(
                        outpath,
                        FileMode.Open, FileAccess.Read, FileShare.Read)
                   )
            {
                using (BsonBinaryReader reader = new BsonBinaryReader(fs))
                {
                    var document = BsonSerializer.Deserialize<BsonDocument>(reader);

                    var jsonStr = document.ToJson();

                    Assert.AreEqual(expectedJsonStr, jsonStr);
                }
            }
        }
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
            var expectedJsonStr = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 1 }, \"rows\" : [{ \"dict a\" : \"dict b\", \"record key 1\" : \"record val 1\" }, { \"dict a\" : \"dict b\", \"record key 2\" : \"record val 2\" }] }";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();
            string outpath = Path.Combine(Path.GetTempPath(), "out1.bson");
            MatrixRecorder.Open(outpath, headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key 1", "record val 1");
            
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key 2", "record val 2");
            
            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed()
        {
            var expectedJsonStr = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 0 }, \"rows\" : [{ \"dict a\" : \"dict b\", \"record key1\" : \"record val1\", \"record key2\" : \"record val2\" }] }";
                    
            string outpath = Path.Combine(Path.GetTempPath(), "out2.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(outpath, headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(60000);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed_two_bson_doc_subseq_first_without_close()
        {
            var expectedJsonStr = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 1 }, \"rows\" : [{ \"dict a\" : \"dict b\", \"record key2\" : \"record val2\" }, { \"dict a\" : \"dict b\" }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out3.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(outpath, headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(60000);

            MatrixRecorder.Open(outpath, headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_two_bson_doc_subseq()
        {
            var expectedJsonStr = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 0 }, \"rows\" : [{ \"dict a\" : \"dict b\", \"record key1\" : \"record val1\" }] }";

                    
            string outpath = Path.Combine(Path.GetTempPath(), "out4.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(outpath, headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            MatrixRecorder.Close();

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out4.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_null_header()
        {
            var expectedJsonStr = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 1 }, \"rows\" : [{ }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out5.bson");

            MatrixRecorder.Open(outpath, null);

            MatrixRecorder.NewRow(null);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_custom_header()
        {
            var expectedJsonStr = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"CycleName\" : \"\", \"CycleType\" : 3, \"row0\" : 1 }, \"rows\" : [{ }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out6.bson");
            DrivingCycleData cycle = new DrivingCycleData
            {
                Name = "",
                CycleType = CycleType.MeasuredSpeed
            };

            Dictionary<string, object> headerDict = new Dictionary<string, object>()
            {
                ["CycleName"] = cycle.Name,
                ["CycleType"] = cycle.CycleType
            };

            MatrixRecorder.Open(outpath, headerDict);

            MatrixRecorder.NewRow(null);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }
    }
}

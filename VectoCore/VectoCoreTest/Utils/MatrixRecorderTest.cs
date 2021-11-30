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
        private const int store_delay_sec = 1;

        private static void CheckOutput(string outpath, string expectedJsonStr)
        {
            expectedJsonStr = expectedJsonStr.Replace("@@", MatrixRecorder.GetVectoCommonVersionInformation());

            using (FileStream fs = new FileStream(
                        outpath,
                        FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BsonBinaryReader reader = new BsonBinaryReader(fs))
                {
                    int ndoc = 0;
                    int initExpLength = expectedJsonStr.Length;
                    while (!reader.IsAtEndOfFile())
                    {
                        var document = BsonSerializer.Deserialize<BsonDocument>(reader);

                        var jsonStr = document.ToJson();
                        var jslen = jsonStr.Length;

                        // Chop expected-string to the chunk just read only if lengths matching,
                        // so that we still receive the assert-equal message on length-mismatches.
                        var chopLen = (jslen <= expectedJsonStr.Length) ? jslen : expectedJsonStr.Length;
                        Assert.AreEqual(expectedJsonStr.Substring(0, chopLen), jsonStr, $"ndoc={ndoc}, initExpLength ={initExpLength}");

                        expectedJsonStr = expectedJsonStr.Substring(jslen);
                        ndoc++;
                    }
                    Assert.Zero(expectedJsonStr.Length, $"ndoc={ndoc}, initExpLength ={initExpLength}, remaining expectedJsonStr={expectedJsonStr}");
                }
            }
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.SetEnvironmentVariable("DocStoreInterval", $"{store_delay_sec}");
            File.Delete(Path.Combine(Path.GetTempPath(), "out1.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out2.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out3.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out4.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out5.bson"));
            File.Delete(Path.Combine(Path.GetTempPath(), "out6.bson"));
        }

        [TestCase]
        public static void BSON_MatrixRecorder_Basic()
        {
            var expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key 1\" : \"record val 1\" }, { \"dict a\" : \"dict b\" }] }";
            var dict = new Dictionary<string, object>()
            {
                { "dict a", "dict b" }
            };

            string outpath = Path.Combine(Path.GetTempPath(), "out1.bson");
            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key 1", "record val 1");

            MatrixRecorder.NewRow();

            MatrixRecorder.Record(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed()
        {
            var expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"row0\" : 1 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key1\" : \"record val1\", \"record key2\" : \"record val2\" }] }"
                + "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\" }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out2.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "dict a", "dict b" }
            };

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(store_delay_sec * 1500);

            MatrixRecorder.Record("record key2", "record val2");

            // 1st doc closes here.

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed_2_open_docs_without_close_between()
        {
            string expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key2\" : \"record val2\" }, { \"dict a\" : \"dict b\" }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out3.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "dict a", "dict b" }
            };

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(store_delay_sec * 1500);

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_interspese_close_open_with_newrow()
        {
            string outpath = Path.Combine(Path.GetTempPath(), "out3.bson");

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow("a", "aa");
            MatrixRecorder.Close();

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow("b", "bb");

            MatrixRecorder.Close();
        }

        [TestCase]
        public static void BSON_MatrixRecorder_interspese_close_open()
        {
            string outpath = Path.Combine(Path.GetTempPath(), "out3.bson");

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow("a", "aa");
            MatrixRecorder.Close();

            MatrixRecorder.Open(outpath);

            MatrixRecorder.Record("b", "bb");

            MatrixRecorder.Close();
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed_interspese_open()
        {
            string outpath = Path.Combine(Path.GetTempPath(), "out3.bson");

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow("a", "aa");

            Thread.Sleep(store_delay_sec * 1500);

            MatrixRecorder.Open(outpath);

            MatrixRecorder.Record("b", "bb");

            MatrixRecorder.Close();
        }

        [TestCase]
        public static void BSON_MatrixRecorder_2_open_close_docs()
        {
            string expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"h\" : 22, \"row0\" : 1 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key1\" : \"record val1\" }] }"
                + "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key2\" : \"record val2\" }, { \"dict a\" : \"dict b\" }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out4.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "dict a", "dict b" }
            };

            Dictionary<string, object> headerDict = new Dictionary<string, object>()
            {
                {"h", 22}
            };

            MatrixRecorder.Open(outpath, headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            MatrixRecorder.Close();

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_null_header()
        {
            var expectedJsonStr =
                "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"row0\" : 1 }"
                + ", \"rows\" : [{ }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out5.bson");

            MatrixRecorder.Open(outpath, null);

            MatrixRecorder.NewRow();

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_custom_header()
        {
            var expectedJsonStr =
                "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"@@\", \"name\" : \"matrixrecorder\", \"CycleName\" : \"\", \"CycleType\" : 3, \"row0\" : 1 }"
                + ", \"rows\" : [{ }] }";

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

            MatrixRecorder.NewRow();

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }
    }
}

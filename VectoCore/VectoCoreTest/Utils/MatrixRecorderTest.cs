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
            Environment.SetEnvironmentVariable("DocStoreInterval", "33");

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
            // FIXME: version-id will fail in the future
            var expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
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
            // FIXME: version-id will fail in the future
            var expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 1 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key1\" : \"record val1\", \"record key2\" : \"record val2\" }] }"
                + "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\" }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out2.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "dict a", "dict b" }
            };

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(40000);

            MatrixRecorder.Record("record key2", "record val2");

            // 1st doc closes here.

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_delayed_2_open_docs_without_close_between()
        {
            // FIXME: version-id will fail in the future
            string expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key2\" : \"record val2\" }, { \"dict a\" : \"dict b\" }] }";

            string outpath = Path.Combine(Path.GetTempPath(), "out3.bson");

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "dict a", "dict b" }
            };

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key1", "record val1");

            Thread.Sleep(40000);

            MatrixRecorder.Open(outpath);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key2", "record val2");
            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public static void BSON_MatrixRecorder_2_open_close_docs()
        {
            // FIXME: version-id will fail in the future
            string expectedJsonStr
                = "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"h\" : 22, \"row0\" : 1 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key1\" : \"record val1\" }] }"
                + "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 2 }"
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
            // FIXME: version-id will fail in the future
            var expectedJsonStr =
                "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"row0\" : 1 }"
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
            // FIXME: version-id will fail in the future
            var expectedJsonStr =
                "{ \"header\" : { \"file_version\" : \"0.0.0\", \"vecto_version\" : \"0.7.3.2171\", \"name\" : \"matrixrecorder\", \"CycleName\" : \"\", \"CycleType\" : 3, \"row0\" : 1 }"
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

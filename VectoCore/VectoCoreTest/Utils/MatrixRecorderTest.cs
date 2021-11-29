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
        private const int store_interval_sec = 1;
        private const int store_sleep_factor = 1200;
        private string MyTempDir;

        [OneTimeSetUp]
        public void Init()
        {
            Environment.SetEnvironmentVariable("MATREC_STORE_INTERVAL_SEC", $"{store_interval_sec}");
            MyTempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(MyTempDir);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Directory.Delete(MyTempDir, true);
        }

        private void CheckOutput(string outpath, string expectedJsonStr)
        {
            expectedJsonStr = expectedJsonStr.Replace("@@", MatRec.VectoVersionId);

            using (FileStream fs = new FileStream(
                outpath, FileMode.Open, FileAccess.Read, FileShare.Read
            ))
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

        // TODO: Cetegorize TCs.

        [TestCase]
        public void MatrixRecorder_Basic()
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

            MatRec.Open(outpath);
            MatRec.NewRow(dict);
            MatRec.Record("record key 1", "record val 1");
            MatRec.NewRow();
            MatRec.Record(dict);
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public void MatrixRecorder_delayed()
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

            Thread.Sleep(store_interval_sec * store_sleep_factor);

            MatRec.Record("A3", "V3");  // Appended in pending row & not yet stored. 

            // 1st doc closes here.

            MatRec.NewRow(dict);
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public void MatrixRecorder_close_and_reopen_overwrites()
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
            var header= new Dictionary<string, object> { { "HK", 3.14} };

            MatrixRecorder.Open(outpath);

            MatRec.Open(outpath);
            MatRec.NewRow("b", "bb");
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr[1]);
        }

        [TestCase]
        public void MatrixRecorder_delayed_reopen_overwrites()
        {
            string outpath = Path.Combine(MyTempDir, "out3d.bson");
            string expectedJsonStr
                = "{ \"header\" : { \"matrec_ver\" : \"0.0.0\", \"vecto_ver\" : \"@@\", \"row0\" : 0 }"
                + ", \"rows\" : [{ \"b\" : \"bb\" }] }";

            MatrixRecorder.Open(outpath);

            Thread.Sleep(store_interval_sec * store_sleep_factor);

            MatRec.Open(outpath);
            Assert.False(File.Exists(outpath), $"File({outpath}) should not have been written!");

            MatRec.NewRow("b", "bb");
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public void MatrixRecorder_forgot_Open()
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

        [TestCase]
        public void MatrixRecorder_forgot_NewRow()
        {
            string outpath = Path.Combine(MyTempDir, "out3cb.bson");
            var d = new Dictionary<string, object>();

            MatRec.Open(outpath);
            Assert.Throws<InvalidOperationException>(() => MatRec.Record("Forgotten", "NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record(d));
            MatRec.Close();

            Assert.Throws<InvalidOperationException>(() => MatRec.Record("Forgotten", "NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record(d));

            MatRec.Open(outpath);
            Assert.Throws<InvalidOperationException>(() => MatRec.Record("Forgotten", "NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record(d));
            MatRec.Close();
        }

        [TestCase]
        public void MatrixRecorder_null_header()
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
        public void MatrixRecorder_custom_header()
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
            var headerDict = new Dictionary<string, object>()
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

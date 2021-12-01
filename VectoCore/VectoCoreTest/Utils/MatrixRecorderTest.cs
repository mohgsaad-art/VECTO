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
        private string MyTempDir;

        [OneTimeSetUp]
        public void Init()
        {
            Environment.SetEnvironmentVariable("DocStoreInterval", $"{store_delay_sec}");
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
            string outpath = Path.Combine(MyTempDir, "out1.bson");
            var expectedJsonStr
                = "{ \"header\" : { \"matrec_ver\" : \"0.0.0\", \"vecto_ver\" : \"@@\", \"row0\" : 0 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"record key 1\" : \"record val 1\" }, { \"dict a\" : \"dict b\" }] }";
            var dict = new Dictionary<string, object>()
            {
                { "dict a", "dict b" }
            };

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
            string outpath = Path.Combine(MyTempDir, "out2.bson");
            var expectedJsonStr
                = "{ \"header\" : { \"matrec_ver\" : \"0.0.0\", \"vecto_ver\" : \"@@\", \"row0\" : 0 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\", \"A1\" : \"V1\" }, { \"A2\" : \"V2\", \"A3\" : \"V3\" }] }"
                + "{ \"header\" : { \"row0\" : 2 }"
                + ", \"rows\" : [{ \"dict a\" : \"dict b\" }] }";
            var dict = new Dictionary<string, object>
            {
                { "dict a", "dict b" }
            };

            MatRec.Open(outpath);
            MatRec.NewRow(dict);
            MatRec.Record("A1", "V1");
            MatRec.NewRow("A2", "V2");

            Thread.Sleep(store_delay_sec * 1500);

            MatRec.Record("A3", "V3");  // Appended in pending row & not yet stored. 

            // 1st doc closes here.

            MatRec.NewRow(dict);
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public void MatrixRecorder_close_and_reopen_overwrites()
        {
            string outpath = Path.Combine(MyTempDir, "out3b.bson");
            string[] expectedJsonStr = {
                "{ \"header\" : { \"matrec_ver\" : \"0.0.0\", \"vecto_ver\" : \"@@\""
                + ", \"HK\" : 3.1400000000000001, \"row0\" : 0 }"
                + ", \"rows\" : [{ \"a\" : \"aa\" }] }",
                "{ \"header\" : { \"matrec_ver\" : \"0.0.0\", \"vecto_ver\" : \"@@\", \"row0\" : 0 }"  // row0 restarted
                + ", \"rows\" : [{ \"b\" : \"bb\" }] }"
            };
            var header= new Dictionary<string, object> { { "HK", 3.14} };

            MatRec.Open(outpath, header);
            MatRec.NewRow("a", "aa");
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr[0]);

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

            MatRec.Open(outpath);
            MatRec.NewRow("a", "aa");

            Thread.Sleep(store_delay_sec * 1500);

            MatRec.Open(outpath);
            Assert.False(File.Exists(outpath), $"File({outpath}) should not have been written!");

            MatRec.NewRow("b", "bb");
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public void MatrixRecorder_forgot_Open()
        {
            string outpath = Path.Combine(MyTempDir, "out3ca.bson");
            var d = new Dictionary<string, object>();

            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow("Forgotten", "Open+NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record("Forgotten", "Open+NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow("Forgotten", "NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.Close());
            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow("Forgotten", "NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record("Forgotten", "Open+NewRow()"));

            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow(d));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record(d));
            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow(d));
            Assert.Throws<InvalidOperationException>(() => MatRec.Close());
            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow(d));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record(d));

            MatRec.Open(outpath);
            MatRec.Close();

            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow("Forgotten", "Open+NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record("Forgotten", "Open+NewRow()"));
            Assert.Throws<InvalidOperationException>(() => MatRec.NewRow(d));
            Assert.Throws<InvalidOperationException>(() => MatRec.Record(d));
            Assert.Throws<InvalidOperationException>(() => MatRec.Close());
        }

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
            string outpath = Path.Combine(MyTempDir, "out5.bson");
            var expectedJsonStr =
                "{ \"header\" : { \"matrec_ver\" : \"0.0.0\", \"vecto_ver\" : \"@@\", \"row0\" : 0 }"
                + ", \"rows\" : [{ }] }";

            MatRec.Open(outpath, null);
            MatRec.NewRow();
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr);
        }

        [TestCase]
        public void MatrixRecorder_custom_header()
        {
            string outpath = Path.Combine(MyTempDir, "out6.bson");
            var expectedJsonStr =
                "{ \"header\" : { \"matrec_ver\" : \"0.0.0\", \"vecto_ver\" : \"@@\", \"CycleName\" : \"\", \"CycleType\" : 3, \"row0\" : 0 }"
                + ", \"rows\" : [{ }] }";
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

            MatRec.Open(outpath, headerDict);
            MatRec.NewRow();
            MatRec.Close();

            CheckOutput(outpath, expectedJsonStr);
        }
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace MatrixRecorder
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MatrixRecorderTests
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
        }

        [TestCase]
        public static void BSON_MatrixRecorder()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("dict a", "dict b");

            Dictionary<string, object> headerDict = new Dictionary<string, object>();

            MatrixRecorder.Open(Path.Combine(Path.GetTempPath(), "out.bson"), headerDict);

            MatrixRecorder.NewRow(dict);

            MatrixRecorder.Record("record key", "record val");

            MatrixRecorder.Close();
            /*
             * {{ "header" : { "file_version" : "0.0.0", "name" : "matrixrecorder", "row0" : 0 }, "rows" : [{ "dict a" : "dict b", "record key" : "record val" }] }}
             * 
             */
        }

    }
}

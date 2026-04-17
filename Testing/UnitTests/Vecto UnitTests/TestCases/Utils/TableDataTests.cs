using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils
{
    public class TableDataTests
    {
        static TableData _td1 = new TableData();
        static TableData _td1Equal = new TableData();
        static TableData _td2 = new TableData();

        static TableDataTests()
        {
            PopulateTableData(_td1, new[] {
                new[]{"0.0", "0.0", "0.0"},
                new[]{"0.0", "0.0", "0.0"},
                new[]{"0.0", "0.0", "0.0"}
            });

            PopulateTableData(_td1Equal, new[] {
                new[]{"0.0", "0.0", "0.0"},
                new[]{"0.0", "0.0", "0.0"},
                new[]{"0.0", "0.0", "0.0"}
            });

            PopulateTableData(_td2, new[] {
                new[]{"0.0", "0.0", "0.0"},
                new[]{"0.0", "0.0", "0.0"},
                new[]{"0.0", "0.0", "1.0"}
            });
        }

        static void PopulateTableData(TableData td, string[][] values)
        {
            foreach (var col in values.First())
            {
                td.Columns.Add(new DataColumn());
            }

            foreach (var row in values)
            {
                td.Rows.Add(td.NewRow().ItemArray = row);
            }
        }

        public static IEnumerable<object[]> EqualityTestCases()
        {
            yield return new object[] { null, null, true };
            yield return new object[] { null, _td1, false };
            yield return new object[] { _td1, null, false };
            yield return new object[] { _td1, _td1, true };
            yield return new object[] { _td1, _td1Equal, true };
            yield return new object[] { _td2, _td1, false };
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_TableData_OperatorNotEqual(TableData a, TableData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_TableData_OperatorEqual(TableData a, TableData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_TableData_Equals(TableData a, TableData b, bool expected)
        {
            if ((a is null) || (b is null))
            {
                return;
            }

            var result = a.Equals(b);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}

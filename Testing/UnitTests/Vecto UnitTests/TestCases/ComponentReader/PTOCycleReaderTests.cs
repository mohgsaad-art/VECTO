using NUnit.Framework;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class PTOCycleReaderTests
{
	[TestCase()]
	public void TestReadingPTOCycleDuringDrive()
	{
		var cyleTbl = InputDataHelper.InputDataAsTableData(PtoCycleHdr, PtoCycleData);

		var cycleData = DrivingCycleDataReader.ReadFromDataTable(cyleTbl, "PTO During Drive", false);

		Assert.AreEqual(7, cycleData.Entries.Count);
	}

	const string PtoCycleHdr = "t,PTO_Power";

	private static readonly string[] PtoCycleData = new[] {
		"# [s],[kW]",
		"0,20",
		"10,20",
		"12,25",
		"15,40",
		"25,20",
		"28,5",
		"30,0",
	};
}
using Moq;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class MeasuredSpeedDrivingCycleReaderTests
{
    /// <summary>
    /// Test if the cycle file can be read.
    /// </summary>
    /// <remarks>VECTO-181</remarks>
    [TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>", "0, 0, 0, 3.2018, 0, 0, 0.504", false, true)] // all data
    [TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>", "0, 0, 0, 3.2018, 0, 0          ", true, true)] // vair only
    [TestCase("<t>,<v>,<grad>,<Padd>", "0, 0, 0, 3.2018", false, true)]  // no aux, no vair
    [TestCase("<t>,<v>,<grad>,<Padd>,<Aux_Alt>", "0  ,0  ,0     ,3.2018,0.504", false, true)] // aux only
    public void MeasuredSpeed_ReadCycle(string hdr, string data, bool crosswindReq, bool autoCycle)
    {
        var inputData = $"{hdr}\n{data}";
        TestCycleRead(inputData, CycleType.MeasuredSpeed, autoCycle, crosswindReq);

    }

    [TestCase("<t>,<v>,<grad>,<Padd>", "0, 0, 0, 3.2018", true, true, "CYCLE_FAILED_TO_READ_STREAM")] // vair required, but not there: error
    [TestCase("<t>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>", "0, 0, 3.2018, 0, 0, 0.504", false, false, "DATATABLE_MISSING_COLUMN")] // missing columns
    [TestCase("<t>,<v>,<wrong>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>", "0, 0, 0, 3.2018, 0, 0, 0.504, 0", false, false, "DATATABLE_WRONG_COLUMNS")] // not allowed columns
	[TestCase("<t>,<v>,<wrong>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>", "0, 0, 0, 3.2018, 0, 0, 0.504, 0", false, true, "CYCLE_UNSUPPORTED_TYPE")] // auto find cycle
    [TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>", "0, 0", false, true, "DATATABLE_WRONG_COLUMNS")] // wrong data

    public void MeasuredSpeed_ReadCycle_Error(string hdr, string data, bool crosswindReq, bool autoCycle, string expectedErrorCodeTxt)
	{
		var inputData = $"{hdr}\n{data}";
		AssertHelper.Exception(
			() => TestCycleRead(inputData, CycleType.MeasuredSpeed, autoCycle, crosswindReq),
			new ErrorCode(expectedErrorCodeTxt));
	}

	/// <summary>
	/// Test if the cycle file can be read.
	/// </summary>
	/// <remarks>VECTO-181</remarks>
	[TestCase("<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<vair_res>,<vair_beta>,<Aux_Alt>", "0, 0, 0, 3.2018, 595.75, 0, 0, 0, 0.504", false, true)] // all data
    [TestCase("<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<vair_res>,<vair_beta>", "0, 0, 0, 3.2018, 595.75, 0, 0, 0          ", true, true)] // vair only
    [TestCase("<t>,<v>,<grad>,<Padd>,<n>   ,<gear>", "0, 0, 0, 3.2018, 595.75, 0     ", false, true)] // no aux, no vair
    [TestCase("<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<Aux_Alt>", "0, 0, 0, 3.2018, 595.75, 0, 0.504", false, true)] // aux only
    public void MeasuredSpeed_ReadCycle_Gear(string hdr, string data, bool crosswindReq, bool autoCycle)
	{
		var inputData = $"{hdr}\n{data}";
		
		TestCycleRead(inputData, CycleType.MeasuredSpeedGear, autoCycle, crosswindReq);
        
	}

    [TestCase("<t>,<v>,<grad>,<Padd>,<n>   ,<gear>", "0, 0, 0, 3.2018, 595.75, 0", true, true, "CYCLE_FAILED_TO_READ_STREAM")] // vair required, but not there: error
    [TestCase("<t>,<grad>,<Padd>,<n>,<gear>", "0, 0, 3.2018, 595.75, 0", false, false, "DATATABLE_MISSING_COLUMN")] // missing columns
	[TestCase("<t>,<grad>,<Padd>,<n>,<gear>", "0, 0, 3.2018, 595.75, 0", false, true, "CYCLE_UNSUPPORTED_TYPE")] // missing columns
    [TestCase("<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<wrong>", "0, 0, 0, 3.2018, 595.75, 0, 0.504", false, false, "DATATABLE_WRONG_COLUMNS")] // not allowed columns
	[TestCase("<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<wrong>", "0, 0, 0, 3.2018, 595.75, 0, 0.504", false, true, "CYCLE_UNSUPPORTED_TYPE")] // not allowed columns
    [TestCase("<t>,<grad>,<Padd>,<n>,<gear>", "0, 0", false, true, "DATATABLE_WRONG_COLUMNS")]  // wrong data
    public void MeasuredSpeed_ReadCycle_Gear_Error(string hdr, string data, bool crosswindReq, bool autoCycle, string expectedErrorCodeTxt)
	{
		var inputData = $"{hdr}\n{data}";
		AssertHelper.Exception(
			() => TestCycleRead(inputData, CycleType.MeasuredSpeedGear, autoCycle, crosswindReq),
            new ErrorCode(expectedErrorCodeTxt));
	}

    private static void TestCycleRead(string inputData, CycleType cycleType, bool autoCycle = true,
        bool crossWindRequired = false)
    {
        var container = new Mock<IVehicleContainer>().Object;

        if (autoCycle) {
            var cycleTypeCalc = DrivingCycleDataReader.DetectCycleType(VectoCSVFile.ReadStream(inputData.ToStream()));
            Assert.AreEqual(cycleType, cycleTypeCalc);
        }
        var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.ToStream(), cycleType, "", crossWindRequired);
        Assert.AreEqual(cycleType, drivingCycle.CycleType);

        var cycle = new MeasuredSpeedDrivingCycle(container, drivingCycle);
    }
}
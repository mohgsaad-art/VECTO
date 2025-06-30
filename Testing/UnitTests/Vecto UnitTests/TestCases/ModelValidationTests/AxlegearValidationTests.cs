using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class AxlegearValidationTests
{
	[TestCase]
	public void AxleGearValidRangeTest()
	{
		var vehicle = new Mock<IVehicleContainer>().Object;
		var inputData = GetAxlegearInputData(true);
		var axleGearData = new AxleGearDataAdapter().CreateAxleGearData(inputData);
		var axleGear = new AxleGear(vehicle, axleGearData);
		Assert.AreEqual(0, axleGear.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false).Count);
	}

	[TestCase]
	public void AxleGearInvalidRangeTest()
	{
		var vehicle = new Mock<IVehicleContainer>().Object;
        var inputData = GetAxlegearInputData(false);
		var axleGearData = new AxleGearDataAdapter().CreateAxleGearData(inputData);
        var axleGear = new AxleGear(vehicle, axleGearData);
		var errors = axleGear.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		Assert.AreEqual(1, errors.Count);
	}

	protected static string AxlMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";
	protected static readonly string[] AxlMapData = new[] {
		"0,0,0",
		"0,-100000,100",
		"0,100000,100",
		"5000,-100000,100",
		"5000,100000,100",
	};

	protected static readonly string[] AxlMapDataInvalid = new[] {
		"0,0,0",
		"-1,-100000,100",
		"-1,100000,100",
		"0,-100001,100",
		"0,100001,100",
		"5001,-100000,100",
		"5001,100000,100",
	};

	protected IAxleGearInputData GetAxlegearInputData(bool validMap)
	{
		var mapData = validMap ? AxlMapData : AxlMapDataInvalid;

        var axl = new Mock<IAxleGearInputData>();
		axl.Setup(a => a.LineType).Returns(AxleLineType.SinglePortalAxle);
		axl.Setup(a => a.Ratio).Returns(3.240355);
		axl.Setup(a => a.LossMap)
			.Returns(InputDataHelper.InputDataAsTableData(AxlMapHdr, mapData));
		return axl.Object;
	}
}
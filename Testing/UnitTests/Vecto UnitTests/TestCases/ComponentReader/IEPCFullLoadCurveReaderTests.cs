using NUnit.Framework;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class IEPCFullLoadCurveReaderTests
{
	[TestCase(1850f, 1f)]
	public void IEPCFldReaderTest(double expectedSpeedRpm, double ratio)
	{
		var fldCruveData = InputDataHelper.InputDataAsTableData(IEPCFldHdr, IEPCFldData);
		var fld = IEPCFullLoadCurveReader.Create(fldCruveData, 1, ratio);

		Assert.AreEqual(expectedSpeedRpm * ratio, ElectricMotorRatedSpeedHelper.GetRatedSpeed(fld.FullLoadEntries, entry => entry.MotorSpeed,
			entry => entry.FullDriveTorque).AsRPM);
	}

	const string IEPCFldHdr = "n [rpm] , T_drive [Nm] , T_drag [Nm]";

	private static readonly string[] IEPCFldData = new[] {
		"0, 1030.00, -1030.00",
		"37, 1030.00, -1030.00",
		"370, 1030.00, -1030.00",
		"740, 1030.00, -1030.00",
		"1110, 1030.00, -1030.00",
		"1480, 1030.00, -1030.00",
		"1850, 1030.00, -1030.00",
		"2220, 858.33, -858.33",
		"2590, 735.71, -735.71",
	};
}
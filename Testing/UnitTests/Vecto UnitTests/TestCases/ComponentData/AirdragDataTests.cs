using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class AirdragDataTests
{
	[TestCase]
	public void VcdbTest()
	{
		var vairbeta = new CrosswindCorrectionVAirBeta(5.SI<SquareMeter>(),
			CrossWindCorrectionCurveReader.ReadCdxABetaTable(InputDataHelper.InputDataAsTableData(VAirHdr, VAirData)));
		
		var cycleEntry = new DrivingCycleData.DrivingCycleEntry() {
			AirSpeedRelativeToVehicle = 20.KMPHtoMeterPerSecond(),
			WindYawAngle = 0
		};
		
		var pAvg =
			vairbeta.AverageAirDragPowerLoss(cycleEntry, 20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond(), Physics.AirDensity).PowerLoss.Value();
		Assert.AreEqual(509.259, pAvg, 1e-3);

		pAvg =
			vairbeta.AverageAirDragPowerLoss(cycleEntry, 20.KMPHtoMeterPerSecond(), 21.KMPHtoMeterPerSecond(), Physics.AirDensity).PowerLoss.Value();
		Assert.AreEqual(521.990, pAvg, 1e-3);

		pAvg =
			vairbeta.AverageAirDragPowerLoss(cycleEntry, 20.KMPHtoMeterPerSecond(), 30.KMPHtoMeterPerSecond(), Physics.AirDensity).PowerLoss.Value();
		Assert.AreEqual(636.574, pAvg, 1e-3);

		cycleEntry.WindYawAngle = 20;

		pAvg =
			vairbeta.AverageAirDragPowerLoss(cycleEntry, 20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond(), Physics.AirDensity).PowerLoss.Value();
		Assert.AreEqual(829.074, pAvg, 1e-3);

		pAvg =
			vairbeta.AverageAirDragPowerLoss(cycleEntry, 20.KMPHtoMeterPerSecond(), 30.KMPHtoMeterPerSecond(), Physics.AirDensity).PowerLoss.Value();
		Assert.AreEqual(1036.343, pAvg, 1e-3);

		cycleEntry.WindYawAngle = -120;

		pAvg =
			vairbeta.AverageAirDragPowerLoss(cycleEntry, 20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond(), Physics.AirDensity).PowerLoss.Value();
		Assert.AreEqual(-1019.5370, pAvg, 1e-3);
	}

	const string VAirHdr = "beta [°],delta CdA [m²]";

	private static string[] VAirData = new[] {
		"0,0.00",
		"1,0.07",
		"2,0.21",
		"3,0.40",
		"4,0.64",
		"5,0.90",
		"6,1.19",
		"7,1.48",
		"8,1.76",
		"9,2.02",
		"10,2.25",
		"11,2.43",
		"12,2.56",
		"13,2.61",
		"14,2.69",
		"15,2.84",
		"20,3.14",
		"25,3.44",
		"30,3.74",
		"35,3.54",
		"40,3.24",
		"45,2.24",
		"50,1.24",
		"55,0.24",
		"60,-0.76",
		"65,-1.76",
		"70,-2.76",
		"75,-3.76",
		"80,-4.76",
		"85,-5.76",
		"90,-6.46",
		"95,-7.51",
		"100,-9.01",
		"105,-10.51",
		"110,-12.01",
		"115,-13.51",
		"120,-15.01",
		"125,-16.51",
		"130,-18.01",
		"135,-19.51",
		"140,-21.01",
		"145,-21.46",
		"150,-21.76",
		"155,-21.31",
		"160,-20.86",
		"165,-20.41",
		"170,-19.53",
		"175,-17.51",
		"180,-16.15",
	};
}
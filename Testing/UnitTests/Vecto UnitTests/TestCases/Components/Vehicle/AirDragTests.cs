using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Vehicle;

public class AirDragTests
{

	public static readonly double Tolerance = 0.001;

    [Test,
	TestCase(5.19, 0, 1.173),
	TestCase(5.19, 40, 1.173),
	TestCase(5.19, 60, 1.173),
	TestCase(5.19, 80, 1.109),
	TestCase(5.19, 100, 1.075),
	TestCase(5.19, 62.5, 1.163),
	]
	public void VehicleAirDragSpeedDependentTest(double crossSectionArea, double velocity, double expectedFactor)
	{
		var header = "v_veh in km/h,Cd factor in -";
        var data = new[] {
			"0,1.173 ",
			"5,1.173 ",
			"10,1.173",
			"15,1.173",
			"20,1.173",
			"25,1.173",
			"30,1.173",
			"35,1.173",
			"40,1.173",
			"45,1.173",
			"50,1.173",
			"55,1.173",
			"60,1.173",
			"65,1.153",
			"70,1.136",
			"75,1.121",
			"80,1.109",
			"85,1.099",
			"90,1.090",
			"95,1.082",
			"100,1.075"
		};
		

		var cwcc =
			new CrosswindCorrectionCdxALookup(crossSectionArea.SI<SquareMeter>(), 0.SI<SquareMeter>(),
				CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurveFromStream(InputDataHelper.InputDataAsStream(header, data),
					crossSectionArea.SI<SquareMeter>()), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);

		Assert.AreEqual(crossSectionArea * expectedFactor,
			cwcc.EffectiveAirDragArea(velocity.KMPHtoMeterPerSecond()).Value(),
			Tolerance);
	}
}
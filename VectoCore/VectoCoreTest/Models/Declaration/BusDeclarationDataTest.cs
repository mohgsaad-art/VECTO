using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class BusDeclarationDataTest
{

	protected CompletedBusSegments segments;

	[OneTimeSetUp]
	public void RunBeforeTest()
	{
		segments = new CompletedBusSegments();
	}

	[TestCase(2, false, VehicleCode.CE, RegistrationClass.I, false, null, null, VehicleClass.Class31a, 0.0),
	TestCase(2, false, VehicleCode.CE, RegistrationClass.I, true, null, null, VehicleClass.Class31b1, 1.0),
	TestCase(2, false, VehicleCode.CE, RegistrationClass.II, true, null, null, VehicleClass.Class31b2, 1.0),
	TestCase(2, false, VehicleCode.CF, RegistrationClass.I, null, null, null, VehicleClass.Class31c, 0.0),
	TestCase(2, false, VehicleCode.CI, RegistrationClass.I, null, null, null, VehicleClass.Class31d, 0.0),
	TestCase(2, false, VehicleCode.CJ, RegistrationClass.I, null, null, null, VehicleClass.Class31e, 0.0),
	TestCase(2, false, VehicleCode.CA, RegistrationClass.II, null, null, null, VehicleClass.Class32a, 0.0),
	TestCase(2, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class32b, 0.0),
	TestCase(2, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class32c, 0.0),
	TestCase(2, false, VehicleCode.CA, RegistrationClass.III, null, null, null, VehicleClass.Class32d, 0.0),
	TestCase(2, false, VehicleCode.CB, RegistrationClass.II, null, 5, null, VehicleClass.Class32e, 0.0),
	TestCase(2, false, VehicleCode.CB, RegistrationClass.II, null, 10, null, VehicleClass.Class32f, 0.0),
	TestCase(3, false, VehicleCode.CE, RegistrationClass.I, false, null, null, VehicleClass.Class33a, 0.0),
	TestCase(3, false, VehicleCode.CE, RegistrationClass.I, true, null, null, VehicleClass.Class33b1, 1.25),
	TestCase(3, false, VehicleCode.CE, RegistrationClass.II, true, null, null, VehicleClass.Class33b2, 1.25),
	TestCase(3, false, VehicleCode.CF, RegistrationClass.I, null, null, null, VehicleClass.Class33c, 0.0),
	TestCase(3, false, VehicleCode.CI, RegistrationClass.I, null, null, null, VehicleClass.Class33d, 0.0),
	TestCase(3, false, VehicleCode.CJ, RegistrationClass.I, null, null, null, VehicleClass.Class33e, 0.0),
	TestCase(3, false, VehicleCode.CA, RegistrationClass.II, null, null, null, VehicleClass.Class34a, 0.0),
	TestCase(3, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class34b, 0.0),
	TestCase(3, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class34c, 0.0),
	TestCase(3, false, VehicleCode.CA, RegistrationClass.III, null, null, null, VehicleClass.Class34d, 0.0),
	TestCase(3, false, VehicleCode.CB, RegistrationClass.II, null, 5, null, VehicleClass.Class34e, 0.0),
	TestCase(3, false, VehicleCode.CB, RegistrationClass.II, null, 10, null, VehicleClass.Class34f, 0.0),
	TestCase(3, true, VehicleCode.CG, RegistrationClass.I, false, null, null, VehicleClass.Class35a, 0.0),
	TestCase(3, true, VehicleCode.CG, RegistrationClass.I, true, null, null, VehicleClass.Class35b1, 1.0),
	TestCase(3, true, VehicleCode.CG, RegistrationClass.II, true, null, null, VehicleClass.Class35b2, 1.0),
	TestCase(3, true, VehicleCode.CH, RegistrationClass.I, null, null, null, VehicleClass.Class35c, 0.0),
	TestCase(3, true, VehicleCode.CC, RegistrationClass.II, null, null, null, VehicleClass.Class36a, 0.0),
	TestCase(3, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class36b, 0.0),
	TestCase(3, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class36c, 0.0),
	TestCase(3, true, VehicleCode.CC, RegistrationClass.III, null, null, null, VehicleClass.Class36d, 0.0),
	TestCase(3, true, VehicleCode.CD, RegistrationClass.II, null, 5, null, VehicleClass.Class36e, 0.0),
	TestCase(3, true, VehicleCode.CD, RegistrationClass.II, null, 10, null, VehicleClass.Class36f, 0.0),
	TestCase(4, false, VehicleCode.CE, RegistrationClass.I, false, null, null, VehicleClass.Class37a, 0.0),
	TestCase(4, false, VehicleCode.CE, RegistrationClass.I, true, null, null, VehicleClass.Class37b1, 1.25),
	TestCase(4, false, VehicleCode.CE, RegistrationClass.II, true, null, null, VehicleClass.Class37b2, 1.25),
	TestCase(4, false, VehicleCode.CF, RegistrationClass.I, null, null, null, VehicleClass.Class37c, 0.0),
	TestCase(4, false, VehicleCode.CI, RegistrationClass.I, null, null, null, VehicleClass.Class37d, 0.0),
	TestCase(4, false, VehicleCode.CJ, RegistrationClass.I, null, null, null, VehicleClass.Class37e, 0.0),
	TestCase(4, false, VehicleCode.CA, RegistrationClass.II, null, null, null, VehicleClass.Class38a, 0.0),
	TestCase(4, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class38b, 0.0),
	TestCase(4, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class38c, 0.0),
	TestCase(4, false, VehicleCode.CA, RegistrationClass.III, null, null, null, VehicleClass.Class38d, 0.0),
	TestCase(4, false, VehicleCode.CB, RegistrationClass.II, null, 5, null, VehicleClass.Class38e, 0.0),
	TestCase(4, false, VehicleCode.CB, RegistrationClass.II, null, 10, null, VehicleClass.Class38f, 0.0),
	TestCase(4, true, VehicleCode.CG, RegistrationClass.I, false, null, null, VehicleClass.Class39a, 0.0),
	TestCase(4, true, VehicleCode.CG, RegistrationClass.I, true, null, null, VehicleClass.Class39b1, 1.25),
	TestCase(4, true, VehicleCode.CG, RegistrationClass.II, true, null, null, VehicleClass.Class39b2, 1.25),
	TestCase(4, true, VehicleCode.CH, RegistrationClass.I, null, null, null, VehicleClass.Class39c, 0.0),
	TestCase(4, true, VehicleCode.CC, RegistrationClass.II, null, null, null, VehicleClass.Class40a, 0.0),
	TestCase(4, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class40b, 0.0),
	TestCase(4, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class40c, 0.0),
	TestCase(4, true, VehicleCode.CC, RegistrationClass.III, null, null, null, VehicleClass.Class40d, 0.0),
	TestCase(4, true, VehicleCode.CD, RegistrationClass.II, null, 5, null, VehicleClass.Class40e, 0.0),
	TestCase(4, true, VehicleCode.CD, RegistrationClass.II, null, 10, null, VehicleClass.Class40f, 0.0),
		Category(Definitions.TESTCASE_MIGRATED)
	]
	public void TestDrivetrainCorrectionLengthDrivetrain(int numAxles, bool articulated, VehicleCode vc, RegistrationClass regCode,  bool? lowEntry, int? passCntLow, double? height,
		VehicleClass expecteClass, double expectedLength)
	{
		var semgent = segments.Lookup(numAxles, vc, regCode, passCntLow, height?.SI<Meter>(), lowEntry);
		Assert.AreEqual(semgent.VehicleClass, expecteClass);

		var len = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(vc, lowEntry, numAxles, articulated);
		Assert.AreEqual(expectedLength, len.Value());
	}


	[
	TestCase(HeatPumpType.R_744, HeaterType.None, false, HeatingDistributionCase.HeatingDistribution1),
	TestCase(HeatPumpType.R_744, HeaterType.WaterElectricHeater, false, HeatingDistributionCase.HeatingDistribution2),
	TestCase(HeatPumpType.R_744, HeaterType.AirElectricHeater, true, HeatingDistributionCase.HeatingDistribution3),
	TestCase(HeatPumpType.R_744, HeaterType.None, true, HeatingDistributionCase.HeatingDistribution4),
	TestCase(HeatPumpType.non_R_744_2_stage, HeaterType.None, false, HeatingDistributionCase.HeatingDistribution5),
	TestCase(HeatPumpType.non_R_744_3_stage, HeaterType.WaterElectricHeater, false, HeatingDistributionCase.HeatingDistribution6),
	TestCase(HeatPumpType.non_R_744_4_stage, HeaterType.AirElectricHeater, true, HeatingDistributionCase.HeatingDistribution7),
	TestCase(HeatPumpType.non_R_744_2_stage, HeaterType.None, true, HeatingDistributionCase.HeatingDistribution8),
	TestCase(HeatPumpType.none, HeaterType.OtherElectricHeating, false, HeatingDistributionCase.HeatingDistribution9),
	TestCase(HeatPumpType.none, HeaterType.WaterElectricHeater, true, HeatingDistributionCase.HeatingDistribution10),
	TestCase(HeatPumpType.none, HeaterType.None, true, HeatingDistributionCase.HeatingDistribution11),
	TestCase(HeatPumpType.none, HeaterType.None, false, HeatingDistributionCase.HeatingDistribution12),
	Category(Definitions.TESTCASE_MIGRATED)
	]
	public void TestBusAuxHeatingDistributionCase(HeatPumpType heatPump, HeaterType heater, bool fuelHeater,
		HeatingDistributionCase expectedCase)
	{
		var heatingCase =
			DeclarationData.BusAuxiliaries.HeatingDistributionCases.GetHeatingDistributionCase(heatPump, heater,
				fuelHeater);
		Assert.AreEqual(expectedCase, heatingCase);
	}


	[
	TestCase(HeatingDistributionCase.HeatingDistribution1, HeatPumpType.R_744, HeaterType.None, false),
	TestCase(HeatingDistributionCase.HeatingDistribution2, HeatPumpType.R_744, HeaterType.WaterElectricHeater, false),
	TestCase(HeatingDistributionCase.HeatingDistribution3, HeatPumpType.R_744, HeaterType.WaterElectricHeater, true),
	TestCase(HeatingDistributionCase.HeatingDistribution4, HeatPumpType.R_744, HeaterType.None, true),
	TestCase(HeatingDistributionCase.HeatingDistribution5, HeatPumpType.non_R_744_2_stage, HeaterType.None, false),
	TestCase(HeatingDistributionCase.HeatingDistribution6, HeatPumpType.non_R_744_3_stage, HeaterType.WaterElectricHeater, false),
	TestCase(HeatingDistributionCase.HeatingDistribution7, HeatPumpType.non_R_744_4_stage, HeaterType.WaterElectricHeater, true),
	TestCase(HeatingDistributionCase.HeatingDistribution8, HeatPumpType.non_R_744_2_stage, HeaterType.None, true),
	TestCase(HeatingDistributionCase.HeatingDistribution9, HeatPumpType.none, HeaterType.WaterElectricHeater, false),
	TestCase(HeatingDistributionCase.HeatingDistribution10, HeatPumpType.none, HeaterType.WaterElectricHeater, true),
	TestCase(HeatingDistributionCase.HeatingDistribution11, HeatPumpType.none, HeaterType.None, true),
//	TestCase(HeatingDistributionCase.HeatingDistribution12, HeatPumpType.none, HeaterType.None, false),
	Category(Definitions.TESTCASE_MIGRATED)
	]
	public void TestBusAuxHeatingDistribution(HeatingDistributionCase hdCase, HeatPumpType hpType, HeaterType heater, bool fuelHeater)
	{
		for (var environmentalId = 1; environmentalId <= 11; environmentalId++) {
			var entry = DeclarationData.BusAuxiliaries.HeatingDistribution.Lookup(hdCase, environmentalId);
			Assert.IsNotNull(entry);
			var hp = entry.GetHeatpumpContribution(hpType);
			var elHeater = entry.GetElectricHeaterContribution(heater);
			var fuel = entry.GetFuelHeaterContribution();
			if (hdCase == HeatingDistributionCase.HeatingDistribution12) {
				// in case 12 no heaters are present!
				Assert.AreEqual(0, hp + elHeater + fuel);
			} else {
				Assert.AreEqual(1, hp + elHeater + fuel,
					$"contributions do not sum up to 1 for configuration {hdCase}, {environmentalId}");
			}
		}
	}

	[Test,
	Category(Definitions.TESTCASE_MIGRATED)]
	public void TestBusAusHeatingDistribution_ALL()
	{
		var heatpumps = EnumHelper.GetValues<HeatPumpType>().Where(x => !x.IsOneOf(HeatPumpType.not_applicable))
			.ToList();
		var electricHeater = EnumHelper.GetValues<HeaterType>().Where(x => x != HeaterType.FuelHeater).ToList();
		var fuelHeater = new[] { true, false };

		foreach (var heatpump in heatpumps) {
			foreach (var heater in electricHeater) {
				foreach (var auxHeater in fuelHeater) {
					var heatingCase =
						DeclarationData.BusAuxiliaries.HeatingDistributionCases.GetHeatingDistributionCase(heatpump,
							heater, auxHeater);

					for (var envId = 1; envId <= 11; envId++) {
						var entry = DeclarationData.BusAuxiliaries.HeatingDistribution.Lookup(heatingCase, envId);
						var hp = entry.GetHeatpumpContribution(heatpump);
						var elHeater = entry.GetElectricHeaterContribution(heater);
						var fuel = entry.GetFuelHeaterContribution();
						if (heatingCase == HeatingDistributionCase.HeatingDistribution12) {
							// in case 12 no heaters are present!
							Assert.AreEqual(0, hp + elHeater + fuel);
						} else {
							Assert.AreEqual(1, hp + elHeater + fuel,
								$"contributions do not sum up to 1 for configuration. {heatpump}, {heater}, {auxHeater} {heatingCase}, {envId}");
						}
					}
				}
			}
		}

	}
}
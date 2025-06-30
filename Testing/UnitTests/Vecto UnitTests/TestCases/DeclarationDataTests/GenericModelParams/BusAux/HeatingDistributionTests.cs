using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.BusAux;

public class HeatingDistributionTests
{
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

    [Test]
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
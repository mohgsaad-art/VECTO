using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockInputData;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.OutputData.XML;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing;

public class BusAuxElectricCompressorPostProcessingTests
{
	private const OvcHevMode CD_Mode = OvcHevMode.ChargeDepleting;
	private const OvcHevMode CS_Mode = OvcHevMode.ChargeSustaining;
	private const OvcHevMode NonOvc = OvcHevMode.NotApplicable;

	private const VectoSimulationJobType P_HEV = VectoSimulationJobType.ParallelHybridVehicle;
	private const VectoSimulationJobType S_HEV = VectoSimulationJobType.SerialHybridVehicle;

	private const AlternatorType NoAlternator = AlternatorType.None;
	private const AlternatorType ConvAlternator = AlternatorType.Conventional;
	private const AlternatorType SmartAlternator = AlternatorType.Smart;

	private const double NaN = double.NaN;

	private IKernel _kernel;

	private const string FuelMap = "engine speed, torque, fuel consumption\n" +
									"500,-31,0\n" +
									"500,0,500\n" +
									"500,1000,24000\n" +
									"2500,-109,0\n" +
									"2500,0,3800\n" +
									"2500,1000,32000\n";

	protected static readonly IFuelProperties Fuel = FuelData.Diesel;

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}


    [
            TestCase(NonOvc, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, false, TestName = "Test Busaux ElectricPS Conventional correction conv alt Case A - 1 no correction"),
            TestCase(NonOvc, ConvAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 0.0000000, -0.0276873, 0.0000000, NaN, false, TestName = "Test Busaux ElectricPS Conventional correction, conv alt, Case A - 2 correction under-consumption"),
            TestCase(NonOvc, ConvAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 0.0000000, 0.0200357, 0.0000000, NaN, false, TestName = "Test Busaux ElectricPS Conventional correction, conv alt, Case A - 3 correction over-consumption"),

            TestCase(NonOvc, SmartAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, false, TestName = "Test Busaux ElectricPS Conventional correction, smart alt, Case B - 1 no correction"),
            TestCase(NonOvc, SmartAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 0.0000000, -0.0276873, 0.0000000, NaN, false, TestName = "Test Busaux ElectricPS Conventional correction, smart alt, Case B - 2 correction under-consumption"),
            TestCase(NonOvc, SmartAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 0.0000000, 0.0200357, 0.0000000, NaN, false, TestName = "Test Busaux ElectricPS Conventional correction, smart alt, Case B - 3 correction over-consumption"),
        ]
    public void TestBusAuxElectricCompressorPostProcessing_Conventional(
            OvcHevMode ovcMode,
            AlternatorType alternatorType,
            double cycleDuration,

            double expectedAirDemandCorr_Nl,
            double expectedDeltaAir_Nl,

            double expPSEnergyDemand_elCorr_kWh,
            double expPSEnergyDemand_SoCCorr_kWh,
            double expPSEnergyDemand_SocRange_kWh,

            double expElEnergyConsumption_kWh,

            double expDeltaFC_elPS_kg,
            double expDeltaFC_REESSSoC_kg,

            double expElectricRange_m,
            bool essSupplyHVREESS)
    {
        var assumedFuelConsumption = 32.SI<Kilogram>();

        var busAux = GetBusAuxParams(BusHVACSystemConfiguration.Configuration1, VectoSimulationJobType.ConventionalVehicle, alternatorType: alternatorType, essSupplyfromHVREESS: essSupplyHVREESS);
        var runData = GetVectoRunDataConventional(busAux);
        var mockModData = GetMockModData(runData, cycleDuration: cycleDuration.SI<Second>(), totalFuelConsumption: assumedFuelConsumption);

        var postProcessor = _kernel.Get<IModalDataPostProcessorFactory>()
            .GetPostProcessor(runData.JobType);

        var corrected = postProcessor.ApplyCorrection(mockModData, runData);

        Mock.Get(mockModData).Setup(x => x.CorrectedModalData).Returns(corrected);

        //var range = DeclarationData.CalculateElectricRangesPEV(runData, mockModData);

        var fc = corrected.FuelConsumptionCorrection(Fuel) as FuelConsumptionCorrection;
        Assert.NotNull(fc);

        Console.WriteLine($"{cycleDuration}, " +

                        $"{corrected.CorrectedAirDemand.Value():F4}, " +
                        $"{corrected.DeltaAir.Value():F4}, " +

                        $"{corrected.WorkBusAux_elPS_Corr_mech.ConvertToKiloWattHour().Value:F4}, " +
                        $"{corrected.WorkBusAux_elPS_SoC_Corr.ConvertToKiloWattHour().Value:F4}, " +
                        $"{corrected.WorkBusAux_elPS_SoC_ElRange.ConvertToKiloWattHour().Value:F7}, " +

                        $"{corrected.ElectricEnergyConsumption_SoC.ConvertToKiloWattHour().Value:F7}, " +

                        $"{fc.FcBusAuxElPS.Value():F7}, " +
                        $"{fc.FcREESSSoc.Value():F7}," +
                        $"NaN"
        );

        Assert.AreEqual(assumedFuelConsumption.Value(), fc.FcModSum.Value(), 1e-9);

        Assert.AreEqual(expectedAirDemandCorr_Nl, corrected.CorrectedAirDemand.Value(), 1e-3);
        Assert.AreEqual(expectedDeltaAir_Nl, corrected.DeltaAir.Value(), 1e-3);


        Assert.AreEqual(0, corrected.WorkBusAuxPSCorr.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_elCorr_kWh, corrected.WorkBusAux_elPS_Corr_mech.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_SoCCorr_kWh, corrected.WorkBusAux_elPS_SoC_Corr.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_SocRange_kWh, corrected.WorkBusAux_elPS_SoC_ElRange.ConvertToKiloWattHour().Value, 1e-6);

        Assert.AreEqual(expElEnergyConsumption_kWh, corrected.ElectricEnergyConsumption_SoC.ConvertToKiloWattHour().Value, 1e-6);
        Assert.AreEqual((expElEnergyConsumption_kWh + expPSEnergyDemand_SocRange_kWh), corrected.ElectricEnergyConsumption_SoC_Corr.ConvertToKiloWattHour().Value, 1e-6);

        Assert.AreEqual(expDeltaFC_elPS_kg, fc.FcBusAuxElPS.Value(), 1e-6);
        Assert.AreEqual(expDeltaFC_REESSSoC_kg, fc.FcREESSSoc.Value(), 1e-6);

        // no mechanical PS fuel consumption
        Assert.AreEqual(0, fc.FcBusAuxPs.Value());

        Assert.AreEqual(assumedFuelConsumption.Value() + expDeltaFC_elPS_kg + expDeltaFC_REESSSoC_kg, fc.TotalFuelConsumptionCorrected.Value(), 1e-6);

        //Assert.AreEqual(expElectricRange_m, range.ActualChargeDepletingRange.Value(), 1e-3);

    }



    [
        TestCase(CD_Mode, NoAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C1, no alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, NoAlternator, 6000, 4021.5180, -4499.1925, 0.0000, 0.0000, -1.2920758, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C1, no alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, NoAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.0000, 0.9350011, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C1, no alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, NoAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C1, no alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, NoAlternator, 6000, 4021.5180, -4499.1925, 0.0000, -1.2921, 0.0000000, 0.0000000, 0.0000000, -0.0221752, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C1, no alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, NoAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.9350, 0.0000000, 0.0000000, 0.0000000, 0.0160469, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C1, no alt, CS Mode - 3 correction over-consumption"),

        TestCase(CD_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2a, conv alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, 0.0000, 0.0000, -1.2920758, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2a, conv alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.0000, 0.9350011, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2a, conv alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2a, conv alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, 0.0000, -1.2921, 0.0000000, 0.0000000, 0.0000000, -0.0221752, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2a, conv alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.9350, 0.0000000, 0.0000000, 0.0000000, 0.0160469, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2a, conv alt, CS Mode - 3 correction over-consumption"),

        TestCase(CD_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2b, conv alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 180.0000000, -0.0276873, 0.0000000, 2000.5452, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2b, conv alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 180.0000000, 0.0200357, 0.0000000, 2000.5452, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2b, conv alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2b, conv alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 0.0000000, -0.0276873, 0.0000000, NaN, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2b, conv alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 0.0000000, 0.0200357, 0.0000000, NaN, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C2b, conv alt, CS Mode - 3 correction over-consumption"),

        TestCase(CD_Mode, SmartAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3a, smart alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, SmartAlternator, 6000, 4021.5180, -4499.1925, 0.0000, 0.0000, -1.2920758, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3a, smart alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, SmartAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.0000, 0.9350011, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3a, smart alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, SmartAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3a, smart alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, SmartAlternator, 6000, 4021.5180, -4499.1925, 0.0000, -1.2921, 0.0000000, 0.0000000, 0.0000000, -0.0221752, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3a, smart alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, SmartAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.9350, 0.0000000, 0.0000000, 0.0000000, 0.0160469, NaN, P_HEV, true, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3a, smart alt, CS Mode - 3 correction over-consumption"),

        TestCase(CD_Mode, SmartAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3b, smart alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, SmartAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 180.0000000, -0.0276873, 0.0000000, 2000.5452, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3b, smart alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, SmartAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 180.0000000, 0.0200357, 0.0000000, 2000.5452, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3b, smart alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, SmartAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3b, smart alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, SmartAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 0.0000000, -0.0276873, 0.0000000, NaN, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3b, smart alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, SmartAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 0.0000000, 0.0200357, 0.0000000, NaN, P_HEV, false, TestName = "Test Busaux ElectricPS P-HEV correction, Case C3b, smart alt, CS Mode - 3 correction over-consumption"),

        // ----
        TestCase(CD_Mode, NoAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C1, no alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, NoAlternator, 6000, 4021.5180, -4499.1925, 0.0000, 0.0000, -1.2920758, 180.0000000, 0.0000000, 0.0000000, 2000.5452, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C1, no alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, NoAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.0000, 0.9350011, 180.0000000, 0.0000000, 0.0000000, 2000.5452, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C1, no alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, NoAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C1, no alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, NoAlternator, 6000, 4021.5180, -4499.1925, 0.0000, -1.2921, 0.0000000, 0.0000000, 0.0000000, -0.2176128, NaN, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C1, no alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, NoAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.9350, 0.0000000, 0.0000000, 0.0000000, 0.1574739, NaN, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C1, no alt, CS Mode - 3 correction over-consumption"),

        TestCase(CD_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2a, conv alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, 0.0000, 0.0000, -1.2920758, 180.0000000, 0.0000000, 0.0000000, 2000.5452, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2a, conv alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.0000, 0.9350011, 180.0000000, 0.0000000, 0.0000000, 2000.5452, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2a, conv alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2a, conv alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, 0.0000, -1.2921, 0.0000000, 0.0000000, 0.0000000, -0.2176128, NaN, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2a, conv alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.9350, 0.0000000, 0.0000000, 0.0000000, 0.1574739, NaN, S_HEV, true, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2a, conv alt, CS Mode - 3 correction over-consumption"),

        TestCase(CD_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, 0.0000000, 0.0000000, 2000.5452, S_HEV, false, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2b, conv alt, CD Mode - 1 no correction"),
        TestCase(CD_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 180.0000000, -0.0276873, 0.0000000, 2000.5452, S_HEV, false, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2b, conv alt, CD Mode - 2 correction under-consumption"),
        TestCase(CD_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 180.0000000, 0.0200357, 0.0000000, 2000.5452, S_HEV, false, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2b, conv alt, CD Mode - 3 correction over-consumption"),

        TestCase(CS_Mode, ConvAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 0.0000000, 0.0000000, 0.0000000, NaN, S_HEV, false, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2b, conv alt, CS Mode - 1 no correction"),
        TestCase(CS_Mode, ConvAlternator, 6000, 4021.5180, -4499.1925, -1.8458, 0.0000, 0.0000000, 0.0000000, -0.0276873, 0.0000000, NaN, S_HEV, false, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2b, conv alt, CS Mode - 2 correction under-consumption"),
        TestCase(CS_Mode, ConvAlternator, 18000, 11776.5180, 3255.8075, 1.3357, 0.0000, 0.0000000, 0.0000000, 0.0200357, 0.0000000, NaN, S_HEV, false, TestName = "Test Busaux ElectricPS S-HEV correction, Case C2b, conv alt, CS Mode - 3 correction over-consumption"),

    ]
    public void TestBusAuxElectricCompressorPostProcessing_Hybrid(
        OvcHevMode ovcMode,
        AlternatorType alternatorType,
        double cycleDuration,

        double expectedAirDemandCorr_Nl,
        double expectedDeltaAir_Nl,

        double expPSEnergyDemand_elCorr_kWh,
        double expPSEnergyDemand_SoCCorr_kWh,
        double expPSEnergyDemand_SocRange_kWh,

        double expElEnergyConsumption_kWh,

        double expDeltaFC_elPS_kg,
        double expDeltaFC_REESSSoC_kg,

        double expElectricRange_m,
        VectoSimulationJobType jobType,
        bool essSupplyHVREESS
        )
    {
        var assumedFuelConsumption = 32.SI<Kilogram>();

        var busAux = GetBusAuxParams(BusHVACSystemConfiguration.Configuration1, jobType, alternatorType: alternatorType, essSupplyfromHVREESS: essSupplyHVREESS);
        var runData = GetVectoRunDataHybrid(busAux, jobType, ovcMode);
        var mockModData = GetMockModData(runData, cycleDuration: cycleDuration.SI<Second>(), totalFuelConsumption: assumedFuelConsumption);

        var postProcessor = _kernel.Get<IModalDataPostProcessorFactory>()
            .GetPostProcessor(runData.JobType);

        var corrected = postProcessor.ApplyCorrection(mockModData, runData);

        Mock.Get(mockModData).Setup(x => x.CorrectedModalData).Returns(corrected);

        //var range = DeclarationData.CalculateElectricRangesPEV(runData, mockModData);
        IWeightedResult weighted = null;
        if (ovcMode == OvcHevMode.ChargeDepleting) {
            var cdResult = new XMLDeclarationReport.ResultEntry();
            cdResult.Initialize(runData);
            cdResult.SetResultData(runData, mockModData, 1.0);
            var csResult = GetMockCSResult(runData);
            weighted = DeclarationData.CalculateWeightedResult(cdResult, csResult);
        }

        var fc = corrected.FuelConsumptionCorrection(Fuel) as FuelConsumptionCorrection;
        Assert.NotNull(fc);

        Console.WriteLine($"{cycleDuration}, " +

                        $"{corrected.CorrectedAirDemand.Value():F4}, " +
                        $"{corrected.DeltaAir.Value():F4}, " +

                        $"{corrected.WorkBusAux_elPS_Corr_mech.ConvertToKiloWattHour().Value:F4}, " +
                        $"{corrected.WorkBusAux_elPS_SoC_Corr.ConvertToKiloWattHour().Value:F4}, " +
                        $"{corrected.WorkBusAux_elPS_SoC_ElRange.ConvertToKiloWattHour().Value:F7}, " +

                        $"{corrected.ElectricEnergyConsumption_SoC.ConvertToKiloWattHour().Value:F7}, " +

                        $"{fc.FcBusAuxElPS.Value():F7}, " +
                        $"{fc.FcREESSSoc.Value():F7}," +
                        (weighted != null ? $"{weighted.ActualChargeDepletingRange.Value():F4}" : $"NaN")
        );

        Assert.AreEqual(assumedFuelConsumption.Value(), fc.FcModSum.Value(), 1e-9);

        Assert.AreEqual(expectedAirDemandCorr_Nl, corrected.CorrectedAirDemand.Value(), 1e-3);
        Assert.AreEqual(expectedDeltaAir_Nl, corrected.DeltaAir.Value(), 1e-3);


        Assert.AreEqual(0, corrected.WorkBusAuxPSCorr.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_elCorr_kWh, corrected.WorkBusAux_elPS_Corr_mech.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_SoCCorr_kWh, corrected.WorkBusAux_elPS_SoC_Corr.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_SocRange_kWh, corrected.WorkBusAux_elPS_SoC_ElRange.ConvertToKiloWattHour().Value, 1e-6);

        Assert.AreEqual(expElEnergyConsumption_kWh, corrected.ElectricEnergyConsumption_SoC.ConvertToKiloWattHour().Value, 1e-6);
        Assert.AreEqual((expElEnergyConsumption_kWh + expPSEnergyDemand_SocRange_kWh), corrected.ElectricEnergyConsumption_SoC_Corr.ConvertToKiloWattHour().Value, 1e-6);

        Assert.AreEqual(expDeltaFC_elPS_kg, fc.FcBusAuxElPS.Value(), 1e-6);
        Assert.AreEqual(expDeltaFC_REESSSoC_kg, fc.FcREESSSoc.Value(), 1e-6);

        // no mechanical PS fuel consumption
        Assert.AreEqual(0, fc.FcBusAuxPs.Value());

        Assert.AreEqual(assumedFuelConsumption.Value() + expDeltaFC_elPS_kg + expDeltaFC_REESSSoC_kg, fc.TotalFuelConsumptionCorrected.Value(), 1e-6);

        if (weighted != null) {
            Assert.AreEqual(expElectricRange_m, weighted.ActualChargeDepletingRange.Value(), 1e-3);
        }
        //Assert.AreEqual(expElectricRange_m, range.ActualChargeDepletingRange.Value(), 1e-3);

    }


    [
        TestCase(CD_Mode, NoAlternator, 12962, 8520.7105, 0.0000, 0.0000, 0.0000, 0.0000000, 180.0000000, NaN, NaN, 2006.6667, true, TestName = "Test Busaux ElectricPS PEV correction - 1 no correction"),
        TestCase(CD_Mode, NoAlternator, 6000, 4021.5180, -4499.1925, 0.0000, 0.0000, -1.2920758, 180.0000000, NaN, NaN, 2021.1751, true, TestName = "Test Busaux ElectricPS PEV correction - 2 correction under-consumption"),
        TestCase(CD_Mode, NoAlternator, 18000, 11776.5180, 3255.8075, 0.0000, 0.0000, 0.9350011, 180.0000000, NaN, NaN, 1996.2970, true, TestName = "Test Busaux ElectricPS PEV correction - 3 correction over-consumption"),
    ]
    public void TestBusAuxElectricCompressorPostProcessing_BatteryElectric(OvcHevMode ovcMode,
        AlternatorType alternatorType,
        double cycleDuration,

        double expectedAirDemandCorr_Nl,
        double expectedDeltaAir_Nl,

        double expPSEnergyDemand_elCorr_kWh,
        double expPSEnergyDemand_SoCCorr_kWh,
        double expPSEnergyDemand_SocRange_kWh,

        double expElEnergyConsumption_kWh,

        double expDeltaFC_elPS_kg,
        double expDeltaFC_REESSSoC_kg,

        double expElectricRange_m,
        bool essSupplyHVREESS)
    {
        //var assumedFuelConsumption = 32.SI<Kilogram>();

        var busAux = GetBusAuxParams(BusHVACSystemConfiguration.Configuration1, VectoSimulationJobType.BatteryElectricVehicle, alternatorType: alternatorType, essSupplyfromHVREESS: essSupplyHVREESS);
        var runData = GetVectoRunDataBatteryElectric(busAux);
        var mockModData = GetMockModData(runData, cycleDuration: cycleDuration.SI<Second>());

        var postProcessor = _kernel.Get<IModalDataPostProcessorFactory>()
            .GetPostProcessor(runData.JobType);

        var corrected = postProcessor.ApplyCorrection(mockModData, runData);

        Mock.Get(mockModData).Setup(x => x.CorrectedModalData).Returns(corrected);

        var range = DeclarationData.CalculateElectricRangesPEV(runData, mockModData);

        Console.WriteLine($"{cycleDuration}, " +

                        $"{corrected.CorrectedAirDemand.Value():F4}, " +
                        $"{corrected.DeltaAir.Value():F4}, " +

                        $"{corrected.WorkBusAux_elPS_Corr_mech.ConvertToKiloWattHour().Value:F4}, " +
                        $"{corrected.WorkBusAux_elPS_SoC_Corr.ConvertToKiloWattHour().Value:F4}, " +
                        $"{corrected.WorkBusAux_elPS_SoC_ElRange.ConvertToKiloWattHour().Value:F7}, " +

                        $"{corrected.ElectricEnergyConsumption_SoC.ConvertToKiloWattHour().Value:F7}, " +

                        $"NaN, " +
                        $"NaN," +
                        $"{range.ActualChargeDepletingRange.Value():F4}"
        );

        //Assert.AreEqual(assumedFuelConsumption.Value(), fc.FcModSum.Value(), 1e-9);

        Assert.AreEqual(expectedAirDemandCorr_Nl, corrected.CorrectedAirDemand.Value(), 1e-3);
        Assert.AreEqual(expectedDeltaAir_Nl, corrected.DeltaAir.Value(), 1e-3);


        Assert.AreEqual(0, corrected.WorkBusAuxPSCorr.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_elCorr_kWh, corrected.WorkBusAux_elPS_Corr_mech.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_SoCCorr_kWh, corrected.WorkBusAux_elPS_SoC_Corr.ConvertToKiloWattHour().Value, 1e-3);
        Assert.AreEqual(expPSEnergyDemand_SocRange_kWh, corrected.WorkBusAux_elPS_SoC_ElRange.ConvertToKiloWattHour().Value, 1e-6);

        Assert.AreEqual(expElEnergyConsumption_kWh, corrected.ElectricEnergyConsumption_SoC.ConvertToKiloWattHour().Value, 1e-6);
        Assert.AreEqual((expElEnergyConsumption_kWh + expPSEnergyDemand_SocRange_kWh), corrected.ElectricEnergyConsumption_SoC_Corr.ConvertToKiloWattHour().Value, 1e-6);

        //Assert.AreEqual(expDeltaFC_elPS_kg, fc.FcBusAuxElPS.Value(), 1e-6);
        //Assert.AreEqual(expDeltaFC_REESSSoC_kg, fc.FcREESSSoc.Value(), 1e-6);

        // no mechanical PS fuel consumption
        //Assert.AreEqual(0, fc.FcBusAuxPs.Value());

        //Assert.AreEqual(assumedFuelConsumption.Value() + expDeltaFC_elPS_kg + expDeltaFC_REESSSoC_kg, fc.TotalFuelConsumptionCorrected.Value(), 1e-6);

        Assert.AreEqual(expElectricRange_m, range.ActualChargeDepletingRange.Value(), 1e-3);
    }

    // ################################################################################################
    // ################################################################################################
    // ################################################################################################


    protected VectoRunData GetVectoRunDataConventional(IAuxiliaryConfig busAux)
    {

        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            Exempted = false,
            JobType = VectoSimulationJobType.ConventionalVehicle,
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            EngineData = new CombustionEngineData() {
                WHRType = WHRType.None,
                IdleSpeed = 600.RPMtoRad(),
                Fuels = new[] { new CombustionEngineFuelData() {
                    FuelData =Fuel,
                    ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
                }}.ToList(),
            },
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
            BusAuxiliaries = busAux
        };
        return runData;
    }

    protected VectoRunData GetVectoRunDataHybrid(IAuxiliaryConfig busAux,
        VectoSimulationJobType jobType, OvcHevMode ovcMode)
    {
        var emData = new Mock<ElectricMotorData>();

        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            OVCMode = ovcMode,
            Exempted = false,
            MaxChargingPower = 50000.SI<Watt>(),
            JobType = jobType,
            Mission = new Mission() {
                MissionType = MissionType.Urban
            },
            VehicleData = new VehicleData() {
                Loading = 0.SI<Kilogram>(),
                VehicleClass = VehicleClass.Class31a
            },
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            EngineData = new CombustionEngineData() {
                WHRType = WHRType.None,
                IdleSpeed = 600.RPMtoRad(),
                Fuels = new[] { new CombustionEngineFuelData() {
                    FuelData =Fuel,
                    ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
                }}.ToList(),
            },
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
                    Tuple.Create(PowertrainPosition.HybridP2, emData.Object)
                },
            BatteryData = new BatterySystemData() {
                Batteries = new List<Tuple<int, BatteryData>>() {
                        Tuple.Create(0, new BatteryData() {
                            BatteryId = 0,
                            Capacity = 7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            ChargeDepletingBattery = false,
                            InternalResistance =
                                BatteryInternalResistanceReader.Create("SoC, Ri\n0,  0.024\n100,  0.024".ToStream(),
                                    false),
                            SOCMap = BatterySOCReader.Create("SOC, V\n0, 590\n100, 614".ToStream()),
                            MinSOC = 0.1,
                            MaxSOC = 0.9,
                            MaxCurrent =
                                BatteryMaxCurrentReader.Create(
                                    "SOC, I_charge, I_discharge\n0, 1620, 1620\n100, 1620, 1620".ToStream()),
                        })
                    }
            },
            BusAuxiliaries = busAux
        };
        if (jobType == VectoSimulationJobType.SerialHybridVehicle) {
            runData.ElectricMachinesData.Add(Tuple.Create(PowertrainPosition.GEN, emData.Object));
        }
        return runData;
    }

    protected VectoRunData GetVectoRunDataSerialHybrid(IAuxiliaryConfig busAux)
    {
        var emData = new Mock<ElectricMotorData>();
        var genData = new Mock<ElectricMotorData>();


        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            Exempted = false,
            JobType = VectoSimulationJobType.SerialHybridVehicle,
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            EngineData = new CombustionEngineData() {
                WHRType = WHRType.None,
                IdleSpeed = 600.RPMtoRad(),
                Fuels = new[] { new CombustionEngineFuelData() {
                    FuelData =Fuel,
                    ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
                }}.ToList(),
            },
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
                Tuple.Create(PowertrainPosition.BatteryElectricE2, emData.Object),
                Tuple.Create(PowertrainPosition.GEN, genData.Object),
            },
            BusAuxiliaries = busAux
        };
        return runData;
    }

    protected VectoRunData GetVectoRunDataBatteryElectric(IAuxiliaryConfig busAux)
    {
        var emData = new Mock<ElectricMotorData>();

        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            OVCMode = OvcHevMode.ChargeDepleting,
            Exempted = false,
            JobType = VectoSimulationJobType.BatteryElectricVehicle,
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            //EngineData = new CombustionEngineData() {
            //	WHRType = WHRType.None,
            //	IdleSpeed = 600.RPMtoRad(),
            //	Fuels = new[] { new CombustionEngineFuelData() {
            //		FuelData =Fuel,
            //		ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
            //	}}.ToList(),
            //},
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
                    Tuple.Create(PowertrainPosition.BatteryElectricE2, emData.Object),
                },
            BusAuxiliaries = busAux,
            BatteryData = new BatterySystemData() {
                Batteries = new List<Tuple<int, BatteryData>>() {
                        Tuple.Create(0, new BatteryData() {
                            BatteryId = 0,
                            Capacity = 7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            ChargeDepletingBattery = false,
                            InternalResistance =
                                BatteryInternalResistanceReader.Create("SoC, Ri\n0,  0.024\n100,  0.024".ToStream(),
                                    false),
                            SOCMap = BatterySOCReader.Create("SOC, V\n0, 590\n100, 614".ToStream()),
                            MinSOC = 0.1,
                            MaxSOC = 0.9,
                            MaxCurrent =
                                BatteryMaxCurrentReader.Create(
                                    "SOC, I_charge, I_discharge\n0, 1620, 1620\n100, 1620, 1620".ToStream()),
                        })
                    }
            }
        };
        return runData;
    }

    protected IModalDataContainer GetMockModData(VectoRunData runData, Kilogram totalFuelConsumption = null,
        WattSecond emLoss = null, WattSecond genLoss = null, Second cycleDuration = null)
    {
        var mockContainer = new Mock<IVehicleContainer>();

        if (runData.JobType != VectoSimulationJobType.BatteryElectricVehicle) {
            var eng = new Mock<IEngineInfo>();
            eng.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);
            mockContainer.Setup(c => c.EngineInfo).Returns(eng.Object);
        } else {
            var eng = new Mock<IEngineInfo>();
            eng.Setup(e => e.EngineIdleSpeed).Returns(100.RPMtoRad());
            mockContainer.Setup(c => c.EngineInfo).Returns(eng.Object);
        }


        var busAux = new BusAuxiliariesAdapter(mockContainer.Object, runData.BusAuxiliaries);

        var m = new Mock<IModalDataContainer>();
        m.Setup(x => x.Duration).Returns(cycleDuration ?? 3600.SI<Second>());
        m.Setup(x => x.Distance).Returns(100000.SI<Meter>());
        m.Setup(x => x.RunStatus).Returns(VectoRun.Status.Success);
        if (totalFuelConsumption != null) {
            m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr, null))
                .Returns(0.SI<WattSecond>());
            m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr, null))
                .Returns(0.SI<WattSecond>());
            m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_ice_start, null))
                .Returns(0.SI<WattSecond>());
            m.Setup(x => x.FuelData).Returns(new IFuelProperties[] { Fuel });
            m.Setup(x => x.TimeIntegral<Kilogram>(It.IsIn(Fuel.FuelType.GetLabel()), null))
                .Returns<string, Func<SI, bool>>((f, _) => totalFuelConsumption);
            m.Setup(x => x.GetColumnName(It.IsNotNull<IFuelProperties>(), It.IsNotNull<ModalResultField>()))
                .Returns<IFuelProperties, ModalResultField>((f, m) => f.GetLabel());
            m.Setup(x => x.EngineLineCorrectionFactor(It.IsIn(Fuel)))
                .Returns(15.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<KilogramPerWattSecond>());
        } else {
            m.Setup(x => x.FuelData).Returns(new IFuelProperties[] { });
        }

        if (runData.JobType == VectoSimulationJobType.SerialHybridVehicle) {
            var genField = string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(),
                PowertrainPosition.GEN);
            m.Setup(x =>
                x.TimeIntegral<WattSecond>(genField, null)).Returns(200.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>());
            //m.Setup(x => x.TimeIntegral<Kilogram>())
        }

        var airDemand = M03Impl.TotalAirDemandCalculation(runData.BusAuxiliaries, runData.BusAuxiliaries.Actuations);
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_consumer_sum, null))
            .Returns(0.SI<WattSecond>());
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_generated, null))
            .Returns(0.SI<WattSecond>());
        m.Setup(x => x.GetValues<NormLiter>(ModalResultField.Nl_busAux_PS_generated))
            .Returns(new[] { airDemand });
        m.Setup(x => x.AuxHeaterDemandCalc).Returns(busAux.AuxHeaterDemandCalculation);

        if (!runData.JobType.IsOneOf(VectoSimulationJobType.ConventionalVehicle,
                VectoSimulationJobType.EngineOnlySimulation)
            && !runData.ElectricMachinesData.Any()) {
            throw new VectoException("hybrid vehicle requires electric machine");
        }

        if (runData.ElectricMachinesData.Any(x => x.Item1 != PowertrainPosition.GEN)) {
            m.Setup(x => x.GetColumnName(It.IsAny<PowertrainPosition>(), It.IsAny<ModalResultField>()))
                .Returns<PowertrainPosition, ModalResultField>((pos, mrf) =>
                    string.Format(mrf.GetCaption(), pos.GetName()));
            var emPos = runData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
            SetupMockEMotorValues(emLoss, m, emPos, runData.OVCMode);

            if (runData.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
                SetupMockEMotorValues(genLoss, m, PowertrainPosition.GEN, runData.OVCMode);
            }
        }

        if (runData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
            m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_DCDC_missing, null))
                .Returns(0.SI<WattSecond>());
        }

        return m.Object;
    }

    private IResultEntry GetMockCSResult(VectoRunData runData)
    {
        var mock = new Mock<IResultEntry>();

        mock.Setup(m => m.Status).Returns(VectoRun.Status.Success);
		mock.Setup(m => m.Distance).Returns(100000.SI<Meter>());
        var fcMock = new Mock<IFuelConsumptionCorrection>();
        fcMock.Setup(f => f.TotalFuelConsumptionCorrected).Returns(32.SI<Kilogram>());


        mock.Setup(m => m.FuelConsumptionFinal(Fuel.FuelType)).Returns(fcMock.Object);
        mock.Setup(m => m.BatteryData).Returns(runData.BatteryData);
        mock.Setup(m => m.FuelData).Returns(runData.EngineData?.Fuels.Select(x => x.FuelData).ToList() ??
                                            new List<IFuelProperties>());
        mock.Setup(m => m.CO2Total).Returns(10.SI<Kilogram>());
        mock.Setup(m => m.ZEV_CO2).Returns(0.SI<Kilogram>());

        return mock.Object;
    }

    private static void SetupMockEMotorValues(WattSecond emLoss, Mock<IModalDataContainer> m,
        PowertrainPosition emPos, OvcHevMode ovcMode)
    {
        var colName = m.Object.GetColumnName(emPos, ModalResultField.P_EM_electricMotorLoss_);
        if (emPos == PowertrainPosition.IEPC) {
            colName = m.Object.GetColumnName(emPos, ModalResultField.P_IEPC_electricMotorLoss_);
        }

        m.Setup(x => x.TimeIntegral<WattSecond>(It.Is<string>(c => colName.Equals(c)), null))
            .Returns(emLoss ?? 0.SI<WattSecond>());
        m.Setup(x => x.ElectricMotorEfficiencyDrive(emPos)).Returns(0.94);
        m.Setup(x => x.ElectricMotorEfficiencyGenerate(emPos)).Returns(0.92);
        SetupMockBatteryValues(m, ovcMode);
    }

    private static void SetupMockBatteryValues(Mock<IModalDataContainer> m, OvcHevMode ovcMode)
    {
        var batChgEff = 0.95;
        var batDischgEff = 0.93;
        var batEnergy = 200.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
        var factorChg = ovcMode.IsOneOf(OvcHevMode.ChargeSustaining, OvcHevMode.NotApplicable) ? 1 : 0.1;
        var batteryEntries = new[] {
				// internal , terminal
				Tuple.Create(batEnergy * factorChg, batEnergy * factorChg / batChgEff),
                Tuple.Create(-batEnergy, -batEnergy * batDischgEff)
            };
        //m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, null))
        //	.Returns(batteryEntries.Select(x => x.Item1).Sum());
        //m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, It.isnull))
        //	.Returns(delegate { return 200.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>(); });
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, It.IsAny<Func<SI, bool>>()))
            .Returns<ModalResultField, Func<SI, bool>>((_, f) =>
                 batteryEntries.Select(x => x.Item1).Where(x => f == null || f(x)).Sum());
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_terminal, It.IsAny<Func<SI, bool>>()))
            .Returns<ModalResultField,
                Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES, It.IsAny<Func<SI, bool>>()))
            .Returns<ModalResultField,
                Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());

        m.Setup(x => x.GetValues<SI>(ModalResultField.REESSStateOfCharge))
            .Returns(new[] { 0.5.SI<Scalar>(), 0.4.SI<Scalar>() });
    }


    private IAuxiliaryConfig GetBusAuxParams(BusHVACSystemConfiguration hvacConfig, VectoSimulationJobType vehicleType,
        HeatPumpType? driverHeatpumpCooling = null,
        HeatPumpType? passengerHeatpumpCooling = null, HeatPumpType? driverHeatPumpHeating = null,
        HeatPumpType? passengerHeatPumpHeating = null,
        Watt auxHeaterPower = null, HeaterType? electricHeater = null, int passengerCount = 40, double length = 12,
        double height = 3, AlternatorType alternatorType = AlternatorType.Conventional, bool essSupplyfromHVREESS = false)
    {
        return SSMBusAuxModelParameters.CreateBusAuxInputParameters(MissionType.Urban,
            VehicleClass.Class31a,
            vehicleType,
            VehicleCode.CA,
            RegistrationClass.II,
            AxleConfiguration.AxleConfig_4x2,
            articulated: false,
            lowEntry: false,
            length: length.SI<Meter>(),
            height: height.SI<Meter>(),
            width: 2.55.SI<Meter>(),
            numPassengersLowerdeck: passengerCount,
            numPassengersUpperdeck: 0,
            hpHeatingDriver: driverHeatPumpHeating ?? HeatPumpType.none,
            hpCoolingDriver: driverHeatpumpCooling ?? HeatPumpType.none,
            hpHeatingPassenger: passengerHeatPumpHeating ?? HeatPumpType.none,
            hpCoolingPassenger: passengerHeatpumpCooling ?? HeatPumpType.none,
            auxHeaterPower: auxHeaterPower ?? 0.SI<Watt>(),
            airElectricHeater: electricHeater != null && (electricHeater & HeaterType.AirElectricHeater) != 0,
            waterElectricHeater: electricHeater != null && (electricHeater & HeaterType.WaterElectricHeater) != 0,
            otherElectricHeater: electricHeater != null && (electricHeater & HeaterType.OtherElectricHeating) != 0,
            hvacConfig: hvacConfig,
            doubleGlazing: false,
            adjustableAuxHeater: false,
            separateAirdistributionDicts: false,
            adjustableCoolantThermostat: false,
            engineWasteGasHeatExchanger: false,
            steeringpumps: new[] { "Dual displacement" },
            fanTech: "Crankshaft mounted - Discrete step clutch",
            alternatorTech: alternatorType,
            essSupplyfromHVREESS: essSupplyfromHVREESS,
            entranceHeight: 0.3.SI<Meter>(),
            loading: LoadingType.LowLoading);
    }
}
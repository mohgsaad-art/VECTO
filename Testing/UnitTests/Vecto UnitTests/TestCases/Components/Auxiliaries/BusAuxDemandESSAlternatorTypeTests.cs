using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Auxiliaries;

public class BusAuxDemandESSAlternatorTypeTests
{
	public const double AlternatorEfficiency = 0.7;
	public const double DCDCEfficiency = 0.97;

	public const double MaxAlternatorPower = 4000.0;

	public const double I_Base = 25.0;
	public const double I_ICEOff_dr = 20.0;
	public const double I_ICEOff_stop = 10.0;

	public const double P_aux_m_Base = 900;
	public const double P_aux_m_ICEOff_dr = 750;
	public const double P_aux_m_ICEOff_st = 300;

	public const double PowernetVoltage = 28.3;
	public const double P_ES_base = I_Base * PowernetVoltage;  // 707,5 W
	public const double P_ES_ICEOff_dr = I_ICEOff_dr * PowernetVoltage; // 566 W
	public const double P_ES_ICEOff_stop = I_ICEOff_stop * PowernetVoltage; // 283 W

	public const double P_PS_600 = 1839.537986 / 0.97; //1896.430913 W
	public const double P_PS_1000 = 2025.38602 / 0.97; // 2088.026824 W

	public const double Nl_PS = 2.1;

	public static WattSecond ElectricStorageCapacity = 28.3.SI(Unit.SI.Watt.Hour).Cast<WattSecond>();

	public const double REESS_Capacity = 4.5;
	public const double REESS_MinSoC = 0.2;
	public const double REESS_MaxSoC = 0.8;

	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}

    [TestCase(DrivingBehavior.Driving, true, 0,
            P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
            P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
            TestName = "BusAux Case A 1; driving, ICE on")]
    [TestCase(DrivingBehavior.Driving, false, 0,
            0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr,
            P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
            TestName = "BusAux Case A 2; driving, ICE off")]
    [TestCase(DrivingBehavior.Halted, true, 0,
            P_aux_m_Base + P_PS_600 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
            P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_600,
            TestName = "BusAux Case A 3; standstill, ICE on")]
    [TestCase(DrivingBehavior.Halted, false, 0,
            0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st,
            P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency, 0, Nl_PS, 0,
            TestName = "BusAux Case A 4; standstill, ICE off")]
    [TestCase(DrivingBehavior.Braking, true, 0,
            P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
            P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
            TestName = "BusAux Case A 5; braking, ICE on")]
    public void TestBusAux_Case_A(DrivingBehavior drivingBehavior, bool iceOn, double batterySoC,
            double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_busAux_ES_consumer_sum_expected,
            double P_busAux_ES_mech,
            double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected,
            double Nl_PS_gen_expected, double Nl_PS_consumer_expected, double P_PS_expected)
    {
        var container = CreatePowerTrain(AlternatorType.Conventional, batterySoC, null, false);

        // check powertrain architecture and config
        Assert.NotNull(container.Components.FirstOrDefault(x => x is StopStartCombustionEngine));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BusAuxiliariesAdapter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is NoBattery));

        // no HEV
        Assert.Null(container.Components.FirstOrDefault(x => x is DCDCConverter));
        Assert.Null(container.Components.FirstOrDefault(x => x is ElectricSystem));
        Assert.Null(container.Components.FirstOrDefault(x => x is BatterySystem));

        Assert.AreEqual(AlternatorType.Conventional, container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType);
        Assert.IsFalse(container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS);

        TestBusAux_Cases(container, drivingBehavior, iceOn, batterySoC, double.NaN,
            P_auxMech_expected, P_busAux_ES_gen_expected, double.NaN, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech,
            Nl_PS_gen_expected, Nl_PS_consumer_expected, P_PS_expected, double.NaN,
            P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected, null, null, null);
    }

    // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

    [TestCase(DrivingBehavior.Driving, true, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case B 1; driving, ICE on, battery not empty")]
    [TestCase(DrivingBehavior.Driving, true, 0,
        P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, 0, P_ES_base,
        P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case B 2; driving, ICE on, battery empty")]
    [TestCase(DrivingBehavior.Driving, false, 0.5,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr,
        P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case B 3; driving, ICE off, battery not empty")]
    [TestCase(DrivingBehavior.Driving, false, 0,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr,
        P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case B 4; driving, ICE off, battery empty")]
    [TestCase(DrivingBehavior.Halted, true, 0.5,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, P_ES_base, 0, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case B 5; standstill, ICE on, battery not empty")]
    [TestCase(DrivingBehavior.Halted, true, 0,
        P_aux_m_Base + P_PS_600 + P_ES_base / AlternatorEfficiency, P_ES_base, 0, P_ES_base,
        P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case B 6; standstill, ICE on, battery empty")]
    [TestCase(DrivingBehavior.Halted, false, 0.5,
        0, 0, P_ES_ICEOff_stop, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st,
        P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case B 7; standstill, ICE off, battery not empty")]
    [TestCase(DrivingBehavior.Halted, false, 0,
        0, 0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st,
        P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case B 8; standstill, ICE off, battery empty")]
    [TestCase(DrivingBehavior.Braking, true, 0.5,
        P_aux_m_Base + P_PS_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base - MaxAlternatorPower, P_ES_base,
        MaxAlternatorPower / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case B 9; braking, ICE on, battery not full")]
    [TestCase(DrivingBehavior.Braking, true, 1,
        P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, 0, P_ES_base,
        P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case B 10; braking, ICE on, battery full")]
    public void TestBusAux_Case_B(DrivingBehavior drivingBehavior, bool iceOn, double batterySoC,
        double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_bat_P0, double P_busAux_ES_consumer_sum_expected,
        double P_busAux_ES_mech,
        double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected,
        double Nl_PS_gen_expected, double Nl_PS_consumer_expected, double P_PS_expected)
    {
        var container = CreatePowerTrain(AlternatorType.Smart, batterySoC, null, false);


        // check powertrain architecture and config
        Assert.NotNull(container.Components.FirstOrDefault(x => x is StopStartCombustionEngine));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BusAuxiliariesAdapter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is SimpleBattery));

        // no HEV
        Assert.Null(container.Components.FirstOrDefault(x => x is DCDCConverter));
        Assert.Null(container.Components.FirstOrDefault(x => x is ElectricSystem));
        Assert.Null(container.Components.FirstOrDefault(x => x is BatterySystem));

        Assert.AreEqual(AlternatorType.Smart, container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType);
        Assert.IsFalse(container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS);

        TestBusAux_Cases(container, drivingBehavior, iceOn, batterySoC, double.NaN,
            P_auxMech_expected, P_busAux_ES_gen_expected, -P_bat_P0, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech,
            Nl_PS_gen_expected, Nl_PS_consumer_expected, P_PS_expected, double.NaN,
            P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected, null, null, null);
    }

    // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

    [TestCase(DrivingBehavior.Driving, true, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C1 1; driving, ICE on, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, true, REESS_MinSoC,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C1 2; driving, ICE on, REESS empty")]
    [TestCase(DrivingBehavior.Driving, false, 0.5,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, P_ES_ICEOff_dr, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C1 3; driving, ICE off, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, false, REESS_MinSoC,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, P_ES_ICEOff_dr, P_ES_base - P_ES_ICEOff_dr, 0, Nl_PS, 0,
        TestName = "BusAux Case C1 4; driving, ICE off, REESS empty")]

    [TestCase(DrivingBehavior.Halted, true, 0.5,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C1 5; standstill, ICE on, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, true, REESS_MinSoC,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C1 6; standstill, ICE on, REESS empty")]
    [TestCase(DrivingBehavior.Halted, false, 0.5,
        0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, P_ES_ICEOff_stop, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C1 7; standstill, ICE off, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, false, REESS_MinSoC,
        0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, 0, P_ES_ICEOff_stop, P_ES_base - P_ES_ICEOff_stop, 0, Nl_PS, 0,
        TestName = "BusAux Case C1 8; standstill, ICE off, REESS empty")]

    [TestCase(DrivingBehavior.Braking, true, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C1 9; braking, ICE on, REESS not empty")]
    [TestCase(DrivingBehavior.Braking, true, REESS_MinSoC,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C1 10; braking, ICE on, REESS empty")]
    [TestCase(DrivingBehavior.Braking, false, 0.5,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, P_ES_ICEOff_dr, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C1 11; braking, ICE off, REESS not empty")]
    [TestCase(DrivingBehavior.Braking, false, REESS_MinSoC,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, P_ES_ICEOff_dr, P_ES_base - P_ES_ICEOff_dr, 0, Nl_PS, 0,
        TestName = "BusAux Case C1 12; braking, ICE off, REESS empty")]
    public void TestBusAux_Case_C1(DrivingBehavior drivingBehavior, bool iceOn, double reessSoC,
        double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_busAux_ES_consumer_sum_expected,
        double P_busAux_ES_mech,
        double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected, double P_DCDC_out_expected, double P_DCDC_missing_expected, double P_DCDC_missing_ESS_ICE_on,
        double Nl_PS_gen_expected, double Nl_PS_consumer_expected, double P_PS_expected)
    {
        var container = CreatePowerTrain(AlternatorType.None, double.NaN, reessSoC, true);

        // check powertrain architecture and config
        Assert.NotNull(container.Components.FirstOrDefault(x => x is StopStartCombustionEngine));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BusAuxiliariesAdapter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is NoBattery));

        // HEV
        Assert.NotNull(container.Components.FirstOrDefault(x => x is DCDCConverter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is ElectricSystem));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BatterySystem));

        Assert.AreEqual(AlternatorType.None, container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType);
        Assert.IsTrue(container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS);

        TestBusAux_Cases(container, drivingBehavior, iceOn, double.NaN, reessSoC,
            P_auxMech_expected, P_busAux_ES_gen_expected, double.NaN, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech,
            Nl_PS_gen_expected, Nl_PS_consumer_expected, P_PS_expected, double.NaN,
            P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected, P_DCDC_out_expected, P_DCDC_missing_expected, P_DCDC_missing_ESS_ICE_on);

    }

    // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .


    [TestCase(DrivingBehavior.Driving, true, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C2a 1; driving, ICE on, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, true, REESS_MinSoC,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C2a 2; driving, ICE on, REESS empty")]
    [TestCase(DrivingBehavior.Driving, false, 0.5,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, P_ES_ICEOff_dr, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C2a 3; driving, ICE off, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, false, REESS_MinSoC,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, P_ES_ICEOff_dr, P_ES_base - P_ES_ICEOff_dr, 0, Nl_PS, 0,
        TestName = "BusAux Case C2a 4; driving, ICE off, REESS empty")]

    [TestCase(DrivingBehavior.Halted, true, 0.5,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C2a 5; standstill, ICE on, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, true, REESS_MinSoC,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C2a 6; standstill, ICE on, REESS empty")]
    [TestCase(DrivingBehavior.Halted, false, 0.5,
        0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, P_ES_ICEOff_stop, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C2a 7; standstill, ICE off, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, false, REESS_MinSoC,
        0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, 0, P_ES_ICEOff_stop, P_ES_base - P_ES_ICEOff_stop, 0, Nl_PS, 0,
        TestName = "BusAux Case C2a 8; standstill, ICE off, REESS empty")]

    [TestCase(DrivingBehavior.Braking, true, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C2a 9; braking, ICE on, REESS not empty")]
    [TestCase(DrivingBehavior.Braking, true, REESS_MinSoC,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C2a 10; braking, ICE on, REESS empty")]
    [TestCase(DrivingBehavior.Braking, false, 0.5,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, P_ES_ICEOff_dr, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C2a 11; braking, ICE off, REESS not empty")]
    [TestCase(DrivingBehavior.Braking, false, REESS_MinSoC,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, P_ES_ICEOff_dr, P_ES_base - P_ES_ICEOff_dr, 0, Nl_PS, 0,
        TestName = "BusAux Case C2a 12; braking, ICE off, REESS empty")]
    public void TestBusAux_Case_C2A(DrivingBehavior drivingBehavior, bool iceOn, double reessSoC,
        double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_busAux_ES_consumer_sum_expected,
        double P_busAux_ES_mech,
        double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected, double P_DCDC_out_expected, double P_DCDC_missing_expected, double P_DCDC_missing_ESS_ICE_on,
        double Nl_PS_gen_expected, double Nl_PS_consumer_expected, double P_PS_expected)
    {
        var container = CreatePowerTrain(AlternatorType.Conventional, double.NaN, reessSoC, true);

        // check powertrain architecture and config
        Assert.NotNull(container.Components.FirstOrDefault(x => x is StopStartCombustionEngine));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BusAuxiliariesAdapter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is NoBattery));

        // HEV
        Assert.NotNull(container.Components.FirstOrDefault(x => x is DCDCConverter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is ElectricSystem));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BatterySystem));

        // simulated with alternator type NONE!
        Assert.AreEqual(AlternatorType.None, container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType);
        Assert.IsTrue(container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS);

        TestBusAux_Cases(container, drivingBehavior, iceOn, double.NaN, reessSoC,
            P_auxMech_expected, P_busAux_ES_gen_expected, double.NaN, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech, Nl_PS_gen_expected, Nl_PS_consumer_expected, P_PS_expected, double.NaN,
            P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected, P_DCDC_out_expected, P_DCDC_missing_expected, P_DCDC_missing_ESS_ICE_on);

    }


    // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

    [TestCase(DrivingBehavior.Driving, true, 0,
        P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base, P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C2b 1; driving, ICE on")]
    [TestCase(DrivingBehavior.Driving, false, 0,
        0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr,
        P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C2b 2; driving, ICE off")]
    [TestCase(DrivingBehavior.Halted, true, 0,
        P_aux_m_Base + P_PS_600 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
        P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C2b 3; standstill, ICE on")]
    [TestCase(DrivingBehavior.Halted, false, 0,
        0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st,
        P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C2b 4; standstill, ICE off")]
    [TestCase(DrivingBehavior.Braking, true, 0,
        P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
        P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C2b 5; braking, ICE on")]
    public void TestBusAux_Case_C2B(DrivingBehavior drivingBehavior, bool iceOn, double batterySoC,
        double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_busAux_ES_consumer_sum_expected,
        double P_busAux_ES_mech,
        double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected, double Nl_PS_gen_expected, double Nl_PS_consumer_expected, double P_PS_expected)
    {
        var container = CreatePowerTrain(AlternatorType.Conventional, batterySoC, 0.5, false);

        // check powertrain architecture and config
        Assert.NotNull(container.Components.FirstOrDefault(x => x is StopStartCombustionEngine));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BusAuxiliariesAdapter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is NoBattery));

        // HEV, no DCDC converter
        Assert.Null(container.Components.FirstOrDefault(x => x is DCDCConverter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is ElectricSystem));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BatterySystem));

        // simulated with alternator type NONE!
        Assert.AreEqual(AlternatorType.Conventional, container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType);
        Assert.IsFalse(container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS);

        TestBusAux_Cases(container, drivingBehavior, iceOn, batterySoC, double.NaN,
            P_auxMech_expected, P_busAux_ES_gen_expected, double.NaN, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech, Nl_PS_gen_expected, Nl_PS_consumer_expected, P_PS_expected, double.NaN,
            P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected, null, null, null);

    }

    // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .


    [TestCase(DrivingBehavior.Driving, true, 0.5, 0.5, double.NaN,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 1; driving, ICE on, battery not empyt, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, true, 0.0, 0.5, double.NaN,
        P_aux_m_Base + P_PS_1000, 0, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 2; driving, ICE on, battery empty, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, true, 0.5, REESS_MinSoC, double.NaN,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 3; driving, ICE on, battery not empty, REESS empty")]
    [TestCase(DrivingBehavior.Driving, true, 0.0, REESS_MinSoC, double.NaN,
        P_aux_m_Base + P_PS_1000, 0, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 4; driving, ICE on, battery empty, REESS empty")]

    [TestCase(DrivingBehavior.Driving, false, 0.5, 0.5, double.NaN,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 5; driving, ICE off, battery not empty, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, false, 0.0, 0.5, double.NaN,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, P_ES_ICEOff_dr, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 6; driving, ICE off, battery empty, REESS not empty")]
    [TestCase(DrivingBehavior.Driving, false, 0.5, REESS_MinSoC, double.NaN,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, 0, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 7; driving, ICE off, battery not empty,  REESS empty")]
    [TestCase(DrivingBehavior.Driving, false, 0.0, REESS_MinSoC, double.NaN,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, P_ES_ICEOff_dr, 0, 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 8; driving, ICE off, battery empty,  REESS empty")]

    [TestCase(DrivingBehavior.Halted, true, 0.5, 0.5, double.NaN,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, P_ES_base, 0, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C3a 9; standstill, ICE on, battery not empty, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, true, 0.0, 0.5, double.NaN,
        P_aux_m_Base + P_PS_600, 0, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C3a 10; standstill, ICE on, battery empty, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, true, 0.5, REESS_MinSoC, double.NaN,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, P_ES_base, 0, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C3a 11; standstill, ICE on, battery not empty, REESS empty")]
    [TestCase(DrivingBehavior.Halted, true, 0.0, REESS_MinSoC, double.NaN,
        P_aux_m_Base + P_PS_600, 0, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C3a 12; standstill, ICE on, battery empty, REESS empty")]

    [TestCase(DrivingBehavior.Halted, false, 0.5, 0.5, double.NaN,
        0, 0, P_ES_ICEOff_stop, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, 0, 0, (P_ES_base - P_ES_ICEOff_stop), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 13; standstill, ICE off, battery not empty, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, false, 0.0, 0.5, double.NaN,
        0, 0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, P_ES_ICEOff_stop, 0, (P_ES_base - P_ES_ICEOff_stop), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 14; standstill, ICE off, battery empty, REESS not empty")]
    [TestCase(DrivingBehavior.Halted, false, 0.5, REESS_MinSoC, double.NaN,
        0, 0, P_ES_ICEOff_stop, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, 0, 0, (P_ES_base - P_ES_ICEOff_stop), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 15; standstill, ICE off, battery not empty, REESS empty")]
    [TestCase(DrivingBehavior.Halted, false, 0.0, REESS_MinSoC, double.NaN,
        0, 0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base, 0, P_ES_ICEOff_stop, (P_ES_base - P_ES_ICEOff_stop), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 16; standstill, ICE off, battery empty, REESS empty")]

    [TestCase(DrivingBehavior.Braking, true, 0.5, 0.5, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 17; braking, ICE on, battery not empty, REESS not empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, true, 0.0, 0.5, 0.5,
        P_aux_m_Base + P_PS_1000, 0, 0, P_ES_base, 0, 0, 0, P_ES_base, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 18; braking, ICE on, battery empty, REESS not empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, true, 0.5, REESS_MinSoC, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 19; braking, ICE on, battery not empty, REESS empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, true, 0.0, REESS_MinSoC, 0.5,
        P_aux_m_Base + P_PS_1000, 0, 0, P_ES_base, 0, 0, 0, 0, P_ES_base, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 20; braking, ICE on, battery empty, REESS empty, P1 recup 0,5")]

    [TestCase(DrivingBehavior.Braking, false, 0.5, 0.5, 0.5,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, 0, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 21; braking, ICE off, battery not empty, REESS not empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, false, 0.0, 0.5, 0.5,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, P_ES_ICEOff_dr, 0, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 22; braking, ICE off, battery empty, REESS not empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, false, 0.5, REESS_MinSoC, 0.5,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, 0, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 23; braking, ICE off, battery not empty, REESS empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, false, 0.0, REESS_MinSoC, 0.5,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, P_ES_ICEOff_dr, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 24; braking, ICE off, battery empty, REESS empty, P1 recup 0,5")]

    [TestCase(DrivingBehavior.Braking, true, 0.5, 0.5, 1,
        P_aux_m_Base + P_PS_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base - MaxAlternatorPower, P_ES_base, MaxAlternatorPower / AlternatorEfficiency, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 25; braking, ICE on, battery not empty, REESS not empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, true, 0.0, 0.5, 1,
        P_aux_m_Base + P_PS_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base - MaxAlternatorPower, P_ES_base, MaxAlternatorPower / AlternatorEfficiency, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 26; braking, ICE on, battery empty, REESS not empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, true, 0.5, REESS_MinSoC, 1,
        P_aux_m_Base + P_PS_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base - MaxAlternatorPower, P_ES_base, MaxAlternatorPower / AlternatorEfficiency, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 27; braking, ICE on, battery not empty, REESS empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, true, 0.0, REESS_MinSoC, 1,
        P_aux_m_Base + P_PS_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base - MaxAlternatorPower, P_ES_base, MaxAlternatorPower / AlternatorEfficiency, 0, 0, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3a 28; braking, ICE on, battery empty, REESS empty, P1 recup 1")]

    [TestCase(DrivingBehavior.Braking, false, 0.5, 0.5, 1,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, 0, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 29; braking, ICE off, battery not empty, REESS not empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, false, 0.0, 0.5, 1,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, P_ES_ICEOff_dr, 0, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 30; braking, ICE off, battery empty, REESS not empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, false, 0.5, REESS_MinSoC, 1,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, 0, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0,
        TestName = "BusAux Case C3a 31; braking, ICE off, battery not empty, REESS empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, false, 0.0, REESS_MinSoC, 1,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base, 0, P_ES_ICEOff_dr, (P_ES_base - P_ES_ICEOff_dr), 0, Nl_PS, 0
        ,
        TestName = "BusAux Case C3a 32; braking, ICE off, battery empty, REESS empty, P1 recup 1")]

    public void TestBusAux_Case_C3A(DrivingBehavior drivingBehavior, bool iceOn, double batterySoC, double reessSoC, double P1recupPct,
        double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_bat_P0, double P_busAux_ES_consumer_sum_expected,
        double P_busAux_ES_mech,
        double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected,
        double P_DCDC_out_expected, double P_DCDC_missing_expected, double P_DCDC_missing_ESS_ICE_on,
        double Nl_PS_gen_expected, double Nl_PS_consumer_expected, double P_PS_expected)
    {
        var container = CreatePowerTrain(AlternatorType.Smart, batterySoC, reessSoC, true);


        // check powertrain architecture and config
        Assert.NotNull(container.Components.FirstOrDefault(x => x is StopStartCombustionEngine));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BusAuxiliariesAdapter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is SimpleBattery));

        // HEV, no DCDC converter
        Assert.NotNull(container.Components.FirstOrDefault(x => x is DCDCConverter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is ElectricSystem));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BatterySystem));

        // simulated with alternator type NONE!
        Assert.AreEqual(AlternatorType.Smart, container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType);
        Assert.IsTrue(container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS);

        TestBusAux_Cases(container, drivingBehavior, iceOn, batterySoC, double.NaN,
            P_auxMech_expected, P_busAux_ES_gen_expected, -P_bat_P0, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech, Nl_PS_gen_expected, Nl_PS_consumer_expected, P_PS_expected, P1recupPct,
            P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected, null, null, null);
    }

    // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .


    [TestCase(DrivingBehavior.Driving, true, 0.5, double.NaN,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3b 1; driving, ICE on, P0 battery not empty")]
    [TestCase(DrivingBehavior.Driving, true, 0, double.NaN,
        P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, 0, P_ES_base, P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
    TestName = "BusAux Case C3b 2; driving, ICE on, P0 battery empty")]
    [TestCase(DrivingBehavior.Driving, false, 0.5, double.NaN,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 3; driving, ICE off, P0 battery not empty")]
    [TestCase(DrivingBehavior.Driving, false, 0, double.NaN,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 4; driving, ICE off, P0 battery empty")]

    [TestCase(DrivingBehavior.Halted, true, 0.5, double.NaN,
        P_aux_m_Base + P_PS_600, 0, P_ES_base, P_ES_base, 0, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C3b 5; standstill, ICE on, P0 battery not empty")]
    [TestCase(DrivingBehavior.Halted, true, 0, double.NaN,
        P_aux_m_Base + P_PS_600 + P_ES_base / AlternatorEfficiency, P_ES_base, 0, P_ES_base, P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_600,
        TestName = "BusAux Case C3b 6; standstill, ICE on, P0 battery empty")]
    [TestCase(DrivingBehavior.Halted, false, 0.5, double.NaN,
        0, 0, P_ES_ICEOff_stop, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 7; standstill, ICE off, P0 battery not empty")]
    [TestCase(DrivingBehavior.Halted, false, 0, double.NaN,
        0, 0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 8; standstill, ICE off, P0 battery empty")]

    [TestCase(DrivingBehavior.Braking, true, 0.5, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3b 9; braking, ICE on, P0 battery not empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, true, 1, 0.5,
        P_aux_m_Base + P_PS_1000, 0, P_ES_base, P_ES_base, 0, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3b 10; braking, ICE on, P0 battery full, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, true, 0, 0.5,
        P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, 0, P_ES_base, P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3b 11; braking, ICE on, P0 battery empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, false, 0.5, 0.5,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 12; braking, ICE off, P0 battery not empty, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, false, 1, 0.5,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 13; braking, ICE off, P0 battery full, P1 recup 0,5")]
    [TestCase(DrivingBehavior.Braking, false, 0, 0.5,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 14; braking, ICE off, P0 battery empty, P1 recup 0,5")]

    [TestCase(DrivingBehavior.Braking, true, 0.5, 1,
        P_aux_m_Base + P_PS_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base - MaxAlternatorPower, P_ES_base, MaxAlternatorPower / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3b 15; braking, ICE on, P0 battery not empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, true, 1, 1,
        P_aux_m_Base + P_PS_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, 0, P_ES_base, P_ES_base / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3b 16; braking, ICE on, P0 battery full, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, true, 0, 1,
        P_aux_m_Base + P_PS_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base - MaxAlternatorPower, P_ES_base, MaxAlternatorPower / AlternatorEfficiency, 0, 0, Nl_PS, Nl_PS, P_PS_1000,
        TestName = "BusAux Case C3b 17; braking, ICE on, P0 battery empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, false, 0.5, 1,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 18; braking, ICE off, P0 battery not empty, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, false, 1, 1,
        0, 0, P_ES_ICEOff_dr, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 19; braking, ICE off, P0 battery full, P1 recup 1")]
    [TestCase(DrivingBehavior.Braking, false, 0, 1,
        0, 0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr, P_aux_m_Base + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency, 0, Nl_PS, 0,
        TestName = "BusAux Case C3b 20; braking, ICE off, P0 battery empty, P1 recup 1")]
    public void TestBusAux_Case_C3B(DrivingBehavior drivingBehavior, bool iceOn, double batterySoC, double P1recupPct,
        double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_bat_P0, double P_busAux_ES_consumer_sum_expected,
        double P_busAux_ES_mech, double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected,
        double Nl_PS_gen_expected, double Nl_PS_consumer_expected, double P_PS_expected)
    {
        var container = CreatePowerTrain(AlternatorType.Smart, batterySoC, 0.5, false);

        // check powertrain architecture and config
        Assert.NotNull(container.Components.FirstOrDefault(x => x is StopStartCombustionEngine));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BusAuxiliariesAdapter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is SimpleBattery));

        // HEV
        Assert.Null(container.Components.FirstOrDefault(x => x is DCDCConverter));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is ElectricSystem));
        Assert.NotNull(container.Components.FirstOrDefault(x => x is BatterySystem));

        // simulated with alternator type NONE!
        Assert.AreEqual(AlternatorType.Smart, container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType);
        Assert.IsFalse(container.RunData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS);

        TestBusAux_Cases(container, drivingBehavior, iceOn, batterySoC, double.NaN,
            P_auxMech_expected, P_busAux_ES_gen_expected, -P_bat_P0, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech, Nl_PS_gen_expected, Nl_PS_consumer_expected, P_PS_expected, P1recupPct,
            P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected, null, null, null);
    }

    // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

    public void TestBusAux_Cases(MockVehicleContainer container, DrivingBehavior drivingBehavior, bool iceOn,
        double batterySoC, double reessSoC,
        double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_bat_P0,
        double P_busAux_ES_consumer_sum_expected,
        double P_busAux_ES_mech,
        double Nl_gen_expected, double Nl_consumed_expected, double P_PS_m_expected,
        double P1_recup_Pct,
        double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected, double? P_DCDC_out_expected,
        double? P_DCDC_missing_expected, double? P_DCDC_missing_ESS_ICE_on)
    {
        container.VehicleStopped = drivingBehavior == DrivingBehavior.Halted;
        container.VehicleSpeed = drivingBehavior == DrivingBehavior.Halted
            ? 0.KMPHtoMeterPerSecond()
            : 30.KMPHtoMeterPerSecond();
        container.DriverBehavior = drivingBehavior;
        container.DrivingAction = drivingBehavior == DrivingBehavior.Halted
            ? DrivingAction.Halt
            : (drivingBehavior == DrivingBehavior.Braking ? DrivingAction.Brake : DrivingAction.Accelerate);
        container.EngineSpeed = drivingBehavior == DrivingBehavior.Halted ? 600.RPMtoRad() : 1000.RPMtoRad();
        container.EngineCtl.CombustionEngineOn = iceOn;


        var ice = container.Components.First(x => x is ICombustionEngine) as StopStartCombustionEngine;
        Assert.NotNull(ice);

        var modData = container.ModalData as ModalDataContainer;
        Assert.NotNull(modData);

        var absTime = 0.SI<Second>();
        var dt = 1.SI<Second>();
        IResponse response = null;
        switch (drivingBehavior) {
            case DrivingBehavior.Halted:
                container.Gear = new GearshiftPosition(0);
                ice.CombustionEngineOn = iceOn;
                response = ice.Request(absTime, dt, 0.SI<NewtonMeter>(), 600.RPMtoRad(), false);
                break;
            case DrivingBehavior.Accelerating:
            case DrivingBehavior.Driving:
                container.Gear = new GearshiftPosition(3);
                ice.CombustionEngineOn = iceOn;
                response = ice.Request(absTime, dt, 0.SI<NewtonMeter>(), iceOn ? 1000.RPMtoRad() : 600.RPMtoRad(), false);
                break;
            case DrivingBehavior.Braking:
                container.Gear = new GearshiftPosition(3);
                container.BrakePower = P1_recup_Pct.IsSmaller(1) ? 0.SI<Watt>() : 50e3.SI<Watt>();
                ice.CombustionEngineOn = iceOn;
                response = ice.Request(absTime, dt, 0.SI<NewtonMeter>(), iceOn ? 1000.RPMtoRad() : 600.RPMtoRad(), false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(drivingBehavior), drivingBehavior, null);
        }

        (container.Components.FirstOrDefault(x => x is IElectricSystem) as ElectricSystem)?.Request(absTime, dt, 0.SI<Watt>());

        Assert.NotNull(response);
        Assert.IsAssignableFrom<ResponseSuccess>(response);

        container.CommitSimulationStep(absTime, dt);
        modData.CommitSimulationStep();

        Assert.AreEqual(1, modData.Data.Rows.Count);

        var row = modData.Data.Rows[0];

        Assert.AreEqual(iceOn, (bool)row[ModalResultField.ICEOn.GetName()], ModalResultField.ICEOn.GetName());
        Assert.AreEqual(P_auxMech_expected, ((SI)row[ModalResultField.P_aux_mech.GetName()]).Value(), 1e-3, ModalResultField.P_aux_mech.GetName());
        Assert.AreEqual(P_busAux_ES_gen_expected, ((SI)row[ModalResultField.P_busAux_ES_generated.GetName()]).Value(), 1e-3, ModalResultField.P_busAux_ES_generated.GetName());
        Assert.AreEqual(P_busAux_ES_consumer_sum_expected, ((SI)row[ModalResultField.P_busAux_ES_consumer_sum.GetName()]).Value(), 1e-3, ModalResultField.P_busAux_ES_consumer_sum.GetName());
        Assert.AreEqual(P_busAux_ES_consumer_sum_expected, ((SI)row[ModalResultField.P_busAux_ES_other.GetName()]).Value(), 1e-3, ModalResultField.P_busAux_ES_other.GetName());
        Assert.AreEqual(P_busAux_ES_mech, ((SI)row[ModalResultField.P_busAux_ES_sum_mech.GetName()]).Value(), 1e-3, ModalResultField.P_busAux_ES_sum_mech.GetName());

        Assert.AreEqual(Nl_consumed_expected, ((SI)row[ModalResultField.Nl_busAux_PS_consumer.GetName()]).Value(), 1e-3, ModalResultField.Nl_busAux_PS_consumer.GetName());
        Assert.AreEqual(Nl_gen_expected, ((SI)row[ModalResultField.Nl_busAux_PS_generated.GetName()]).Value(), 1e-3, ModalResultField.Nl_busAux_PS_generated.GetName());
        Assert.AreEqual(P_PS_m_expected, ((SI)row[ModalResultField.P_busAux_PS_generated.GetName()]).Value(), 1e-3, ModalResultField.P_busAux_PS_generated.GetName());

        Assert.AreEqual(P_aux_ESS_mech_ICE_off_expected, ((SI)row[ModalResultField.P_aux_ESS_mech_ice_off.GetName()]).Value(), 1e-3, ModalResultField.P_aux_ESS_mech_ice_off.GetName());
        Assert.AreEqual(P_aux_ESS_mech_ICE_on_expected, ((SI)row[ModalResultField.P_aux_ESS_mech_ice_on.GetName()]).Value(), 1e-3, ModalResultField.P_aux_ESS_mech_ice_on.GetName());

        if (!double.IsNaN(P_bat_P0)) {
            Assert.AreEqual(P_bat_P0, ((SI)row[ModalResultField.P_busAux_bat.GetName()]).Value(), 1e-3, ModalResultField.P_busAux_bat.GetName());
        } else {
            Assert.IsTrue(container.Components.Any(x => x is NoBattery));
        }

        if (P_DCDC_missing_expected.HasValue && P_DCDC_out_expected.HasValue && P_DCDC_missing_ESS_ICE_on.HasValue) {
            var dcdcConverter = container.Components.FirstOrDefault(x => x is IDCDCConverter) as DCDCConverter;
            Assert.NotNull(dcdcConverter);

            // 'consumed' and 'missing' electric energy is applied in next simulation step - read out directly and perform additional step
            if (reessSoC.IsEqual(REESS_MinSoC)) {
                Assert.AreEqual(P_DCDC_missing_expected.Value, (dcdcConverter.PreviousState.ConsumedEnergy / dt).Value(), 1e-3, ModalResultField.P_DCDC_missing.GetName());
            } else {
                Assert.AreEqual(P_busAux_ES_consumer_sum_expected, (dcdcConverter.PreviousState.ConsumedEnergy / dt).Value(), 1e-3, ModalResultField.P_DCDC_Out.GetName());
            }

            var dcdcDemand = dcdcConverter.PowerDemand(absTime, dt, false);

            if (reessSoC.IsEqual(REESS_MinSoC)) {
                Assert.AreEqual(0, dcdcDemand.Value(), 1e-3, "DC/DC PowerDemand");
            } else {
                Assert.AreEqual(P_DCDC_out_expected.Value / DCDCEfficiency, dcdcDemand.Value(), 1e-3,
                    "DC/DC PowerDemand OUT");
            }

            dcdcConverter.CommitSimulationStep(absTime, dt, modData);
            modData.CommitSimulationStep();
            var row1 = modData.Data.Rows[1];

            Assert.AreEqual(P_DCDC_missing_expected.Value, ((SI)row1[ModalResultField.P_DCDC_missing.GetName()]).Value(), 1e-3, ModalResultField.P_DCDC_missing.GetName());

            Assert.AreEqual(P_DCDC_out_expected.Value, ((SI)row1[ModalResultField.P_DCDC_Out.GetName()]).Value(), 1e-3, ModalResultField.P_DCDC_Out.GetName());
            Assert.AreEqual(P_DCDC_out_expected.Value / DCDCEfficiency, ((SI)row1[ModalResultField.P_DCDC_In.GetName()]).Value(), 1e-3, ModalResultField.P_DCDC_In.GetName());

            // TODO: Assertion P_DCDC_missing_ESS_ICE_on

        }
    }

	public const string EngineFLDHeader = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	public static readonly string[] EngineFLDData = new string[] {
		"560,1500,-149,0.6",
		"600,1600,-148,0.6",
		"799.9999999,2150,-149,0.6",
		"1000,2300,-160,0.6",
		"1200,2300,-179,0.6",
		"1400,2300,-203,0.6",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25",
	};

    public const string EngineMapHdr = "engine speed [1/min],torque [Nm],fuel consumption [g/h]";

	public static readonly string[] EngineMapData = new[] {
		"560,-149,0",
		"560,0,1256",
		"560,200,3197",
		"560,400,5295",
		"560,600,7615",
		"560,800,9375",
		"560,1000,11239",
		"560,1180,12869",
		"2100,-320,0",
		"2100,0,10470",
		"2100,200,16332",
		"2100,400,22396",
		"2100,600,28914",
		"2100,800,35717",
		"2100,1000,45643",
		"2100,1100,50653",
	};
    public MockVehicleContainer CreatePowerTrain(AlternatorType alternatorType, double initialSoC,
            double? reessSoC, bool connectEsToReess)
	{
		var fld = FullLoadCurveReader.Create(
			InputDataHelper.InputDataAsTableData(EngineFLDHeader, EngineFLDData));
		var fuelmap =
			FuelConsumptionMapReader.Create(InputDataHelper.InputDataAsTableData(EngineMapHdr, EngineMapData));
		var engineData = new CombustionEngineData() {
			IdleSpeed = 560.RPMtoRad(),
			Inertia = 3.8.SI<KilogramSquareMeter>(),
			EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime,
			FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
				{ 0u, fld },
				{ 1u, fld },
				{ 2u, fld },
				{ 3u, fld },
			},
			Fuels = new List<CombustionEngineFuelData>() {
				new CombustionEngineFuelData() {
					WHTCMotorway = 1,
					WHTCRural = 1,
					WHTCUrban = 1,
					ColdHotCorrectionFactor = 1,
					CorrectionFactorRegPer = 1,
					FuelConsumptionCorrectionFactor = 1,
					FuelData = FuelData.Diesel,
					ConsumptionMap = fuelmap
				}
			}

		}; // MockSimulationDataFactory.CreateEngineDataFromFile(EngineFileHigh, 6);
        var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
        
        var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);

        var runData = new VectoRunData() {
            JobRunId = 0,
            VehicleData = vehicleData,
            EngineData = engineData,
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
            SimulationType = SimulationType.DistanceCycle,
            Cycle = cycleData,
            BusAuxiliaries = CreateBusAuxData(alternatorType, vehicleData, connectEsToReess),
            Aux = CreateAuxiliaryData(P_aux_m_Base.SI<Watt>(), P_aux_m_ICEOff_dr.SI<Watt>(), P_aux_m_ICEOff_st.SI<Watt>())
        };


		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, null, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);


        var container = new MockVehicleContainer() {
            CycleData = new CycleData() { LeftSample = cycleData.Entries.First() },
            ModalData = modData,
            HasCombustionEngine = true,
            HasElectricMotor = false,
            RunData = runData
        };
		var engine = new StopStartCombustionEngine(container, engineData);

        container.EngineInfo = engine;

        var conventionalAux = CreateAuxiliaries(runData.Aux, container);
        var aux = new BusAuxiliariesAdapter(container, runData.BusAuxiliaries, conventionalAux);

        var auxCfg = runData.BusAuxiliaries;
        var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
            ? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity,
                auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency, initialSoC)
            : (ISimpleBattery)new NoBattery(container);
        aux.ElectricStorage = electricStorage;
        engine.Connect(aux.Port());


        if (reessSoC.HasValue) {
            // hybrid powertrain
            var packCount = 2;
            runData.BatteryData = new BatterySystemData() {
                Batteries = new List<Tuple<int, BatteryData>>() {
                        Tuple.Create(0, new BatteryData() {
                            Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            MinSOC = REESS_MinSoC,
                            MaxSOC = REESS_MaxSoC,
                            SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                            InternalResistance =
                                BatteryInternalResistanceReader.Create("SoC, Ri\n0,0.02\n100,0.02".ToStream(), false),
                            MaxCurrent = BatteryMaxCurrentReader.Create(
                                "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                        }),
                    },
                InitialSoC = reessSoC.Value
            };
            var es = new ElectricSystem(container, runData.BatteryData);
            var battery = new BatterySystem(container, runData.BatteryData);
            battery.Initialize(runData.BatteryData.InitialSoC);
            container.BatteryInfo = battery;
            es.Connect(battery);
            if (connectEsToReess) {
                var dcdc = new DCDCConverter(container, DCDCEfficiency);
                dcdc.Initialize();
                aux.DCDCConverter = dcdc;
                es.Connect(dcdc);
            }
        }


        return container;
    }

    private static IAuxPort CreateAuxiliaries(IEnumerable<VectoRunData.AuxData> auxDataList,
        IVehicleContainer container)
    {
        var aux = new EngineAuxiliary(container);
        foreach (var auxData in auxDataList) {
            // id's in upper case
            var id = auxData.ID.ToUpper();

            switch (auxData.DemandType) {
                case AuxiliaryDemandType.Constant:
                    aux.AddConstant(id, auxData.PowerDemandMech);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
            }

            container.AddAuxiliary(id);
        }

        return aux;
    }

    private static IList<VectoRunData.AuxData> CreateAuxiliaryData(Watt pwrICEOn, Watt pwrICEOffDriving, Watt pwrICEOffStandstill)
    {
        var baseDemand = pwrICEOffStandstill;
        var stpDemand = pwrICEOffDriving - pwrICEOffStandstill;
        var fanDemand = pwrICEOn - pwrICEOffDriving;

        var auxList = new List<VectoRunData.AuxData>() {
                new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_BASE, DemandType = AuxiliaryDemandType.Constant, PowerDemandMech = baseDemand},
                new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_STP, DemandType = AuxiliaryDemandType.Constant, PowerDemandMech = stpDemand},
                new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_FAN, DemandType = AuxiliaryDemandType.Constant, PowerDemandMech = fanDemand},
            };

        return auxList;
    }

    public const string CompressorMapHdr = "rpm,flowRate [l/min],power on [W],power off [W]";

	public static readonly string[] CompressorMapData = new[] {
		"800,250.5365596,3139.5,524.3",
		"1200,374.3533986,4609.5,1027.2",
		"1600,508.4123859,6205.5,1572.9",
		"2000,619.1263282,7770,2065.1",
		"2400,762.6185788,9723,2696.4",
		"2550,819.2371476,10363.5,2856.9",
		"2800,898.7501978,11613,3349.1",
		"3200,979.4827586,13282.5,4012.5",
	};

    private static AuxiliaryConfig CreateBusAuxData(AlternatorType alternatorType, VehicleData vehicleData,
        bool esSupplyFromHevreess)
    {
        //var CompressorMap = "TestData/Integration/BusAuxiliaries/DEFAULT_2-Cylinder_1-Stage_650ccm.ACMP";
        var AverageAirDemand = Nl_PS.SI<NormLiterPerSecond>();
        var SmartAirCompression = false;
        var GearRatio = 1.0;

        var BatteryEfficiency = 1.0;
        //var ESSupplyFromHEVREESS = false;
        //var AlternatorType = VectoCommon.Models.AlternatorType.Smart;
        var ElectricPowerDemand = 0.0.SI<Watt>();
        var MechanicalPowerDemand = 0.0.SI<Watt>();
        var AuxHeaterPower = 0.0.SI<Watt>();
        var AverageHeatingDemand = 0.0.SI<Joule>();

        var retVal = new AuxiliaryConfig() {
            //InputData = auxInputData.BusAuxiliariesData,
            ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
                PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
                //StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
                ResultCardIdle = new DummyResultCard(),
                ResultCardOverrun = new DummyResultCard(),
                ResultCardTraction = new DummyResultCard(),
                AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
                DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
                AlternatorMap = new SimpleAlternator(AlternatorEfficiency) {
                    //Technologies = new List<string>() { "engineering mode" }
                },
                AlternatorType = esSupplyFromHevreess &&
                                alternatorType != AlternatorType.Smart
                    ? AlternatorType.None
                    : alternatorType,
                ConnectESToREESS = esSupplyFromHevreess,
                DCDCEfficiency = DCDCEfficiency,
                MaxAlternatorPower = MaxAlternatorPower.SI<Watt>(),
                ElectricStorageCapacity = ElectricStorageCapacity ?? 0.SI<WattSecond>(),
                StoredEnergyEfficiency = BatteryEfficiency,
                ElectricalConsumers = GetElectricConsumers(I_Base.SI<Ampere>(), I_ICEOff_dr.SI<Ampere>(), I_ICEOff_stop.SI<Ampere>()),
            },
            PneumaticAuxiliariesConfig = new PneumaticsConsumersDemand() {
                AdBlueInjection = 0.SI<NormLiterPerSecond>(),
                AirControlledSuspension = AverageAirDemand,
                Braking = 0.SI<NormLiterPerKilogram>(),
                BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
                DeadVolBlowOuts = 0.SI<PerSecond>(),
                DeadVolume = 0.SI<NormLiter>(),
                NonSmartRegenFractionTotalAirDemand = 0,
                SmartRegenFractionTotalAirDemand = 0,
                OverrunUtilisationForCompressionFraction =
                    Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
                DoorOpening = 0.SI<NormLiter>(),
                StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
            },
            PneumaticUserInputsConfig = new PneumaticUserInputsConfig() {
                CompressorMap =
                    new CompressorMap(CompressorMapReader.Create(VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream(CompressorMapHdr, CompressorMapData)), 1.0),
                        "engineering mode", ""),
                CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
                CompressorGearRatio = GearRatio,
                SmartAirCompression = SmartAirCompression,
                SmartRegeneration = false,
                KneelingHeight = 0.SI<Meter>(),
                AirSuspensionControl = ConsumerTechnology.Pneumatically,
                AdBlueDosing = ConsumerTechnology.Electrically,
                Doors = ConsumerTechnology.Electrically
            },
            Actuations = new Actuations() {
                Braking = 0,
                Kneeling = 0,
                ParkBrakeAndDoors = 0,
                CycleTime = 1.SI<Second>()
            },
            SSMInputsCooling = new SSMEngineeringInputs() {
                MechanicalPower = MechanicalPowerDemand,
                ElectricPower = ElectricPowerDemand,
                AuxHeaterPower = AuxHeaterPower,
                HeatingDemand = AverageHeatingDemand,
                AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
                FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
                CoolantHeatTransferredToAirCabinHeater =
                    Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
            },
            VehicleData = vehicleData,
        };
        retVal.SSMInputsHeating = retVal.SSMInputsCooling;
        return retVal;
    }

    private static IDictionary<string, ElectricConsumerEntry> GetElectricConsumers(Ampere currentDemand, Ampere currentDemandEngineOffDriving, Ampere currentDemandEngineOffStandstill)
    {
        var retVal = new Dictionary<string, ElectricConsumerEntry>();

        var iBase = currentDemandEngineOffStandstill;
        var iSP = currentDemandEngineOffDriving -
                currentDemandEngineOffStandstill;
        var iFan = currentDemand - currentDemandEngineOffDriving;

        retVal["BaseLoad"] = new ElectricConsumerEntry() {
            Current = iBase,
            BaseVehicle = true
        };
        retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry() {
            Current = iSP,
            ActiveDuringEngineStopStandstill = false,
        };
        retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry() {
            Current = iFan,
            ActiveDuringEngineStopStandstill = false,
            ActiveDuringEngineStopDriving = false,
        };
        return retVal;
    }



    private static VehicleData CreateVehicleData(Kilogram loading)
    {
        var axles = new List<Axle> {
                new Axle {
                    AxleWeightShare = 0.4375,
                    Inertia = 21.66667.SI<KilogramSquareMeter>(),
                    RollResistanceCoefficient = 0.0055,
                    TwinTyres = false,
                    TyreTestLoad = 62538.75.SI<Newton>()
                },
                new Axle {
                    AxleWeightShare = 0.375,
                    Inertia = 10.83333.SI<KilogramSquareMeter>(),
                    RollResistanceCoefficient = 0.0065,
                    TwinTyres = true,
                    TyreTestLoad = 52532.55.SI<Newton>()
                },
                new Axle {
                    AxleWeightShare = 0.1875,
                    Inertia = 21.66667.SI<KilogramSquareMeter>(),
                    RollResistanceCoefficient = 0.0055,
                    TwinTyres = false,
                    TyreTestLoad = 62538.75.SI<Newton>()
                }
            };
        return new VehicleData {
            AirDensity = DeclarationData.AirDensity,
            AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
            CurbMass = 15700.SI<Kilogram>(),
            Loading = loading,
            DynamicTyreRadius = 0.52.SI<Meter>(),
            AxleData = axles,
            SavedInDeclarationMode = false
        };
    }
}
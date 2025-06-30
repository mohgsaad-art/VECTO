using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.Vecto.UnitTests.Utils.MockInputData;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.BusAux.HVAC;

public class SanityCheckTests
{

    [TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration8, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 8/false"),
        TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration9, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 9/false"),
        TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration10, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 10/false"),

        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration1, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 1/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration2, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 2/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration3, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 3/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration4, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 4/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration5, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 5/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration6, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 6/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration7, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 7/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration8, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 8/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration9, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 9/false"),
        TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration10, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 10/false"),
        ]
    public void TestSeparateAirDistributionDuctsError(VehicleClass vehicleClass, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
    {
		var dataAdapter = new SpecificCompletedBusAuxiliaryDataAdapter();

		var axleconfig = vehicleClass == VehicleClass.Class32a ? AxleConfiguration.AxleConfig_6x2 : AxleConfiguration
			.AxleConfig_4x2;
		var primary = SSMBusAuxModelParameters.GetPrimaryVehicleMockInputData(
			VectoSimulationJobType.ConventionalVehicle, axleconfig, false, false, false,
			false, false, false, new[] { "Fixed displacement" }, "Crankshaft mounted - Discrete step clutch", AlternatorType.Conventional, false).Object;
        var completed = GetCompletedMockInputData(vehicleClass, hvacConfig, separateDucts);

        var segment = DeclarationData.CompletedBusSegments.Lookup(axleconfig.NumAxles(),
			completed.VehicleCode, completed.RegisteredClass, completed.NumberPassengerSeatsLowerDeck, completed.Height, completed.LowEntry);
		var mission = segment.Missions.FirstOrDefault();

		var runData = new VectoRunData() {
			Mission = mission,
			Loading = LoadingType.LowLoading,
			VehicleData = new VehicleData() {
				VehicleClass = VehicleClass.Class31c,
			},
			Retarder = new RetarderData() {
				Type = RetarderType.None
			}
		};

        AssertHelper.Exception<VectoException>(() => {
            var runs = dataAdapter.CreateBusAuxiliariesData(runData.Mission, primary, completed, runData);
        }, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
    }

	[
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration8, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 8/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration9, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 9/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration10, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 10/true"),

    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration1, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 1/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration2, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 2/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration3, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 3/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration4, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 4/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration5, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 5/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration6, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 6/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration7, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 7/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration8, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 8/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration9, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 9/true"),
    TestCase(VehicleClass.Class32a, BusHVACSystemConfiguration.Configuration10, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 10/true"),

    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration1, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 1/false"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration2, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 2/false"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration3, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 3/false"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration4, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 4/false"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration5, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 5/false"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration6, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 6/false"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration7, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 7/false"),

    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration1, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 1/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration2, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 2/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration3, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 3/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration4, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 4/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration5, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 5/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration6, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 6/true"),
    TestCase(VehicleClass.Class33b1, BusHVACSystemConfiguration.Configuration7, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 7/true"),
    ]
    public void TestSeparateAirDistributionDuctsOk(VehicleClass vehicleClass, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
    {
		var dataAdapter = new SpecificCompletedBusAuxiliaryDataAdapter();

		var axleconfig = vehicleClass == VehicleClass.Class32a ? AxleConfiguration.AxleConfig_6x2 : AxleConfiguration
			.AxleConfig_4x2;
        var primary = SSMBusAuxModelParameters.GetPrimaryVehicleMockInputData(
			VectoSimulationJobType.ConventionalVehicle, axleconfig, false, false, false,
			false, false, false, new[] { "Fixed displacement" }, "Crankshaft mounted - Discrete step clutch", AlternatorType.Conventional, false).Object;
		var completed = GetCompletedMockInputData(vehicleClass, hvacConfig, separateDucts);

		var segment = DeclarationData.CompletedBusSegments.Lookup(axleconfig.NumAxles(),
			completed.VehicleCode, completed.RegisteredClass, completed.NumberPassengerSeatsLowerDeck, completed.Height, completed.LowEntry);
		var mission = segment.Missions.FirstOrDefault();

        var runData = new VectoRunData() {
			Mission = mission,
			Loading = LoadingType.LowLoading,
			VehicleData = new VehicleData() {
				VehicleClass = VehicleClass.Class31c,
			},
			Retarder = new RetarderData() {
				Type = RetarderType.None
			}
		};

        //AssertHelper.Exception<VectoException>(() => {
        var runs = dataAdapter.CreateBusAuxiliariesData(runData.Mission, primary, completed, runData);
        //}, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
    }

	private IVehicleDeclarationInputData GetCompletedMockInputData(VehicleClass vehicleClass, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
	{
		var (hpDriver, hpPassenger) = GetHeatpumps(hvacConfig);

		var vehicleCode = VehicleCode.CE;
		var registrationClass = RegistrationClass.I_II;
		switch (vehicleClass) {
			case VehicleClass.Class32a:
				vehicleCode = VehicleCode.CA;
				registrationClass = RegistrationClass.II_III;
				break;
			case VehicleClass.Class33b1:
				break;
			default:
				throw new ArgumentException("unhandled vehicle class", "vehicleClass");
		}

		var completed = SSMBusAuxModelParameters.GetCompletedVehicleMockInputData(vehicleCode, registrationClass, true,
			12.SI<Meter>(), 3.SI<Meter>(), 2.55.SI<Meter>(), 30, 0, HeatPumpType.none, hpDriver, HeatPumpType.none, hpPassenger, 0.SI<Watt>(),
			false, false, null, false, hvacConfig, 0.12.SI<Meter>());
		Mock.Get(completed.Object.Components.BusAuxiliaries.HVACAux).Setup(c => c.SeparateAirDistributionDucts)
			.Returns(separateDucts);
		return completed.Object;
	}

    private (HeatPumpType, HeatPumpType) GetHeatpumps(BusHVACSystemConfiguration? hvacConfig)
	{
		var mapping = new Dictionary<BusHVACSystemConfiguration, Tuple<HeatPumpType, HeatPumpType>>() {
			{ BusHVACSystemConfiguration.Configuration1, Tuple.Create(HeatPumpType.none, HeatPumpType.none) },
			{ BusHVACSystemConfiguration.Configuration2, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.none) },
			{ BusHVACSystemConfiguration.Configuration3, Tuple.Create(HeatPumpType.none, HeatPumpType.none) },
			{ BusHVACSystemConfiguration.Configuration4, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.none) },
			{ BusHVACSystemConfiguration.Configuration5, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
			{ BusHVACSystemConfiguration.Configuration6, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
			{ BusHVACSystemConfiguration.Configuration7, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.non_R_744_3_stage) },
			{ BusHVACSystemConfiguration.Configuration8, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
			{ BusHVACSystemConfiguration.Configuration9, Tuple.Create(HeatPumpType.non_R_744_2_stage, HeatPumpType.non_R_744_3_stage) },
			{ BusHVACSystemConfiguration.Configuration10, Tuple.Create(HeatPumpType.none, HeatPumpType.non_R_744_3_stage) },
		};
		if (!hvacConfig.HasValue || !mapping.ContainsKey(hvacConfig.Value)) {
			throw new VectoException("invalid hvac configuration");
		}
		var entry = mapping[hvacConfig.Value];
		return (entry.Item1, entry.Item2);
	}

}
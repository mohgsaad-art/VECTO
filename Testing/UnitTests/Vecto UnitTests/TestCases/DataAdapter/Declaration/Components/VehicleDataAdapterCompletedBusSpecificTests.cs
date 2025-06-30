using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class VehicleDataAdapterCompletedBusSpecificTests
{
    private const MissionType IU = MissionType.Interurban;
    private const MissionType CO = MissionType.Coach;

    private const LoadingType RL = LoadingType.ReferenceLoad;

    [
        TestCase(IU, RL, 20, 5, 17, 9, 49.572, TestName = "CompleteBus PassengerCount IU generic RL"),
        TestCase(CO, RL, 20, 5, 17, 9, 38.556, TestName = "CompleteBus PassengerCount CO generic RL"),
        ]
    public void TestPassengerCountAllocationGenericCompletedBus(MissionType missionType, LoadingType loadingType, int pSeatsLower, int pStdLower, int pSeatsUpper, int pStdUpper, double expectedPassengers)
    {
        var doa = new CompletedBusGenericVehicleDataAdapter();

        var auxDoa = new GenericCompletedBusAuxiliaryDataAdapter();

        var primaryVehicle = GetPrimaryVehicleMockInputData();

        var segment = DeclarationData.PrimaryBusSegments.Lookup(
            primaryVehicle.VehicleCategory, primaryVehicle.AxleConfiguration, primaryVehicle.Articulated);

        var mission = segment.Missions.First(m => m.MissionType.Equals(missionType));
        var loading = mission.Loadings.First(l => l.Key.Equals(loadingType));

        var vehicleData = doa.CreateVehicleData(primaryVehicle, segment, mission, loading, false);

        Assert.NotNull(vehicleData.PassengerCount);
        Assert.AreEqual(expectedPassengers, vehicleData.PassengerCount.Value, 1e-3);

        var runData = new VectoRunData()
        {
            VehicleData = vehicleData,
            Mission = mission,
            Retarder = new RetarderData()
            {
                Type = RetarderType.TransmissionOutputRetarder,
            }
        };
        var busAux = auxDoa.CreateBusAuxiliariesData(mission, primaryVehicle, null, runData);
        var ssmInputs = busAux.SSMInputsCooling as ISSMDeclarationInputs;
        Assert.NotNull(ssmInputs);
        Assert.AreEqual(expectedPassengers + 1, ssmInputs.NumberOfPassengers, 1e-3); // adding driver for SSM
    }

    [
        TestCase(IU, RL, 20, 5, 17, 9, 51, TestName = "CompleteBus PassengerCount IU specific RL"),
        TestCase(CO, RL, 20, 5, 17, 9, 37, TestName = "CompleteBus PassengerCount CO specific RL"),
    ]
    public void TestPassengerCountAllocationSpecificCompletedBus(MissionType missionType, LoadingType loadingType, int pSeatsLower, int pStdLower, int pSeatsUpper, int pStdUpper, double expectedPassengers)
    {
        var doa = new CompletedBusSpecificVehicleDataAdapter();

        var auxDoa = new SpecificCompletedBusAuxiliaryDataAdapter();


        var primaryVehicle = GetPrimaryVehicleMockInputData();
        var completedVehicle = GetCompletedVehicleMockInputData(pSeatsLower, pStdLower, pSeatsUpper, pStdUpper);

        var segment = DeclarationData.CompletedBusSegments.Lookup(primaryVehicle.AxleConfiguration.NumAxles(),
            completedVehicle.VehicleCode, completedVehicle.RegisteredClass,
            completedVehicle.NumberPassengerSeatsLowerDeck, completedVehicle.Height, completedVehicle.LowEntry);

        var mission = segment.Missions.First(m => m.MissionType.Equals(missionType));
        var loading = mission.Loadings.First(l => l.Key.Equals(loadingType));

        var vehicleData = doa.CreateVehicleData(primaryVehicle, completedVehicle, segment, mission, loading);

        Assert.NotNull(vehicleData.PassengerCount);
        Assert.AreEqual(expectedPassengers, vehicleData.PassengerCount.Value, 1e-3);

        var runData = new VectoRunData()
        {
            VehicleData = vehicleData,
            Mission = mission,
            Retarder = new RetarderData()
            {
                Type = RetarderType.TransmissionOutputRetarder,
            },
            Loading = loadingType,
        };
        var busAux = auxDoa.CreateBusAuxiliariesData(mission, primaryVehicle, completedVehicle, runData);
        var ssmInputs = busAux.SSMInputsCooling as ISSMDeclarationInputs;
        Assert.NotNull(ssmInputs);
        Assert.AreEqual(expectedPassengers + 1, ssmInputs.NumberOfPassengers, 1e-3); // adding driver for SSM
    }


    private IVehicleDeclarationInputData GetCompletedVehicleMockInputData(int pSeatsLower, int pStdLower, int pSeatsUpper, int pStdUpper)
    {
        var compl = new Mock<IVehicleDeclarationInputData>();

        compl.Setup(c => c.NumberPassengerSeatsLowerDeck).Returns(pSeatsLower);
        compl.Setup(c => c.NumberPassengersStandingLowerDeck).Returns(pStdLower);
        compl.Setup(c => c.NumberPassengerSeatsUpperDeck).Returns(pSeatsUpper);
        compl.Setup(c => c.NumberPassengersStandingUpperDeck).Returns(pStdUpper);

        compl.Setup(c => c.VehicleCode).Returns(VehicleCode.CA);
        compl.Setup(c => c.RegisteredClass).Returns(RegistrationClass.II_III);
        compl.Setup(c => c.LowEntry).Returns(true);
        compl.Setup(c => c.Height).Returns(3.SI<Meter>());
        compl.Setup(c => c.Length).Returns(11.83.SI<Meter>());
        compl.Setup(c => c.Width).Returns(2.55.SI<Meter>());

        compl.Setup(c => c.EntranceHeight).Returns(0.12.SI<Meter>());
        compl.Setup(c => c.DoorDriveTechnology).Returns(ConsumerTechnology.Pneumatically);

        compl.Setup(p => p.GrossVehicleMassRating).Returns(18000.SI<Kilogram>());

        var comp = new Mock<IVehicleComponentsDeclaration>();
        compl.Setup(c => c.Components).Returns(comp.Object);

        var aux = new Mock<IBusAuxiliariesDeclarationData>();
        comp.Setup(c => c.BusAuxiliaries).Returns(aux.Object);

        var hvac = new Mock<IHVACBusAuxiliariesDeclarationData>();
        aux.Setup(a => a.HVACAux).Returns(hvac.Object);

        var ec = new Mock<IElectricConsumersDeclarationData>();
        aux.Setup(a => a.ElectricConsumers).Returns(ec.Object);

        hvac.Setup(h => h.SystemConfiguration).Returns(BusHVACSystemConfiguration.Configuration7);
        hvac.Setup(h => h.HeatPumpTypeCoolingDriverCompartment).Returns(HeatPumpType.non_R_744_2_stage);
        hvac.Setup(h => h.HeatPumpTypeHeatingDriverCompartment).Returns(HeatPumpType.none);
        hvac.Setup(h => h.HeatPumpTypeCoolingPassengerCompartment).Returns(HeatPumpType.non_R_744_4_stage);
        hvac.Setup(h => h.HeatPumpTypeHeatingPassengerCompartment).Returns(HeatPumpType.none);
        hvac.Setup(h => h.SeparateAirDistributionDucts).Returns(true);

        ec.Setup(e => e.DayrunninglightsLED).Returns(false);
        ec.Setup(e => e.PositionlightsLED).Returns(false);
        ec.Setup(e => e.BrakelightsLED).Returns(false);
        ec.Setup(e => e.InteriorLightsLED).Returns(false);
        ec.Setup(e => e.HeadlightsLED).Returns(false);

        return compl.Object;
    }

    private IVehicleDeclarationInputData GetPrimaryVehicleMockInputData()
    {
        var prim = new Mock<IVehicleDeclarationInputData>();

        prim.Setup(p => p.AxleConfiguration).Returns(AxleConfiguration.AxleConfig_4x2);
        prim.Setup(p => p.GrossVehicleMassRating).Returns(40000.SI<Kilogram>());
        prim.Setup(p => p.VehicleType).Returns(VectoSimulationJobType.ConventionalVehicle);

        var adas = new Mock<IAdvancedDriverAssistantSystemDeclarationInputData>();
        prim.Setup(v => v.ADAS).Returns(adas.Object);

        adas.Setup(a => a.EngineStopStart).Returns(false);
        adas.Setup(a => a.PredictiveCruiseControl).Returns(PredictiveCruiseControlType.None);
        adas.Setup(a => a.EcoRoll).Returns(EcoRollType.None);

        var comp = new Mock<IVehicleComponentsDeclaration>();
        prim.Setup(p => p.Components).Returns(comp.Object);

        var axls = new Mock<IAxlesDeclarationInputData>();
        comp.Setup(c => c.AxleWheels).Returns(axls.Object);

        var axl = new Mock<IAxleDeclarationInputData>();
        axls.Setup(a => a.AxlesDeclaration).Returns(new List<IAxleDeclarationInputData>() { axl.Object, axl.Object });

        var tyr = new Mock<ITyreDeclarationInputData>();
        axl.Setup(a => a.Tyre).Returns(tyr.Object);

        tyr.Setup(t => t.Dimension).Returns("315/70 R22.5");

        var aux = new Mock<IBusAuxiliariesDeclarationData>();
        comp.Setup(c => c.BusAuxiliaries).Returns(aux.Object);

        var hvac = new Mock<IHVACBusAuxiliariesDeclarationData>();
        var es = new Mock<IElectricSupplyDeclarationData>();
        var ps = new Mock<IPneumaticSupplyDeclarationData>();
        var pc = new Mock<IPneumaticConsumersDeclarationData>();

        aux.Setup(a => a.SteeringPumpTechnology).Returns(new List<string>() { "Full electric steering gear" });
        aux.Setup(a => a.FanTechnology).Returns("Electrically driven - Electronically controlled");
        aux.Setup(a => a.HVACAux).Returns(hvac.Object);
        aux.Setup(a => a.ElectricSupply).Returns(es.Object);
        aux.Setup(a => a.PneumaticSupply).Returns(ps.Object);
        aux.Setup(a => a.PneumaticConsumers).Returns(pc.Object);

        es.Setup(e => e.AlternatorTechnology).Returns(AlternatorType.Conventional);
        es.Setup(e => e.ElectricStorage).Returns(new List<IBusAuxElectricStorageDeclarationInputData>());
        es.Setup(e => e.Alternators).Returns(new List<IAlternatorDeclarationInputData>());

        ps.Setup(p => p.CompressorSize).Returns("Large Supply 2-stage");
        ps.Setup(p => p.CompressorDrive).Returns(CompressorDrive.mechanically);
        //hvac.set


        return prim.Object;
    }
}
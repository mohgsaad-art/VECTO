using Moq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.Vecto.UnitTests.Utils.MockInputData;

static internal class SSMBusAuxModelParameters
{
    public static IAuxiliaryConfig CreateBusAuxInputParameters(MissionType missionType, VehicleClass vehicleClass,
        VectoSimulationJobType vehicleType,
        VehicleCode vehicleCode, RegistrationClass registrationClass,
        AxleConfiguration axleconfiguration, bool articulated,
        bool lowEntry, Meter length,
        Meter height, Meter width, int numPassengersLowerdeck, int numPassengersUpperdeck,
        HeatPumpType hpHeatingDriver, HeatPumpType hpCoolingDriver, HeatPumpType hpHeatingPassenger,
        HeatPumpType hpCoolingPassenger, Watt auxHeaterPower,
        bool airElectricHeater, bool waterElectricHeater, bool otherElectricHeater,
        BusHVACSystemConfiguration hvacConfig, bool doubleGlazing, bool adjustableAuxHeater,
        bool separateAirdistributionDicts, bool adjustableCoolantThermostat, bool engineWasteGasHeatExchanger,
        string[] steeringpumps, string fanTech, AlternatorType alternatorTech, Meter entranceHeight,
        LoadingType loading = LoadingType.ReferenceLoad, bool essSupplyfromHVREESS = false)
    {
        var dao = new SpecificCompletedBusAuxiliaryDataAdapter();

        var segment = DeclarationData.CompletedBusSegments.Lookup(axleconfiguration.NumAxles(),
            vehicleCode, registrationClass, numPassengersLowerdeck, height, lowEntry);
        var mission = segment.Missions.FirstOrDefault();

        var primaryVehicle = GetPrimaryVehicleMockInputData(vehicleType, axleconfiguration, articulated, doubleGlazing, adjustableAuxHeater, separateAirdistributionDicts, adjustableCoolantThermostat, engineWasteGasHeatExchanger, steeringpumps, fanTech, alternatorTech, essSupplyfromHVREESS);

        var completedVehicle = GetCompletedVehicleMockInputData(vehicleCode, registrationClass, lowEntry, length, height, width, numPassengersLowerdeck, numPassengersUpperdeck, hpHeatingDriver, hpCoolingDriver, hpHeatingPassenger, hpCoolingPassenger, auxHeaterPower, airElectricHeater, waterElectricHeater, mission, otherElectricHeater, hvacConfig, entranceHeight);

        var runData = new VectoRunData()
        {
            Mission = mission,
            Loading = loading,
            VehicleData = new VehicleData()
            {
                VehicleClass = vehicleClass,
            },
            Retarder = new RetarderData()
            {
                Type = RetarderType.None
            }
        };

        var retVal = dao.CreateBusAuxiliariesData(mission, primaryVehicle.Object, completedVehicle.Object, runData);

        return retVal;
    }

    public static Mock<IVehicleDeclarationInputData> GetCompletedVehicleMockInputData(VehicleCode vehicleCode, RegistrationClass registrationClass,
        bool lowEntry, Meter length, Meter height, Meter width, int numPassengersLowerdeck, int numPassengersUpperdeck,
        HeatPumpType hpHeatingDriver, HeatPumpType hpCoolingDriver, HeatPumpType hpHeatingPassenger,
        HeatPumpType hpCoolingPassenger, Watt auxHeaterPower, bool airElectricHeater, bool waterElectricHeater,
        Mission? mission, bool otherElectricHeater, BusHVACSystemConfiguration hvacConfig, Meter entranceHeight)
    {
        var completedVehicle = new Mock<IVehicleDeclarationInputData>();
        completedVehicle.Setup(c => c.VehicleCode).Returns(vehicleCode);
        completedVehicle.Setup(c => c.Length).Returns(length);
        completedVehicle.Setup(c => c.Height).Returns(height);
        completedVehicle.Setup(c => c.Width).Returns(width);
        completedVehicle.Setup(c => c.NumberPassengerSeatsLowerDeck).Returns(numPassengersLowerdeck);
        completedVehicle.Setup(c => c.NumberPassengersStandingLowerDeck).Returns(0);
        completedVehicle.Setup(c => c.NumberPassengerSeatsUpperDeck).Returns(numPassengersUpperdeck);
        completedVehicle.Setup(c => c.NumberPassengersStandingUpperDeck).Returns(0);
        completedVehicle.Setup(c => c.RegisteredClass).Returns(registrationClass);
        completedVehicle.Setup(c => c.LowEntry).Returns(lowEntry);
        completedVehicle.Setup(c => c.EntranceHeight).Returns(entranceHeight);

        var completedComponents = new Mock<IVehicleComponentsDeclaration>();
        var completedBusAux = new Mock<IBusAuxiliariesDeclarationData>();
        var completedHVACAux = new Mock<IHVACBusAuxiliariesDeclarationData>();
        var completedElectricConsumers = new Mock<IElectricConsumersDeclarationData>();
        completedBusAux.Setup(c => c.HVACAux).Returns(completedHVACAux.Object);
        completedBusAux.Setup(c => c.ElectricConsumers).Returns(completedElectricConsumers.Object);
        completedComponents.Setup(c => c.BusAuxiliaries).Returns(completedBusAux.Object);
        completedVehicle.Setup(c => c.Components).Returns(completedComponents.Object);

        completedHVACAux.Setup(c => c.SystemConfiguration).Returns(hvacConfig);
        completedHVACAux.Setup(c => c.AuxHeaterPower).Returns(auxHeaterPower);
        completedHVACAux.Setup(c => c.SeparateAirDistributionDucts)
            .Returns(mission?.BusParameter.SeparateAirDistributionDuctsHVACCfg.Contains(hvacConfig));
        completedHVACAux.Setup(c => c.HeatPumpTypeHeatingDriverCompartment).Returns(hpHeatingDriver);
        completedHVACAux.Setup(c => c.HeatPumpTypeHeatingPassengerCompartment).Returns(hpHeatingPassenger);
        completedHVACAux.Setup(c => c.HeatPumpTypeCoolingDriverCompartment).Returns(hpCoolingDriver);
        completedHVACAux.Setup(c => c.HeatPumpTypeCoolingPassengerCompartment).Returns(hpCoolingPassenger);
        completedHVACAux.Setup(c => c.AirElectricHeater).Returns(airElectricHeater);
        completedHVACAux.Setup(c => c.WaterElectricHeater).Returns(waterElectricHeater);
        completedHVACAux.Setup(c => c.OtherHeatingTechnology).Returns(otherElectricHeater);

        completedElectricConsumers.Setup(c => c.DayrunninglightsLED).Returns(false);
        completedElectricConsumers.Setup(c => c.PositionlightsLED).Returns(false);
        completedElectricConsumers.Setup(c => c.BrakelightsLED).Returns(false);
        completedElectricConsumers.Setup(c => c.InteriorLightsLED).Returns(false);
        completedElectricConsumers.Setup(c => c.HeadlightsLED).Returns(false);
        return completedVehicle;
    }

    public static Mock<IVehicleDeclarationInputData> GetPrimaryVehicleMockInputData(VectoSimulationJobType vehicleType,
        AxleConfiguration axleconfiguration, bool articulated, bool doubleGlazing, bool adjustableAuxHeater,
        bool separateAirdistributionDicts, bool adjustableCoolantThermostat, bool engineWasteGasHeatExchanger,
        string[] steeringpumps, string fanTech, AlternatorType alternatorTech, bool essSupplyfromHVREESS)
    {
        var primaryVehicle = new Mock<IVehicleDeclarationInputData>();
        primaryVehicle.Setup(p => p.VehicleType).Returns(vehicleType);
        primaryVehicle.Setup(p => p.AxleConfiguration).Returns(axleconfiguration);
        primaryVehicle.Setup(p => p.Articulated).Returns(articulated);

        var primaryComponents = new Mock<IVehicleComponentsDeclaration>();
        var primaryBusAux = new Mock<IBusAuxiliariesDeclarationData>();
        var primaryBusAuxPS_S = new Mock<IPneumaticSupplyDeclarationData>();
        var primaryBusAuxPS_C = new Mock<IPneumaticConsumersDeclarationData>();
        var primaryBusAuxHVAC = new Mock<IHVACBusAuxiliariesDeclarationData>();
        var primaryBusAuxES = new Mock<IElectricSupplyDeclarationData>();
        primaryVehicle.Setup(p => p.Components).Returns(primaryComponents.Object);
        primaryComponents.Setup(p => p.BusAuxiliaries).Returns(primaryBusAux.Object);
        primaryBusAux.Setup(p => p.PneumaticSupply).Returns(primaryBusAuxPS_S.Object);
        primaryBusAux.Setup(p => p.PneumaticConsumers).Returns(primaryBusAuxPS_C.Object);
        primaryBusAux.Setup(p => p.HVACAux).Returns(primaryBusAuxHVAC.Object);
        primaryBusAux.Setup(p => p.ElectricSupply).Returns(primaryBusAuxES.Object);

        primaryBusAux.Setup(p => p.FanTechnology).Returns(fanTech);
        primaryBusAux.Setup(p => p.SteeringPumpTechnology).Returns(steeringpumps.ToList);

        primaryBusAuxPS_S.Setup(p => p.CompressorDrive).Returns(CompressorDrive.electrically);
        primaryBusAuxPS_S.Setup(p => p.CompressorSize).Returns("Medium Supply 2-stage");
        primaryBusAuxPS_S.Setup(p => p.SmartAirCompression).Returns(false);
        primaryBusAuxPS_S.Setup(p => p.SmartRegeneration).Returns(false);

        primaryBusAuxES.Setup(p => p.AlternatorTechnology).Returns(alternatorTech);
        primaryBusAuxES.Setup(p => p.Alternators)
            .Returns(new[] { new AlternatorInputData(28.3.SI<Volt>(), 50.SI<Ampere>()) }
                .Cast<IAlternatorDeclarationInputData>().ToList());
        if (alternatorTech == AlternatorType.Smart)
        {
            var battery = new BusAuxBatteryInputData("li-ion battery - high energy", 12.SI<Volt>(),
                (20 / 12.0).SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>());
            primaryBusAuxES.Setup(p => p.ElectricStorage).Returns(new List<IBusAuxElectricStorageDeclarationInputData>()
                { battery });
        }
        else
        {
            primaryBusAuxES.Setup(p => p.ElectricStorage)
                .Returns(new List<IBusAuxElectricStorageDeclarationInputData>());
        }

        primaryBusAuxES.Setup(p => p.ESSupplyFromHEVREESS).Returns(essSupplyfromHVREESS);

        primaryBusAuxPS_C.Setup(p => p.AdBlueDosing).Returns(ConsumerTechnology.Mechanically);
        primaryBusAuxPS_C.Setup(p => p.AirsuspensionControl).Returns(ConsumerTechnology.Electrically);
        primaryBusAuxHVAC.Setup(p => p.DoubleGlazing).Returns(doubleGlazing);
        primaryBusAuxHVAC.Setup(p => p.AdjustableAuxiliaryHeater).Returns(adjustableAuxHeater);
        primaryBusAuxHVAC.Setup(p => p.SeparateAirDistributionDucts).Returns(separateAirdistributionDicts);
        primaryBusAuxHVAC.Setup(p => p.AdjustableCoolantThermostat).Returns(adjustableCoolantThermostat);
        primaryBusAuxHVAC.Setup(p => p.EngineWasteGasHeatExchanger).Returns(engineWasteGasHeatExchanger);
        return primaryVehicle;
    }
}
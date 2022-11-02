Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Configuration
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.BusAuxiliaries
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data

Public Class Utils
    'public shared function GetElectricConsumers As IElectricalConsumerList
    '    Dim _powernetVoltage = 26.3.SI (Of Volt)
    '    Dim list As List(Of IElectricalConsumer) = New List(Of IElectricalConsumer)()
    '    list.add(New ElectricalConsumer(False, "Doors", "Doors per vehicle", 0.096339, 3))
    '    list.add(New ElectricalConsumer(True, "Veh Electronics &Engine", "Controllers,Valves etc", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment", "Radio City", 0.8, 1))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment", "Radio Intercity",
    '                                    0.8, 0))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment", "Radio/Audio Tourism", 0.8, 0))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment", "Fridge", 0.5, 0))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment", "Kitchen Standard", 0.05, 0))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment",
    '                                    "Interior lights City/ Intercity + Doorlights [Should be 1/m]", 0.7, 12))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment",
    '                                    "LED Interior lights ceiling city/Intercity + door [Should be 1/m]", 0.7, 0))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment", "Interior lights Tourism + reading [1/m]", 0.7, 0))
    '    list.add(New ElectricalConsumer(False, "Vehicle basic equipment",
    '                                    "LED Interior lights ceiling Tourism + LED reading [Should be 1/m]", 0.7, 0))
    '    list.add(New ElectricalConsumer(False, "Customer Specific Equipment", "External Displays Font/Side/Rear", 1.0, 4))
    '    list.add(New ElectricalConsumer(False, "Customer Specific Equipment",
    '                                    "Internal display per unit ( front side rear)", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Customer Specific Equipment",
    '                                    "CityBus Ref EBSF Table4 Devices ITS No Displays", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Lights", "Exterior Lights BULB", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Lights", "Day running lights LED bonus", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Lights", "Antifog rear lights LED bonus", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Lights", "Position lights LED bonus", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Lights", "Direction lights LED bonus", 1.0, 1))
    '    list.add(New ElectricalConsumer(False, "Lights", "Brake Lights LED bonus", 1.0, 1))

    '    Return New ElectricalConsumerList(list)
    'End function

    Public Shared Function GetDefaultVehicleData(optional vehicleWeight As Kilogram = Nothing) as VehicleData
        Return New VehicleData With {
            .CurbMass = If(vehicleWeight, 0.si (of Kilogram))
            }
            '.Length = 10.655.SI (Of Meter)(),
            '.Width = 2.55.SI (Of Meter)(),
            '.Height = 2.275.SI (of Meter)(),
            '.FloorType = FloorType.HighFloor,
            '.PassengerCount = 47,
            '.DoubleDecker = False
            '}
    End Function

    public shared Function CreatePneumaticAuxConfig(retarder As boolean) As IPneumaticsConsumersDemand

        return new PneumaticsConsumersDemand() with {
            .AdBlueInjection = Constants.BusAuxiliaries.PneumaticConsumersDemands.AdBlueInjection,
            .AirControlledSuspension =
                Constants.BusAuxiliaries.PneumaticConsumersDemands.AirControlledSuspension,
            .Braking = If (retarder, Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingNoRetarder,
                     Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingWithRetarder),
            .BreakingWithKneeling =
                Constants.BusAuxiliaries.PneumaticConsumersDemands.BreakingAndKneeling,
            .DeadVolBlowOuts =
                Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolBlowOuts,
            .DeadVolume = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolume,
            .NonSmartRegenFractionTotalAirDemand =
                Constants.BusAuxiliaries.PneumaticConsumersDemands.NonSmartRegenFractionTotalAirDemand,
            .SmartRegenFractionTotalAirDemand =
                Constants.BusAuxiliaries.PneumaticConsumersDemands.SmartRegenFractionTotalAirDemand,
            .OverrunUtilisationForCompressionFraction =
                Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
            .DoorOpening = Constants.BusAuxiliaries.PneumaticConsumersDemands.DoorOpening,
            .StopBrakeActuation = Constants.BusAuxiliaries.PneumaticConsumersDemands.StopBrakeActuation
            }
    End function

    public Shared Function GetAuxTestConfig(Optional retarder As boolean = true) as AuxiliaryConfig

        dim vehicleData = GetDefaultVehicleData()
        Dim heatingFuel As FuelData.Entry = New FuelData.Entry(FuelType.DieselCI, Nothing, Nothing, 1, 1, 11.8.SI(Unit.SI.Kilo.Watt.Hour.Per.kilo.Gramm).Cast _
                                                                  (Of JoulePerKilogramm), 11.8.SI(Unit.SI.Kilo.Watt.Hour.Per.kilo.Gramm).Cast _
                                                                  (Of JoulePerKilogramm))

        Dim techBenefits = New TechnologyBenefits
        For Each item As SSMTechnology  In DeclarationData.BusAuxiliaries.SSMTechnologyList
            techBenefits.CValueVariation +=  item.RaisedFloorC
            techBenefits.HValueVariation += item.RaisedFloorH
            techBenefits.VCValueVariation += If(item.ActiveVC, item.RaisedFloorV, 0)
            techBenefits.VHValueVariation += if (item.ActiveVH, item.RaisedFloorV, 0)
            techBenefits.VVValueVariation += If (item.ActiveVV, item.RaisedFloorV, 0)

        Next

        '.AverageCurrentDemandInclBaseLoad = 60.631777385159026.SI(Of Ampere),
        '.AverageCurrentDemandWithoutBaseLoad = 35.631777385159026.SI(of Ampere), 
        Dim retval = New AuxiliaryConfig() With {
                .ElectricalUserInputsConfig = New ElectricsUserInputsConfig() With {
                .ResultCardIdle = New DummyResultCard(), ' New ResultCard(New List(Of SmartResult)()),
                .ResultCardTraction = New DummyResultCard(), 'New ResultCard(New List(Of SmartResult)()),
                .ResultCardOverrun = New DummyResultCard(), 'New ResultCard(New List(Of SmartResult)()),
                .AlternatorMap = AlternatorReader.ReadMap("TestFiles\testAlternatormap.aalt"),
                .DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
                .PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
                .AlternatorType = AlternatorType.Conventional
                },
                .PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(retarder),
                .PneumaticUserInputsConfig = New PneumaticUserInputsConfig() With {
                .CompressorGearRatio = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearRatio,
                .CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
                .AdBlueDosing = ConsumerTechnology.Pneumatically,
                .AirSuspensionControl = ConsumerTechnology.Mechanically,
                .Doors = ConsumerTechnology.Pneumatically,
                .KneelingHeight = 70.SI(Unit.SI.Milli.Meter).Cast (Of Meter), 
                .SmartAirCompression = False,
                .SmartRegeneration = False 
                },
                .SSMInputs = New SSMInputs(Nothing, heatingFuel) With {
                .Technologies = techBenefits,
                .BusFloorType = FloorType.HighFloor,
                .BusSurfaceArea = 0.SI(Of SquareMeter),
                .BusVolume = 0.SI(Of CubicMeter),
                .BusWindowSurface = 0.SI(of SquareMeter),
                .UValue = 3.SI(Of WattPerKelvinSquareMeter),
                .COP = 3.5,
                .VentilationRate = 20.SI(Unit.SI.Per.Hour).Cast (Of PerSecond),
                .VentilationRateHeating = 20.SI(Unit.SI.Per.Hour).Cast (Of PerSecond),
                .DefaultConditions =
                New EnvironmentalConditionMapEntry(25.0.DegCelsiusToKelvin(), 400.SI (Of WattPerSquareMeter), 1.0),
                .EnvironmentalConditionsMap = DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions,
                .HeatingBoundaryTemperature = 18.0.DegCelsiusToKelvin(),
                .CoolingBoundaryTemperature = 23.0.DegCelsiusToKelvin(),
                .SpecificVentilationPower = 0.56.SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast (Of JoulePerCubicMeter),
                .HVACCompressorType = HeatPumpType.non_R_744_2_stage, 
                .HVACMaxCoolingPower = 18.si(Unit.SI.kilo.watt).Cast (of Watt),
                .AuxHeaterEfficiency =  0.84,
                .FuelFiredHeaterPower = 30.SI(Unit.SI.kilo.watt).Cast (Of Watt),
                .FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
                .CoolantHeatTransferredToAirCabinHeater =
                Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
                .MaxPossibleBenefitFromTechnologyList =
                Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList
                },
                .VehicleData = vehicleData,
                .Actuations = New Actuations() With {
                    .Braking = 153,
                    .ParkBrakeAndDoors = 24,
                    .Kneeling = 25,
                    .CycleTime = 1000.SI(of Second)()
                 }
                }
        Return retVal
    End Function
End Class
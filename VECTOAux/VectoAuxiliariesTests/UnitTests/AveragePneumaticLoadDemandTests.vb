
Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.BusAuxiliaries
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.SimulationComponent.Data

Namespace UnitTests
    <TestFixture>
    Public Class M3_AveragePneumaticLoadDemandTests
        Private _pneumaticUserInputsConfig As IPneumaticUserInputsConfig
        Private _pneumaticAuxillariesConfig As IPneumaticsConsumersDemand
        Private _actuationsMap As IActuationsMap
        Private _pneumaticsCompressorFlowRateMap As ICompressorMap
        Private _vehicleMassKG As Single = 16500
        Private _cycleName As String = "Urban"
        Private _cycleDurationMinutes As Single = 51.9
        Private _totalAirDemand As Single

        Private _actuationsMapPath As String = "Testfiles\testPneumaticActuationsMap_GOODMAP.apac"
        Private _compressorMapPath As String = "Testfiles\testCompressorMap.acmp"

        Private _defaultInputConfig As PneumaticUserInputsConfig
        Private _Signals As ISignals = New Signals

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        'Constructors
        Public Sub New()

            initialise()
        End Sub

        Private Sub initialise()

            _defaultInputConfig = New PneumaticUserInputsConfig()

            _defaultInputConfig.CompressorGearRatio = 1.3
            _defaultInputConfig.CompressorGearEfficiency = 0.8
            _defaultInputConfig.SmartRegeneration = True
            '_defaultInputConfig.RetarderBrake = True
            _defaultInputConfig.KneelingHeight = 80.SI(Unit.SI.Milli.Meter).Cast (of Meter)
            _defaultInputConfig.AirSuspensionControl = ConsumerTechnology.Electrically ' "Electrically"
            _defaultInputConfig.AdBlueDosing = ConsumerTechnology.Pneumatically ' "Pneumatic"
            _defaultInputConfig.Doors = ConsumerTechnology.Pneumatically ' "Pneumatic"
            _defaultInputConfig.SmartAirCompression = True

            '_Signals.TotalCycleTimeSeconds = 3114

            _Signals.EngineSpeed = 3000.RPMtoRad()
        End Sub


        <TestCase()>
        Public Sub CreateNewtest()

            Dim psUserInputsConfig = New PneumaticUserInputsConfig()
            psUserInputsConfig.AirSuspensionControl = ConsumerTechnology.Mechanically  ' "Mechanically"
            psUserInputsConfig.Doors = ConsumerTechnology.Pneumatically  '"Pneumatic"
            psUserInputsConfig.AdBlueDosing = ConsumerTechnology.Pneumatically  ' "Pneumatic"

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
                                        
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                        

            Dim auxCfg = Utils.GetAuxTestConfig()
            'auxCfg.VectoInputs = New VectoInputs() 

            Dim _
                target As _
                    New M03Impl(auxCfg, psCompressorMap, auxCfg.Actuations, _Signals)


            Assert.IsNotNull(target)
        End Sub

        <TestCase()>
        Public Sub AverageLoadValueUsingDefaultAuxValuesTest()

            initialise()

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                       

            'psCompressorMap.Initialise()

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)

           
            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Double = 7947.684
            Dim actual As NormLiter = target.AverageAirConsumed * auxConfig.Actuations.CycleTime

            Assert.AreEqual(expected, actual.Value(), 0.000001)
        End Sub

        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_AveragePowerAtTheCrankTest()

            initialise()

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap =CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                       

            'psCompressorMap.Initialise()

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Single = 5832.091
            Dim actual As Watt = target.GetAveragePowerDemandAtCrankFromPneumatics()

            Assert.AreEqual(expected, actual.Value(), 0.001)
        End Sub


        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_AveragePowerAtTheCrank_0_80EFTest()

            initialise()

            _defaultInputConfig.CompressorGearEfficiency = 0.8

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                        

            'psCompressorMap.Initialise()
            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Single = 5832.091

            Assert.AreEqual(expected, target.GetAveragePowerDemandAtCrankFromPneumatics().Value(), 0.001)
        End Sub

        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRateTest()

            initialise()

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                        

            'psCompressorMap.Initialise()

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)


            Dim expected As Double = 7947.55127/ auxConfig.Actuations.CycleTime.Value()
                                    
            ' _Signals.TotalCycleTimeSeconds

            Assert.AreEqual(expected, target.AverageAirConsumed().Value(), 0.001)
        End Sub

        'SmartRegeneration = False
        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_SmartRegenOffTest()

            initialise()

            _defaultInputConfig.SmartRegeneration = False

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                       

            'psCompressorMap.Initialise()

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Double = 8863.378/auxConfig.Actuations.CycleTime.Value() _
            ' _Signals.TotalCycleTimeSeconds

            Assert.AreEqual(expected, target.AverageAirConsumed().Value(), 0.001)
        End Sub

        'RetarderBrake = False
        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_RetarderBrakeOffTest()

            initialise()

            '_defaultInputConfig.RetarderBrake = False
           
            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.None)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                       

            'psCompressorMap.Initialise()

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Double = 8541.45/auxConfig.Actuations.CycleTime.Value() _
            ' _Signals.TotalCycleTimeSeconds


            Assert.AreEqual(expected, target.AverageAirConsumed().Value(), 0.001)
        End Sub

        'KneelingHeightMilimeters = 100
        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_Kneeling100mmTest()

            initialise()

            _defaultInputConfig.KneelingHeight = 100.SI(Unit.si.Milli.Meter).Cast (Of Meter)

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                       

            'psCompressorMap.Initialise()
            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Double = 8557.524/ auxConfig.Actuations.CycleTime.Value() ' _Signals.TotalCycleTimeSeconds

            Assert.AreEqual(expected, target.AverageAirConsumed().Value(), 0.001)
        End Sub

        'AirSuspensionControl = "mechanically"
        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_AirSuspension_mechanicallyTest()

            initialise()

            _defaultInputConfig.AirSuspensionControl = ConsumerTechnology.Mechanically

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                        

            'psCompressorMap.Initialise()

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Double = 8726.1840/auxConfig.Actuations.CycleTime.Value() _
            ' _Signals.TotalCycleTimeSeconds

            Assert.AreEqual(expected, target.AverageAirConsumed().Value(), 0.001)
        End Sub

        'AdBlueDosing = "electric"
        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_AdBlueDosing_electric_Test()

            initialise()

            _defaultInputConfig.AdBlueDosing = ConsumerTechnology.Pneumatically

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")
                                        

            'psCompressorMap.Initialise()

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Double = 7947.68457/auxConfig.Actuations.CycleTime.Value() _
            ' _Signals.TotalCycleTimeSeconds

            Assert.AreEqual(expected, target.AverageAirConsumed().Value(), 0.001)
        End Sub

        'Doors = "Electric"
        <Test()>
        Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_Doors_electric_Test()

            initialise()

            _defaultInputConfig.Doors = ConsumerTechnology.Electrically

            Dim psAuxConfig = New DeclarationDataAdapterCompletedBusGeneric().CreatePneumaticAuxConfig(RetarderType.LossesIncludedInTransmission)
            Dim psCompressorMap = CompressorMapReader.ReadFile(_compressorMapPath, 1.0, "")

            Dim auxConfig As IAuxiliaryConfig = GetAuxConfig(psAuxConfig)


            'psCompressorMap.Initialise()

            Dim _
                target As _
                    New M03Impl(auxConfig, psCompressorMap, auxConfig.Actuations, _Signals)

            Dim expected As Double = 6880.88428/auxConfig.Actuations.CycleTime.Value() _
            ' _Signals.TotalCycleTimeSeconds

            Assert.AreEqual(expected, target.AverageAirConsumed().Value(), 0.001)
        End Sub

        Private Function GetAuxConfig(psAuxConfig As IPneumaticsConsumersDemand) As IAuxiliaryConfig

            Return New AuxiliaryConfig() with {
                .PneumaticAuxillariesConfig = psAuxConfig,
                .PneumaticUserInputsConfig = _defaultInputConfig,
                .VehicleData = New VehicleData() with {
                    .CurbMass = _vehicleMassKG.SI(of Kilogram)
                    },
                .Actuations = New Actuations() With {
                .Braking = 153,
                .ParkBrakeAndDoors = 75,
                .Kneeling = 25,
                .CycleTime = 3114.SI(Of Second)    
            }
                }
        End Function
    End Class
End Namespace



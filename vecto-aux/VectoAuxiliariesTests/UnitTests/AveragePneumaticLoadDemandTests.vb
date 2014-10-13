
Imports VectoAuxiliaries.Pneumatics
Imports NUnit.Framework
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries


Namespace UnitTests

    <TestFixture>
    Public Class AveragePneumaticLoadDemandTests



        Private _pneumaticUserInputsConfig As IPneumaticUserInputsConfig
        Private _pneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig
        Private _pneumaticsActuationsMap As IPneumaticActuationsMAP
        Private _pneumaticsCompressorFlowRateMap As ICompressorMap
        Private _vehicleMassKG As Single = 16500
        Private _cycleName As String = "Urban"
        Private _cycleDurationMinutes As Single = 51.9
        Private _totalAirDemand As Single

        Private _actuationsMapPath As String = "Testfiles\testPneumaticActuationsMap_GOODMAP.csv"
        Private _compressorMapPath As String = "Testfiles\testCompressorMap.csv"

        Private _defaultInputConfig As IPneumaticUserInputsConfig








        'Constructors
        Public Sub New()

            initialise()

        End Sub

        Private Sub initialise()

        _defaultInputConfig = New PneumaticUserInputsConfig()

        _defaultInputConfig.CompressorGearEfficiency = 0.8
        _defaultInputConfig.SmartRegeneration = True
        _defaultInputConfig.RetarderBrake = True
        _defaultInputConfig.KneelingHeightMilimeters = 80
        _defaultInputConfig.AirSuspensionControl = "electrically"
        _defaultInputConfig.AdBlueDosing = "pneumatic"
        _defaultInputConfig.Doors = "pneumatic"
        _defaultInputConfig.SmartAirCompression = True


        End Sub


        <Test>
         Public Sub CreateNewtest()

         Dim psUserInputsConfig = CType(New PneumaticUserInputsConfig(), IPneumaticUserInputsConfig)
         psUserInputsConfig.AirSuspensionControl = "mechanically"
         psUserInputsConfig.Doors = "pneumatic"
         psUserInputsConfig.AdBlueDosing = "pneumatic"


         Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
         Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
         Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

          psCompressorMap.Initialise()
          Dim target As New AveragePneumaticLoadDemand(psUserInputsConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)



          Assert.IsNotNull(target)

        End Sub

        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValuesTest()

         initialise()

         Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
         Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
         Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

         psCompressorMap.Initialise()

         Dim target As New AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

         Dim expected As Single = 7947.684
         Dim actual As Single = target.TotalAirDemand()

         Assert.AreEqual(expected, actual)


         End Sub

        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_AveragePowerAtTheCrankTest()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            Dim target As New AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 0.0319030322
            Dim actual As Single = target.GetAveragePowerDemandAtCrankFromPneumatics()

            Assert.AreEqual(expected, actual)

         End Sub

        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRateTest()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            Dim target As New AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 7947.684
            Dim actual As Single = target.TotalAirConsumedPerCycle()

            Assert.AreEqual(expected, actual)

         End Sub


        'CompressorGearEfficiency = 0.99

        'SmartRegeneration = False

        'RetarderBrake = False

        'KneelingHeightMilimeters = 100

        'AirSuspensionControl = "mechanically"

        'AdBlueDosing = "electric"

        'Doors = "electric"

        'SmartAirCompression = false


    End Class



End Namespace



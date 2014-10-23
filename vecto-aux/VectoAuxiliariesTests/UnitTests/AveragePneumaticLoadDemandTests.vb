
Imports VectoAuxiliaries.Pneumatics
Imports NUnit.Framework
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries


Namespace UnitTests

    <TestFixture>
    Public Class M3_AveragePneumaticLoadDemandTests



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
        _defaultInputConfig.KneelingHeightMillimeters = 80
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
          Dim target As New M3_AveragePneumaticLoadDemand(psUserInputsConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)



          Assert.IsNotNull(target)

        End Sub

        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValuesTest()

         initialise()

         Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
         Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
         Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

         psCompressorMap.Initialise()

         Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

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

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 0.0319030322
            Dim actual As Single = target.GetAveragePowerDemandAtCrankFromPneumatics()

            Assert.AreEqual(expected, actual)

         End Sub

        'CompressorGearEfficiency = 0.99
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_AveragePowerAtTheCrank_0_99EFTest()

         initialise()

         _defaultInputConfig.CompressorGearEfficiency = 0.99

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 0.025780227
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

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 7947.684
            Dim actual As Single = target.TotalAirConsumedPerCycle()

            Assert.AreEqual(expected, actual)

         End Sub

        'SmartRegeneration = False
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_SmartRegenOffTest()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            _defaultInputConfig.SmartRegeneration = False

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 8863.295
            Dim actual As Single = target.TotalAirConsumedPerCycle()

            Assert.AreEqual(expected, actual)

         End Sub

        'RetarderBrake = False
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_RetarderBrakeOffTest()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            _defaultInputConfig.RetarderBrake = False

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 8541.45

            Dim actual As Single = Math.Round(target.TotalAirConsumedPerCycle(), 2)

            Assert.AreEqual(expected, actual)

         End Sub

        'KneelingHeightMilimeters = 100
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_Kneeling100mmTest()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            _defaultInputConfig.KneelingHeightMillimeters = 100

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 8557.52

            Dim actual As Single = Math.Round(target.TotalAirConsumedPerCycle(), 2)

            Assert.AreEqual(expected, actual)

         End Sub

        'AirSuspensionControl = "mechanically"
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_AirSuspension_mechanicallyTest()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            _defaultInputConfig.AirSuspensionControl = "mechanically"

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 8726.18

            Dim actual As Single = Math.Round(target.TotalAirConsumedPerCycle(), 2)

            Assert.AreEqual(expected, actual)

         End Sub

        'AdBlueDosing = "electric"
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_AdBlueDosing_electric_Test()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            _defaultInputConfig.AdBlueDosing = "electric"

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 6712.46

            Dim actual As Single = Math.Round(target.TotalAirConsumedPerCycle(), 2)

            Assert.AreEqual(expected, actual)

         End Sub

        'Doors = "electric"
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_Doors_electric_Test()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            _defaultInputConfig.Doors = "electric"

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _cycleDurationMinutes)

            Dim expected As Single = 6880.88

            Dim actual As Single = Math.Round(target.TotalAirConsumedPerCycle(), 2)

            Assert.AreEqual(expected, actual)

         End Sub



    End Class



End Namespace




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
        Private _Signals As ISignals = New Signals


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
        _defaultInputConfig.AirSuspensionControl = "Electrically"
        _defaultInputConfig.AdBlueDosing = "Pneumatic"
        _defaultInputConfig.Doors = "Pneumatic"
        _defaultInputConfig.SmartAirCompression = True

        _Signals.TotalCycleTimeSeconds=3114



        End Sub


        <Test>
         Public Sub CreateNewtest()

         Dim psUserInputsConfig = CType(New PneumaticUserInputsConfig(), IPneumaticUserInputsConfig)
         psUserInputsConfig.AirSuspensionControl = "Mechanically"
         psUserInputsConfig.Doors = "Pneumatic"
         psUserInputsConfig.AdBlueDosing = "Pneumatic"


         Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
         Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
         Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)


          psCompressorMap.Initialise()
          Dim target As New M3_AveragePneumaticLoadDemand(psUserInputsConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)



          Assert.IsNotNull(target)

        End Sub

        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValuesTest()

         initialise()

         Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
         Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
         Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

         psCompressorMap.Initialise()

         Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

         Dim expected As Single = 7664.94
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

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 0.512801051
            Dim actual As Single = target.GetAveragePowerDemandAtCrankFromPneumatics()

            Assert.AreEqual(expected, actual)

         End Sub


        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_AveragePowerAtTheCrank_0_80EFTest()

         initialise()

         _defaultInputConfig.CompressorGearEfficiency = 0.8

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 0.512801051
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

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 7664.94
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

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 8545.207
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

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

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

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 8274.78

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

            _defaultInputConfig.AirSuspensionControl = "Mechanically"

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 8443.44

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

            _defaultInputConfig.AdBlueDosing = "Pneumatic"

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 7664.94

            Dim actual As Single = Math.Round(target.TotalAirConsumedPerCycle(), 2)

            Assert.AreEqual(expected, actual)

         End Sub

        'Doors = "Electric"
        <Test()>
         Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_Doors_electric_Test()

         initialise()

            Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
            Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
            Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

            psCompressorMap.Initialise()

            _defaultInputConfig.Doors = "Electric"

            Dim target As New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap, _vehicleMassKG, "Urban", _signals)

            Dim expected As Single = 6598.14

            Dim actual As Single = Math.Round(target.TotalAirConsumedPerCycle(), 2)

            Assert.AreEqual(expected, actual)

         End Sub



    End Class



End Namespace



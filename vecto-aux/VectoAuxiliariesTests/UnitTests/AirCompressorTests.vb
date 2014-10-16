Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries


Namespace UnitTests

    <TestFixture()>
    Public Class M4_AirCompressorTests
#Region "Test Constants"
        Private Const GoodEfficiency As Single = 1
        Private Const TooLowEfficiency As Single = 0.1
        Private Const TooHighEfficiency As Single = 1.0

        Private Const GoodRatio As Single = 1
        Private Const TooLowRatio As Single = 1.0
        Private Const TooHighRatio As Single = 6.0

#End Region


Private _signals As ISignals = New Signals


#Region "Factory Methods"

        Private Function GetNonFailingCompressorMapMock() As ICompressorMap
            Return New CompressorMapMock(False)
        End Function

        Private Function GetFailingCompressorMapMock() As ICompressorMap
            Return New CompressorMapMock(True)
        End Function

        Private Function GetGoodCompressor() As M4_AirCompressor
            Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
            Dim target As M4_AirCompressor = New M4_AirCompressor(map, GoodRatio, GoodEfficiency)
            Return target
        End Function
#End Region

        <Test()>
        Public Sub CreateNewJustPathTest()
            Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
            _signals.EngineSpeed=100
            Dim target As M4_AirCompressor = New M4_AirCompressor(map,_signals)
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub CreateNewAllParametersTest()
            Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
            Dim target As M4_AirCompressor = New M4_AirCompressor(map, GoodRatio, GoodEfficiency)
            Assert.IsNotNull(target)
        End Sub


        <Test()>
        Public Sub InitialiseTest()
            Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
            _signals.EngineSpeed=100
            Dim target As M4_AirCompressor = New M4_AirCompressor(map,_signals)
            Assert.IsTrue(target.Initialise())
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialiseInvalidMapTest()
            Dim map As ICompressorMap = GetFailingCompressorMapMock()
            _signals.EngineSpeed=100
            Dim target As M4_AirCompressor = New M4_AirCompressor(map,_signals)
            target.Initialise()
        End Sub

        <Test()>
        Public Sub GetEfficiencyTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim target = comp.PulleyGearEfficiency
            Assert.AreEqual(target, GoodEfficiency)
        End Sub

        <Test()>
        Public Sub SetEfficiencyTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim target As Single = 0.3
            comp.PulleyGearEfficiency = target
            Dim actual As Single = comp.PulleyGearEfficiency
            Assert.AreEqual(target, actual)
        End Sub

        <TestCase(TooLowEfficiency)> _
        <TestCase(TooHighEfficiency)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub SetEfficiencyOutOfRangeTest(ByVal efficiency As Single)
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            comp.PulleyGearEfficiency = efficiency
        End Sub


        <Test()>
        Public Sub GetRatioTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim target = comp.PulleyGearRatio
            Assert.AreEqual(target, GoodRatio)
        End Sub

        <Test()>
        Public Sub SetRatioTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim target As Single = 3
            comp.PulleyGearRatio = target
            Dim actual As Single = comp.PulleyGearRatio
            Assert.AreEqual(target, actual)
        End Sub

        <TestCase(TooLowRatio)> _
        <TestCase(TooHighRatio)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub SetRatioOutOfRangeTest(ByVal ratio As Single)
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            comp.PulleyGearRatio = ratio
        End Sub

        <Test()>
        Public Sub GetCompressorFlowRateTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim expected As Single = 2.0
            Dim actual = comp.GetFlowRate()
            Assert.AreEqual(expected, actual)
        End Sub

        <Test()>
        Public Sub GetPowerCompressorOffTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim expected As Single = 5.0
            Dim actual = comp.GetPowerCompressorOff()
            Assert.AreEqual(expected, actual)
        End Sub


        <Test()>
        Public Sub GetPowerCompressorOnTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim expected As Single = 8.0
            Dim actual = comp.GetPowerCompressorOn()
            Assert.AreEqual(expected, actual)
        End Sub


        <Test()>
        Public Sub GetPowerDifferenceTest()
            Dim comp As M4_AirCompressor = GetGoodCompressor()
            Dim expected As Single = 3.0
            Dim actual = comp.GetPowerDifference()
            Assert.AreEqual(expected, actual)
        End Sub


        <Test>
        Public Sub GetAveragePowerDemandPerCompressorUnitFlowRate()

            Dim comp As M4_AirCompressor = GetGoodCompressor()

            Dim expected As Single = 0.01
            Dim actual As Single = comp.GetAveragePowerDemandPerCompressorUnitFlowRate
            Assert.AreEqual(actual, expected)


        End Sub



    End Class
End Namespace
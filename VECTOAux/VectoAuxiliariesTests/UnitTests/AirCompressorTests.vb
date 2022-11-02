Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports VectoAuxiliariesTests.Mocks



Namespace UnitTests
	<TestFixture()>
	Public Class M4_AirCompressorTests

#Region "Test Constants"

		Private Const GoodEfficiency As Single = 0.7
		Private Const TooLowEfficiency As Single = -1
		Private Const TooHighEfficiency As Single = 1.1

		Private Const GoodRatio As Single = 1
		Private Const TooLowRatio As Single = -1
		Private Const TooHighRatio As Single = 12

#End Region


		Private _signals As ISignals = New Signals


		Public Sub New()
			_signals.EngineSpeed = 100.RPMtoRad()
		End Sub


#Region "Factory Methods"

		Private Function GetNonFailingCompressorMapMock() As ICompressorMap
			Return New CompressorMapMock(False)
		End Function

		Private Function GetFailingCompressorMapMock() As ICompressorMap
			Return New CompressorMapMock(True)
		End Function

		Private Function GetGoodCompressor() As IM4_AirCompressor
			Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
			Dim target As IM4_AirCompressor = New M04Impl(map, GoodRatio, GoodEfficiency, _signals)
			Return target
		End Function

#End Region

		<TestCase()>
		Public Sub CreateNewJustPathTest()
			Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
			_signals.EngineSpeed = 100.RPMtoRad()
			Dim target As IM4_AirCompressor = New M04Impl(map, 2, 0.8, _signals)
			Assert.IsNotNull(target)
		End Sub

		<TestCase()>
		Public Sub CreateNewAllParametersTest()
			Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
			Dim target As IM4_AirCompressor = New M04Impl(map, GoodRatio, GoodEfficiency, _signals)
			Assert.IsNotNull(target)
		End Sub

        ''
        ''@QUAM 20191017: These tests are not really useful. FailingMockCompressor always throwns exeption on initialize, M04Impl.Initialize only initializes compressor map...
        ''
		'<TestCase()>
		'Public Sub InitialiseTest()
		'	Dim map As ICompressorMap = GetNonFailingCompressorMapMock()
		'	_signals.EngineSpeed = 100.RPMtoRad()
		'	Dim target As IM4_AirCompressor = New M04Impl(map, 2, 0.8, _signals)
		'	Assert.IsTrue(target.Initialise())
		'End Sub

  '      <TestCase()>
  '      Public Sub InitialiseInvalidMapTest()
		'	Dim map As ICompressorMap = GetFailingCompressorMapMock()
		'	_signals.EngineSpeed = 100.RPMtoRad()
		'	Dim target As IM4_AirCompressor = New M04Impl(map, 2, 0.8, _signals)
  '          Assert.That(Sub() target.Initialise(), Throws.InstanceOf(Of System.ArgumentException))
  '      End Sub

		<TestCase()>
		Public Sub GetEfficiencyTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim target = comp.PulleyGearEfficiency
			Assert.AreEqual(target, GoodEfficiency)
		End Sub

		<TestCase()>
		Public Sub SetEfficiencyTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim target As Double = 0.3
			comp.PulleyGearEfficiency = target
			Dim actual As Double = comp.PulleyGearEfficiency
			Assert.AreEqual(target, actual)
		End Sub

		<TestCase(TooLowEfficiency)>
        <TestCase(TooHighEfficiency)>
        Public Sub SetEfficiencyOutOfRangeTest(ByVal efficiency As Single)
            Dim comp As IM4_AirCompressor = GetGoodCompressor()

            Assert.That(Sub() comp.PulleyGearEfficiency = efficiency, Throws.InstanceOf(Of ArgumentException))
        End Sub


        <TestCase()>
		Public Sub GetRatioTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim target = comp.PulleyGearRatio
			Assert.AreEqual(target, GoodRatio)
		End Sub

		<TestCase()>
		Public Sub SetRatioTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim target As Double = 3
			comp.PulleyGearRatio = target
			Dim actual As Double = comp.PulleyGearRatio
			Assert.AreEqual(target, actual)
		End Sub

		<TestCase(TooLowRatio)>
        <TestCase(TooHighRatio)>
        Public Sub SetRatioOutOfRangeTest(ByVal ratio As Single)
            Dim comp As IM4_AirCompressor = GetGoodCompressor()

            Assert.That(Sub() comp.PulleyGearRatio = ratio, Throws.InstanceOf(Of ArgumentException))
        End Sub

        <TestCase()>
		Public Sub GetCompressorFlowRateTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim expected As Double = 0.0333333351
			Dim actual = comp.GetFlowRate()
			Assert.AreEqual(expected, actual.Value(), 0.00000001)
		End Sub

		<TestCase()>
		Public Sub GetPowerCompressorOffTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim expected As Double = 5.0
			Dim actual = comp.GetPowerCompressorOff()
			Assert.AreEqual(expected, actual.Value(), 0.00000001)
		End Sub


		<TestCase()>
		Public Sub GetPowerCompressorOnTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim expected As Double = 8.0
			Dim actual = comp.GetPowerCompressorOn()
			Assert.AreEqual(expected, actual.Value(), 0.00000001)
		End Sub


		<TestCase()>
		Public Sub GetPowerDifferenceTest()
			Dim comp As IM4_AirCompressor = GetGoodCompressor()
			Dim expected As Double = 3.0
			Dim actual = comp.GetPowerDifference()
			Assert.AreEqual(expected, actual.Value(), 0.00000001)
		End Sub


		<TestCase>
		Public Sub GetAveragePowerDemandPerCompressorUnitFlowRate()

			Dim comp As IM4_AirCompressor = GetGoodCompressor()

			Dim expected As Double = 0.01.SI(Unit.SI.Watt.Per.Liter.Per.Hour).Value()
			Dim actual As SI = comp.GetAveragePowerDemandPerCompressorUnitFlowRate
			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub
	End Class
End Namespace
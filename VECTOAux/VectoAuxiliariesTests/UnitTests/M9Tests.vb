
Imports NUnit.Framework
Imports Moq
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Namespace UnitTests
	<TestFixture()>
	Public Class M9Tests
		<Test()> _
		<TestCase(50, 50, 400, 200, 100, 1200, 50, 0, 0, 0.5, False, 50.0F, 0, 0.181108221, 0.180887148)> _
		<TestCase(50, 50, 400, 200, 100, 1200, 50, 1, 0, 0.5, False, 50.0F, 0, 0.181108221, 0.180887148)> _
		<TestCase(50, 50, 400, 200, 100, 1200, 50, 0, 1, 0.5, False, 50.0F, 0, 0.181108221, 0.180887148)> _
		<TestCase(50, 50, 400, 200, 100, 1200, 50, 1, 1, 0.5, False, 50.0F, 0, 0.181108221, 0.180887148)> _
		<TestCase(50, 50, 400, 200, 100, 1200, 50, 1, 1, 0.5, True, 0, 0, 0, 0)>
		Public Sub ValuesInOutTests(IP1 As Double,
									IP2 As Double,
									IP3 As Double,
									IP4 As Double,
									IP5 As Double,
									IP6 As Double,
									IP7 As Double,
									IP8 As Double,
									IP9 As Double,
									IP10 As Double,
									IP11 As Boolean,
									AG1 As Double,
									AG2 As Double,
									AG3 As Double,
									AG4 As Double)

			Dim m1Mock As New Mock(Of IM1_AverageHVACLoadDemand)
			Dim m4Mock As New Mock(Of IM4_AirCompressor)
			Dim m6Mock As New Mock(Of IM6)
			Dim m8Mock As New Mock(Of IM8)
			Dim fMapMock As New MockFuel50PC()
			Dim sgnlsMock As New Mock(Of ISignals)
			Dim psac As New Mock(Of IPneumaticsConsumersDemand)

			m6Mock.Setup(Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC).Returns(IP1.SI(Of Watt))
			m1Mock.Setup(Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicals).Returns(IP2.SI(Of Watt))
			m4Mock.Setup(Function(x) x.GetPowerCompressorOn).Returns(IP3.SI(Of Watt))
			m4Mock.Setup(Function(x) x.GetPowerCompressorOff).Returns(IP4.SI(Of Watt))
			sgnlsMock.Setup(Function(x) x.EngineDrivelineTorque).Returns(IP5.SI(Of NewtonMeter))
			sgnlsMock.Setup(Function(x) x.EngineSpeed).Returns(IP6.RPMtoRad())
			sgnlsMock.Setup(Function(x) x.PreExistingAuxPower).Returns(0.SI(Of Watt))
			m4Mock.Setup(Function(x) x.GetFlowRate).Returns(IP7.SI(Of NormLiterPerSecond))
			m6Mock.Setup(Function(x) x.OverrunFlag).Returns(IP8 <> 0)
			m8Mock.Setup(Function(x) x.CompressorFlag).Returns(IP9 <> 0)
			psac.Setup(Function(x) x.OverrunUtilisationForCompressionFraction).Returns(IP10)
			sgnlsMock.Setup(Function(x) x.EngineStopped).Returns(IP11)

			Dim _
				target As _
					New M09Impl(m1Mock.Object, m4Mock.Object, m6Mock.Object, m8Mock.Object, fMapMock, psac.Object, sgnlsMock.Object)

			target.CycleStep(1.SI(Of Second))

			Assert.AreEqual(target.LitresOfAirCompressorOnContinually.Value(), AG1, 0.000001)
			Assert.AreEqual(target.LitresOfAirCompressorOnOnlyInOverrun.Value(), AG2, 0.000001)
            		Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOnContinuously.Value(), AG3.SI(Unit.SI.Gramm).Value(), 0.000001)
            		Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOffContinuously.Value(), AG4.SI(Unit.SI.Gramm).Value(), 0.000001)
		End Sub


		<Test()> _
		<TestCase(50, 50, 400, 200, 100, 1200, 50, 0, 0, 0.5F, False, 50.0F, 0, 0.181108221F, 0.180887148F)>
		Public Sub NEGATIVEINTERPADJUSTMENTValuesInOutTests(IP1 As Double,
															IP2 As Double,
															IP3 As Double,
															IP4 As Double,
															IP5 As Double,
															IP6 As Double,
															IP7 As Double,
															IP8 As Double,
															IP9 As Double,
															IP10 As Double,
															IP11 As Boolean,
															AG1 As Double,
															AG2 As Double,
															AG3 As Double,
															AG4 As Double)

			Dim m1Mock As New Mock(Of IM1_AverageHVACLoadDemand)
			Dim m4Mock As New Mock(Of IM4_AirCompressor)
			Dim m6Mock As New Mock(Of IM6)
			Dim m8Mock As New Mock(Of IM8)
			Dim fMapMock As New Mock(Of IFuelConsumptionMap)
			Dim sgnlsMock As New Mock(Of ISignals)
			Dim psac As New Mock(Of IPneumaticsConsumersDemand)

			fMapMock.Setup(Function(x) x.GetFuelConsumptionValue(1.SI(Of NewtonMeter), 1.RPMtoRad())).Returns(
				(-1 / 1000).SI(Of KilogramPerSecond)())
			m6Mock.Setup(Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC).Returns(IP1.SI(Of Watt))
			m1Mock.Setup(Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicals).Returns(IP2.SI(Of Watt))
			m4Mock.Setup(Function(x) x.GetPowerCompressorOn).Returns(IP3.SI(Of Watt))
			m4Mock.Setup(Function(x) x.GetPowerCompressorOff).Returns(IP4.SI(Of Watt))
			sgnlsMock.Setup(Function(x) x.EngineDrivelineTorque).Returns(IP5.SI(Of NewtonMeter))
			sgnlsMock.Setup(Function(x) x.EngineSpeed).Returns(IP6.RPMtoRad())
			sgnlsMock.Setup(Function(x) x.PreExistingAuxPower).Returns(0.SI(Of Watt))
			m4Mock.Setup(Function(x) x.GetFlowRate).Returns(IP7.SI(Of NormLiterPerSecond))
			m6Mock.Setup(Function(x) x.OverrunFlag).Returns(IP8 <> 0)
			m8Mock.Setup(Function(x) x.CompressorFlag).Returns(IP9 <> 0)
			psac.Setup(Function(x) x.OverrunUtilisationForCompressionFraction).Returns(IP10)
			sgnlsMock.Setup(Function(x) x.EngineStopped).Returns(IP11)

			Dim _
				target As _
					New M09Impl(m1Mock.Object, m4Mock.Object, m6Mock.Object, m8Mock.Object, fMapMock.Object, psac.Object, sgnlsMock.Object)

			target.CycleStep(1.SI(Of Second))

			Assert.AreEqual(target.LitresOfAirCompressorOnContinually.Value(), AG1, 0.000001)
			Assert.AreEqual(target.LitresOfAirCompressorOnOnlyInOverrun.Value(), AG2, 0.000001)
			Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOnContinuously.Value(), 0, 0.000001)
			Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOffContinuously.Value(), 0, 0.000001)
		End Sub
	End Class
End Namespace



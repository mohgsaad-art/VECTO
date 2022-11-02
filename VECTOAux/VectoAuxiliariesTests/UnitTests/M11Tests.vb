
Imports NUnit.Framework
Imports Moq
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Namespace UnitTests
	<TestFixture()>
	Public Class M11Tests
		<Test()> _
		<TestCase(0, 50, 60, 70, 80, 90, 1500, False, 0, 50, 60, 0.2182501F, 0.2182059, 60)> _
		<TestCase(1, 50, 60, 70, 80, 90, 1500, False, 50, 50, 60, 0.2182501F, 0.2182059, 60)> _
		<TestCase(1, 50, 60, 70, 80, 90, 1500, True, 0, 0, 60, 0, 0, 0)>
		Public Sub InputOutputValues(IP1 As Double,
									IP2 As Double,
									IP3 As Double,
									IP4 As Double,
									IP5 As Double,
									IP6 As Double,
									IP7 As Double,
									IP8 As Boolean,
									OUT1 As Double,
									OUT2 As Double,
									OUT3 As Double,
									OUT4 As Double,
									OUT5 As Double,
									OUT6 As Double)

			'Arrange

			Dim m1Mock As New Mock(Of IM1_AverageHVACLoadDemand)
			Dim m3Mock As New Mock(Of IM3_AveragePneumaticLoadDemand)


			Dim m6Mock As New Mock(Of IM6)
			Dim m8Mock As New Mock(Of IM8)
			Dim sgnlsMock As New Mock(Of ISignals)
			Dim fmap As New MockFuel50PC


			m6Mock.Setup(Function(x) x.OverrunFlag).Returns(IP1 <> 0)
			m8Mock.Setup(Function(x) x.SmartElectricalAlternatorPowerGenAtCrank).Returns(IP2.SI(Of Watt))
			m6Mock.Setup(Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC).Returns(IP3.SI(Of Watt))
			sgnlsMock.Setup(Function(x) x.EngineDrivelineTorque).Returns(IP4.SI(Of NewtonMeter))
			sgnlsMock.Setup(Function(x) x.PreExistingAuxPower).Returns(0.SI(Of Watt))
			m3Mock.Setup(Function(x) x.GetAveragePowerDemandAtCrankFromPneumatics).Returns(IP5.SI(Of Watt))
			m1Mock.Setup(Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicals).Returns(IP6.SI(Of Watt))
			sgnlsMock.Setup(Function(x) x.EngineSpeed).Returns(IP7.RPMtoRad())
			sgnlsMock.Setup(Function(x) x.EngineStopped).Returns(IP8)


			'Act
			Dim target = New M11Impl(m1Mock.Object, m3Mock.Object, m6Mock.Object, m8Mock.Object, fmap, sgnlsMock.Object) _
			',m3Mock.Object,m6Mock.Object,m8Mock.Object,fmap,sgnlsMock.Object)

			'Add Current Calculation to Internal Aggregates ( Accesseed by public output properties which are external interface )
			target.CycleStep(1.SI(Of Second))


			'Assert
			Assert.AreEqual(target.SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly.Value(), OUT1, 0.00001)
			Assert.AreEqual(target.SmartElectricalTotalCycleEletricalEnergyGenerated.Value(), OUT2, 0.00001)
			Assert.AreEqual(target.TotalCycleElectricalDemand.Value(), OUT3, 0.00001)
		    	Assert.AreEqual(target.TotalCycleFuelConsumptionSmartElectricalLoad.ConvertToGramm(), OUT4, 0.00001)
		    	Assert.AreEqual(target.TotalCycleFuelConsumptionZeroElectricalLoad.Value(), OUT5.SI(Unit.SI.Gramm).Value(), 0.00001)
			Assert.AreEqual(target.StopStartSensitiveTotalCycleElectricalDemand.Value(), OUT6, 0.00001)
		End Sub


		<Test()> _
		<TestCase(1, 50, 60, 70, 80, 90, 1500, True, 0, 0, 60, 0, 0, 0)>
		Public Sub UnaggregatedInputOutputValues(IP1 As Double,
												IP2 As Double,
												IP3 As Double,
												IP4 As Double,
												IP5 As Double,
												IP6 As Double,
												IP7 As Double,
												IP8 As Boolean,
												OUT1 As Double,
												OUT2 As Double,
												OUT3 As Double,
												OUT4 As Double,
												OUT5 As Double,
												OUT6 As Double)

			'Arrange

			Dim m1Mock As New Mock(Of IM1_AverageHVACLoadDemand)
			Dim m3Mock As New Mock(Of IM3_AveragePneumaticLoadDemand)


			Dim m6Mock As New Mock(Of IM6)
			Dim m8Mock As New Mock(Of IM8)
			Dim sgnlsMock As New Mock(Of ISignals)
			Dim fmap As New MockFuel50PC

			m6Mock.Setup(Function(x) x.OverrunFlag).Returns(IP1 <> 0)
			m8Mock.Setup(Function(x) x.SmartElectricalAlternatorPowerGenAtCrank).Returns(IP2.SI(Of Watt))
			m6Mock.Setup(Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC).Returns(IP3.SI(Of Watt))
			sgnlsMock.Setup(Function(x) x.EngineDrivelineTorque).Returns(IP4.SI(Of NewtonMeter))
			m3Mock.Setup(Function(x) x.GetAveragePowerDemandAtCrankFromPneumatics).Returns(IP5.SI(Of Watt))
			m1Mock.Setup(Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicals).Returns(IP6.SI(Of Watt))
			sgnlsMock.Setup(Function(x) x.EngineSpeed).Returns(IP7.RPMtoRad())
			sgnlsMock.Setup(Function(x) x.EngineStopped).Returns(IP8)


			'Act
			Dim target = New M11Impl(m1Mock.Object, m3Mock.Object, m6Mock.Object, m8Mock.Object, fmap, sgnlsMock.Object) _
			',m3Mock.Object,m6Mock.Object,m8Mock.Object,fmap,sgnlsMock.Object)


			target.CycleStep(1.SI(Of Second))

			'Assert
			Assert.AreEqual(target.SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly.Value(), OUT1, 0.00001)
			Assert.AreEqual(target.SmartElectricalTotalCycleEletricalEnergyGenerated.Value(), OUT2, 0.00001)
			Assert.AreEqual(target.TotalCycleElectricalDemand.Value(), OUT3, 0.00001)
			Assert.AreEqual(target.TotalCycleFuelConsumptionSmartElectricalLoad.Value(), OUT4, 0.00001)
			Assert.AreEqual(target.TotalCycleFuelConsumptionZeroElectricalLoad.Value(), OUT5, 0.00001)
			Assert.AreEqual(target.StopStartSensitiveTotalCycleElectricalDemand.Value(), OUT6, 0.00001)
		End Sub
	End Class
End Namespace



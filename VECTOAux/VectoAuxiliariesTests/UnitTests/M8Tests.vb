
Imports NUnit.Framework
Imports Moq
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules

Namespace UnitTests
	<TestFixture()>
	Public Class M8Tests

        Private Function GetAuxConfigDummy(smartElectrics As Boolean, smartPneumatics As Boolean) As IAuxiliaryConfig
            Dim auxCfg As New Mock(Of IAuxiliaryConfig)
            Dim elecCfg = New Mock(Of IElectricsUserInputsConfig)
            elecCfg.Setup(Function(x) x.AlternatorType()).Returns(If(smartElectrics, AlternatorType.Smart, AlternatorType.Conventional))
            Dim psconfig = New Mock(Of IPneumaticUserInputsConfig)
            psconfig.Setup(Function(x) x.SmartAirCompression).Returns(smartPneumatics)
            auxCfg.Setup(Function(x) x.ElectricalUserInputsConfig).Returns(elecCfg.Object)
            auxCfg.Setup(Function(x) x.PneumaticUserInputsConfig).Returns(psconfig.Object)

            return auxCfg.Object
        End Function

		<TestCase()>
		Public Sub CreateInstanceTest()

			'Arrange
			Dim m1MOCK = New Mock(Of IM1_AverageHVACLoadDemand)()
			Dim m6Mock = New Mock(Of IM6)()
			Dim m7MOCK = New Mock(Of IM7)()
			Dim sigsMock = New Mock(Of ISignals)()

			'Act
			Dim target As IM8 = New M08Impl(GetAuxConfigDummy(false, False), m1MOCK.Object, m6Mock.Object, m7MOCK.Object, sigsMock.Object)

			'Assert
			Assert.IsNotNull(target)
		End Sub


		<Test()> _
		<TestCase(10, 20, 30, 40, 50, 60, 70, 0, 1, False, False, 140, 40, True)> _
		<TestCase(10, 20, 30, 40, 50, 60, 70, 1, 0, False, True, 120, 40, True)> _
		<TestCase(10, 20, 30, 40, 50, 60, 70, 1, 0, True, False, 120, 20, False)> _
		<TestCase(10, 20, 30, 40, 50, 60, 70, 0, 1, True, True, 60, 20, False)>
		Public Sub ValueInOutTest(IP1 As Double,
								IP2 As Double,
								IP3 As Double,
								IP4 As Double,
								IP5 As Double,
								IP6 As Double,
								IP7 As Double,
								IP8 As Integer,
								IP9 As Integer,
								IP10 As Boolean,
								IP11 As Boolean,
								OUT1 As Double,
								OUT2 As Double,
								OUT3 As Boolean)

			'Arrange
			Dim m1MOCK = New Mock(Of IM1_AverageHVACLoadDemand)()
			Dim m6Mock = New Mock(Of IM6)()
			Dim m7MOCK = New Mock(Of IM7)()
			Dim sigsMock = New Mock(Of ISignals)()

			m1MOCK.Setup(Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicals).Returns(IP1.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank()).Returns(IP2.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank()).Returns(IP3.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartElectricalOnlyAuxAltPowerGenAtCrank()).Returns(IP4.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank).Returns(IP5.SI(Of Watt))
			m6Mock.Setup(Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC).Returns(IP6.SI(Of Watt))
			m6Mock.Setup(Function(x) x.AveragePowerDemandAtCrankFromPneumatics).Returns(IP7.SI(Of Watt))
			m6Mock.Setup(Function(x) x.SmartElecAndPneumaticsCompressorFlag).Returns(IP8 <> 0)
			m6Mock.Setup(Function(x) x.SmartPneumaticsOnlyCompressorFlag).Returns(IP9 <> 0)
			'sigsMock.Setup(Function(x) x.SmartPneumatics).Returns(IP10)
			'sigsMock.Setup(Function(x) x.SmartElectrics).Returns(IP11)

			'Act
			Dim target As IM8 = New M08Impl(GetAuxConfigDummy(IP11, IP10), m1MOCK.Object, m6Mock.Object, m7MOCK.Object, sigsMock.Object)

			'Assert
			Assert.AreEqual(OUT1, target.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries.Value(), 0.001)
			Assert.AreEqual(OUT2, target.SmartElectricalAlternatorPowerGenAtCrank.Value(), 0.001)
			Assert.AreEqual(OUT3, target.CompressorFlag)
		End Sub

		<Test()> _
		<TestCase(10, 20, 30, 40, 50, 60, 70, 0, 1, False, False, True, 0, 40, True)> _
		<TestCase(10, 20, 30, 40, 50, 60, 70, 0, 1, False, False, False, 140, 40, True)>
		Public Sub TestIdlingFunction(IP1 As Double,
									IP2 As Double,
									IP3 As Double,
									IP4 As Double,
									IP5 As Double,
									IP6 As Double,
									IP7 As Double,
									IP8 As Integer,
									IP9 As Integer,
									IP10 As Boolean,
									IP11 As Boolean,
									IP12 As Boolean,
									OUT1 As Double,
									OUT2 As Double,
									OUT3 As Boolean)

			'Arrange
			Dim m1MOCK = New Mock(Of IM1_AverageHVACLoadDemand)()
			Dim m6Mock = New Mock(Of IM6)()
			Dim m7MOCK = New Mock(Of IM7)()
			Dim sigsMock = New Mock(Of ISignals)()

			m1MOCK.Setup(Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicals).Returns(IP1.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank()).Returns(IP2.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank()).Returns(IP3.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartElectricalOnlyAuxAltPowerGenAtCrank()).Returns(IP4.SI(Of Watt))
			m7MOCK.Setup(Function(x) x.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank).Returns(IP5.SI(Of Watt))
			m6Mock.Setup(Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC).Returns(IP6.SI(Of Watt))
			m6Mock.Setup(Function(x) x.AveragePowerDemandAtCrankFromPneumatics).Returns(IP7.SI(Of Watt))
			m6Mock.Setup(Function(x) x.SmartElecAndPneumaticsCompressorFlag).Returns(IP8 <> 0)
			m6Mock.Setup(Function(x) x.SmartPneumaticsOnlyCompressorFlag).Returns(IP9 <> 0)
			'sigsMock.Setup(Function(x) x.SmartPneumatics).Returns(IP10)
			'sigsMock.Setup(Function(x) x.SmartElectrics).Returns(IP11)
			sigsMock.Setup(Function(x) x.EngineStopped).Returns(IP12)


			'Act
			Dim target As IM8 = New M08Impl(GetAuxConfigDummy(IP11, IP10), m1MOCK.Object, m6Mock.Object, m7MOCK.Object, sigsMock.Object)

			'Assert
			Assert.AreEqual(OUT1, target.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries.Value(), 0.001)
			Assert.AreEqual(OUT2, target.SmartElectricalAlternatorPowerGenAtCrank.Value(), 0.001)
			Assert.AreEqual(OUT3, target.CompressorFlag)
		End Sub
	End Class
End Namespace



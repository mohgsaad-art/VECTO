
Imports NUnit.Framework
Imports Moq
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Namespace UnitTests
	<TestFixture()>
	Public Class M12Tests
		<Test()> _
		<TestCase(2, 8, 6, 5, 8, 6, 4.67379665, 4.5)>
		Public Sub InputOutputValues(IP2 As Double,
									IP3 As Double,
									IP4 As Double,
									IP5 As Double,
									IP6 As Double,
									IP7 As Double,
									OUT1 As Double,
									OUT2 As Double
									)

			'Arrange
			Dim M10Mock As New Mock(Of IM10)
			Dim m11Mock As New Mock(Of IM11)

			Dim sgnlsMock As New Mock(Of ISignals)


			'sgnlsMock.Setup(Function(x) x.StoredEnergyEfficiency).Returns(0.935)

			m11Mock.Setup(Function(x) x.TotalCycleFuelConsumptionZeroElectricalLoad).Returns((IP2 / 1000).SI(Of Kilogram))
			m11Mock.Setup(Function(x) x.SmartElectricalTotalCycleEletricalEnergyGenerated).Returns(IP3.SI(Of Joule))
			m11Mock.Setup(Function(x) x.TotalCycleFuelConsumptionSmartElectricalLoad).Returns((IP4 / 1000).SI(Of Kilogram))
			m11Mock.Setup(Function(x) x.TotalCycleElectricalDemand).Returns(IP5.SI(Of Joule))
			m11Mock.Setup(Function(x) x.StopStartSensitiveTotalCycleElectricalDemand).Returns(IP6.SI(Of Joule))
			M10Mock.Setup(Function(x) x.AverageLoadsFuelConsumptionInterpolatedForPneumatics).Returns((IP7 / 1000).SI(Of Kilogram))

			'Act
			Dim target = New M12Impl(M10Mock.Object, m11Mock.Object)

			'Assert
			Assert.AreEqual(target.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand().Value(),
                            OUT1.SI(Unit.SI.Gramm).Value(), 0.001)
            Assert.AreEqual(target.BaseFuelConsumptionWithTrueAuxiliaryLoads().Value(), OUT2.SI(Unit.SI.Gramm).Value(), 0.001)
		End Sub
	End Class
End Namespace



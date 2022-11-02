
Imports NUnit.Framework
Imports Moq
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Namespace UnitTests
	<TestFixture()>
	Public Class M10Test
		<Test()> _
		<TestCase(8, 8, 0, 2, 4, 2, 5, 5.75, 3.5)>
		Public Sub InterpolationTests(x1 As Double, y1 As Double, x2 As Double, y2 As Double, x3 As Double, y3 As Double,
									xTAir As Double, out1 As Double, out2 As Double)

			Dim m3 As New Mock(Of IM3_AveragePneumaticLoadDemand)
			Dim m9 As New Mock(Of IM9)
			Dim signals As New Signals() ' Not required , here for expansion only.

			m3.Setup(Function(x) x.AverageAirConsumed).Returns(xTAir.SI(Of NormLiterPerSecond))
			m9.Setup(Function(x) x.LitresOfAirCompressorOnContinually).Returns(x1.SI(Of NormLiter))
			m9.Setup(Function(x) x.TotalCycleFuelConsumptionCompressorOnContinuously).Returns((y1 / 1000).SI(Of Kilogram))
			'x2 is not an output of m9, an is allways zero, but to keep in line with schematic, is represented anyway although it is a constant.
			m9.Setup(Function(x) x.TotalCycleFuelConsumptionCompressorOffContinuously).Returns((y2 / 1000).SI(Of Kilogram))
			m9.Setup(Function(x) x.LitresOfAirCompressorOnOnlyInOverrun).Returns(x3.SI(Of NormLiter))
			m9.Setup(Function(x) x.TotalCycleFuelConsumptionCompressorOffContinuously).Returns((y3 / 1000).SI(Of Kilogram))


			Dim target As IM10 = New M10Impl(m3.Object, m9.Object)

			target.CycleStep(1.SI(Of Second))

			Assert.AreEqual(out1.SI(Unit.SI.Gramm).Value(), target.AverageLoadsFuelConsumptionInterpolatedForPneumatics.Value(), 0.001)
			Assert.AreEqual(out2.SI(Unit.SI.Gramm).Value(), target.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand.Value(), 0.001)
		End Sub
	End Class
End Namespace



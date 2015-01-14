Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests

<TestFixture()> 
Public Class M10Test



<Test()> _
<TestCase(8,8,0,2,4,2,5,5.75f,3.5f)> _
Public Sub InterpolationTests( x1 As Single ,y1 As Single ,x2 As single,y2 As single,x3 As single,y3 As single,xTAir As single, out1 As single, out2 as single)

Dim m3 As new Mock( Of IM3_AveragePneumaticLoadDemand)
Dim m9 As new Mock( Of IM9)
Dim signals As New Signals() ' Not required , here for expansion only.

m3.Setup( Function(x) x.AverageAirConsumedPerSecondLitre).Returns( xTAir )
m9.Setup( Function(x) x.LitresOfAirCompressorOnContinually).Returns(x1)
m9.Setup( Function(x) x.TotalCycleFuelConsumptionCompressorOnContinuously).Returns(y1)
'x2 is not an output of m9, an is allways zero, but to keep in line with schematic, is represented anyway although it is a constant.
m9.Setup( Function(x) x.TotalCycleFuelConsumptionCompressorOffContinuously).Returns( y2)
m9.Setup( Function(x) x.LitresOfAirCompressorOnOnlyInOverrun).Returns( x3)
m9.Setup( Function(x) x.TotalCycleFuelConsumptionCompressorOffContinuously).Returns(y3)


Dim target As IM10 = New M10(m3.Object,m9.Object,Signals)

target.CycleStep(1)

Assert.AreEqual(out1, target.AverageLoadsFuelConsumptionInterpolatedForPneumatics )
Assert.AreEqual(out2, target.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand )




End Sub


End Class

End Namespace




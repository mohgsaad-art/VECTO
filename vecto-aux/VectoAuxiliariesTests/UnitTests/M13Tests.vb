Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests
<TestFixture()> _
Public Class M13Tests

Private Const FUEL_DENSITY_percm3 As Single = 0.835


<Test()> _
<TestCase(50,	60,	70,	FALSE,	FALSE,	100, 72287.5f , 86.57185629f )> _
<TestCase(50,	60,	70,	FALSE,	TRUE,	100, 72277.5f , 86.55988024f )> _
<TestCase(50,	60,	70,	TRUE,	FALSE,	100, 72297.5f , 86.58383234f )> _
<TestCase(50,	60,	70,	TRUE,	TRUE,	100, 72287.5f , 86.57185629f )> _
Public Sub InputOutputValues( IP1  As single,
                              IP2  As single, 
                              IP3  As single,
                              IP4  As Boolean, 
                              IP5  As Boolean, 
                              IP6  As single,
                              OUT1 As single,
                              OUT2 As single)

'Arrange
Dim m1  As New Mock(Of  IM1_AverageHVACLoadDemand)
Dim m10 As New Mock(Of IM10)
Dim M12 As New Mock(Of IM12)
Dim Signals As New Mock(Of ISignals)

m12.Setup     ( Function(x) x.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand) .Returns( IP1 )
m10.Setup     ( Function(x) x.BaseFuelConsumptionWithAverageAuxiliaryLoads)                    .Returns( IP2 )
m10.Setup     ( Function(x) x.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand)   .Returns( IP3 )
Signals.Setup ( Function(x) x.SmartPneumatics)                                                 .Returns( IP4 )
Signals.Setup ( Function(x) x.SmartElectrics)                                                  .Returns( IP5 )
m1.Setup      ( Function(x) x.HVACFuelingLitresPerHour)                                        .Returns( IP6 )
Signals.Setup ( Function(x) x.TotalCycleTimeSeconds)                                           .Returns( 3114)


'Act
 Dim target  = New M13( m1.Object, m10.Object, M12.Object, Signals.Object)
       
'Assert
Assert.AreEqual( OUT1, target.TotalCycleFuelConsumptionGrams )
Assert.AreEqual( OUT2,target.TotalCycleFuelConsumptionLitres )
End Sub

End Class

End Namespace




Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests

<TestFixture()> _
Public Class M8Tests

<Test()> _
Public Sub CreateInstanceTest()

 'Arrange
 Dim m1MOCK     = New Mock(Of IM1_AverageHVACLoadDemand )()
 Dim m6Mock     = new Mock(Of IM6 )()
 Dim m7MOCK     = New Mock(Of IM7 )()
 Dim sigsMock   = New Mock(Of ISignals)()
 
 'Act
 Dim target As IM8 = New M8(m1MOCK.Object, m6Mock.Object, m7MOCK.Object, sigsMock.Object)

 'Assert
 Assert.IsNotNull( target )

End Sub


<Test()> _
<TestCase(10,20,30,40,50,60,70,0,1,FALSE,FALSE,140 ,40 ,1 )> _
<TestCase(10,20,30,40,50,60,70,1,0,FALSE,TRUE ,120 ,40 ,1 )> _
<TestCase(10,20,30,40,50,60,70,1,0,TRUE,FALSE ,120 ,20 ,0 )> _
<TestCase(10,20,30,40,50,60,70,0,1,TRUE,TRUE  , 60 ,20 ,0 )> _
Public sub ValueInOutTest( IP1 As Single,
                           IP2 As Single,
                           IP3 As Single,
                           IP4 As Single,
                           IP5 As Single,
                           IP6 As Single,
                           IP7 As Single,
                           IP8 as Integer,
                           IP9 as Integer,
                           IP10 As Boolean,
                           IP11 As Boolean,
                           OUT1 As Single,
                           OUT2 As Single,
                           OUT3 As Single)

 'Arrange
 Dim m1MOCK     = New Mock(Of IM1_AverageHVACLoadDemand )()
 Dim m6Mock     = new Mock(Of IM6 )()
 Dim m7MOCK     = New Mock(Of IM7 )()
 Dim sigsMock   = New Mock(Of ISignals)()

 m1MOCK.Setup   ( Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicalsWatts    ).Returns(IP1 )
 m7MOCK.Setup   ( Function(x) x.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank     ).Returns(IP2 )
 m7MOCK.Setup   ( Function(x) x.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank ).Returns(IP3 )
 m7MOCK.Setup   ( Function(x) x.SmartElectricalOnlyAuxAltPowerGenAtCrank             ).Returns(IP4 )
 m7MOCK.Setup   ( Function(x) x.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank          ).Returns(IP5 )
 m6Mock.Setup   ( Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC            ).Returns(IP6 )
 m6Mock.Setup   ( Function(x) x.AveragePowerDemandAtCrankFromPneumatics              ).Returns(IP7 )
 m6Mock.Setup   ( Function(x) x.SmartElecAndPneumaticsCompressorFlag                 ).Returns(IP8 )
 m6Mock.Setup   ( Function(x) x.SmartPneumaticsOnlyCompressorFlag                    ).Returns(IP9 )
 sigsMock.Setup ( Function(x) x.SmartPneumatics                                      ).Returns(IP10)
 sigsMock.Setup ( Function(x) x.SmartElectrics                                       ).Returns(IP11)

 'Act
 Dim target As IM8 = New M8(m1MOCK.Object, m6Mock.Object, m7MOCK.Object, sigsMock.Object)

'Assert
 Assert.AreEqual(OUT1, target.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries )
 Assert.AreEqual(OUT2, target.SmartElectricalAlternatorPowerGenAtCrank                  )
 Assert.AreEqual(OUT3, target.CompressorFlag                                            )

End Sub

<Test()> _
<TestCase(10,20,30,40,50,60,70,0,1,FALSE,FALSE,TRUE,0 ,40 ,1 )> _
<TestCase(10,20,30,40,50,60,70,0,1,FALSE,FALSE,FALSE,140 ,40 ,1 )> _
Public sub TestIdlingFunction( IP1 As Single,
                               IP2 As Single,
                               IP3 As Single,
                               IP4 As Single,
                               IP5 As Single,
                               IP6 As Single,
                               IP7 As Single,
                               IP8 as Integer,
                               IP9 as Integer,
                               IP10 As Boolean,
                               IP11 As Boolean,
                               IP12 As Boolean,
                               OUT1 As Single,
                               OUT2 As Single,
                               OUT3 As Single)

 'Arrange
 Dim m1MOCK     = New Mock(Of IM1_AverageHVACLoadDemand )()
 Dim m6Mock     = new Mock(Of IM6 )()
 Dim m7MOCK     = New Mock(Of IM7 )()
 Dim sigsMock   = New Mock(Of ISignals)()

 m1MOCK.Setup   ( Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicalsWatts    ).Returns(IP1 )
 m7MOCK.Setup   ( Function(x) x.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank     ).Returns(IP2 )
 m7MOCK.Setup   ( Function(x) x.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank ).Returns(IP3 )
 m7MOCK.Setup   ( Function(x) x.SmartElectricalOnlyAuxAltPowerGenAtCrank             ).Returns(IP4 )
 m7MOCK.Setup   ( Function(x) x.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank          ).Returns(IP5 )
 m6Mock.Setup   ( Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC            ).Returns(IP6 )
 m6Mock.Setup   ( Function(x) x.AveragePowerDemandAtCrankFromPneumatics              ).Returns(IP7 )
 m6Mock.Setup   ( Function(x) x.SmartElecAndPneumaticsCompressorFlag                 ).Returns(IP8 )
 m6Mock.Setup   ( Function(x) x.SmartPneumaticsOnlyCompressorFlag                    ).Returns(IP9 )
 sigsMock.Setup ( Function(x) x.SmartPneumatics                                      ).Returns(IP10)
 sigsMock.Setup ( Function(x) x.SmartElectrics                                       ).Returns(IP11)
 sigsMock.Setup ( Function(x) x.EngineStopped                                        ).Returns(IP12)


 'Act
 Dim target As IM8 = New M8(m1MOCK.Object, m6Mock.Object, m7MOCK.Object, sigsMock.Object)

'Assert
 Assert.AreEqual(OUT1, target.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries )
 Assert.AreEqual(OUT2, target.SmartElectricalAlternatorPowerGenAtCrank                  )
 Assert.AreEqual(OUT3, target.CompressorFlag                                            )

End Sub



End Class

End Namespace




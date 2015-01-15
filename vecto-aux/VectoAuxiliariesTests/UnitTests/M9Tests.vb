Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests
<TestFixture()>
Public Class M9Tests

<Test()> _
<TestCase(50,50,400,200,100,1200,50,0,0,0.5f,false,50f,0,  0.18110822f,0.18088715f)> _
<TestCase(50,50,400,200,100,1200,50,1,0,0.5f,false,50f,0,  0.18110822f,0.18088715f)> _
<TestCase(50,50,400,200,100,1200,50,0,1,0.5f,false,50f,0,  0.18110822f,0.18088715f)> _
<TestCase(50,50,400,200,100,1200,50,1,1,0.5f,false,50f,25, 0.18110822f,0.18088715f)> _               
<TestCase(50,50,400,200,100,1200,50,1,1,0.5f,true,   0, 0, 0          ,0          )> _
Public Sub ValuesInOutTests(IP1  As Single,
                            IP2  As Single,
                            IP3  As Single,
                            IP4  As Single,
                            IP5  As Single,
                            IP6  As Single,
                            IP7  As Single,
                            IP8  As Single,
                            IP9  As Single,
                            IP10 As Single, 
                            IP11 As Boolean,
                            AG1  As Single,
                            AG2  As Single,
                            AG3  As Single,
                            AG4  As Single)

      Dim m1Mock    As New Mock(Of IM1_AverageHVACLoadDemand)
      Dim m4Mock    As New Mock(Of IM4_AirCompressor)
      Dim m6Mock    As New Mock(Of IM6)
      Dim m8Mock    As New Mock(Of IM8)
      Dim fMapMock  As New MockFuel50PC()
      Dim sgnlsMock As New Mock(Of ISignals)
      Dim psac      As New Mock(Of IPneumaticsAuxilliariesConfig )

      m6Mock.Setup   ( Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC )        .Returns(IP1)
      m1Mock.Setup   ( Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicalsWatts ).Returns(IP2)
      m4Mock.Setup   ( Function(x) x.GetPowerCompressorOn)                              .Returns(IP3)
      m4Mock.Setup   ( Function(x) x.GetPowerCompressorOff)                             .Returns(IP4)
      sgnlsMock.Setup( Function(x) x.EngineDrivelineTorque)                             .Returns(IP5)
      sgnlsMock.Setup( Function(x) x.EngineSpeed)                                       .Returns(IP6)
      m4Mock.Setup   ( Function(x) x.GetFlowRate)                                       .Returns(IP7)
      m6Mock.Setup   ( Function(x) x.OverrunFlag)                                       .Returns(IP8)
      m8Mock.Setup   ( Function(x) x.CompressorFlag)                                    .Returns(IP9)
      psac.Setup     ( Function(x) x.OverrunUtilisationForCompressionFraction)          .Returns(IP10)
      sgnlsMock.Setup( Function(x) x.EngineStopped)                                     .Returns(IP11)

      Dim target As New M9( m1Mock.Object, m4Mock.Object, m6Mock.Object,m8Mock.Object,fMapMock,psac.Object,sgnlsMock.Object)

      target.CycleStep(1)

      Assert.AreEqual(target.LitresOfAirCompressorOnContinually                , AG1 )
      Assert.AreEqual(target.LitresOfAirCompressorOnOnlyInOverrun              , AG2 )
      Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOnContinuously , CType(Math.round(AG3,7),Single) )
      Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOffContinuously, AG4)


End Sub



<Test()> _
<TestCase(50,50,400,200,100,1200,50,0,0,0.5f,false,50f,0,  0.18110822f,0.18088715f)> _
Public Sub NEGATIVEINTERPADJUSTMENTValuesInOutTests(IP1  As Single,
                            IP2  As Single,
                            IP3  As Single,
                            IP4  As Single,
                            IP5  As Single,
                            IP6  As Single,
                            IP7  As Single,
                            IP8  As Single,
                            IP9  As Single,
                            IP10 As Single, 
                            IP11 As Boolean,
                            AG1  As Single,
                            AG2  As Single,
                            AG3  As Single,
                            AG4  As Single)

      Dim m1Mock    As New Mock(Of IM1_AverageHVACLoadDemand)
      Dim m4Mock    As New Mock(Of IM4_AirCompressor)
      Dim m6Mock    As New Mock(Of IM6)
      Dim m8Mock    As New Mock(Of IM8)
      Dim fMapMock  As New Mock(Of IFuelMap )
      Dim sgnlsMock As New Mock(Of ISignals)
      Dim psac      As New Mock(Of IPneumaticsAuxilliariesConfig )

      fMapMock.Setup ( Function(x) x.fFCdelaunay_Intp(1,1)                             ).Returns(-1)
      m6Mock.Setup   ( Function(x) x.AvgPowerDemandAtCrankFromElectricsIncHVAC )        .Returns(IP1)
      m1Mock.Setup   ( Function(x) x.AveragePowerDemandAtCrankFromHVACMechanicalsWatts ).Returns(IP2)
      m4Mock.Setup   ( Function(x) x.GetPowerCompressorOn)                              .Returns(IP3)
      m4Mock.Setup   ( Function(x) x.GetPowerCompressorOff)                             .Returns(IP4)
      sgnlsMock.Setup( Function(x) x.EngineDrivelineTorque)                             .Returns(IP5)
      sgnlsMock.Setup( Function(x) x.EngineSpeed)                                       .Returns(IP6)
      m4Mock.Setup   ( Function(x) x.GetFlowRate)                                       .Returns(IP7)
      m6Mock.Setup   ( Function(x) x.OverrunFlag)                                       .Returns(IP8)
      m8Mock.Setup   ( Function(x) x.CompressorFlag)                                    .Returns(IP9)
      psac.Setup     ( Function(x) x.OverrunUtilisationForCompressionFraction)          .Returns(IP10)
      sgnlsMock.Setup( Function(x) x.EngineStopped)                                     .Returns(IP11)

      Dim target As New M9( m1Mock.Object, m4Mock.Object, m6Mock.Object,m8Mock.Object,fMapMock.Object,psac.Object,sgnlsMock.Object)

      target.CycleStep(1)

      Assert.AreEqual(target.LitresOfAirCompressorOnContinually                , AG1 )
      Assert.AreEqual(target.LitresOfAirCompressorOnOnlyInOverrun              , AG2 )
      Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOnContinuously , 0 )
      Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOffContinuously, 0)


End Sub



End Class

End Namespace




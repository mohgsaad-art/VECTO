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


public class MockFuel50PC
Implements IFUELMAP


   Public Function fFCdelaunay_Intp(nU As Single, Tq As Single) As Single Implements IFUELMAP.fFCdelaunay_Intp

       Return (nU + Tq ) * 0.5

   End Function

   Public Property FilePath As String Implements IFUELMAP.FilePath

   Public Function ReadFile(Optional ShowMsg As Boolean = True) As Boolean Implements IFUELMAP.ReadFile
   Return true
   End Function

            Public ReadOnly Property FC As List(Of Single) Implements IFUELMAP.FC
                Get

                End Get
            End Property

            Public ReadOnly Property MapDim As Integer Implements IFUELMAP.MapDim
                Get

                End Get
            End Property

            Public ReadOnly Property nU As List(Of Single) Implements IFUELMAP.nU
                Get

                End Get
            End Property

            Public ReadOnly Property Tq As List(Of Single) Implements IFUELMAP.Tq
                Get

                End Get
            End Property

            Public Function Triangulate() As Boolean Implements IFUELMAP.Triangulate

            End Function
End Class


Private M1 As IM1_AverageHVACLoadDemand
Private M4 As IM4_AirCompressor
Private M6 As IM6
Private M8 As IM8


<Test()> _
<TestCase(50,50,400,200,100,1200,50,0,0,0.5f,50f,0,650.2083333f,650.125f)> _
<TestCase(50,50,400,200,100,1200,50,1,0,0.5f,50f,0,650.2083333f,650.125f)> _
<TestCase(50,50,400,200,100,1200,50,0,1,0.5f,50f,0,650.2083333f,650.125f)> _
<TestCase(50,50,400,200,100,1200,50,1,1,0.5f,50f,25,650.2083333f,650.125f)> _
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
      m4Mock.Setup   ( Function(x) x.GetAveragePowerDemandPerCompressorUnitFlowRate)    .Returns(IP7)
      m6Mock.Setup   ( Function(x) x.OverrunFlag)                                       .Returns(IP8)
      m8Mock.Setup   ( Function(x) x.CompressorFlag)                                    .Returns(IP9)
      psac.Setup     ( Function(x) x.OverrunUtilisationForCompressionFraction)          .Returns(IP10)

      Dim target As New M9( m1Mock.Object, m4Mock.Object, m6Mock.Object,m8Mock.Object,fMapMock,psac.Object,sgnlsMock.Object)

      target.CycleStep(1)

      Assert.AreEqual(target.LitresOfAirCompressorOnContinually                , AG1 )
      Assert.AreEqual(target.LitresOfAirCompressorOnOnlyInOverrun              , AG2 )
      Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOnContinuously , AG3 )
      Assert.AreEqual(target.TotalCycleFuelConsumptionCompressorOffContinuously, AG4 )


End Sub



End Class

End Namespace




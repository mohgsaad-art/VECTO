Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests
<TestFixture()> _
Public Class M11Tests

<Test()> _
<TestCase(0,50,60,70,80,90,1500,False , 0,50,60,0.2182501f,0.2182059f,60)> _
<TestCase(1,50,60,70,80,90,1500,False ,50,50,60,0.2182501f,0.2182059f,60)> _
<TestCase(1,50,60,70,80,90,1500,True  , 0, 0,60,0          ,0         ,0)> _
Public Sub InputOutputValues( IP1  As single, 
                              IP2  As single, 
                              IP3  As single,
                              IP4  As single, 
                              IP5  As single, 
                              IP6  As single,
                              IP7  As single, 
                              IP8  As Boolean,
                              OUT1 As single, 
                              OUT2 As single, 
                              OUT3 As single, 
                              OUT4 As single, 
                              OUT5 As single,
                              OUT6 As single)

'Arrange

 Dim m1Mock     As New Mock(Of IM1_AverageHVACLoadDemand)
 Dim m3Mock     As New Mock(Of IM3_AveragePneumaticLoadDemand)


 Dim m6Mock     As New Mock(Of IM6)
 Dim m8Mock     As New Mock(Of IM8)
 Dim sgnlsMock  As New Mock(Of ISignals)
 Dim fmap       As New MockFuel50pc




 m6Mock   .Setup( Function(x)  x.OverrunFlag)                                       .Returns( IP1 )
 m8Mock   .Setup( Function(x)  x.SmartElectricalAlternatorPowerGenAtCrank)          .Returns( IP2 )
 m6Mock   .Setup( Function(x)  x.AvgPowerDemandAtCrankFromElectricsIncHVAC)         .Returns( IP3 )
 sgnlsMock.Setup( Function(x)  x.EngineDrivelineTorque)                             .Returns( IP4 )
 m3Mock   .Setup( Function(x)  x.GetAveragePowerDemandAtCrankFromPneumatics)        .Returns( IP5 )
 m1Mock   .Setup( Function(x)  x.AveragePowerDemandAtCrankFromHVACMechanicalsWatts) .Returns( IP6 )
 sgnlsMock.Setup( Function(x)  x.EngineSpeed)                                       .Returns( IP7 )
 sgnlsMock.Setup( Function(x)  x.EngineStopped)                                     .Returns( IP8 )


 'Act
  Dim target  = New M11( m1Mock.Object,m3Mock.Object, m6Mock.Object,m8Mock.Object,fmap ,sgnlsMock.Object)',m3Mock.Object,m6Mock.Object,m8Mock.Object,fmap,sgnlsMock.Object)
        
 'Add Current Calculation to Internal Aggregates ( Accesseed by public output properties which are external interface )
 target.CycleStep(1)


 'Assert
 Assert.AreEqual( target.SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly,OUT1)
 Assert.AreEqual( target.SmartElectricalTotalCycleEletricalEnergyGenerated,OUT2)
 Assert.AreEqual( target.TotalCycleElectricalDemand,OUT3)
 Assert.AreEqual( Math.Round(CType(target.TotalCycleFuelConsumptionSmartElectricalLoad,Decimal),7),OUT4)
 Assert.AreEqual( Math.Round(CType(target.TotalCycleFuelConsumptionZeroElectricalLoad,Decimal),7),OUT5)
 Assert.AreEqual( Math.Round(CType(target.StopStartSensitiveTotalCycleElectricalDemand,Decimal),7),OUT6)



End Sub


End Class



End Namespace




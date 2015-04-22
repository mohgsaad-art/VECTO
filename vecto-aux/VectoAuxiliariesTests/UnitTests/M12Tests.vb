Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests
<TestFixture()> _
Public Class M12Tests

<Test()> _
<TestCase(2,8,6,5,8,6,4.67379665f,4.5f)> _
Public Sub InputOutputValues( IP2  As single, 
                              IP3  As single,
                              IP4  As single, 
                              IP5  As single, 
                              IP6  As Single,
                              IP7  As Single,
                              OUT1 As single,
                              OUT2 As single
                              )

'Arrange
 Dim M10Mock     As New Mock(Of IM10)
 Dim m11Mock     As New Mock(Of IM11)

 Dim sgnlsMock   As New Mock(Of ISignals)


m11Mock.Setup( Function(x) x.TotalCycleFuelConsumptionZeroElectricalLoad)          .Returns( IP2 )
m11Mock.Setup( Function(x) x.SmartElectricalTotalCycleEletricalEnergyGenerated)    .Returns( IP3 )
m11Mock.Setup( Function(x) x.TotalCycleFuelConsumptionSmartElectricalLoad)         .Returns( IP4 )
m11Mock.Setup( Function(x) x.TotalCycleElectricalDemand)                           .Returns( IP5 )
m11Mock.Setup( Function(x) x.StopStartSensitiveTotalCycleElectricalDemand)         .Returns( IP6 )
M10Mock.Setup( Function(x) x.AverageLoadsFuelConsumptionInterpolatedForPneumatics) .Returns( IP7 )

 'Act
  Dim target  = New M12(M10Mock.Object, m11Mock.Object ,sgnlsMock.Object )
        
 'Assert
 Assert.AreEqual( target.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand, OUT1 )
 Assert.AreEqual( target.BaseFuelConsumptionWithAverageAuxiliaryLoads,                    OUT2 )

End Sub





End Class



End Namespace




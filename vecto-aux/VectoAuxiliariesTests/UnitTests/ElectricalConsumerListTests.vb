Imports NUnit.Framework
Imports VectoAuxiliaries.Electrics

<TestFixture()>
Public Class ElectricalConsumerListTests

Private TestConsumerList As ElectricalConsumerList = New ElectricalConsumerList(26.3,0.096,True)


Sub New()

End Sub


<Test()>
Public Sub CreateNewTest()

   Dim target As New ElectricalConsumerList(26.3,0.096, True)

   Assert.IsNotNull(target)

End Sub


<Test()>
Public Sub SumAllConsumersTest()

     TestConsumerList.Items("Controllers,Valves etc").NumberInActualVehicle=1

     Dim actual As Single = TestConsumerList.GetTotalAverageDemandAmps( False)

    TestConsumerList.Items("Controllers,Valves etc").NumberInActualVehicle=1

     Dim expected = 60.63

     Assert.AreEqual(expected, Math.Round(actual,2))

End Sub

<Test()>
Public Sub SumNonExcludedConsumersTest()

     TestConsumerList.Items("Controllers,Valves etc").NumberInActualVehicle=1
     Dim actual As Single = TestConsumerList.GetTotalAverageDemandAmps(True)
     TestConsumerList.Items("Controllers,Valves etc").NumberInActualVehicle=0
     Dim expected = 35.63
     Assert.AreEqual(expected, Math.Round(actual,2))

End Sub


<Test()>            
<ExpectedException("System.ArgumentException")>  _
Public Sub DuplicateConsumersTest_ThrowsArgumentException()

   Dim target As New ElectricalConsumerList(0.096,26.3)
   'Add two OnBaseVehicle consumers
   target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10, 1, 26.3, 1))
   target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10, 1, 26.3, 1))

End Sub



End Class

Imports NUnit.Framework
Imports VectoAuxiliaries.Electrics

<TestFixture()>
Public Class ElectricalConsumerListTests

Private TestConsumerList As ElectricalConsumerList


Sub New()


   TestConsumerList = New ElectricalConsumerList(26.3, True)

   'Add two OnBaseVehicle consumers
   TestConsumerList.AddConsumer(New ElectricalConsumer(True, "TEST1", "Exclude1", 10, 1, 26.3, 1))
   TestConsumerList.AddConsumer(New ElectricalConsumer(True, "TEST2", "Exclude2", 10, 1, 26.3, 1))

   'Add two NOT onBaseVehicle consumers
   TestConsumerList.AddConsumer(New ElectricalConsumer(False, "TEST3", "Include1", 10, 1, 26.3, 1))
   TestConsumerList.AddConsumer(New ElectricalConsumer(False, "TEST4", "Include2", 10, 1, 26.3, 1))


End Sub


<Test()>
Public Sub CreateNewTest()

   Dim target As New ElectricalConsumerList(26.3)

   Assert.IsNotNull(target)

End Sub


<Test()>
Public Sub SumAllConsumersTest()

     Dim actual As Single = TestConsumerList.GetTotalAverageDemandAmps(1, False)
     Dim expected = 40

     Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub SumNonExcludedConsumersTest()

     Dim actual As Single = TestConsumerList.GetTotalAverageDemandAmps(1, True)
     Dim expected = 20

     Assert.AreEqual(expected, actual)

End Sub


<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub DuplicateConsumersTest_ThrowsArgumentException()

   Dim target As New ElectricalConsumerList(26.3)

   'Add two OnBaseVehicle consumers
   target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10, 1, 26.3, 1))
   target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10, 1, 26.3, 1))

End Sub

End Class

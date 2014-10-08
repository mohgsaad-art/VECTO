Imports NUnit.Framework
Imports VectoAuxiliaries.Electrics

<TestFixture()>
Public Class ElectricalConsumerListTests

Private TestConsumerList As ElectricalConsumerList


Sub New()


   TestConsumerList = New ElectricalConsumerList()

   'Add one two OnBaseVhicile consumers

   TestConsumerList.AddConsumer(New ElectricalConsumer(True, "TEST1", "Exclude1", 10, 1, 26.3, 1))
   TestConsumerList.AddConsumer(New ElectricalConsumer(True, "TEST2", "Exclude2", 10, 1, 26.3, 1))

   TestConsumerList.AddConsumer(New ElectricalConsumer(False, "TEST3", "Include1", 10, 1, 26.3, 1))
   TestConsumerList.AddConsumer(New ElectricalConsumer(False, "TEST4", "Include2", 10, 1, 26.3, 1))


End Sub


<Test()>
Public Sub CreateNewTest()

   Dim target As New ElectricalConsumerList()

   Assert.IsNotNull(target)

End Sub


<Test()>
Public Sub SumAllConsumers()

     Dim actual As Single = TestConsumerList.GetTotalAverageDemandAmps(1, False)
     Dim expected = 40

     Assert.AreEqual(expected, actual)


End Sub

<Test()>
Public Sub SumNonExcludedConsumers()

     Dim actual As Single = TestConsumerList.GetTotalAverageDemandAmps(1, True)
     Dim expected = 20

     Assert.AreEqual(expected, actual)


End Sub

End Class

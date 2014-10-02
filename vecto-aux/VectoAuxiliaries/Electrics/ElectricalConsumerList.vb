
Imports System.Collections.Generic

Namespace Electrics

Public Class ElectricalConsumerList

Private _items As New Dictionary(Of String, IElectricalConsumer)



   Public ReadOnly Property Items As Dictionary(Of String, IElectricalConsumer)
       Get
        Return _items

       End Get
   End Property

   Public Sub AddConsumer(consumer As IElectricalConsumer)

     If Not _items.ContainsKey(consumer.ConsumerName) Then

     _items.Add(consumer.ConsumerName, consumer)

     Else

     Throw New ArgumentException("Consumer Already Present in the list")

     End If

   End Sub

   Public Sub RemoveConsumer(consumer As IElectricalConsumer)

    If _items.ContainsKey(consumer.ConsumerName) Then

    _items.Remove(consumer.ConsumerName)

    Else

      Throw New ArgumentException("Consumer Not In List")

    End If

   End Sub

   Public Function GetTotalAveragePowerWatts(doorDutyCyclePercentage? As Single) As Single

   'Sanity check.
   If doorDutyCyclePercentage Is Nothing Or doorDutyCyclePercentage > 1 Or doorDutyCyclePercentage < 0 Then
     Throw New ArgumentException("doorDutyCyclePercentage must be between 0 and 1")
   End If

    Dim power As Single = Aggregate item In Items Into Sum(item.Value.TotalAvgConumptionAmps(doorDutyCyclePercentage))

    Return power

   End Function


End Class


End Namespace



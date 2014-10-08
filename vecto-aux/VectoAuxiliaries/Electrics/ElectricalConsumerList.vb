
Imports System.Collections.Generic

Namespace Electrics

Public Class ElectricalConsumerList
Implements IElectricalConsumerList

Private _items As New Dictionary(Of String, IElectricalConsumer)


   Public ReadOnly Property Items As Dictionary(Of String, IElectricalConsumer) Implements Electrics.IElectricalConsumerList.Items
       Get
        Return _items

       End Get
   End Property

   Public Sub AddConsumer(consumer As IElectricalConsumer) Implements Electrics.IElectricalConsumerList.AddConsumer

     If Not _items.ContainsKey(consumer.ConsumerName) Then

     _items.Add(consumer.ConsumerName, consumer)

     Else

     Throw New ArgumentException("Consumer Already Present in the list")

     End If

   End Sub

   Public Sub RemoveConsumer(consumer As IElectricalConsumer) Implements Electrics.IElectricalConsumerList.RemoveConsumer

    If _items.ContainsKey(consumer.ConsumerName) Then

    _items.Remove(consumer.ConsumerName)

    Else

      Throw New ArgumentException("Consumer Not In List")

    End If

   End Sub

   Public Function GetTotalAverageDemandAmps(doorDutyCyclePercentage? As Single, excludeOnBase As Boolean) As Single Implements Electrics.IElectricalConsumerList.GetTotalAverageDemandAmps

   'Sanity check.
   If doorDutyCyclePercentage Is Nothing Or doorDutyCyclePercentage > 1 Or doorDutyCyclePercentage < 0 Then

     Throw New ArgumentException("doorDutyCyclePercentage must be between 0 and 1")

   End If

     Dim Amps As Single

     If excludeOnBase Then
       Amps = Aggregate item In Items Where item.Value.BaseVehicle = False Into Sum(item.Value.TotalAvgConumptionAmps(doorDutyCyclePercentage))
     Else
       Amps = Aggregate item In Items Into Sum(item.Value.TotalAvgConumptionAmps(doorDutyCyclePercentage))
     End If


    Return Amps

   End Function


End Class

End Namespace



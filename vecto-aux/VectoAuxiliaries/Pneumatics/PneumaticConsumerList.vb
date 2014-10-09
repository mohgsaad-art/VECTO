Imports VectoAuxiliaries.Pneumatics

Namespace Pneumatics


Public Class PneumaticConsumerList
Implements IPneumaticConsumerList

Private _items As New Dictionary(Of String, IPneumaticConsumer)

   Public ReadOnly Property Items As Dictionary(Of String, IPneumaticConsumer) Implements Pneumatics.IPneumaticConsumerList.Items
       Get
        Return _items

       End Get
   End Property

   Public Sub AddConsumer(consumer As IPneumaticConsumer) Implements IPneumaticConsumerList.AddConsumer

     If Not _items.ContainsKey(consumer.ConsumerName) Then
       _items.Add(consumer.ConsumerName, consumer)
     Else

     Throw New ArgumentException("Consumer Already Present in the list")

     End If

  End Sub

   Public Sub RemoveConsumer(consumer As IPneumaticConsumer) Implements IPneumaticConsumerList.RemoveConsumer

      If _items.ContainsKey(consumer.ConsumerName) Then

    _items.Remove(consumer.ConsumerName)

    Else

      Throw New ArgumentException("Consumer Not In List")

    End If

  End Sub







End Class


End Namespace



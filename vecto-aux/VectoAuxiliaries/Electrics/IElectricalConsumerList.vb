
Namespace Electrics

Public Interface IElectricalConsumerList

    ReadOnly Property Items As List(Of  IElectricalConsumer)
    Sub AddConsumer(consumer As IElectricalConsumer)
    Sub RemoveConsumer(consumer As IElectricalConsumer)
    Function GetTotalAverageDemandAmps(excludeOnBase As Boolean) As Single

End Interface


End Namespace




Namespace Electrics

Public Interface IElectricalConsumerList

    ReadOnly Property Items As Dictionary(Of String, IElectricalConsumer)
    Sub AddConsumer(consumer As IElectricalConsumer)
    Sub RemoveConsumer(consumer As IElectricalConsumer)
    Function GetTotalAverageDemandAmps(doorDutyCyclePercentage? As Single) As Single

End Interface


End Namespace



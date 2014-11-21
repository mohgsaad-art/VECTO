
Namespace Electrics


Public Interface IElectricalConsumerList

     Property Items As List(Of  IElectricalConsumer)
    Sub AddConsumer(consumer As IElectricalConsumer)
    Sub RemoveConsumer(consumer As IElectricalConsumer)
    Function GetTotalAverageDemandAmps(excludeOnBase As Boolean) As Single
    Property DoorDutyCycleFraction As single

End Interface


End Namespace



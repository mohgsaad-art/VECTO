Imports VectoAuxiliaries.Electrics

Namespace Electrics

Public Interface IElectricsUserInputsConfig

Property PowerNetVoltage As Single
Property AlternatorMap As String
Property AlternatorGearEfficiency As Single
Property ElectricalConsumers As IElectricalConsumerList
Property DoorActuationTimeSecond As Integer

Property ResultCardIdle As List(Of SmartResult)
Property ResultCardTraction As List(Of SmartResult)
Property ResultCardOverrun As List(Of SmartResult)
Property SmartElectrical As Boolean

End Interface


End Namespace



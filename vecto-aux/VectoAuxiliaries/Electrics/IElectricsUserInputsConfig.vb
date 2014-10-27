Imports VectoAuxiliaries.Electrics

Namespace Electrics

Public Interface IElectricsUserInputsConfig

Property PowerNetVoltage As Single
Property AlternatorMap As String
Property AlternatorGearEfficiency As Single
Property ElectricalConsumers As IElectricalConsumerList
Property DoorActuationTimeSecond As Integer

Property ResultCardIdle As IResultCard
Property ResultCardTraction As IResultCard
Property ResultCardOverrun As IResultCard
Property SmartElectrical As Boolean

End Interface


End Namespace



Imports VectoAuxiliaries.Electrics

Namespace Electrics

Public Interface IElectricsUserInputsConfig

Property PowerNetVoltage As Single
Property AlternatorMap As String
Property AlternatorGearEfficiency As Single
Property ElectricalConsumers As IElectricalConsumerList
Property DoorActuationTimeSecond As Integer

Property ResultCardIdleAmps As Dictionary(Of Single, Single)
Property ResultCardTractionAmps As Dictionary(Of Single, Single)
Property ResultCardOverrunAmps As Dictionary(Of Single, Single)
Property SmartElectrical As Boolean

End Interface


End Namespace



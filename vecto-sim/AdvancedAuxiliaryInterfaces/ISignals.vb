Public Interface ISignals

Property PreExistingAuxPower As single
Property EngineMotoringPower As single
property EngineDrivelinePower as single
property SmartElectrics As Boolean
Property ClutchEngaged As Boolean
Property EngineSpeed as integer
Property SmartPneumatics As Boolean
Property TotalCycleTimeSeconds As Integer
Property CurrentCycleTimeInSeconds As Integer
property EngineDrivelineTorque as single
Property Idle As Boolean
Property InNeutral As Boolean
Property AuxiliaryEventReportingLevel As AdvancedAuxiliaryMessageType
Property EngineStopped As Boolean

End Interface

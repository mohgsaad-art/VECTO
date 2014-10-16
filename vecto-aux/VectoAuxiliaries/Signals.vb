
Public Class Signals
Implements ISignals

    Public Property ClutchEngaged As Boolean Implements ISignals.ClutchEngaged

    Public Property EngineDrivelinePower As Single Implements ISignals.EngineDrivelinePower

    Public Property EngineDrivelineTorque As Single Implements ISignals.EngineDrivelineTorque

    Public Property EngineMotoringPower As Single Implements ISignals.EngineMotoringPower

    Public Property EngineSpeed As Integer Implements ISignals.EngineSpeed

    Public Property SmartElectrics As Boolean Implements ISignals.SmartElectrics

    Public Property SmartPneumatics As Boolean Implements ISignals.SmartPneumatics

    Public Property TotalCycleTimeSeconds As Integer Implements ISignals.TotalCycleTimeSeconds

End Class





Public Class Signals
Implements ISignals




  Private _smartElectrics As Boolean

    Public Property ClutchEngaged As Boolean Implements ISignals.ClutchEngaged

    Public Property EngineDrivelinePower As Single Implements ISignals.EngineDrivelinePower

    Public Property EngineDrivelineTorque As Single Implements ISignals.EngineDrivelineTorque

    Public Property EngineMotoringPower As Single Implements ISignals.EngineMotoringPower

    Public Property EngineSpeed As Integer Implements ISignals.EngineSpeed

    Public Property SmartElectrics As Boolean Implements ISignals.SmartElectrics
    Get
    Return _smartElectrics
    End Get
    Set(value As Boolean)

    _smartElectrics=value

    Debug.WriteLine(String.Format("SmartElectrics {0}", _smartElectrics))

    End Set

     end property


    Public Property SmartPneumatics As Boolean Implements ISignals.SmartPneumatics

    Public Property TotalCycleTimeSeconds As Integer Implements ISignals.TotalCycleTimeSeconds

    public Property CurrentCycleTimeInSeconds As Integer Implements ISignals.CurrentCycleTimeInSeconds

    Public Property PreExistingAuxPower As Single Implements ISignals.PreExistingAuxPower

    Public Property Idle As Boolean Implements ISignals.Idle

    Public Property InNeutral As Boolean Implements ISignals.InNeutral

    Public Property AuxiliaryEventReportingLevel As AdvancedAuxiliaryMessageType Implements ISignals.AuxiliaryEventReportingLevel



End Class




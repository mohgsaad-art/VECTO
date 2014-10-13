Namespace Pneumatics

Public Class PneumaticUserInputsConfig
Implements IPneumaticUserInputsConfig

'pnmeumatic or electric
Public Property AdBlueDosing As String Implements IPneumaticUserInputsConfig.AdBlueDosing
'mechanical or electrical
Public Property AirSuspensionControl As String Implements IPneumaticUserInputsConfig.AirSuspensionControl

Public Property CompressorType As String Implements IPneumaticUserInputsConfig.CompressorType

Public Property CompressorMap As String Implements IPneumaticUserInputsConfig.CompressorMap
Public Property CompressorGearEfficiency As Single Implements IPneumaticUserInputsConfig.CompressorGearEfficiency
Public Property CompressorGearRatio As Single Implements IPneumaticUserInputsConfig.CompressorGearRatio

'PneumaticActuationsMap
Public Property ActuationsMap As String Implements IPneumaticUserInputsConfig.ActuationsMap

'pneumatic or electric
Public Property Doors As String Implements IPneumaticUserInputsConfig.Doors
Public Property KneelingHeightMilimeters As Single Implements IPneumaticUserInputsConfig.KneelingHeightMilimeters
Public Property RetarderBrake As Boolean Implements IPneumaticUserInputsConfig.RetarderBrake
Public Property SmartAirCompression As Boolean Implements IPneumaticUserInputsConfig.SmartAirCompression
Public Property SmartRegeneration As Boolean Implements IPneumaticUserInputsConfig.SmartRegeneration




Public Sub New()


End Sub



End Class





End Namespace




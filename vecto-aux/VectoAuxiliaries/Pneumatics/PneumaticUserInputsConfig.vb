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


Public Sub New( optional setToDefaults As Boolean = False)


If setToDefaults then SetPropertiesToDefaults()

End Sub

Public sub SetPropertiesToDefaults()


            ActuationsMap=""
            AdBlueDosing="pneumatic"
            AirSuspensionControl="electrically"
            CompressorGearEfficiency=0.8
            CompressorGearRatio=0
            CompressorMap=""
            CompressorType=""
            Doors="pneumatic"
            KneelingHeightMilimeters=80
            RetarderBrake=True
            SmartAirCompression=True
            SmartRegeneration=True  

End Sub



End Class





End Namespace




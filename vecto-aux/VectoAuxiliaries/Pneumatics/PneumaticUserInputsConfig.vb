Namespace Pneumatics

Public Class PneumaticUserInputsConfig
Implements IPneumaticUserInputsConfig





'pnmeumatic or electric
Public Property AdBlueDosing As String Implements IPneumaticUserInputsConfig.AdBlueDosing
'mechanical or electrical
Public Property AirSuspensionControl As String Implements IPneumaticUserInputsConfig.AirSuspensionControl

Public Property CompressorMap As String Implements IPneumaticUserInputsConfig.CompressorMap
Public Property CompressorGearEfficiency As Single Implements IPneumaticUserInputsConfig.CompressorGearEfficiency
Public Property CompressorGearRatio As Single Implements IPneumaticUserInputsConfig.CompressorGearRatio

'PneumaticActuationsMap
Public Property ActuationsMap As String Implements IPneumaticUserInputsConfig.ActuationsMap

'pneumatic or electric
Public Property Doors As String Implements IPneumaticUserInputsConfig.Doors
Public Property KneelingHeightMillimeters As Single Implements IPneumaticUserInputsConfig.KneelingHeightMillimeters
Public Property RetarderBrake As Boolean Implements IPneumaticUserInputsConfig.RetarderBrake
Public Property SmartAirCompression As Boolean Implements IPneumaticUserInputsConfig.SmartAirCompression
Public Property SmartRegeneration As Boolean Implements IPneumaticUserInputsConfig.SmartRegeneration


Public Sub New( optional setToDefaults As Boolean = False)


If setToDefaults then SetPropertiesToDefaults()

End Sub

Public sub SetPropertiesToDefaults()


            ActuationsMap="testPneumaticActuationsMap_GOODMAP.csv"
            AdBlueDosing="Pneumatic"
            AirSuspensionControl="Electrically"
            CompressorGearEfficiency=0.8
            CompressorGearRatio=1.0
            CompressorMap="testCompressorMap.csv"
            Doors="Pneumatic"
            KneelingHeightMillimeters=80
            RetarderBrake=True
            SmartAirCompression=True
            SmartRegeneration=True  

End Sub



End Class





End Namespace




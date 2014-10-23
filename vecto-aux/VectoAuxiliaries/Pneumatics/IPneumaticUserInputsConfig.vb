
Namespace Pneumatics


Public Interface IPneumaticUserInputsConfig


Property CompressorType            As String
Property CompressorMap             As String
Property CompressorGearEfficiency  As Single
Property CompressorGearRatio       As Single
Property ActuationsMap             As String
Property SmartAirCompression       As Boolean
Property SmartRegeneration         As Boolean
Property RetarderBrake             As Boolean
Property KneelingHeightMillimeters As Single
Property AirSuspensionControl      As String 'mechanical or electrical
Property AdBlueDosing              As String 'pnmeumatic or electric
Property Doors                     As String 'pneumatic or electric


End Interface


End Namespace



' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

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


            ActuationsMap="testPneumaticActuationsMap_GOODMAP.apac"
            AdBlueDosing="Pneumatic"
            AirSuspensionControl="Electrically"
            CompressorGearEfficiency=0.8
            CompressorGearRatio=1.0
            CompressorMap="testCompressorMap.acmp"
            Doors="Pneumatic"
            KneelingHeightMillimeters=80
            RetarderBrake=True
            SmartAirCompression=True
            SmartRegeneration=True  

End Sub



End Class





End Namespace




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

        Public Property CompressorMap As String Implements IPneumaticUserInputsConfig.CompressorMap
        Public Property CompressorGearRatio As Single Implements IPneumaticUserInputsConfig.CompressorGearRatio
        Public Property CompressorGearEfficiency As Single Implements IPneumaticUserInputsConfig.CompressorGearEfficiency

        'pnmeumatic or electric
        Public Property AdBlueDosing As String Implements IPneumaticUserInputsConfig.AdBlueDosing

        'mechanical or electrical
        Public Property AirSuspensionControl As String Implements IPneumaticUserInputsConfig.AirSuspensionControl

        'pneumatic or electric
        Public Property Doors As String Implements IPneumaticUserInputsConfig.Doors
        Public Property KneelingHeightMillimeters As Single Implements IPneumaticUserInputsConfig.KneelingHeightMillimeters

        'PneumaticActuationsMap
        Public Property ActuationsMap As String Implements IPneumaticUserInputsConfig.ActuationsMap

        Public Property RetarderBrake As Boolean Implements IPneumaticUserInputsConfig.RetarderBrake
        Public Property SmartAirCompression As Boolean Implements IPneumaticUserInputsConfig.SmartAirCompression
        Public Property SmartRegeneration As Boolean Implements IPneumaticUserInputsConfig.SmartRegeneration

        Public Sub New(Optional setToDefaults As Boolean = False)

            If setToDefaults Then SetPropertiesToDefaults()

        End Sub

        Public Sub SetPropertiesToDefaults()

            CompressorMap = String.Empty
            CompressorGearRatio = 1.0
            CompressorGearEfficiency = 0.97
            AdBlueDosing = "Pneumatic"
            AirSuspensionControl = "Mechanically"
            Doors = "Pneumatic"
            KneelingHeightMillimeters = 70
            ActuationsMap = "testPneumaticActuationsMap.apac"
            RetarderBrake = True
            SmartAirCompression = False
            SmartRegeneration = False

        End Sub

    End Class

End Namespace




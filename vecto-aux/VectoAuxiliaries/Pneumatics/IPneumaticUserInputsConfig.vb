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

    Public Interface IPneumaticUserInputsConfig

        Property CompressorMap As String
        Property CompressorGearEfficiency As Single
        Property CompressorGearRatio As Single
        Property ActuationsMap As String
        Property SmartAirCompression As Boolean
        Property SmartRegeneration As Boolean
        Property RetarderBrake As Boolean
        Property KneelingHeightMillimeters As Single
        Property AirSuspensionControl As String 'mechanical or electrical
        Property AdBlueDosing As String 'pnmeumatic or electric
        Property Doors As String 'pneumatic or electric

    End Interface

End Namespace



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

Namespace Electrics

Public Class M5__SmartAlternatorSetGeneration
 Implements IM5_SmartAlternatorSetGeneration

Private _powerNetVoltage As Single
Private _m05 As M0_5_SmartAlternatorSetEfficiency
Private _alternatorGearEfficiency As single




    Public Sub new ( m05 As M0_5_SmartAlternatorSetEfficiency, ByVal powernetVoltage As single, alternatorGearEfficiency As single)

    'sanity check
    If m05 is Nothing then Throw New ArgumentException("Please supply a valid module M05")
    If powernetVoltage < ElectricConstants.PowenetVoltageMin orelse powernetVoltage > ElectricConstants.PowenetVoltageMax then Throw New ArgumentException("Powernet Voltage out of range")
    If alternatorGearEfficiency < 0 or alternatorGearEfficiency>1 then Throw New ArgumentException("AlternatorGearEfficiency Out of bounds, should be 0 to 1")

    'assign private variables.
    _m05=m05
    _powerNetVoltage=powernetVoltage
    _alternatorGearEfficiency = alternatorGearEfficiency


    End Sub

    Public Function AlternatorsGenerationPowerAtCrankIdleWatts() As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankIdleWatts

      Return (_m05.SmartIdleCurrent() * _powerNetVoltage) / ( _m05.AlternatorsEfficiencyIdleResultCard() * _alternatorGearEfficiency)

    End Function

    Public Function AlternatorsGenerationPowerAtCrankOverrunWatts() As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankOverrunWatts

          Return (_m05.SmartOverrunCurrent() * _powerNetVoltage) / ( _m05.AlternatorsEfficiencyOverrunResultCard() * _alternatorGearEfficiency)

    End Function

    Public Function AlternatorsGenerationPowerAtCrankTractionOnWatts() As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankTractionOnWatts

              Return (_m05.SmartTractionCurrent() * _powerNetVoltage) / ( _m05.AlternatorsEfficiencyTractionOnResultCard() * _alternatorGearEfficiency)

    End Function

End Class
End Namespace




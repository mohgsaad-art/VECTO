
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

    Public Function AlternatorsGenerationPowerAtCrankIdleWatts(rpm As Integer) As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankIdleWatts

      Return (_m05.SmartIdleCurrent() * _powerNetVoltage) / ( _m05.AlternatorsEfficiencyIdleResultCard(rpm) * _alternatorGearEfficiency)

    End Function

    Public Function AlternatorsGenerationPowerAtCrankOverrunWatts(rpm As Integer) As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankOverrunWatts

          Return (_m05.SmartOverrunCurrent() * _powerNetVoltage) / ( _m05.AlternatorsEfficiencyOverrunResultCard(rpm) * _alternatorGearEfficiency)

    End Function

    Public Function AlternatorsGenerationPowerAtCrankTractionOnWatts(rpm As Integer) As Single Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankTractionOnWatts

              Return (_m05.SmartTractionCurrent() * _powerNetVoltage) / ( _m05.AlternatorsEfficiencyTractionOnResultCard(rpm) * _alternatorGearEfficiency)

    End Function

End Class
End Namespace




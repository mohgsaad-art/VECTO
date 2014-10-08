Namespace Electrics

Public Class ElectricConstants

    'Anticipated Min and Max Allowable values for Powernet, normally 26.3 volts but could be 48 in the future.
    Public Const PowenetVoltageMin As Single = 6
    Public Const PowenetVoltageMax As Single = 50

    'Duty Cycle IE Percentage of use
    Public Const PhaseIdleTractionOnMin As Single = 0
    Public Const PhaseIdleTractionMax As Single = 1

    'Max Min Expected Consumption for a Single Consumer, negative values allowed as bonuses.
    Public Const NonminalConsumerConsumptionAmpsMin As Integer = -10
    Public Const NominalConsumptionAmpsMax As Integer = 100



End Class


End Namespace










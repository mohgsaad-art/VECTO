Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M3_Mock
 Implements IM3_AveragePneumaticLoadDemand

 public _GetAveragePowerDemandAtCrankFromPneumatics    as single
 public _TotalAirConsumedPerCycle                      as single


    Public Function GetAveragePowerDemandAtCrankFromPneumatics() As Single Implements IM3_AveragePneumaticLoadDemand.GetAveragePowerDemandAtCrankFromPneumatics
     Return _GetAveragePowerDemandAtCrankFromPneumatics
    End Function

    Public Function TotalAirConsumedPerCycle() As Single Implements IM3_AveragePneumaticLoadDemand.TotalAirConsumedPerCycle
     Return _TotalAirConsumedPerCycle
    End Function



Public Sub new()

End Sub

Public Sub new ( GetAveragePowerDemandAtCrankFromPneumatics As Single,TotalAirConsumedPerCycle As single )

_GetAveragePowerDemandAtCrankFromPneumatics= GetAveragePowerDemandAtCrankFromPneumatics
_TotalAirConsumedPerCycle = TotalAirConsumedPerCycle

End Sub



End Class


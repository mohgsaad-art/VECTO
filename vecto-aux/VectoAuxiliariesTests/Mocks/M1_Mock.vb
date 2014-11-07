
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules


Public Class M1_Mock
Implements IM1_AverageHVACLoadDemand

Public  _AveragePowerDemandAtAlternatorFromHVACElectricsWatts As Single
public _AveragePowerDemandAtCrankFromHVACElectricsWatts As single
Public _AveragePowerDemandAtCrankFromHVACMechanicalsWatts As Single
Public _HVACFuelingLitresPerHour As single

    Public Function AveragePowerDemandAtAlternatorFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtAlternatorFromHVACElectricsWatts
       Return _AveragePowerDemandAtAlternatorFromHVACElectricsWatts
    End Function

    Public Function AveragePowerDemandAtCrankFromHVACElectricsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACElectricsWatts
        Return _AveragePowerDemandAtCrankFromHVACElectricsWatts
    End Function

    Public Function AveragePowerDemandAtCrankFromHVACMechanicalsWatts() As Single Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
       Return _AveragePowerDemandAtCrankFromHVACMechanicalsWatts
    End Function

    Public Function HVACFuelingLitresPerHour() As Single Implements IM1_AverageHVACLoadDemand.HVACFuelingLitresPerHour
       Return _HVACFuelingLitresPerHour
    End Function



Public Sub new()

End Sub

Public Sub new ( AveragePowerDemandAtAlternatorFromHVACElectricsWatts  As Single, _
                 AveragePowerDemandAtCrankFromHVACElectricsWatts       As single, _
                 AveragePowerDemandAtCrankFromHVACMechanicalsWatts     As Single, _
                 HVACFuelingLitresPerHour                              As single)

                 'Assign Values
                _AveragePowerDemandAtAlternatorFromHVACElectricsWatts  = AveragePowerDemandAtAlternatorFromHVACElectricsWatts
                _AveragePowerDemandAtCrankFromHVACElectricsWatts       = AveragePowerDemandAtCrankFromHVACElectricsWatts
                _AveragePowerDemandAtCrankFromHVACMechanicalsWatts     = AveragePowerDemandAtCrankFromHVACMechanicalsWatts
                _HVACFuelingLitresPerHour                              = HVACFuelingLitresPerHour
                                                              
                                                            
End Sub                                                                              




End Class


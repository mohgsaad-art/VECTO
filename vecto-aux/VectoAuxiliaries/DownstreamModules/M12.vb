Imports AdvancedAuxiliaryInterfaces.Electrics
Imports AdvancedAuxiliaryInterfaces.Pneumatics
Imports AdvancedAuxiliaryInterfaces.Hvac

Namespace DownstreamModules

Public Class M12
Implements IM12

Private Class Point

Public X As Single
Public Y As Single 

End Class


Private M11 As IM11
Private Signals As ISignals

'Interpolation 
Private function Sum1()As single

Dim  P1 as Point = New Point with {.X=0, .Y=M11.TotalCycleFuelConsumptionZeroElectricalLoad }
Dim  P2 As Point  = New Point With {.X=M11.SmartElectricalTotalCycleEletricalEnergyGenerated, .Y=M11.TotalCycleFuelConsumptionSmartElectricalLoad}

Dim IP5x As Single = M11.TotalCycleElectricalDemand
Dim IP5y As Single = 0

Dim TanTeta as Single = (P2.Y-P1.Y)/(P2.X-P1.X)

IP5y = P1.Y + ( TanTeta * IP5x )

Return IP5Y

End Function


Public Sub new ( m11 As IM11, signals As ISignals)

   Me.M11=m11
   Me.Signals=signals

End Sub

Public ReadOnly Property FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand As Single Implements IM12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand
  Get
     Return Sum1()
  End Get

End Property

End Class

End Namespace




Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules


Public Class M13
 Implements IM13

 Private Const FUEL_DENSITY_L3 As Single = 835
 
Private m1  As IM1_AverageHVACLoadDemand
Private m10 As IM10
Private m12 As IM12
Private signals As ISignals

Private readonly Property Sum1 As Single
    Get
     return  - m12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand + m12.BaseFuelConsumptionWithAverageAuxiliaryLoads
    End Get
End Property
Private readonly Property Sum2 As Single
    Get
     Return m12.BaseFuelConsumptionWithAverageAuxiliaryLoads - m10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand
    End Get
End Property
Private readonly Property Sum3 As Single
    Get
     Return m12.BaseFuelConsumptionWithAverageAuxiliaryLoads-Sum2
    End Get
End Property
Private readonly Property Sum4 As Single
    Get
     Return -Sum1+Sum3
    End Get
End Property
Private readonly Property Sum5 As Single
    Get
     Return  ( m1.HVACFuelingLitresPerHour * ( signals.CurrentCycleTimeInSeconds/3600)) * FUEL_DENSITY_L3
    End Get
End Property
Private ReadOnly Property Sum6 As Single
    Get
     Return SW3 + Sum5
    End Get
End Property
Private ReadOnly Property Sum7 As Single
    Get
      Return  Sum6/ FUEL_DENSITY_L3
    End Get
End Property

   
Private readonly Property SW1 As Single
    Get
      Return If( signals.SmartPneumatics,Sum4,m12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand)
    End Get
End Property
private readonly Property SW2 as Single
    Get
     Return  If( signals.SmartPneumatics,m10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand, m12.BaseFuelConsumptionWithAverageAuxiliaryLoads)
    End Get
End Property
Private readonly Property SW3 As Single
    Get
     Return If( signals.SmartElectrics, SW1, SW2)
    End Get
End Property

Public Sub new ( m1 As IM1_AverageHVACLoadDemand, m10 As IM10, m12 As IM12 , signals As ISignals)

      me.m1   =  m1
      me.m10  =  m10
      me.m12  =  m12 
      Me.signals=signals
                          
End Sub


        Public ReadOnly Property TotalCycleFuelConsumptionGrams As Single Implements IM13.TotalCycleFuelConsumptionGrams
            Get
          Return Sum6
            End Get
        End Property

        Public ReadOnly Property TotalCycleFuelConsumptionLitres As Single Implements IM13.TotalCycleFuelConsumptionLitres
            Get
             Return  Sum7
            End Get
        End Property
End Class



End Namespace




Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules

Public Class M11
 Implements IM11
 
 'Private Fields Assigned by Constructor.
  Private m1      As M1_AverageHVACLoadDemand
  Private m3      As IM3_AveragePneumaticLoadDemand
  Private m6      As IM6
  Private m8      As IM8
  Private fmap    As IFUELMAP
  Private signals As ISignals

 'Private Aggregations
  Private AG1 As Single
  Private AG2 As Single
  Private AG3 As Single
  Private AG4 As single
  Private AG5 As single

  'Staging Calculations
  Private ReadOnly Property Sum1 As single
      Get

      End Get
  End Property
  Private ReadOnly Property Sum2 As Single
      Get

      End Get
  End Property
  Private ReadOnly Property Sum3 As Single
      Get

      End Get
  End Property
  Private ReadOnly Property Sum4 As Single
      Get

      End Get
  End Property
  Private ReadOnly Property Sum5 As Single
    Get

    End Get
End Property
  Private ReadOnly Property Sum6 As Single
    Get

    End Get
End Property
  Private readonly Property Sum7 As Single
    Get

    End Get
End Property
  Private ReadOnly Property Sum8 As Single
    Get

    End Get
  End Property

  'OUT1
  Public ReadOnly Property SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly As Single Implements IM11.SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly
            Get
              Return AG1
            End Get
        End Property
  'OUT2
  Public ReadOnly Property SmartElectricalTotalCycleEletricalEnergyGenerated As Single Implements IM11.SmartElectricalTotalCycleEletricalEnergyGenerated
            Get
             Return AG2
            End Get
        End Property
  'OUT3
  Public ReadOnly Property TotalCycleElectricalDemand As Single Implements IM11.TotalCycleElectricalDemand
            Get
             Return AG3
            End Get
        End Property
  'OUT4
  Public ReadOnly Property TotalCycleFuelConsumptionSmartElectricalLoad As Single Implements IM11.TotalCycleFuelConsumptionSmartElectricalLoad
            Get
             Return AG4
            End Get
        End Property
  'OUT5
  Public ReadOnly Property TotalCycleFuelConsumptionZeroElectricalLoad As Single Implements IM11.TotalCycleFuelConsumptionZeroElectricalLoad
            Get
             Return AG5
            End Get
        End Property

 'Clear at the beginning of cycle
 Sub ClearAggregates() Implements IM11.ClearAggregates
   
   AG1=0
   AG2=0
   AG3=0
   AG4=0
   AG5=0


 End Sub

 'Add to Aggregates dependent on cycle step time.
 Sub CycleStep(Optional stepTimeInSeconds As Single = 0.0) Implements IM11.CycleStep

   'TODO : Finish this calculations and subtitute the 1's for the staging sums or direct outputs from modules within this module.

   AG1  =    AG1   +  (  stepTimeInSeconds *   sum1                                           )
   AG2  =    AG2   +  (  stepTimeInSeconds *   m8.SmartElectricalAlternatorPowerGenAtCrank    )
   AG3  =    AG3   +  (  stepTimeInSeconds *   m6.AvgPowerDemandAtCrankFromElectricsIncHVAC   )
   AG4  =    AG4   +  (  stepTimeInSeconds *   sum7                                           )
   AG5  =    AG5   +  (  stepTimeInSeconds *   sum8                                           )

 End Sub

 'Constructor
 Public Sub new ( m1 As IM1_AverageHVACLoadDemand, m3 As IM3_AveragePneumaticLoadDemand, m6 As IM6, m8 As IM8, fmap As IFUELMAP, signals As ISignals)

      me.m1      = m1
      me.m3      = m3
      me.m6      = m6
      me.m8      = m8
      Me.fmap    = fmap
      Me.signals = signals

 End Sub

End Class

End Namespace





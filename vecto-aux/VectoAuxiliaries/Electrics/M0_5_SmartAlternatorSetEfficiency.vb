
Namespace Electrics


Public Class M0_5_SmartAlternatorSetEfficiency
Implements IM0_5_SmartAlternatorSetEfficiency


private _m0 As IM0_NonSmart_AlternatorsSetEfficiency
Private _electricalConsumables As IElectricalConsumerList
Private _alternatorMap As IAlternatorMap
Private _resultCardIdle As IResultCard
Private _resultCardTraction As IResultCard
Private _resultCardOverrun As IResultCard
Private _signals As ISignals



Public ReadOnly property SmartIdleCurrent() As single Implements  IM0_5_SmartAlternatorSetEfficiency.SmartIdleCurrent
    Get
       Dim hvac_Plus_None_Base  As Single = HvacPlusNonBaseCurrents()
       Dim smart_idle_current As Single = _resultCardIdle.GetSmartCurrentResult(hvac_Plus_None_Base)
       
        Return  smart_idle_current
    End Get
End Property


Public ReadOnly property AlternatorsEfficiencyIdleResultCard( ) As single Implements IM0_5_SmartAlternatorSetEfficiency.AlternatorsEfficiencyIdleResultCard
    Get
    Return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartIdleCurrent()).Efficiency
    End Get
End Property


Public ReadOnly property SmartTractionCurrent As Single Implements IM0_5_SmartAlternatorSetEfficiency.SmartTractionCurrent
    Get
 Return _resultCardTraction.GetSmartCurrentResult(HvacPlusNonBaseCurrents())
    End Get
End Property


Public ReadOnly Property  AlternatorsEfficiencyTractionOnResultCard() As Single Implements IM0_5_SmartAlternatorSetEfficiency.AlternatorsEfficiencyTractionOnResultCard
    Get
    Return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartTractionCurrent()).Efficiency
    End Get
End Property


Public ReadOnly property SmartOverrunCurrent As Single Implements IM0_5_SmartAlternatorSetEfficiency.SmartOverrunCurrent
    Get
       Return _resultCardOverrun.GetSmartCurrentResult(HvacPlusNonBaseCurrents())
    End Get
End Property


Public readonly property  AlternatorsEfficiencyOverrunResultCard() As single Implements IM0_5_SmartAlternatorSetEfficiency.AlternatorsEfficiencyOverrunResultCard
    Get
    Return _alternatorMap.GetEfficiency(_signals.EngineSpeed, SmartOverrunCurrent()).Efficiency
    End Get
End Property



Private function HvacPlusNonBaseCurrents() As Single

   Return _m0.GetHVACElectricalPowerDemandAmps() + _electricalConsumables.GetTotalAverageDemandAmps(true)

End Function

Public Sub new ( m0 As IM0_NonSmart_AlternatorsSetEfficiency, _
                 electricalConsumables as IElectricalConsumerList, _ 
                 alternatorMap As IAlternatorMap, _
                 resultCardIdle As IResultCard, _
                 resultCardTraction As IResultCard, _
                 resultCardOverrun As IResultCard,
                 signals As ISignals)

                 'Sanity Check on supplied arguments, throw an argument exception
                 If m0 is Nothing  then Throw New ArgumentException("Module 0 must be supplied")
                 If electricalConsumables is Nothing then Throw New ArgumentException("ElectricalConsumablesList must be supplied even if empty")   
                 If alternatorMap         is Nothing then throw new ArgumentException("Must supply a valid alternator map")
                 if  resultCardIdle       is nothing then throw new ArgumentException("Result Card 'IDLE' must be supplied even if it has no contents")
                 if  resultCardTraction   is nothing then throw new ArgumentException("Result Card 'TRACTION' must be supplied even if it has no contents")
                 if  resultCardOverrun    is nothing then throw new ArgumentException("Result Card 'OVERRUN' must be supplied even if it has no contents") 
                 If  signals              is Nothing then Throw New ArgumentException("No Signals Reference object was provided ")
 
                 'Assignments to private variables.
                 _m0=m0
                 _electricalConsumables   = electricalConsumables
                 _alternatorMap           = alternatorMap
                 _resultCardIdle          = resultCardIdle
                 _resultCardTraction      = resultCardTraction
                 _resultCardOverrun       = resultCardOverrun
                 _signals                 = signals

                                                                                                                             
End Sub                                      

End Class

End Namespace



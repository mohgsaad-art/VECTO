
Namespace Electrics


Public Class M0_5_SmartAlternatorSetEfficiency

private _m0 As IM0_NonSmart_AlternatorsSetEfficiency
Private _electricalConsumables As IElectricalConsumerList
Private _alternatorMap As IAlternatorMap
Private _resultCardIdle As IResultCard
Private _resultCardTraction As IResultCard
Private _resultCardOverrun As IResultCard


Public ReadOnly Property SmartIdleCurrent As single
    Get

    End Get
End Property
Public ReadOnly Property AlternatorsEfficiencyIdleResultCard As single
    Get

    End Get
End Property


Public readonly Property SmartTractionCurrent As Single
    Get

    End Get
End Property
Public readonly Property AlternatorsEfficiencyTractionOnResultCard As Single
    Get

    End Get
End Property


Public ReadOnly Property SmartOverrunCurrent As Single
    Get

    End Get
End Property
Public ReadOnly Property AlternatorsEfficiencyOverrunResultCard As single
    Get

    End Get
End Property



Public Sub new ( m0 As IM0_NonSmart_AlternatorsSetEfficiency, _
                 electricalConsumables as IElectricalConsumerList, _ 
                 alternatorMap As IAlternatorMap, _
                 resultCardIdle As IResultCard, _
                 resultCardTraction As IResultCard, _
                 resultCardOverrun As IResultCard)

                 'Sanity Check on supplied arguments, throw an argument exception
                 If m0 is Nothing  then Throw New ArgumentException("Module 0 must be supplied")
                 If electricalConsumables is Nothing then Throw New ArgumentException("ElectricalConsumablesList must be supplied even if empty")   
                 If alternatorMap        is Nothing then throw new ArgumentException("Must supply a valid alternator map")
                 if  resultCardIdle      is nothing then throw new ArgumentException("Result Card 'IDLE' must be supplied even if it has no contents")
                 if  resultCardTraction  is nothing then throw new ArgumentException("Result Card 'TRACTION' must be supplied even if it has no contents")
                 if  resultCardOverrun   is nothing then throw new ArgumentException("Result Card 'OVERRUN' must be supplied even if it has no contents")               
 
                 'Assignments to private variables.
                 _electricalConsumables   = electricalConsumables
                 _alternatorMap           = alternatorMap
                 _resultCardIdle          = resultCardIdle
                 _resultCardTraction      = resultCardTraction
                 _resultCardOverrun       = resultCardOverrun
                                                                                                                             
End Sub                                      




End Class

End Namespace



Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M4_Mock
 Implements IM4_AirCompressor


Public Property _AveragePowerDemandPerCompressorUnitFlowRate As Single
Public Property _FlowRate                                    As Single
Public Property _PowerCompressorOff                          As single
Public Property _PowerCompressorOn                           As Single
Public Property _PowerDifference                             As single

    Public Function GetAveragePowerDemandPerCompressorUnitFlowRate() As Single Implements IM4_AirCompressor.GetAveragePowerDemandPerCompressorUnitFlowRate
       Return _AveragePowerDemandPerCompressorUnitFlowRate
    End Function

    Public Function GetFlowRate() As Single Implements IM4_AirCompressor.GetFlowRate
       Return _FlowRate
    End Function

    Public Function GetPowerCompressorOff() As Single Implements IM4_AirCompressor.GetPowerCompressorOff
        Return _PowerCompressorOff
    End Function

    Public Function PowerCompressorOn() As Single Implements IM4_AirCompressor.GetPowerCompressorOn
      Return _PowerCompressorOn
    End Function

    Public Function GetPowerDifference() As Single Implements IM4_AirCompressor.GetPowerDifference
       Return _PowerDifference
    End Function


   Public Sub new(AveragePowerDemandPerCompressorUnitFlowRate as single , _
                  FlowRate           As single, _
                  PowerCompressorOff As Single, _
                  PowerCompressorOn  As Single, _
                  PowerDifference    As Single)
                               
                 _AveragePowerDemandPerCompressorUnitFlowRate = AveragePowerDemandPerCompressorUnitFlowRate
                 _FlowRate                                    = FlowRate
                 _PowerCompressorOff                          = PowerCompressorOff
                 _PowerCompressorOn                           = PowerCompressorOn
                 _PowerDifference                             = PowerDifference
                                                                                           
   End Sub                                                                       
                                                                                  
                                                                                    
    'Non Essential 
    Public Function Initialise() As Boolean Implements IM4_AirCompressor.Initialise
     Return true
    End Function

    Public Property PulleyGearEfficiency As Single Implements IM4_AirCompressor.PulleyGearEfficiency
    Public Property PulleyGearRatio As Single Implements IM4_AirCompressor.PulleyGearRatio



End Class


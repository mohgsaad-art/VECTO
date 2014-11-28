Imports AdvancedAuxiliaryInterfaces.Electrics
Imports AdvancedAuxiliaryInterfaces.Pneumatics
Imports AdvancedAuxiliaryInterfaces.Hvac

Namespace DownstreamModules


Public Interface IM9


 sub ClearAggregates() 
 Sub CycleStep( Optional stepTimeInSeconds As Single = nothing)

 ReadOnly Property LitresOfAirCompressorOnContinually As Single
 ReadOnly Property LitresOfAirCompressorOnOnlyInOverrun As Single 
 readonly property TotalCycleFuelConsumptionCompressorOnContinuously as single
 readonly property TotalCycleFuelConsumptionCompressorOffContinuously as single


End Interface


End Namespace








Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules


Public Interface IM11

ReadOnly Property SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly As Single
ReadOnly Property SmartElectricalTotalCycleEletricalEnergyGenerated As single
ReadOnly Property TotalCycleElectricalDemand As single
ReadOnly Property TotalCycleFuelConsumptionSmartElectricalLoad As single
ReadOnly Property TotalCycleFuelConsumptionZeroElectricalLoad As single
ReadOnly Property StopStartSensitiveTotalCycleElectricalDemand As Single



 Sub ClearAggregates()  

      
 Sub CycleStep(Optional stepTimeInSeconds As Single = 0.0) 


End Interface



End Namespace





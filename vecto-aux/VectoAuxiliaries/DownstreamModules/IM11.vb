' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

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





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
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules



Public Interface IM10

'Interpolated FC between points 2-1 Representing non-smart Pneumatics = BaseFuel Consumption with average auxillary loads

'changed to ASP MPreston 8/1/14

'AverageLoadsFuelConsumptionInterpolatedForPneumatics

ReadOnly Property AverageLoadsFuelConsumptionInterpolatedForPneumatics As Single


'Interpolated FC between points 2-3-1 Representing smart Pneumatics = Fuel consumption with smart Pneumatics and average electrical  power demand
ReadOnly Property FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand As Single

 Sub CycleStep( Optional stepTimeInSeconds As Single = nothing)


ReadOnly Property P1X as single

ReadOnly Property P1Y as single

ReadOnly Property P2X as single

ReadOnly Property P2Y  as single

ReadOnly Property P3X as single

ReadOnly Property P3Y as single

ReadOnly Property XTAIN  as single

ReadOnly Property INTRP1  as single

ReadOnly Property INTRP2   as single
  




End Interface



End Namespace


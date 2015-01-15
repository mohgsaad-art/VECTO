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

  Public Interface IM9
Inherits IAuxiliaryEvent


   ''' <summary>
  ''' Clears aggregated values ( Sets them to zero )
  ''' </summary>
  ''' <remarks></remarks>
   Sub ClearAggregates() 

   ''' <summary>
  ''' Increments all aggregated outputs
  ''' </summary>
  ''' <param name="stepTimeInSeconds">Single : Mutiplies the values to be aggregated by number of seconds</param>
  ''' <remarks></remarks>
   Sub CycleStep( Optional stepTimeInSeconds As Single = nothing)

   ''' <summary>
 ''' Litres Of Air: Compressor On Continually (L)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks>Start/Stop Sensitive</remarks>
   ReadOnly Property LitresOfAirCompressorOnContinually                 As Single

   ''' <summary>
 ''' Litres Of Air Compressor On Only In Overrun (L)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
   ReadOnly Property LitresOfAirCompressorOnOnlyInOverrun               As Single 

   ''' <summary>
 ''' Total Cycle Fuel Consumption Compressor *On* Continuously (G)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
   Readonly property TotalCycleFuelConsumptionCompressorOnContinuously  As single

   ''' <summary>
 ''' Total Cycle Fuel Consumption Compressor *OFF* Continuously (G)
 ''' </summary>
 ''' <value></value>
 ''' <returns></returns>
 ''' <remarks></remarks>
   Readonly property TotalCycleFuelConsumptionCompressorOffContinuously As single


End Interface

End Namespace








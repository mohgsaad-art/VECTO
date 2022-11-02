// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
{
	public interface IM9 : IAbstractModule // ,IAuxiliaryEvent
	{

		/// <summary>
		/// 	Clears aggregated values ( Sets them to zero )
		/// 	</summary>
		/// 	<remarks></remarks>
		void ClearAggregates();

		/// <summary>
		/// 	Increments all aggregated outputs
		/// 	</summary>
		/// 	<param name="stepTimeInSeconds">Single : Mutiplies the values to be aggregated by number of seconds</param>
		/// 	<remarks></remarks>
		void CycleStep(Second stepTimeInSeconds);

		/// <summary>
		/// 	Litres Of Air: Compressor On Continually (L)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks>Start/Stop Sensitive</remarks>
		NormLiter LitresOfAirCompressorOnContinually { get; }

		/// <summary>
		/// 	Litres Of Air Compressor On Only In Overrun (L)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		NormLiter LitresOfAirCompressorOnOnlyInOverrun { get; }

		/// <summary>
		/// 	Total Cycle Fuel Consumption Compressor *On* Continuously (G)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Kilogram TotalCycleFuelConsumptionCompressorOnContinuously { get; }

		/// <summary>
		/// 	Total Cycle Fuel Consumption Compressor *OFF* Continuously (G)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Kilogram TotalCycleFuelConsumptionCompressorOffContinuously { get; }
	}
}

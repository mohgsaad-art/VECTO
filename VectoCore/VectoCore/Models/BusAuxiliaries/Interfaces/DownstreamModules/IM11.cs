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
	public interface IM11 : IAbstractModule
	{

		/// <summary>
		/// Smart Electrical Total Cycle Electrical Energy Generated During Overrun Only(J)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Joule SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly { get; }

		/// <summary>
		/// Smart Electrical Total Cycle Eletrical EnergyGenerated (J)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Joule SmartElectricalTotalCycleEletricalEnergyGenerated { get; }

		/// <summary>
		/// Total Cycle Electrical Demand (J)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Joule TotalCycleElectricalDemand { get; }

		/// <summary>
		/// Total Cycle Fuel Consumption: Smart Electrical Load (g)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Kilogram TotalCycleFuelConsumptionSmartElectricalLoad { get; }

		/// <summary>
		/// Total Cycle Fuel Consumption: Zero Electrical Load (g)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Kilogram TotalCycleFuelConsumptionZeroElectricalLoad { get; }

		/// <summary>
		/// Stop Start Sensitive: Total Cycle Electrical Demand (J)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Joule StopStartSensitiveTotalCycleElectricalDemand { get; }

		/// <summary>
		/// Total Cycle Fuel Consuption : Average Loads (g)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Kilogram TotalCycleFuelConsuptionAverageLoads { get; }

		/// <summary>
		/// Clears aggregated values ( Sets them to zero ).
		/// </summary>
		/// <remarks></remarks>
		void ClearAggregates();

		/// <summary>
		/// Increments all aggregated outputs
		/// </summary>
		/// <param name="stepTimeInSeconds">Single : Mutiplies the values to be aggregated by number of seconds</param>
		/// <remarks></remarks>
		void CycleStep(Second stepTimeInSeconds);
	}
}

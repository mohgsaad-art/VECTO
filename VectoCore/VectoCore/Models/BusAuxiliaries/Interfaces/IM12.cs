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
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
{
	public interface IM12 : IAbstractModule
	{

		/// <summary>
		/// Fuel consumption with smart Electrics and Average Pneumatic Power Demand
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Kilogram FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand { get; }

		/// <summary>
		/// Base Fuel Consumption With Average Auxiliary Loads
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		Kilogram BaseFuelConsumptionWithTrueAuxiliaryLoads { get; }

		/// <summary>
		/// Stop Start Correction
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		double StopStartCorrection { get; }
	}
}

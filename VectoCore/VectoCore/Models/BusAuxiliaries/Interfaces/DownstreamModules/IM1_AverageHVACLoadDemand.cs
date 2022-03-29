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
	public interface IM1_AverageHVACLoadDemand : IAbstractModule
	{

		/// <summary>
		/// 	Average Power Demand At Crank From HVAC Mechanicals (W)
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AveragePowerDemandAtCrankFromHVACMechanicals { get; }

		/// <summary>
		/// 	Average Power Demand At Alternator From HVAC Electrics (W)
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AveragePowerDemandAtAlternatorFromHVACElectrics { get; }

		/// <summary>
		/// 	Average Power Demand At Crank From HVAC Electrics  (W)
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AveragePowerDemandAtCrankFromHVACElectrics { get; }

		///// <summary>
		///// 	HVAC Fueling   (L/H)
		///// 	</summary>
		///// 	<returns>Litres Per Hour</returns>
		///// 	<remarks></remarks>
		//KilogramPerSecond HVACFueling();
	}
}

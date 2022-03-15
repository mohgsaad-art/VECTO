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
	public interface IM7 : IAbstractModule
	{

		/// <summary>
		/// 	Smart Electrical And Pneumatic Aux: Alternator Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartElectricalAndPneumaticAuxAltPowerGenAtCrank { get; }

		/// <summary>
		/// 	Smart Electrical And Pneumatic Aux : Air Compressor Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank { get; }

		/// <summary>
		/// 	Smart Electrical Only Aux : Alternator Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartElectricalOnlyAuxAltPowerGenAtCrank { get; }

		/// <summary>
		/// 	Smart Pneumatic Only Aux : Air Comppressor Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartPneumaticOnlyAuxAirCompPowerGenAtCrank { get; }
	}
}

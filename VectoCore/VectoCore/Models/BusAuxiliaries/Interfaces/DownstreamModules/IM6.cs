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
	public interface IM6 : IAbstractModule
	{

		/// <summary>
		/// 	OverrunFlag
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns>0 = Not in overrun, 1 = In Overrun</returns>
		/// 	<remarks></remarks>
		bool OverrunFlag { get; }

		/// <summary>
		/// 	Smart Pneumatics Only CompressorFlag
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns>Less than Zero = No, Greater then Zero = Yes </returns>
		/// 	<remarks></remarks>
		bool SmartPneumaticsOnlyCompressorFlag { get; }

		/// <summary>
		/// 	Smart Elec And Pneumatics Compressor Flag
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		bool SmartElecAndPneumaticsCompressorFlag { get; }

		/// <summary>
		/// 	Smart Elec And Pneumatic: Alternator Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartElecAndPneumaticAltPowerGenAtCrank { get; }

		/// <summary>
		/// 	Smart Elec And Pneumatic: Air Compressor Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartElecAndPneumaticAirCompPowerGenAtCrank { get; }

		/// <summary>
		/// 	Smart Electrics Only :  Alternator Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartElecOnlyAltPowerGenAtCrank { get; }

		/// <summary>
		/// 	Average Power Demand At Crank From Pneumatics (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AveragePowerDemandAtCrankFromPneumatics { get; }

		/// <summary>
		/// 	Smart Pneumatic Only Air Comp Power Gen At Crank (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt SmartPneumaticOnlyAirCompPowerGenAtCrank { get; }

		/// <summary>
		/// 	Avgerage Power Demand At Crank From Electrics Including HVAC electrics (W)
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt AvgPowerDemandAtCrankFromElectricsIncHVAC { get; }

		
	}
}

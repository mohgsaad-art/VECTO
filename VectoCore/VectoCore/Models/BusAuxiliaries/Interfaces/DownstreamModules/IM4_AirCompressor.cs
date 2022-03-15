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
	public interface IM4_AirCompressor : IAbstractModule
	{

		/// <summary>
		/// 	Ratio of Gear or Pulley used to drive the compressor
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		double PulleyGearRatio { get; set; }

		/// <summary>
		/// 	Efficiency of the Pulley or Gear used to drive the compressor
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		double PulleyGearEfficiency { get; set; }

		
		/// <summary>
		/// 	Returns the flow rate [litres/second] of compressor for the given engine rpm
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		NormLiterPerSecond GetFlowRate();

		/// <summary>
		/// 	Returns the power consumed for the given engine rpm when compressor is off
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt GetPowerCompressorOff();

		/// <summary>
		/// 	Returns the power consumed for the given engine rpm when compressor is on
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		Watt GetPowerCompressorOn();

		/// <summary>
		/// 	Returns the difference in power between compressonr on and compressor off operation at the given engine rpm
		/// 	</summary>
		/// 	<returns>Single / Watts</returns>
		/// 	<remarks></remarks>
		Watt GetPowerDifference();

		/// <summary>
		/// 	Returns Average PoweDemand PeCompressor UnitFlowRate 
		/// 	</summary>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		JoulePerNormLiter GetAveragePowerDemandPerCompressorUnitFlowRate();
	}
}

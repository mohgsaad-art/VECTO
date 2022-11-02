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

using System.Diagnostics;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface ICompressorMap //: IAuxiliaryEvent
	{

		/// <summary>
		/// 	Initilaises the map from the supplied csv data
		/// 	</summary>
		/// 	<remarks></remarks>
		//bool Initialise();

		///// <summary>
		///// 	Returns compressor flow rate at the given rotation speed
		///// 	</summary>
		///// 	<param name="rpm">compressor rotation speed</param>
		///// 	<returns></returns>
		///// 	<remarks>Single</remarks>
		//NormLiterPerSecond GetFlowRate(PerSecond rpm);

		///// <summary>
		///// 	Returns mechanical power at rpm when compressor is on
		///// 	</summary>
		///// 	<param name="rpm">compressor rotation speed</param>
		///// 	<returns></returns>
		///// 	<remarks>Single</remarks>
		//Watt GetPowerCompressorOn(PerSecond rpm);

		///// <summary>
		///// 	Returns mechanical power at rpm when compressor is off
		///// 	</summary>
		///// 	<param name="rpm">compressor rotation speed</param>
		///// 	<returns></returns>
		///// 	<remarks>Single</remarks>
		//Watt GetPowerCompressorOff(PerSecond rpm);

		CompressorResult Interpolate(PerSecond rpm);

		// Returns Average Power Demand Per Compressor Unit FlowRate
		JoulePerNormLiter GetAveragePowerDemandPerCompressorUnitFlowRate();

		string Technology { get; }

		string Source { get; }
	}

	[DebuggerDisplay("{RPM} {FlowRate} {PowerOn} {PowerOff} (exceeded: {BoundariesExceeded})")]
	public class CompressorResult
	{
		public bool BoundariesExceeded;
		public PerSecond RPM;
		public Watt PowerOn;
		public Watt PowerOff;
		public NormLiterPerSecond FlowRate;
	}
}

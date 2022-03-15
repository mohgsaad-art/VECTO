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

using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class PneumaticUserInputsConfig : IPneumaticUserInputsConfig
	{
		public ICompressorMap CompressorMap { get; set; }
		public double CompressorGearRatio { get; set; }
		public double CompressorGearEfficiency { get; set; }

		// pnmeumatic or electric
		public ConsumerTechnology AdBlueDosing { get; set; }

		// mechanical or electrical
		public ConsumerTechnology AirSuspensionControl { get; set; }

		// pneumatic or electric
		public ConsumerTechnology? Doors { get; set; }
		public Meter KneelingHeight { get; set; }

		//public bool RetarderBrake { get; set; }
		public bool SmartAirCompression { get; set; }
		public bool SmartRegeneration { get; set; }
	}
}

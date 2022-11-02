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

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	/// <summary>
	/// 	''' Compressor Flow Rate and Power Map
	/// 	''' </summary>
	/// 	''' <remarks></remarks>
	public class CompressorMap : ICompressorMap //, IAuxiliaryEvent
	{
		//private readonly string filePath;
		private JoulePerNormLiter _averagePowerDemandPerCompressorUnitFlowRateLitresperSec;

		//private bool _MapBoundariesExceeded;

		/// <summary>
		/// 		''' Dictionary of values keyed by the rpm valaues in the csv file
		/// 		''' Values are held as a tuple as follows
		/// 		''' Item1 = flow rate
		/// 		''' Item2 - power [compressor on] 
		/// 		''' Item3 - power [compressor off]
		/// 		''' </summary>
		/// 		''' <remarks></remarks>
		//private Dictionary<int, CompressorMapValues> map;

		private readonly IList<CompressorMapValues> Entries;

		// Returns the AveragePowerDemand  per unit flow rate in seconds.
		public JoulePerNormLiter GetAveragePowerDemandPerCompressorUnitFlowRate()
		{
			return _averagePowerDemandPerCompressorUnitFlowRateLitresperSec;
		}


		/// <summary>
		/// 		''' Creates a new instance of the CompressorMap class
		/// 		''' </summary>
		/// <param name="entries"></param>
		/// <param name="technology"></param>
		/// <param name="source"></param>
		/// ''' <remarks></remarks>
		public CompressorMap(IList<CompressorMapValues> entries, string technology, string source)
		{
			Source = source;
			Technology = technology;
			Entries = entries;
			var tmp = Entries.Where(x => !x.FlowRate.IsEqual(0)).ToList();
			if (tmp.Count == 0) {
				_averagePowerDemandPerCompressorUnitFlowRateLitresperSec = 0.SI<JoulePerNormLiter>();
			} else {
				var powerDividedByFlowRateSum = 0.0.SI<JoulePerNormLiter>();
				powerDividedByFlowRateSum = tmp.Aggregate(powerDividedByFlowRateSum, (current, entry) => current + (entry.PowerCompressorOn - entry.PowerCompressorOff) / entry.FlowRate);

				_averagePowerDemandPerCompressorUnitFlowRateLitresperSec =
					powerDividedByFlowRateSum / Entries.Count(x => !x.FlowRate.IsEqual(0));
			}
		}

		public string Technology { get; }

		public string Source { get; }


		public CompressorResult Interpolate(PerSecond rpm)
		{
			var retVal = new CompressorResult();
			var min = Entries.Min(x => x.CompressorSpeed);
			var max = Entries.Max(x => x.CompressorSpeed);
			if (rpm < min || rpm > max) {
				retVal.BoundariesExceeded = true;
				rpm = rpm.LimitTo(min, max);
			}

			var s = Entries.GetSection(e => e.CompressorSpeed < rpm);

			retVal.RPM = rpm;
			retVal.FlowRate = VectoMath.Interpolate(
				s.Item1.CompressorSpeed, s.Item2.CompressorSpeed, s.Item1.FlowRate, s.Item2.FlowRate, rpm);
			retVal.PowerOn = VectoMath.Interpolate(
				s.Item1.CompressorSpeed, s.Item2.CompressorSpeed, s.Item1.PowerCompressorOn, s.Item2.PowerCompressorOn, rpm);
			retVal.PowerOff = VectoMath.Interpolate(
				s.Item1.CompressorSpeed, s.Item2.CompressorSpeed, s.Item1.PowerCompressorOff, s.Item2.PowerCompressorOff, rpm);

			return retVal;
		}



		


		//public event MessageEventHandler Message;

		//public delegate void MessageEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

		//private void OnMessage(object sender, string message, AdvancedAuxiliaryMessageType messageType)
		//{
		//	if (message != null) {
		//		object compressorMap = this;
		//		Message?.Invoke(ref compressorMap, message, messageType);
		//	}
		//}

		#region Implementation of IAuxiliaryEvent

		//public event AuxiliaryEventEventHandler AuxiliaryEvent;

		#endregion
	}
}

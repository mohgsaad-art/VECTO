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

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class ElectricsUserInputsConfig : IElectricsUserInputsConfig
	{
		public Volt PowerNetVoltage { get; set; }
		public IAlternatorMap AlternatorMap { get; set; }
		public double AlternatorGearEfficiency { get; set; }


		

		public Second DoorActuationTimeSecond { get; set; }
		public double StoredEnergyEfficiency { get; set; }

		public IResultCard ResultCardIdle { get; set; }
		public IResultCard ResultCardTraction { get; set; }
		public IResultCard ResultCardOverrun { get; set; }

		public AlternatorType AlternatorType { get; set; }

		[JsonIgnore]
		public Dictionary<string, ElectricConsumerEntry> ElectricalConsumers { get; set; }

		public string[] ElectricalConsumersSerialized
		{
			get {
				return ElectricalConsumers.Select(
											x =>
												$"{x.Key}: {x.Value.Current}  (Base: {x.Value.BaseVehicle}, active ESS standstill: {x.Value.ActiveDuringEngineStopStandstill} active ESS driving: {x.Value.ActiveDuringEngineStopDriving}")
										.ToArray();
			}
		}

		public Ampere AverageCurrentDemand => AverageCurrentDemandInclBaseLoad(false, false);

		public Ampere AverageCurrentDemandEngineOffStandstill => AverageCurrentDemandInclBaseLoad(true, true);

		public Ampere AverageCurrentDemandEngineOffDriving => AverageCurrentDemandInclBaseLoad(true, false);

		public Ampere AverageCurrentDemandWithoutBaseLoad(bool engineOff, bool vehicleStopped)
		{
			if (!engineOff) {
				return ElectricalConsumers?.Where(x => !x.Value.BaseVehicle).Select(x => x.Value.Current).Sum() ??
						0.SI<Ampere>();
			}
			
			if (vehicleStopped) {
				return ElectricalConsumers?.Where(x => !x.Value.BaseVehicle && x.Value.ActiveDuringEngineStopStandstill)
										.Select(x => x.Value.Current)
										.Sum().Cast<Ampere>() ?? 0.SI<Ampere>();
			}

			return ElectricalConsumers?.Where(x => !x.Value.BaseVehicle && x.Value.ActiveDuringEngineStopDriving)
									.Select(x => x.Value.Current)
									.Sum().Cast<Ampere>() ?? 0.SI<Ampere>();

		}

		public Ampere AverageCurrentDemandInclBaseLoad (bool engineOff, bool vehicleStopped)
		{
			if (!engineOff) {
				return ElectricalConsumers?.Select(x => x.Value.Current).Sum() ?? 0.SI<Ampere>();
			}

			if (vehicleStopped) {
				return ElectricalConsumers?.Where(x => x.Value.ActiveDuringEngineStopStandstill)
										.Select(x => x.Value.Current)
										.Sum().Cast<Ampere>() ?? 0.SI<Ampere>();
			}
			return ElectricalConsumers?.Where(x => x.Value.ActiveDuringEngineStopDriving)
									.Select(x => x.Value.Current)
									.Sum().Cast<Ampere>() ?? 0.SI<Ampere>();
		}

		public Watt MaxAlternatorPower { get; set; }
		
		public WattSecond ElectricStorageCapacity { get; set; }
		
		public bool ConnectESToREESS { get; set; }
		
		public double DCDCEfficiency { get; set; }
		
	}

}

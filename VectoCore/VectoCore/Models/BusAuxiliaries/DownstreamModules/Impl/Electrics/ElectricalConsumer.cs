using System;
using System.Collections.Generic;
using System.ComponentModel;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

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

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	/// <summary>
	/// 	''' Described a consumer of Alternator electrical power
	/// 	''' </summary>
	/// 	''' <remarks></remarks>
	public class ElectricalConsumer // : IElectricalConsumer
	{
		// Fields

		//private Volt _PowerNetVoltage;
		private readonly Dictionary<MissionType, Ampere> _missions = new Dictionary<MissionType, Ampere>();


		protected void ValidateInput(
			string category, string consumerName, Ampere nominalConsumptionAmps, double phaseIdleTractionOn, Volt powerNetVoltage,
			int numberInVehicle)
		{
			if (category.Trim().Length == 0) {
				throw new ArgumentException("Category Name cannot be empty");
			}
			if (consumerName.Trim().Length == 0) {
				throw new ArgumentException("ConsumerName Name cannot be empty");
			}
			if (phaseIdleTractionOn < Constants.BusAuxiliaries.ElectricConstants.PhaseIdleTractionOnMin |
				phaseIdleTractionOn > Constants.BusAuxiliaries.ElectricConstants.PhaseIdleTractionMax) {
				throw new ArgumentException(
					string.Format(
						"PhaseIdle_TractionOn must have a value between {0} and {1}",
						Constants.BusAuxiliaries.ElectricConstants.PhaseIdleTractionOnMin,
						Constants.BusAuxiliaries.ElectricConstants.PhaseIdleTractionMax));
			}
			if (nominalConsumptionAmps < Constants.BusAuxiliaries.ElectricConstants.NonminalConsumerConsumptionAmpsMin |
				nominalConsumptionAmps > Constants.BusAuxiliaries.ElectricConstants.NominalConsumptionAmpsMax) {
				throw new ArgumentException(
					string.Format(
						"NominalConsumptionAmps must have a value between {0} and {1}",
						Constants.BusAuxiliaries.ElectricConstants.NonminalConsumerConsumptionAmpsMin,
						Constants.BusAuxiliaries.ElectricConstants.NominalConsumptionAmpsMax));
			}
			if (powerNetVoltage < Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMin |
				powerNetVoltage > Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException(
					string.Format(
						"PowerNetVoltage must have a value between {0} and {1}",
						Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMin,
						Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMax));
			}
			if (numberInVehicle < 0) {
				throw new ArgumentException("Cannot have less than 0 consumers in the vehicle");
			}
		}


		// Properties

		public bool BaseVehicle { get; set; }

		public string Category { get; set; }

		public string ConsumerName { get; set; }

		public string NumberInActualVehicle { get; set; }

		public double PhaseIdleTractionOn { get; set; }
		public bool Bonus { get; set; }
		public bool DefaultConsumer { get; set; }

		//public Volt PowerNetVoltage
		//{
		//	get { return _PowerNetVoltage; }
		//	set {
		//		_PowerNetVoltage = value;
		//		NotifyPropertyChanged("PowerNetVoltage");
		//	}
		//}

		
		// Public class outputs
		//public Ampere TotalAvgConumptionAmps
		//{
		//	get { return NominalConsumptionAmps * (NumberInActualVehicle * PhaseIdle_TractionOn); }
		//}

		

		// Comparison Overrides
		//public override bool Equals(object obj)
		//{
		//	if (obj == null || GetType() != obj.GetType()) {
		//		return false;
		//	}

		//	var other = (IElectricalConsumer)obj;

		//	return ConsumerName == other.ConsumerName;
		//}

		//[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		//public override int GetHashCode()
		//{
		//	return 0;
		//}


		//public event PropertyChangedEventHandler PropertyChanged;

		//private void NotifyPropertyChanged(string p)
		//{
		//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
		//}

		public Ampere NominalCurrent(MissionType mission)
		{
			return _missions.GetValueOrDefault(mission, 0.SI<Ampere>());
		}

		public Ampere this[MissionType mission]
		{
			set {
				if (_missions.ContainsKey(mission)) {
					throw new VectoException("key {0} already exists!", mission.ToString());
				}

				_missions[mission] = value;
			}
		}

		
	}
}

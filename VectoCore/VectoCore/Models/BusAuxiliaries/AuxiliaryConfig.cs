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
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries
{
	public class AuxiliaryConfig : IAuxiliaryConfig
	{
		[JsonIgnore]
		public IBusAuxiliariesDeclarationData InputData { get; internal set; }

		// Electrical
		public IElectricsUserInputsConfig ElectricalUserInputsConfig { get; internal set; }

		// Pneumatics
		public IPneumaticUserInputsConfig PneumaticUserInputsConfig { get; internal set; }

		public IPneumaticsConsumersDemand PneumaticAuxillariesConfig { get; internal set; }

		public ISSMInputs SSMInputs { get; internal set; }

		//public IActuationsMap ActuationsMap { get; internal set; }
		public IActuations Actuations { get; internal set; }


		[JsonIgnore]
		public IVehicleData VehicleData { get; internal set; }

		//public IFuelConsumptionMap FuelMap { get; internal set; }

		// Vecto Signals
		//public ISignals Signals { get; internal set; }

		// Constructors


		private bool CompareElectricalConfiguration(IAuxiliaryConfig other)
		{
			// AlternatorGearEfficiency
			if (ElectricalUserInputsConfig.AlternatorGearEfficiency !=
				other.ElectricalUserInputsConfig.AlternatorGearEfficiency) {
				return false;
			}

			// AlternatorMap
			if (!ElectricalUserInputsConfig.AlternatorMap.Equals(other.ElectricalUserInputsConfig.AlternatorMap)) {
				return false;
			}

			// DoorActuationTimeSecond
			if (ElectricalUserInputsConfig.DoorActuationTimeSecond != other.ElectricalUserInputsConfig.DoorActuationTimeSecond) {
				return false;
			}

			//if (ElectricalUserInputsConfig.AverageCurrentDemandInclBaseLoad !=
			//	other.ElectricalUserInputsConfig.AverageCurrentDemandInclBaseLoad) {
			//	return false;
			//}

			//if (ElectricalUserInputsConfig.AverageCurrentDemandWithoutBaseLoad !=
			//	other.ElectricalUserInputsConfig.AverageCurrentDemandWithoutBaseLoad) {
			//	return false;
			//}

			// PowerNetVoltage
			if (ElectricalUserInputsConfig.PowerNetVoltage != other.ElectricalUserInputsConfig.PowerNetVoltage) {
				return false;
			}

			try {
				// ResultCardIdle
				if (ElectricalUserInputsConfig.ResultCardIdle.Entries
											.ZipAll(other.ElectricalUserInputsConfig.ResultCardIdle.Entries, Tuple.Create).Any(
												t => !t.Item1.Key.IsEqual(t.Item2.Key) || !t.Item1.Value.IsEqual(t.Item2.Value))) {
					return false;
				}

				// ResultCardOverrun
				if (ElectricalUserInputsConfig.ResultCardOverrun.Entries
											.ZipAll(other.ElectricalUserInputsConfig.ResultCardOverrun.Entries, Tuple.Create).Any(
												t => !t.Item1.Key.IsEqual(t.Item2.Key) || !t.Item1.Value.IsEqual(t.Item2.Value))) {
					return false;
				}

				// ResultCardTraction
				if (ElectricalUserInputsConfig.ResultCardTraction.Entries
											.ZipAll(other.ElectricalUserInputsConfig.ResultCardTraction.Entries, Tuple.Create).Any(
												t => !t.Item1.Key.IsEqual(t.Item2.Key) || !t.Item1.Value.IsEqual(t.Item2.Value))) {
					return false;
				}
			} catch (Exception) {
				// zipall may throw an exeption...
				return false;
			}

			// SmartElectrical
			if (ElectricalUserInputsConfig.AlternatorType != other.ElectricalUserInputsConfig.AlternatorType) {
				return false;
			}

			return true;
		}

		private bool ComparePneumaticAuxiliariesConfig(IAuxiliaryConfig other)
		{
			if (PneumaticAuxillariesConfig.AdBlueInjection != other.PneumaticAuxillariesConfig.AdBlueInjection) {
				return false;
			}
			if (PneumaticAuxillariesConfig.AirControlledSuspension !=
				other.PneumaticAuxillariesConfig.AirControlledSuspension) {
				return false;
			}
			if (PneumaticAuxillariesConfig.Braking != other.PneumaticAuxillariesConfig.Braking) {
				return false;
			}

			//if (PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG !=
			//	other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG) {
			//	return false;
			//}
			if (PneumaticAuxillariesConfig.BreakingWithKneeling !=
				other.PneumaticAuxillariesConfig.BreakingWithKneeling) {
				return false;
			}
			if (PneumaticAuxillariesConfig.DeadVolBlowOuts !=
				other.PneumaticAuxillariesConfig.DeadVolBlowOuts) {
				return false;
			}
			if (PneumaticAuxillariesConfig.DeadVolume != other.PneumaticAuxillariesConfig.DeadVolume) {
				return false;
			}
			if (PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand !=
				other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand) {
				return false;
			}
			if (PneumaticAuxillariesConfig.DoorOpening != other.PneumaticAuxillariesConfig.DoorOpening) {
				return false;
			}
			if (PneumaticAuxillariesConfig.StopBrakeActuation !=
				other.PneumaticAuxillariesConfig.StopBrakeActuation) {
				return false;
			}
			if (PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand !=
				other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand) {
				return false;
			}
			if (PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction !=
				other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction) {
				return false;
			}

			return true;
		}

		private bool ComparePneumaticUserConfig(IAuxiliaryConfig other)
		{
			//if (PneumaticUserInputsConfig.ActuationsMap != other.PneumaticUserInputsConfig.ActuationsMap)
			//	return false;
			if (PneumaticUserInputsConfig.AdBlueDosing != other.PneumaticUserInputsConfig.AdBlueDosing) {
				return false;
			}
			if (PneumaticUserInputsConfig.AirSuspensionControl != other.PneumaticUserInputsConfig.AirSuspensionControl) {
				return false;
			}
			if (PneumaticUserInputsConfig.CompressorGearEfficiency != other.PneumaticUserInputsConfig.CompressorGearEfficiency) {
				return false;
			}
			if (PneumaticUserInputsConfig.CompressorGearRatio != other.PneumaticUserInputsConfig.CompressorGearRatio) {
				return false;
			}
			if (PneumaticUserInputsConfig.CompressorMap != other.PneumaticUserInputsConfig.CompressorMap) {
				return false;
			}
			if (PneumaticUserInputsConfig.Doors != other.PneumaticUserInputsConfig.Doors) {
				return false;
			}
			if (PneumaticUserInputsConfig.KneelingHeight != other.PneumaticUserInputsConfig.KneelingHeight) {
				return false;
			}

			//if (PneumaticUserInputsConfig.RetarderBrake != other.PneumaticUserInputsConfig.RetarderBrake)
			//	return false;
			if (PneumaticUserInputsConfig.SmartAirCompression != other.PneumaticUserInputsConfig.SmartAirCompression) {
				return false;
			}
			if (PneumaticUserInputsConfig.SmartRegeneration != other.PneumaticUserInputsConfig.SmartRegeneration) {
				return false;
			}

			return true;
		}


		public bool ConfigValuesAreTheSameAs(IAuxiliaryConfig other)
		{
			if (!CompareElectricalConfiguration(other)) {
				return false;
			}
			if (!ComparePneumaticAuxiliariesConfig(other)) {
				return false;
			}
			if (!ComparePneumaticUserConfig(other)) {
				return false;
			}

			//if (!CompareHVACConfig(other))
			//	return false;

			return true;
		}


		public override bool Equals(object other)
		{
			var myOhter = other as AuxiliaryConfig;
			if (myOhter == null) {
				return false;
			}

			return ConfigValuesAreTheSameAs(myOhter);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

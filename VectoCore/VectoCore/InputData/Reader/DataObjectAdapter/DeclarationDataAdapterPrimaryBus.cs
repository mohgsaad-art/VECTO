using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterPrimaryBus : DeclarationDataAdapterHeavyLorry
	{
		#region Overrides of DeclarationDataAdapterHeavyLorry

		public override DriverData CreateDriverData()
		{
			var retVal = base.CreateDriverData();
			retVal.LookAheadCoasting.Enabled = true;
			return retVal;
		}


		public override VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			var retVal = base.CreateVehicleData(data, segment, mission, loading, allowVocational);
			if (data.ExemptedVehicle) {
				return retVal;
			}
			retVal.CurbMass = mission.CurbMass;
			retVal.GrossVehicleMass = 40000.SI<Kilogram>();
			return retVal;
		}

		protected override VehicleData CreateExemptedVehicleData(IVehicleDeclarationInputData data)
		{
			var exempted = new VehicleData {
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				//CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				VehicleCategory = data.VehicleCategory,
				//CurbMass = data.CurbMassChassis,
				GrossVehicleMass = data.GrossVehicleMassRating,
				AirDensity = Physics.AirDensity,
			};
			exempted.VIN = data.VIN;
			exempted.ManufacturerAddress = data.ManufacturerAddress;
			exempted.LegislativeClass = data.LegislativeClass;
			exempted.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			exempted.HybridElectricHDV = data.HybridElectricHDV;
			exempted.DualFuelVehicle = data.DualFuelVehicle;
			exempted.MaxNetPower1 = data.MaxNetPower1;
			exempted.MaxNetPower2 = data.MaxNetPower2;
			exempted.AxleConfiguration = data.AxleConfiguration;
			return exempted;
		}

		public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			return null;
		}

		public override IList<VectoRunData.AuxData> CreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxData, MissionType mission,
			VehicleClass hdvClass, Meter vehicleLength, int? numSteeredAxles)
		{
			if (auxInputData != null) {
				throw new VectoException("Only BusAuxiliaries can be provided as input!");
			}

			if (numSteeredAxles.HasValue && busAuxData.SteeringPumpTechnology.Count != numSteeredAxles.Value) {
				throw new VectoException($"Number of steering pump technologies does not match number of steered axles ({numSteeredAxles.Value}, {busAuxData.SteeringPumpTechnology.Count})");
			}
			var retVal = new List<VectoRunData.AuxData>();

			retVal.Add(
				new VectoRunData.AuxData() {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = new List<string>() { busAuxData.FanTechnology },
					ID = Constants.Auxiliaries.IDs.Fan,
					PowerDemand = DeclarationData.Fan.LookupMechanicalPowerDemand(hdvClass, mission, busAuxData.FanTechnology)
				});
			retVal.Add(
				new VectoRunData.AuxData() {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = busAuxData.SteeringPumpTechnology,
					ID = Constants.Auxiliaries.IDs.SteeringPump,
					PowerDemand = DeclarationData.SteeringPumpBus.LookupMechanicalPowerDemand(
						mission, busAuxData.SteeringPumpTechnology, vehicleLength)
				});
			return retVal;
		}

		#endregion

		public virtual IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);

			var retVal = new AuxiliaryConfig {
				InputData = vehicleData.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = GetElectricalUserConfig(mission, vehicleData, actuations, runData.VehicleData.VehicleClass),
				PneumaticUserInputsConfig = GetPneumaticUserConfig(vehicleData, mission),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputs = CreateSSMModelParameters(
					vehicleData.Components.BusAuxiliaries, mission, FuelData.Diesel, runData.Loading),
				VehicleData = runData.VehicleData,
			};

			return retVal;
		}

		protected virtual ElectricsUserInputsConfig GetElectricalUserConfig(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, vehicleData, actuations, vehicleClass);
			var busAux = vehicleData.Components.BusAuxiliaries;

			var retVal = GetDefaultElectricalUserConfig();

			retVal.AlternatorType = busAux.ElectricSupply.AlternatorTechnology;
			retVal.ElectricalConsumers = currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(CalculateAlternatorEfficiency(busAux.ElectricSupply.Alternators));

			switch (retVal.AlternatorType) {
				case AlternatorType.Smart when busAux.ElectricSupply.Alternators.Count == 0:
					throw new VectoException("at least one alternator is required when specifying smart electrics!");
				case AlternatorType.Smart when busAux.ElectricSupply.ElectricStorage.Count == 0:
					throw new VectoException("at least one electric storage (battery or capacitor) is required when specifying smart electrics!");
			}

			retVal.MaxAlternatorPower = busAux.ElectricSupply.Alternators.Sum(x => x.RatedVoltage * x.RatedCurrent);
			retVal.ElectricStorageCapacity = busAux.ElectricSupply.ElectricStorage.Sum(x => x.ElectricStorageCapacity) ?? 0.SI<WattSecond>();

			return retVal;
		}

		protected virtual ElectricsUserInputsConfig GetDefaultElectricalUserConfig()
		{
			return new ElectricsUserInputsConfig() {
				PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
				StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
				ResultCardIdle = new DummyResultCard(),
				ResultCardOverrun = new DummyResultCard(),
				ResultCardTraction = new DummyResultCard(),
				AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
				DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
			};
		}

		protected virtual double CalculateAlternatorEfficiency(IList<IAlternatorDeclarationInputData> alternators)
		{
			return  DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup("default");
		}

		protected virtual Dictionary<string, ElectricConsumerEntry> GetElectricConsumers(Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations, VehicleClass vehicleClass)
		{
			var retVal = GetDefaultElectricConsumers(mission, vehicleData, actuations);

			foreach (var entry in GetElectricAuxConsumers(mission, vehicleData, vehicleClass, vehicleData.Components.BusAuxiliaries)) {
				retVal[entry.Key] = entry.Value;
			}

			return retVal;
		}

		protected virtual Dictionary<string, ElectricConsumerEntry> GetDefaultElectricConsumers(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var doorDutyCycleFraction =
				(actuations.ParkBrakeAndDoors * Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond) /
				actuations.CycleTime;
			var busAux = vehicleData.Components.BusAuxiliaries;
			var electricDoors = vehicleData.DoorDriveTechnology == ConsumerTechnology.Electrically ||
				vehicleData.DoorDriveTechnology == ConsumerTechnology.Mixed;

			foreach (var consumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items) {
				var applied = consumer.DefaultConsumer || consumer.Bonus
					? 1.0
					: GetNumberOfElectricalConsumersForMission(mission, consumer, vehicleData);
				var nbr = consumer.DefaultConsumer
					? GetNumberOfElectricalConsumersInVehicle(consumer.NumberInActualVehicle, mission, vehicleData)
					: 1.0;

				var dutyCycle = electricDoors && consumer.ConsumerName.Equals(
									Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
									StringComparison.CurrentCultureIgnoreCase)
					? doorDutyCycleFraction
					: consumer.PhaseIdleTractionOn;

				var current = applied * consumer.NominalCurrent(mission.MissionType) * dutyCycle * nbr;
				if (consumer.Bonus && !VehicleHasElectricalConsumer(consumer.ConsumerName, busAux)) {
					current = 0.SI<Ampere>();
				}

				retVal[consumer.ConsumerName] = new ElectricConsumerEntry {
					BaseVehicle = consumer.BaseVehicle,
					Current = current
				};
			}

			return retVal;
		}

		private double GetNumberOfElectricalConsumersForMission(Mission mission, ElectricalConsumer consumer,
			IVehicleDeclarationInputData vehicleData)
		{
			if (consumer.ConsumerName.Equals(Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
				StringComparison.CurrentCultureIgnoreCase)) {
				var count = DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items.First(x =>
					x.ConsumerName.Equals(
						Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
						StringComparison.CurrentCultureIgnoreCase)).NumberInActualVehicle.ToDouble();

				switch (vehicleData.DoorDriveTechnology)
				{
					case ConsumerTechnology.Electrically:
						return count;
					case ConsumerTechnology.Mixed:
						return count * 0.5;
					default:
						return 0;
				}
			}

			return mission.BusParameter.ElectricalConsumers.GetVECTOValueOrDefault(consumer.ConsumerName, 0);
		}

		protected virtual Dictionary<string, ElectricConsumerEntry> GetElectricAuxConsumers(Mission mission, IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAux)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var spPower = DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(
				mission.MissionType, busAux.SteeringPumpTechnology,
				vehicleData.Length ?? mission.BusParameter.VehicleLength);
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry {
				ActiveDuringEngineStopStandstill = false,
				BaseVehicle = false,
				Current = spPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};

			var fanPower = DeclarationData.Fan.LookupElectricalPowerDemand(
				vehicleClass, mission.MissionType, busAux.FanTechnology);
			retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry {
				ActiveDuringEngineStopStandstill = false,
				ActiveDuringEngineStopDriving = false,
				BaseVehicle = false,
				Current = fanPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};
			return retVal;
		}

		protected virtual bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			switch (consumerName) {
				case "Day running lights LED bonus":
				case "Position lights LED bonus":
				case "Brake lights LED bonus": // return false;
				case "Interior lights LED bonus":
				case "Headlights LED bonus": return true;
				default: return false;
			}
		}

		protected virtual double GetNumberOfElectricalConsumersInVehicle(string nbr, Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase)) {
				return CalculateLengthDependentElectricalConsumers(mission, vehicleData);
			}

			return nbr.ToDouble();
		}

		protected virtual double CalculateLengthDependentElectricalConsumers(Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			var busParams = mission.BusParameter;
			return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
									busParams.VehicleLength, busParams.VehicleCode, busParams.NumberPassengersLowerDeck)
								.Value();
		}

		protected virtual PneumaticUserInputsConfig GetPneumaticUserConfig(IVehicleDeclarationInputData vehicleData, Mission mission)
		{
			var busAux = vehicleData.Components.BusAuxiliaries;

			//throw new NotImplementedException();
			return new PneumaticUserInputsConfig() {
				KneelingHeight = VectoMath.Max(0.SI<Meter>(), mission.BusParameter.EntranceHeight - Constants.BusParameters.EntranceHeight),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = busAux.PneumaticSupply.Ratio,
				CompressorMap = DeclarationData.BusAuxiliaries.GetCompressorMap(busAux.PneumaticSupply.CompressorSize, busAux.PneumaticSupply.Clutch),
				SmartAirCompression = busAux.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = busAux.PneumaticSupply.SmartRegeneration,
				AdBlueDosing = busAux.PneumaticConsumers.AdBlueDosing,
				Doors = ConsumerTechnology.Pneumatically,
				AirSuspensionControl = busAux.PneumaticConsumers.AirsuspensionControl,
			};
		}

		

		public SSMInputs GetDefaulSSMInputs(IFuelProperties heatingFuel)
		{
			return new SSMInputs(null, heatingFuel) {
				DefaultConditions = new EnvironmentalConditionMapEntry(
					Constants.BusAuxiliaries.SteadyStateModel.DefaultTemperature,
					Constants.BusAuxiliaries.SteadyStateModel.DefaultSolar,
					1.0),
				EnvironmentalConditionsMap = DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions,
				HeatingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.HeatingBoundaryTemperature,
				CoolingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.CoolingBoundaryTemperature,

				SpecificVentilationPower = Constants.BusAuxiliaries.SteadyStateModel.SpecificVentilationPower,

				AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
				FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
				CoolantHeatTransferredToAirCabinHeater = Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				GFactor = Constants.BusAuxiliaries.SteadyStateModel.GFactor,

				VentilationOnDuringHeating = true,
				VentilationWhenBothHeatingAndACInactive = true,
				VentilationDuringAC = true,

				MaxPossibleBenefitFromTechnologyList =
					Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList,
			};
		}

		public virtual ISSMDeclarationInputs CreateSSMModelParameters(IBusAuxiliariesDeclarationData busAuxInputData, Mission mission, IFuelProperties heatingFuel, LoadingType loadingType)
		{
			var busParams = mission.BusParameter;

			var isDoubleDecker = busParams.VehicleCode.IsDoubleDeckerBus();
			var internalLength = busParams.HVACConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength // OK
				: DeclarationData.BusAuxiliaries.CalculateInternalLength(busParams.VehicleLength, 
				busParams.VehicleCode, 10); // missing: correction length for low floor buses
			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(mission.BusParameter.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var coolingPower = CalculateMaxCoolingPower(null, null, mission);

			var retVal = GetDefaulSSMInputs(heatingFuel);
			retVal.BusFloorType = busParams.VehicleCode.GetFloorType();
			retVal.Technologies = GetSSMTechnologyBenefits(busAuxInputData, mission.BusParameter.VehicleCode.GetFloorType());

			retVal.FuelFiredHeaterPower = busParams.HVACAuxHeaterPower;
			retVal.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(busParams.DoubleDecker) * internalLength +
									DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(busParams.DoubleDecker);
			retVal.BusSurfaceArea = 2 * (internalLength * busParams.VehicleWidth + internalLength * internalHeight +
										(isDoubleDecker ? 2.0 : 1.0) * busParams.VehicleWidth * busParams.BodyHeight);
			retVal.BusVolume = internalLength * busParams.VehicleWidth * internalHeight;

			retVal.UValue = DeclarationData.BusAuxiliaries.UValue(busParams.VehicleCode.GetFloorType());
			retVal.NumberOfPassengers =
				DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(internalLength, busParams.VehicleWidth) *
				(loadingType == LoadingType.LowLoading ? mission.BusParameter.PassengerDensityLow : mission.BusParameter.PassengerDensityRef) *
				(loadingType == LoadingType.LowLoading ? mission.MissionType.GetLowLoadFactorBus() : 1.0) + 1; // add driver for 'heat input'
			retVal.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(busParams.HVACConfiguration, false);
			retVal.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(busParams.HVACConfiguration, true);

			retVal.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			retVal.HVACCompressorType = busParams.HVACCompressorType; // use passenger compartment
			retVal.COP = DeclarationData.BusAuxiliaries.CalculateCOP(
			
				coolingPower.Item1, HeatPumpType.none, coolingPower.Item2, busParams.HVACCompressorType,
				busParams.VehicleCode.GetFloorType());
			retVal.HVACTechnology = $"{busParams.HVACConfiguration.GetName()} " +
									$"({string.Join(", ", busParams.HVACCompressorType.GetName(), HeatPumpType.none.GetName())})";
			
			//SetHVACParameters(retVal, vehicleData, mission);

			return retVal;
		}


		protected virtual TechnologyBenefits GetSSMTechnologyBenefits(IBusAuxiliariesDeclarationData inputData, FloorType floorType)
		{
			var onVehicle = new List<SSMTechnology>();
			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList) {
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.AdjustableCoolantThermostat ?? false)) {
					onVehicle.Add(item);
				}

				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.EngineWasteGasHeatExchanger ?? false)) {
					onVehicle.Add(item);
				}
			}

			return SelectBenefitForFloorType(floorType, onVehicle);
		}

		protected virtual TechnologyBenefits SelectBenefitForFloorType(FloorType floorType, List<SSMTechnology> onVehicle)
		{
			var retVal = new TechnologyBenefits();

			switch (floorType) {
				case FloorType.LowFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.LowFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.LowFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.LowFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.LowFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.LowFloorV : 0);
					break;
				case FloorType.HighFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.RaisedFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.RaisedFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.RaisedFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.RaisedFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.RaisedFloorV : 0);
					break;
				case FloorType.SemiLowFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.SemiLowFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.SemiLowFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.SemiLowFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.SemiLowFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.SemiLowFloorV : 0);
					break;
			}

			return retVal;
		}


		protected virtual Tuple<Watt, Watt> CalculateMaxCoolingPower(IVehicleDeclarationInputData vehicleData, IVehicleDeclarationInputData primaryVehicle, Mission mission)
		{
			var busParams = mission.BusParameter;

			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				busParams.VehicleLength, busParams.VehicleCode,
				busParams.NumberPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(busParams.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var volume = length * height * busParams.VehicleWidth;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				busParams.HVACConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				busParams.HVACConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		protected internal virtual IPneumaticsConsumersDemand CreatePneumaticAuxConfig(RetarderType retarderType)
		{
			return new PneumaticsConsumersDemand() {
				AdBlueInjection = Constants.BusAuxiliaries.PneumaticConsumersDemands.AdBlueInjection,
				AirControlledSuspension = Constants.BusAuxiliaries.PneumaticConsumersDemands.AirControlledSuspension,
				Braking = retarderType == RetarderType.None
					? Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingNoRetarder
					: Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingWithRetarder,
				BreakingWithKneeling = Constants.BusAuxiliaries.PneumaticConsumersDemands.BreakingAndKneeling,
				DeadVolBlowOuts = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolBlowOuts,
				DeadVolume = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolume,
				NonSmartRegenFractionTotalAirDemand =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.NonSmartRegenFractionTotalAirDemand,
				SmartRegenFractionTotalAirDemand =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.SmartRegenFractionTotalAirDemand,
				OverrunUtilisationForCompressionFraction =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
				DoorOpening = Constants.BusAuxiliaries.PneumaticConsumersDemands.DoorOpening,
				StopBrakeActuation = Constants.BusAuxiliaries.PneumaticConsumersDemands.StopBrakeActuation,
			};
		}
	}

	public class ElectricConsumerEntry {

		public ElectricConsumerEntry()
		{
			ActiveDuringEngineStopStandstill = true;
			ActiveDuringEngineStopDriving = true;
		}

		public bool ActiveDuringEngineStopDriving { get; set; }

		public bool ActiveDuringEngineStopStandstill { get; set; }

		public bool BaseVehicle { get; set; }
		public Ampere Current { get; set; }
	}
}

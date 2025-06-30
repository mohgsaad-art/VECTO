using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter
{
    public class PrimaryBusAuxiliaryDataAdapter : AuxiliaryDataAdapter, IPrimaryBusAuxiliaryDataAdapter
	{
		public override AuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);

			var (ssmCooling, ssmHeating) = GetPrimarySSMInput(mission, primaryVehicle, runData);

			var electricUserInputs =
				GetElectricalUserConfig(mission, primaryVehicle, actuations, runData.VehicleData.VehicleClass);

			var pneumaticUserInputsConfig = GetPneumaticUserConfig(primaryVehicle, mission);
			var pneumaticAuxiliariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type);
			
			if (primaryVehicle.Components.BusAuxiliaries.PneumaticSupply.CompressorDrive == CompressorDrive.electrically)
			{
				var auxConfig = new AuxiliaryConfig
				{
					VehicleData = runData.VehicleData,
					PneumaticAuxiliariesConfig = pneumaticAuxiliariesConfig,
					PneumaticUserInputsConfig = pneumaticUserInputsConfig
				};
				var airDemand = M03Impl.TotalAirDemandCalculation(auxConfig, actuations) / actuations.CycleTime;
				electricUserInputs.ElectricalConsumers[Constants.Auxiliaries.IDs.PneumaticSystem] =
					new ElectricConsumerEntry()
					{
						Current = DeclarationData.BusAuxiliaries.PneumaticSystemElectricDemandPerAirGenerated * airDemand / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
						ActiveDuringEngineStopDriving = true,
						ActiveDuringEngineStopStandstill = true,
						BaseVehicle = false
					};
			}

			var retVal = new AuxiliaryConfig
			{
				InputData = primaryVehicle.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = electricUserInputs,
				PneumaticUserInputsConfig = pneumaticUserInputsConfig,
				PneumaticAuxiliariesConfig = pneumaticAuxiliariesConfig,
				Actuations = actuations,
				SSMInputsCooling = ssmCooling,
				SSMInputsHeating = ssmHeating,
				VehicleData = runData.VehicleData,
			};

			return retVal;
		}

		private (SSMInputs ssmCooling, SSMInputs ssmHeating) GetPrimarySSMInput(Mission mission,
			IVehicleDeclarationInputData primaryVehicle, VectoRunData runData)
		{
			var hvacParams = GetHVACParams(primaryVehicle.VehicleType, mission.BusParameter);

			var applicableHVACConfigCooling = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacParams.HVACConfiguration,
				HeatPumpType.none, hvacParams.HeatPumpTypePassengerCompartmentCooling);
			var applicableHVACConfigHeating = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacParams.HVACConfiguration,
				HeatPumpType.none, hvacParams.HeatPumpTypePassengerCompartmentHeating);

			// Diesel is hardcoded to calculate SSM parameters, but is unused and does not influence output.
			var fuelData = FuelData.Diesel;

			var ssmCooling = CreatePrimarySSMModelParameters(primaryVehicle.Components.BusAuxiliaries, mission,
				runData.Loading, applicableHVACConfigCooling, hvacParams, fuelData, true);
			var ssmHeating = CreatePrimarySSMModelParameters(primaryVehicle.Components.BusAuxiliaries, mission,
				runData.Loading, applicableHVACConfigHeating, hvacParams, fuelData, false);
			ssmHeating.ElectricHeater = GetElectricHeater(mission, runData);
			ssmHeating.HeatingDistributions = DeclarationData.BusAuxiliaries.HeatingDistributionCases;
			return (ssmCooling, ssmHeating);
		}

		protected internal virtual HashSet<AuxiliaryType> AuxiliaryTypes { get; } = new HashSet<AuxiliaryType>() {
			AuxiliaryType.Fan,
			AuxiliaryType.SteeringPump
		};
		protected virtual string errorStringVehicleType => "conventional/hybrid";

		public ElectricsUserInputsConfig GetDefaultElectricalUserConfig()
		{
			return new ElectricsUserInputsConfig()
			{
				PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
				StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
				ResultCardIdle = new DummyResultCard(),
				ResultCardOverrun = new DummyResultCard(),
				ResultCardTraction = new DummyResultCard(),
				AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
				DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
			};
		}

		private double GetNumberOfElectricalConsumersForMission(Mission mission, ElectricalConsumer consumer,
			IVehicleDeclarationInputData vehicleData)
		{
			if (consumer.ConsumerName.Equals(Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
					StringComparison.CurrentCultureIgnoreCase))
			{
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

		protected virtual Dictionary<string, ElectricConsumerEntry> GetDefaultElectricConsumers(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var doorDutyCycleFraction =
				actuations.ParkBrakeAndDoors * Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond /
				actuations.CycleTime;
			var busAux = vehicleData.Components.BusAuxiliaries;
			var electricDoors = vehicleData.DoorDriveTechnology == ConsumerTechnology.Electrically ||
								vehicleData.DoorDriveTechnology == ConsumerTechnology.Mixed;

			foreach (var consumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items)
			{
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
				if (consumer.Bonus && !VehicleHasElectricalConsumer(consumer.ConsumerName, busAux))
				{
					current = 0.SI<Ampere>();
				}

				retVal[consumer.ConsumerName] = new ElectricConsumerEntry
				{
					BaseVehicle = consumer.BaseVehicle,
					Current = current
				};
			}

			return retVal;
		}

		protected virtual bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			switch (consumerName)
			{
				case "Day running lights LED bonus":
				case "Position lights LED bonus":
				case "Brake lights LED bonus": // return false;
				case "Interior lights LED bonus":
				case "Headlights LED bonus": return true;
				default: return false;
			}
		}
		protected virtual double CalculateLengthDependentElectricalConsumers(Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			var busParams = mission.BusParameter;
			return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
					busParams.VehicleLength, busParams.VehicleCode, busParams.NumberPassengersLowerDeck)
				.Value();
		}

		protected virtual double GetNumberOfElectricalConsumersInVehicle(string nbr, Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase))
			{
				return CalculateLengthDependentElectricalConsumers(mission, vehicleData);
			}

			return nbr.ToDouble();
		}

		public IDictionary<string, ElectricConsumerEntry> GetElectricConsumers(Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations, VehicleClass vehicleClass)
		{
			var retVal = GetDefaultElectricConsumers(mission, vehicleData, actuations);

			foreach (var entry in GetElectricAuxConsumers(mission, vehicleData, vehicleClass, vehicleData.Components.BusAuxiliaries))
			{
				retVal[entry.Key] = entry.Value;
			}

			return retVal;
		}

		protected virtual Dictionary<string, ElectricConsumerEntry> GetElectricAuxConsumers(Mission mission,
			IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAux)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var spPower = DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(
				mission.MissionType, busAux.SteeringPumpTechnology,
				vehicleData.Length ?? mission.BusParameter.VehicleLength);
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry
			{
				ActiveDuringEngineStopStandstill = false,
				BaseVehicle = false,
				Current = spPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};

			if (!vehicleData.ArchitectureID.IsBatteryElectricVehicle()) {
				var fanPower = DeclarationData.Fan.LookupElectricalPowerDemand(
					vehicleClass, mission.MissionType, busAux.FanTechnology);
				retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry {
					ActiveDuringEngineStopStandstill = false,
					ActiveDuringEngineStopDriving = false,
					BaseVehicle = false,
					Current = fanPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
				};
			}


			return retVal;
		}
		public double CalculateAlternatorEfficiency(IList<IAlternatorDeclarationInputData> alternators)
		{
			return DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup("default");
		}

		private ElectricsUserInputsConfig GetElectricalUserConfig(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, vehicleData, actuations, vehicleClass);
			var busAux = vehicleData.Components.BusAuxiliaries;

			var retVal = GetDefaultElectricalUserConfig();

			retVal.AlternatorType =
				vehicleData.VehicleType.IsOneOf(
					VectoSimulationJobType.BatteryElectricVehicle,
					VectoSimulationJobType.IEPC_E,
					VectoSimulationJobType.FCHV,
					VectoSimulationJobType.FCHV_IEPC)
					? AlternatorType.None
					: busAux.ElectricSupply.AlternatorTechnology;
			retVal.ElectricalConsumers = currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(CalculateAlternatorEfficiency(busAux.ElectricSupply.Alternators));

			switch (retVal.AlternatorType)
			{
				case AlternatorType.Smart when busAux.ElectricSupply.Alternators.Count == 0:
					throw new VectoException("at least one alternator is required when specifying smart electrics!");
				case AlternatorType.Smart when busAux.ElectricSupply.ElectricStorage.Count == 0:
					throw new VectoException("at least one electric storage (battery or capacitor) is required when specifying smart electrics!");
			}

			retVal.ElectricStorageCapacity = CalculateBatteryCapacity(busAux.ElectricSupply.ElectricStorage);
			retVal.MaxAlternatorPower = CalculateMaxAlternatorPower(busAux);

			if (vehicleData.ArchitectureID.IsBatteryElectricVehicle())
			{
				retVal.ConnectESToREESS = true;
			}
			else if (vehicleData.ArchitectureID.IsHybridVehicle())
			{
				retVal.ConnectESToREESS =
					vehicleData.Components.BusAuxiliaries.ElectricSupply.ESSupplyFromHEVREESS;
			}
			retVal.DCDCEfficiency = DeclarationData.DCDCEfficiency;

			return retVal;
		}

		protected virtual Watt CalculateMaxAlternatorPower(IBusAuxiliariesDeclarationData busAux)
		{
			return DeclarationData.BusAuxiliaries.CalculateMaxAlternatorPower(busAux.ElectricSupply);
		}

		protected virtual WattSecond CalculateBatteryCapacity(
			IList<IBusAuxElectricStorageDeclarationInputData> electricStorage)
		{
			return DeclarationData.BusAuxiliaries.CalculateBatteryCapacity(electricStorage);
		}

		protected virtual PneumaticUserInputsConfig GetPneumaticUserConfig(IVehicleDeclarationInputData vehicleData, Mission mission)
		{
			var busAux = vehicleData.Components.BusAuxiliaries;

			//throw new NotImplementedException();
			return new PneumaticUserInputsConfig()
			{
				KneelingHeight = VectoMath.Max(0.SI<Meter>(), mission.BusParameter.EntranceHeight - Constants.BusParameters.EntranceHeight),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = busAux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically ? 0.0 : busAux.PneumaticSupply.Ratio,
				CompressorMap = DeclarationData.BusAuxiliaries.GetCompressorMap(busAux.PneumaticSupply),
				SmartAirCompression = busAux.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = busAux.PneumaticSupply.SmartRegeneration,
				AdBlueDosing = busAux.PneumaticConsumers.AdBlueDosing,
				Doors = ConsumerTechnology.Pneumatically,
				AirSuspensionControl = busAux.PneumaticConsumers.AirsuspensionControl,
			};
		}

		public IPneumaticsConsumersDemand CreatePneumaticAuxConfig(RetarderType retarderType)
		{
			return new PneumaticsConsumersDemand()
			{
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

		public virtual SSMInputs CreatePrimarySSMModelParameters(IBusAuxiliariesDeclarationData busAuxInputData,
			Mission mission,
			LoadingType loadingType, BusHVACSystemConfiguration applicableHVACConfiguration,
			HVACParameters hvacParameters,
			IFuelProperties heatingFuel, bool cooling)
		{
			var busParams = mission.BusParameter;
			var auxHeaterPower = hvacParameters.HVACAuxHeaterPower;
			var driverHeatpumpType = cooling
				? hvacParameters.HeatPumpTypeDriverCompartmentCooling
				: hvacParameters.HeatPumpTypeDriverCompartmentHeating;
			var passengerHeatpumpType = cooling
				? hvacParameters.HeatPumpTypePassengerCompartmentCooling
				: hvacParameters.HeatPumpTypePassengerCompartmentHeating;

            var isDoubleDecker = busParams.VehicleCode.IsDoubleDeckerBus();
			var internalLength = applicableHVACConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength // OK
				: DeclarationData.BusAuxiliaries.CalculateInternalLength(busParams.VehicleLength,
					busParams.VehicleCode, 10); // missing: correction length for low floor buses
			var ventilationLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(busParams.VehicleLength,
				busParams.VehicleCode, 10);
			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(mission.BusParameter.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var coolingPower = CalculateMaxCoolingPower(mission, applicableHVACConfiguration);
			var heatingPower = CalculateMaxHeatingPower(mission, applicableHVACConfiguration);

			var retVal = GetDefaulSSMInputs(heatingFuel);
			retVal.BusFloorType = busParams.VehicleCode.GetFloorType();
			retVal.Technologies = GetSSMTechnologyBenefits(busAuxInputData, mission.BusParameter.VehicleCode.GetFloorType(), hvacParameters);

			retVal.FuelFiredHeaterPower = auxHeaterPower;
			retVal.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(busParams.DoubleDecker) * internalLength +
									DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(busParams.DoubleDecker);
			retVal.BusSurfaceArea = 2 * (internalLength * busParams.VehicleWidth + internalLength * internalHeight +
										(isDoubleDecker ? 2.0 : 1.0) * busParams.VehicleWidth * busParams.BodyHeight);
			retVal.BusVolumeVentilation = ventilationLength * busParams.VehicleWidth * internalHeight;

			retVal.UValue = DeclarationData.BusAuxiliaries.UValue(busParams.VehicleCode.GetFloorType());
			retVal.NumberOfPassengers =
				DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(internalLength, busParams.VehicleWidth) *
				(loadingType == LoadingType.LowLoading ? mission.BusParameter.PassengerDensityLow : mission.BusParameter.PassengerDensityRef) *
				(loadingType == LoadingType.LowLoading ? mission.MissionType.GetLowLoadFactorBus() : 1.0) + 1; // add driver for 'heat input'
			retVal.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(applicableHVACConfiguration, false);
			retVal.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(applicableHVACConfiguration, true);

			//retVal.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			retVal.HVACMaxCoolingPowerDriver = coolingPower.Item1;
			retVal.HVACMaxCoolingPowerPassenger = coolingPower.Item2;
			retVal.MaxHeatingPowerDriver = heatingPower.Item1;
			retVal.MaxHeatingPowerPassenger = heatingPower.Item2;

			retVal.HeatPumpTypeDriverCompartment = driverHeatpumpType;
			retVal.HeatPumpTypePassengerCompartment = passengerHeatpumpType;

			retVal.HVACSystemConfiguration = applicableHVACConfiguration;

			//retVal.HVACCompressorType = passengerHeatpumpType; // use passenger compartment

			if (cooling)
			{
				retVal.DriverCompartmentLength = applicableHVACConfiguration.RequiresDriverAC()
					? applicableHVACConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration2,
						BusHVACSystemConfiguration.Configuration4)
						? 2 * Constants.BusParameters.DriverCompartmentLength
						: Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
				retVal.PassengerCompartmentLength = applicableHVACConfiguration.RequiresPassengerAC()
					? applicableHVACConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration2,
						BusHVACSystemConfiguration.Configuration4)
						? 0.SI<Meter>()
						: internalLength - Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
			}
			else
			{
				retVal.DriverCompartmentLength = applicableHVACConfiguration.RequiresDriverAC()
					? Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
				retVal.PassengerCompartmentLength = applicableHVACConfiguration.RequiresDriverAC()
					? internalLength - Constants.BusParameters.DriverCompartmentLength
					: internalLength;
			}

			return retVal;
		}

		private HVACParameters GetHVACParams(VectoSimulationJobType vehicleType, BusParameters busParams)
		{
			switch (vehicleType)
			{
				case VectoSimulationJobType.ConventionalVehicle:
					return busParams.HVACConventional;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.IHPC:
					return busParams.HVACHEV;
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
					return busParams.HVACPEV;
				case VectoSimulationJobType.EngineOnlySimulation:
				default:
					throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType, null);
			}
		}

		protected virtual TechnologyBenefits GetSSMTechnologyBenefits(IBusAuxiliariesDeclarationData inputData,
			FloorType floorType, HVACParameters hvacParameters)
		{
			var onVehicle = new List<SSMTechnology>();
			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList)
			{
				if ("Double-glazing".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					hvacParameters.HVACDoubleGlasing) {
					onVehicle.Add(item);
				}
				if ("Adjustable auxiliary heater".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					hvacParameters.HVACAdjustableAuxHeater) {
					onVehicle.Add(item);
				}
				if ("Separate air distribution ducts".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					hvacParameters.HVACSeparateAirDistributionDucts) {
					onVehicle.Add(item);
				}
				
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.AdjustableCoolantThermostat ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.EngineWasteGasHeatExchanger ?? false))
				{
					onVehicle.Add(item);
				}
			}

			return SelectBenefitForFloorType(floorType, onVehicle);
		}

		public virtual TechnologyBenefits SelectBenefitForFloorType(FloorType floorType, List<SSMTechnology> onVehicle)
		{
			var retVal = new TechnologyBenefits();

			switch (floorType)
			{
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

		public SSMInputs GetDefaulSSMInputs(IFuelProperties heatingFuel)
		{
			return new SSMInputs(null, heatingFuel)
			{
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
				ElectricWasteHeatToCoolant = Constants.BusAuxiliaries.Heater.ElectricWasteHeatToCoolant,
				GFactor = Constants.BusAuxiliaries.SteadyStateModel.GFactor,

				VentilationOnDuringHeating = true,
				VentilationWhenBothHeatingAndACInactive = true,
				VentilationDuringAC = true,

				MaxPossibleBenefitFromTechnologyList =
					Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList,
			};
		}

		protected virtual Tuple<Watt, Watt> CalculateMaxCoolingPower(Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			var busParams = mission.BusParameter;

			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				busParams.VehicleLength, busParams.VehicleCode,
				busParams.NumberPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(busParams.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var volume = (length - Constants.BusParameters.DriverCompartmentLength) * height * busParams.VehicleWidth;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		protected virtual Tuple<Watt, Watt> CalculateMaxHeatingPower(Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			var busParams = mission.BusParameter;

			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				busParams.VehicleLength, busParams.VehicleCode,
				busParams.NumberPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(busParams.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var volume = (length - Constants.BusParameters.DriverCompartmentLength) * height * busParams.VehicleWidth;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.DriverMaxHeatingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.PassengerMaxHeatingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		#region Overrides of AuxiliaryDataAdapter



		private HeaterType GetElectricHeater(Mission mission, VectoRunData runData)
		{
			HVACParameters hvacParams = null;
			switch (runData.JobType)
			{
				case VectoSimulationJobType.ConventionalVehicle:
					hvacParams = mission.BusParameter.HVACConventional;
					break;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.IHPC:
					hvacParams = mission.BusParameter.HVACHEV;
					break;
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.FCHV_IEPC:
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.IEPC_E:
					hvacParams = mission.BusParameter.HVACPEV;
					break;
				case VectoSimulationJobType.EngineOnlySimulation:
				default:
					throw new ArgumentOutOfRangeException();
			}

			return hvacParams.WaterElectricHeater ? HeaterType.WaterElectricHeater : HeaterType.None;
		}

		protected override IList<VectoRunData.AuxData> DoCreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxData,
			MissionType mission, VehicleClass hdvClass, Meter vehicleLength, int? numSteeredAxles,
			VectoSimulationJobType jobType)
		{
			if (auxInputData != null)
			{
				throw new VectoException("Only BusAuxiliaries can be provided as input!");
			}

			if (numSteeredAxles.HasValue && busAuxData.SteeringPumpTechnology.Count != numSteeredAxles.Value)
			{
				throw new VectoException($"Number of steering pump technologies does not match number of steered axles ({numSteeredAxles.Value}, {busAuxData.SteeringPumpTechnology.Count})");
			}
			var retVal = new List<VectoRunData.AuxData>();

			var alternatorEfficiency = DeclarationData.AlternatorEfficiency;

			foreach (var auxType in AuxiliaryTypes)
			{

				var aux = new VectoRunData.AuxData
				{
					DemandType = AuxiliaryDemandType.Constant,
					//Technology = auxData.Technology,
					MissionType = mission,
				};

				switch (auxType)
				{
					case AuxiliaryType.Fan:
						AddFan(mission, hdvClass, jobType, busAuxData, aux, alternatorEfficiency, retVal);
						break;
					case AuxiliaryType.SteeringPump:
						AddSteeringPumps(mission, busAuxData.SteeringPumpTechnology, vehicleLength, retVal);
						break;
				}
			}

			if (CreateConditioningAux(jobType))
			{
				AddConditioning(mission, jobType, retVal, hdvClass);
			}


			return retVal;
		}

		private static void AddConditioning(MissionType mission, VectoSimulationJobType jobType, List<VectoRunData.AuxData> auxDataList, VehicleClass hdv)
		{
			var aux = new VectoRunData.AuxData()
			{
				IsFullyElectric = true,
				MissionType = mission,
				DemandType = AuxiliaryDemandType.Dynamic,
				ID = Constants.Auxiliaries.IDs.Cond,
				ConnectToREESS = true,
				PowerDemandElectric = DeclarationData.Conditioning.LookupPowerDemand(hdv, jobType, mission),
			};

			auxDataList.Add(aux);
		}

		private static void AddFan(MissionType mission, VehicleClass hdvClass, VectoSimulationJobType jobType,
			IBusAuxiliariesDeclarationData auxData, VectoRunData.AuxData aux, double alternatorEfficiency,
			List<VectoRunData.AuxData> auxDataList)
		{
			if (!DeclarationData.Fan.IsApplicable(hdvClass, jobType, auxData.FanTechnology))
			{
				throw new VectoException(
					$"Fan technology '{auxData.FanTechnology}' is not applicable for '{jobType}'");
			}

			aux.PowerDemandMech = DeclarationData.Fan.LookupPowerDemand(hdvClass, mission, auxData.FanTechnology);
			aux.ID = Constants.Auxiliaries.IDs.Fan;
			aux.IsFullyElectric = DeclarationData.Fan.IsFullyElectric(hdvClass, auxData.FanTechnology);
			aux.PowerDemandElectric = aux.PowerDemandMech * alternatorEfficiency;
			aux.ConnectToREESS = aux.IsFullyElectric && !jobType.IsOneOf(VectoSimulationJobType.ConventionalVehicle,
				VectoSimulationJobType.EngineOnlySimulation);
			aux.Technology = new[] { auxData.FanTechnology }.ToList();
			auxDataList.Add(aux);
		}

		private static void AddSteeringPumps(MissionType mission, IList<string> technologies, Meter lenght,
			List<VectoRunData.AuxData> auxDataList)
		{
			var powerDemand = DeclarationData.SteeringPumpBus.LookupMechanicalPowerDemand(mission, technologies, lenght);
			var spMech = new VectoRunData.AuxData
			{
				DemandType = AuxiliaryDemandType.Constant,
				Technology = technologies,
				IsFullyElectric = false,
				ID = Constants.Auxiliaries.IDs.SteeringPump,
				PowerDemandMech = powerDemand,
				MissionType = mission,
			};

			if (spMech.PowerDemandMech.IsGreater(0))
			{
				auxDataList.Add(spMech);
			}

		}

		#endregion
	}

	public class PrimaryBusPEVAuxiliaryDataAdapter : PrimaryBusAuxiliaryDataAdapter
	{
		protected internal override HashSet<AuxiliaryType> AuxiliaryTypes { get; } = new HashSet<AuxiliaryType>() {
			//AuxiliaryType.Fan,
			AuxiliaryType.SteeringPump
		};

	}
}
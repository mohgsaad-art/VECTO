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

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter
{
	public class SpecificCompletedBusAuxiliaryDataAdapter : PrimaryBusAuxiliaryDataAdapter, ICompletedBusAuxiliaryDataAdapter
	{

		public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);

			var (ssmCooling, ssmHeating) =
				GetCompletedSSMInput(mission, completedVehicle, primaryVehicle, runData.Loading);

			var electricUserInputs = CreateElectricsUserInputsConfig(
				primaryVehicle, completedVehicle, mission, actuations, runData.VehicleData.VehicleClass);

            var retarderType = (runData.AxlePowertrains?.Count() > 0)
                ? (runData.AxlePowertrains.FirstOrDefault(x => x.Retarder.Type != RetarderType.None)?.Retarder.Type ?? RetarderType.None)
                : runData.RetarderSinglePwt.Type;

            var pneumaticUserInputsConfig = CreatePneumaticUserInputsConfig(primaryVehicle, completedVehicle);
			var pneumaticAuxiliariesConfig = CreatePneumaticAuxConfig(retarderType);

			if (primaryVehicle.Components.BusAuxiliaries.PneumaticSupply.CompressorDrive == CompressorDrive.electrically) {
				var auxConfig = new AuxiliaryConfig {
					VehicleData = runData.VehicleData,
					PneumaticAuxiliariesConfig = pneumaticAuxiliariesConfig,
					PneumaticUserInputsConfig = pneumaticUserInputsConfig
				};
				var airDemand = M03Impl.TotalAirDemandCalculation(auxConfig, actuations) / actuations.CycleTime;
				electricUserInputs.ElectricalConsumers[Constants.Auxiliaries.IDs.PneumaticSystem] =
					new ElectricConsumerEntry() {
						Current = DeclarationData.BusAuxiliaries.PneumaticSystemElectricDemandPerAirGenerated * airDemand / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
						ActiveDuringEngineStopDriving = true,
						ActiveDuringEngineStopStandstill = true,
						BaseVehicle = false
					};
			}

            return new AuxiliaryConfig
			{
				InputData = completedVehicle.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = electricUserInputs,
				PneumaticUserInputsConfig = pneumaticUserInputsConfig,
				PneumaticAuxiliariesConfig = pneumaticAuxiliariesConfig,
				Actuations = actuations,
				SSMInputsCooling = ssmCooling,
				SSMInputsHeating = ssmHeating,
				VehicleData = runData.VehicleData
			};
		}

		#region Avarage Current Demand Calculation

		protected override bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			if (consumerName == "Day running lights LED bonus" && (bool)busAux.ElectricConsumers.DayrunninglightsLED)
				return true;
			if (consumerName == "Position lights LED bonus" && (bool)busAux.ElectricConsumers.PositionlightsLED)
				return true;
			if (consumerName == "Brake lights LED bonus" && (bool)busAux.ElectricConsumers.BrakelightsLED)
				return true;
			if (consumerName == "Interior lights LED bonus" && (bool)busAux.ElectricConsumers.InteriorLightsLED)
				return true;
			if (consumerName == "Headlights LED bonus" && (bool)busAux.ElectricConsumers.HeadlightsLED)
				return true;

			return false;
		}

		protected override double CalculateLengthDependentElectricalConsumers(Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			var busParams = mission.BusParameter;
			return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
					vehicleData.Length, vehicleData.VehicleCode, busParams.NumberPassengersLowerDeck)
				.Value();
		}


		#endregion

		private Tuple<Watt, Watt> CalculateMaxCoolingPower(IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle,
			Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			if (hvacConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration0, BusHVACSystemConfiguration.Unknown)) {
				throw new VectoException(
					$"HVAC Configuration {hvacConfiguration.ToXmlFormat()} is invalid for final step");
			}

			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var passengerCompartmentLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				completedVehicle.Length, completedVehicle.VehicleCode,
				(int)completedVehicle.NumberPassengerSeatsLowerDeck) - Constants.BusParameters.DriverCompartmentLength - correctionLengthDrivetrainVolume;

			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var volume = passengerCompartmentLength * internalHeight * completedVehicle.Width;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		private Tuple<Watt, Watt> CalculateMaxHeatingPower(IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle,
			Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			//var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			if (hvacConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration0, BusHVACSystemConfiguration.Unknown))
			{
				throw new VectoException(
					$"HVAC Configuration {hvacConfiguration.ToXmlFormat()} is invalid for final step");
			}

			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var passengerCompartmentLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				completedVehicle.Length, completedVehicle.VehicleCode,
				(int)completedVehicle.NumberPassengerSeatsLowerDeck) - Constants.BusParameters.DriverCompartmentLength - correctionLengthDrivetrainVolume;

			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var volume = passengerCompartmentLength * internalHeight * completedVehicle.Width;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.DriverMaxHeatingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.PassengerMaxHeatingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}



		private PneumaticUserInputsConfig CreatePneumaticUserInputsConfig(IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle)
		{
			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			return new PneumaticUserInputsConfig
			{
				CompressorMap = DeclarationData.BusAuxiliaries.GetCompressorMap(primaryBusAuxiliaries.PneumaticSupply),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = (primaryVehicle.ArchitectureID.IsBatteryElectricVehicle() || primaryVehicle.ArchitectureID.IsFuelCellVehicle())
					? 0 : primaryBusAuxiliaries.PneumaticSupply.Ratio,
				SmartAirCompression = primaryBusAuxiliaries.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = primaryBusAuxiliaries.PneumaticSupply.SmartRegeneration,
				KneelingHeight = VectoMath.Max(0.SI<Meter>(),
					completedVehicle.EntranceHeight - Constants.BusParameters.EntranceHeight),
				AirSuspensionControl = primaryBusAuxiliaries.PneumaticConsumers.AirsuspensionControl,
				AdBlueDosing = primaryBusAuxiliaries.PneumaticConsumers.AdBlueDosing,
				Doors = completedVehicle.DoorDriveTechnology
			};
		}

		private (SSMInputs, SSMInputs) GetCompletedSSMInput(Mission mission,
			IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle, LoadingType loadingType)
		{
			DoHVACInputSanityChecks(primaryVehicle, completedVehicle, mission);

            var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			var busAux = completedVehicle.Components.BusAuxiliaries.HVACAux;

			var hpDriverCooling =
				busAux.HeatPumpTypeCoolingDriverCompartment == HeatPumpType.not_applicable &&
				busAux.SystemConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration6,
					BusHVACSystemConfiguration.Configuration10) //&& busAux.HeatPumpTypeCoolingPassengerCompartment == HeatPumpType.none
					? HeatPumpType.none
					: busAux.HeatPumpTypeCoolingDriverCompartment;
			var hpDriverHeating =
				busAux.HeatPumpTypeHeatingDriverCompartment == HeatPumpType.not_applicable &&
				busAux.SystemConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration6,
					BusHVACSystemConfiguration.Configuration10) //&& busAux.HeatPumpTypeHeatingPassengerCompartment == HeatPumpType.none
					? HeatPumpType.none
					: busAux.HeatPumpTypeHeatingDriverCompartment;

            var applicableSystemConfigCooling = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				hpDriverCooling.Value, busAux.HeatPumpTypeCoolingPassengerCompartment.Value);
			var applicableSystemConfigHeating = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				hpDriverHeating.Value, busAux.HeatPumpTypeHeatingPassengerCompartment.Value);


			var ssmCooling = CreateSpecificSSMModelParameters(mission, completedVehicle, primaryVehicle, loadingType,
				applicableSystemConfigCooling, hpDriverCooling.Value,
				busAux.HeatPumpTypeCoolingPassengerCompartment.Value, true);
			ssmCooling.ElectricHeater = HeaterType.None;
			var ssmHeating = CreateSpecificSSMModelParameters(mission, completedVehicle, primaryVehicle, loadingType,
				applicableSystemConfigHeating, hpDriverHeating.Value,
				busAux.HeatPumpTypeHeatingPassengerCompartment.Value, false);
			ssmHeating.ElectricHeater = GetElectricHeater(busAux);
			ssmHeating.HeatingDistributions = DeclarationData.BusAuxiliaries.HeatingDistributionCases;
			return (ssmCooling, ssmHeating);
		}

		private static void DoHVACInputSanityChecks(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Mission mission)
		{
			var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			var busAux = completedVehicle.Components.BusAuxiliaries.HVACAux;

            if (hvacConfiguration == null || hvacConfiguration.Value == BusHVACSystemConfiguration.Configuration0) {
				throw new VectoException("HVAC Configuration has to be set for completed stage!");
			}

			if (mission.BusParameter.SeparateAirDistributionDuctsHVACCfg.Contains(hvacConfiguration.Value) &&
				(busAux.SeparateAirDistributionDucts == null || !busAux.SeparateAirDistributionDucts.Value)) {
				throw new VectoException(
					"Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group '{0}' and HVAC configuration '{1}'",
					mission.BusParameter.BusGroup.GetClassNumber(), hvacConfiguration.GetName());
			}

			if (completedVehicle.NumberPassengerSeatsLowerDeck == null) {
				throw new VectoException("NumberOfPassengerSeatsLowerDeck input parameter is required");
			}

			if (completedVehicle.NumberPassengerSeatsUpperDeck == null) {
				throw new VectoException("NumberOfPassengerSeatsUpperDeck input parameter is required");
			}

			if (completedVehicle.NumberPassengersStandingLowerDeck == null) {
				throw new VectoException("NumberOfPassengersStandingLowerDeck input parameter is required");
			}

			if (completedVehicle.NumberPassengersStandingUpperDeck == null) {
				throw new VectoException("NumberOfPassengersStandingUpperDeck input parameter is required");
			}

			if (busAux.HeatPumpTypeCoolingDriverCompartment == null) {
				throw new VectoException("HeatPumpTypeDriverCompartment Cooling input parameter is required");
			}

			if (busAux.HeatPumpTypeHeatingDriverCompartment == null) {
				throw new VectoException("HeatPumpTypeDriverCompartment Heating input parameter is required");
			}

			if (busAux.HeatPumpTypeCoolingPassengerCompartment == null) {
				throw new VectoException("HeatPumpTypePassengerCompartment Cooling input parameter is required");
			}

			if (busAux.HeatPumpTypeHeatingPassengerCompartment == null) {
				throw new VectoException("HeatPumpTypePassengerCompartment Heating input parameter is required");
			}

			var hvacConfigCooling = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				busAux.HeatPumpTypeCoolingDriverCompartment.Value, busAux.HeatPumpTypeCoolingPassengerCompartment.Value);
			var hvacConfigHeating = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				busAux.HeatPumpTypeHeatingDriverCompartment.Value, busAux.HeatPumpTypeHeatingPassengerCompartment.Value);

			if (hvacConfigHeating != hvacConfiguration && hvacConfigCooling != hvacConfiguration) {
				throw new VectoException(
					$"The HVAC System Configuration must be either matched for the case heating or cooling input: {hvacConfiguration.GetName()},  h:{hvacConfigHeating.GetName()}/c:{hvacConfigCooling.GetName()}");
			}

			var xEVBus = primaryVehicle.ArchitectureID.IsHybridVehicle() || primaryVehicle.ArchitectureID.IsBatteryElectricVehicle();
			if (xEVBus && busAux.WaterElectricHeater == null) {
				throw new VectoException("WaterElectricHeater input parameter is required for xEV vehicles");
			}
			if (xEVBus && busAux.AirElectricHeater == null) {
				throw new VectoException("AirElectricHeater input parameter is required for xEV vehicles");
			}
			if (xEVBus && busAux.OtherHeatingTechnology == null) {
				throw new VectoException("OtherElectricHeater input parameter is required for xEV vehicles");
			}

			if (primaryVehicle.ArchitectureID.IsBatteryElectricVehicle() ||
				primaryVehicle.ArchitectureID.IsSerialHybridVehicle()) {
				var heatpumps = new Dictionary<string, HeatPumpType>() {
					{ "Heatpump Driver Compartment - cooling", busAux.HeatPumpTypeCoolingDriverCompartment.Value },
					{ "Heatpump Driver Compartment - heating", busAux.HeatPumpTypeHeatingDriverCompartment.Value },
					{ "Heatpump Passenger Compartment - cooling", busAux.HeatPumpTypeCoolingPassengerCompartment.Value },
					{ "Heatpump Passenger Compartment - heating", busAux.HeatPumpTypeHeatingPassengerCompartment.Value },
				};
				var errors = new List<string>();
				foreach (var entry in heatpumps) {
					if (!entry.Value.IsElectrical() && !entry.Value.IsOneOf(HeatPumpType.none, HeatPumpType.not_applicable)) {
						errors.Add($"{entry.Key}: must not be mechanically driven for PEV/S-HEV ({entry.Value.ToXML()})");
					}
				}

				if (errors.Count > 0) {
					throw new VectoException(errors.Join(Environment.NewLine));
				}
			}
        }

		private HeaterType GetElectricHeater(IHVACBusAuxiliariesDeclarationData busAux)
		{
			var retVal = HeaterType.None;
			if (busAux.AirElectricHeater.HasValue && busAux.AirElectricHeater.Value)
			{
				retVal |= HeaterType.AirElectricHeater;
			}
			if (busAux.WaterElectricHeater.HasValue && busAux.WaterElectricHeater.Value)
			{
				retVal |= HeaterType.WaterElectricHeater;
			}
			if (busAux.OtherHeatingTechnology.HasValue && busAux.OtherHeatingTechnology.Value)
			{
				retVal |= HeaterType.OtherElectricHeating;
			}

			return retVal;
		}

		private SSMInputs CreateSpecificSSMModelParameters(Mission mission, IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle, LoadingType loadingType,
			BusHVACSystemConfiguration hvacConfiguration, HeatPumpType heatPumpTypeDriverCompartment,
			HeatPumpType heatPumpTypePassengerCompartment, bool cooling)
		{
			var isDoubleDecker = completedVehicle.VehicleCode.IsDoubleDeckerBus();

			var driverAcOnly = hvacConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration2,
				BusHVACSystemConfiguration.Configuration4);
			var internalLength = cooling && driverAcOnly
				? 2 * Constants.BusParameters.DriverCompartmentLength // OK
				: DeclarationData.BusAuxiliaries.CalculateInternalLength(
					completedVehicle.Length, completedVehicle.VehicleCode,
					completedVehicle.NumberPassengerSeatsLowerDeck.Value);
			var ventilationLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(completedVehicle.Length,
				completedVehicle.VehicleCode,
				completedVehicle.NumberPassengerSeatsLowerDeck.Value);
			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var correctedBusWidth = DeclarationData.BusAuxiliaries.CorrectedBusWidth(completedVehicle.Width);

			var coolingPower = CalculateMaxCoolingPower(completedVehicle, primaryVehicle, mission, hvacConfiguration);
			var heatingPower = CalculateMaxHeatingPower(completedVehicle, primaryVehicle, mission, hvacConfiguration);

			// Diesel is hardcoded to calculate SSM parameters, but is unused and does not influence output.
			var ssmInputs = GetDefaulSSMInputs(FuelData.Diesel);

			ssmInputs.BusFloorType = completedVehicle.VehicleCode.GetFloorType();
			ssmInputs.Technologies = CreateTechnologyBenefits(completedVehicle, primaryVehicle.Components.BusAuxiliaries);
			ssmInputs.FuelFiredHeaterPower = completedVehicle.Components.BusAuxiliaries.HVACAux.AuxHeaterPower;
			ssmInputs.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(isDoubleDecker) * internalLength +
										DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(isDoubleDecker);
			ssmInputs.BusSurfaceArea = 2 * (completedVehicle.Length * correctedBusWidth + internalLength *
				internalHeight + (isDoubleDecker ? 2.0 : 1.0) * correctedBusWidth * completedVehicle.Height); // use equations sent by Tobias
			ssmInputs.BusVolumeVentilation = (ventilationLength - correctionLengthDrivetrainVolume) * correctedBusWidth * internalHeight;

			ssmInputs.UValue = DeclarationData.BusAuxiliaries.UValue(completedVehicle.VehicleCode.GetFloorType());
			ssmInputs.NumberOfPassengers = DeclarationData.GetNumberOfPassengers(
				mission, internalLength, correctedBusWidth,
				(completedVehicle.NumberPassengerSeatsLowerDeck ?? 0) + (completedVehicle.NumberPassengerSeatsUpperDeck ?? 0),
				(completedVehicle.NumberPassengersStandingLowerDeck ?? 0) + (completedVehicle.NumberPassengersStandingUpperDeck ?? 0),
				loadingType) + 1; // add driver for 'heat input' // passenger count can't be null as this is checked in the calling method already. use ?? to avoid compiler warning
			ssmInputs.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, false);
			ssmInputs.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, true);

			//ssmInputs.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			ssmInputs.HVACMaxCoolingPowerDriver = coolingPower.Item1;
			ssmInputs.HVACMaxCoolingPowerPassenger = coolingPower.Item2;
			ssmInputs.MaxHeatingPowerDriver = heatingPower.Item1;
			ssmInputs.MaxHeatingPowerPassenger = heatingPower.Item2;

			//ssmInputs.HeatPumpTypeHeatingDriverCompartment = busAux.HeatPumpTypeHeatingDriverCompartment.Value;
			ssmInputs.HeatPumpTypeDriverCompartment = heatPumpTypeDriverCompartment;
			//ssmInputs.HeatPumpTypeHeatingPassengerCompartment = busAux.HeatPumpTypeHeatingPassengerCompartment.Value;
			ssmInputs.HeatPumpTypePassengerCompartment = heatPumpTypePassengerCompartment;

			ssmInputs.HVACSystemConfiguration = hvacConfiguration;

			if (cooling)
			{
				ssmInputs.DriverCompartmentLength = hvacConfiguration.RequiresDriverAC()
					? driverAcOnly
						? 2 * Constants.BusParameters.DriverCompartmentLength
						: Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
				ssmInputs.PassengerCompartmentLength = hvacConfiguration.RequiresPassengerAC()
					? driverAcOnly
						? 0.SI<Meter>()
						: internalLength - Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
			}
			else
			{
				ssmInputs.DriverCompartmentLength = hvacConfiguration.RequiresDriverAC()
					? Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
				ssmInputs.PassengerCompartmentLength = hvacConfiguration.RequiresDriverAC()
					? internalLength - Constants.BusParameters.DriverCompartmentLength
					: internalLength;
			}

			//ssmInputs.HVACCompressorType = heatPumpTypePassengerCompartment; // use passenger compartment

			return ssmInputs;
		}

		private TechnologyBenefits CreateTechnologyBenefits(IVehicleDeclarationInputData completedVehicle,
			IBusAuxiliariesDeclarationData primaryBusAux)
		{
			var onVehicle = new List<SSMTechnology>();
			var completedBuxAux = completedVehicle.Components.BusAuxiliaries;

			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList)
			{
				if ("Double-glazing".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.DoubleGlazing ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Adjustable auxiliary heater".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.AdjustableAuxiliaryHeater ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Separate air distribution ducts".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.SeparateAirDistributionDucts ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(primaryBusAux?.HVACAux.AdjustableCoolantThermostat ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(primaryBusAux?.HVACAux.EngineWasteGasHeatExchanger ?? false))
				{
					onVehicle.Add(item);
				}
			}

			return SelectBenefitForFloorType(completedVehicle.VehicleCode.GetFloorType(), onVehicle);
		}

		protected override Dictionary<string, ElectricConsumerEntry> GetElectricAuxConsumers(Mission mission,
			IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAux)
		{
			return new Dictionary<string, ElectricConsumerEntry>();
		}

		protected virtual Dictionary<string, ElectricConsumerEntry> GetElectricAuxConsumersPrimary(Mission mission,
			IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle,
			VehicleClass vehicleClass)
		{
			var busAuxPrimary = primaryVehicle.Components.BusAuxiliaries;

			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var spPower = DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(
				mission.MissionType, busAuxPrimary.SteeringPumpTechnology,
				completedVehicle.Length ?? mission.BusParameter.VehicleLength);
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry
			{
				ActiveDuringEngineStopStandstill = false,
				BaseVehicle = false,
				Current = spPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};

			if (!primaryVehicle.ArchitectureID.IsBatteryElectricVehicle() && 
				!primaryVehicle.ArchitectureID.IsFuelCellVehicle()) 
			{
				var fanPower = DeclarationData.Fan.LookupElectricalPowerDemand(
					vehicleClass, mission.MissionType, busAuxPrimary.FanTechnology);
				retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry {
					ActiveDuringEngineStopStandstill = false,
					ActiveDuringEngineStopDriving = false,
					BaseVehicle = false,
					Current = fanPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
				};
			}

			return retVal;
		}

		protected virtual ElectricsUserInputsConfig CreateElectricsUserInputsConfig(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Mission mission, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, completedVehicle, actuations, vehicleClass);

			// add electrical steering pump or electric fan defined in primary vehicle
			foreach (var entry in GetElectricAuxConsumersPrimary(mission, primaryVehicle, completedVehicle, vehicleClass))
			{
				currentDemand[entry.Key] = entry.Value;
			}

			var retVal = GetDefaultElectricalUserConfig();

			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;
			retVal.AlternatorType = primaryVehicle.VehicleType.IsOneOf(
				VectoSimulationJobType.BatteryElectricVehicle, 
				VectoSimulationJobType.IEPC_E,
				VectoSimulationJobType.FCHV,
				VectoSimulationJobType.FCHV_IEPC,
				VectoSimulationJobType.Multiple_FCHV,
				VectoSimulationJobType.Multiple_PEV)
				? AlternatorType.None
				: primaryBusAuxiliaries.ElectricSupply.AlternatorTechnology;
			//primaryBusAuxiliaries.ElectricSupply.AlternatorTechnology;
			retVal.ElectricalConsumers = (Dictionary<string, ElectricConsumerEntry>)currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(
				CalculateAlternatorEfficiency(
					primaryBusAuxiliaries.ElectricSupply.Alternators
						.Concat(completedVehicle.Components.BusAuxiliaries.ElectricSupply?.Alternators ??
								new List<IAlternatorDeclarationInputData>()).ToList()));
			switch (retVal.AlternatorType)
			{
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.Alternators.Count == 0:
					throw new VectoException("at least one alternator is required when specifying smart electrics!");
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.ElectricStorage.Count == 0:
					throw new VectoException("at least one electric storage (battery or capacitor) is required when specifying smart electrics!");
			}

			retVal.MaxAlternatorPower = CalculateMaxAlternatorPower(primaryBusAuxiliaries);
			retVal.ElectricStorageCapacity = CalculateBatteryCapacity(primaryBusAuxiliaries.ElectricSupply.ElectricStorage);

            if (primaryVehicle.VehicleType.IsOneOf(
				VectoSimulationJobType.BatteryElectricVehicle,
				VectoSimulationJobType.IEPC_E,
				VectoSimulationJobType.Multiple_PEV,
				VectoSimulationJobType.FCHV,
				VectoSimulationJobType.FCHV_IEPC,
				VectoSimulationJobType.Multiple_FCHV))
			{
				retVal.ConnectESToREESS = true;
			}
			else if (primaryVehicle.VehicleType.IsOneOf(VectoSimulationJobType.ParallelHybridVehicle,
							VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.IEPC_S,
							VectoSimulationJobType.IHPC, VectoSimulationJobType.Multiple_SHEV))
			{
				retVal.ConnectESToREESS =
					primaryVehicle.Components.BusAuxiliaries.ElectricSupply.ESSupplyFromHEVREESS;
			}

			retVal.DCDCEfficiency = DeclarationData.DCDCEfficiency;

			return retVal;
		}
		#region Implementation of ICompletedBusAuxiliaryDataAdapter


		#endregion
	}

	public class SpecificCompletedPEVBusAuxiliaryDataAdapter : SpecificCompletedBusAuxiliaryDataAdapter
	{
		protected internal override HashSet<AuxiliaryType> AuxiliaryTypes { get; } = new HashSet<AuxiliaryType>() {
			//AuxiliaryType.Fan,
			AuxiliaryType.SteeringPump
		};
	}
}
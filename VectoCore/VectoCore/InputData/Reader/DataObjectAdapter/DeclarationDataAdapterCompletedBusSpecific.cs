using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterCompletedBusSpecific : DeclarationDataAdapterCompletedBusGeneric
	{

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission, 
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
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
			var passengers = GetNumberOfPassengers(
				mission, completedVehicle.Length, completedVehicle.Width,
				completedVehicle.NumberPassengerSeatsLowerDeck.Value + completedVehicle.NumberPassengerSeatsUpperDeck.Value, 
				completedVehicle.NumberPassengersStandingLowerDeck.Value + completedVehicle.NumberPassengersStandingUpperDeck.Value, 
				loading.Key);

			var vehicleData = base.CreateVehicleData(primaryVehicle, segment, mission, loading, false);
			vehicleData.InputData = completedVehicle;
			vehicleData.VIN = completedVehicle.VIN;
			vehicleData.LegislativeClass = completedVehicle.LegislativeClass;
			vehicleData.VehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
			vehicleData.Manufacturer = completedVehicle.Manufacturer;
			vehicleData.ModelName = completedVehicle.Model;
			vehicleData.ManufacturerAddress = completedVehicle.ManufacturerAddress;
			vehicleData.CurbMass = completedVehicle.CurbMassChassis;

			vehicleData.Loading = passengers * mission.MissionType.GetAveragePassengerMass();
			vehicleData.PassengerCount = passengers;
			vehicleData.GrossVehicleMass = completedVehicle.GrossVehicleMassRating;
			vehicleData.DigestValueInput = completedVehicle.DigestValue?.DigestValue ?? "";

			vehicleData.RegisteredClass = completedVehicle.RegisteredClass;

			vehicleData.VehicleCode = completedVehicle.VehicleCode;
			if (vehicleData.TotalVehicleMass.IsGreater(vehicleData.GrossVehicleMass)) {
				throw new VectoException("Total Vehicle Mass exceeds Gross Vehicle Mass for completed bus specific ({0}/{1})", 
					vehicleData.TotalVehicleMass, vehicleData.GrossVehicleMass);
			}
			return vehicleData;
		}

		public AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission)
		{
			if (!mission.BusParameter.AirDragMeasurementAllowed ||
				completedVehicle.Components.AirdragInputData?.AirDragArea == null) {
				return new AirdragData() {
					CertificationMethod = CertificationMethod.StandardValues,
					DeclaredAirdragArea = mission.DefaultCDxA,
					CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						mission.DefaultCDxA,
						GetDeclarationAirResistanceCurve(
							mission.CrossWindCorrectionParameters, mission.DefaultCDxA, completedVehicle.Height + mission.BusParameter.DeltaHeight),
						CrossWindCorrectionMode.DeclarationModeCorrection),
					CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection
				};
			}

			var retVal = SetCommonAirdragData(completedVehicle.Components.AirdragInputData);
			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;
			var aerodynamicDragArea = completedVehicle.Components.AirdragInputData.AirDragArea;

			retVal.DeclaredAirdragArea = aerodynamicDragArea;
			retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				aerodynamicDragArea,
				GetDeclarationAirResistanceCurve(
					mission.CrossWindCorrectionParameters,
					aerodynamicDragArea,
					completedVehicle.Height + mission.BusParameter.DeltaHeight),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			return retVal;
		}

		public virtual IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);
			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			return new AuxiliaryConfig {
				InputData = completedVehicle.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = CreateElectricsUserInputsConfig(
					primaryVehicle, completedVehicle, mission, actuations, runData.VehicleData.VehicleClass),
				PneumaticUserInputsConfig = CreatePneumaticUserInputsConfig(
					primaryBusAuxiliaries, completedVehicle),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputs = GetCompletedSSMInput(mission, completedVehicle, primaryVehicle, runData.Loading),
				VehicleData = runData.VehicleData
			};
		}

		protected virtual ElectricsUserInputsConfig CreateElectricsUserInputsConfig(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Mission mission, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, completedVehicle, actuations, vehicleClass);

			// add electrical steering pump or electric fan defined in primary vehicle
			foreach (var entry in GetElectricAuxConsumersPrimary(mission, completedVehicle, vehicleClass, primaryVehicle.Components.BusAuxiliaries)) {
				currentDemand[entry.Key] = entry.Value;
			}

			var retVal = GetDefaultElectricalUserConfig();

			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;
			retVal.AlternatorType = primaryBusAuxiliaries.ElectricSupply.AlternatorTechnology;
			retVal.ElectricalConsumers = currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(
				CalculateAlternatorEfficiency(
					primaryBusAuxiliaries.ElectricSupply.Alternators
						.Concat(completedVehicle.Components.BusAuxiliaries.ElectricSupply?.Alternators ??
								new List<IAlternatorDeclarationInputData>()).ToList()));
			switch (retVal.AlternatorType) {
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.Alternators.Count == 0:
					throw new VectoException("at least one alternator is required when specifying smart electrics!");
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.ElectricStorage.Count == 0:
					throw new VectoException("at least one electric storage (battery or capacitor) is required when specifying smart electrics!");
			}

			retVal.MaxAlternatorPower = primaryBusAuxiliaries.ElectricSupply.Alternators.Sum(x => x.RatedVoltage * x.RatedCurrent);
			retVal.ElectricStorageCapacity = primaryBusAuxiliaries.ElectricSupply.ElectricStorage.Sum(x => x.ElectricStorageCapacity) ?? 0.SI<WattSecond>();
			
			return retVal;
		}

		protected override Dictionary<string, ElectricConsumerEntry> GetElectricAuxConsumers(Mission mission, IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAux)
		{
			return new Dictionary<string, ElectricConsumerEntry>();
		}

		protected virtual Dictionary<string, ElectricConsumerEntry> GetElectricAuxConsumersPrimary(Mission mission, IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAuxPrimary)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var spPower = DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(
				mission.MissionType, busAuxPrimary.SteeringPumpTechnology,
				vehicleData.Length ?? mission.BusParameter.VehicleLength);
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry {
				ActiveDuringEngineStopStandstill = false,
				BaseVehicle = false,
				Current = spPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};

			var fanPower = DeclarationData.Fan.LookupElectricalPowerDemand(
				vehicleClass, mission.MissionType, busAuxPrimary.FanTechnology);
			retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry {
				ActiveDuringEngineStopStandstill = false,
				ActiveDuringEngineStopDriving = false,
				BaseVehicle = false,
				Current = fanPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};
			return retVal;
		}

		protected PneumaticUserInputsConfig CreatePneumaticUserInputsConfig(IBusAuxiliariesDeclarationData primaryBusAuxiliaries,
			IVehicleDeclarationInputData completedVehicle)
		{
			return new PneumaticUserInputsConfig {
				CompressorMap = DeclarationData.BusAuxiliaries.GetCompressorMap(primaryBusAuxiliaries.PneumaticSupply.CompressorSize, primaryBusAuxiliaries.PneumaticSupply.Clutch),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = primaryBusAuxiliaries.PneumaticSupply.Ratio,
				SmartAirCompression = primaryBusAuxiliaries.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = primaryBusAuxiliaries.PneumaticSupply.SmartRegeneration,
				KneelingHeight = VectoMath.Max(0.SI<Meter>(),
					completedVehicle.EntranceHeight - Constants.BusParameters.EntranceHeight),
				AirSuspensionControl = primaryBusAuxiliaries.PneumaticConsumers.AirsuspensionControl,
				AdBlueDosing = primaryBusAuxiliaries.PneumaticConsumers.AdBlueDosing,
				Doors = completedVehicle.DoorDriveTechnology
			};
		}

		protected SSMInputs GetCompletedSSMInput(Mission mission, IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle, LoadingType loadingType)
		{
			var isDoubleDecker = completedVehicle.VehicleCode.IsDoubleDeckerBus();
			var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			var busAux = completedVehicle.Components.BusAuxiliaries.HVACAux;

			
            if (mission.BusParameter.SeparateAirDistributionDuctsHVACCfg.Contains(hvacConfiguration) &&
				(busAux.SeparateAirDistributionDucts == null || !busAux.SeparateAirDistributionDucts.Value)) {
				throw new VectoException("Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group '{0}' and HVAC configuration '{1}'",
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


			if (hvacConfiguration.RequiresDriverAC() && busAux.HeatPumpTypeCoolingDriverCompartment == HeatPumpType.none && busAux.HeatPumpTypeHeatingDriverCompartment == HeatPumpType.none) {
				throw new VectoException("HVAC System Configuration {0} requires DriverAC Technology", hvacConfiguration);
			}
			
			if (hvacConfiguration.RequiresPassengerAC() && busAux.HeatPumpTypeCoolingPassengerCompartment == HeatPumpType.none && busAux.HeatPumpTypeHeatingPassengerCompartment == HeatPumpType.none) {
				throw new VectoException("HVAC System Configuration {0} requires PassengerAC Technology", hvacConfiguration);
			}

			

            var heatPumpTypeDriverCompartment = busAux.HeatPumpTypeCoolingPassengerCompartment.Value;

            var heatPumpTypePassengerCompartment = busAux.HeatPumpTypeCoolingPassengerCompartment.Value;

            var internalLength = hvacConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength // OK
				: DeclarationData.BusAuxiliaries.CalculateInternalLength(
					completedVehicle.Length, completedVehicle.VehicleCode,
					completedVehicle.NumberPassengerSeatsLowerDeck.Value);
			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var correctedBusWidth = DeclarationData.BusAuxiliaries.CorrectedBusWidth(completedVehicle.Width);

			var coolingPower = CalculateMaxCoolingPower(completedVehicle, primaryVehicle, mission);

			
			var floorType = completedVehicle.VehicleCode.GetFloorType();

			var ssmInputs =GetDefaulSSMInputs(FuelData.Diesel);
			
			ssmInputs.BusFloorType = completedVehicle.VehicleCode.GetFloorType();
			ssmInputs.Technologies = CreateTechnologyBenefits(completedVehicle, primaryVehicle.Components.BusAuxiliaries);
			ssmInputs.FuelFiredHeaterPower = busAux.AuxHeaterPower;
			ssmInputs.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(isDoubleDecker) * internalLength +
										DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(isDoubleDecker);
			ssmInputs.BusSurfaceArea = 2 * (completedVehicle.Length * correctedBusWidth + internalLength *
											internalHeight + (isDoubleDecker ? 2.0 : 1.0) * correctedBusWidth * completedVehicle.Height); // use equations sent by Tobias
			ssmInputs.BusVolume = (internalLength - correctionLengthDrivetrainVolume) * correctedBusWidth * internalHeight;

			ssmInputs.UValue = DeclarationData.BusAuxiliaries.UValue(completedVehicle.VehicleCode.GetFloorType());
			ssmInputs.NumberOfPassengers = GetNumberOfPassengers(
				mission, internalLength, correctedBusWidth,
				completedVehicle.NumberPassengerSeatsLowerDeck.Value + completedVehicle.NumberPassengerSeatsUpperDeck.Value,
				completedVehicle.NumberPassengersStandingLowerDeck.Value + completedVehicle.NumberPassengersStandingUpperDeck.Value,
				loadingType) + 1; // add driver for 'heat input'
			ssmInputs.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, false);
			ssmInputs.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, true);

			ssmInputs.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;

            //ToDo FK COP calculation
            ssmInputs.HVACCompressorType = heatPumpTypePassengerCompartment; // use passenger compartment
            ssmInputs.HVACTechnology = $"{busAux.SystemConfiguration.GetName()} " + 
									   $"({string.Join(", ", heatPumpTypePassengerCompartment.GetName(), heatPumpTypeDriverCompartment.GetName())})";
			ssmInputs.COP = DeclarationData.BusAuxiliaries.CalculateCOP(
                coolingPower.Item1, heatPumpTypeDriverCompartment, coolingPower.Item2, heatPumpTypePassengerCompartment /* average */,
                floorType);

            return ssmInputs;
		}

		
		public TechnologyBenefits CreateTechnologyBenefits(IVehicleDeclarationInputData completedVehicle,
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

		

		protected override Tuple<Watt, Watt> CalculateMaxCoolingPower(IVehicleDeclarationInputData completedVehicle, IVehicleDeclarationInputData primaryVehicle,
			Mission mission)
		{
			var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var pasengerCompartmentLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(
			 	completedVehicle.Length , completedVehicle.VehicleCode, 
				(int)completedVehicle.NumberPassengerSeatsLowerDeck) - Constants.BusParameters.DriverCompartmentLength - correctionLengthDrivetrainVolume;
			
			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var volume = pasengerCompartmentLength * internalHeight * completedVehicle.Width;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
			 	hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
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


		protected double GetNumberOfPassengers(Mission mission, Meter length, Meter width, double registeredPassengerSeats,
			double registeredPassengersStanding, LoadingType loading)
		{
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(length, width);
			var passengerCountRef = busFloorArea * (loading == LoadingType.LowLoading
				? mission.BusParameter.PassengerDensityLow
				: mission.BusParameter.PassengerDensityRef);
			
			if (loading != LoadingType.ReferenceLoad && loading != LoadingType.LowLoading) {
				throw new VectoException("Unhandled loading type: {0}", loading);
			}

			var passengerCount = registeredPassengerSeats +
								(mission.MissionType == MissionType.Coach ? 0 : registeredPassengersStanding);

			return loading == LoadingType.ReferenceLoad
				? VectoMath.Min(passengerCountRef, passengerCount)
				: VectoMath.Min(passengerCountRef * mission.MissionType.GetLowLoadFactorBus(), passengerCount);
		}
		
		
		


	}
}

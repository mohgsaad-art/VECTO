/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Models.Declaration
{
	public static class DeclarationData
	{
		/// <summary>
		/// The standard acceleration for gravity on earth.
		/// http://physics.nist.gov/Pubs/SP330/sp330.pdf (page 52)
		/// </summary>
		public static readonly MeterPerSquareSecond GravityAccelleration = 9.80665.SI<MeterPerSquareSecond>();

		public static readonly KilogramPerCubicMeter AirDensity = 1.188.SI<KilogramPerCubicMeter>();


		public const string DeclarationDataResourcePrefix = "TUGraz.VectoCore.Resources.Declaration";

		public static readonly Watt MinEnginePowerForEMS = 300e3.SI<Watt>();

		public static readonly TruckSegments TruckSegments = new TruckSegments();
		public static readonly PrimaryBusSegments PrimaryBusSegments = new PrimaryBusSegments();
		public static readonly CompletedBusSegments CompletedBusSegments = new CompletedBusSegments();
		public static readonly Wheels Wheels = new Wheels();
		public static readonly PT1 PT1 = new PT1();
		public static readonly FuelData FuelData = FuelData.Instance();
		public static readonly ElectricSystem ElectricSystem = new ElectricSystem();
		public static readonly Fan Fan = new Fan();

		public static readonly HeatingVentilationAirConditioning HeatingVentilationAirConditioning =
			new HeatingVentilationAirConditioning();

		public static readonly PneumaticSystem PneumaticSystem = new PneumaticSystem();
		public static readonly SteeringPump SteeringPump = new SteeringPump();
		public static readonly SteeringPumpBus SteeringPumpBus = new SteeringPumpBus();
		public static readonly WHTCCorrection WHTCCorrection = new WHTCCorrection();
		public static readonly AirDrag AirDrag = new AirDrag();
		public static readonly StandardBodies StandardBodies = new StandardBodies();
		public static readonly Payloads Payloads = new Payloads();

		public static readonly PTOTransmission PTOTransmission = new PTOTransmission();

		//public static MeterPerSecond CycleSpeedLimit;
		public const double LossMapExtrapolationFactor = 6;

		public static readonly ADASCombinations ADASCombinations = new ADASCombinations();

		public static readonly WeightingGroups WeightingGroup = new WeightingGroups();
		public static readonly WeightingFactors WeightingFactors = new WeightingFactors();

		public const double AlternatorEfficiency = 0.7;

		public const double WHRChargerEfficiency = 0.98;

		public static readonly ConcurrentDictionary<MissionType, DrivingCycleData> CyclesCache =
			new ConcurrentDictionary<MissionType, DrivingCycleData>();



		/// <summary>
		/// Formula for calculating the payload for a given gross vehicle weight.
		/// (so called "pc-formula", Whitebook Apr 2016, Part 1, p.187)
		/// </summary>
		public static Kilogram GetPayloadForGrossVehicleWeight(Kilogram grossVehicleWeight, string equationName)
		{
			if (equationName.ToLower().StartsWith("pc10")) {
				return Payloads.Lookup10Percent(grossVehicleWeight);
			}

			if (equationName.ToLower().StartsWith("pc75")) {
				return Payloads.Lookup75Percent(grossVehicleWeight);
			}

			return Payloads.Lookup50Percent(grossVehicleWeight);
		}

		/// <summary>
		/// Returns the payload for a trailer. This is 75% of (GVW-CurbWeight).
		/// </summary>
		public static Kilogram GetPayloadForTrailerWeight(Kilogram grossVehicleWeight, Kilogram curbWeight,
			bool lowLoading)
		{
			return
				(Math.Round(
					(Payloads.LookupTrailer(grossVehicleWeight, curbWeight) / (lowLoading ? 7.5 : 1)).LimitTo(
						0.SI<Kilogram>(),
						grossVehicleWeight - curbWeight).Value() / 100, 0) * 100).SI<Kilogram>();
		}




		public static class BusAuxiliaries
		{
			//private static ISSMInputs ssmInputs = null;

			private static IEnvironmentalConditionsMap envMap;

			//private static AuxiliaryConfig busAuxConfig = null;
			private static ElectricalConsumerList elUserConfig;

			private static IActuationsMap actuationsMap;
			//private static PneumaticsAuxilliariesConfig pneumaticAuxConfig;
			private static List<SSMTechnology> ssmTechnologies;


			//public static ISSMInputs SSMDefaultValues
			//{
			//	get {
			//		return ssmInputs ?? (ssmInputs = SSMInputData.ReadStream(
			//					RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.SSMDefaults.AHSM"),
			//					DefaultEnvironmentalConditions));
			//	}
			//}

			public static ICompressorMap GetCompressorMap(string compressorSize, string clutchType)
			{
				var resource = GetCompressorResourceForSize(compressorSize);

				var dragCurveFactorClutch = 1.0;
				switch (clutchType) {
					case "visco":
						dragCurveFactorClutch = Constants.BusAuxiliaries.PneumaticUserConfig.ViscoClutchDragCurveFactor;
						break;
					case "mechanically":
						dragCurveFactorClutch = Constants.BusAuxiliaries.PneumaticUserConfig.MechanicClutchDragCurveFactor;
						break;
				}

				return CompressorMapReader.ReadStream(
					RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".VAUXBus." + resource), dragCurveFactorClutch, $"{compressorSize} - {clutchType}");
			}

			private static string GetCompressorResourceForSize(string compressorSize)
			{
				switch (compressorSize) {
					case "Small": return "DEFAULT_1-Cylinder_1-Stage_393ccm.acmp";
					case "Medium Supply 1-stage": return "DEFAULT_1-Cylinder_1-Stage_393ccm.acmp";
					case "Medium Supply 2-stage": return "DEFAULT_2-Cylinder_1-Stage_650ccm.acmp";
					case "Large Supply 1-stage": return "DEFAULT_2-Cylinder_2-Stage_398ccm.acmp";
					case "Large Supply 2-stage": return "DEFAULT_3-Cylinder_2-Stage_598ccm.acmp";
					default: throw new ArgumentException($"unknown compressor size {compressorSize}", compressorSize);
				}
			}

			public static BusAlternatorTechnologies AlternatorTechnologies = new BusAlternatorTechnologies();
			private static HVACCoolingPower hvacMaxCoolingPower;

			public static List<SSMTechnology> SSMTechnologyList =>
				ssmTechnologies ?? (ssmTechnologies = SSMTechnologiesReader.ReadFromStream(
					RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.SSMTechList.csv")));

			public static IEnvironmentalConditionsMap DefaultEnvironmentalConditions =>
				envMap ?? (envMap = EnvironmentalContidionsMapReader.ReadStream(
					RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.DefaultClimatic.aenv")));

			public static ElectricalConsumerList DefaultElectricConsumerList =>
				elUserConfig ?? (elUserConfig = ElectricConsumerReader.ReadStream(
					RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.ElectricConsumers.csv")));


			public static IActuationsMap ActuationsMap =>
				actuationsMap ?? (actuationsMap = ActuationsMapReader.ReadStream(
					RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.DefaultActuationsMap.apac")));

			public static HVACCoolingPower HVACMaxCoolingPower => hvacMaxCoolingPower ?? (hvacMaxCoolingPower = new HVACCoolingPower());

			public static PerSecond VentilationRate(BusHVACSystemConfiguration? hvacSystemConfig, bool heating)
			{

				switch (hvacSystemConfig) {

					case BusHVACSystemConfiguration.Configuration1:
					case BusHVACSystemConfiguration.Configuration2:
						return Constants.BusAuxiliaries.SteadyStateModel.LowVentilation;

					case BusHVACSystemConfiguration.Configuration3:
					case BusHVACSystemConfiguration.Configuration4:
					case BusHVACSystemConfiguration.Configuration5:
					case BusHVACSystemConfiguration.Configuration6:
					case BusHVACSystemConfiguration.Configuration7:
					case BusHVACSystemConfiguration.Configuration8:
					case BusHVACSystemConfiguration.Configuration9:
					case BusHVACSystemConfiguration.Configuration10:
						return heating
							? Constants.BusAuxiliaries.SteadyStateModel.HighVentilationHeating
							: Constants.BusAuxiliaries.SteadyStateModel.HighVentilation;

					default: throw new ArgumentOutOfRangeException(nameof(hvacSystemConfig), hvacSystemConfig, null);
				}
			}

			public static SquareMeter CalculateBusFloorSurfaceArea(Meter busLength, Meter busWidth)
			{
				return (busLength - Constants.BusParameters.DriverCompartmentLength) * CorrectedBusWidth(busWidth);
			}

			public static Meter CorrectedBusWidth(Meter busWidth)
			{
				return busWidth.IsBetween(Constants.BusParameters.VehicleWidthLow, Constants.BusParameters.VehicleWidthHigh)
					? Constants.BusParameters.VehicleWidthHigh
					: busWidth;
			}


			public static Meter CalculateInternalLength(Meter vehicleLength, VehicleCode? vehicleCode, double numPassSeatsLowerDeck)
			{
				if (vehicleCode.GetFloorType() == FloorType.LowFloor) {
					return vehicleCode.IsDoubleDeckerBus() ? 2 * vehicleLength : vehicleLength;
				}

				if (vehicleCode.GetFloorType() == FloorType.HighFloor) {
					if (vehicleCode.IsDoubleDeckerBus()) {
						return numPassSeatsLowerDeck > 6 ? 1.5 * vehicleLength : vehicleLength + 2.4.SI<Meter>();
					}

					return vehicleLength;
				}
				throw new VectoException("Internal Length for floorType {0} {1} not defined", vehicleCode.GetFloorType().ToString(), vehicleCode.IsDoubleDeckerBus() ? "DD" : "SD");
			}

			public static Meter CalculateLengthInteriorLights(
				Meter vehicleLength, VehicleCode? vehicleCode, double numPassLowFloor)
			{
				return CalculateInternalLength(vehicleLength, vehicleCode, numPassLowFloor);
			}

			public static Meter CalculateInternalHeight(VehicleCode? vehicleCode, RegistrationClass? registrationClass, Meter bodyHeight)
			{
				if (vehicleCode.IsDoubleDeckerBus()) {
					return Constants.BusParameters.InternalHeightDoubleDecker;
				}

				switch (vehicleCode.GetFloorType()) {
					case FloorType.LowFloor:
						return bodyHeight;
					case FloorType.HighFloor:
						if ((registrationClass == RegistrationClass.II_III && bodyHeight > 3.1.SI<Meter>()) ||
							registrationClass == RegistrationClass.III || registrationClass == RegistrationClass.B) {
							return Constants.BusParameters.InternalHeightDoubleDecker;
						}
						return bodyHeight - Constants.BusParameters.HeightLuggageCompartment;
				}

				throw new VectoException("Internal height for vehicle floor type '{0}' {1} not defined", vehicleCode.GetFloorType().ToString(), vehicleCode.IsDoubleDeckerBus() ? "double decker" : "single decker");
			}

			public static Meter WindowHeight(bool doubleDecker)
			{
				return doubleDecker
					? Constants.BusParameters.WindowHeightDoubleDecker
					: Constants.BusParameters.WindowHeightSingleDecker;
			}

			public static SquareMeter FrontAndRearWindowArea(bool doubleDecker)
			{
				return doubleDecker
					? Constants.BusParameters.FrontAndRearWindowAreaDoubleDecker
					: Constants.BusParameters.FrontAndRearWindowAreaSingleDecker;
			}

			public static WattPerKelvinSquareMeter UValue(FloorType floorType)
			{
				switch (floorType) {
					case FloorType.LowFloor:
						return 4.SI<WattPerKelvinSquareMeter>();
					case FloorType.SemiLowFloor:
						return 3.5.SI<WattPerKelvinSquareMeter>();
					case FloorType.HighFloor:
						return 3.SI<WattPerKelvinSquareMeter>();
					default: throw new ArgumentOutOfRangeException(nameof(floorType), floorType, null);
				}
			}

			public static double CalculateCOP(Watt coolingPwrDriver, HeatPumpType comprTypeDriver, Watt coolingPwrPass, HeatPumpType comprTypePass, FloorType floorType)
			{
				if (coolingPwrDriver.IsGreater(0) && comprTypeDriver == HeatPumpType.none) {
					comprTypeDriver = comprTypePass;
				}
				if (coolingPwrDriver.IsEqual(0) && coolingPwrPass.IsEqual(0)) {
					return 1.0;
				}
				return (coolingPwrDriver * comprTypeDriver.COP(floorType) + coolingPwrPass * comprTypePass.COP(floorType)) /
						(coolingPwrDriver + coolingPwrPass);
			}

			public static Meter CorrectionLengthDrivetrainVolume(VehicleCode? vehicleCode, bool? lowEntry, int numAxles, bool articulated)
			{
				if ((vehicleCode == VehicleCode.CE || vehicleCode == VehicleCode.CG) && (bool)lowEntry) {
					switch (numAxles) {
						case 2: return 1.0.SI<Meter>();
						case 3: return articulated ? 1.0.SI<Meter>() : 1.25.SI<Meter>();
						case 4: return 1.25.SI<Meter>();
						default: throw new VectoException("invalid number of axles {0}", numAxles);
					}
				}
				return 0.SI<Meter>();
			}
		}

		public static class Driver
		{
			public static class LookAhead
			{
				public const bool Enabled = true;

				public const double DecisionFactorCoastingOffset = 2.5;
				public const double DecisionFactorCoastingScaling = 1.5;
				public const double LookAheadDistanceFactor = 10;
				public static readonly MeterPerSecond MinimumSpeed = 50.KMPHtoMeterPerSecond();
			}

			public static class OverSpeed
			{
				public static readonly MeterPerSecond MinSpeed = 50.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond AllowedOverSpeed = 2.5.KMPHtoMeterPerSecond();
			}

			public static class EngineStopStart
			{
				public static readonly Second ActivationDelay = 2.SI<Second>();
				public static readonly Second MaxEngineOffTimespan = 120.SI<Second>();
				public const double UtilityFactor = 0.8;
			}

			public static class EcoRoll
			{
				public static readonly MeterPerSecond MinSpeed = 60.KMPHtoMeterPerSecond();
				public static readonly Second ActivationDelay = 2.SI<Second>();
				public static readonly MeterPerSecond UnderspeedThreshold = 0.KMPHtoMeterPerSecond();

				public static readonly MeterPerSquareSecond AccelerationLowerLimit = 0.SI<MeterPerSquareSecond>();
				public static readonly MeterPerSquareSecond AccelerationUpperLimit = 0.1.SI<MeterPerSquareSecond>();
			}

			public static class PCC
			{
				public static readonly MeterPerSecond PCCEnableSpeed = 80.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond MinSpeed = 50.KMPHtoMeterPerSecond();
				public static readonly Meter PreviewDistanceUseCase1 = 1500.SI<Meter>();
				public static readonly Meter PreviewDistanceUseCase2 = 1000.SI<Meter>();
				public static readonly MeterPerSecond Underspeed = 8.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond OverspeedUseCase3 = 5.KMPHtoMeterPerSecond();
			}
		}

		public static class Trailer
		{
			public const double RollResistanceCoefficient = 0.0055;
			public const string FuelEfficiencyClass = "X";
			public const double TyreTestLoad = 37500;

			public const bool TwinTyres = false;

			//public const string WheelsType = "385/65 R 22.5";
		}

		public static class Engine
		{
			public static readonly KilogramSquareMeter ClutchInertia = 1.3.SI<KilogramSquareMeter>();
			public static readonly KilogramSquareMeter TorqueConverterInertia = 1.2.SI<KilogramSquareMeter>();

			public static readonly KilogramSquareMeter EngineBaseInertia = 0.41.SI<KilogramSquareMeter>();
			public static readonly SI EngineDisplacementInertia = (0.27 * 1000).SI(Unit.SI.Kilo.Gramm.Per.Meter); // [kg/m]
			public static readonly Second DefaultEngineStartTime = 1.SI<Second>();

			public const double TorqueLimitGearboxFactor = 0.9;
			public const double TorqueLimitVehicleFactor = 0.95;

			public static KilogramSquareMeter EngineInertia(CubicMeter displacement, GearboxType gbxType)
			{
				// VB Code:    Return 1.3 + 0.41 + 0.27 * (Displ / 1000)
				return (gbxType.AutomaticTransmission() ? TorqueConverterInertia : ClutchInertia) + EngineBaseInertia +
						EngineDisplacementInertia * displacement;
			}
		}

		public static class GearboxTCU
		{

			public static readonly MeterPerSecond MIN_SPEED_AFTER_TRACTION_INTERRUPTION = 5.KMPHtoMeterPerSecond();

			public const double TorqueReserve = 0;
			public const double TorqueReserveStart = 0.2;

			public static readonly MeterPerSecond StartSpeed = 8.KMPHtoMeterPerSecond();
			public static readonly MeterPerSquareSecond StartAcceleration = 0.8.SI<MeterPerSquareSecond>();

			public static readonly Second GearResidenceTime = 5.SI<Second>();
			public static readonly Watt CurrentCardanPowerThresholdPropulsion = 5000.SI<Watt>();
			public static readonly Watt AverageCardanPowerThresholdPropulsion = 1000.SI<Watt>();
			public static readonly Second LookBackInterval = 4.SI<Second>();
			public static readonly Second DriverAccelerationLookBackInterval = 2.SI<Second>();
			public const double EngineSpeedHighDriveOffFactor = 1.05;
			public const double DnT99L_highMin1 = 0.4;
			public const double DnT99L_highMin2 = 0.5;

			public const int AllowedGearRangeUp = 3;
			public const int AllowedGearRangeDown = 3;

			public const double TargetSpeedDeviationFactor = 0.1;

			public static double RatingFactorCurrentGear = 0.97;
			public static double RatingFactorCurrentGearAT = 0.97;

			public static readonly MeterPerSquareSecond DriverAccelerationThresholdLow = 0.1.SI<MeterPerSquareSecond>();
			public static double VelocityDropFactor = 1.0;
			public static double AccelerationFactor = 0.5;

			public static double RatioEarlyUpshiftFC = 24;
			public static double RatioEarlyDownshiftFC = 24;

			public static int AllowedGearRangeFCAMT = 2;
			public static int AllowedGearRangeFCAT = 1;
			public static int AllowedGearRangeFCATSkipGear = 2;
			public static int ATSkipGearsThreshold = 6;

			public static PerSecond MinEngineSpeedPostUpshift = 0.RPMtoRad();

			public static Second ATLookAheadTime = 1.5.SI<Second>(); //Gearbox.PowershiftShiftTime;

			public static double[] LoadStageThresholdsUp = { 19.7, 36.34, 53.01, 69.68, 86.35 };
			public static double[] LoadStageThresoldsDown = { 13.7, 30.34, 47.01, 63.68, 80.35 };

			public static double[][] ShiftSpeedsTCToLocked = {
				new[] {  90.0, 120, 165,  90, 120, 165 },
				new[] {  90.0, 120, 165,  90, 120, 165 },
				new[] {  90.0, 120, 165,  90, 120, 165 },
				new[] {  90.0, 120, 165, 110, 140, 185 },
				new[] { 100.0, 130, 175, 120, 150, 195 },
				new[] { 110.0, 140, 185, 130, 160, 205 },
			};

			public const double DownhillSlope = -5;
			public const double UphillSlope = 5;

			public const double DragMarginFactor = 0.7;


			// TODO: MQ 2019-11-26 remove, once the parameters are fixed! make fields above read-only or const
			static GearboxTCU()
			{
				//#if RELEASE_CANDIDATE
				var expectedFile = @"Declaration\EffShiftParameters.vtcu";
				if (!File.Exists(expectedFile)) {
					return;
				}

				var tcuData = JSONInputDataFactory.ReadShiftParameters(expectedFile, true);
				if (tcuData.RatingFactorCurrentGear.HasValue) {
					RatingFactorCurrentGear = tcuData.RatingFactorCurrentGear.Value;
					RatingFactorCurrentGearAT = tcuData.RatingFactorCurrentGear.Value;
				}
				if (tcuData.RatioEarlyDownshiftFC.HasValue) {
					RatioEarlyDownshiftFC = tcuData.RatioEarlyDownshiftFC.Value;
				}
				if (tcuData.RatioEarlyUpshiftFC.HasValue) {
					RatioEarlyUpshiftFC = tcuData.RatioEarlyUpshiftFC.Value;
				}
				if (tcuData.AllowedGearRangeFC.HasValue) {
					AllowedGearRangeFCAMT = tcuData.AllowedGearRangeFC.Value;
					AllowedGearRangeFCAT = tcuData.AllowedGearRangeFC.Value;
				}
				if (tcuData.VeloictyDropFactor.HasValue) {
					VelocityDropFactor = tcuData.VeloictyDropFactor.Value;
				}
				if (tcuData.AccelerationFactor.HasValue) {
					AccelerationFactor = tcuData.AccelerationFactor.Value;
				}
				if (tcuData.ATLookAheadTime != null) {
					ATLookAheadTime = tcuData.ATLookAheadTime;
				}
				if (tcuData.LoadStageThresholdsDown != null && LoadStageThresoldsDown.Length > 0) {
					LoadStageThresoldsDown = tcuData.LoadStageThresholdsDown.ToArray();
				}
				if (tcuData.LoadStageThresholdsUp != null && LoadStageThresholdsUp.Length > 0) {
					LoadStageThresholdsUp = tcuData.LoadStageThresholdsUp.ToArray();
				}
				if (tcuData.ShiftSpeedsTCToLocked != null && ShiftSpeedsTCToLocked.Length > 0) {
					ShiftSpeedsTCToLocked = tcuData.ShiftSpeedsTCToLocked;
				}
				if (tcuData.MinEngineSpeedPostUpshift != null) {
					MinEngineSpeedPostUpshift = tcuData.MinEngineSpeedPostUpshift;
				}
				//#endif
			}
		}

		public static class Gearbox
		{
			public static readonly KilogramSquareMeter Inertia = 0.SI<KilogramSquareMeter>();

			public static readonly MeterPerSecond TruckMaxAllowedSpeed = 85.KMPHtoMeterPerSecond();
			public const double ShiftPolygonRPMMargin = 7; // %
			private const double ShiftPolygonEngineFldMargin = 0.98;

			public static readonly Second MinTimeBetweenGearshifts = 2.SI<Second>();
			public static readonly Second DownshiftAfterUpshiftDelay = 6.SI<Second>();
			public static readonly Second UpshiftAfterDownshiftDelay = 6.SI<Second>();

			public static readonly MeterPerSquareSecond UpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();

			//public static readonly PerSecond TorqueConverterSpeedLimit = 1600.RPMtoRad();
			public static double TorqueConverterSecondGearThreshold(VehicleCategory category)
			{
				return category.IsLorry() ? 1.8 : 1.85;
			}

			public static readonly Second PowershiftShiftTime = 0.8.SI<Second>();

			/// <summary>
			/// computes the shift polygons for a single gear according to the whitebook 2016
			/// </summary>
			/// <param name="type"></param>
			/// <param name="gearIdx">index of the gear to compute the shift polygons for  -- gear number - 1!</param>
			/// <param name="fullLoadCurve">engine full load curve, potentially limited by the gearbox</param>
			/// <param name="gears">list of gears</param>
			/// <param name="engine">engine data</param>
			/// <param name="axlegearRatio"></param>
			/// <param name="dynamicTyreRadius"></param>
			/// <param name="electricMotorData"></param>
			/// <returns></returns>
			public static ShiftPolygon ComputeShiftPolygon(
				GearboxType type, int gearIdx, EngineFullLoadCurve fullLoadCurve,
				IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData)
			{
				switch (type) {
					case GearboxType.AMT:
					case GearboxType.APTN:
					case GearboxType.IHPC:
						// TODO MQ: 2020-10-14: compute for AMT with ICE and AMT with EM differently
						return ComputeEfficiencyShiftPolygon(gearIdx, fullLoadCurve, gears, engine, axlegearRatio, dynamicTyreRadius);
					case GearboxType.MT:
						return ComputeManualTransmissionShiftPolygon(
							gearIdx, fullLoadCurve, gears, engine, axlegearRatio, dynamicTyreRadius);
					case GearboxType.ATSerial:
					case GearboxType.ATPowerSplit:
						return TorqueConverter.ComputeShiftPolygon(fullLoadCurve, gearIdx == 0, gearIdx >= gears.Count - 1);
					case GearboxType.DrivingCycle: break;
					default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}

				return type.AutomaticTransmission()
					? TorqueConverter.ComputeShiftPolygon(fullLoadCurve, gearIdx == 0, gearIdx >= gears.Count - 1)

					// That's the same for all gears, so call the same method...
					: ComputeManualTransmissionShiftPolygon(gearIdx, fullLoadCurve, gears, engine, axlegearRatio, dynamicTyreRadius);
			}

			public static ShiftPolygon ComputeElectricMotorShiftPolygon(int gearIdx,
				ElectricMotorFullLoadCurve fullLoadCurve, double emRatio, IList<ITransmissionInputData> gears,
				double axlegearRatio, Meter dynamicTyreRadius, PerSecond downshiftMaxSpeed = null, PerSecond downshiftMinSpeed = null)
			{
				if (gears.Count < 2) {
					throw new VectoException("ComputeShiftPolygon needs at least 2 gears. {0} gears given.", gears.Count);
				}

				var downShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				var upShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx > 0) {
					var nMax = downshiftMaxSpeed ?? fullLoadCurve.NP80low;
					var nMin = downshiftMinSpeed ?? 0.1 * fullLoadCurve.RatedSpeed;

					downShift.AddRange(DownshiftLineDrive(fullLoadCurve, nMin, fullLoadCurve.NP80low));
					downShift.AddRange(DownshiftLineDrag(fullLoadCurve, nMin, nMax));

				}
				if (gearIdx >= gears.Count - 1) {
					return new ShiftPolygon(downShift, upShift);
				}

				upShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxGenerationTorque * 1.1, fullLoadCurve.MaxSpeed * 0.9));
				upShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxDriveTorque * 1.1, fullLoadCurve.MaxSpeed * 0.9));
				return new ShiftPolygon(downShift, upShift);
			}

			private static List<ShiftPolygon.ShiftPolygonEntry> DownshiftLineDrive(ElectricMotorFullLoadCurve fullLoadCurve, PerSecond nMin, PerSecond nMax)
			{
				var retVal = new List<ShiftPolygon.ShiftPolygonEntry>();
				var downShiftPoints = fullLoadCurve
					.FullLoadEntries.Where(fldEntry => fldEntry.MotorSpeed >= nMin && fldEntry.MotorSpeed <= nMax)
					.Select(
						fldEntry =>
							new Point(fldEntry.MotorSpeed.Value(), fldEntry.FullDriveTorque.Value() * ShiftPolygonEngineFldMargin))
					.ToList();
				//retVal.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxDriveTorque * 1.1, nMax));
				if (downShiftPoints.Count == 0) {
					// coarse grid points in FLD
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.MaxDriveTorque * 1.1,
							nMax));
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.FullLoadDriveTorque(nMax) * ShiftPolygonEngineFldMargin,
							nMax));
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.FullLoadDriveTorque(nMin) * ShiftPolygonEngineFldMargin,
							nMin));

				} else {
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.MaxDriveTorque * 1.1,
							nMax));
					if (downShiftPoints.Max(x => x.X) < nMax) {
						retVal.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullLoadDriveTorque(nMax) * ShiftPolygonEngineFldMargin,
								nMax));
					}

					retVal.AddRange(
						downShiftPoints.Select(
							x => new ShiftPolygon.ShiftPolygonEntry(
								x.Y.SI<NewtonMeter>(), x.X.SI<PerSecond>())).OrderByDescending(x => x.AngularSpeed.Value()));
					if (downShiftPoints.Min(x => x.X) > nMin) {
						retVal.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullLoadDriveTorque(nMin) * ShiftPolygonEngineFldMargin,
								nMin));
					}
				}

				return retVal;
			}

			private static List<ShiftPolygon.ShiftPolygonEntry> DownshiftLineDrag(ElectricMotorFullLoadCurve fullLoadCurve, PerSecond nMin, PerSecond nMax)
			{
				var retVal = new List<ShiftPolygon.ShiftPolygonEntry>();
				var downShiftPoints = fullLoadCurve
					.FullLoadEntries.Where(fldEntry => fldEntry.MotorSpeed >= nMin && fldEntry.MotorSpeed <= nMax)
					.Select(
						fldEntry =>
							new Point(fldEntry.MotorSpeed.Value(), fldEntry.FullGenerationTorque.Value() * ShiftPolygonEngineFldMargin))
					.ToList();
				//retVal.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxGenerationTorque * 1.1, nMax));
				if (downShiftPoints.Count == 0) {
					// coarse grid points in FLD
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.FullGenerationTorque(nMin) * ShiftPolygonEngineFldMargin,
							nMin));
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.FullGenerationTorque(nMax) * ShiftPolygonEngineFldMargin,
							nMax));
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.MaxGenerationTorque * 1.1,
							nMax));
				} else {
					if (downShiftPoints.Min(x => x.X) > nMin) {
						retVal.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullGenerationTorque(nMin) * ShiftPolygonEngineFldMargin,
								nMin));
					}

					retVal.AddRange(
						downShiftPoints.Select(
							x => new ShiftPolygon.ShiftPolygonEntry(
								x.Y.SI<NewtonMeter>(), x.X.SI<PerSecond>())));
					if (downShiftPoints.Max(x => x.X) < nMax) {
						retVal.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullGenerationTorque(nMax) * ShiftPolygonEngineFldMargin,
								nMax));
					}
					retVal.Add(
						new ShiftPolygon.ShiftPolygonEntry(
							fullLoadCurve.MaxGenerationTorque * 1.1,
							nMax));
				}

				return retVal;
			}

			public static ShiftPolygon ComputeEfficiencyShiftPolygon(
				int gearIdx, EngineFullLoadCurve fullLoadCurve, IList<ITransmissionInputData> gears, CombustionEngineData engine,
				double axlegearRatio, Meter dynamicTyreRadius)
			{
				if (gears.Count < 2) {
					throw new VectoException("ComputeShiftPolygon needs at least 2 gears. {0} gears given.", gears.Count);
				}

				var clutchClosingSpeed = (engine.FullLoadCurves[0].RatedSpeed - engine.IdleSpeed) *
									Constants.SimulationSettings.ClutchClosingSpeedNorm + engine.IdleSpeed;

				var p2 = new Point(Math.Min((clutchClosingSpeed - 10.RPMtoRad()).Value(), engine.IdleSpeed.Value() * 1.1), 0);
				var p3 = new Point(fullLoadCurve.NTq99lSpeed.Value(), 0);
				var p5 = new Point(fullLoadCurve.NP98hSpeed.Value(), fullLoadCurve.MaxTorque.Value() * 1.1);

				var downShift = new List<ShiftPolygon.ShiftPolygonEntry>();

				if (gearIdx > 0) {
					var downShiftPoints = fullLoadCurve
						.FullLoadEntries.Where(fldEntry => fldEntry.EngineSpeed >= p2.X && fldEntry.EngineSpeed <= p3.X)
						.Select(
							fldEntry =>
								new Point(fldEntry.EngineSpeed.Value(), fldEntry.TorqueFullLoad.Value() * ShiftPolygonEngineFldMargin))
						.ToList();
					downShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxDragTorque * 1.1, p2.X.SI<PerSecond>()));
					if (downShiftPoints.Count == 0) {
						// coarse grid points in FLD
						downShift.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullLoadStationaryTorque(p2.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
								p2.X.SI<PerSecond>()));
						downShift.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullLoadStationaryTorque(p3.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
								p3.X.SI<PerSecond>()));
					} else {
						if (downShiftPoints.Min(x => x.X) > p2.X) {
							downShift.Add(
								new ShiftPolygon.ShiftPolygonEntry(
									fullLoadCurve.FullLoadStationaryTorque(p2.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
									p2.X.SI<PerSecond>()));
						}

						downShift.AddRange(
							downShiftPoints.Select(
								x => new ShiftPolygon.ShiftPolygonEntry(
									x.Y.SI<NewtonMeter>() * ShiftPolygonEngineFldMargin, x.X.SI<PerSecond>())));
						if (downShiftPoints.Max(x => x.X) < p3.X) {
							downShift.Add(
								new ShiftPolygon.ShiftPolygonEntry(
									fullLoadCurve.FullLoadStationaryTorque(p3.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
									p3.X.SI<PerSecond>()));
						}
					}
					downShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxTorque * 1.1, p3.X.SI<PerSecond>()));
				}
				var upShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx >= gears.Count - 1) {
					return new ShiftPolygon(downShift, upShift);
				}

				upShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxDragTorque * 1.1, p5.X.SI<PerSecond>()));
				upShift.Add(new ShiftPolygon.ShiftPolygonEntry(p5.Y.SI<NewtonMeter>(), p5.X.SI<PerSecond>()));
				return new ShiftPolygon(downShift, upShift);
			}


			public static ShiftPolygon ComputeManualTransmissionShiftPolygon(
				int gearIdx, EngineFullLoadCurve fullLoadCurve,
				IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius)
			{
				if (gears.Count < 2) {
					throw new VectoException("ComputeShiftPolygon needs at least 2 gears. {0} gears given.", gears.Count);
				}

				// ReSharper disable once InconsistentNaming
				var engineSpeed85kmhLastGear = ComputeEngineSpeed85kmh(gears[gears.Count - 1], axlegearRatio, dynamicTyreRadius);

				var nVHigh = VectoMath.Min(engineSpeed85kmhLastGear, engine.FullLoadCurves[0].RatedSpeed);

				var diffRatio = gears[gears.Count - 2].Ratio / gears[gears.Count - 1].Ratio - 1;

				var maxDragTorque = fullLoadCurve.MaxDragTorque * 1.1;

				var p1 = new Point(engine.IdleSpeed.Value() / 2, 0);
				var p2 = new Point(engine.IdleSpeed.Value() * 1.1, 0);
				var p3 = new Point(
					nVHigh.Value() * 0.9,
					fullLoadCurve.FullLoadStationaryTorque(nVHigh * 0.9).Value());

				var p4 = new Point((nVHigh * (1 + diffRatio / 3)).Value(), 0);
				var p5 = new Point(fullLoadCurve.N95hSpeed.Value(), fullLoadCurve.MaxTorque.Value());

				var p6 = new Point(p2.X, VectoMath.Interpolate(p1, p3, p2.X));
				var p7 = new Point(p4.X, VectoMath.Interpolate(p2, p5, p4.X));

				var fldMargin = ShiftPolygonFldMargin(fullLoadCurve.FullLoadEntries, (p3.X * 0.95).SI<PerSecond>());
				var downshiftCorr = MoveDownshiftBelowFld(Edge.Create(p6, p3), fldMargin, 1.1 * fullLoadCurve.MaxTorque);

				var downShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx > 0) {
					downShift =
						new[] { p2, downshiftCorr.P1, downshiftCorr.P2 }.Select(
																			point => new ShiftPolygon.ShiftPolygonEntry(point.Y.SI<NewtonMeter>(), point.X.SI<PerSecond>()))
																		.ToList();

					downShift[0].Torque = maxDragTorque;
				}
				var upShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx >= gears.Count - 1) {
					return new ShiftPolygon(downShift, upShift);
				}

				var gearRatio = gears[gearIdx].Ratio / gears[gearIdx + 1].Ratio;
				var rpmMarginFactor = 1 + ShiftPolygonRPMMargin / 100.0;

				// ReSharper disable InconsistentNaming
				var p2p = new Point(p2.X * gearRatio * rpmMarginFactor, p2.Y / gearRatio);
				var p3p = new Point(p3.X * gearRatio * rpmMarginFactor, p3.Y / gearRatio);
				var p6p = new Point(p6.X * gearRatio * rpmMarginFactor, p6.Y / gearRatio);
				var edgeP6pP3p = new Edge(p6p, p3p);
				var p3pExt = new Point((1.1 * p5.Y - edgeP6pP3p.OffsetXY) / edgeP6pP3p.SlopeXY, 1.1 * p5.Y);

				// ReSharper restore InconsistentNaming

				var upShiftPts = IntersectTakeHigherShiftLine(new[] { p4, p7, p5 }, new[] { p2p, p6p, p3pExt });
				if (gears[gearIdx].MaxInputSpeed != null) {
					var maxSpeed = gears[gearIdx].MaxInputSpeed.Value();
					upShiftPts = IntersectTakeLowerShiftLine(
						upShiftPts,
						new[] { new Point(maxSpeed, 0), new Point(maxSpeed, upShiftPts.Max(pt => pt.Y)) });
				}
				upShift =
					upShiftPts.Select(point => new ShiftPolygon.ShiftPolygonEntry(point.Y.SI<NewtonMeter>(), point.X.SI<PerSecond>()))
							.ToList();
				upShift[0].Torque = maxDragTorque;
				return new ShiftPolygon(downShift, upShift);
			}

			/// <summary>
			/// ensures the original downshift line is below the (already reduced) full-load curve
			/// </summary>
			/// <param name="shiftLine">second part of the shift polygon (slope)</param>
			/// <param name="fldMargin">reduced full-load curve</param>
			/// <param name="maxTorque">max torque</param>
			/// <returns>returns a corrected shift polygon segment (slope) that is below the full load curve and reaches given maxTorque. the returned segment has the same slope</returns>
			internal static Edge MoveDownshiftBelowFld(Edge shiftLine, IEnumerable<Point> fldMargin, NewtonMeter maxTorque)
			{
				var slope = shiftLine.SlopeXY;
				var d = shiftLine.P2.Y - slope * shiftLine.P2.X;

				d = fldMargin.Select(point => point.Y - slope * point.X).Concat(new[] { d }).Min();
				var p6Corr = new Point(shiftLine.P1.X, shiftLine.P1.X * slope + d);
				var p3Corr = new Point((maxTorque.Value() - d) / slope, maxTorque.Value());
				return Edge.Create(p6Corr, p3Corr);
			}

			/// <summary>
			/// reduce the torque of the full load curve up to the given rpms
			/// </summary>
			/// <param name="fullLoadCurve"></param>
			/// <param name="rpmLimit"></param>
			/// <returns></returns>
			internal static IEnumerable<Point> ShiftPolygonFldMargin(
				List<EngineFullLoadCurve.FullLoadCurveEntry> fullLoadCurve,
				PerSecond rpmLimit)
			{
				return fullLoadCurve.TakeWhile(fldEntry => fldEntry.EngineSpeed < rpmLimit)
									.Select(
										fldEntry =>
											new Point(fldEntry.EngineSpeed.Value(), fldEntry.TorqueFullLoad.Value() * ShiftPolygonEngineFldMargin))
									.ToList();
			}

			// ReSharper disable once InconsistentNaming
			private static PerSecond ComputeEngineSpeed85kmh(
				ITransmissionInputData gear, double axleRatio,
				Meter dynamicTyreRadius)
			{
				var engineSpeed = TruckMaxAllowedSpeed / dynamicTyreRadius * axleRatio * gear.Ratio;
				return engineSpeed;
			}

			internal static Point[] IntersectTakeHigherShiftLine(Point[] orig, Point[] transformedDownshift)
			{
				var intersections = Intersect(orig, transformedDownshift);

				// add all points (i.e. intersecting points and both line segments) to a single list
				var pointSet = new List<Point>(orig);
				pointSet.AddRange(transformedDownshift);
				pointSet.AddRange(intersections);
				pointSet.AddRange(ProjectPointsToLineSegments(orig, transformedDownshift));
				pointSet.AddRange(ProjectPointsToLineSegments(transformedDownshift, orig));

				// line sweeping from max_X to 0: select point with lowest Y coordinate, abort if a point has Y = 0
				var shiftPolygon = new List<Point>();
				foreach (var xCoord in pointSet.Select(pt => pt.X).Distinct().OrderBy(x => x).Reverse()) {
					var coord = xCoord;
					var xPoints = pointSet.Where(pt => pt.X.IsEqual(coord) && !pt.Y.IsEqual(0)).ToList();
					shiftPolygon.Add(xPoints.MinBy(pt => pt.Y));
					var tmp = pointSet.Where(pt => pt.X.IsEqual(coord)).Where(pt => pt.Y.IsEqual(0)).ToList();
					if (!tmp.Any()) {
						continue;
					}

					shiftPolygon.Add(tmp.First());
					break;
				}

				// find and remove colinear points
				var toRemove = new List<Point>();
				for (var i = 0; i < shiftPolygon.Count - 2; i++) {
					var edge = new Edge(shiftPolygon[i], shiftPolygon[i + 2]);
					if (edge.ContainsXY(shiftPolygon[i + 1])) {
						toRemove.Add(shiftPolygon[i + 1]);
					}
				}
				foreach (var point in toRemove) {
					shiftPolygon.Remove(point);
				}

				// order points first by x coordinate and the by Y coordinate ASC
				return shiftPolygon.OrderBy(pt => pt.X).ThenBy(pt => pt.Y).ToArray();
			}

			internal static Point[] IntersectTakeLowerShiftLine(Point[] upShiftPts, Point[] upperLimit)
			{
				var intersections = Intersect(upShiftPts, upperLimit);
				if (!intersections.Any()) {
					return upShiftPts[0].X < upperLimit[0].X ? upShiftPts : upperLimit;
				}

				var pointSet = new List<Point>(upShiftPts);
				pointSet.AddRange(upperLimit);
				pointSet.AddRange(intersections);
				pointSet.AddRange(ProjectPointsToLineSegments(upShiftPts, upperLimit, true));
				pointSet.AddRange(ProjectPointsToLineSegments(upperLimit, upShiftPts, true));

				var shiftPolygon = new List<Point>();
				foreach (var yCoord in pointSet.Select(pt => pt.Y).Distinct().OrderBy(y => y).Reverse()) {
					var yPoints = pointSet.Where(pt => pt.Y.IsEqual(yCoord)).ToList();
					shiftPolygon.Add(yPoints.MinBy(pt => pt.X));
				}

				// find and remove colinear points
				var toRemove = new List<Point>();
				for (var i = 0; i < shiftPolygon.Count - 2; i++) {
					var edge = new Edge(shiftPolygon[i], shiftPolygon[i + 2]);
					if (edge.ContainsXY(shiftPolygon[i + 1])) {
						toRemove.Add(shiftPolygon[i + 1]);
					}
				}
				foreach (var point in toRemove) {
					shiftPolygon.Remove(point);
				}

				// order points first by x coordinate and the by Y coordinate ASC
				return shiftPolygon.OrderBy(pt => pt.X).ThenBy(pt => pt.Y).ToArray();
			}

			private static Point[] Intersect(Point[] orig, Point[] transformedDownshift)
			{
				var intersections = new List<Point>();

				// compute all intersection points between both line segments
				// ReSharper disable once LoopCanBeConvertedToQuery
				foreach (var origLine in orig.Pairwise(Edge.Create)) {
					// ReSharper disable once LoopCanBeConvertedToQuery
					foreach (var transformedLine in transformedDownshift.Pairwise(Edge.Create)) {
						var isect = VectoMath.Intersect(origLine, transformedLine);
						if (isect != null) {
							intersections.Add(isect);
						}
					}
				}

				return intersections.ToArray();
			}

			private static IEnumerable<Point> ProjectPointsToLineSegments(
				IEnumerable<Point> lineSegments, Point[] points,
				bool projectToVertical = false)
			{
				var pointSet = new List<Point>();
				foreach (var segment in lineSegments.Pairwise(Edge.Create)) {
					if (segment.P1.X.IsEqual(segment.P2.X)) {
						if (projectToVertical) {
							pointSet.AddRange(
								points.Select(point => new Point(segment.P1.X, point.Y))
									.Where(pt => pt.Y.IsBetween(segment.P1.Y, segment.P2.Y)));
						}
						continue;
					}

					var k = segment.SlopeXY;
					var d = segment.P1.Y - segment.P1.X * k;
					pointSet.AddRange(points.Select(point => new Point(point.X, point.X * k + d)));
				}

				return pointSet;
			}
		}

		public static class TorqueConverter
		{
			public static readonly PerSecond ReferenceRPM = 1000.RPMtoRad();
			public static readonly PerSecond MaxInputSpeed = 5000.RPMtoRad();
			public static readonly MeterPerSquareSecond CLUpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();
			public static readonly MeterPerSquareSecond CCUpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();

			private static readonly PerSecond DownshiftPRM = 700.RPMtoRad();
			private static readonly PerSecond UpshiftLowRPM = 900.RPMtoRad();
			private static readonly PerSecond UpshiftHighRPM = 1150.RPMtoRad();

			public static ShiftPolygon ComputeShiftPolygon(
				EngineFullLoadCurve fullLoadCurve, bool first = false,
				bool last = false)
			{
				var maxDragTorque = fullLoadCurve.MaxDragTorque * 1.1;
				var maxTorque = fullLoadCurve.MaxTorque * 1.1;
				var p0 = new Point(UpshiftLowRPM.Value(), maxDragTorque.Value());
				var p1 = new Point(UpshiftLowRPM.Value(), 0);
				var p2 = new Point(UpshiftHighRPM.Value(), fullLoadCurve.FullLoadStationaryTorque(UpshiftHighRPM).Value());
				var edge = new Edge(p1, p2);
				var p2corr = new Point((maxTorque.Value() - edge.OffsetXY) / edge.SlopeXY, maxTorque.Value());

				var downshift = new[] {
					new ShiftPolygon.ShiftPolygonEntry(maxDragTorque, DownshiftPRM),
					new ShiftPolygon.ShiftPolygonEntry(maxTorque, DownshiftPRM)
				};
				var upshift = new[] { p0, p1, p2corr }.Select(
					pt => new ShiftPolygon.ShiftPolygonEntry(pt.Y.SI<NewtonMeter>(), pt.X.SI<PerSecond>()));

				return new ShiftPolygon(
					first ? new List<ShiftPolygon.ShiftPolygonEntry>() : downshift.ToList(),
					last ? new List<ShiftPolygon.ShiftPolygonEntry>() : upshift.ToList());
			}

			public static IEnumerable<TorqueConverterEntry> GetTorqueConverterDragCurve(double ratio, TorqueConverterEntry first, TorqueConverterEntry last)
			{
				var characteristicTorque = new[] {
					new TorqueConverterEntry() {
						SpeedRatio = 1,
						TorqueRatio = last.TorqueRatio * 0.99,
						Torque = 0.SI<NewtonMeter>()
					},
					new TorqueConverterEntry(
					) {
						SpeedRatio = 5,
						TorqueRatio = 0.9,
						Torque =  -4 * first.Torque
					},
				};
				foreach (var torqueConverterEntry in characteristicTorque) {
					torqueConverterEntry.SpeedRatio = torqueConverterEntry.SpeedRatio * ratio;
					torqueConverterEntry.TorqueRatio = torqueConverterEntry.TorqueRatio / ratio;
				}

				return characteristicTorque.Where(x => x.SpeedRatio >= ratio).ToArray();
			}
		}

		public static class PTO
		{
			public const string DefaultPTOTechnology =
				"only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel";

			public const string DefaultPTOIdleLosses =
				DeclarationDataResourcePrefix + ".MissionCycles.MunicipalUtility_PTO_generic.vptol";

			public const string DefaultPTOActivationCycle =
				DeclarationDataResourcePrefix + ".MissionCycles.MunicipalUtility_PTO_generic.vptoc";
		}

		public static class VTPMode
		{
			public static readonly Meter RunInThreshold = 15000.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
			public const double EvolutionCoefficient = 0.98;

			public const MissionType SelectedMissionHeavyLorry = MissionType.LongHaul;
			public const MissionType SelectedMissionMediumLorry = MissionType.RegionalDelivery;

			public const MissionType SelectedMissionLowFloorBus = MissionType.Urban;
			public const MissionType SelectedMissionHighFloorBus = MissionType.Coach;

			public const LoadingType SelectedLoading = LoadingType.ReferenceLoad;

			// verification of input data

			public const double WheelSpeedDifferenceFactor = 1.4;
			public const double WheelTorqueDifferenceFactor = 3;

			public static readonly PerSecond WheelSpeedZeroTolerance = 0.1.RPMtoRad();
			public static readonly PerSecond MaxWheelSpeedDifferenceStandstill = 1.RPMtoRad();

			public static readonly PerSecond MinFanSpeed = 20.RPMtoRad();
			public static readonly PerSecond MaxFanSpeed = 4000.RPMtoRad();

			public static readonly Second SamplingInterval = 0.5.SI<Second>();

			public static readonly WattSecond MinPosWorkAtWheelsForFC = 1.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();

			public static readonly SpecificFuelConsumption LowerFCThreshold =
				180.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<SpecificFuelConsumption>();

			public static readonly SpecificFuelConsumption UpperFCThreshold =
				600.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<SpecificFuelConsumption>();

			public static readonly Second FCAccumulationWindow = 10.SI(Unit.SI.Minute).Cast<Second>();
			public static readonly double[] FanParameters = { 7.320, 1200.0, 810 };
		}

		public static class Vehicle
		{
			public const bool DualFuelVehicleDefault = false;
			public const bool HybridElectricHDVDefault = false;
			public const bool ZeroEmissionVehicleDefault = false;
			public const TankSystem TankSystemDefault = TankSystem.Compressed;
			public const bool SleeperCabDefault = true;
			public const bool VocationalVehicleDefault = false;

			public static class ADAS
			{
				public const PredictiveCruiseControlType PredictiveCruiseControlDefault = PredictiveCruiseControlType.None;
				public const EcoRollType EcoRoll = EcoRollType.None;
				public const bool EngineStopStartDefault = false;
			}
		}
	}
}

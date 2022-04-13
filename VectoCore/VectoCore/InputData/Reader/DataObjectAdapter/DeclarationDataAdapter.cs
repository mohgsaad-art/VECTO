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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapter : AbstractSimulationDataAdapter
	{
		public static readonly GearboxType[] SupportedGearboxTypes = {GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial};

		public DriverData CreateDriverData()
		{
			var lookAheadData = new DriverData.LACData {
				Enabled = DeclarationData.Driver.LookAhead.Enabled,
				//Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
				MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed,
				LookAheadDecisionFactor = new LACDecisionFactor(),
				LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
			};
			var overspeedData = new DriverData.OverSpeedData {
				Enabled = true,
				MinSpeed = DeclarationData.Driver.OverSpeed.MinSpeed,
				OverSpeed = DeclarationData.Driver.OverSpeed.AllowedOverSpeed,
			};
			var retVal = new DriverData {
				LookAheadCoasting = lookAheadData,
				OverSpeed = overspeedData,
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay = DeclarationData.Driver.EngineStopStart.ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan,
					UtilityFactorStandstill = DeclarationData.Driver.EngineStopStart.UtilityFactor,
					UtilityFactorDriving = DeclarationData.Driver.EngineStopStart.UtilityFactor,
				},
				EcoRoll = new DriverData.EcoRollData() {
					UnderspeedThreshold = DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
					MinSpeed = DeclarationData.Driver.EcoRoll.MinSpeed,
					ActivationPhaseDuration = DeclarationData.Driver.EcoRoll.ActivationDelay,
					AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
					AccelerationUpperLimit = DeclarationData.Driver.EcoRoll.AccelerationUpperLimit,
				},
				PCC = new DriverData.PCCData() {
					PCCEnableSpeed = DeclarationData.Driver.PCC.PCCEnableSpeed,
					MinSpeed = DeclarationData.Driver.PCC.MinSpeed,
					PreviewDistanceUseCase1 = DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
					PreviewDistanceUseCase2 = DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
					UnderSpeed = DeclarationData.Driver.PCC.Underspeed,
					OverspeedUseCase3 = DeclarationData.Driver.PCC.OverspeedUseCase3
				}
			};
			return retVal;
		}

		internal VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Mission mission, Kilogram loading, bool allowVocational)
		{
			if (!data.SavedInDeclarationMode) {
				WarnDeclarationMode("VehicleData");
			}
			return data.ExemptedVehicle
				? CreateExemptedVehicleData(data)
				: CreateNonExemptedVehicleData(data, mission, loading, allowVocational);
		}

		private VehicleData CreateNonExemptedVehicleData(
			IVehicleDeclarationInputData data, Mission mission, Kilogram loading, bool allowVocational)
		{
			var retVal = SetCommonVehicleData(data);
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.AirDensity = DeclarationData.AirDensity;
			retVal.VIN = data.VIN;
			retVal.ManufacturerAddress = data.ManufacturerAddress;
			retVal.LegislativeClass = data.LegislativeClass;
			retVal.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			retVal.SleeperCab = data.SleeperCab;
			retVal.TrailerGrossVehicleMass = mission.Trailer.Sum(t => t.TrailerGrossVehicleMass).DefaultIfNull(0);

			retVal.BodyAndTrailerMass = mission.BodyCurbWeight + mission.Trailer.Sum(t => t.TrailerCurbMass).DefaultIfNull(0);

			retVal.Loading = loading;
			retVal.DynamicTyreRadius =
				data.Components.AxleWheels.AxlesDeclaration.Where(axle => axle.AxleType == AxleType.VehicleDriven)
					.Select(da => DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius)
					.Average();
			retVal.CargoVolume = mission.MissionType != MissionType.Construction ? mission.TotalCargoVolume : 0.SI<CubicMeter>();

			retVal.VocationalVehicle = allowVocational && data.VocationalVehicle;
			retVal.ADAS = CreateADAS(data.ADAS);

			var axles = data.Components.AxleWheels.AxlesDeclaration;
			if (axles.Count < mission.AxleWeightDistribution.Length) {
				throw new VectoException(
					"Vehicle does not contain sufficient axles. {0} axles defined, {1} axles required",
					axles.Count, mission.AxleWeightDistribution.Length);
			}

			var axleData = new List<Axle>();
			for (var i = 0; i < mission.AxleWeightDistribution.Length; i++) {
				var axleInput = axles[i];
				var axle = new Axle {
					WheelsDimension = axleInput.Tyre.Dimension,
					AxleType = axleInput.AxleType,
					AxleWeightShare = mission.AxleWeightDistribution[i],
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.Tyre.RollResistanceCoefficient,
					TyreTestLoad = axleInput.Tyre.TyreTestLoad,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Tyre.Dimension.RemoveWhitespace()).Inertia,
					CertificationNumber = axleInput.Tyre.CertificationNumber,
					DigestValueInput = axleInput.Tyre.DigestValue == null ? "" : axleInput.Tyre.DigestValue.DigestValue,
				};
				axleData.Add(axle);
			}

			foreach (var trailer in mission.Trailer) {
				axleData.AddRange(
					trailer.TrailerWheels.Select(
						trailerWheel => new Axle {
							AxleType = AxleType.Trailer,
							AxleWeightShare = trailer.TrailerAxleWeightShare / trailer.TrailerWheels.Count,
							TwinTyres = DeclarationData.Trailer.TwinTyres,
							RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
							TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
							Inertia = trailerWheel.Inertia,
							WheelsDimension = trailerWheel.WheelType
						}));
			}

			retVal.AxleData = axleData;
			return retVal;
		}

		private VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			var retVal = new VehicleData.ADASData {
				EngineStopStart = adas.EngineStopStart,
				EcoRoll = adas.EcoRoll,
				PredictiveCruiseControl = adas.PredictiveCruiseControl
			};

			if (retVal.PredictiveCruiseControl == PredictiveCruiseControlType.None &&
				retVal.EcoRoll.WithoutEngineStop()) {
				retVal.EcoRoll = EcoRollType.None;
			}

			return retVal;
		}

		private VehicleData CreateExemptedVehicleData(IVehicleDeclarationInputData data)
		{
			var exempted = SetCommonVehicleData(data);
			exempted.VIN = data.VIN;
			exempted.ManufacturerAddress = data.ManufacturerAddress;
			exempted.LegislativeClass = data.LegislativeClass;
			exempted.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			exempted.HybridElectricHDV = data.HybridElectricHDV;
			exempted.DualFuelVehicle = data.DualFuelVehicle;
			exempted.MaxNetPower1 = data.MaxNetPower1;
			exempted.MaxNetPower2 = data.MaxNetPower2;
			exempted.SleeperCab = data.SleeperCab;
			exempted.AxleConfiguration = data.AxleConfiguration;

			return exempted;
		}


		internal CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
		{
			var engine = vehicle.Components.EngineInputData;
			var gearbox = vehicle.Components.GearboxInputData;
			
			if (!engine.SavedInDeclarationMode) {
				WarnDeclarationMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine, vehicle.TankSystem);
			retVal.IdleSpeed = VectoMath.Max(mode.IdleSpeed, vehicle.EngineIdleSpeed);
			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in mode.Fuels) {
				retVal.Fuels.Add(
					new CombustionEngineFuelData() {
						WHTCUrban = fuel.WHTCUrban,
						WHTCRural = fuel.WHTCRural,
						WHTCMotorway = fuel.WHTCMotorway,
						ColdHotCorrectionFactor = fuel.ColdHotBalancingFactor,
						CorrectionFactorRegPer = fuel.CorrectionFactorRegPer,
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, vehicle.TankSystem),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
							mission.MissionType.GetNonEMSMissionType(), fuel.WHTCRural, fuel.WHTCUrban,
							fuel.WHTCMotorway) * fuel.ColdHotBalancingFactor * fuel.CorrectionFactorRegPer,
					});
			}
			retVal.Inertia = DeclarationData.Engine.EngineInertia(retVal.Displacement, gearbox.Type);
			retVal.EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime;
			var limits = vehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var numGears = gearbox.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(mode.FullLoadCurve, true);
			fullLoadCurves[0].EngineData = retVal;
			foreach (var gear in gearbox.Gears) {
				var maxTorque = VectoMath.Min(
					GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}
			retVal.FullLoadCurves = fullLoadCurves;
			return retVal;
		}

		private static NewtonMeter VehMaxTorque(ITransmissionInputData gear, int numGears,
			Dictionary<int, ITorqueLimitInputData> limits,
			NewtonMeter maxEngineTorque)
		{
			if (gear.Gear - 1 >= numGears / 2) {
				// only upper half of gears can limit if max-torque <= 0.95 of engine max torque
				if (limits.ContainsKey(gear.Gear) &&
					limits[gear.Gear].MaxTorque <= DeclarationData.Engine.TorqueLimitVehicleFactor * maxEngineTorque) {
					return limits[gear.Gear].MaxTorque;
				}
			}
			return null;
		}

		private static NewtonMeter GbxMaxTorque(ITransmissionInputData gear, int numGears, NewtonMeter maxEngineTorque
			)
		{
			if (gear.Gear - 1 < numGears / 2) {
				// gears count from 1 to n, -> n entries, lower half can always limit
				if (gear.MaxTorque != null) {
					return gear.MaxTorque;
				}
			} else {
				// upper half can only limit if max-torque <= 90% of engine max torque
				if (gear.MaxTorque != null && gear.MaxTorque <= DeclarationData.Engine.TorqueLimitGearboxFactor * maxEngineTorque) {
					return gear.MaxTorque;
				}
			}
			return null;
		}

		public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc)
		{
			if (!inputData.Components.GearboxInputData.SavedInDeclarationMode) {
				WarnDeclarationMode("GearboxData");
			}
			return DoCreateGearboxData(inputData, runData, shiftPolygonCalc);
		}

		public virtual GearboxData DoCreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc)
		{
			var gearbox = inputData.Components.GearboxInputData;

			var adas = inputData.ADAS;
			var torqueConverter = inputData.Components.TorqueConverterInputData;

			var engine = runData.EngineData;
			var axlegearRatio = runData.AxleGearData.AxleGear.Ratio;
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;
			var retVal = SetCommonGearboxData(gearbox);

			retVal.ATEcoRollReleaseLockupClutch = DeclarationData.GearboxTCU.Fallback_ATEcoRollReleaseLockupClutch;

			if (!SupportedGearboxTypes.Contains(gearbox.Type)) {
				throw new VectoSimulationException("Unsupported gearbox type: {0}!", retVal.Type);
			}
			var gearsInput = gearbox.Gears;
			if (gearsInput.Count < 1) {
				throw new VectoSimulationException(
					"At least one Gear-Entry must be defined in Gearbox!");
			}

			SetDeclarationData(retVal);

			var gearDifferenceRatio = gearbox.Type.AutomaticTransmission() && gearbox.Gears.Count > 2
				? gearbox.Gears[0].Ratio / gearbox.Gears[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();
			var tcShiftPolygon = DeclarationData.TorqueConverter.ComputeShiftPolygon(engine.FullLoadCurves[0]);
			var vehicleCategory = inputData.VehicleCategory;
			//var vehicleCategory = runData.VehicleData.VehicleCategory == VehicleCategory.GenericBusVehicle
			//	? VehicleCategory.GenericBusVehicle
			//	: inputData.VehicleCategory;
			for (uint i = 0; i < gearsInput.Count; i++) {
				var gear = gearsInput[(int)i];
				var lossMap = CreateGearLossMap(gear, i, false, vehicleCategory, gearbox.Type);

				var shiftPolygon = shiftPolygonCalc != null
					? shiftPolygonCalc.ComputeDeclarationShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[i + 1], gearbox.Gears, engine, axlegearRatio, dynamicTyreRadius)
					: DeclarationData.Gearbox.ComputeShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[i + 1],
						gearsInput, engine,
						axlegearRatio, dynamicTyreRadius);

				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				CreateATGearData(gearbox, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears, vehicleCategory);
				gears.Add(i + 1, gearData);
			}
			retVal.Gears = gears;
			if (retVal.Type.AutomaticTransmission()) {
				var ratio = double.IsNaN(retVal.Gears[1].Ratio) ? 1 : retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime;
				retVal.TorqueConverterData = TorqueConverterDataReader.Create(torqueConverter.TCData,
					DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
					ExecutionMode.Declaration, ratio,
					DeclarationData.TorqueConverter.CLUpshiftMinAcceleration, DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
				retVal.TorqueConverterData.ModelName = torqueConverter.Model;
				retVal.TorqueConverterData.DigestValueInput = torqueConverter.DigestValue?.DigestValue;
				retVal.TorqueConverterData.CertificationMethod = torqueConverter.CertificationMethod;
				retVal.TorqueConverterData.CertificationNumber = torqueConverter.CertificationNumber;
			}

			// update disengageWhenHaltingSpeed
			if (retVal.Type.AutomaticTransmission()) {
				var firstGear = retVal.GearList.First(x => x.IsLockedGear());
				if (retVal.Gears[firstGear.Gear].ShiftPolygon.Downshift.Any()) {
					var downshiftSpeedInc = retVal.Gears[firstGear.Gear].ShiftPolygon
						.InterpolateDownshiftSpeed(0.SI<NewtonMeter>()) * 1.05;
					var vehicleSpeedDisengage = downshiftSpeedInc / axlegearRatio / retVal.Gears[firstGear.Gear].Ratio *
												dynamicTyreRadius;
					retVal.DisengageWhenHaltingSpeed = vehicleSpeedDisengage;
				}
			}

			return retVal;
		}

		private static void CreateATGearData(IGearboxDeclarationInputData gearbox, uint i, GearData gearData,
			ShiftPolygon tcShiftPolygon, double gearDifferenceRatio, Dictionary<uint, GearData> gears,
			VehicleCategory vehicleCategory)
		{
			if (gearbox.Type == GearboxType.ATPowerSplit && i == 0) {
				// powersplit transmission: torque converter already contains ratio and losses
				CretateTCFirstGearATPowerSplit(gearData, i, tcShiftPolygon);
			}
			if (gearbox.Type == GearboxType.ATSerial) {
				if (i == 0) {
					// torqueconverter is active in first gear - duplicate ratio and lossmap for torque converter mode
					CreateTCFirstGearATSerial(gearData, tcShiftPolygon);
				}
				if (i == 1 && gearDifferenceRatio >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold(vehicleCategory)) {
					// ratio between first and second gear is above threshold, torqueconverter is active in second gear as well
					// -> duplicate ratio and lossmap for torque converter mode, remove locked transmission for previous gear
					CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					// NOTE: the lower gear in 'gears' dictionary has index i !!
					gears[i].Ratio = double.NaN;
					gears[i].LossMap = null;
				}
			}
		}

		private static void SetDeclarationData(GearboxData retVal)
		{
			retVal.Inertia = DeclarationData.Gearbox.Inertia;
			retVal.TractionInterruption = retVal.Type.TractionInterruption();
			
		}

		public AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			var retVal = SetCommonAxleGearData(data);
			retVal.AxleGear.LossMap = ReadAxleLossMap(data, false);
			return retVal;
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return DoCreateAngledriveData(data, false);
		}


		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
			MissionType mission, VehicleClass hvdClass)
		{
			if (!auxInputData.SavedInDeclarationMode) {
				WarnDeclarationMode("AuxiliariesData");
			}
			var retVal = new List<VectoRunData.AuxData>();

			if (auxInputData.Auxiliaries.Count != 5) {
				Log.Error(
					"In Declaration Mode exactly 5 Auxiliaries must be defined: Fan, Steering pump, HVAC, Electric System, Pneumatic System.");
				throw new VectoException(
					"In Declaration Mode exactly 5 Auxiliaries must be defined: Fan, Steering pump, HVAC, Electric System, Pneumatic System.");
			}

			foreach (var auxType in EnumHelper.GetValues<AuxiliaryType>()) {
				var auxData = auxInputData.Auxiliaries.FirstOrDefault(a => a.Type == auxType);
				if (auxData == null) {
					throw new VectoException("Auxiliary {0} not found.", auxType);
				}
				var aux = new VectoRunData.AuxData {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = auxData.Technology
				};

				mission = mission.GetNonEMSMissionType();
				switch (auxType) {
					case AuxiliaryType.Fan:
						aux.PowerDemand = DeclarationData.Fan.Lookup(mission, auxData.Technology.FirstOrDefault()).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.Fan;
						break;
					case AuxiliaryType.SteeringPump:
						aux.PowerDemand = DeclarationData.SteeringPump.Lookup(mission, hvdClass, auxData.Technology);
						aux.ID = Constants.Auxiliaries.IDs.SteeringPump;
						break;
					case AuxiliaryType.HVAC:
						aux.PowerDemand = DeclarationData.HeatingVentilationAirConditioning.Lookup(mission,
							auxData.Technology.FirstOrDefault(), hvdClass).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition;
						break;
					case AuxiliaryType.PneumaticSystem:
						aux.PowerDemand = DeclarationData.PneumaticSystem.Lookup(mission, auxData.Technology.FirstOrDefault()).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.PneumaticSystem;
						break;
					case AuxiliaryType.ElectricSystem:
						aux.PowerDemand = DeclarationData.ElectricSystem.Lookup(mission, auxData.Technology.FirstOrDefault()).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.ElectricSystem;
						break;
					default:
						continue;
				}
				retVal.Add(aux);
			}
			return retVal;
		}

		private void WarnDeclarationMode(string inputData)
		{
			Log.Warn("{0} not in Declaration Mode!", inputData);
		}

		public RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(
			string crosswindCorrectionParameters, SquareMeter aerodynamicDragAera, Meter vehicleHeight)
		{
			const int startSpeed = 60;
			const int maxSpeed = 130;
			const int speedStep = 5;

			const int maxAlpha = 180;
			const int alphaStep = 10;

			const int startHeightPercent = 5;
			const int maxHeightPercent = 100;
			const int heightPercentStep = 10;
			const double heightShare = (double)heightPercentStep / maxHeightPercent;

			var values = DeclarationData.AirDrag.Lookup(crosswindCorrectionParameters);

			// first entry (0m/s) will get CdxA of second entry.
			var points = new List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> {
				new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
					Velocity = 0.SI<MeterPerSecond>(),
					EffectiveCrossSectionArea = 0.SI<SquareMeter>()
				}
			};

			for (var speed = startSpeed; speed <= maxSpeed; speed += speedStep) {
				var vVeh = speed.KMPHtoMeterPerSecond();

				var cdASum = 0.SI<SquareMeter>();

				for (var heightPercent = startHeightPercent; heightPercent < maxHeightPercent; heightPercent += heightPercentStep) {
					var height = heightPercent / 100.0 * vehicleHeight;
					var vWind = Physics.BaseWindSpeed * Math.Pow(height / Physics.BaseWindHeight, Physics.HellmannExponent);

					for (var alpha = 0; alpha <= maxAlpha; alpha += alphaStep) {
						var vAirX = vVeh + vWind * Math.Cos(alpha.ToRadian());
						var vAirY = vWind * Math.Sin(alpha.ToRadian());

						var beta = Math.Atan(vAirY / vAirX).ToDegree();

						// ΔCdxA = A1β + A2β² + A3β³
						var deltaCdA = values.A1 * beta + values.A2 * beta * beta + values.A3 * beta * beta * beta;

						// CdxA(β) = CdxA(0) + ΔCdxA(β)
						var cdA = aerodynamicDragAera + deltaCdA;

						var share = (alpha == 0 || alpha == maxAlpha ? alphaStep / 2.0 : alphaStep) / maxAlpha;

						// v_air = sqrt(v_airX²+vAirY²)
						// cdASum = CdxA(β) * v_air²/v_veh²
						cdASum += heightShare * share * cdA * (vAirX * vAirX + vAirY * vAirY) / (vVeh * vVeh);
					}
				}
				points.Add(new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
					Velocity = vVeh,
					EffectiveCrossSectionArea = cdASum
				});
			}

			points[0].EffectiveCrossSectionArea = points[1].EffectiveCrossSectionArea;
			return points;
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType != "None") {
				return new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				};
			}

			return null;
		}

		public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragInputData, Mission mission,
			Segment segment)
		{
			if (airdragInputData == null || airdragInputData.AirDragArea == null) {
				return DefaultAirdragData(mission, segment);
			}

			var retVal = SetCommonAirdragData(airdragInputData);

			retVal.DeclaredAirdragArea = mission.MissionType == MissionType.Construction ? mission.DefaultCDxA : airdragInputData.AirDragArea;
			retVal.DeclaredAirdragAreaInput = airdragInputData.AirDragArea;

			var aerodynamicDragArea = retVal.DeclaredAirdragArea + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			retVal.CrossWindCorrectionCurve =
				new CrosswindCorrectionCdxALookup(aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(mission.CrossWindCorrectionParameters, aerodynamicDragArea, segment.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection);
			return retVal;
		}

		private AirdragData DefaultAirdragData(Mission mission, Segment segment)
		{
			var aerodynamicDragArea = mission.DefaultCDxA + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			return new AirdragData() {
				CertificationMethod = CertificationMethod.StandardValues,
				DeclaredAirdragArea = mission.DefaultCDxA,
				DeclaredAirdragAreaInput = mission.DefaultCDxA,

				CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(mission.CrossWindCorrectionParameters, aerodynamicDragArea, segment.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection)
			};
		}

		public virtual ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
		{
			var retVal = new ShiftStrategyParameters {
				TorqueReserve = gbx.Type == GearboxType.MT ? DeclarationData.GearboxTCU.TorqueReserveSMT: DeclarationData.GearboxTCU.TorqueReserveAMT,
				StartTorqueReserve = DeclarationData.GearboxTCU.TorqueReserveStart,
				TimeBetweenGearshifts = gbx.Type == GearboxType.MT ? DeclarationData.Gearbox.MinTimeBetweenGearshiftsMT : DeclarationData.Gearbox.MinTimeBetweenGearshifts,
				StartSpeed = gbx.Type == GearboxType.MT ? DeclarationData.Gearbox.StartSpeed : DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,

				StartVelocity = DeclarationData.GearboxTCU.StartSpeed,
				
				RatingFactorCurrentGear = gbx.Type.AutomaticTransmission()
					? DeclarationData.GearboxTCU.RatingFactorCurrentGearAPT
					: DeclarationData.GearboxTCU.RatingFactorCurrentGear,

				//--------------------
				RatioEarlyUpshiftFC = DeclarationData.GearboxTCU.RatioEarlyUpshiftFC / axleRatio,
				RatioEarlyDownshiftFC = DeclarationData.GearboxTCU.RatioEarlyDownshiftFC / axleRatio,
				AllowedGearRangeFC = gbx.Type.AutomaticTransmission()
					? (gbx.Gears.Count > DeclarationData.GearboxTCU.ATSkipGearsThreshold
						? DeclarationData.GearboxTCU.AllowedGearRangeFCAPTSkipGear
						: DeclarationData.GearboxTCU.AllowedGearRangeFCAPT)
					: DeclarationData.GearboxTCU.AllowedGearRangeFCAMT,
				VelocityDropFactor = DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = 0.RPMtoRad(),
				ATLookAheadTime = DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				ShiftSpeedsTCToLocked = DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
														.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
				EffshiftAccelerationFactorAT = DeclarationData.GearboxTCU.DownshiftMaxAccelerationAT,
			};

			return retVal;
		}

	}
}

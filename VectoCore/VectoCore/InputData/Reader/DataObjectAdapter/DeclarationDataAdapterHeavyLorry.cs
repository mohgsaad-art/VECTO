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
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	
	public class DeclarationDataAdapterHeavyLorry : AbstractSimulationDataAdapter, IDeclarationDataAdapter
	{
		public static readonly GearboxType[] SupportedGearboxTypes =
			{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

		public virtual DriverData CreateDriverData()
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

		public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			if (!data.SavedInDeclarationMode) {
				WarnDeclarationMode("VehicleData");
			}
			return data.ExemptedVehicle
				? CreateExemptedVehicleData(data)
				: CreateNonExemptedVehicleData(data, segment, mission, loading.Value.Item1, loading.Value.Item2, allowVocational);
		}

		protected virtual VehicleData CreateNonExemptedVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading, double? passengerCount, bool allowVocational)
		{
			var retVal = SetCommonVehicleData(data);
			retVal.LegislativeClass = data.LegislativeClass;
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.AirDensity = DeclarationData.AirDensity;
			retVal.VIN = data.VIN;
			retVal.ManufacturerAddress = data.ManufacturerAddress;
//			retVal.LegislativeClass = data.LegislativeClass;
			retVal.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			retVal.VehicleClass = segment.VehicleClass;
			retVal.SleeperCab = retVal.VehicleClass.IsMediumLorry() ? false : data.SleeperCab;
			retVal.TrailerGrossVehicleMass = mission.Trailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0);

			retVal.BodyAndTrailerMass =
				mission.BodyCurbWeight + mission.Trailer.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0);

			retVal.Loading = loading;
			retVal.PassengerCount = passengerCount;
			retVal.DynamicTyreRadius =
				data.Components.AxleWheels.AxlesDeclaration.Where(axle => axle.AxleType == AxleType.VehicleDriven)
					.Select(da => DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius)
					.Average();
			if (segment.VehicleClass.IsMediumLorry() && segment.VehicleClass.IsVan()) {
				retVal.CargoVolume = data.CargoVolume;
			} else {
				retVal.CargoVolume = mission.MissionType != MissionType.Construction
					? mission.TotalCargoVolume
					: 0.SI<CubicMeter>();
			}

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
					FuelEfficiencyClass = axleInput.Tyre.FuelEfficiencyClass,
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
							FuelEfficiencyClass = DeclarationData.Trailer.FuelEfficiencyClass,
							Inertia = trailerWheel.Inertia,
							WheelsDimension = trailerWheel.WheelType
						}));
			}

			retVal.AxleData = axleData;
			return retVal;
		}


		protected virtual VehicleData CreateExemptedVehicleData(IVehicleDeclarationInputData data)
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
			return exempted;
		}


		public virtual CombustionEngineData CreateEngineData(
			IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
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
			var limits = vehicle.TorqueLimits?.ToDictionary(e => e.Gear) ?? new Dictionary<int, ITorqueLimitInputData>();
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

			retVal.WHRType = engine.WHRType;
			if ((retVal.WHRType & WHRType.ElectricalOutput) != 0) {
				retVal.ElectricalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataElectrical, mission.MissionType, WHRType.ElectricalOutput);
			}
			if ((retVal.WHRType & WHRType.MechanicalOutputDrivetrain) != 0) {
				retVal.MechanicalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataMechanical, mission.MissionType, WHRType.MechanicalOutputDrivetrain);
			}
			
			return retVal;
		}

		protected static WHRData CreateWHRData(IWHRData whrInputData, MissionType missionType, WHRType type)
		{
			if (whrInputData == null || whrInputData.GeneratedPower == null) {
				throw new VectoException("Missing WHR Data");
			}

			var whr = new WHRData() {
				CFUrban = whrInputData.UrbanCorrectionFactor,
				CFRural = whrInputData.RuralCorrectionFactor,
				CFMotorway = whrInputData.MotorwayCorrectionFactor,
				CFColdHot = whrInputData.BFColdHot,
				CFRegPer = whrInputData.CFRegPer,
				WHRMap = WHRPowerReader.Create(whrInputData.GeneratedPower, type)
			};
			whr.WHRCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
										missionType.GetNonEMSMissionType(), whr.CFRural, whr.CFUrban,
										whr.CFMotorway) * whr.CFColdHot * whr.CFRegPer;
			return whr;
		}

		protected internal static NewtonMeter VehMaxTorque(
			ITransmissionInputData gear, int numGears,
			Dictionary<int, ITorqueLimitInputData> limits,
			NewtonMeter maxEngineTorque)
		{
			if (gear.Gear - 1 >= numGears / 2) {
				// only upper half of gears can limit if max-torque <= 0.95 of engine max torque
				if (limits.TryGetValue(gear.Gear, out var limit) && limit.MaxTorque <= DeclarationData.Engine.TorqueLimitVehicleFactor * maxEngineTorque) {
					return limit.MaxTorque;
				}
			}

			return null;
		}

		protected internal static NewtonMeter GbxMaxTorque(
			ITransmissionInputData gear, int numGears, NewtonMeter maxEngineTorque
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

			//if (adas != null && retVal.Type.AutomaticTransmission() && adas.EcoRoll != EcoRollType.None &&
			//	!adas.ATEcoRollReleaseLockupClutch.HasValue) {
			//	throw new VectoException("Input parameter ATEcoRollReleaseLockupClutch required for AT transmission");
			//}

			retVal.ATEcoRollReleaseLockupClutch =
				adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission()
					? (adas.ATEcoRollReleaseLockupClutch.HasValue ? adas.ATEcoRollReleaseLockupClutch.Value : false)
					: false;

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
			var vehicleCategory = runData.VehicleData.VehicleCategory == VehicleCategory.GenericBusVehicle
				? VehicleCategory.GenericBusVehicle
				: inputData.VehicleCategory;
			for (uint i = 0; i < gearsInput.Count; i++) {
				var gear = gearsInput[(int)i];
				var lossMap = CreateGearLossMap(gear, i, false, vehicleCategory, gearbox.Type);

				var shiftPolygon = shiftPolygonCalc != null
					? shiftPolygonCalc.ComputeDeclarationShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[i + 1], gearbox.Gears, engine, axlegearRatio, dynamicTyreRadius)
					: DeclarationData.Gearbox.ComputeShiftPolygon(
						gearbox.Type, (int)i, engine.FullLoadCurves[i + 1],
						gearsInput, engine,
						axlegearRatio, dynamicTyreRadius, null);

				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					MaxTorque = gear.MaxTorque,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				CreateATGearData(gearbox, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears, runData.VehicleData.VehicleCategory);
				gears.Add(i + 1, gearData);
			}

			// remove disabled gears (only the last or last two gears may be removed)
			if (inputData.TorqueLimits != null) {
				var toRemove = (from tqLimit in inputData.TorqueLimits where tqLimit.Gear >= gears.Keys.Max() - 1 && tqLimit.MaxTorque.IsEqual(0) select (uint)tqLimit.Gear).ToList();
				if (toRemove.Count > 0 && toRemove.Min() <= gears.Count - toRemove.Count) {
					throw new VectoException("Only the last 1 or 2 gears can be disabled. Disabling gear {0} for a {1}-speed gearbox is not allowed.", toRemove.Min(), gears.Count);
				}

				foreach (var entry in toRemove) {
					gears.Remove(entry);
				}
			}

			retVal.Gears = gears;
			if (retVal.Type.AutomaticTransmission()) {
				var ratio = double.IsNaN(retVal.Gears[1].Ratio) ? 1 : retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime;

				retVal.TorqueConverterData = CreateTorqueConverterData(gearbox.Type, torqueConverter, ratio, engine);
				
				if (torqueConverter != null) {
					retVal.TorqueConverterData.Manufacturer = torqueConverter.Manufacturer;
					retVal.TorqueConverterData.ModelName = torqueConverter.Model;
					retVal.TorqueConverterData.DigestValueInput = torqueConverter.DigestValue?.DigestValue;
					retVal.TorqueConverterData.CertificationMethod = torqueConverter.CertificationMethod;
					retVal.TorqueConverterData.CertificationNumber = torqueConverter.CertificationNumber;
					retVal.TorqueConverterData.Date = torqueConverter.Date;
					retVal.TorqueConverterData.AppVersion = torqueConverter.AppVersion;
				}
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

		protected virtual TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio,
			CombustionEngineData componentsEngineInputData)
		{
			return TorqueConverterDataReader.Create(
				torqueConverter.TCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratio,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}

		protected virtual void CreateATGearData(
			IGearboxDeclarationInputData gearbox, uint i, GearData gearData,
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

		public virtual AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			var retVal = SetCommonAxleGearData(data);
			retVal.AxleGear.LossMap = ReadAxleLossMap(data, false);
			return retVal;
		}

		public virtual AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return DoCreateAngledriveData(data, false);
		}


		public virtual IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData, 
			IBusAuxiliariesDeclarationData busAuxData, MissionType mission, VehicleClass hvdClass, Meter vehicleLength, int? numSteeredAxles)
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
						aux.PowerDemand = DeclarationData.Fan.LookupPowerDemand(hvdClass, mission, auxData.Technology.FirstOrDefault());
						aux.ID = Constants.Auxiliaries.IDs.Fan;
						break;
					case AuxiliaryType.SteeringPump:
						if (numSteeredAxles.HasValue && auxData.Technology.Count != numSteeredAxles.Value) {
							throw new VectoException($"Number of steering pump technologies does not match number of steered axles ({numSteeredAxles.Value}, {auxData.Technology.Count})");
						}
						aux.PowerDemand = DeclarationData.SteeringPump.Lookup(mission, hvdClass, auxData.Technology);
						aux.ID = Constants.Auxiliaries.IDs.SteeringPump;
						break;
					case AuxiliaryType.HVAC:
						aux.PowerDemand = DeclarationData.HeatingVentilationAirConditioning.Lookup(
							mission,
							auxData.Technology.FirstOrDefault(), hvdClass).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition;
						break;
					case AuxiliaryType.PneumaticSystem:
						aux.PowerDemand = DeclarationData.PneumaticSystem.Lookup(mission, auxData.Technology.FirstOrDefault())
														.PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.PneumaticSystem;
						break;
					case AuxiliaryType.ElectricSystem:
						aux.PowerDemand = DeclarationData.ElectricSystem.Lookup(mission, auxData.Technology.FirstOrDefault()).PowerDemand;
						aux.ID = Constants.Auxiliaries.IDs.ElectricSystem;
						break;
					default: continue;
				}

				retVal.Add(aux);
			}

			return retVal;
		}

		public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
		{
			if (double.IsNaN(gbxData.AxlegearRatio)) {
				throw new VectoException("Axlegear ratio required in gearbox data for gearboxes with included axlegear");
			}
			return new AxleGearData() {
				AxleGear = new TransmissionData() {
					Ratio = gbxData.AxlegearRatio,
					LossMap = TransmissionLossMapReader.Create(1.0, gbxData.AxlegearRatio, "Axlegear")
				}
			};
		}

		protected void WarnDeclarationMode(string inputData)
		{
			Log.Warn("{0} not in Declaration Mode!", inputData);
		}

		public virtual RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(
			string crosswindCorrectionParameters, SquareMeter aerodynamicDragAera, Meter vehicleHeight)
		{
			 var startSpeed = Constants.SimulationSettings.CrosswindCorrection.MinVehicleSpeed;
			 var maxSpeed = Constants.SimulationSettings.CrosswindCorrection.MaxVehicleSpeed;
			 var speedStep = Constants.SimulationSettings.CrosswindCorrection.VehicleSpeedStep;

			 var maxAlpha = Constants.SimulationSettings.CrosswindCorrection.MaxAlpha;
			 var alphaStep = Constants.SimulationSettings.CrosswindCorrection.AlphaStep;

			 var startHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MinHeight;
			 var maxHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MaxHeight;
			 var heightPercentStep = Constants.SimulationSettings.CrosswindCorrection.HeightStep;
			 var heightShare = (double)heightPercentStep / maxHeightPercent;

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

				points.Add(
					new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
						Velocity = vVeh,
						EffectiveCrossSectionArea = cdASum
					});
			}

			points[0].EffectiveCrossSectionArea = points[1].EffectiveCrossSectionArea;
			return points;
		}

		public virtual PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto != null && pto.PTOTransmissionType != "None") {
				return new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				};
			}

			return null;
		}

		public virtual AirdragData CreateAirdragData(
			IAirdragDeclarationInputData airdragInputData, Mission mission,
			Segment segment)
		{
			if (airdragInputData == null || airdragInputData.AirDragArea == null) {
				return DefaultAirdragData(mission);
			}

			var retVal = SetCommonAirdragData(airdragInputData);
			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;
			retVal.DeclaredAirdragArea = mission.MissionType == MissionType.Construction
				? mission.DefaultCDxA
				: airdragInputData.AirDragArea;

			var aerodynamicDragArea = retVal.DeclaredAirdragArea + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			retVal.CrossWindCorrectionCurve =
				new CrosswindCorrectionCdxALookup(
					aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(
						mission.CrossWindCorrectionParameters, aerodynamicDragArea, mission.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection);
			return retVal;
		}

		protected virtual AirdragData DefaultAirdragData(Mission mission)
		{
			var aerodynamicDragArea = mission.DefaultCDxA + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			return new AirdragData() {
				CertificationMethod = CertificationMethod.StandardValues,
				CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection,
				DeclaredAirdragArea = mission.DefaultCDxA,
				CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
					aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(
						mission.CrossWindCorrectionParameters, aerodynamicDragArea, mission.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection)
			};
		}

		public virtual ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
		{
			var retVal = new ShiftStrategyParameters {
				TorqueReserve = DeclarationData.GearboxTCU.TorqueReserve,
				StartTorqueReserve = DeclarationData.GearboxTCU.TorqueReserveStart,
				TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
				StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,

				StartVelocity = DeclarationData.GearboxTCU.StartSpeed,
				GearResidenceTime = DeclarationData.GearboxTCU.GearResidenceTime,
				DnT99L_highMin1 = DeclarationData.GearboxTCU.DnT99L_highMin1,
				DnT99L_highMin2 = DeclarationData.GearboxTCU.DnT99L_highMin2,
				AllowedGearRangeUp = DeclarationData.GearboxTCU.AllowedGearRangeUp,
				AllowedGearRangeDown = DeclarationData.GearboxTCU.AllowedGearRangeDown,
				LookBackInterval = DeclarationData.GearboxTCU.LookBackInterval,
				DriverAccelerationLookBackInterval = DeclarationData.GearboxTCU.DriverAccelerationLookBackInterval,
				DriverAccelerationThresholdLow = DeclarationData.GearboxTCU.DriverAccelerationThresholdLow,
				AverageCardanPowerThresholdPropulsion = DeclarationData.GearboxTCU.AverageCardanPowerThresholdPropulsion,
				CurrentCardanPowerThresholdPropulsion = DeclarationData.GearboxTCU.CurrentCardanPowerThresholdPropulsion,
				TargetSpeedDeviationFactor = DeclarationData.GearboxTCU.TargetSpeedDeviationFactor,
				EngineSpeedHighDriveOffFactor = DeclarationData.GearboxTCU.EngineSpeedHighDriveOffFactor,
				RatingFactorCurrentGear = gbx.Type.AutomaticTransmission()
					? DeclarationData.GearboxTCU.RatingFactorCurrentGearAT
					: DeclarationData.GearboxTCU.RatingFactorCurrentGear,
				AccelerationReserveLookup = AccelerationReserveLookupReader.ReadFromStream(
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.AccelerationReserveLookup.csv")),
				ShareTorque99L = ShareTorque99lLookupReader.ReadFromStream(
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareTq99L.csv")
				),
				PredictionDurationLookup = PredictionDurationLookupReader.ReadFromStream(
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.PredictionTimeLookup.csv")
				),
				ShareIdleLow = ShareIdleLowReader.ReadFromStream(
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareIdleLow.csv")
				),
				ShareEngineHigh = EngineSpeedHighLookupReader.ReadFromStream(
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareEngineSpeedHigh.csv")
				),

				//--------------------
				RatioEarlyUpshiftFC = DeclarationData.GearboxTCU.RatioEarlyUpshiftFC / axleRatio,
				RatioEarlyDownshiftFC = DeclarationData.GearboxTCU.RatioEarlyDownshiftFC / axleRatio,
				AllowedGearRangeFC = gbx.Type.AutomaticTransmission()
					? (gbx.Gears.Count > DeclarationData.GearboxTCU.ATSkipGearsThreshold
						? DeclarationData.GearboxTCU.AllowedGearRangeFCATSkipGear
						: DeclarationData.GearboxTCU.AllowedGearRangeFCAT)
					: DeclarationData.GearboxTCU.AllowedGearRangeFCAMT,
				VelocityDropFactor = DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = 0.RPMtoRad(),
				ATLookAheadTime = DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				ShiftSpeedsTCToLocked = DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
														.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
			};

			return retVal;
		}


	}
}

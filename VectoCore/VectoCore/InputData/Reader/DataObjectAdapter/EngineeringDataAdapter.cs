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
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class EngineeringDataAdapter : AbstractSimulationDataAdapter
	{
		internal VehicleData CreateVehicleData(IVehicleEngineeringInputData data)
		{
			if (data.SavedInDeclarationMode) {
				WarnEngineeringMode("VehicleData");
			}
			var retVal = SetCommonVehicleData(data);
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.BodyAndTrailerMass = data.CurbMassExtra;

			//retVal.CurbWeight += data.CurbMassExtra;
			retVal.TrailerGrossVehicleMass = 0.SI<Kilogram>();
			retVal.Loading = data.Loading;
			retVal.DynamicTyreRadius = data.DynamicTyreRadius;
			retVal.ADAS = CreateADAS(data.ADAS);

			var axles = data.Components.AxleWheels.AxlesEngineering;

			retVal.AxleData = axles.Select(
				axle => new Axle {
					WheelsDimension = axle.Tyre.Dimension,
					Inertia = axle.Tyre.Inertia,
					TwinTyres = axle.TwinTyres,
					RollResistanceCoefficient = axle.Tyre.RollResistanceCoefficient,
					AxleWeightShare = axle.AxleWeightShare,
					TyreTestLoad = axle.Tyre.TyreTestLoad,
					FuelEfficiencyClass = axle.Tyre.FuelEfficiencyClass,
					AxleType = axle.AxleType,

					//Wheels = axle.WheelsStr
				}).ToList();
			return retVal;
		}

		private VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemsEngineering adas)
		{
			return adas == null ?
				new VehicleData.ADASData() {
					EngineStopStart = false,
					EcoRoll = EcoRollType.None,
					PredictiveCruiseControl = PredictiveCruiseControlType.None
				}: 
				new VehicleData.ADASData {
					EngineStopStart = adas.EngineStopStart,
					EcoRoll = adas.EcoRoll,
					PredictiveCruiseControl = adas.PredictiveCruiseControl
			};
		}


		public AirdragData CreateAirdragData(IAirdragEngineeringInputData airdragData, IVehicleEngineeringInputData data)
		{
			var retVal = SetCommonAirdragData(airdragData);
			retVal.CrossWindCorrectionMode = airdragData.CrossWindCorrectionMode;

			switch (airdragData.CrossWindCorrectionMode) {
				case CrossWindCorrectionMode.NoCorrection:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(airdragData.AirDragArea),
						CrossWindCorrectionMode.NoCorrection);
					break;
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurve(
							airdragData.CrosswindCorrectionMap,
							airdragData.AirDragArea), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
					break;
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionVAirBeta(
						airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.ReadCdxABetaTable(airdragData.CrosswindCorrectionMap));
					break;
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					var airDragArea = airdragData.AirDragArea ??
									DeclarationData.TruckSegments.LookupCdA(
										data.VehicleCategory, data.AxleConfiguration, data.GrossVehicleMassRating, false);
					var height = data.Height ?? (data.VehicleCategory.IsLorry()
									? DeclarationData.TruckSegments.LookupHeight(
										data.VehicleCategory, data.AxleConfiguration,
										data.GrossVehicleMassRating, false)
									: 4.SI<Meter>());
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						airDragArea,
						DeclarationDataAdapterHeavyLorry.GetDeclarationAirResistanceCurve(
							GetAirdragParameterSet(
								data.VehicleCategory, data.AxleConfiguration, data.Components.AxleWheels.AxlesEngineering.Count, data.GrossVehicleMassRating), airDragArea,
							height),
						CrossWindCorrectionMode.DeclarationModeCorrection);
					break;
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrection", airdragData.CrossWindCorrectionMode.ToString());
			}

			return retVal;
		}

		private string GetAirdragParameterSet(VehicleCategory vehicleCategory, AxleConfiguration axles, int numAxles,
			Kilogram tpmlm)
		{
			//TODO: check if is i
			switch (vehicleCategory) {
				case VehicleCategory.Van:
					return "MediumLorriesVan";
				case VehicleCategory.RigidTruck:
					if (tpmlm.IsSmallerOrEqual(7400)) {
						return "MediumLorriesRigid";
					}
					return numAxles > axles.NumAxles() ? "RigidTrailer" : "RigidSolo";
				case VehicleCategory.Tractor: return "TractorSemitrailer";
				case VehicleCategory.CityBus:
				//case VehicleCategory.InterurbanBus:
				case VehicleCategory.Coach: return "CoachBus";
				default: throw new ArgumentOutOfRangeException("vehicleCategory", vehicleCategory, null);
			}
		}

		private void WarnEngineeringMode(string msg)
		{
			Log.Error("{0} is in Declaration Mode but is used for Engineering Mode!", msg);
		}

		internal CombustionEngineData CreateEngineData(
			IVehicleEngineeringInputData vehicle, IEngineModeEngineeringInputData engineMode)
		{
			var engine = vehicle.Components.EngineInputData;
			var gbx = vehicle.Components.GearboxInputData;
			var torqueLimits = vehicle.TorqueLimits;
			var torqueConverter = vehicle.Components.TorqueConverterInputData;
			var tankSystem = vehicle.TankSystem;
			
			if (engine.SavedInDeclarationMode) {
				WarnEngineeringMode("EngineData");
			}
			
			var retVal = SetCommonCombustionEngineData(engine, tankSystem);
			retVal.IdleSpeed = VectoMath.Max(engineMode.IdleSpeed, vehicle.EngineIdleSpeed);
			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in engineMode.Fuels) {
				retVal.Fuels.Add(
					new CombustionEngineFuelData() {
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, tankSystem),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = fuel.WHTCEngineering,
					});
			}

			retVal.Inertia = engine.Inertia +
							(gbx != null && gbx.Type.AutomaticTransmission()
								? (gbx.Type == GearboxType.APTN || gbx.Type == GearboxType.IHPC ? 0.SI<KilogramSquareMeter>() : torqueConverter.Inertia)
								: 0.SI<KilogramSquareMeter>());
			retVal.EngineStartTime = engine.EngineStartTime ?? DeclarationData.Engine.DefaultEngineStartTime;
			var limits = torqueLimits.ToDictionary(e => e.Gear);
			var numGears = gbx?.Gears.Count ?? 0;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve);
			fullLoadCurves[0].EngineData = retVal;
			if (gbx != null) {
				foreach (var gear in gbx.Gears) {
					var maxTorque = VectoMath.Min(gear.MaxTorque, limits.GetVECTOValueOrDefault(gear.Gear)?.MaxTorque);
					fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
				}
			}

			retVal.FullLoadCurves = fullLoadCurves;

				
			retVal.WHRType = engine.WHRType;
			if ((retVal.WHRType & WHRType.ElectricalOutput) != 0) {
				retVal.ElectricalWHR = CreateWHRData(
					engineMode.WasteHeatRecoveryDataElectrical, WHRType.ElectricalOutput);
			}
			if ((retVal.WHRType & WHRType.MechanicalOutputDrivetrain) != 0) {
				retVal.MechanicalWHR = CreateWHRData(
					engineMode.WasteHeatRecoveryDataMechanical, WHRType.MechanicalOutputDrivetrain);
			}

			return retVal;
		}

		private WHRData CreateWHRData(IWHRData whrInputData, WHRType whrType)
		{
			if (whrInputData == null || whrInputData.GeneratedPower == null) {
				return null;
			}

			return new WHRData() {
				CFUrban = 1,
				CFRural = 1,
				CFMotorway = 1,
				CFColdHot = 1,
				CFRegPer = 1,
				WHRMap = WHRPowerReader.Create(whrInputData.GeneratedPower, whrType),
				WHRCorrectionFactor = whrInputData.EngineeringCorrectionFactor,
			};
		}


		internal CombustionEngineData CreateEngineData(IEngineEngineeringInputData engine, IEngineModeEngineeringInputData engineMode)
		{
			if (engine.SavedInDeclarationMode) {
				WarnEngineeringMode("EngineData");
			}
			var retVal = SetCommonCombustionEngineData(engine, null);
			retVal.IdleSpeed = engineMode.IdleSpeed;
			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in engineMode.Fuels) {
				retVal.Fuels.Add(
					new CombustionEngineFuelData() {
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, null),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = fuel.WHTCEngineering,
					});
			}

			retVal.Inertia = engine.Inertia;
			retVal.EngineStartTime = engine.EngineStartTime ?? DeclarationData.Engine.DefaultEngineStartTime;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
			fullLoadCurves[0] = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve);
			retVal.FullLoadCurves = fullLoadCurves;
			return retVal;
		}


		internal GearboxData CreateGearboxData(IEngineeringInputDataProvider inputData, VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalc)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			var gearbox = vehicle.Components.GearboxInputData;
			var torqueConverter = vehicle.Components.TorqueConverterInputData;

			var adas = vehicle.ADAS;
			var gearshiftData = inputData.DriverInputData.GearshiftInputData;

			var engineData = runData.EngineData;
			var axlegearRatio = runData.AxleGearData.AxleGear.Ratio;
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;
			var vehicleCategory = runData.VehicleData.VehicleCategory;

			if (gearbox.SavedInDeclarationMode) {
				WarnEngineeringMode("GearboxData");
			}

			var retVal = SetCommonGearboxData(gearbox);

			if (adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission() && !adas.ATEcoRollReleaseLockupClutch.HasValue) {
				throw new VectoException("Parameter ATEcoRollReleaseLockupClutch required for AT gearbox");
			}

			if ((vehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle || vehicle.VehicleType == VectoSimulationJobType.SerialHybridVehicle)&&
				gearbox.Type.AutomaticTransmission()) {
				// PEV with APT-S or APT-P transmission are simulated as APT-N
				retVal.Type = GearboxType.APTN;
			}
			retVal.ATEcoRollReleaseLockupClutch = adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission() ? adas.ATEcoRollReleaseLockupClutch.Value : false;

			if (retVal.Type == GearboxType.IEPC) {
				if (gearbox.Gears.Count > 0) {
					throw new VectoSimulationException("No gears are allowed for IEPC gearbox.");
				}

				retVal.Inertia = 0.SI<KilogramSquareMeter>();
				retVal.TractionInterruption = 0.SI<Second>();
				return retVal;
			}

			//var gears = gearbox.Gears;
			if (gearbox.Gears.Count < 2) {
				throw new VectoSimulationException("At least two Gear-Entries must be defined in Gearbox!");
			}

			SetEngineeringData(gearbox, gearshiftData, retVal);

			var hasTorqueConverter = retVal.Type.AutomaticTransmission() && retVal.Type != GearboxType.APTN && retVal.Type != GearboxType.IHPC;

			var gearDifferenceRatio = hasTorqueConverter && gearbox.Gears.Count > 2
				? gearbox.Gears[0].Ratio / gearbox.Gears[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();
			ShiftPolygon tcShiftPolygon = null;
			if (hasTorqueConverter) {
				tcShiftPolygon = torqueConverter.ShiftPolygon != null
					? ShiftPolygonReader.Create(torqueConverter.ShiftPolygon)
					: DeclarationData.TorqueConverter.ComputeShiftPolygon(engineData.FullLoadCurves[0]);
			}
			for (uint i = 0; i < gearbox.Gears.Count; i++) {
				var gear = gearbox.Gears[(int)i];
				var lossMap = CreateGearLossMap(gear, i, true, VehicleCategory.Unknown, gearbox.Type);

				ShiftPolygon shiftPolygon;
				if (gear.ShiftPolygon != null && gear.ShiftPolygon.SourceType != DataSourceType.Missing) {
					shiftPolygon = ShiftPolygonReader.Create(gear.ShiftPolygon);
				} else if (shiftPolygonCalc != null) {
					shiftPolygon = shiftPolygonCalc.ComputeDeclarationShiftPolygon(gearbox.Type, (int)i,
						engineData?.FullLoadCurves[i + 1], gearbox.Gears, engineData, axlegearRatio,
						dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
				} else {
					shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(gearbox.Type, (int)i,
						engineData?.FullLoadCurves[i + 1], gearbox.Gears, engineData, axlegearRatio,
						dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
				}

				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					MaxTorque = gear.MaxTorque,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				CreateATGearData(retVal.Type, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears, vehicleCategory, runData.Cycle);
				gears.Add(i + 1, gearData);
			}

			retVal.Gears = gears;

			if (hasTorqueConverter) {
				var ratio = double.IsNaN(retVal.Gears[1].Ratio) ? 1 : retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = gearbox.PowershiftShiftTime;
				retVal.TorqueConverterData = TorqueConverterDataReader.Create(
					torqueConverter.TCData,
					torqueConverter.ReferenceRPM, torqueConverter.MaxInputSpeed, ExecutionMode.Engineering, ratio,
					gearshiftData.CLUpshiftMinAcceleration, gearshiftData.CCUpshiftMinAcceleration);
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

		protected virtual void CreateATGearData(
			GearboxType gearboxType, uint i, GearData gearData,
			ShiftPolygon tcShiftPolygon, double gearDifferenceRatio, Dictionary<uint, GearData> gears,
			VehicleCategory vehicleCategory, IDrivingCycleData cycle)
		{
			if (gearboxType == GearboxType.ATPowerSplit && i == 0) {
				// powersplit transmission: torque converter already contains ratio and losses
				CretateTCFirstGearATPowerSplit(gearData, i, tcShiftPolygon);
			}
			if (gearboxType == GearboxType.ATSerial) {
				if (i == 0) {
					// torqueconverter is active in first gear - duplicate ratio and lossmap for torque converter mode
					CreateTCFirstGearATSerial(gearData, tcShiftPolygon);
				}
				if (i == 1) {
					if ((cycle != null) && ((cycle.CycleType == CycleType.MeasuredSpeedGear) || (cycle.CycleType == CycleType.VTP))) {
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					}	
					else if (gearDifferenceRatio >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold(vehicleCategory)) {
						// ratio between first and second gear is above threshold, torqueconverter is active in second gear as well
						// -> duplicate ratio and lossmap for torque converter mode, remove locked transmission for previous gear
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);

						// NOTE: the lower gear in 'gears' dictionary has index i !!
						gears[i].Ratio = double.NaN;
						gears[i].LossMap = null;
					}
				}
				else {
					if ((cycle != null) && ((cycle.CycleType == CycleType.MeasuredSpeedGear) || (cycle.CycleType == CycleType.VTP))) {
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					}
				}
			}
		}

		private static void SetEngineeringData(
			IGearboxEngineeringInputData gearbox, IGearshiftEngineeringInputData gearshiftData, GearboxData retVal)
		{
			retVal.Inertia = gearbox.Type.ManualTransmission() ? gearbox.Inertia : 0.SI<KilogramSquareMeter>();
			retVal.TractionInterruption = gearbox.Type == GearboxType.APTN || gearbox.Type == GearboxType.IHPC ? 0.SI<Second>() : gearbox.TractionInterruption;
		}

		public AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			var retVal = SetCommonAxleGearData(data);
			retVal.AxleGear.LossMap = ReadAxleLossMap(data, true);
			return retVal;
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return DoCreateAngledriveData(data, true);
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesEngineeringInputData auxInputData) {
			if (auxInputData.Auxiliaries is null)
				throw new VectoException("Missing Power Demands for ICE Off Driving, ICE Off Standstill, and Base Demand");

			var pwrICEOn = auxInputData.Auxiliaries.ConstantPowerDemand;
			var pwrICEOffDriving = auxInputData.Auxiliaries.PowerDemandICEOffDriving;
			var pwrICEOffStandstill = auxInputData.Auxiliaries.PowerDemandICEOffStandstill;

			var baseDemand = pwrICEOffStandstill;
			var stpDemand = pwrICEOffDriving - pwrICEOffStandstill;
			var fanDemand = pwrICEOn - pwrICEOffDriving;

			var auxList = new List<VectoRunData.AuxData>() {
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENG_AUX_MECH_BASE, DemandType = AuxiliaryDemandType.Constant, PowerDemand = baseDemand},
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENG_AUX_MECH_STP, DemandType = AuxiliaryDemandType.Constant, PowerDemand = stpDemand},
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENG_AUX_MECH_FAN, DemandType = AuxiliaryDemandType.Constant, PowerDemand = fanDemand},
			};

			return auxList;
		}

		internal DriverData CreateDriverData(IDriverEngineeringInputData driver)
		{
			if (driver.SavedInDeclarationMode) {
				WarnEngineeringMode("DriverData");
			}

			AccelerationCurveData accelerationData = null;
			if (driver.AccelerationCurve != null) {
				accelerationData = AccelerationCurveReader.Create(driver.AccelerationCurve.AccelerationCurve);
			}

			if (driver.Lookahead == null) {
				throw new VectoSimulationException("Error: Lookahead Data is missing.");
			}

			var lookAheadData = new DriverData.LACData {
				Enabled = driver.Lookahead.Enabled,

				//Deceleration = driver.Lookahead.Deceleration,
				MinSpeed = driver.Lookahead.MinSpeed,
				LookAheadDecisionFactor =
					new LACDecisionFactor(
						driver.Lookahead.CoastingDecisionFactorOffset, driver.Lookahead.CoastingDecisionFactorScaling,
						driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup,
						driver.Lookahead.CoastingDecisionFactorVelocityDropLookup),
				LookAheadDistanceFactor = driver.Lookahead.LookaheadDistanceFactor
			};
			var overspeedData = new DriverData.OverSpeedData {
				Enabled = driver.OverSpeedData.Enabled,
				MinSpeed = driver.OverSpeedData.MinSpeed,
				OverSpeed = driver.OverSpeedData.OverSpeed,
			};
			var retVal = new DriverData {
				AccelerationCurve = accelerationData,
				LookAheadCoasting = lookAheadData,
				OverSpeed = overspeedData,
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay =
						driver.EngineStopStartData?.ActivationDelay ?? DeclarationData.Driver.EngineStopStart.ActivationDelay,
					MaxEngineOffTimespan = driver.EngineStopStartData?.MaxEngineOffTimespan ?? DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan,
					UtilityFactorStandstill = driver.EngineStopStartData?.UtilityFactorStandstill ?? DeclarationData.Driver.EngineStopStart.UtilityFactor,
					UtilityFactorDriving = driver.EngineStopStartData?.UtilityFactorDriving ?? DeclarationData.Driver.EngineStopStart.UtilityFactor,
				},
				EcoRoll = new DriverData.EcoRollData() {
					UnderspeedThreshold = driver.EcoRollData?.UnderspeedThreshold ?? DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
					MinSpeed = driver.EcoRollData?.MinSpeed ?? DeclarationData.Driver.EcoRoll.MinSpeed,
					ActivationPhaseDuration = driver.EcoRollData?.ActivationDelay ?? DeclarationData.Driver.EcoRoll.ActivationDelay,
					AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
					AccelerationUpperLimit = driver.EcoRollData?.AccelerationUpperLimit ?? DeclarationData.Driver.EcoRoll.AccelerationUpperLimit,
				},
				PCC = new DriverData.PCCData() {
					PCCEnableSpeed = driver.PCCData?.PCCEnabledSpeed ?? DeclarationData.Driver.PCC.PCCEnableSpeed,
					MinSpeed = driver.PCCData?.MinSpeed ?? DeclarationData.Driver.PCC.MinSpeed,
					PreviewDistanceUseCase1 = driver.PCCData?.PreviewDistanceUseCase1 ?? DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
					PreviewDistanceUseCase2 = driver.PCCData?.PreviewDistanceUseCase2 ?? DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
					UnderSpeed = driver.PCCData?.Underspeed ?? DeclarationData.Driver.PCC.Underspeed,
					OverspeedUseCase3 = driver.PCCData?.OverspeedUseCase3 ?? DeclarationData.Driver.PCC.OverspeedUseCase3
				}
			};
			return retVal;
		}

		//=================================
		public RetarderData CreateRetarderData(IRetarderInputData retarder, PowertrainPosition powertrainPosition)
		{
			return SetCommonRetarderData(retarder, powertrainPosition);
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType != "None") {
				var ptoData = new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = pto.PTOLossMap == null ? PTOIdleLossMapReader.GetZeroLossMap() : PTOIdleLossMapReader.Create(pto.PTOLossMap),
				};
				if (pto.PTOCycleDuringStop != null) {
					ptoData.PTOCycle = DrivingCycleDataReader.ReadFromDataTable(pto.PTOCycleDuringStop, "PTO", false);
				}
				return ptoData;
			}

			return null;
		}

		public IAuxiliaryConfig CreateBusAuxiliariesData(IAuxiliariesEngineeringInputData auxInputData,
			VehicleData vehicleData, VectoSimulationJobType jobType)
		{
			if (auxInputData == null || auxInputData.BusAuxiliariesData == null) {
				return null;
			}

			var busAux = auxInputData.BusAuxiliariesData;
			return jobType == VectoSimulationJobType.BatteryElectricVehicle
				? GetBatteryElectricBusAuxiliariesData(vehicleData, busAux)
				: GetBusAuxiliariesData(vehicleData, busAux);
		}

		private IAuxiliaryConfig GetBusAuxiliariesData(VehicleData vehicleData, IBusAuxiliariesEngineeringData busAux)
		{
			return new AuxiliaryConfig() {
				//InputData = auxInputData.BusAuxiliariesData,
				ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
					PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
					//StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
					ResultCardIdle = new DummyResultCard(),
					ResultCardOverrun = new DummyResultCard(),
					ResultCardTraction = new DummyResultCard(),
					AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
					DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
					AlternatorMap = new SimpleAlternator(busAux.ElectricSystem.AlternatorEfficiency),
					AlternatorType =
						busAux.ElectricSystem.ESSupplyFromHEVREESS &&
						busAux.ElectricSystem.AlternatorType != AlternatorType.Smart
							? AlternatorType.None
							: busAux.ElectricSystem.AlternatorType,
					ConnectESToREESS = busAux.ElectricSystem.ESSupplyFromHEVREESS,
					DCDCEfficiency = busAux.ElectricSystem.DCDCConverterEfficiency.LimitTo(0, 1),
					MaxAlternatorPower = busAux.ElectricSystem.MaxAlternatorPower,
					ElectricStorageCapacity = busAux.ElectricSystem.ElectricStorageCapacity ?? 0.SI<WattSecond>(),
					StoredEnergyEfficiency = busAux.ElectricSystem.ElectricStorageEfficiency,
					ElectricalConsumers = GetElectricConsumers(busAux.ElectricSystem)
				},
				PneumaticAuxillariesConfig = new PneumaticsConsumersDemand() {
					AdBlueInjection = 0.SI<NormLiterPerSecond>(),
					AirControlledSuspension = busAux.PneumaticSystem.AverageAirConsumed,
					Braking = 0.SI<NormLiterPerKilogram>(),
					BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
					DeadVolBlowOuts = 0.SI<PerSecond>(),
					DeadVolume = 0.SI<NormLiter>(),
					NonSmartRegenFractionTotalAirDemand = 0,
					SmartRegenFractionTotalAirDemand = 0,
					OverrunUtilisationForCompressionFraction =
						Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
					DoorOpening = 0.SI<NormLiter>(),
					StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
				},
				PneumaticUserInputsConfig = new PneumaticUserInputsConfig() {
					CompressorMap =
						new CompressorMap(CompressorMapReader.Create(busAux.PneumaticSystem.CompressorMap, 1.0),
							"engineering mode", busAux.PneumaticSystem.CompressorMap.Source),
					CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
					CompressorGearRatio = busAux.PneumaticSystem.GearRatio,
					SmartAirCompression = busAux.PneumaticSystem.SmartAirCompression,
					SmartRegeneration = false,
					KneelingHeight = 0.SI<Meter>(),
					AirSuspensionControl = ConsumerTechnology.Pneumatically,
					AdBlueDosing = ConsumerTechnology.Electrically,
					Doors = ConsumerTechnology.Electrically
				},
				Actuations = new Actuations() {
					Braking = 0,
					Kneeling = 0,
					ParkBrakeAndDoors = 0,
					CycleTime = 1.SI<Second>()
				},
				SSMInputs = new SSMEngineeringInputs() {
					MechanicalPower = busAux.HVACData.MechanicalPowerDemand,
					ElectricPower = busAux.HVACData.ElectricalPowerDemand,
					AuxHeaterPower = busAux.HVACData.AuxHeaterPower,
					HeatingDemand = busAux.HVACData.AverageHeatingDemand,
					AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
					FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
					CoolantHeatTransferredToAirCabinHeater =
						Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				},
				VehicleData = vehicleData,
			};
		}

		private IAuxiliaryConfig GetBatteryElectricBusAuxiliariesData(VehicleData vehicleData, IBusAuxiliariesEngineeringData busAux)
		{
			return new AuxiliaryConfig() {
				//InputData = auxInputData.BusAuxiliariesData,
				ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
					PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
					//StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
					ResultCardIdle = new DummyResultCard(),
					ResultCardOverrun = new DummyResultCard(),
					ResultCardTraction = new DummyResultCard(),
					AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
					DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
					AlternatorMap = new SimpleAlternator(1),
					AlternatorType = AlternatorType.None,
					ConnectESToREESS = true,
					DCDCEfficiency = busAux.ElectricSystem.DCDCConverterEfficiency.LimitTo(0, 1),
					MaxAlternatorPower = 0.SI<Watt>(),
					ElectricStorageCapacity =  0.SI<WattSecond>(),
					StoredEnergyEfficiency = 1,
					ElectricalConsumers = GetElectricConsumers(busAux.ElectricSystem)
				},
				PneumaticAuxillariesConfig = new PneumaticsConsumersDemand() {
					AdBlueInjection = 0.SI<NormLiterPerSecond>(),
					AirControlledSuspension = 0.SI<NormLiterPerSecond>(),
					Braking = 0.SI<NormLiterPerKilogram>(),
					BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
					DeadVolBlowOuts = 0.SI<PerSecond>(),
					DeadVolume = 0.SI<NormLiter>(),
					NonSmartRegenFractionTotalAirDemand = 0,
					SmartRegenFractionTotalAirDemand = 0,
					OverrunUtilisationForCompressionFraction =
						Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
					DoorOpening = 0.SI<NormLiter>(),
					StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
				},
				PneumaticUserInputsConfig = new PneumaticUserInputsConfig() {
					CompressorMap = new CompressorMap(new List<CompressorMapValues>() {
							new CompressorMapValues(0.RPMtoRad(), 1.SI<NormLiterPerSecond>() , 1.SI<Watt>(), 0.SI<Watt>()),
							new CompressorMapValues(1e12.RPMtoRad(), 1.SI<NormLiterPerSecond>() , 1.SI<Watt>(), 0.SI<Watt>())
						}, 
						"engineering mode", "none"),
					CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
					CompressorGearRatio = 0,
					SmartAirCompression = false,
					SmartRegeneration = false,
					KneelingHeight = 0.SI<Meter>(),
					AirSuspensionControl = ConsumerTechnology.Pneumatically,
					AdBlueDosing = ConsumerTechnology.Electrically,
					Doors = ConsumerTechnology.Electrically
				},
				Actuations = new Actuations() {
					Braking = 0,
					Kneeling = 0,
					ParkBrakeAndDoors = 0,
					CycleTime = 1.SI<Second>()
				},
				SSMInputs = new SSMEngineeringInputs() {
					MechanicalPower = 0.SI<Watt>(),
					ElectricPower = 0.SI<Watt>(),
					AuxHeaterPower = 0.SI<Watt>(),
					HeatingDemand = 0.SI<WattSecond>(),
					AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
					FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
					CoolantHeatTransferredToAirCabinHeater =
						Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				},
				VehicleData = vehicleData,
			};
		}

		

		private Dictionary<string, ElectricConsumerEntry> GetElectricConsumers(IBusAuxElectricSystemEngineeringData busAuxElectricSystem)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();

			var iBase = busAuxElectricSystem.CurrentDemandEngineOffStandstill;
			var iSP = busAuxElectricSystem.CurrentDemandEngineOffDriving -
					busAuxElectricSystem.CurrentDemandEngineOffStandstill;
			var iFan = busAuxElectricSystem.CurrentDemand - busAuxElectricSystem.CurrentDemandEngineOffDriving;

			retVal["BaseLoad"] = new ElectricConsumerEntry() {
				Current = iBase,
				BaseVehicle = true
			};
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry() {
				Current = iSP,
				ActiveDuringEngineStopStandstill = false,
			};
			retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry() {
				Current = iFan,
				ActiveDuringEngineStopStandstill = false,
				ActiveDuringEngineStopDriving = false,
			};
			return retVal;
		}


		public ShiftStrategyParameters CreateGearshiftData(GearboxType gbxType, IGearshiftEngineeringInputData gsInputData, double axleRatio, PerSecond engineIdlingSpeed)
		{
			if (gsInputData == null) {
				return null;
			}

			var retVal = new ShiftStrategyParameters {
				TorqueReserve = gsInputData.TorqueReserve,
				StartTorqueReserve = gsInputData.StartTorqueReserve,
				TimeBetweenGearshifts = gsInputData.MinTimeBetweenGearshift,
				StartSpeed = gsInputData.StartSpeed,
				DownshiftAfterUpshiftDelay = gsInputData.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = gsInputData.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = gsInputData.UpshiftMinAcceleration,

				StartVelocity = gsInputData.StartSpeed ?? DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = gsInputData.StartAcceleration ?? DeclarationData.GearboxTCU.StartAcceleration,
				GearResidenceTime = gsInputData.GearResidenceTime ?? DeclarationData.GearboxTCU.GearResidenceTime,
				DnT99L_highMin1 = gsInputData.DnT99LHMin1 ?? DeclarationData.GearboxTCU.DnT99L_highMin1,
				DnT99L_highMin2 = gsInputData.DnT99LHMin2 ?? DeclarationData.GearboxTCU.DnT99L_highMin2,
				AllowedGearRangeUp = gsInputData.AllowedGearRangeUp ?? DeclarationData.GearboxTCU.AllowedGearRangeUp,
				AllowedGearRangeDown = gsInputData.AllowedGearRangeDown ?? DeclarationData.GearboxTCU.AllowedGearRangeDown,
				LookBackInterval = gsInputData.LookBackInterval ?? DeclarationData.GearboxTCU.LookBackInterval,
				DriverAccelerationLookBackInterval = gsInputData.DriverAccelerationLookBackInterval ?? DeclarationData.GearboxTCU.DriverAccelerationLookBackInterval,
				DriverAccelerationThresholdLow = gsInputData.DriverAccelerationThresholdLow ?? DeclarationData.GearboxTCU.DriverAccelerationThresholdLow,
				AverageCardanPowerThresholdPropulsion = gsInputData.AvgCardanPowerThresholdPropulsion ??
														DeclarationData.GearboxTCU.AverageCardanPowerThresholdPropulsion,
				CurrentCardanPowerThresholdPropulsion = gsInputData.CurrCardanPowerThresholdPropulsion ??
														DeclarationData.GearboxTCU.CurrentCardanPowerThresholdPropulsion,
				TargetSpeedDeviationFactor = gsInputData.TargetSpeedDeviationFactor ?? DeclarationData.GearboxTCU.TargetSpeedDeviationFactor,
				EngineSpeedHighDriveOffFactor = gsInputData.EngineSpeedHighDriveOffFactor ?? DeclarationData.GearboxTCU.EngineSpeedHighDriveOffFactor,
				AccelerationReserveLookup = AccelerationReserveLookupReader.Create(gsInputData.AccelerationReserveLookup) ??
											AccelerationReserveLookupReader.ReadFromStream(
												RessourceHelper.ReadStream(
													DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.AccelerationReserveLookup.csv")),
				ShareTorque99L = ShareTorque99lLookupReader.Create(gsInputData.ShareTorque99L) ??
								ShareTorque99lLookupReader.ReadFromStream(
									RessourceHelper.ReadStream(
										DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareTq99L.csv")
								),
				PredictionDurationLookup = PredictionDurationLookupReader.Create(gsInputData.PredictionDurationLookup) ??
											PredictionDurationLookupReader.ReadFromStream(
												RessourceHelper.ReadStream(
													DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.PredictionTimeLookup.csv")
											),
				ShareIdleLow = ShareIdleLowReader.Create(gsInputData.ShareIdleLow) ?? ShareIdleLowReader.ReadFromStream(
									RessourceHelper.ReadStream(
										DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareIdleLow.csv")
								),
				ShareEngineHigh = EngineSpeedHighLookupReader.Create(gsInputData.ShareEngineHigh) ??
								EngineSpeedHighLookupReader.ReadFromStream(
									RessourceHelper.ReadStream(
										DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareEngineSpeedHigh.csv")
								),
				//---------------
				RatingFactorCurrentGear = gsInputData.RatingFactorCurrentGear ?? (gbxType.AutomaticTransmission()
											? DeclarationData.GearboxTCU.RatingFactorCurrentGearAT
											: DeclarationData.GearboxTCU.RatingFactorCurrentGear),

				RatioEarlyUpshiftFC = (gsInputData.RatioEarlyUpshiftFC ?? DeclarationData.GearboxTCU.RatioEarlyUpshiftFC) / axleRatio,
				RatioEarlyDownshiftFC = (gsInputData.RatioEarlyDownshiftFC ?? DeclarationData.GearboxTCU.RatioEarlyDownshiftFC) / axleRatio,
				AllowedGearRangeFC = gsInputData.AllowedGearRangeFC ?? (gbxType.AutomaticTransmission() ? DeclarationData.GearboxTCU.AllowedGearRangeFCAT : DeclarationData.GearboxTCU.AllowedGearRangeFCAMT),
				VelocityDropFactor = gsInputData.VeloictyDropFactor ?? DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = gsInputData.AccelerationFactor ?? DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = gsInputData.MinEngineSpeedPostUpshift ?? DeclarationData.GearboxTCU.MinEngineSpeedPostUpshift,
				ATLookAheadTime = gsInputData.ATLookAheadTime ?? DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsDown = gsInputData.LoadStageThresholdsDown?.ToArray() ?? DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				LoadStageThresoldsUp = gsInputData.LoadStageThresholdsUp?.ToArray() ?? DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				ShiftSpeedsTCToLocked = engineIdlingSpeed == null ? null : (gsInputData.ShiftSpeedsTCToLocked ?? DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked).Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),

				// voith gs parameters

				GearshiftLines = gsInputData.LoadStageShiftLines,

				LoadstageThresholds = gsInputData.LoadStageThresholdsUp != null && gsInputData.LoadStageThresholdsDown != null ? gsInputData.LoadStageThresholdsUp.Zip(gsInputData.LoadStageThresholdsDown, Tuple.Create) : null
			};

			if (gsInputData.PEV_DeRatingDownshiftSpeedFactor != null) {
				retVal.PEV_DeRatedDownshiftSpeedFactor = gsInputData.PEV_DeRatingDownshiftSpeedFactor.Value;
			}

			if (gsInputData.PEV_DownshiftSpeedFactor != null) {
				retVal.PEV_DownshiftSpeedFactor = gsInputData.PEV_DownshiftSpeedFactor.Value;
			}

			if (gsInputData.PEV_TargetSpeedBrakeNorm != null) {
				retVal.PEV_TargetSpeedBrakeNorm = gsInputData.PEV_TargetSpeedBrakeNorm.Value;
			}

			if (gsInputData.PEV_DownshiftMinSpeedFactor != null) {
				retVal.PEV_DownshiftMinSpeedFactor = gsInputData.PEV_DownshiftMinSpeedFactor.Value;
			}

			return retVal;
		}

		public BatterySystemData CreateBatteryData(IElectricStorageSystemEngineeringInputData batteryInputData, double initialSOC)
		{
			if (batteryInputData == null) {
				return null;
			}

			var bat = batteryInputData.ElectricStorageElements.Where(x => x.REESSPack.StorageType == REESSType.Battery).ToArray();

			if (bat.Length == 0) {
				return null;
			}

			var retVal = new BatterySystemData();
			foreach (var entry in bat) {
				var b = entry.REESSPack as IBatteryPackDeclarationInputData;
				if (b == null) {
					continue;
				}

				for (var i = 0; i < entry.Count; i++) {
					retVal.Batteries.Add(Tuple.Create(entry.StringId, new BatteryData() {
						MinSOC = b.MinSOC.Value,
						MaxSOC = b.MaxSOC.Value,
						MaxCurrent = BatteryMaxCurrentReader.Create(b.MaxCurrentMap),
						Capacity = b.Capacity,
						InternalResistance =
							BatteryInternalResistanceReader.Create(b.InternalResistanceCurve, false),
						SOCMap = BatterySOCReader.Create(b.VoltageCurve),
					}));
				}
			}

			retVal.InitialSoC = initialSOC;
			return retVal;
		}

		public SuperCapData CreateSuperCapData(IElectricStorageSystemEngineeringInputData reessInputData, double initialSOC)
		{
			if (reessInputData == null)
			{
				return null;
			}

			var superCaps = reessInputData.ElectricStorageElements.Where(x => x.REESSPack.StorageType == REESSType.SuperCap).ToArray();

			var superCap = superCaps.FirstOrDefault()?.REESSPack as ISuperCapDeclarationInputData;

			if (superCap == null) {
				return null;
			}

			return new SuperCapData()
			{
				Capacity = superCaps.First().Count * superCap.Capacity,
				InternalResistance = superCap.InternalResistance / superCaps.First().Count,
				MinVoltage = superCap.MinVoltage,
				MaxVoltage = superCap.MaxVoltage,
				MaxCurrentCharge = superCap.MaxCurrentCharge,
				MaxCurrentDischarge = -superCap.MaxCurrentDischarge,
				InitialSoC = initialSOC
			};
		}

		public List<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
			IElectricMachinesEngineeringInputData electricMachines,
			Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
			GearList gearlist = null)
		{
			if (electricMachines == null) {
				return null;
			}

			if (electricMachines.Entries.Any(x => x.ElectricMachine.SavedInDeclarationMode)) {
				WarnEngineeringMode("Electric motor");
			}

			if (electricMachines.Entries.Select(x => x.Position).Where(x => x != PowertrainPosition.GEN).Distinct().Count() > 1) {
				throw new VectoException("multiple electric propulsion motors are not supported at the moment");
			}

			return electricMachines.Entries
				.Select(x => Tuple.Create(x.Position,
					CreateElectricMachine(x.Position, x.ElectricMachine, x.Count, x.RatioADC, x.RatioPerGear, x.MechanicalTransmissionEfficiency,
						x.MechanicalTransmissionLossMap, torqueLimits?.First(t =>t.Key == x.Position).Value, averageVoltage, gearlist))).ToList();
		}

		private ElectricMotorData CreateElectricMachine(PowertrainPosition powertrainPosition,
			IElectricMotorEngineeringInputData motorData, int count,
			double ratio, double[] ratioPerGear, double efficiency, TableData adcLossMap,
			List<Tuple<Volt, TableData>> torqueLimits, Volt averageVoltage, GearList gearList)
		{
			var voltageLevels = new List<ElectricMotorVoltageLevelData>();

			foreach (var entry in motorData.VoltageLevels.OrderBy(x => x.VoltageLevel)) {
				var fullLoadCurve = ElectricFullLoadCurveReader.Create(entry.FullLoadCurve, count);
				var maxTorqueCurve = torqueLimits == null
					? null
					: ElectricFullLoadCurveReader.Create(
						torqueLimits.Where(x => x.Item1 == null || x.Item1.IsEqual(entry.VoltageLevel)).FirstOrDefault()?.Item2, count);

				var fullLoadCurveCombined = IntersectEMFullLoadCurves(fullLoadCurve, maxTorqueCurve);

				var vLevelData = powertrainPosition == PowertrainPosition.IHPC
					? CreateIHPCVoltageLevelData(count, entry, fullLoadCurveCombined, gearList)
					: CreateEmVoltageLevelData(count, entry, fullLoadCurveCombined);
				voltageLevels.Add(vLevelData);
				
			}

			if (averageVoltage == null) {
				// if no average voltage is provided (e.g. for supercap) use mean value of measured voltage maps
				averageVoltage = (voltageLevels.Min(x => x.Voltage) + voltageLevels.Max(x => x.Voltage)) / 2.0;
			}

			var lossMap = powertrainPosition == PowertrainPosition.IHPC
				? TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC IHPC LossMap Eff")
				: adcLossMap != null
					? TransmissionLossMapReader.CreateEmADCLossMap(adcLossMap, ratio, "EM ADC LossMap")
					: TransmissionLossMapReader.CreateEmADCLossMap(efficiency, ratio, "EM ADC LossMap Eff");

			var retVal = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() { VoltageLevels = voltageLevels},
				EMDragCurve = ElectricMotorDragCurveReader.Create(motorData.DragCurve, count),
				Inertia = motorData.Inertia * count,
				OverloadRegenerationFactor = motorData.OverloadRecoveryFactor,
				RatioADC = ratio,
				RatioPerGear = ratioPerGear,
				TransmissionLossMap = lossMap,
			};
			retVal.Overload = CalculateOverloadData(motorData, count, retVal.EfficiencyData, averageVoltage);
			return retVal;
		}

		private ElectricMotorVoltageLevelData CreateIHPCVoltageLevelData(int count, IElectricMotorVoltageLevel entry, ElectricMotorFullLoadCurve fullLoadCurveCombined, GearList gearList)
		{
			if (gearList == null) {
				throw new VectoException("no gears provided for IHPC EM");
			}

			if (gearList.Count() != entry.PowerMap.Count) {
				throw new VectoException(
					$"number of gears in transmission does not match gears in electric motor (IHPC) - {gearList.Count()}/{entry.PowerMap.Count}");
			}
			var effMap = new Dictionary<uint, EfficiencyMap>();
			foreach (var gear in gearList) {
				effMap.Add(gear.Gear, ElectricMotorMapReader.Create(entry.PowerMap[(int)gear.Gear - 1].PowerMap, count));
			}
			return new IEPCVoltageLevelData() {
				Voltage = entry.VoltageLevel,
				FullLoadCurve = fullLoadCurveCombined,
				EfficiencyMaps = effMap,
			};
		}

		private static ElectricMotorVoltageLevelData CreateEmVoltageLevelData(int count, IElectricMotorVoltageLevel entry, ElectricMotorFullLoadCurve fullLoadCurveCombined)
		{
			return new ElectricMotorVoltageLevelData() {
				Voltage = entry.VoltageLevel,
					
				FullLoadCurve = fullLoadCurveCombined,
				// DragCurve = ElectricMotorDragCurveReader.Create(entry.DragCurve, count),
				EfficiencyMap = ElectricMotorMapReader.Create(entry.PowerMap.First().PowerMap, count), //PowerMap
			};
		}

		private OverloadData CalculateOverloadData(IElectricMotorEngineeringInputData motorData, int count, VoltageLevelData voltageLevels, Volt averageVoltage)
		{
			
			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < motorData.VoltageLevels.Min(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(motorData.VoltageLevels.First(), count, voltageLevels);
			}
			if (averageVoltage > motorData.VoltageLevels.Max(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(motorData.VoltageLevels.Last(), count, voltageLevels);
			}

			var (vLow, vHigh) = motorData.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevels);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevels);

			var retVal = new OverloadData() {
				OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
				ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
				ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
			};
			return retVal;
		}

		private OverloadData CalculateOverloadData(IIEPCEngineeringInputData iepc, int count,
			VoltageLevelData voltageLevels, Volt averageVoltage, Tuple<uint, double> gearRatioUsedForMeasurement)
		{
			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < iepc.VoltageLevels.Min(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(iepc.VoltageLevels.First(), count, voltageLevels, gearRatioUsedForMeasurement);
			}
			if (averageVoltage > iepc.VoltageLevels.Max(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(iepc.VoltageLevels.Last(), count, voltageLevels, gearRatioUsedForMeasurement);
			}

			var (vLow, vHigh) = iepc.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevels, gearRatioUsedForMeasurement);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevels, gearRatioUsedForMeasurement);

			var retVal = new OverloadData() {
				OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
				ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
				ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
			};
			return retVal;
		}

		private OverloadData CalculateOverloadBuffer(IElectricMotorVoltageLevel voltageEntry,
			int count, VoltageLevelData voltageLevels, Tuple<uint, double> gearUsedForMeasurement = null)
		{
			var gearRatioUsedForMeasurement = gearUsedForMeasurement?.Item2 ?? 1.0;
			var gear = new GearshiftPosition(gearUsedForMeasurement?.Item1 ?? 1);
			var continuousTorque = voltageEntry.ContinuousTorque * count / gearRatioUsedForMeasurement;
			var continuousTorqueSpeed = voltageEntry.ContinuousTorqueSpeed * gearRatioUsedForMeasurement;
			var overloadTorque = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count / gearRatioUsedForMeasurement;
			var overloadTestSpeed = (voltageEntry.OverloadTestSpeed ?? 0.RPMtoRad()) * gearRatioUsedForMeasurement;


			var peakElPwr = voltageLevels.LookupElectricPower(voltageEntry.VoltageLevel, overloadTestSpeed, -overloadTorque, gear, true)
				.ElectricalPower;
			var peakPwrLoss = -peakElPwr - overloadTorque * overloadTestSpeed; // losses need to be positive
			
			var contElPwr = voltageLevels.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
								-continuousTorque, gear).ElectricalPower ??
							voltageLevels.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
								voltageLevels.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed),
								gear, true).ElectricalPower;
			var continuousPowerLoss = -contElPwr - continuousTorque * continuousTorqueSpeed; // loss needs to be positive
			var overloadBuffer = (peakPwrLoss - continuousPowerLoss) * voltageEntry.OverloadTime;
			return new OverloadData() {
				OverloadBuffer = overloadBuffer,
				ContinuousTorque = continuousTorque,
				ContinuousPowerLoss = continuousPowerLoss
			};
		}

		public HybridStrategyParameters CreateHybridStrategyParameters(
			IEngineeringJobInputData jobInputData,
			CombustionEngineData combustionEngineData, GearboxData gearboxData)
		{
			var hybridStrategyParameters = jobInputData.HybridStrategyParameters;
			
			var torqueLimit = CreateMaxPropulsionTorque(jobInputData.Vehicle, combustionEngineData, gearboxData);

			var retVal = new HybridStrategyParameters() {
				EquivalenceFactorDischarge = hybridStrategyParameters.EquivalenceFactorDischarge,
				EquivalenceFactorCharge = hybridStrategyParameters.EquivalenceFactorCharge,
				MinSoC = hybridStrategyParameters.MinSoC,
				MaxSoC = hybridStrategyParameters.MaxSoC,
				TargetSoC = hybridStrategyParameters.TargetSoC,
				MinICEOnTime = hybridStrategyParameters.MinimumICEOnTime,
				AuxReserveTime = hybridStrategyParameters.AuxBufferTime,
				AuxReserveChargeTime = hybridStrategyParameters.AuxBufferChargeTime,
				MaxPropulsionTorque = torqueLimit,
				ICEStartPenaltyFactor = hybridStrategyParameters.ICEStartPenaltyFactor,
				CostFactorSOCExponent = double.IsNaN(hybridStrategyParameters.CostFactorSOCExpponent) ? 5 : hybridStrategyParameters.CostFactorSOCExpponent,
				GensetMinOptPowerFactor = double.IsNaN(hybridStrategyParameters.GensetMinOptPowerFactor) ? 0 : hybridStrategyParameters.GensetMinOptPowerFactor,
			};
			return retVal;
		}

		protected internal static Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque> CreateMaxPropulsionTorque(IVehicleEngineeringInputData vehicleInputData, CombustionEngineData engineData, GearboxData gearboxData)
		{

			// engine data contains full-load curves already cropped with max gearbox torque and max ICE torque (vehicle level)

			var maxBoostingTorque = vehicleInputData.BoostingLimitations;
			var offset = maxBoostingTorque == null ? null : MaxBoostingTorqueReader.Create(maxBoostingTorque);
			var belowIdle = offset?.FullLoadEntries.Where(x => x.MotorSpeed < engineData.IdleSpeed).ToList();

			var retVal = new Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque>();
			var isP3OrP4Hybrid = vehicleInputData.Components.ElectricMachines.Entries.Select(x => x.Position)
				.Any(x => x == PowertrainPosition.HybridP3 || x == PowertrainPosition.HybridP4);
			foreach (var key in engineData.FullLoadCurves.Keys) {
				if (key == 0) {
					continue;
				}
				if (maxBoostingTorque == null) {
					if (gearboxData.Gears[key].MaxTorque == null) {
						continue;
					}
					// don't know what to do...
					// idea 1: apply gearbox limit for whole speed range
					// idea 2: use em max torque as boosting limitation
					var gbxLimit = new[] {
						new VehicleMaxPropulsionTorque.FullLoadEntry()
							{ MotorSpeed = 0.RPMtoRad(), FullDriveTorque = gearboxData.Gears[key].MaxTorque },
						new VehicleMaxPropulsionTorque.FullLoadEntry() {
							MotorSpeed = engineData.FullLoadCurves[0].N95hSpeed * 1.1,
							FullDriveTorque = gearboxData.Gears[key].MaxTorque
						}
					}.ToList();
					retVal[new GearshiftPosition(key, true)] = new VehicleMaxPropulsionTorque(gbxLimit);
					continue;
				} 

				// case boosting limit is defined, gearbox limit can be defined or not (handled in Intersect method)

				// entries contains ICE full-load curve with the boosting torque added. handles ICE speeds below idle
				var entries = belowIdle.Select(fullLoadEntry => new VehicleMaxPropulsionTorque.FullLoadEntry()
						{ MotorSpeed = fullLoadEntry.MotorSpeed, FullDriveTorque = fullLoadEntry.FullDriveTorque })
					.Concat(
						engineData.FullLoadCurves[key].FullLoadEntries.Where(x => x.EngineSpeed > engineData.IdleSpeed)
							.Select(fullLoadCurveEntry =>
								new VehicleMaxPropulsionTorque.FullLoadEntry() {
									MotorSpeed = fullLoadCurveEntry.EngineSpeed,
									FullDriveTorque = fullLoadCurveEntry.TorqueFullLoad +
													VectoMath.Max(
														offset?.FullLoadDriveTorque(fullLoadCurveEntry.EngineSpeed),
														0.SI<NewtonMeter>())
								}))
					.Concat(
						new[] { engineData.IdleSpeed, engineData.IdleSpeed - 0.1.RPMtoRad() }.Select(x =>
							new VehicleMaxPropulsionTorque.FullLoadEntry() {
								MotorSpeed = x,
								FullDriveTorque = engineData.FullLoadCurves[0].FullLoadStationaryTorque(x) +
												VectoMath.Max(offset?.FullLoadDriveTorque(x),
													0.SI<NewtonMeter>())
							}))
					.OrderBy(x => x.MotorSpeed).ToList();

				// if no gearbox limit is defined, MaxTorque is null;
				// in case of P3 or P4, do not apply gearbox limit to propulsion limit as ICE is already cropped with max torque
				var gearboxTorqueLimit = isP3OrP4Hybrid ? null : gearboxData.Gears[key].MaxTorque;
				retVal[new GearshiftPosition(key, true)] = new VehicleMaxPropulsionTorque(IntersectMaxPropulsionTorqueCurve(entries, gearboxTorqueLimit));

			}

			return retVal;
		}

		public List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCEngineeringInputData iepc, Volt averageVoltage)
		{
			if (iepc == null) {
				return null;
			}

			var pos = PowertrainPosition.IEPC;
			var count = iepc.DesignTypeWheelMotor && iepc.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;

			// the full-load curve is measured in the gear with the ratio closest to 1,
			// in case two gears have the same difference, the higher one is used
			var gearRatioUsedForMeasurement = iepc.Gears
				.Select(x => new { x.GearNumber, x.Ratio, Diff = Math.Round(Math.Abs(x.Ratio - 1), 6) }).GroupBy(x => x.Diff)
				.OrderBy(x => x.Key).First().OrderBy(x => x.Ratio).Reverse().First();
			

			var voltageLevels = new List<ElectricMotorVoltageLevelData>();
			foreach (var entry in iepc.VoltageLevels.OrderBy(x => x.VoltageLevel)) {
				var effMap = new Dictionary<uint, EfficiencyMap>();
				for (var i = 0u; i < entry.PowerMap.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					effMap.Add(i + 1, IEPCMapReader.Create(entry.PowerMap[(int)i].PowerMap, count, ratio));
					//fullLoadCurves.Add(i + 1, IEPCFullLoadCurveReader.Create(entry.FullLoadCurve, count, ratio));
				}
				voltageLevels.Add(new IEPCVoltageLevelData() {
					Voltage = entry.VoltageLevel,
					FullLoadCurve = IEPCFullLoadCurveReader.Create(entry.FullLoadCurve, count, gearRatioUsedForMeasurement.Ratio),
					EfficiencyMaps = effMap,
				});
			}

			var dragCurves = new Dictionary<uint, DragCurve>();
			if (iepc.DragCurves.Count > 1) {
				for (var i = 0u; i < iepc.DragCurves.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(iepc.DragCurves[(int)i].DragCurve, count, ratio));
				}
			} else {
				var dragCurve = iepc.DragCurves.First().DragCurve;
				for (var i = 0u; i < iepc.Gears.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(dragCurve, count, ratio));
				}
			}

			var retVal = new IEPCElectricMotorData() {
				EfficiencyData = new VoltageLevelData() { VoltageLevels = voltageLevels },
				IEPCDragCurves = dragCurves,
				Inertia = iepc.Inertia * count,
				OverloadRegenerationFactor = iepc.OverloadRecoveryFactor,
				RatioADC = 1,
				RatioPerGear = null,
				TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC LossMap Eff"),
				
			};
			retVal.Overload = CalculateOverloadData(iepc, count, retVal.EfficiencyData, averageVoltage,
				Tuple.Create((uint)gearRatioUsedForMeasurement.GearNumber, gearRatioUsedForMeasurement.Ratio));
			;
			return new List<Tuple<PowertrainPosition, ElectricMotorData>>() { Tuple.Create<PowertrainPosition, ElectricMotorData>(pos, retVal) };
		}




		public GearboxData CreateIEPCGearboxData(IEngineeringInputDataProvider inputData, VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalc)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var iepc = vehicle.Components.IEPCEngineeringInputData;

			var axlegearRatio = runData.AxleGearData?.AxleGear.Ratio ?? 1.0; 
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;


			var retVal = new GearboxData() {
				Type = GearboxType.APTN,
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 0.SI<Second>(),
				InputData = new IEPCGearboxInputData(iepc),
			};

			var gearInput = iepc.Gears.Select((x, idx) => new TransmissionInputData() { Gear = idx + 1, Ratio = x.Ratio }).Cast<ITransmissionInputData>().ToList();
			var gears = new Dictionary<uint, GearData>();
			for (uint i = 0; i < iepc.Gears.Count; i++) {
				var gear = iepc.Gears[(int)i];
				var lossMap = TransmissionLossMapReader.Create(1, gear.Ratio, $"Gear{i+1}");

				ShiftPolygon shiftPolygon = null;
				if (iepc.Gears.Count > 1) {
					if (shiftPolygonCalc != null) {
						shiftPolygon = shiftPolygonCalc.ComputeDeclarationShiftPolygon(GearboxType.APTN, (int)i,
							null, gearInput, null, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
					} else {
						shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(GearboxType.APTN, (int)i,
							null, gearInput, null, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
					}
				}
				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxOutputShaftSpeed == null ? null : gear.MaxOutputShaftSpeed * gear.Ratio,
					MaxTorque = gear.MaxOutputShaftTorque == null ? null : gear.MaxOutputShaftTorque / gear.Ratio,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				gears.Add(i + 1, gearData);
			}

			retVal.Gears = gears;

			// update disengageWhenHaltingSpeed
			var firstGear = retVal.GearList.First(x => x.IsLockedGear());
			if (iepc.Gears.Count > 1 && retVal.Gears[firstGear.Gear].ShiftPolygon.Downshift.Any()) {
				var downshiftSpeedInc = retVal.Gears[firstGear.Gear].ShiftPolygon
					.InterpolateDownshiftSpeed(0.SI<NewtonMeter>()) * 1.05;
				var vehicleSpeedDisengage = downshiftSpeedInc / axlegearRatio / retVal.Gears[firstGear.Gear].Ratio *
											dynamicTyreRadius;
				retVal.DisengageWhenHaltingSpeed = vehicleSpeedDisengage;
			}

			return retVal;
		}

		public PTOData CreateBatteryElectricPTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType != "None") {
				var ptoData = new PTOData() {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = pto.PTOLossMap == null
						? PTOIdleLossMapReader.GetZeroLossMap()
						: PTOIdleLossMapReader.Create(pto.PTOLossMap)
				};
				return ptoData;
			}

			return null;
		}
	}

	public class IEPCGearboxInputData : IGearboxDeclarationInputData
	{
		protected readonly IIEPCEngineeringInputData _iepc;
		private IList<ITransmissionInputData> _gears;

		public IEPCGearboxInputData(IIEPCEngineeringInputData iepc)
		{
			_iepc = iepc;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource => _iepc.DataSource;
		public bool SavedInDeclarationMode => _iepc.SavedInDeclarationMode;
		public string Manufacturer => _iepc.Manufacturer;
		public string Model => _iepc.Model;
		public DateTime Date => _iepc.Date;
		public string AppVersion => _iepc.AppVersion;
		public CertificationMethod CertificationMethod => _iepc.CertificationMethod;
		public string CertificationNumber => _iepc.CertificationNumber;
		public DigestData DigestValue => _iepc.DigestValue;

		#endregion

		#region Implementation of IGearboxDeclarationInputData

		public GearboxType Type => GearboxType.APTN;
		public IList<ITransmissionInputData> Gears => _gears ?? (_gears = ConvertGears());

		public bool DifferentialIncluded => false;
		public double AxlegearRatio => double.NaN;

		#endregion
		protected virtual IList<ITransmissionInputData> ConvertGears()
		{
			return _iepc.Gears.Select(x => new TransmissionInputData() {
				Gear = x.GearNumber,
				Ratio = x.Ratio,
				MaxInputSpeed = x.MaxOutputShaftSpeed == null ? null : x.MaxOutputShaftSpeed * x.Ratio,
				MaxTorque = x.MaxOutputShaftTorque == null ? null : x.MaxOutputShaftTorque / x.Ratio,
			}).Cast<ITransmissionInputData>().ToList();
		}


	}
}

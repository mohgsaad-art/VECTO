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
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
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

			retVal.AxleData = axles.Select(axle => new Axle {
				WheelsDimension = axle.Tyre.Dimension,
				Inertia = axle.Tyre.Inertia,
				TwinTyres = axle.TwinTyres,
				RollResistanceCoefficient = axle.Tyre.RollResistanceCoefficient,
				AxleWeightShare = axle.AxleWeightShare,
				TyreTestLoad = axle.Tyre.TyreTestLoad,
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
				} :
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
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(airdragData.AirDragArea),
						CrossWindCorrectionMode.NoCorrection);
					break;
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurve(airdragData.CrosswindCorrectionMap,
							airdragData.AirDragArea), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
					break;
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionVAirBeta(airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.ReadCdxABetaTable(airdragData.CrosswindCorrectionMap));
					break;
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					var airDragArea = airdragData.AirDragArea ??
									DeclarationData.Segments.LookupCdA(data.VehicleCategory, data.AxleConfiguration, data.GrossVehicleMassRating, false);
					var height = data.Height ?? DeclarationData.Segments.LookupHeight(data.VehicleCategory, data.AxleConfiguration,
						data.GrossVehicleMassRating, false);
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(airDragArea,
						DeclarationDataAdapter.GetDeclarationAirResistanceCurve(
							GetAirdragParameterSet(data.VehicleCategory, data.AxleConfiguration, data.Components.AxleWheels.AxlesEngineering.Count), airDragArea, height),
						CrossWindCorrectionMode.DeclarationModeCorrection);
					break;
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrection", airdragData.CrossWindCorrectionMode.ToString());
			}
			return retVal;
		}

		private string GetAirdragParameterSet(VehicleCategory vehicleCategory, AxleConfiguration axles, int numAxles)
		{
			switch (vehicleCategory) {
				case VehicleCategory.RigidTruck:
					return numAxles > axles.NumAxles() ? "RigidTrailer" : "RigidSolo";
				case VehicleCategory.Tractor:
					return "TractorSemitrailer";
				case VehicleCategory.CityBus:
				case VehicleCategory.InterurbanBus:
				case VehicleCategory.Coach:
					return "CoachBus";
				default:
					throw new ArgumentOutOfRangeException("vehicleCategory", vehicleCategory, null);
			}
		}

		private void WarnEngineeringMode(string msg)
		{
			Log.Error("{0} is in Declaration Mode but is used for Engineering Mode!", msg);
		}

		internal CombustionEngineData CreateEngineData(IVehicleEngineeringInputData vehicle, IEngineModeEngineeringInputData engineMode)
		{
			var engine = vehicle.Components.EngineInputData;
			var gbx = vehicle.Components.GearboxInputData;
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
							(gbx != null && gbx.Type.AutomaticTransmission() ? torqueConverter.Inertia : 0.SI<KilogramSquareMeter>());
			retVal.EngineStartTime = engine.EngineStartTime ?? DeclarationData.Engine.DefaultEngineStartTime;
			var limits = vehicle.TorqueLimits?.ToDictionary(e => e.Gear) ?? new Dictionary<int, ITorqueLimitInputData>();
			IList<ITransmissionInputData> gears = null;
			if (gbx != null){
				 gears = DeclarationDataAdapter.FilterDisabledGears(gbx.Gears, limits);
			}
			var numGears = gbx == null ? 0 : gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(engine.EngineModes.First().FullLoadCurve);
			fullLoadCurves[0].EngineData = retVal;
			if (gbx != null) {
				foreach (var gear in gears) {
					var maxTorque = VectoMath.Min(gear.MaxTorque, limits.ContainsKey(gear.Gear) ? limits[gear.Gear].MaxTorque : null);
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
		//IGearboxEngineeringInputData gearbox, CombustionEngineData engineData, IGearshiftEngineeringInputData gearshiftData,
		//double axlegearRatio, Meter dynamicTyreRadius, VehicleCategory vehicleCategory,
		//ITorqueConverterEngineeringInputData torqueConverter, IShiftPolygonCalculator shiftPolygonCalc, IAdvancedDriverAssistantSystemDeclarationInputData adas)
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
			retVal.ATEcoRollReleaseLockupClutch = adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission() ? adas.ATEcoRollReleaseLockupClutch.Value : false;

			var limits = inputData.JobInputData.Vehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var gearsInput = DeclarationDataAdapter.FilterDisabledGears(gearbox.Gears, limits);
			if (gearsInput.Count < 2) {
				throw new VectoSimulationException("At least two Gear-Entries must be defined in Gearbox!");
			}
			
			if (gearsInput.Count < 2) {
				throw new VectoSimulationException("At least two Gear-Entries must be defined in Gearbox!");
			}

			SetEngineeringData(gearbox, gearshiftData, retVal);

			var gearDifferenceRatio = gearbox.Type.AutomaticTransmission() && gearsInput.Count > 2
				? gearsInput[0].Ratio / gearsInput[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();
			ShiftPolygon tcShiftPolygon = null;
			if (gearbox.Type.AutomaticTransmission()) {
				tcShiftPolygon = torqueConverter.ShiftPolygon != null
					? ShiftPolygonReader.Create(torqueConverter.ShiftPolygon)
					: DeclarationData.TorqueConverter.ComputeShiftPolygon(engineData.FullLoadCurves[0]);
			}
			for (uint i = 0; i < gearsInput.Count; i++) {
				var gear = gearsInput[(int)i];
				var lossMap = CreateGearLossMap(gear, i, true, VehicleCategory.Unknown, gearbox.Type);

				var shiftPolygon = gear.ShiftPolygon != null && gear.ShiftPolygon.SourceType != DataSourceType.Missing
					? ShiftPolygonReader.Create(gear.ShiftPolygon)
					: shiftPolygonCalc != null
						? shiftPolygonCalc.ComputeDeclarationShiftPolygon(
							gearbox.Type, (int)i, engineData?.FullLoadCurves[i + 1], gearsInput,
							engineData,
							axlegearRatio, dynamicTyreRadius)
						: DeclarationData.Gearbox.ComputeShiftPolygon(
							gearbox.Type, (int)i, engineData?.FullLoadCurves[i + 1], gearsInput,
							engineData,
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

		private static void CreateATGearData(IGearboxEngineeringInputData gearbox, uint i, GearData gearData,
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

		private static void SetEngineeringData(IGearboxEngineeringInputData gearbox, IGearshiftEngineeringInputData gearshiftData, GearboxData retVal)
		{
			retVal.Inertia = gearbox.Type.ManualTransmission() ? gearbox.Inertia : 0.SI<KilogramSquareMeter>();
			retVal.TractionInterruption = gearbox.TractionInterruption;
			
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

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesEngineeringInputData auxInputData)
		{
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
				new VectoRunData.AuxData { ID = DrivingCycleDataReader.Fields.AdditionalAuxPowerDemand, DemandType = AuxiliaryDemandType.Direct }
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
					new LACDecisionFactor(driver.Lookahead.CoastingDecisionFactorOffset, driver.Lookahead.CoastingDecisionFactorScaling,
						driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup,
						driver.Lookahead.CoastingDecisionFactorVelocityDropLookup),
				LookAheadDistanceFactor = driver.Lookahead.LookaheadDistanceFactor
			};
			var overspeedData = new DriverData.OverSpeedData()  {
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
		public RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType != "None") {
				var ptoData = new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.Create(pto.PTOLossMap),
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
			//return jobType == VectoSimulationJobType.BatteryElectricVehicle
			//	? GetBatteryElectricBusAuxiliariesData(vehicleData, busAux)
			//	: GetBusAuxiliariesData(vehicleData, busAux);
			return GetBusAuxiliariesData(vehicleData, busAux);
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
				TimeBetweenGearshifts = VectoMath.Max(gsInputData.MinTimeBetweenGearshift, 0.01.SI<Second>()),
				StartSpeed = gsInputData.StartSpeed,
				DownshiftAfterUpshiftDelay = gsInputData.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = gsInputData.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = gsInputData.UpshiftMinAcceleration,

				StartVelocity = gsInputData.StartSpeed ?? DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = gsInputData.StartAcceleration ?? DeclarationData.GearboxTCU.StartAcceleration,
				RatingFactorCurrentGear = gsInputData.RatingFactorCurrentGear ?? (gbxType.AutomaticTransmission()
											? DeclarationData.GearboxTCU.RatingFactorCurrentGearAPT
											: DeclarationData.GearboxTCU.RatingFactorCurrentGear),

				RatioEarlyUpshiftFC = (gsInputData.RatioEarlyUpshiftFC ?? DeclarationData.GearboxTCU.RatioEarlyUpshiftFC) / axleRatio,
				RatioEarlyDownshiftFC = (gsInputData.RatioEarlyDownshiftFC ?? DeclarationData.GearboxTCU.RatioEarlyDownshiftFC) / axleRatio,
				AllowedGearRangeFC = gsInputData.AllowedGearRangeFC ?? (gbxType.AutomaticTransmission() ? DeclarationData.GearboxTCU.AllowedGearRangeFCAPT : DeclarationData.GearboxTCU.AllowedGearRangeFCAMT),
				VelocityDropFactor = gsInputData.VeloictyDropFactor ?? DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = gsInputData.AccelerationFactor ?? DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = gsInputData.MinEngineSpeedPostUpshift ?? DeclarationData.GearboxTCU.MinEngineSpeedPostUpshift,
				ATLookAheadTime = gsInputData.ATLookAheadTime ?? DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsDown = gsInputData.LoadStageThresholdsDown?.ToArray() ?? DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				LoadStageThresoldsUp = gsInputData.LoadStageThresholdsUp?.ToArray() ?? DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				ShiftSpeedsTCToLocked = engineIdlingSpeed == null ? null : (gsInputData.ShiftSpeedsTCToLocked ?? DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked).Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
				EffshiftAccelerationFactorAT = gsInputData.EffshiftAccelerationFactorAT ?? DeclarationData.GearboxTCU.DownshiftMaxAccelerationAT,

			};

			return retVal;
		}


		public class ElectricConsumerEntry
		{

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
}
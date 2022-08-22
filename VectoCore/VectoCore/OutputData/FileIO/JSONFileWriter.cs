using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.OutputData;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCommon.Utils;

public class JSONFileWriter : IOutputFileWriter
{
	public const int EngineFormatVersion = 5;

	public const int GearboxFormatVersion = 6;

	public const int VehicleFormatVersion = 9;

	//public const int BusVehicleFormatVersion = 9;
	
	//public const int HEV_BEVVehicleFormatVersion = 10;


	private const int VectoJobFormatVersion = 5;

	//private const int HEVVectoJobFormatVersion = 8;

	//private const int BEVVectoJobFormatVersion = 9;


	private const int VectoVTPJobFormatVersion = 4;

	//private const int ElectricMotorFormatVersion = 3;

	//private const int REESSFormatVersion = 1;

	//private const int HybridStrategyParamsVersion = 1;

	private const int BusAuxEngParamsVersion = 1;


	private static JSONFileWriter _instance;

	public const string VECTOvers = "3";

	public static JSONFileWriter Instance
	{
		get {
			if (_instance == null)
				_instance = new JSONFileWriter();
			return _instance;
		}
	}

	public static string GetRelativePath(string filePath, string basePath)
	{
		if (String.IsNullOrEmpty(filePath)) {
			return "";
		}

		if (string.IsNullOrEmpty(basePath)) {
			return filePath;
		}

		if (Path.GetDirectoryName(Path.GetFullPath(filePath)).StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) {
			return Path.GetFullPath(filePath).Substring(basePath.Length + (basePath.EndsWith(@"\") ? 0 : 1));
		}

		return filePath;
	}

	public void SaveEngine(IEngineEngineeringInputData eng, string filename, bool DeclMode)
	{
		// Header
		var header = GetHeader(EngineFormatVersion);

		// Body
		var body = new Dictionary<string, object>();

		body.Add("SavedInDeclMode", DeclMode);

		body.Add("ModelName", eng.Model);

		body.Add("Displacement", eng.Displacement.ConvertToCubicCentiMeter().ToString());
		body.Add("IdlingSpeed", eng.EngineModes.First().IdleSpeed.AsRPM);
		body.Add("Inertia", eng.Inertia.Value());

		var fuels = new List<object>();

		foreach (var fuel in eng.EngineModes.First().Fuels) {
			var entry = new Dictionary<string, object>();
			entry.Add("WHTC-Urban", fuel.WHTCUrban);
			entry.Add("WHTC-Rural", fuel.WHTCRural);
			entry.Add("WHTC-Motorway", fuel.WHTCMotorway);
			entry.Add("WHTC-Engineering", fuel.WHTCEngineering);
			entry.Add("ColdHotBalancingFactor", fuel.ColdHotBalancingFactor);
			entry.Add("CFRegPer", fuel.CorrectionFactorRegPer);
			entry.Add("FuelMap", GetRelativePath(fuel.FuelConsumptionMap.Source, Path.GetDirectoryName(filename)));
			entry.Add("FuelType", fuel.FuelType.ToString());

			fuels.Add(entry);
		}

		body.Add("Fuels", fuels);

		body.Add("RatedPower", eng.RatedPowerDeclared.Value());
		body.Add("RatedSpeed", eng.RatedSpeedDeclared.AsRPM);
		body.Add("MaxTorque", eng.MaxTorqueDeclared.Value());

		body.Add(
			"FullLoadCurve", GetRelativePath(eng.EngineModes.First().FullLoadCurve.Source, Path.GetDirectoryName(filename)));

		var whrtypes = new List<string>();
		if ((eng.WHRType & WHRType.ElectricalOutput) != 0) {
			whrtypes.Add(WHRType.ElectricalOutput.ToString());
		}
		if ((eng.WHRType & WHRType.MechanicalOutputDrivetrain) != 0) {
			whrtypes.Add(WHRType.MechanicalOutputDrivetrain.ToString());
		}
		if ((eng.WHRType & WHRType.MechanicalOutputICE) != 0) {
			whrtypes.Add(WHRType.MechanicalOutputICE.ToString());
		}
		
		body.Add("WHRType", whrtypes.Count > 0 ? whrtypes : new[] {WHRType.None.ToString()}.ToList());

		var whrCF = new Dictionary<string, object>();
		if ((eng.WHRType & WHRType.ElectricalOutput) != 0) {
			whrCF.Add("Electrical", GetWhr(eng.EngineModes.First().WasteHeatRecoveryDataElectrical));
		}
		if ((eng.WHRType & WHRType.MechanicalOutputDrivetrain) != 0) {
			whrCF.Add("Mechanical", GetWhr(eng.EngineModes.First().WasteHeatRecoveryDataMechanical));
		}

		body.Add("WHRCorrectionFactors", whrCF);
		WriteFile(header, body, filename);
	}

	private Dictionary<string, object> GetWhr(IWHRData whrInput)
	{
		return new Dictionary<string, object> {
			{ "Urban", whrInput.UrbanCorrectionFactor },
			{ "Rural", whrInput.RuralCorrectionFactor },
			{ "Motorway", whrInput.MotorwayCorrectionFactor },
			{ "ColdHotBalancingFactor", whrInput.BFColdHot },
			{ "CFRegPer", whrInput.CFRegPer },
			{ "EngineeringCorrectionFactor", whrInput.EngineeringCorrectionFactor }
		};
	}

	protected Dictionary<string, object> GetHeader(int fileVersion)
	{
		var header = new Dictionary<string, object>();

		header.Add("CreatedBy", "");
		header.Add("Date", DateTime.Now.ToUniversalTime().ToString("o"));
		header.Add("AppVersion", VECTOvers);
		header.Add("FileVersion", fileVersion);
		return header;
	}

	public void SaveGearbox(
		IGearboxEngineeringInputData gbx, IAxleGearInputData axl, ITorqueConverterEngineeringInputData torqueConverter,
		IGearshiftEngineeringInputData gshift, string filename, bool DeclMode)
	{
		// Header
		var header = GetHeader(GearboxFormatVersion);

		// Body
		var body = new Dictionary<string, object>();

		body.Add(JsonKeys.SavedInDeclMode, DeclMode);
		body.Add(JsonKeys.Gearbox_ModelName, gbx.Model);
		body.Add(JsonKeys.Gearbox_Inertia, gbx.Inertia.Value());
		body.Add(JsonKeys.Gearbox_TractionInterruption, gbx.TractionInterruption.Value());

		var ls = new List<Dictionary<string, object>>();
		var axlgDict = new Dictionary<string, object>();
		axlgDict.Add(JsonKeys.Gearbox_Gear_Ratio, axl.Ratio);
		if (axl.LossMap == null)
			axlgDict.Add(JsonKeys.Gearbox_Gear_Efficiency, axl.Efficiency);
		else
			axlgDict.Add(
				JsonKeys.Gearbox_Gear_LossMapFile, GetRelativePath(axl.LossMap.Source, Path.GetDirectoryName(filename)));
		ls.Add(axlgDict);

		foreach (var gear in gbx.Gears) {
			var gearDict = new Dictionary<string, object>();
			gearDict.Add(JsonKeys.Gearbox_Gear_Ratio, gear.Ratio);
			if (gear.LossMap == null)
				gearDict.Add(JsonKeys.Gearbox_Gear_Efficiency, gear.Efficiency);
			else
				gearDict.Add(
					JsonKeys.Gearbox_Gear_LossMapFile, GetRelativePath(gear.LossMap.Source, Path.GetDirectoryName(filename)));
			gearDict.Add(
				JsonKeys.Gearbox_Gear_ShiftPolygonFile,
				!gbx.SavedInDeclarationMode && gear.ShiftPolygon != null
					? GetRelativePath(gear.ShiftPolygon.Source, Path.GetDirectoryName(filename))
					: "");
			gearDict.Add("MaxTorque", gear.MaxTorque == null ? "" : gear.MaxTorque.Value().ToString());
			gearDict.Add("MaxSpeed", gear.MaxInputSpeed == null ? "" : gear.MaxInputSpeed.AsRPM.ToString());

			ls.Add(gearDict);
		}

		body.Add(JsonKeys.Gearbox_Gears, ls);
		body.Add(JsonKeys.Gearbox_TorqueReserve, gshift.TorqueReserve * 100);
		body.Add(JsonKeys.Gearbox_ShiftTime, gshift.MinTimeBetweenGearshift.Value());
		body.Add(JsonKeys.Gearbox_StartTorqueReserve, gshift.StartTorqueReserve * 100);
		body.Add(JsonKeys.Gearbox_StartSpeed, gshift.StartSpeed.Value());
		body.Add(JsonKeys.Gearbox_StartAcceleration, gshift.StartAcceleration.Value());
		body.Add(JsonKeys.Gearbox_GearboxType, gbx.Type.ToString());

		var torqueConverterDict = new Dictionary<string, object>();
		torqueConverterDict.Add("Enabled", torqueConverter != null && gbx.Type.AutomaticTransmission());
		if (gbx.Type.AutomaticTransmission() && torqueConverter != null) {
			torqueConverterDict.Add("File", GetRelativePath(torqueConverter.TCData.Source, Path.GetDirectoryName(filename)));
			torqueConverterDict.Add(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM, torqueConverter.ReferenceRPM.AsRPM);
			torqueConverterDict.Add(JsonKeys.Gearbox_TorqueConverter_Inertia, torqueConverter.Inertia.Value());
			torqueConverterDict.Add("MaxTCSpeed", torqueConverter.MaxInputSpeed.AsRPM);
			torqueConverterDict.Add(
				"ShiftPolygon",
				!gbx.SavedInDeclarationMode && torqueConverter.ShiftPolygon != null
					? GetRelativePath(torqueConverter.ShiftPolygon.Source, Path.GetDirectoryName(filename))
					: "");
			torqueConverterDict.Add("CLUpshiftMinAcceleration", gshift.CLUpshiftMinAcceleration.Value());
			torqueConverterDict.Add("CCUpshiftMinAcceleration", gshift.CCUpshiftMinAcceleration.Value());
		}
		body.Add(JsonKeys.Gearbox_TorqueConverter, torqueConverterDict);

		body.Add("DownshiftAfterUpshiftDelay", gshift.DownshiftAfterUpshiftDelay.Value());
		body.Add("UpshiftAfterDownshiftDelay", gshift.UpshiftAfterDownshiftDelay.Value());
		body.Add("UpshiftMinAcceleration", gshift.UpshiftMinAcceleration.Value());

		body.Add("PowershiftShiftTime", gbx.PowershiftShiftTime.Value());

		WriteFile(header, body, filename);
	}

	public void SaveVehicle(
		IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag, IRetarderInputData retarder,
		IPTOTransmissionInputData pto, IAngledriveInputData angledrive, string filename, bool DeclMode)
	{
		//switch (vehicle.VehicleType) {
		//	case VectoSimulationJobType.ConventionalVehicle:
				SaveConventionalVehicle(vehicle, airdrag, retarder, pto, angledrive, filename, DeclMode);
		//		break;
		//	case VectoSimulationJobType.ParallelHybridVehicle:
		//		SaveHybridVehicle(vehicle, airdrag, retarder, pto, angledrive, filename, DeclMode);
		//		break;
		//	case VectoSimulationJobType.BatteryElectricVehicle:
		//		SaveBatteryElectricVehicle(vehicle, airdrag, retarder, pto, angledrive, filename, DeclMode);
		//		break;
		//	case VectoSimulationJobType.EngineOnlySimulation:
		//		break;
		//	default:
		//		throw new ArgumentOutOfRangeException();
		//}
	}


	public void SaveConventionalVehicle(
		IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag, IRetarderInputData retarder,
		IPTOTransmissionInputData pto, IAngledriveInputData angledrive, string filename, bool DeclMode)
	{

		var basePath = Path.GetDirectoryName(filename);

		// Header
		var header = GetHeader(VehicleFormatVersion);

		// Body
		var retarderOut = GetRetarderOut(retarder, basePath);

		var ptoOut = GetPTOOut(pto, basePath);

		var angledriveOut = GetAngledriveOut(angledrive, basePath);

		var torqueLimits = GetTorqueLimits(vehicle);

		var body = GetVehicle(vehicle, airdrag, DeclMode, basePath);

		body.Add("IdlingSpeed", vehicle.EngineIdleSpeed.AsRPM);
		body.Add("Retarder", retarderOut);
		body.Add("Angledrive", angledriveOut);
		body.Add("PTO", ptoOut);
		body.Add("TorqueLimits", torqueLimits);
		
		if ((vehicle.TankSystem.HasValue))
			body["TankSystem"] = vehicle.TankSystem.Value.ToString();

		WriteFile(header, body, filename);
	}

	private static Dictionary<string, object> GetVehicle(IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag,
		bool DeclMode, string basePath)
	{
		var body = new Dictionary<string, object>() {
			{ "SavedInDeclMode", DeclMode },
			{ "VehCat", vehicle.VehicleCategory.ToString() },
			{ "LegislativeClass", vehicle.LegislativeClass.ToString() },
			{ "CurbWeight", vehicle.CurbMassChassis.Value() },
			{ "CurbWeightExtra", vehicle.CurbMassExtra.Value() },
			{ "MassMax", vehicle.GrossVehicleMassRating.ConvertToTon().Value },
			{ "Loading", vehicle.Loading.Value() },
			{ "rdyn", vehicle.DynamicTyreRadius.ConvertToMilliMeter().Value },
			{ "CdCorrMode", airdrag.CrossWindCorrectionMode.GetName() }, {
				"CdCorrFile", (airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.SpeedDependentCorrectionFactor ||
								airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.VAirBetaLookupTable) &&
							airdrag.CrosswindCorrectionMap != null
					? GetRelativePath(airdrag.CrosswindCorrectionMap.Source, basePath)
					: ""
			}, {
				"AxleConfig", new Dictionary<string, object>() {
					{ "Type", vehicle.AxleConfiguration.GetName() }, {
						"Axles", from axle in vehicle.Components.AxleWheels.AxlesEngineering
						select new Dictionary<string, object>() {
							{ "Inertia", axle.Tyre.Inertia.Value() },
							{ "Wheels", axle.Tyre.Dimension },
							{ "AxleWeightShare", axle.AxleWeightShare },
							{ "TwinTyres", axle.TwinTyres },
							{ "RRCISO", axle.Tyre.RollResistanceCoefficient },
							{ "FzISO", axle.Tyre.TyreTestLoad.Value() },
							{ "Type", axle.AxleType.ToString() }
						}
					}
				}
			},
			{ "EngineStopStart", vehicle.ADAS.EngineStopStart },
			{ "EcoRoll", vehicle.ADAS.EcoRoll.ToString() },
			{ "PredictiveCruiseControl", vehicle.ADAS.PredictiveCruiseControl.ToString() }, {
				"ATEcoRollReleaseLockupClutch",
				vehicle.ADAS.ATEcoRollReleaseLockupClutch ?? false
			}
		};
		if (airdrag.AirDragArea != null)
			body["CdA"] = airdrag.AirDragArea.Value();
		if (vehicle.Height != null)
			body["VehicleHeight"] = vehicle.Height.Value();
		return body;
	}

	private static Dictionary<string, string> GetTorqueLimits(IVehicleEngineeringInputData vehicle)
	{
		var torqueLimits = new Dictionary<string, string>();
		foreach (var entry in vehicle.TorqueLimits)
			torqueLimits.Add(entry.Gear.ToString(), entry.MaxTorque.Value().ToString());
		return torqueLimits;
	}

	private static Dictionary<string, object> GetAngledriveOut(IAngledriveInputData angledrive, string basePath)
	{
		var angledriveOut = new Dictionary<string, object>() {
			{ "Type", angledrive.Type.ToString() },
			{ "Ratio", angledrive.Ratio }, {
				"LossMap",
				angledrive.Type == AngledriveType.SeparateAngledrive && angledrive.LossMap != null
					? GetRelativePath(angledrive.LossMap.Source, basePath)
					: ""
			}
		};
		return angledriveOut;
	}

	private static Dictionary<string, object> GetPTOOut(IPTOTransmissionInputData pto, string basePath)
	{
		var ptoOut = new Dictionary<string, object>();
		if (pto == null)
			ptoOut.Add("Type", "None");
		else {
			ptoOut.Add("Type", pto.PTOTransmissionType);
			ptoOut.Add("LossMap",
				pto.PTOTransmissionType != "None" && pto.PTOLossMap != null
					? GetRelativePath(pto.PTOLossMap.Source, basePath)
					: "");
			ptoOut.Add("Cycle",
				pto.PTOTransmissionType != "None" && pto.PTOCycleDuringStop != null
					? GetRelativePath(pto.PTOCycleDuringStop.Source, basePath)
					: "");
			ptoOut.Add("CycleDriving",
				pto.PTOTransmissionType != "None" && pto.PTOCycleWhileDriving != null
					? GetRelativePath(pto.PTOCycleWhileDriving.Source, basePath)
					: "");
		}

		return ptoOut;
	}

	private static Dictionary<string, object> GetRetarderOut(IRetarderInputData retarder, string basePath)
	{
		var retarderOut = new Dictionary<string, object>();
		if (retarder == null)
			retarderOut.Add("Type", RetarderType.None.GetName());
		else {
			retarderOut.Add("Type", retarder.Type.GetName());
			retarderOut.Add("Ratio", retarder.Ratio);
			retarderOut.Add(
				"File",
				retarder.Type.IsDedicatedComponent() && retarder.LossMap != null
					? GetRelativePath(retarder.LossMap.Source, basePath)
					: "");
		}

		return retarderOut;
	}

	
	public void SaveJob(IEngineeringInputDataProvider input, string filename, bool DeclMode)
	{

        switch (input.JobInputData.JobType) {
            case VectoSimulationJobType.ConventionalVehicle:
                SaveConventionalJob(input, filename, DeclMode);
                break;
            //	case VectoSimulationJobType.ParallelHybridVehicle:
            //		SaveHybridJob(input, filename, DeclMode);
            //		break;
            //	case VectoSimulationJobType.BatteryElectricVehicle:
            //		SaveBatteryElectricJob(input, filename, DeclMode);
            //		break;
            case VectoSimulationJobType.EngineOnlySimulation:
                SaveEngineOnlyJob(input, filename, DeclMode);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

	public void SaveEngineOnlyJob(IEngineeringInputDataProvider input, string filename, bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);

		// Header
		var header = GetHeader(VectoJobFormatVersion);

		// Body
		var body = new Dictionary<string, object>();

		// SavedInDeclMode = Cfg.DeclMode

		var job = input.JobInputData;

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);
		body.Add("EngineOnlyMode", job.JobType == VectoSimulationJobType.EngineOnlySimulation);

		
			body.Add("EngineFile", GetRelativePath(job.EngineOnly.DataSource.SourceFile, basePath));
			body.Add(
				"Cycles",
				job.Cycles.Select(x => GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray());
	}


	public void SaveConventionalJob(IEngineeringInputDataProvider input, string filename, bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);

		// Header
		var header = GetHeader(VectoJobFormatVersion);

		// Body
		var body = new Dictionary<string, object>();

		// SavedInDeclMode = Cfg.DeclMode

		var job = input.JobInputData;

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);
		body.Add("EngineOnlyMode", job.JobType == VectoSimulationJobType.EngineOnlySimulation);

		// Main Files
		body.Add("VehicleFile", GetRelativePath(job.Vehicle.DataSource.SourceFile, basePath));
		body.Add(
			"EngineFile",
			GetRelativePath(input.JobInputData.Vehicle.Components.EngineInputData.DataSource.SourceFile, basePath));
		body.Add(
			"GearboxFile",
			GetRelativePath(input.JobInputData.Vehicle.Components.GearboxInputData.DataSource.SourceFile, basePath));

		if (!job.SavedInDeclarationMode) {
			body.Add("TCU", GetRelativePath(input.DriverInputData.GearshiftInputData.Source, basePath));
			
		}
		body.Add("ShiftStrategy", input.JobInputData.ShiftStrategy);


        if (job.SavedInDeclarationMode && job.Vehicle is IVehicleDeclarationInputData declVehicle) {
            var aux = declVehicle.Components.AuxiliaryInputData;
			var auxList = new List<object>();
			foreach (var auxEntry in aux.Auxiliaries) {
				var auxOut = new Dictionary<string, object> {
					{ "ID", auxEntry.Type.Key() },
					{ "Type", auxEntry.Type.Name() },
					{ "Technology", auxEntry.Technology }
				};
				auxList.Add(auxOut);
			}

			body.Add("Aux", auxList);
        }

		if (!job.SavedInDeclarationMode && job.Vehicle is IVehicleEngineeringInputData engVehicle) {
			var aux = engVehicle.Components.AuxiliaryInputData;
			if (aux.BusAuxiliariesData != null) {
				body.Add("BusAux",
					GetRelativePath(job.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData.DataSource.SourceFile,
						basePath));
			}

			body.Add("Padd", aux.Auxiliaries.ConstantPowerDemand.Value());
			body.Add("Paux_ICEOff_Driving", aux.Auxiliaries.PowerDemandICEOffDriving.Value());
			body.Add("Paux_ICEOff_Standstill", aux.Auxiliaries.PowerDemandICEOffStandstill.Value());

		}

		var driver = input.DriverInputData;

		if (!job.SavedInDeclarationMode) {
			body.Add("VACC", GetRelativePath(driver.AccelerationCurve.AccelerationCurve.Source, basePath));
			body.Add("EngineStopStartAtVehicleStopThreshold", driver.EngineStopStartData.ActivationDelay.Value());
			body.Add("EngineStopStartMaxOffTimespan", driver.EngineStopStartData.MaxEngineOffTimespan.Value());
			body.Add("EngineStopStartUtilityFactor", driver.EngineStopStartData.UtilityFactorStandstill);
			body.Add("EngineStopStartUtilityFactorDriving", driver.EngineStopStartData.UtilityFactorDriving);

			body.Add("EcoRollMinSpeed", driver.EcoRollData.MinSpeed.AsKmph);
			body.Add("EcoRollActivationDelay", driver.EcoRollData.ActivationDelay.Value());
			body.Add("EcoRollUnderspeedThreshold", driver.EcoRollData.UnderspeedThreshold.AsKmph);

			body.Add("EcoRollMaxAcceleration", driver.EcoRollData.AccelerationUpperLimit.Value());
			body.Add("PCCEnableSpeed", driver.PCCData.PCCEnabledSpeed.AsKmph);
			body.Add("PCCMinSpeed", driver.PCCData.MinSpeed.AsKmph);
			body.Add("PCCUnderspeed", driver.PCCData.Underspeed.AsKmph);
			body.Add("PCCOverSpeed", driver.PCCData.OverspeedUseCase3.AsKmph);
			body.Add("PCCPreviewDistanceUC1", driver.PCCData.PreviewDistanceUseCase1.Value());
			body.Add("PCCPreviewDistanceUC2", driver.PCCData.PreviewDistanceUseCase2.Value());

		}

		// body.Add("StartStop", New Dictionary(Of String, Object) From {
		// {"Enabled", driver.StartStop.Enabled},
		// {"MaxSpeed", driver.StartStop.MaxSpeed.AsKmph},
		// {"MinTime", driver.StartStop.MinTime.Value()},
		// {"Delay", driver.StartStop.Delay.Value()}})
		if (!job.SavedInDeclarationMode) {
			var dfTargetSpeed =
				driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup != null &&
				File.Exists(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source)
					? GetRelativePath(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source, basePath)
					: "";
			var dfVelocityDrop =
				driver.Lookahead.CoastingDecisionFactorVelocityDropLookup != null &&
				File.Exists(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source)
					? GetRelativePath(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source, basePath)
					: "";
			body.Add(
				"LAC",
				new Dictionary<string, object>() {
					{ "Enabled", driver.Lookahead.Enabled },
					{ "PreviewDistanceFactor", driver.Lookahead.LookaheadDistanceFactor },
					{ "DF_offset", driver.Lookahead.CoastingDecisionFactorOffset },
					{ "DF_scaling", driver.Lookahead.CoastingDecisionFactorScaling },
					{ "DF_targetSpeedLookup", dfTargetSpeed },
					{ "Df_velocityDropLookup", dfVelocityDrop },
					{ "MinSpeed", driver.Lookahead.MinSpeed.AsKmph }
				});
		}

		// Overspeed / EcoRoll
		var overspeedDic = new Dictionary<string, object>();

		overspeedDic.Add("Mode", driver.OverSpeedData.Enabled ? "Overspeed" : "Off");

		overspeedDic.Add("MinSpeed", driver.OverSpeedData.MinSpeed.AsKmph);
		overspeedDic.Add("OverSpeed", driver.OverSpeedData.OverSpeed.AsKmph);
		body.Add("OverSpeedEcoRoll", overspeedDic);

		// Cycles
		if (!job.SavedInDeclarationMode)
			body.Add(
				"Cycles", job.Cycles.Select(x => GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray());

		WriteFile(header, body, filename);
	}

	public void SaveJob(IVTPDeclarationInputDataProvider input, string filename, bool DeclMode)
	{
		var header = GetHeader(VectoVTPJobFormatVersion);
		var body = SaveVTPJob(input.JobInputData, filename, true);
		WriteFile(header, body, filename);
	}

	public void SaveJob(IVTPEngineeringInputDataProvider input, string filename, bool DeclMode)
	{
		var header = GetHeader(VectoVTPJobFormatVersion);
		var body = SaveVTPJob(input.JobInputData, filename, false);
		WriteFile(header, body, filename);
	}

	private Dictionary<string, object> SaveVTPJob(IVTPDeclarationJobInputData job, string filename, bool declarationmode)
	{
		// Body
		var body = new Dictionary<string, object>();
		body.Add("SavedInDeclMode", declarationmode);
		body.Add("DeclarationVehicle", GetRelativePath(job.Vehicle.DataSource.SourceFile, Path.GetDirectoryName(filename)));
		if (declarationmode) {
			body.Add(
				"ManufacturerRecord", GetRelativePath(job.ManufacturerReportInputData.Source, Path.GetDirectoryName(filename)));
			body.Add("Mileage", job.Mileage.ConvertToKiloMeter().Value);
		}
		body.Add("FanPowerCoefficients", job.FanPowerCoefficents);
		body.Add("FanDiameter", job.FanDiameter.Value());
		body.Add(
			"Cycles", job.Cycles.Select(x => GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray());
		return body;
	}

	public void ExportJob(IEngineeringInputDataProvider input, string filename, bool separateFiles)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// 	''' Writes the Content variable into a JSON file.
	/// 	''' </summary>
	/// 	'''
	/// <param name="content"></param>
	/// <param name="path"></param>
	/// 	''' <remarks></remarks>
	public static void WriteFile(JToken content, string path)
	{
		
		string str;

		if (!content.Any())
			return;

		str = JsonConvert.SerializeObject(content, Formatting.Indented);
		File.WriteAllText(path, str);
	}

	public static void WriteFile(Dictionary<string, object> content, string path)
	{
		WriteFile(JToken.FromObject(content), path);
	}

	protected static void WriteFile(Dictionary<string, object> header, Dictionary<string, object> body, string path)
	{
		WriteFile(JToken.FromObject(new Dictionary<string, object>() { { "Header", header }, { "Body", body } }), path);
	}


	public void SaveBusAuxEngineeringParameters(IBusAuxiliariesEngineeringData busAux, string filePath, bool declMode)
	{
		var header = GetHeader(BusAuxEngParamsVersion);


		var ps = new Dictionary<string, object>() {
			{"CompressorMap", busAux.PneumaticSystem.CompressorMap != null ? GetRelativePath(busAux.PneumaticSystem.CompressorMap.Source, Path.GetDirectoryName(filePath)) : ""},
			{"AverageAirDemand", busAux.PneumaticSystem.AverageAirConsumed.Value()},
			{"SmartAirCompression", busAux.PneumaticSystem.SmartAirCompression},
			{"GearRatio", busAux.PneumaticSystem.GearRatio},
		};
		var es = new Dictionary<string, object>() {
			{"AlternatorEfficiency", busAux.ElectricSystem.AlternatorEfficiency},
			{"CurrentDemand", busAux.ElectricSystem.CurrentDemand.Value()},
			{"CurrentDemandEngineOffDriving", busAux.ElectricSystem.CurrentDemandEngineOffDriving.Value()},
			{"CurrentDemandEngineOffStandstill", busAux.ElectricSystem.CurrentDemandEngineOffStandstill.Value()},
			{"AlternatorType", busAux.ElectricSystem.AlternatorType.ToString()},
			{"ElectricStorageCapacity", busAux.ElectricSystem.ElectricStorageCapacity.ConvertToWattHour().Value},
			{"BatteryEfficiency", busAux.ElectricSystem.ElectricStorageEfficiency},
			{"MaxAlternatorPower", busAux.ElectricSystem.MaxAlternatorPower.Value()},
			{"DCDCConverterEfficiency", busAux.ElectricSystem.DCDCConverterEfficiency},
			{"ESSupplyFromHEVREESS", busAux.ElectricSystem.ESSupplyFromHEVREESS}
		};
		var hvac = new Dictionary<string, object>() {
			{"ElectricPowerDemand", busAux.HVACData.ElectricalPowerDemand.Value()},
			{"MechanicalPowerDemand", busAux.HVACData.MechanicalPowerDemand.Value()},
			{"AuxHeaterPower", busAux.HVACData.AuxHeaterPower.Value()},
			{"AverageHeatingDemand", busAux.HVACData.AverageHeatingDemand.Value() / 1e6}
		};

		var body = new Dictionary<string, object>() {
			{"PneumaticSystem", ps},
			{"ElectricSystem", es},
			{ "HVAC", hvac}
		};
		WriteFile(header, body, filePath);
	}
}

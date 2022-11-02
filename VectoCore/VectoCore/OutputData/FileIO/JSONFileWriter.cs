using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
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

	#region Vehicle File Version Numbers

	public const int VehicleFormatVersion = 8;
	public const int BusVehicleFormatVersion = 9;
	public const int HEV_BEVVehicleFormatVersion = 10;
	public const int IEPCVehicleFormatVersion = 11;

	#endregion

	#region Job File Version Numbers

	private const int VectoVTPJobFormatVersion = 4;
	private const int VectoJobFormatVersion = 5;
	private const int PHEVVectoJobFormatVersion = 8;
	private const int BEVVectoJobFormatVersion = 9;
	private const int SHEVVectoJobFormatVersion = 11;

	private const int IEPCVectoJobFormatVersion = 12;
	private const int IHPCVectoJobFormatVersion = 13;
	#endregion


	private const int ElectricMotorFormatVersion = 5;
	
	private const int IEPCFormatVersion = 1;

	private const int REESSFormatVersion = 1;

	private const int HybridStrategyParamsVersion = 1;

	private static JSONFileWriter _instance;

	public const string VECTOvers = "3";

	public static JSONFileWriter Instance => _instance ?? (_instance = new JSONFileWriter());

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

	public void SaveElectricMotor(IElectricMotorEngineeringInputData electricMachine, string filename, bool declMode)
	{
		var header = GetHeader(ElectricMotorFormatVersion);

		var body = new Dictionary<string, object> {
			{ JsonKeys.SavedInDeclMode, declMode },
			{ JsonKeys.Component_Model, electricMachine.Model },
			{ JsonKeys.Engine_Inertia, electricMachine.Inertia.Value() },
			{ JsonKeys.EM_ThermalOverloadRecoveryFactor, electricMachine.OverloadRecoveryFactor }
		};

		var vlevels = GetVoltageLevelEntries(electricMachine.VoltageLevels, filename);
		body.Add(JsonKeys.EM_DragCurve, GetRelativePath(electricMachine.DragCurve.Source, Path.GetDirectoryName(filename)));

		body.Add(JsonKeys.EM_VoltageLevels, vlevels);
		WriteFile(header, body, filename);
	}
	
	public void SaveIHPC(IElectricMotorEngineeringInputData electricMachine, string filename, bool declMode)
	{
		var header = GetHeader(ElectricMotorFormatVersion);
		
		var body = new Dictionary<string, object> {
			{ JsonKeys.SavedInDeclMode, declMode },
			{ JsonKeys.Component_Model, electricMachine.Model },
			{ JsonKeys.EM_DragCurve,  GetRelativePath(electricMachine.DragCurve.Source, Path.GetDirectoryName(filename))},
			{ JsonKeys.Engine_Inertia, electricMachine.Inertia.Value() },
			{ JsonKeys.EM_ThermalOverloadRecoveryFactor, electricMachine.OverloadRecoveryFactor }
		};
		
		var vlevels = GetVoltageLevelEntries(electricMachine.VoltageLevels, filename);
		body.Add(JsonKeys.EM_VoltageLevels, vlevels);
		WriteFile(header, body, filename);
	}
	
	private List<Dictionary<string, object>> GetVoltageLevelEntries(IList<IElectricMotorVoltageLevel> voltageLevels, string filename)
	{
		var vlevels = new List<Dictionary<string, object>>();
		foreach (var entry in voltageLevels.OrderBy(x => x.VoltageLevel))
		{
			var vlevel = new Dictionary<string, object> {
				{ JsonKeys.EM_Voltage, entry.VoltageLevel.Value() },
				{ JsonKeys.EM_ContinuousTorque, entry.ContinuousTorque.Value() },
				{ JsonKeys.EM_ContinuousTorqueSpeed, entry.ContinuousTorqueSpeed.AsRPM },
				{ JsonKeys.EM_OverloadTorque, entry.OverloadTorque.Value() },
				{ JsonKeys.EM_OverloadTorqueSpeed, entry.OverloadTestSpeed.AsRPM },
				{ JsonKeys.EM_OverloadTime, entry.OverloadTime.Value() },
				{ JsonKeys.EM_FullLoadCurve, GetRelativePath(entry.FullLoadCurve.Source, Path.GetDirectoryName(filename)) }
			};
			var powerMaps = new Dictionary<int, object>();
			foreach (var pMap in entry.PowerMap.OrderBy(x => x.Gear))
			{
				powerMaps.Add(pMap.Gear, GetRelativePath(pMap.PowerMap.Source, Path.GetDirectoryName(filename)));
			}
			vlevel.Add(JsonKeys.EM_EfficiencyMap, powerMaps); //PowerMap
			vlevels.Add(vlevel);
		}
		
		return vlevels;
	}


	public void SaveIEPC(IIEPCEngineeringInputData iepc, string filename, bool declMode)
	{
		var header = GetHeader(IEPCFormatVersion);

		var body = new Dictionary<string, object>
		{
			{JsonKeys.SavedInDeclMode, declMode},
			{JsonKeys.Component_Model, iepc.Model},
			{JsonKeys.IEPC_Inertia, iepc.Inertia.Value()},
			{JsonKeys.IEPC_DifferentialIncluded, iepc.DifferentialIncluded},
			{JsonKeys.IEPC_DesignTypeWheelMotor, iepc.DesignTypeWheelMotor},
			{JsonKeys.IEPC_NrOfDesignTypeWheelMotorMeasured, iepc.NrOfDesignTypeWheelMotorMeasured},
			{JsonKeys.IEPC_ThermalOverloadRecoveryFactor, iepc.OverloadRecoveryFactor}
		};

		var gears = new List<Dictionary<string, object>>();

		foreach (var gear in iepc.Gears.OrderBy(x => x.GearNumber)) {
			var currentGear = new Dictionary<string, object> {
				{ JsonKeys.Gearbox_Gear_Ratio, gear.Ratio }
			};
			if(gear.MaxOutputShaftTorque != null)
				currentGear.Add(JsonKeys.Gearbox_Gear_MaxOutShaftTorque, gear.MaxOutputShaftTorque.Value());
			if(gear.MaxOutputShaftSpeed != null)
				currentGear.Add(JsonKeys.Gearbox_Gear_MaxOutShaftSpeed, gear.MaxOutputShaftSpeed.AsRPM);
			gears.Add(currentGear);
		}

		var voltageLevels = new List<Dictionary<string, object>>();
		foreach (var voltageLevel in iepc.VoltageLevels.OrderBy(x => x.VoltageLevel)) {
			var currentLevel = new Dictionary<string, object>
			{
				{JsonKeys.IEPC_Voltage, voltageLevel.VoltageLevel.Value()},
				{JsonKeys.IEPC_ContinuousTorque, voltageLevel.ContinuousTorque.Value()},
				{JsonKeys.IEPC_ContinuousTorqueSpeed, Convert.ToDouble(voltageLevel.ContinuousTorqueSpeed.AsRPM.ToString())},
				{JsonKeys.IEPC_OverloadTorque, voltageLevel.OverloadTorque.Value()},
				{JsonKeys.IEPC_OverloadTorqueSpeed,Convert.ToDouble(voltageLevel.OverloadTestSpeed.AsRPM.ToString())},
				{JsonKeys.IEPC_OverloadTime, voltageLevel.OverloadTime.Value()},
				{JsonKeys.IEPC_FullLoadCurve, GetRelativePath(voltageLevel.FullLoadCurve.Source, Path.GetDirectoryName(filename))},

			};
			var powerMaps = new Dictionary<string, object>();
			foreach (var pMap in voltageLevel.PowerMap.OrderBy(x => x.Gear))
			{
				powerMaps.Add(pMap.Gear.ToString(), GetRelativePath(pMap.PowerMap.Source, Path.GetDirectoryName(filename)));
			}
			currentLevel.Add(JsonKeys.IEPC_PowerMaps, powerMaps); //PowerMap
			voltageLevels.Add(currentLevel);
		}

		var dragCurves = new Dictionary<string, object>();
		foreach (var dragCurveEntry in iepc.DragCurves.OrderBy(x => x.Gear)) {
			dragCurves.Add(dragCurveEntry.Gear.ToString(), 
				GetRelativePath(dragCurveEntry.DragCurve.Source, Path.GetDirectoryName(filename)));
		}
		
		body.Add(JsonKeys.Gearbox_Gears, gears);
		body.Add(JsonKeys.IEPC_VoltageLevels, voltageLevels);
		body.Add(JsonKeys.IEPC_DragCurves, dragCurves);
		
		WriteFile(header, body, filename);
	}
	
	public void SaveBattery(IBatteryPackEngineeringInputData battery, string filename, bool declMode)
	{
		var header = GetHeader(REESSFormatVersion);

		var body = new Dictionary<string, object> {
			{ "SavedInDeclMode", declMode },
			{ "REESSType", "Battery" },
			{ "Model", battery.Model },
			{ "Capacity", battery.Capacity.AsAmpHour },
			{ "SOC_min", battery.MinSOC * 100.0 },
			{ "SOC_max", battery.MaxSOC * 100.0 },
			{ "MaxCurrentMap", GetRelativePath(battery.MaxCurrentMap.Source, Path.GetDirectoryName(filename)) },
			{ "InternalResistanceCurve", GetRelativePath(battery.InternalResistanceCurve.Source, Path.GetDirectoryName(filename)) },
			{ "SoCCurve", GetRelativePath(battery.VoltageCurve.Source, Path.GetDirectoryName(filename)) }
		};

		WriteFile(header, body, filename);
	}

	public void SaveSuperCap(ISuperCapEngineeringInputData superCap, string filename, bool declMode)
	{
		var header = GetHeader(REESSFormatVersion);

		var body = new Dictionary<string, object> {
			{ "SavedInDeclMode", declMode },
			{ "REESSType", "SuperCap" },
			{ "Model", superCap.Model },
			{ "Capacity", superCap.Capacity.Value() },
			{ "InternalResistance", superCap.InternalResistance.Value() },
			{ "U_min", superCap.MinVoltage.Value() },
			{ "U_max", superCap.MaxVoltage.Value() },
			{ "I_maxCharge", superCap.MaxCurrentCharge.Value() },
			{ "I_maxDischarge", superCap.MaxCurrentDischarge.Value() }
		};

		WriteFile(header, body, filename);
	}

	public void SaveEngine(IEngineEngineeringInputData eng, string filename, bool DeclMode)
	{
		var header = GetHeader(EngineFormatVersion);
		var body = new Dictionary<string, object> {
			{ "SavedInDeclMode", DeclMode },
			{ "ModelName", eng.Model },
			{ "Displacement", eng.Displacement.ConvertToCubicCentiMeter().ToString() },
			{ "IdlingSpeed", eng.EngineModes.First().IdleSpeed.AsRPM },
			{ "Inertia", eng.Inertia.Value() }
		};

		var fuels = new List<object>();

		foreach (var fuel in eng.EngineModes.First().Fuels) {
			var entry = new Dictionary<string, object> {
				{ "WHTC-Urban", fuel.WHTCUrban },
				{ "WHTC-Rural", fuel.WHTCRural },
				{ "WHTC-Motorway", fuel.WHTCMotorway },
				{ "WHTC-Engineering", fuel.WHTCEngineering },
				{ "ColdHotBalancingFactor", fuel.ColdHotBalancingFactor },
				{ "CFRegPer", fuel.CorrectionFactorRegPer },
				{ "FuelMap", GetRelativePath(fuel.FuelConsumptionMap.Source, Path.GetDirectoryName(filename)) },
				{ "FuelType", fuel.FuelType.ToString() }
			};

			fuels.Add(entry);
		}

		body.Add("Fuels", fuels);
		body.Add("RatedPower", eng.RatedPowerDeclared.Value());
		body.Add("RatedSpeed", eng.RatedSpeedDeclared.AsRPM);
		body.Add("MaxTorque", eng.MaxTorqueDeclared.Value());

		body.Add("FullLoadCurve", GetRelativePath(eng.EngineModes.First().FullLoadCurve.Source, Path.GetDirectoryName(filename)));

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

		body.Add("WHRType", whrtypes.Count > 0 ? whrtypes : new[] { WHRType.None.ToString() }.ToList());

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

	private Dictionary<string, object> GetWhr(IWHRData whrInput) =>
		new Dictionary<string, object> {
			{ "Urban", whrInput.UrbanCorrectionFactor },
			{ "Rural", whrInput.RuralCorrectionFactor },
			{ "Motorway", whrInput.MotorwayCorrectionFactor },
			{ "ColdHotBalancingFactor", whrInput.BFColdHot },
			{ "CFRegPer", whrInput.CFRegPer },
			{ "EngineeringCorrectionFactor", whrInput.EngineeringCorrectionFactor }
		};

	protected Dictionary<string, object> GetHeader(int fileVersion) =>
		new Dictionary<string, object> {
			{ JsonKeys.JsonHeader_CreatedBy, "" },
			{ JsonKeys.JsonHeader_Date , DateTime.Now.ToUniversalTime().ToString("o") },
			{ JsonKeys.AppVersion, VECTOvers },
			{ JsonKeys.JsonHeader_FileVersion, fileVersion }
		};

	public void SaveGearbox(IGearboxEngineeringInputData gbx, IAxleGearInputData axl,
		ITorqueConverterEngineeringInputData torqueConverter, IGearshiftEngineeringInputData gshift, string filename,
		bool DeclMode)
	{
		var header = GetHeader(GearboxFormatVersion);
		var body = new Dictionary<string, object> {
			{ JsonKeys.SavedInDeclMode, DeclMode },
			{ JsonKeys.Gearbox_ModelName, gbx.Model },
			{ JsonKeys.Gearbox_Inertia, gbx.Inertia.Value() },
			{ JsonKeys.Gearbox_TractionInterruption, gbx.TractionInterruption.Value() }
		};

		var ls = new List<Dictionary<string, object>>();
		var axlgDict = new Dictionary<string, object> { { JsonKeys.Gearbox_Gear_Ratio, axl.Ratio } };
		if (axl.LossMap == null)
			axlgDict.Add(JsonKeys.Gearbox_Gear_Efficiency, axl.Efficiency);
		else
			axlgDict.Add(JsonKeys.Gearbox_Gear_LossMapFile, GetRelativePath(axl.LossMap.Source, Path.GetDirectoryName(filename)));
		ls.Add(axlgDict);

		foreach (var gear in gbx.Gears) {
			var gearDict = new Dictionary<string, object> { { JsonKeys.Gearbox_Gear_Ratio, gear.Ratio } };
			if (gear.LossMap == null)
				gearDict.Add(JsonKeys.Gearbox_Gear_Efficiency, gear.Efficiency);
			else
				gearDict.Add(JsonKeys.Gearbox_Gear_LossMapFile, GetRelativePath(gear.LossMap.Source, Path.GetDirectoryName(filename)));
			gearDict.Add(JsonKeys.Gearbox_Gear_ShiftPolygonFile, !gbx.SavedInDeclarationMode && gear.ShiftPolygon != null
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

		var torqueConverterDict = new Dictionary<string, object> { { "Enabled", torqueConverter != null && gbx.Type.AutomaticTransmission() } };
		if (gbx.Type.AutomaticTransmission() && gbx.Type != GearboxType.APTN && gbx.Type != GearboxType.IHPC) {
			torqueConverterDict.Add("File", GetRelativePath(torqueConverter.TCData.Source, Path.GetDirectoryName(filename)));
			torqueConverterDict.Add(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM, Math.Round(torqueConverter.ReferenceRPM.AsRPM, 4));
			torqueConverterDict.Add(JsonKeys.Gearbox_TorqueConverter_Inertia, torqueConverter.Inertia.Value());
			torqueConverterDict.Add("MaxTCSpeed", torqueConverter.MaxInputSpeed.AsRPM);
			torqueConverterDict.Add("ShiftPolygon", !gbx.SavedInDeclarationMode && torqueConverter.ShiftPolygon != null
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
		switch (vehicle.VehicleType) {
			case VectoSimulationJobType.ConventionalVehicle:
				SaveConventionalVehicle(vehicle, airdrag, retarder, pto, angledrive, filename, DeclMode);
				break;
			case VectoSimulationJobType.ParallelHybridVehicle:
			case VectoSimulationJobType.SerialHybridVehicle:
			case VectoSimulationJobType.IHPC:
				SaveHybridVehicle(vehicle, airdrag, retarder, pto, angledrive, filename, DeclMode);
				break;
			case VectoSimulationJobType.BatteryElectricVehicle:
				SaveBatteryElectricVehicle(vehicle, airdrag, retarder, pto, angledrive, filename, DeclMode);
				break;
			case VectoSimulationJobType.EngineOnlySimulation:
				break;
			case VectoSimulationJobType.IEPC_E:
			case VectoSimulationJobType.IEPC_S:
				SaveIEPCVehicle(vehicle, airdrag, retarder, pto, angledrive, filename, DeclMode);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	
	public void SaveConventionalVehicle(
		IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag, IRetarderInputData retarder,
		IPTOTransmissionInputData pto, IAngledriveInputData angledrive, string filename, bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(VehicleFormatVersion);
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

		if (vehicle.TankSystem.HasValue) {
			body["TankSystem"] = vehicle.TankSystem.Value.ToString();
		}

		WriteFile(header, body, filename);
	}

	private static Dictionary<string, object> GetVehicle(IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag,
		bool DeclMode, string basePath)
	{
		var body = new Dictionary<string, object> {
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
				"AxleConfig", new Dictionary<string, object> {
					{ "Type", vehicle.AxleConfiguration.GetName() }, {
						"Axles", from axle in vehicle.Components.AxleWheels.AxlesEngineering
						select new Dictionary<string, object> {
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

	private static Dictionary<string, string> GetTorqueLimits(IVehicleEngineeringInputData vehicle) =>
		vehicle.TorqueLimits.ToDictionary(
			entry => entry.Gear.ToString(),
			entry => entry.MaxTorque.Value().ToString(CultureInfo.InvariantCulture));

	private static Dictionary<string, object> GetAngledriveOut(IAngledriveInputData angledrive, string basePath)
	{
		var angledriveOut = new Dictionary<string, object> {
			{ "Type", angledrive.Type.ToString() },
			{ "Ratio", angledrive.Ratio }, {
				"LossMap", angledrive.Type == AngledriveType.SeparateAngledrive && angledrive.LossMap != null
					? GetRelativePath(angledrive.LossMap.Source, basePath)
					: ""
			}
		};
		return angledriveOut;
	}

	private static Dictionary<string, object> GetPTOOut(IPTOTransmissionInputData pto, string basePath)
	{
		var ptoOut = new Dictionary<string, object>();
		if (pto == null) {
			ptoOut.Add("Type", "None");
		} else {
			ptoOut.Add("Type", pto.PTOTransmissionType);
			ptoOut.Add("LossMap", pto.PTOTransmissionType != "None" && pto.PTOLossMap != null
					? GetRelativePath(pto.PTOLossMap.Source, basePath)
					: "");
			ptoOut.Add("Cycle", pto.PTOTransmissionType != "None" && pto.PTOCycleDuringStop != null
					? GetRelativePath(pto.PTOCycleDuringStop.Source, basePath)
					: "");
			ptoOut.Add("CycleDriving", pto.PTOTransmissionType != "None" && pto.PTOCycleWhileDriving != null
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
			retarderOut.Add("File", retarder.Type.IsDedicatedComponent() && retarder.LossMap != null
					? GetRelativePath(retarder.LossMap.Source, basePath)
					: "");
		}

		return retarderOut;
	}

	public void SaveHybridVehicle(IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag,
		IRetarderInputData retarder, IPTOTransmissionInputData pto, IAngledriveInputData angledrive, string filename,
		bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(HEV_BEVVehicleFormatVersion);
		var body = GetVehicle(vehicle, airdrag, DeclMode, basePath);

		if (!vehicle.EngineIdleSpeed.IsEqual(0)) {
			body.Add("IdlingSpeed", vehicle.EngineIdleSpeed.AsRPM);
		}

		body.Add("Retarder", GetRetarderOut(retarder, basePath));

		if (angledrive.Type != AngledriveType.None) {
			body.Add("Angledrive", GetAngledriveOut(angledrive, basePath));
		}

		if (pto != null && pto.PTOTransmissionType != "None") {
			body.Add("PTO", GetPTOOut(pto, basePath));
		}

		if (vehicle.TorqueLimits.Any()) {
			body.Add("TorqueLimits", GetTorqueLimits(vehicle));
		}

		if (vehicle.TankSystem.HasValue) {
			body["TankSystem"] = vehicle.TankSystem.Value.ToString();
		}

		//body.Add(JsonKeys.HEV_Vehicle_MaxDrivetrainPower, vehicle.MaxDrivetrainPower.ConvertToKiloWatt().Value);

		//ToDo ElectricMotorTorqueLimits changed
		// if (vehicle.ElectricMotorTorqueLimits != null) {
		// 	body.Add("EMTorqueLimits", GetRelativePath(vehicle.ElectricMotorTorqueLimits.Source, basePath));
		// }
		if (vehicle.BoostingLimitations != null) {
			body.Add("MaxPropulsionTorque", GetRelativePath(vehicle.BoostingLimitations.Source, basePath));
		}

		body.Add("InitialSoC", vehicle.InitialSOC * 100);
		body.Add("PowertrainConfiguration", vehicle.VehicleType == VectoSimulationJobType.SerialHybridVehicle ? "SerialHybrid" : vehicle.VehicleType == VectoSimulationJobType.IHPC ? "IHPC":  "ParallelHybrid");
		body.Add("ElectricMotors", GetElectricMotors(vehicle, basePath));
		body.Add("Batteries", GetBattery(vehicle, basePath));

		WriteFile(header, body, filename);
	}

	public void SaveBatteryElectricVehicle(
		IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag, IRetarderInputData retarder,
		IPTOTransmissionInputData pto, IAngledriveInputData angledrive, string filename, bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(HEV_BEVVehicleFormatVersion);
		var retarderOut = GetRetarderOut(retarder, basePath);

        var ptoOut = GetPTOOut(pto, basePath);

        //var angledriveOut = GetAngledriveOut(angledrive, basePath);

        //var torqueLimits = GetTorqueLimits(vehicle);

        var electricMotorsOut = GetElectricMotors(vehicle, basePath);
		var battery = GetBattery(vehicle, basePath);
		var body = GetVehicle(vehicle, airdrag, DeclMode, basePath);
		body.Add("InitialSoC", vehicle.InitialSOC * 100);
		body.Add("PowertrainConfiguration", "BatteryElectric");
		body.Add("ElectricMotors", electricMotorsOut);
		body.Add("Batteries", battery);

		//body.Add("IdlingSpeed", vehicle.EngineIdleSpeed.AsRPM);
		if (retarder.Type != RetarderType.None)
			body.Add("Retarder", retarderOut);
        //body.Add("Angledrive", angledriveOut);
        body.Add("PTO", ptoOut);
        //body.Add("TorqueLimits", torqueLimits);

        if ((vehicle.TankSystem.HasValue))
			body["TankSystem"] = vehicle.TankSystem.Value.ToString();

		WriteFile(header, body, filename);
	}

	private void SaveIEPCVehicle(IVehicleEngineeringInputData vehicle, IAirdragEngineeringInputData airdrag,
		IRetarderInputData retarder, IPTOTransmissionInputData pto, IAngledriveInputData angledrive, string filename,
		bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(IEPCVehicleFormatVersion);
		var retarderOut = GetRetarderOut(retarder, basePath);

		var ptoOut = GetPTOOut(pto, basePath);

		//var angledriveOut = GetAngledriveOut(angledrive, basePath);

		//var torqueLimits = GetTorqueLimits(vehicle);

		var electricMotorsOut = vehicle.VehicleType == VectoSimulationJobType.IEPC_S ? GetElectricMotors(vehicle, basePath) : null;
		var battery = GetBattery(vehicle, basePath);
		var body = GetVehicle(vehicle, airdrag, DeclMode, basePath);
		body.Add("InitialSoC", vehicle.InitialSOC * 100);
		body.Add("PowertrainConfiguration", vehicle.VehicleType.ToString());
		body.Add("IEPC", vehicle.Components.IEPCEngineeringInputData.DataSource.SourceFile);
		if (electricMotorsOut != null) {
			body.Add("ElectricMotors", electricMotorsOut);
		}

		body.Add("Batteries", battery);

		//body.Add("IdlingSpeed", vehicle.EngineIdleSpeed.AsRPM);
		if (retarder.Type != RetarderType.None)
			body.Add("Retarder", retarderOut);
		//body.Add("Angledrive", angledriveOut);
		body.Add("PTO", ptoOut);
		//body.Add("TorqueLimits", torqueLimits);

		if ((vehicle.TankSystem.HasValue))
			body["TankSystem"] = vehicle.TankSystem.Value.ToString();

		WriteFile(header, body, filename);
	}

	private Dictionary<string, object>[] GetBattery(IVehicleEngineeringInputData vehicle, string basePath) =>
		vehicle.Components.ElectricStorage.ElectricStorageElements.Select(
			entry => new Dictionary<string, object> {
				{ "NumPacks", entry.Count },
				{ "BatteryFile", GetRelativePath(entry.REESSPack.DataSource.SourceFile, basePath) },
				{ "StreamId", entry.StringId } }).ToArray();

	private Array GetElectricMotors(IVehicleEngineeringInputData vehicle, string basePath)
	{
		//var em = vehicle.Components.ElectricMachines.Entries.First();
		return vehicle.Components.ElectricMachines.Entries.Select(em => {
			var d = new Dictionary<string, object> {
				{ "Count", em.Count },
				{ "Ratio", em.RatioADC },
				{ "Position", em.Position.GetName() },
				{ "MotorFile", GetRelativePath(em.ElectricMachine.DataSource.SourceFile, basePath) }
			};
			if (!double.IsNaN(em.MechanicalTransmissionEfficiency)) {
				d["MechanicalEfficiency"] = em.MechanicalTransmissionEfficiency;

			}

			if (em.MechanicalTransmissionLossMap != null) {
				d["MechanicalTransmissionLossMap"] = GetRelativePath(em.MechanicalTransmissionLossMap.Source, basePath);

			}

			if (em.Position == PowertrainPosition.HybridP2_5) {
				d["RatioPerGear"] = em.RatioPerGear;
			}
			return d;
		}).ToArray();
	}


	public void SaveJob(IEngineeringInputDataProvider input, string filename, bool DeclMode)
	{

		switch (input.JobInputData.JobType) {
			case VectoSimulationJobType.ConventionalVehicle:
				SaveConventionalJob(input, filename, DeclMode);
				break;
			case VectoSimulationJobType.SerialHybridVehicle:
				SaveSerialHybridJob(input, filename, DeclMode);
				break;
			case VectoSimulationJobType.ParallelHybridVehicle:
				SaveParallelHybridJob(input, filename, DeclMode);
				break;
			case VectoSimulationJobType.BatteryElectricVehicle:
				SaveBatteryElectricJob(input, filename, DeclMode);
				break;
			case VectoSimulationJobType.IEPC_E:
				SaveIEPCEJob(input, filename, DeclMode);
				break;
			case VectoSimulationJobType.IEPC_S:
				SaveIEPCEJob(input, filename, DeclMode);
				break;
			case VectoSimulationJobType.IHPC:
				SaveIHPCJob(input, filename, DeclMode);
				break;
			case VectoSimulationJobType.EngineOnlySimulation:
				SaveEngineOnlyJob(input, filename, DeclMode);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void SaveIHPCJob(IEngineeringInputDataProvider input, string filename, bool declMode)
	{
		DoSaveParallelHybridJob(input, filename, declMode, IHPCVectoJobFormatVersion);
	}

	private void SaveParallelHybridJob(IEngineeringInputDataProvider input, string filename, bool declMode)
	{
		DoSaveParallelHybridJob(input, filename, declMode, PHEVVectoJobFormatVersion);
	}

	private void DoSaveParallelHybridJob(IEngineeringInputDataProvider input, string filename, bool declMode,
		int versionNumber)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(versionNumber);
		var body = new Dictionary<string, object>();
		// SavedInDeclMode = Cfg.DeclMode
		var job = input.JobInputData;

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);
		body.Add("EngineOnlyMode", job.JobType == VectoSimulationJobType.EngineOnlySimulation);

		// Main Files
		body.Add("VehicleFile", GetRelativePath(job.Vehicle.DataSource.SourceFile, basePath));
		body.Add("EngineFile", GetRelativePath(input.JobInputData.Vehicle.Components.EngineInputData.DataSource.SourceFile, basePath));

		if (input.JobInputData.Vehicle.Components.GearboxInputData != null) {
			body.Add("GearboxFile", GetRelativePath(input.JobInputData.Vehicle.Components.GearboxInputData.DataSource.SourceFile, basePath));
		}

		if (!job.SavedInDeclarationMode && input.DriverInputData.GearshiftInputData != null) {
			body.Add("TCU", GetRelativePath(input.DriverInputData.GearshiftInputData.Source, basePath));

		}
		body.Add("HybridStrategyParams", GetRelativePath(input.JobInputData.HybridStrategyParameters.Source, basePath));

		var auxList = new List<object>();
		if (job.SavedInDeclarationMode && job.Vehicle is IVehicleDeclarationInputData declVehicle) {
			var aux = declVehicle.Components.AuxiliaryInputData;
			foreach (var auxEntry in aux.Auxiliaries) {

				var auxOut = new Dictionary<string, object>();
				var engineeringAuxEntry = auxEntry;
				if (!job.SavedInDeclarationMode) {
					auxOut.Add("Type", auxEntry.Type.Name());
					auxOut.Add("Technology", new string[] { });
				} else {
					auxOut.Add("Type", auxEntry.Type.Name());
					auxOut.Add("Technology", engineeringAuxEntry.Technology);
				}

				auxList.Add(auxOut);
			}
			if (declVehicle.Components.BusAuxiliaries != null) {
				body.Add("BusAux", GetRelativePath(job.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData.DataSource.SourceFile, basePath));
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
			body.Add("Padd_electric", aux.Auxiliaries.ElectricPowerDemand.Value());
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
				new Dictionary<string, object> {
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
		var overspeedDic = new Dictionary<string, object> {
			{ "Mode", driver.OverSpeedData.Enabled ? "Overspeed" : "Off" },
			{ "MinSpeed", driver.OverSpeedData.MinSpeed.AsKmph },
			{ "OverSpeed", driver.OverSpeedData.OverSpeed.AsKmph }
		};

		body.Add("OverSpeedEcoRoll", overspeedDic);

		// Cycles
		if (!job.SavedInDeclarationMode)
			body.Add("Cycles", GetCycles(job, filename));

		WriteFile(header, body, filename);
	}

	private void SaveSerialHybridJob(IEngineeringInputDataProvider input, string filename, bool declMode)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(SHEVVectoJobFormatVersion);
		var body = new Dictionary<string, object>();
		var job = input.JobInputData;

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);

		// Main Files
		body.Add("VehicleFile", GetRelativePath(job.Vehicle.DataSource.SourceFile, basePath));
		body.Add("EngineFile", GetRelativePath(input.JobInputData.Vehicle.Components.EngineInputData.DataSource.SourceFile, basePath));

		if (input.JobInputData.Vehicle.Components.GearboxInputData != null) {
			body.Add("GearboxFile", GetRelativePath(input.JobInputData.Vehicle.Components.GearboxInputData.DataSource.SourceFile, basePath));
		}

		if (!job.SavedInDeclarationMode && input.DriverInputData.GearshiftInputData != null) {
			body.Add("TCU", GetRelativePath(input.DriverInputData.GearshiftInputData.Source, basePath));
		}

		body.Add("HybridStrategyParams", GetRelativePath(input.JobInputData.HybridStrategyParameters.Source, basePath));

		var auxList = new List<object>();
		if (job.SavedInDeclarationMode && job.Vehicle is IVehicleDeclarationInputData declVehicle) {
			var aux = declVehicle.Components.AuxiliaryInputData;
			foreach (var auxEntry in aux.Auxiliaries) {

				var auxOut = new Dictionary<string, object>();
				if (!job.SavedInDeclarationMode) {
					auxOut.Add("Type", auxEntry.Type.Name());
					auxOut.Add("Technology", new string[] { });
				} else {
					auxOut.Add("Type", auxEntry.Type.Name());
					auxOut.Add("Technology", auxEntry.Technology);
				}

				auxList.Add(auxOut);
			}
			if (declVehicle.Components.BusAuxiliaries != null) {
				body.Add("BusAux", GetRelativePath(job.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData.DataSource.SourceFile, basePath));
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

			if (!aux.Auxiliaries.ConstantPowerDemand.Value().IsEqual(0))
				body.Add("Padd", aux.Auxiliaries.ConstantPowerDemand.Value());

			if (!aux.Auxiliaries.PowerDemandICEOffDriving.Value().IsEqual(0))
				body.Add("Paux_ICEOff_Driving", aux.Auxiliaries.PowerDemandICEOffDriving.Value());

			if (!aux.Auxiliaries.PowerDemandICEOffStandstill.Value().IsEqual(0))
				body.Add("Paux_ICEOff_Standstill", aux.Auxiliaries.PowerDemandICEOffStandstill.Value());

			body.Add("Padd_electric", aux.Auxiliaries.ElectricPowerDemand.Value());
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
				new Dictionary<string, object> {
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
		var overspeedDic = new Dictionary<string, object> {
			{ "Mode", driver.OverSpeedData.Enabled ? "Overspeed" : "Off" },
			{ "MinSpeed", driver.OverSpeedData.MinSpeed.AsKmph },
			{ "OverSpeed", driver.OverSpeedData.OverSpeed.AsKmph }
		};

		body.Add("OverSpeedEcoRoll", overspeedDic);

		// Cycles
		if (!job.SavedInDeclarationMode)
			body.Add(
				"Cycles", GetCycles(job, filename));

		WriteFile(header, body, filename);
	}

	public void SaveBatteryElectricJob(IEngineeringInputDataProvider input, string filename, bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);

		// Header
		var header = GetHeader(BEVVectoJobFormatVersion);

		// Body
		var body = new Dictionary<string, object>();

		// SavedInDeclMode = Cfg.DeclMode

		var job = input.JobInputData;

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);
		//body.Add("EngineOnlyMode", job.JobType);

		// Main Files
		body.Add("VehicleFile", GetRelativePath(job.Vehicle.DataSource.SourceFile, basePath));
		if (input.JobInputData.Vehicle.Components.GearboxInputData != null) {
			body.Add("GearboxFile",
				GetRelativePath(input.JobInputData.Vehicle.Components.GearboxInputData.DataSource.SourceFile, basePath));
			if (input.DriverInputData.GearshiftInputData != null) {
				body.Add("TCU", GetRelativePath(input.DriverInputData.GearshiftInputData.Source, basePath));
			}
		}
		body.Add("Padd_electric", input.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand.Value());

		if (!job.SavedInDeclarationMode && job.Vehicle is IVehicleEngineeringInputData engVehicle) {
			var aux = engVehicle.Components.AuxiliaryInputData;
			if (aux.BusAuxiliariesData != null) {
				body.Add("BusAux",
					GetRelativePath(job.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData.DataSource.SourceFile,
						basePath));
			}
		}
		//if (!job.SavedInDeclarationMode)
		//      {
		//}

		//body.Add("ShiftStrategy", input.JobInputData.ShiftStrategy);

		//var aux = job.Vehicle.Components.AuxiliaryInputData;

		// AA-TB
		// ADVANCED AUXILIARIES 
		//body.Add("AuxiliaryAssembly", aux.AuxiliaryAssembly.GetName());
		//body.Add("AuxiliaryVersion", aux.AuxiliaryVersion);
		//body.Add("AdvancedAuxiliaryFilePath", GetRelativePath(aux.AdvancedAuxiliaryFilePath, basePath));

		//var pAdd = 0.0;
		//var auxList = new List<object>();
		//foreach (var auxEntry in aux.Auxiliaries)
		//{
		//    if (auxEntry.AuxiliaryType == AuxiliaryDemandType.Constant)
		//    {
		//        pAdd += auxEntry.ConstantPowerDemand.Value();
		//        continue;
		//    }

		//    var auxOut = new Dictionary<string, object>();
		//    var engineeringAuxEntry = auxEntry as IAuxiliaryDeclarationInputData;
		//    if (!job.SavedInDeclarationMode)
		//    {
		//        auxOut.Add("ID", auxEntry.ID);
		//        auxOut.Add("Type", AuxiliaryTypeHelper.ParseKey(auxEntry.ID).Name());
		//        auxOut.Add("Path", GetRelativePath(auxEntry.DemandMap.Source, basePath));
		//        auxOut.Add("Technology", new string[] { });
		//    }
		//    else
		//    {
		//        auxOut.Add("ID", auxEntry.ID);
		//        auxOut.Add("Type", AuxiliaryTypeHelper.ParseKey(auxEntry.ID).Name());
		//        auxOut.Add("Technology", engineeringAuxEntry.Technology);
		//    }
		//    auxList.Add(auxOut);
		//}

		//body.Add("Aux", auxList);
		//if (!job.SavedInDeclarationMode)
		//    body.Add("Padd", pAdd);

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

		if (!job.SavedInDeclarationMode) {
			var dfTargetSpeed = driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup != null &&
								File.Exists(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source)
				? GetRelativePath(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source, basePath)
				: "";
			var dfVelocityDrop = driver.Lookahead.CoastingDecisionFactorVelocityDropLookup != null &&
								File.Exists(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source)
				? GetRelativePath(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source, basePath)
				: "";
			body.Add("LAC", new Dictionary<string, object> {
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
		var overspeedDic = new Dictionary<string, object> {
			{ "Mode", driver.OverSpeedData.Enabled ? "Overspeed" : "Off" },
			{ "MinSpeed", driver.OverSpeedData.MinSpeed.AsKmph },
			{ "OverSpeed", driver.OverSpeedData.OverSpeed.AsKmph }
		};

		body.Add("OverSpeedEcoRoll", overspeedDic);

		// Cycles
		if (!job.SavedInDeclarationMode)
			body.Add("Cycles", GetCycles(job, filename));

		WriteFile(header, body, filename);
	}

	private void SaveIEPCEJob(IEngineeringInputDataProvider input, string filename, bool declMode)
	{
		var basePath = Path.GetDirectoryName(filename);

		// Header
		var header = GetHeader(IEPCVectoJobFormatVersion);

		// Body
		var body = new Dictionary<string, object>();

		// SavedInDeclMode = Cfg.DeclMode

		var job = input.JobInputData;

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);
		//body.Add("EngineOnlyMode", job.JobType);

		// Main Files
		body.Add("VehicleFile", GetRelativePath(job.Vehicle.DataSource.SourceFile, basePath));
		if (job.JobType == VectoSimulationJobType.IEPC_S) {
			body.Add("EngineFile", GetRelativePath(input.JobInputData.Vehicle.Components.EngineInputData.DataSource.SourceFile, basePath));
		}

		if (job.Vehicle.Components.GearboxInputData != null) {
			body.Add("GearboxFile",
				GetRelativePath(input.JobInputData.Vehicle.Components.GearboxInputData.DataSource.SourceFile, basePath));
			
		}
		if (input.DriverInputData.GearshiftInputData != null && !job.SavedInDeclarationMode) {
			body.Add("TCU", GetRelativePath(input.DriverInputData.GearshiftInputData.Source, basePath));
		}

		if (!job.SavedInDeclarationMode && job.Vehicle.VehicleType == VectoSimulationJobType.IEPC_S) {
			body.Add("HybridStrategyParams", GetRelativePath(input.JobInputData.HybridStrategyParameters.Source, basePath));
		}
		body.Add("Padd_electric", input.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand.Value());

		if (!job.SavedInDeclarationMode && job.Vehicle is IVehicleEngineeringInputData engVehicle) {
			var aux = engVehicle.Components.AuxiliaryInputData;
			if (aux.BusAuxiliariesData != null) {
				body.Add("BusAux",
					GetRelativePath(job.Vehicle.Components.AuxiliaryInputData.BusAuxiliariesData.DataSource.SourceFile,
						basePath));
			}
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

		if (!job.SavedInDeclarationMode) {
			var dfTargetSpeed = driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup != null &&
								File.Exists(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source)
				? GetRelativePath(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source, basePath)
				: "";
			var dfVelocityDrop = driver.Lookahead.CoastingDecisionFactorVelocityDropLookup != null &&
								File.Exists(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source)
				? GetRelativePath(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source, basePath)
				: "";
			body.Add("LAC", new Dictionary<string, object> {
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
		var overspeedDic = new Dictionary<string, object> {
			{ "Mode", driver.OverSpeedData.Enabled ? "Overspeed" : "Off" },
			{ "MinSpeed", driver.OverSpeedData.MinSpeed.AsKmph },
			{ "OverSpeed", driver.OverSpeedData.OverSpeed.AsKmph }
		};

		body.Add("OverSpeedEcoRoll", overspeedDic);

		// Cycles
		if (!job.SavedInDeclarationMode)
			body.Add("Cycles", GetCycles(job, filename));

		WriteFile(header, body, filename);
	}

	private string[] GetCycles(IEngineeringJobInputData job, string filename)
	{
		return job.Cycles.Select(x => x.CycleData.SourceType == DataSourceType.Embedded ? x.Name : GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray();
	}

	public void SaveEngineOnlyJob(IEngineeringInputDataProvider input, string filename, bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(VectoJobFormatVersion);
		var body = new Dictionary<string, object>();

		// SavedInDeclMode = Cfg.DeclMode

		var job = input.JobInputData;

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);
		body.Add("EngineOnlyMode", job.JobType == VectoSimulationJobType.EngineOnlySimulation);

		body.Add("EngineFile", GetRelativePath(job.EngineOnly.DataSource.SourceFile, basePath));
		body.Add("Cycles", job.Cycles.Select(x => GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray());
	}


	public void SaveConventionalJob(IEngineeringInputDataProvider input, string filename, bool DeclMode)
	{
		var basePath = Path.GetDirectoryName(filename);
		var header = GetHeader(VectoJobFormatVersion);
		var body = new Dictionary<string, object>();
		// SavedInDeclMode = Cfg.DeclMode
		var job = input.JobInputData;
		body.Add("SavedInDeclMode", job.SavedInDeclarationMode);
		body.Add("EngineOnlyMode", job.JobType == VectoSimulationJobType.EngineOnlySimulation);

		// Main Files
		body.Add("VehicleFile", GetRelativePath(job.Vehicle.DataSource.SourceFile, basePath));
		body.Add("EngineFile", GetRelativePath(input.JobInputData.Vehicle.Components.EngineInputData.DataSource.SourceFile, basePath));
		body.Add("GearboxFile", GetRelativePath(input.JobInputData.Vehicle.Components.GearboxInputData.DataSource.SourceFile, basePath));

		if (!job.SavedInDeclarationMode) {
			body.Add("TCU", GetRelativePath(input.DriverInputData.GearshiftInputData.Source, basePath));
		}

		if (job.SavedInDeclarationMode && job.Vehicle is IVehicleDeclarationInputData declVehicle) {
			var aux = declVehicle.Components.AuxiliaryInputData;
			var auxList = new List<object>();
			foreach (var auxEntry in aux.Auxiliaries) {
				var auxOut = new Dictionary<string, object>();
				var engineeringAuxEntry = auxEntry;
				if (!job.SavedInDeclarationMode) {
					auxOut.Add("Type", auxEntry.Type.Name());
					auxOut.Add("Technology", new string[] { });
				} else {
					auxOut.Add("ID", auxEntry.Type.Key());
					auxOut.Add("Type", auxEntry.Type.Name());
					auxOut.Add("Technology", engineeringAuxEntry.Technology);
				}

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
			var dfTargetSpeed = driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup != null &&
								File.Exists(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source)
				? GetRelativePath(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source, basePath)
				: "";
			var dfVelocityDrop = driver.Lookahead.CoastingDecisionFactorVelocityDropLookup != null &&
								File.Exists(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source)
				? GetRelativePath(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source, basePath)
				: "";
			body.Add("LAC", new Dictionary<string, object> {
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
		var overspeedDic = new Dictionary<string, object> {
			{ "Mode", driver.OverSpeedData.Enabled ? "Overspeed" : "Off" },
			{ "MinSpeed", driver.OverSpeedData.MinSpeed.AsKmph },
			{ "OverSpeed", driver.OverSpeedData.OverSpeed.AsKmph }
		};

		body.Add("OverSpeedEcoRoll", overspeedDic);

		// Cycles
		if (!job.SavedInDeclarationMode)
			body.Add("Cycles",
				job.Cycles.Select(x => GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray());

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
		var body = new Dictionary<string, object> {
			{ "SavedInDeclMode", declarationmode },
			{ "DeclarationVehicle", GetRelativePath(job.Vehicle.DataSource.SourceFile, Path.GetDirectoryName(filename)) }
		};
		if (declarationmode) {
			body.Add("ManufacturerRecord", GetRelativePath(job.ManufacturerReportInputData.Source, Path.GetDirectoryName(filename)));
			body.Add("Mileage", job.Mileage.ConvertToKiloMeter().Value);
		}
		body.Add("FanPowerCoefficients", job.FanPowerCoefficents);
		body.Add("FanDiameter", job.FanDiameter.Value());
		body.Add("Cycles", job.Cycles.Select(x => GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray());
		return body;
	}

	public void ExportJob(IEngineeringInputDataProvider input, string filename, bool separateFiles)
	{
		throw new NotImplementedException();
	}

	public static void WriteFile(JToken content, string path)
	{
		if (!content.Any())
			return;

		var str = JsonConvert.SerializeObject(content, Formatting.Indented);
		File.WriteAllText(path, str);
	}

	public static void WriteFile(Dictionary<string, object> content, string path) =>
		WriteFile(JToken.FromObject(content), path);

	protected static void WriteFile(Dictionary<string, object> header, Dictionary<string, object> body, string path) =>
		WriteFile(JToken.FromObject(new Dictionary<string, object> { { "Header", header }, { "Body", body } }), path);

	public void SaveStrategyParameters(IHybridStrategyParameters hp, string filePath, bool declMode)
	{
		var header = GetHeader(HybridStrategyParamsVersion);
		var body = new Dictionary<string, object> {
			{ "EquivalenceFactorDischarge", hp.EquivalenceFactorDischarge },
			{ "EquivalenceFactorCharge", hp.EquivalenceFactorCharge },
			{ "MinSoC", hp.MinSoC * 100 },
			{ "MaxSoC", hp.MaxSoC * 100 },
			{ "TargetSoC", hp.TargetSoC * 100 },
			{ "AuxBufferTime", hp.AuxBufferTime.Value() },
			{ "AuxBufferChgTime", hp.AuxBufferChargeTime.Value() },
			{ "MinICEOnTime", hp.MinimumICEOnTime.Value() },
			{ "ICEStartPenaltyFactor", hp.ICEStartPenaltyFactor },
			{ "CostFactorSOCExponent", hp.CostFactorSOCExpponent },
			{ "GensetMinOptPowerFactor", hp.GensetMinOptPowerFactor }
		};
		WriteFile(header, body, filePath);
	}

	public void SaveBusAuxEngineeringParameters(IBusAuxiliariesEngineeringData busAux, string filePath, bool declMode)
	{
		var header = GetHeader(HybridStrategyParamsVersion);

		var ps = new Dictionary<string, object> {
			{ "CompressorMap", busAux.PneumaticSystem.CompressorMap != null ? GetRelativePath(busAux.PneumaticSystem.CompressorMap.Source, Path.GetDirectoryName(filePath)) : "" },
			{ "AverageAirDemand", busAux.PneumaticSystem.AverageAirConsumed.Value() },
			{ "SmartAirCompression", busAux.PneumaticSystem.SmartAirCompression },
			{ "GearRatio", busAux.PneumaticSystem.GearRatio },
		};
		var es = new Dictionary<string, object> {
			{ "AlternatorEfficiency", busAux.ElectricSystem.AlternatorEfficiency },
			{ "CurrentDemand", busAux.ElectricSystem.CurrentDemand.Value() },
			{ "CurrentDemandEngineOffDriving", busAux.ElectricSystem.CurrentDemandEngineOffDriving.Value() },
			{ "CurrentDemandEngineOffStandstill", busAux.ElectricSystem.CurrentDemandEngineOffStandstill.Value() },
			{ "AlternatorType", busAux.ElectricSystem.AlternatorType.ToString() },
			{ "ElectricStorageCapacity", busAux.ElectricSystem.ElectricStorageCapacity.ConvertToWattHour().Value },
			{ "BatteryEfficiency", busAux.ElectricSystem.ElectricStorageEfficiency },
			{ "MaxAlternatorPower", busAux.ElectricSystem.MaxAlternatorPower.Value() },
			{ "DCDCConverterEfficiency", busAux.ElectricSystem.DCDCConverterEfficiency },
			{ "ESSupplyFromHEVREESS", busAux.ElectricSystem.ESSupplyFromHEVREESS}
		};
		var hvac = new Dictionary<string, object> {
			{ "ElectricPowerDemand", busAux.HVACData.ElectricalPowerDemand.Value() },
			{ "MechanicalPowerDemand", busAux.HVACData.MechanicalPowerDemand.Value() },
			{ "AuxHeaterPower", busAux.HVACData.AuxHeaterPower.Value() },
			{ "AverageHeatingDemand", busAux.HVACData.AverageHeatingDemand.Value() / 1e6 }
		};

		var body = new Dictionary<string, object> {
			{ "PneumaticSystem", ps },
			{ "ElectricSystem", es },
			{ "HVAC", hvac }
		};
		WriteFile(header, body, filePath);
	}
}

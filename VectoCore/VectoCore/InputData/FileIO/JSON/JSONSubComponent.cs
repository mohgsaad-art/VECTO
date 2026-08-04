using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public abstract class JSONSubComponent
	{
		protected JSONVehicleDataV7 Base;

		protected JSONSubComponent(JSONVehicleDataV7 jsonFile)
		{
			Base = jsonFile;
		}

		protected JObject Body => Base.Body;

		protected TableData ReadTableData(string filename, string tableType, bool required = true)
		{
			return Base.ReadTableData(filename, tableType, required);
		}

		protected string BasePath => Base.BasePath;

		protected bool TolerateMissing => Base.TolerateMissing;

		public DataSource DataSource => Base.DataSource;

		public bool SavedInDeclarationMode => Base.SavedInDeclarationMode;

		public string Manufacturer => Base.Manufacturer;

		public string Model => Base.Model;

		public DateTime Date => Base.Date;

		public string AppVersion => Base.AppVersion;

		public CertificationMethod CertificationMethod => Base.CertificationMethod;

		public string CertificationNumber => Base.CertificationNumber;

		public DigestData DigestValue => Base.DigestValue;

		public virtual XmlNode XMLSource => Base.XMLSource;
	}

	// ###################################################################
	// ###################################################################

	internal class JSONAxlePowertrainInputData : IAxlePowertrainEngineeringInputData
	{
		private JToken _axlePt;
		private string _basePath;
		private IGearboxEngineeringInputData _gearbox;
		private IGearshiftEngineeringInputData _gearShift;
		private ElectricMachineEntry<IElectricMotorEngineeringInputData> _electricMotor;
		private IIEPCEngineeringInputData _IEPC;

		public JSONAxlePowertrainInputData(JToken axlePt, string basePath)
		{
			_axlePt = axlePt;
			_basePath = basePath;
			
			if (AxleNumber == int.MinValue)
			{
				throw new VectoException($"{JsonKeys.Vehicle_AxlePowertrain_AxleNumber} not defined in {JsonKeys.Vehicle_AxlePowertrains}");
			}

			if (Architecture == ArchitectureID.UNKNOWN)
			{
				throw new VectoException($"Architecture bad or missing in {JsonKeys.Vehicle_AxlePowertrains}");
			}

			if (Architecture.IsOneOf(ArchitectureID.E2, ArchitectureID.F2))
			{
                throw new VectoException($"Architecture {Architecture} is not supported yet!");
            }

			if (Architecture.IsIEPC() && (IEPCEngineeringInputData.Gears.Count > 1))
			{
                throw new VectoException($"Architecture {Architecture} with multiple gears is not supported yet!");
            }
		}

		public virtual int AxleNumber => (_axlePt[JsonKeys.Vehicle_AxlePowertrain_AxleNumber] == null)
			? int.MinValue
			: _axlePt.GetEx<int>(JsonKeys.Vehicle_AxlePowertrain_AxleNumber);

		public virtual ArchitectureID Architecture => (_axlePt["Architecture"] == null)
			? ArchitectureID.UNKNOWN
			: ArchitectureIDHelper.Parse(_axlePt.GetEx<string>("Architecture"));

		public virtual IGearboxEngineeringInputData GearboxInputData => _gearbox ?? (_gearbox = ReadGearbox());

		public virtual IAxleGearInputData AxleGearInputData => GearboxInputData as IAxleGearInputData;

		public virtual ITorqueConverterEngineeringInputData TorqueConverterInputData => 
			GearboxInputData as ITorqueConverterEngineeringInputData;

		public virtual IAngledriveInputData AngledriveInputData => 
			new JTokenAngleDriveInputData(_axlePt[JsonKeys.Vehicle_Angledrive], _basePath);

		public virtual IRetarderInputData RetarderInputData => 
			new JTokenRetarderInputData(_axlePt[JsonKeys.Vehicle_Retarder], _basePath);

		public virtual IPTOTransmissionInputData PTOTransmissionInputData => 
			new JTokenPTOTransmissionInputData(_axlePt[JsonKeys.Vehicle_PTO], _basePath) ;

		public virtual IGearshiftEngineeringInputData GearshiftInputData => _gearShift ?? (_gearShift = ReadGearShift());

		public virtual ElectricMachineEntry<IElectricMotorEngineeringInputData> ElectricMotor => _electricMotor ?? 
			(_electricMotor = new JTokenElectricMotorInputData(_axlePt[JsonKeys.Vehicle_ElectricMotor], _basePath)
				.ReadElectricMotor());

		public virtual IIEPCEngineeringInputData IEPCEngineeringInputData => _IEPC ?? (_IEPC = ReadIEPC());

		private IGearshiftEngineeringInputData ReadGearShift()
		{
			var tcuFile = "";

			try
			{
				tcuFile = (_axlePt[JsonKeys.Vehicle_TCU] != null)
					? _axlePt.GetEx(JsonKeys.Vehicle_TCU).Value<string>()
					: null;

				return (tcuFile != null)
					? JSONInputDataFactory.ReadShiftParameters(Path.Combine(_basePath, tcuFile), false)
					: null;
			}
			catch (Exception e)
			{
				throw new VectoException($"JobFile: Failed to read TCU file '{tcuFile}': {e.Message}", e);
			}
		}

		private IGearboxEngineeringInputData ReadGearbox()
		{
			var gearboxFile = "";

			try
			{
				gearboxFile = (_axlePt[JsonKeys.Vehicle_GearboxFile] != null) 
					? _axlePt.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>()
					: null;

				return (gearboxFile != null)
					? JSONInputDataFactory.ReadGearbox(Path.Combine(_basePath, gearboxFile))
					: null;
			}
			catch (Exception e)
			{
				throw new VectoException($"JobFile: Failed to read Gearbox file '{gearboxFile}': {e.Message}", e);
			}
		}

		private IIEPCEngineeringInputData ReadIEPC()
		{
			var iepcFile = "";

			try
			{
				iepcFile = (_axlePt[JsonKeys.Vehicle_IEPC] != null)
					? _axlePt.GetEx(JsonKeys.Vehicle_IEPC).Value<string>()
					: null;

				return (iepcFile != null)
					? JSONInputDataFactory.ReadIEPCEngineeringInputData(Path.Combine(_basePath, iepcFile))
					: null;
			}
			catch (Exception e)
			{
				throw new VectoException($"JobFile: Failed to read IEPC file '{iepcFile}': {e.Message}", e);
			}
		}

	}

	internal class JSONRetarderInputData : JSONSubComponent, IRetarderInputData
	{
		public JSONRetarderInputData(JSONVehicleDataV7 jsonFile) : base(jsonFile) { }

		#region IRetarderInputData

		public virtual RetarderType Type
		{
			get {
				if (Body[JsonKeys.Vehicle_Retarder] != null) {
					var retarderType = Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<string>(JsonKeys.Vehicle_Retarder_Type);
					return RetarderTypeHelper.Parse(retarderType);
				}

				return RetarderType.None;
			}
		}

		public virtual double Ratio
		{
			get {
				if (Body[JsonKeys.Vehicle_Retarder] != null) {
					return Body.GetEx(JsonKeys.Vehicle_Retarder).GetEx<double>(JsonKeys.Vehicle_Retarder_Ratio);
				}
				return 1.0;
			}
		}

		public virtual TableData LossMap
		{
			get {
				if (Body[JsonKeys.Vehicle_Retarder] != null 
					&& Body.GetEx(JsonKeys.Vehicle_Retarder)[JsonKeys.Vehicle_Retarder_LossMapFile] != null) {
					var lossmapFile = Body.GetEx(JsonKeys.Vehicle_Retarder)[JsonKeys.Vehicle_Retarder_LossMapFile];
					if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>())) {
						return null;
					}

					try {
						return ReadTableData(lossmapFile.Value<string>(), "LossMap");
					} catch (Exception) {
						if (!TolerateMissing) {
							throw;
						}

						return new TableData(
							Path.Combine(BasePath, lossmapFile.Value<string>()) + JSONFile.MissingFileSuffix,
							DataSourceType.Missing);
					}
				}
				return null;
			}
		}

		#endregion
	}

	// ###################################################################
	// ###################################################################

	public class JTokenBase : LoggingObject, IComponentInputData
	{
		public enum TableType {
			LossMap,
			Cycle,
			PTO_Cycle_Driving,
			EPTO_Cycle_Standstill,
			PTO_Cycle_Standstill,
			EM_ADC_LossMap
		};

		protected JToken _token;
		protected string _basePath;

		public JTokenBase(JToken token, string basePath)
		{
			_token = token;
			_basePath = basePath;
		}

		public virtual string CertificationNumber => null;

		public virtual CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public virtual bool SavedInDeclarationMode => false;

		public virtual string Manufacturer => null;

		public virtual string Model => null;

		public virtual DataSource DataSource => null;

		public virtual String AppVersion => null;

		public virtual DateTime Date => DateTime.MinValue;

		public virtual DigestData DigestValue => null;

		protected internal TableData ReadTableData(string filename, string tableType, bool required = true)
		{
			if (!JSONFile.EmptyOrInvalidFileName(filename) && File.Exists(Path.Combine(_basePath, filename)))
			{
				try
				{
					return VectoCSVFile.Read(Path.Combine(_basePath, filename), true);
				}
				catch (Exception e)
				{
					Log.Warn("Failed to read file {0} {1}", Path.Combine(_basePath, filename), tableType);
					throw new VectoException("Failed to read file for {0}: {1}", e, tableType, filename);
				}
			}

			if (required)
			{
				throw new VectoException("Invalid filename for {0}: {1}", tableType, filename);
			}

			return null;
		}

		protected TableData ReadTable(string fieldName, TableType tableType)
		{
			var tableFile = _token?[fieldName]?.Value<string>();

			return string.IsNullOrWhiteSpace(tableFile)
				? null
				: ReadTableData(tableFile, tableType.ToString().Replace("_", " "));
		}
	}

	internal class JTokenElectricMotorInputData : JTokenBase
	{
		public JTokenElectricMotorInputData(JToken emToken, string basePath) : base(emToken, basePath)
		{}

		public ElectricMachineEntry<IElectricMotorEngineeringInputData> ReadElectricMotor()
		{
			return (_token == null)
				? null
				: new ElectricMachineEntry<IElectricMotorEngineeringInputData>
					{
						Position = PowertrainPositionHelper.Parse(_token.GetEx<string>(JsonKeys.Vehicle_EM_Position)),
						RatioADC = _token.GetEx<double>(JsonKeys.Vehicle_EM_Ratio),
						MechanicalTransmissionLossMap = ReadTable(JsonKeys.Vehicle_EM_MechTransLossMap, TableType.EM_ADC_LossMap),
						Count = _token.GetEx<int>(JsonKeys.Vehicle_EM_Count),

						RatioPerGear = (_token[JsonKeys.Vehicle_EM_RatioPerGear] != null)
									? _token[JsonKeys.Vehicle_EM_RatioPerGear].Select(x => x.Value<double>()).ToArray()
									: new double[] { },

						MechanicalTransmissionEfficiency = (_token[JsonKeys.Vehicle_EM_MechEff] != null)
									? _token.GetEx<double>(JsonKeys.Vehicle_EM_MechEff)
									: double.NaN,

						ElectricMachine = JSONInputDataFactory.ReadElectricMotorData(
							Path.Combine(_basePath, _token.GetEx<string>(JsonKeys.Vehicle_EM_MotorFile)), false)
					};
		}
	}

	internal class JTokenRetarderInputData : JTokenBase, IRetarderInputData
	{
		private TableData _lossMap;

		public JTokenRetarderInputData(JToken retarderToken, string basePath) : base(retarderToken, basePath)
		{}

		public virtual RetarderType Type => (_token == null) || (_token[JsonKeys.Vehicle_Retarder_Type] == null)
			? RetarderType.None
			: RetarderTypeHelper.Parse(_token.GetEx<string>(JsonKeys.Vehicle_Retarder_Type));

		public virtual double Ratio => (_token == null) || (_token[JsonKeys.Vehicle_Retarder_Ratio] == null)
			? 1.0
			: _token.GetEx<double>(JsonKeys.Vehicle_Retarder_Ratio);

		public virtual TableData LossMap => 
			_lossMap ?? (_lossMap = ReadTable(JsonKeys.Vehicle_Retarder_LossMapFile, TableType.LossMap));

	}

	internal class JTokenAngleDriveInputData : JTokenBase, IAngledriveInputData 
	{
		private TableData _lossMap;

		public JTokenAngleDriveInputData(JToken angleDriveToken, string basePath) : base(angleDriveToken, basePath) 
		{}

		public virtual AngledriveType Type => (_token == null) || (_token[JsonKeys.Vehicle_Angledrive_Type] == null)
			? AngledriveType.None
			: _token.GetEx<string>(JsonKeys.Vehicle_Angledrive_Type).ParseEnum<AngledriveType>();

		public virtual double Ratio => (_token == null) || (_token[JsonKeys.Vehicle_Angledrive_Ratio] == null)
			? double.NaN
			: _token.GetEx<double>(JsonKeys.Vehicle_Angledrive_Ratio);

		public virtual double Efficiency => (_token == null) || (_token[JsonKeys.Vehicle_Angledrive_Efficiency] == null)
			? double.NaN
			: _token.GetEx<double>(JsonKeys.Vehicle_Angledrive_Efficiency);

		public virtual TableData LossMap => 
			_lossMap ?? (_lossMap = ReadTable(JsonKeys.Vehicle_Angledrive_LossMapFile, TableType.LossMap));

	}

	internal class JTokenPTOTransmissionInputData : JTokenBase, IPTOTransmissionInputData
	{
		public const string PTO_TYPE_NONE = "None";

		private TableData _lossMap;
		private TableData _cycle;
		private TableData _cycleDriving;
		private TableData _cycleEPTOStop;
		private TableData _cyclePTOStop;

		public JTokenPTOTransmissionInputData(JToken ptoToken, string basePath) : base(ptoToken, basePath)
		{
			if ((EPTOCycleDuringStop != null) && (PTOCycleDuringStop != null))
			{
				throw new VectoException(
					$"Setting {JsonKeys.Vehicle_PTO_Cycle} AND {JsonKeys.Vehicle_EPTO_Cycle} is not allowed");
			}
		}

        public virtual int AxleNumber => Constants.NOT_IN_AXLE_POWERTRAIN;

        public virtual string PTOTransmissionType => (_token == null) || (_token[JsonKeys.Vehicle_PTO_Type] == null)
			? PTO_TYPE_NONE
			: _token[JsonKeys.Vehicle_PTO_Type].Value<string>();

		public virtual TableData PTOLossMap => 
			_lossMap ?? (_lossMap = ReadTable(JsonKeys.Vehicle_PTO_LossMapFile, TableType.LossMap));

		public virtual TableData PTOCycle => _cycle ?? (_cycle = ReadTable(JsonKeys.Vehicle_PTO_Cycle, TableType.Cycle));

		public virtual TableData PTOCycleWhileDriving => 
			_cycleDriving ?? (_cycleDriving = ReadTable(JsonKeys.Vehicle_PTO_CycleDriving, TableType.PTO_Cycle_Driving));

		public TableData EPTOCycleDuringStop => 
			_cycleEPTOStop ?? (_cycleEPTOStop = ReadTable(JsonKeys.Vehicle_EPTO_Cycle, TableType.EPTO_Cycle_Standstill));

		public virtual TableData PTOCycleDuringStop => 
			_cyclePTOStop ?? (_cyclePTOStop = ReadTable(JsonKeys.Vehicle_PTO_Cycle, TableType.PTO_Cycle_Standstill));
	}

	internal class JSONAngledriveInputData : JSONSubComponent, IAngledriveInputData
	{
		public JSONAngledriveInputData(JSONVehicleDataV7 jsonFile) : base(jsonFile)
		{ }

		#region IAngledriveInputData

		public virtual AngledriveType Type
		{
			get {
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null) {
					return AngledriveType.None;
				}

				return angleDrive.GetEx<string>(JsonKeys.Vehicle_Angledrive_Type).ParseEnum<AngledriveType>();
			}
		}

		public virtual double Ratio
		{
			get {
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null) {
					return double.NaN;
				}

				return Body.GetEx(JsonKeys.Vehicle_Angledrive).GetEx<double>(JsonKeys.Vehicle_Angledrive_Ratio);
			}
		}

		public virtual TableData LossMap
		{
			get {
				var angleDrive = Body[JsonKeys.Vehicle_Angledrive];
				if (angleDrive == null || angleDrive[JsonKeys.Vehicle_Angledrive_LossMapFile] == null) {
					return null;
				}

				var lossmapFile = angleDrive[JsonKeys.Vehicle_Angledrive_LossMapFile];
				if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>())) {
					return null;
				}

				try {
					return ReadTableData(lossmapFile.Value<string>(), "LossMap");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, lossmapFile.Value<string>()) + JSONFile.MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public virtual double Efficiency => Body.GetEx(JsonKeys.Vehicle_Angledrive).GetEx<double>(JsonKeys.Vehicle_Angledrive_Efficiency);

		#endregion
	}

	// ###################################################################
	// ###################################################################

	internal class JSONAirdragInputData : JSONSubComponent, IAirdragEngineeringInputData
	{
		public JSONAirdragInputData(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region Airdrag

		public virtual SquareMeter AirDragArea =>
			Body[JsonKeys.Vehicle_DragCoefficient] == null
				? null
				: Body.GetEx<double>(JsonKeys.Vehicle_DragCoefficient).SI<SquareMeter>();

		public SquareMeter TransferredAirDragArea => AirDragArea;

		public SquareMeter AirDragArea_0 => AirDragArea;

		public SquareMeter DeltaCdxA_CFD => (Body["DeltaCdxA_CFD"] == null) ? null : Body.GetEx<double>("DeltaCdxA_CFD").SI<SquareMeter>();

		public SquareMeter DeltaCdxA_declared => (Body["DeltaCdxA_declared"] == null) ? null : Body.GetEx<double>("DeltaCdxA_declared").SI<SquareMeter>();

		public SquareMeter DeltaTransferredCdxA => (Body["DeltaTransferredCdxA"] == null) ? null : Body.GetEx<double>("DeltaTransferredCdxA").SI<SquareMeter>();

		public string LicenseNumberCFDMethod => (Body["LicenseNumberCFDMethod"] == null) ? null : Body.GetEx<string>("LicenseNumberCFDMethod");

        public virtual CrossWindCorrectionMode CrossWindCorrectionMode => CrossWindCorrectionModeHelper.Parse(Body.GetEx<string>("CdCorrMode"));

		public virtual TableData CrosswindCorrectionMap
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>("CdCorrFile"), "CrosswindCorrection File");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["CdCorrFile"].ToString()) + JSONFile.MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		#endregion
	}

	// ###################################################################
	// ###################################################################

	internal class JSONPTOTransmissioninputData : JSONSubComponent, IPTOTransmissionInputData
	{
		public JSONPTOTransmissioninputData(JSONVehicleDataV7 vehicle) : base(vehicle)
		{

		}

		#region IPTOTransmissionInputData

		public virtual int AxleNumber => Constants.NOT_IN_AXLE_POWERTRAIN;

		public virtual string PTOTransmissionType
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null) {
					return "None";
				}

				return pto[JsonKeys.Vehicle_PTO_Type].Value<string>();
			}
		}

		public virtual TableData PTOLossMap
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto?[JsonKeys.Vehicle_PTO_LossMapFile] == null) {
					return null;
				}

				var lossmapFile = pto[JsonKeys.Vehicle_PTO_LossMapFile];
				if (string.IsNullOrWhiteSpace(lossmapFile.Value<string>())) {
					return null;
				}

				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_LossMapFile), "LossMap");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, lossmapFile.Value<string>()) + JSONFile.MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public virtual TableData PTOCycle
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_Cycle] == null) {
					return null;
				}

				var cycle = pto[JsonKeys.Vehicle_PTO_Cycle];
				if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
					return null;
				}

				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Cycle), "Cycle");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}

		public virtual TableData PTOCycleDuringStop
		{
			get
			{

				var ePtoTableData = GetEPTOCycle();
				var ptoTableData = GetPTOCycle();
				ThrowIfBothTablesArePresent(ePtoTableData, ptoTableData);
				return ptoTableData;
			}
		}

		private TableData GetPTOCycle()
		{
			var pto = Body[JsonKeys.Vehicle_PTO];
			var cycle = pto?[JsonKeys.Vehicle_PTO_Cycle];
			if (cycle == null) {
				return null;
			}

			if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
				return null;
			}

			try {
				return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Cycle),
					"PTO Cycle Standstill");
			} catch (Exception) {
				if (!TolerateMissing) {
					throw;
				}

				return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix,
					DataSourceType.Missing);
			}
		}

		public TableData EPTOCycleDuringStop
		{
			get
			{
				var ePtoTableData = GetEPTOCycle();
				var ptoTableData = GetPTOCycle();
				ThrowIfBothTablesArePresent(ePtoTableData, ptoTableData);
				return ePtoTableData;
			}

		}

		private void ThrowIfBothTablesArePresent(TableData epto, TableData pto)
		{
			if((epto != null && pto != null)) {
				throw new VectoException(
					$"Setting {JsonKeys.Vehicle_PTO_Cycle} AND {JsonKeys.Vehicle_EPTO_Cycle} is not allowed");
			}
		}

		private TableData GetEPTOCycle()
		{
			var pto = Body[JsonKeys.Vehicle_PTO];
			var cycle = pto?[JsonKeys.Vehicle_EPTO_Cycle];
			if (cycle == null) {
				return null;
			}

			if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
				return null;
			}

			try {
				return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_EPTO_Cycle),
					"EPTO Cycle Standstill");
			} catch (Exception) {
				if (!TolerateMissing) {
					throw;
				}

				return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix,
					DataSourceType.Missing);
			}
		}

		public virtual TableData PTOCycleWhileDriving
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_CycleDriving] == null) {
					return null;
				}
				var cycle = pto[JsonKeys.Vehicle_PTO_CycleDriving];
				if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
					return null;
				}
				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_CycleDriving), "PTO Cycle Driving");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}

		#endregion

	}

	// ###################################################################
	// ###################################################################

	internal class JSONADASInputDataV7 : JSONSubComponent, IAdvancedDriverAssistantSystemsEngineering
	{
		public JSONADASInputDataV7(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public virtual bool EngineStopStart => DeclarationData.Vehicle.ADAS.EngineStopStartDefault;

		public virtual EcoRollType EcoRoll => DeclarationData.Vehicle.ADAS.EcoRoll;

		public virtual PredictiveCruiseControlType PredictiveCruiseControl => DeclarationData.Vehicle.ADAS.PredictiveCruiseControlDefault;

		public virtual bool? ATEcoRollReleaseLockupClutch => null;

		#endregion

	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV8 : JSONADASInputDataV7
	{
		public JSONADASInputDataV8(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region Overrides of JSONADASInputDataV7

		public override bool EngineStopStart => Body.GetEx<bool>("EngineStopStart");

		public override EcoRollType EcoRoll => EcorollTypeHelper.Parse(Body.GetEx<string>("EcoRoll"));

		public override PredictiveCruiseControlType PredictiveCruiseControl => Body.GetEx<string>("PredictiveCruiseControl").ParseEnum<PredictiveCruiseControlType>();

		#endregion
	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV9 : JSONADASInputDataV8
	{
		public JSONADASInputDataV9(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		public override bool? ATEcoRollReleaseLockupClutch => Body["ATEcoRollReleaseLockupClutch"]?.Value<bool>();
	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV10HEV : JSONADASInputDataV8
	{
		public JSONADASInputDataV10HEV(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		public override EcoRollType EcoRoll => EcoRollType.None;
	}

	internal class JSONADASInputDataV10BEV : JSONADASInputDataV8
	{
		public JSONADASInputDataV10BEV(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		public override bool EngineStopStart => false;

		public override EcoRollType EcoRoll => EcoRollType.None;
	}



	// ###################################################################
	// ###################################################################

	internal class JSONBusAuxiliariesData : JSONSubComponent, IBusAuxiliariesDeclarationData, IElectricSupplyDeclarationData,
		IElectricConsumersDeclarationData, IPneumaticSupplyDeclarationData, IPneumaticConsumersDeclarationData,
		IHVACBusAuxiliariesDeclarationData
	{
		public JSONBusAuxiliariesData(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region Implementation of IBusAuxiliariesDeclarationData

		public virtual string FanTechnology => Body["Aux"]?.Value<string>("FanTechnology");

		public virtual IList<string> SteeringPumpTechnology
		{
			get { return Body["Aux"]?["SteeringPumpTechnology"].Select(x => x.Value<string>()).ToList(); }
		}

		public virtual IElectricSupplyDeclarationData ElectricSupply => this;

		public virtual IElectricConsumersDeclarationData ElectricConsumers => this;

		public virtual IPneumaticSupplyDeclarationData PneumaticSupply => this;

		public virtual IPneumaticConsumersDeclarationData PneumaticConsumers => this;

		public virtual IHVACBusAuxiliariesDeclarationData HVACAux => this;

		#endregion

		#region Implementation of IElectricSupplyDeclarationData

		public virtual AlternatorType AlternatorTechnology => Body["Aux"]?["ElectricSupply"]?.GetEx<string>("Technology").ParseEnum<AlternatorType>() ?? AlternatorType.Conventional;

		public virtual IList<IAlternatorDeclarationInputData> Alternators
		{
			get {
				var maxAlternatorPower =
					Body["Aux"]?["ElectricSupply"]?.GetEx<double>("MaxAlternatorPower").SI<Watt>();

				if (maxAlternatorPower == null) {
					return null;
				}

				return new[] { new AlternatorInputData(48.SI<Volt>(), maxAlternatorPower / 48.SI<Volt>()) }
					.Cast<IAlternatorDeclarationInputData>().ToList();
			}
		}

		public bool ESSupplyFromHEVREESS { get; }

		public IList<IBusAuxElectricStorageDeclarationInputData> ElectricStorage
		{
			get {
				var batteryCapacity = Body["Aux"]?["ElectricSupply"]?.GetEx<double>("ElectricStorageCapacity").SI(Unit.SI.Watt.Hour).Cast<WattSecond>();
				if (batteryCapacity == null) {
					return null;
				}

				return new[] { new BusAuxBatteryInputData("none", 48.SI<Volt>(), batteryCapacity / 48.SI<Volt>()), }
					.Cast<IBusAuxElectricStorageDeclarationInputData>().ToList();
			}
		}

		#endregion

		#region Implementation of IElectricConsumersDeclarationData

		public virtual bool? InteriorLightsLED => false;

		public virtual bool? DayrunninglightsLED => false;

		public virtual bool? PositionlightsLED => false;

		public virtual bool? HeadlightsLED => false;

		public virtual bool? BrakelightsLED => false;

		public virtual bool SmartElectrics => Body["Aux"]?["ElectricSupply"]?.GetEx<bool>("SmartElectrics") ?? false;

		public WattSecond ElectricStorageCapacity => Body["Aux"]?["ElectricSupply"]?.GetEx<double>("ElectricStorageCapacity").SI(Unit.SI.Watt.Hour).Cast<WattSecond>();

		#endregion

		#region Implementation of IPneumaticSupplyDeclarationData

		public CompressorDrive CompressorDrive { get; }
		public string Clutch { get; }
		public virtual double Ratio => Body["Aux"]?["PneumaticSupply"]?.GetEx<double>("Ratio") ?? 0.0;

		public virtual string CompressorSize => Body["Aux"]?["PneumaticSupply"]?.GetEx<string>("CompressorSize");

		public bool SmartAirCompression { get; }
		public bool SmartRegeneration { get; }

		#endregion

		#region Implementation of IPneumaticConsumersDeclarationData

		public virtual ConsumerTechnology AirsuspensionControl =>
			Body["Aux"]?["PneumaticConsumers"]?.GetEx<string>("AirsuspensionControl").ParseEnum<ConsumerTechnology>() ??
			ConsumerTechnology.Unknown;

		public virtual ConsumerTechnology AdBlueDosing =>
			Body["Aux"]?["PneumaticConsumers"]?.GetEx<string>("AdBlueDosing").ParseEnum<ConsumerTechnology>() ??
			ConsumerTechnology.Unknown;

		#endregion

		#region Implementation of IHVACBusAuxiliariesDeclarationData

		public virtual BusHVACSystemConfiguration? SystemConfiguration { get; set; }
		public virtual HeatPumpType? HeatPumpTypeCoolingDriverCompartment => null;

		public virtual HeatPumpType? HeatPumpTypeHeatingDriverCompartment => null;

		public virtual HeatPumpType? HeatPumpTypeCoolingPassengerCompartment => null;
		public virtual HeatPumpType? HeatPumpTypeHeatingPassengerCompartment => null;

		public virtual Watt AuxHeaterPower => null;
		public virtual bool? DoubleGlazing => false;
		public virtual bool HeatPump => false;
		public bool? OtherHeatingTechnology { get; }
		public virtual bool? AdjustableCoolantThermostat => Body["Aux"]?["HVAC"]?.GetEx<bool>("AdjustableCoolantThermostat") ?? false;
		public virtual bool? AdjustableAuxiliaryHeater => false;
		public virtual bool EngineWasteGasHeatExchanger => Body["Aux"]?["HVAC"]?.GetEx<bool>("EngineWasteGasHeatExchanger") ?? false;
		public virtual bool? SeparateAirDistributionDucts => false;
		public virtual bool? WaterElectricHeater { get; }
		public virtual bool? AirElectricHeater { get; }

		#endregion

	}

	// ###################################################################
	// ###################################################################

	public class JSONElectricMotors : IElectricMachinesEngineeringInputData
	{
		private readonly IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> _entries;

		public JSONElectricMotors(IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> entries) =>
			_entries = entries;

		IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> IElectricMachinesDeclarationInputData.Entries =>
			_entries.Select(entry => new ElectricMachineEntry<IElectricMotorDeclarationInputData>() {
				ElectricMachine = entry.ElectricMachine,
				ADC = entry.ADC,
				Count = entry.Count,
				MechanicalTransmissionEfficiency = entry.MechanicalTransmissionEfficiency,
				MechanicalTransmissionLossMap = entry.MechanicalTransmissionLossMap,
				Position = entry.Position,
				RatioADC = entry.RatioADC,
				RatioPerGear = entry.RatioPerGear
			}).ToList();
		//_entries.Cast<ElectricMachineEntry<IElectricMotorDeclarationInputData>>().ToList();

		public virtual IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> Entries =>
			_entries;


	}

	// ###################################################################
	// ###################################################################

	public class JSONElectricStorageEngineeringInputData : IElectricStorageEngineeringInputData
	{
		public IREESSPackInputData REESSPack { get; set; }


		public int Count { get; internal set; }
		public int StringId { get; internal set; }
	}

	public class JSONElectricStorageSystemEngineeringInputData : IElectricStorageSystemEngineeringInputData
	{
		private IList<IElectricStorageEngineeringInputData> _electricStorageElements;


		public JSONElectricStorageSystemEngineeringInputData(IList<IElectricStorageEngineeringInputData> entries)
		{
			_electricStorageElements = entries;
		}

		public IList<IElectricStorageEngineeringInputData> ElectricStorageElements => _electricStorageElements;

		#region Implementation of IElectricStorageSystemDeclarationInputData

		IList<IElectricStorageDeclarationInputData> IElectricStorageSystemDeclarationInputData.ElectricStorageElements => _electricStorageElements.Cast<IElectricStorageDeclarationInputData>().ToList();

		#endregion
	}

    public class JSONInMotionChargingInputData : JSONSubComponent, IVehicleInMotionChargingEngineering
    {
		public JSONInMotionChargingInputData(JSONVehicleDataV7 jsonFile) : base(jsonFile) { }

		public bool Enabled => (Body["InMotionCharging"]?["IMC_Enabled"] != null) && Body["InMotionCharging"].GetEx<bool>("IMC_Enabled");

		public double ShareIMCAvailabilityTotalMission => Body["InMotionCharging"]?["IMC_TotalDistance"] != null ? Body["InMotionCharging"].GetEx<double>("IMC_TotalDistance") / 100.0 : 0;

		public SquareMeter DeltaCdxA => Body["InMotionCharging"]?["IMC_CdxA"] != null ? Body["InMotionCharging"].GetEx<double>("IMC_CdxA").SI<SquareMeter>() : 0.SI<SquareMeter>();

		public bool IMCOnMotorwayOnly => Body["InMotionCharging"]?["IMC_MotorwaySection"] != null && Body["InMotionCharging"].GetEx<bool>("IMC_MotorwaySection");


		#region Implementation of IVehicleInMotionChargingDeclaration

		public IMCTechnology Technology => Body["InMotionCharging"]?["Technology"] != null
			? Body["InMotionCharging"].GetEx<string>("Technology").ParseEnum<IMCTechnology>()
			: IMCTechnology.None;

		#endregion
	}

	public class JSONInMotionChargingNotApplicable : IVehicleInMotionChargingEngineering
    {
		#region Implementation of IVehicleInMotionChargingDeclaration

		public IMCTechnology Technology => IMCTechnology.None;

		#endregion

		#region Implementation of IVehicleInMotionChargingEngineering

		public bool Enabled => false;
		public double ShareIMCAvailabilityTotalMission => 0.0;
		public SquareMeter DeltaCdxA => 0.0.SI<SquareMeter>();
		public bool IMCOnMotorwayOnly => false;

		#endregion
	}

	public class JSONFuelCellSystemEngineeringInputData : IFuelCellSystemEngineeringInputData
	{
		public IList<FuelCellStringEntry<IFuelCellComponentEngineeringInputData>> FuelCellStrings { get; internal set; }

        public Meter MaxWindowSize { get; internal set; }
    }
}
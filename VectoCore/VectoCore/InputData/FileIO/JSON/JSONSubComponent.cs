using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;

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
		public JSONPTOTransmissioninputData(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region IPTOTransmissionInputData

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
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Cycle), "PTO Cycle Standstill");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + JSONFile.MissingFileSuffix, DataSourceType.Missing);
				}
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
			_entries.Cast<ElectricMachineEntry<IElectricMotorDeclarationInputData>>().ToList();

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


}
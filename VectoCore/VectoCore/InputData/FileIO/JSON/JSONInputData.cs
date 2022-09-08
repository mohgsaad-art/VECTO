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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public abstract class JSONFile : LoggingObject
	{
		public const string MissingFileSuffix = "   -- (MISSING!)";

		protected readonly string _sourceFile;

		protected readonly string Version;

		protected internal readonly JObject Body;

		protected JSONFile(JObject data, string filename, bool tolerateMissing = false)
		{
			var header = (JObject)data.GetEx(JsonKeys.JsonHeader);
			Version = header[JsonKeys.JsonHeader_FileVersion] != null ? header.GetEx<string>(JsonKeys.JsonHeader_FileVersion) : string.Empty;
			Body = (JObject)data.GetEx(JsonKeys.JsonBody);
			_sourceFile = Path.GetFullPath(filename);
			TolerateMissing = tolerateMissing;
		}

		protected bool TolerateMissing { get; set; }

		public DataSource DataSource
		{
			get { return new DataSource { SourceType = DataSourceType.JSONFile, SourceFile = _sourceFile, SourceVersion = Version }; }
		}

		public string Source
		{
			get { return _sourceFile; }
		}

		public bool SavedInDeclarationMode
		{
			get { return Body.GetEx(JsonKeys.SavedInDeclMode).Value<bool>(); }
		}

		internal string BasePath
		{
			get { return Path.GetDirectoryName(_sourceFile); }
		}

		protected internal TableData ReadTableData(string filename, string tableType, bool required = true)
		{
			if (!EmptyOrInvalidFileName(filename) && File.Exists(Path.Combine(BasePath, filename))) {
				try {
					return VectoCSVFile.Read(Path.Combine(BasePath, filename), true);
				} catch (Exception e) {
					Log.Warn("Failed to read file {0} {1}", Path.Combine(BasePath, filename), tableType);
					throw new VectoException("Failed to read file for {0}: {1}", e, tableType, filename);
				}
			}

			if (required) {
				throw new VectoException("Invalid filename for {0}: {1}", tableType, filename);
			}

			return null;
		}

		internal static bool EmptyOrInvalidFileName(string filename)
		{
			return filename == null || !filename.Any() ||
					filename.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)
					|| filename.Equals("-");
		}

		public static JObject GetDummyJSONStructure()
		{
			return JObject.FromObject(
				new Dictionary<string, object>() {
					{ JsonKeys.JsonHeader, new object() },
					{ JsonKeys.JsonBody, new object() }
				});
		}
	}

	/// <summary>
	/// Class for reading json data of vecto-job-file.
	/// Fileformat: .vecto
	/// </summary>
	public class JSONInputDataV2 : JSONFile, IEngineeringInputDataProvider, IDeclarationInputDataProvider,
		IEngineeringJobInputData, IDriverEngineeringInputData, IAuxiliariesEngineeringInputData,
		IAuxiliariesDeclarationInputData, IJSONVehicleComponents
	{
		private IBusAuxiliariesEngineeringData _busAux;

		public JSONInputDataV2(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing)
		{
			_jobname = Path.GetFileNameWithoutExtension(filename);

			Engine = ReadEngine();

			if (Body.GetEx(JsonKeys.Job_EngineOnlyMode).Value<bool>()) {
				return;
			}

			Gearbox = ReadGearbox();
			AxleGear = Gearbox as IAxleGearInputData;
			TorqueConverter = Gearbox as ITorqueConverterEngineeringInputData;
			GearshiftInputData = Gearbox as IGearshiftEngineeringInputData;

			VehicleData = ReadVehicle();
		}

		public IGearboxEngineeringInputData Gearbox { get; internal set; }

		public virtual IGearshiftEngineeringInputData GearshiftInputData { get; internal set; }

		public virtual IEngineStopStartEngineeringInputData EngineStopStartData => null;

		public virtual IEcoRollEngineeringInputData EcoRollData => null;

		public virtual IPCCEngineeringInputData PCCData => null;

		public IAxleGearInputData AxleGear { get; internal set; }
		public ITorqueConverterEngineeringInputData TorqueConverter { get; internal set; }
		public IEngineEngineeringInputData Engine { get; internal set; }


		protected readonly IVehicleEngineeringInputData VehicleData;

		private readonly string _jobname;


		public IAuxiliariesEngineeringInputData EngineeringAuxiliaries
		{
			get { return this; }
		}

		public IAuxiliariesDeclarationInputData DeclarationAuxiliaries
		{
			get { return this; }
		}

		private IVehicleEngineeringInputData ReadVehicle()
		{
			try {
				var vehicleFile = Body.GetEx(JsonKeys.Vehicle_VehicleFile).Value<string>();
				return JSONInputDataFactory.ReadJsonVehicle(
					Path.Combine(BasePath, vehicleFile), this);
			} catch (Exception e) {
				if (!TolerateMissing) {
					throw new VectoException(
						"JobFile: Failed to read Vehicle file '{0}': {1}", e,
						Body[JsonKeys.Vehicle_VehicleFile],
						e.Message);
				}

				return new JSONVehicleDataV7(
					GetDummyJSONStructure(),
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_VehicleFile).Value<string>()) +
					MissingFileSuffix, this);
			}
		}

		private IGearboxEngineeringInputData ReadGearbox()
		{
			try {
				var gearboxFile = Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>();

				return JSONInputDataFactory.ReadGearbox(Path.Combine(BasePath, gearboxFile));
			} catch (Exception e) {
				if (!TolerateMissing) {
					throw new VectoException(
						"JobFile: Failed to read Gearbox file '{0}': {1}", e,
						Body[JsonKeys.Vehicle_GearboxFile],
						e.Message);
				}

				return new JSONGearboxDataV6(
					GetDummyJSONStructure(),
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>()) +
					MissingFileSuffix);
			}
		}

		private IEngineEngineeringInputData ReadEngine()
		{
			try {
				return JSONInputDataFactory.ReadEngine(
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_EngineFile).Value<string>()));
			} catch (Exception e) {
				if (!TolerateMissing) {
					throw new VectoException(
						"JobFile: Failed to read Engine file '{0}': {1}", e,
						Body[JsonKeys.Vehicle_EngineFile],
						e.Message);
				}

				return
					new JSONEngineDataV3(
						GetDummyJSONStructure(),
						Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_EngineFile).Value<string>()) +
						MissingFileSuffix);
			}
		}

		#region IInputDataProvider

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle
		{
			get { return VehicleInputData; }
		}

		public virtual IEngineeringJobInputData JobInputData
		{
			get { return this; }
		}

		public void ValidateComponentHashes()
		{
			
		}

		public XElement XMLHash
		{
			get { return new XElement(XMLNames.DI_Signature); }
		}

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData
		{
			get { return this; }
		}

		public virtual IVehicleEngineeringInputData VehicleInputData
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage(
				"Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (VehicleData == null) {
					throw new InvalidFileFormatException("VehicleData not found ");
				}

				return VehicleData;
			}
		}

		public virtual IEngineEngineeringInputData EngineOnly
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage(
				"Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (Engine == null) {
					throw new InvalidFileFormatException("EngineData not found");
				}

				return Engine;
			}
		}

		public virtual TableData PTOCycleWhileDrive
		{
			get { return null; }
		}

		IDriverEngineeringInputData IEngineeringInputDataProvider.DriverInputData
		{
			get { return this; }
		}

		#endregion

		#region IJobInputData

		public virtual IVehicleEngineeringInputData Vehicle
		{
			get { return VehicleData; }
		}

		public virtual IList<ICycleData> Cycles
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage(
				"Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				var retVal = new List<ICycleData>();
				if (Body[JsonKeys.Job_Cycles] == null) {
					return retVal;
				}

				foreach (var cycle in Body.GetEx(JsonKeys.Job_Cycles)) {
					//.Select(cycle => 
					var cycleFile = Path.Combine(BasePath, cycle.Value<string>());
					TableData cycleData;
					if (File.Exists(cycleFile)) {
						cycleData = VectoCSVFile.Read(cycleFile);
					} else {
						try {
							var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
												cycle.Value<string>() + Constants.FileExtensions.CycleFile;
							cycleData = VectoCSVFile.ReadStream(
								RessourceHelper.ReadStream(resourceName),
								source: resourceName);
						} catch (Exception e) {
							Log.Debug("Driving Cycle could not be read: " + cycleFile);
							if (!TolerateMissing) {
								throw new VectoException("Driving Cycle could not be read: " + cycleFile, e);
							}

							cycleData = new TableData(cycleFile + MissingFileSuffix, DataSourceType.Missing);
						}
					}

					retVal.Add(
						new CycleInputData() {
							Name = Path.GetFileNameWithoutExtension(cycle.Value<string>()),
							CycleData = cycleData
						});
				}

				return retVal;
			}
		}

		public virtual VectoSimulationJobType JobType => Body.GetEx(JsonKeys.Job_EngineOnlyMode).Value<bool>() ? VectoSimulationJobType.EngineOnlySimulation : VectoSimulationJobType.ConventionalVehicle;

		public virtual string JobName
		{
			get { return _jobname; }
		}

		public string ShiftStrategy {
			get {
				if (Body["ShiftStrategy"] == null) {
					return "";
				}

				return Body.GetEx<string>("ShiftStrategy");
			}
		}


		#endregion

		#region DriverInputData

		public virtual ILookaheadCoastingInputData Lookahead
		{
			get {
				if (Body[JsonKeys.DriverData_LookaheadCoasting] == null) {
					return null;
				}

				var lac = Body.GetEx(JsonKeys.DriverData_LookaheadCoasting);
				var distanceScalingFactor = lac["PreviewDistanceFactor"] != null
					? lac.GetEx<double>("PreviewDistanceFactor")
					: DeclarationData.Driver.LookAhead.LookAheadDistanceFactor;
				var lacDfOffset = lac["DF_offset"] != null
					? lac.GetEx<double>("DF_offset")
					: DeclarationData.Driver.LookAhead.DecisionFactorCoastingOffset;
				var lacDfScaling = lac["DF_scaling"] != null
					? lac.GetEx<double>("DF_scaling")
					: DeclarationData.Driver.LookAhead.DecisionFactorCoastingScaling;
				var speedDependentLookup = GetSpeedDependentLookupTable(lac);
				var velocityDropLookup = GetVelocityDropLookupTable(lac);
				var minSpeed = lac["MinSpeed"] != null
					? lac.GetEx<double>(JsonKeys.DriverData_Lookahead_MinSpeed).KMPHtoMeterPerSecond()
					: DeclarationData.Driver.LookAhead.MinimumSpeed;
				return new LookAheadCoastingInputData() {
					Enabled = lac.GetEx<bool>(JsonKeys.DriverData_Lookahead_Enabled),

					//Deceleration = lac.GetEx<double>(JsonKeys.DriverData_Lookahead_Deceleration).SI<MeterPerSquareSecond>(),
					MinSpeed = minSpeed,
					LookaheadDistanceFactor = distanceScalingFactor,
					CoastingDecisionFactorOffset = lacDfOffset,
					CoastingDecisionFactorScaling = lacDfScaling,
					CoastingDecisionFactorTargetSpeedLookup = speedDependentLookup,
					CoastingDecisionFactorVelocityDropLookup = velocityDropLookup
				};
			}
		}

		private TableData GetVelocityDropLookupTable(JToken lac)
		{
			if (lac["Df_velocityDropLookup"] == null ||
				string.IsNullOrWhiteSpace(lac["Df_velocityDropLookup"].Value<string>())) {
				return null;
			}

			try {
				return ReadTableData(
					lac.GetEx<string>("Df_velocityDropLookup"),
					"Lookahead Coasting Decisionfactor - Velocity drop");
			} catch (Exception) {
				if (TolerateMissing) {
					return
						new TableData(
							Path.Combine(BasePath, lac["Df_velocityDropLookup"].Value<string>()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}

			return null;
		}

		private TableData GetSpeedDependentLookupTable(JToken lac)
		{
			if (lac["DF_targetSpeedLookup"] == null ||
				string.IsNullOrWhiteSpace(lac["DF_targetSpeedLookup"].Value<string>())) {
				return null;
			}

			try {
				return ReadTableData(
					lac.GetEx<string>("DF_targetSpeedLookup"),
					"Lookahead Coasting Decisionfactor - Target speed");
			} catch (Exception) {
				if (TolerateMissing) {
					return
						new TableData(
							Path.Combine(BasePath, lac["DF_targetSpeedLookup"].Value<string>()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}

			return null;
		}

		public virtual IOverSpeedEngineeringInputData OverSpeedData {
			get {
				var overspeed = Body.GetEx(JsonKeys.DriverData_OverspeedEcoRoll);
				return new OverSpeedInputData() {
					Enabled = DriverData.ParseDriverMode(
						overspeed.GetEx<string>(JsonKeys.DriverData_OverspeedEcoRoll_Mode)),
					MinSpeed = overspeed.GetEx<double>(JsonKeys.DriverData_OverspeedEcoRoll_MinSpeed)
						.KMPHtoMeterPerSecond(),
					OverSpeed = overspeed.GetEx<double>(JsonKeys.DriverData_OverspeedEcoRoll_OverSpeed)
						.KMPHtoMeterPerSecond(),
				};
			}
		}

		public virtual IDriverAccelerationData AccelerationCurve
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage(
				"Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				var acceleration = Body[JsonKeys.DriverData_AccelerationCurve];
				if (acceleration == null || EmptyOrInvalidFileName(acceleration.Value<string>())) {
					return null;

					//					throw new VectoException("AccelerationCurve (VACC) required");
				}

				try {
					return new DriverAccelerationInputData() {
						AccelerationCurve = ReadTableData(acceleration.Value<string>(), "DriverAccelerationCurve")
					};
				} catch (VectoException e) {
					Log.Warn("Could not find file for acceleration curve. Trying lookup in declaration data.");
					try {
						var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".VACC." +
											acceleration.Value<string>() +
											Constants.FileExtensions.DriverAccelerationCurve;
						return new DriverAccelerationInputData() {
							AccelerationCurve = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName)
						};
					} catch (Exception) {
						if (!TolerateMissing) {
							throw new VectoException("Failed to read Driver Acceleration Curve: " + e.Message, e);
						}

						return new DriverAccelerationInputData() {
							AccelerationCurve = new TableData(
								Path.Combine(BasePath, acceleration.Value<string>()) + MissingFileSuffix,
								DataSourceType.Missing)
						};
					}
				}
			}
		}

		#endregion

		#region IAuxiliariesEngineeringInputData

		IAuxiliaryEngineeringInputData IAuxiliariesEngineeringInputData.Auxiliaries
		{
			get { return new EngineeringAuxiliaryDataInputData() {
					ElectricPowerDemand = Body["Padd_electric"] != null ? Body.GetEx<double>("Padd_electric").SI<Watt>() : 0.SI<Watt>(),
					ConstantPowerDemand = Body["Padd"] != null ? Body.GetEx<double>("Padd").SI<Watt>() : 0.SI<Watt>(),
					PowerDemandICEOffDriving = Body["Paux_ICEOff_Driving"] != null ? Body.GetEx<double>("Paux_ICEOff_Driving").SI<Watt>() : 0.SI<Watt>(),
					PowerDemandICEOffStandstill = Body["Paux_ICEOff_Standstill"] != null ? Body.GetEx<double>("Paux_ICEOff_Standstill").SI<Watt>() : 0.SI<Watt>()
				};
			}
		}

		IList<IAuxiliaryDeclarationInputData> IAuxiliariesDeclarationInputData.Auxiliaries
		{
			get { return AuxData().Cast<IAuxiliaryDeclarationInputData>().ToList(); }
		}

		public IBusAuxiliariesEngineeringData BusAuxiliariesData {
			get {
				if (Body["BusAux"] == null) {
					return null;
				}

				return _busAux ?? (_busAux = JSONInputDataFactory.ReadEngineeringBusAuxiliaries(Path.Combine(BasePath, Body.GetEx<string>("BusAux"))));
			}
		}

		protected virtual IList<IAuxiliaryDeclarationInputData> AuxData()
		{
			var retVal = new List<IAuxiliaryDeclarationInputData>();
			foreach (var aux in Body["Aux"] ?? Enumerable.Empty<JToken>()) {
				var type = AuxiliaryTypeHelper.Parse(aux.GetEx<string>("Type"));

				var auxData = new DeclarationAuxiliaryDataInputData {
					ID = aux.GetEx<string>("ID"),
					Type = type,
					Technology = new List<string>(),
				};
				var tech = aux.GetEx<string>("Technology");

				if (auxData.Type == AuxiliaryType.ElectricSystem) {
					if (aux["TechList"] == null || aux["TechList"].Any()) {
						auxData.Technology.Add("Standard technology");
					} else {
						auxData.Technology.Add("Standard technology - LED headlights, all");
					}
				}

				if (auxData.Type == AuxiliaryType.SteeringPump) {
					auxData.Technology.Add(tech);
				}

				if (auxData.Type == AuxiliaryType.Fan) {
					auxData.Technology.Add(MapLegacyFanTechnologies(tech));
				}

				retVal.Add(auxData);
			}

			return retVal;
		}

		private static string MapLegacyFanTechnologies(string tech)
		{
			string newTech;
			switch (tech) {
				case "Crankshaft mounted - Electronically controlled visco clutch (Default)":
					newTech = "Crankshaft mounted - Electronically controlled visco clutch";
					break;
				case "Crankshaft mounted - On/Off clutch":
					newTech = "Crankshaft mounted - On/off clutch";
					break;
				case "Belt driven or driven via transm. - On/Off clutch":
					newTech = "Belt driven or driven via transm. - On/off clutch";
					break;
				default:
					newTech = tech;
					break;
			}

			return newTech;
		}

		#endregion

		#region AdvancedAuxiliaries

		public AuxiliaryModel AuxiliaryAssembly
		{
			get {
				return AuxiliaryModelHelper.Parse(
					Body["AuxiliaryAssembly"] == null
						? ""
						: Body["AuxiliaryAssembly"].ToString());
			}
		}

		public string AuxiliaryVersion
		{
			get { return Body["AuxiliaryVersion"] != null ? Body["AuxiliaryVersion"].Value<string>() : "<CLASSIC>"; }
		}

		public string AdvancedAuxiliaryFilePath
		{
			get {
				return Body["AdvancedAuxiliaryFilePath"] != null
					? Path.Combine(Path.GetFullPath(BasePath), Body["AdvancedAuxiliaryFilePath"].Value<string>())
					: "";
			}
		}

		#endregion
	}

	public class JSONInputDataV3 : JSONInputDataV2
	{
		public JSONInputDataV3(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) { }

		protected override IList<IAuxiliaryDeclarationInputData> AuxData()
		{
			var retVal = new List<IAuxiliaryDeclarationInputData>();

			foreach (var aux in Body["Aux"] ?? Enumerable.Empty<JToken>()) {
				try {
					aux.GetEx("Technology").ToObject<List<string>>();
				} catch (Exception) {
					throw new VectoException(
						"Aux: Technology for aux '{0}' list could not be read. Maybe it is a single string instead of a list of strings?",
						aux.GetEx<string>("ID"));
				}

				var type = AuxiliaryTypeHelper.Parse(aux.GetEx<string>("Type"));

				var auxData = new DeclarationAuxiliaryDataInputData {
					ID = aux.GetEx<string>("ID"),
					Type = type,
					Technology = aux.GetEx("Technology").ToObject<List<string>>()
				};


				retVal.Add(auxData);

			}

			return retVal;
		}
	}


	public class JSONInputDataV4 : JSONInputDataV3
	{
		public JSONInputDataV4(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) { }

		public override TableData PTOCycleWhileDrive
		{
			get { return Body["PTOCycleDuringDrive"] != null ? VectoCSVFile.Read(Path.Combine(BasePath, Body.GetEx<string>("PTOCycleDuringDrive"))) : null; }
		}

		public override IGearshiftEngineeringInputData GearshiftInputData =>
			Body["TCU"] == null
				? Body["GearboxFile"] == null ? null : JSONInputDataFactory.ReadShiftParameters(Path.Combine(BasePath, Body.GetEx<string>("GearboxFile")), false)
				: JSONInputDataFactory.ReadShiftParameters(Path.Combine(BasePath, Body.GetEx<string>("TCU")), false);

	}


	public class JSONInputDataV5 : JSONInputDataV4
	{
		protected IEngineStopStartEngineeringInputData engineStopStartData;
		protected IEcoRollEngineeringInputData ecoRollData;
		protected IPCCEngineeringInputData pccData;

		public JSONInputDataV5(JObject data, string filename, bool tolerateMissing = false) : base(
			data, filename, tolerateMissing)
		{ }

		#region Overrides of JSONInputDataV2

		public override IEngineStopStartEngineeringInputData EngineStopStartData =>
			engineStopStartData ?? (engineStopStartData = new EngineStopStartInputData {
				MaxEngineOffTimespan = Body["EngineStopStartMaxOffTimespan"] == null
					? null
					: Body.GetEx<double>("EngineStopStartMaxOffTimespan").SI<Second>(),
				UtilityFactorStandstill = Body["EngineStopStartUtilityFactor"] == null
					? DeclarationData.Driver.EngineStopStart.UtilityFactor
					: Body.GetEx<double>("EngineStopStartUtilityFactor"),
				UtilityFactorDriving = Body["EngineStopStartUtilityFactorDriving"] == null
					? (Body["EngineStopStartUtilityFactor"] == null
						? DeclarationData.Driver.EngineStopStart.UtilityFactor
						: Body.GetEx<double>("EngineStopStartUtilityFactor"))
					: Body.GetEx<double>("EngineStopStartUtilityFactorDriving"),
				ActivationDelay = Body["EngineStopStartAtVehicleStopThreshold"] == null
					? null
					: Body.GetEx<double>("EngineStopStartAtVehicleStopThreshold").SI<Second>()
			});


		public override IEcoRollEngineeringInputData EcoRollData =>
			ecoRollData ?? (ecoRollData = new EcoRollInputData {
				UnderspeedThreshold = Body["EcoRollUnderspeedThreshold"] == null
					? null
					: Body.GetEx<double>("EcoRollUnderspeedThreshold").KMPHtoMeterPerSecond(),
				MinSpeed = Body["EcoRollMinSpeed"] == null
					? null
					: Body.GetEx<double>("EcoRollMinSpeed").KMPHtoMeterPerSecond(),
				ActivationDelay = Body["EcoRollActivationDelay"] == null
					? null
					: Body.GetEx<double>("EcoRollActivationDelay").SI<Second>(),
				AccelerationUpperLimit = Body["EcoRollMaxAcceleration"] == null ? null : Body.GetEx<double>("EcoRollMaxAcceleration").SI<MeterPerSquareSecond>()
			});

		public override IPCCEngineeringInputData PCCData =>
			pccData ?? (pccData = new PCCInputData() {
				PCCEnabledSpeed = Body["PCCEnableSpeed"] == null ? null : Body.GetEx<double>("PCCEnableSpeed").KMPHtoMeterPerSecond(),
				MinSpeed = Body["PCCMinSpeed"] == null ? null : Body.GetEx<double>("PCCMinSpeed").KMPHtoMeterPerSecond(),
				Underspeed = Body["PCCUnderspeed"] == null ? null : Body.GetEx<double>("PCCUnderspeed").KMPHtoMeterPerSecond(),
				OverspeedUseCase3 = Body["PCCOverspeed"] == null ? null : Body.GetEx<double>("PCCOverspeed").KMPHtoMeterPerSecond(),
				PreviewDistanceUseCase1 = Body["PCCPreviewDistanceUC1"] == null ? null : Body.GetEx<double>("PCCPreviewDistanceUC1").SI<Meter>(),
				PreviewDistanceUseCase2 = Body["PCCPreviewDistanceUC2"] == null ? null : Body.GetEx<double>("PCCPreviewDistanceUC2").SI<Meter>()
			});

		#endregion
	}


	public class EcoRollInputData : IEcoRollEngineeringInputData
	{
		#region Implementation of IEcoRollEngineeringInputData

		public MeterPerSecond MinSpeed { get; set; }
		public Second ActivationDelay { get; set; }
		public MeterPerSecond UnderspeedThreshold { get; set; }

		public MeterPerSquareSecond AccelerationUpperLimit { get; set; }

		#endregion
	}

	public class EngineStopStartInputData : IEngineStopStartEngineeringInputData
	{
		#region Implementation of IEngineStopStartEngineeringInputData

		public Second ActivationDelay { get; set; }

		public Second MaxEngineOffTimespan { get; set; }

		public double UtilityFactorStandstill { get; set; }

		public double UtilityFactorDriving { get; set; }

		#endregion
	}

	public class PCCInputData : IPCCEngineeringInputData
	{
		#region Implementation of IPCCEngineeringInputData

		public MeterPerSecond PCCEnabledSpeed { get; set; }
		public MeterPerSecond MinSpeed { get; set; }
		public Meter PreviewDistanceUseCase1 { get; set; }
		public Meter PreviewDistanceUseCase2 { get; set; }
		public MeterPerSecond Underspeed { get; set; }
		public MeterPerSecond OverspeedUseCase3 { get; set; }

		#endregion
	}


	public class JSONVTPInputDataV4 : JSONFile, IVTPEngineeringInputDataProvider, IVTPEngineeringJobInputData,
		IVTPDeclarationInputDataProvider, IManufacturerReport
	{
		private IDictionary<VectoComponents, IList<string>> _componentDigests = null;
		private DigestData _jobDigest = null;
		private IXMLInputDataReader _inputReader;

		public JSONVTPInputDataV4(JObject data, string filename, bool tolerateMissing = false) : base(
			data, filename, tolerateMissing)
		{
			VectoJobHash = VectoHash.Load(
				Path.Combine(Path.GetFullPath(BasePath), Body["DeclarationVehicle"].Value<string>()));
			VectoManufacturerReportHash = Body["ManufacturerRecord"] != null ? VectoHash.Load(
				Path.Combine(Path.GetFullPath(BasePath), Body["ManufacturerRecord"].Value<string>())) : null;

			var kernel = new StandardKernel(new VectoNinjectModule());
			_inputReader = kernel.Get<IXMLInputDataReader>();
		}

		public IVTPEngineeringJobInputData JobInputData
		{
			get { return this; }
		}

		public IManufacturerReport ManufacturerReportInputData
		{
			get { return this; }
		}

		public IVehicleDeclarationInputData Vehicle
		{
			get {
				return _inputReader.CreateDeclaration(
					Path.Combine(Path.GetFullPath(BasePath), Body["DeclarationVehicle"].Value<string>())).JobInputData.Vehicle;
			}
		}

		public IVectoHash VectoJobHash { get; }

		public IVectoHash VectoManufacturerReportHash { get; }

		public Meter Mileage
		{
			get { return Body.GetEx<double>("Mileage").SI(Unit.SI.Kilo.Meter).Cast<Meter>(); }
		}

		string IManufacturerReport.Source { get { return Body["ManufacturerRecord"].Value<string>(); } }

		public IList<ICycleData> Cycles
		{
			get {
				var retVal = new List<ICycleData>();
				if (Body[JsonKeys.Job_Cycles] == null) {
					return retVal;
				}

				foreach (var cycle in Body.GetEx(JsonKeys.Job_Cycles)) {
					var cycleFile = Path.Combine(BasePath, cycle.Value<string>());
					if (File.Exists(cycleFile)) {
						var cycleData = VectoCSVFile.Read(cycleFile);
						retVal.Add(
							new CycleInputData() {
								Name = Path.GetFileNameWithoutExtension(cycle.Value<string>()),
								CycleData = cycleData
							});
					}
				}

				return retVal;
			}
		}

		public IEnumerable<double> FanPowerCoefficents
		{
			get { return Body.GetEx("FanPowerCoefficients").Select(entry => entry.ToString().ToDouble()).ToList(); }
		}

		public Meter FanDiameter
		{
			get { return Body.GetEx<double>("FanDiameter").SI<Meter>(); }
		}

		public IList<IFuelNCVData> FuelNCVs
		{
			get 
			{
				var fuelNCVs = new List<IFuelNCVData>();

				if (Body[JsonKeys.Job_FuelNCVs] == null) {
					return fuelNCVs;
                }

				foreach (var fuelNCV in Body.GetEx(JsonKeys.Job_FuelNCVs)) {
					var type = fuelNCV.GetEx<string>(JsonKeys.Job_FuelNCV_Type);
					var ncv = fuelNCV.GetEx<double>(JsonKeys.Job_FuelNCV_NCV);

					IEnumerable<FuelType> matches = Enum.GetValues(typeof(FuelType)).Cast<FuelType>().Where(x => x.GetLabel().Equals(type));
					if (matches.Count() == 0) {
						throw new Exception($"{JsonKeys.Job_FuelNCVs}: invalid fuel type: {type}");
                    }

					FuelType fuelType = matches.First();

					fuelNCVs.Add(new FuelNCVData() { Type = fuelType, NCV = ncv });
                }

				return fuelNCVs;
            }
        }

		#region Implementation of IVTPDeclarationInputDataProvider

		IVTPDeclarationJobInputData IVTPDeclarationInputDataProvider.JobInputData
		{
			get { return JobInputData; }
		}

		#endregion

		#region Implementation of IManufacturerReport

		IDictionary<VectoComponents, IList<string>> IManufacturerReport.ComponentDigests
		{
			get {
				if (_componentDigests == null) {
					ReadManufacturerReport();
				}
				return _componentDigests;
			}
		}

		public DigestData JobDigest
		{
			get {
				if (_jobDigest == null) {
					ReadManufacturerReport();
				}
				return _jobDigest;
			}
		}

		#endregion

		private void ReadManufacturerReport()
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(Path.Combine(Path.GetFullPath(BasePath), Body["ManufacturerRecord"].Value<string>()));
			var components = XMLManufacturerReportReader.GetContainingComponents(xmlDoc).GroupBy(s => s)
														.Select(g => new { Entry = g.Key, Count = g.Count() });
			_componentDigests = new Dictionary<VectoComponents, IList<string>>();

			try {
				foreach (var component in components) {
					if (component.Entry == VectoComponents.Vehicle) {
						continue;
					}

					for (var i = 0; i < component.Count; i++) {
						if (!_componentDigests.ContainsKey(component.Entry)) {
							_componentDigests[component.Entry] = new List<string>();
						}
						_componentDigests[component.Entry].Add(
							XMLManufacturerReportReader.GetComponentDataDigestValue(xmlDoc, component.Entry, i));
					}
				}
			} catch (Exception) { }

			try {
				_jobDigest = new DigestData(xmlDoc.SelectSingleNode("//*[local-name()='InputDataSignature']"));
			} catch (Exception) {
				_jobDigest = new DigestData("", new string[] {},"","" );
			}
		}
	}
}

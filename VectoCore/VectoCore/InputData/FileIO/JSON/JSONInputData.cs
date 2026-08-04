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
using Newtonsoft.Json;
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
using TUGraz.VectoCore.Models.Declaration.Auxiliaries;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using TUGraz.VectoHashing.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using Castle.Core.Internal;
using TUGraz.VectoCore.Ninject;

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
			Version = header[JsonKeys.JsonHeader_FileVersion] != null
				? header.GetEx<string>(JsonKeys.JsonHeader_FileVersion)
				: string.Empty;
			Body = (JObject)data.GetEx(JsonKeys.JsonBody);
			_sourceFile = Path.GetFullPath(filename);
			TolerateMissing = tolerateMissing;
		}

		protected internal bool TolerateMissing { get; set; }

		public DataSource DataSource => new DataSource { SourceType = DataSourceType.JSONFile, SourceFile = _sourceFile, SourceVersion = Version };

		public string Source => _sourceFile;

		public virtual bool SavedInDeclarationMode => Body.GetEx(JsonKeys.SavedInDeclMode).Value<bool>();

		public virtual string AppVersion => "VECTO-JSON";

		internal string BasePath => Path.GetDirectoryName(_sourceFile);

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

		public static VectoSimulationJobType ParsePowertrainType(JToken json, string field)
		{
			switch (json.GetEx<String>(field))
			{
				case "ParallelHybrid": return VectoSimulationJobType.ParallelHybridVehicle;
				case "BatteryElectric": return VectoSimulationJobType.BatteryElectricVehicle;
				case "SerialHybrid": return VectoSimulationJobType.SerialHybridVehicle;
				case "IEPC_E":
				case "IEPC": return VectoSimulationJobType.IEPC_E;
				case "IEPC_S":
				case "IEPC-S": return VectoSimulationJobType.IEPC_S;
				case "IHPC": return VectoSimulationJobType.IHPC;
				case "Multiple_FCHV": return VectoSimulationJobType.Multiple_FCHV;
				case "Mulitple_SHEV": return VectoSimulationJobType.Multiple_SHEV;
				case "Multiple_PEV": return VectoSimulationJobType.Multiple_PEV;
                default: throw new VectoException("Invalid parameter value {0}", json.GetEx<String>(field));
			}
		}
	}

	/// <summary>
	/// Class for reading json data of vecto-job-file.
	/// Fileformat: .vecto
	/// </summary>
	public abstract class AbstractJSONInputData : JSONFile, IEngineeringInputDataProvider, IDeclarationInputDataProvider,
		IEngineeringJobInputData, IDriverEngineeringInputData, IAuxiliariesEngineeringInputData,
		IAuxiliariesDeclarationInputData, IJSONVehicleComponents
	{
		public AbstractJSONInputData(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing)
		{
			_jobname = Path.GetFileNameWithoutExtension(filename);
		}


		public virtual IGearboxEngineeringInputData Gearbox { get; internal set; }

		public virtual IGearshiftEngineeringInputData GearshiftInputData { get; internal set; }

		public virtual IEngineStopStartEngineeringInputData EngineStopStartData => null;

		public virtual IEcoRollEngineeringInputData EcoRollData => null;

		public virtual IPCCEngineeringInputData PCCData => null;


		public IAxleGearInputData AxleGear { get; internal set; }
		public ITorqueConverterEngineeringInputData TorqueConverter { get; internal set; }
		public IEngineEngineeringInputData Engine { get; internal set; }


		protected IVehicleEngineeringInputData VehicleData;

		private readonly string _jobname;
		private IBusAuxiliariesEngineeringData _busAux;


		public IAuxiliariesEngineeringInputData EngineeringAuxiliaries => this;

		public IAuxiliariesDeclarationInputData DeclarationAuxiliaries => this;

		protected IVehicleEngineeringInputData ReadVehicle()
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

		protected IGearboxEngineeringInputData ReadGearbox()
		{
			try {
				var gearboxFile = Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>();

				return JSONInputDataFactory.ReadGearbox(Path.Combine(BasePath, gearboxFile));
			} catch (Exception e) {
				if (!TolerateMissing) {
					throw new VectoException("JobFile: Failed to read Gearbox file '{0}': {1}", e,
						Body[JsonKeys.Vehicle_GearboxFile], e.Message);
				}

				return new JSONGearboxDataV6(GetDummyJSONStructure(),
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>())
					+ MissingFileSuffix);
			}
		}

		protected IEngineEngineeringInputData ReadEngine()
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

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle => VehicleInputData;

		public virtual IHybridStrategyParameters HybridStrategyParameters => null;

		public virtual IEngineeringJobInputData JobInputData => this;

		public virtual IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData => null;

		public XElement XMLHash => XMLHelper.CreateDummySig("http://www.w3.org/2000/09/xmldsig#"); //new XElement(XMLNames.DI_Signature);

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData => this;

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

		public virtual TableData PTOCycleWhileDrive => null;

		IDriverEngineeringInputData IEngineeringInputDataProvider.DriverInputData => this;

		#endregion

		#region IJobInputData

		public virtual IVehicleEngineeringInputData Vehicle => VehicleData;

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

		public virtual string JobName => _jobname;

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

		public virtual IOverSpeedEngineeringInputData OverSpeedData
		{
			get {
				if (!Body.ContainsKey(JsonKeys.DriverData_OverspeedEcoRoll)) {
					return new OverSpeedInputData() {
						Enabled = true,
						MinSpeed = DeclarationData.Driver.OverSpeed.MinSpeed,
						OverSpeed = DeclarationData.Driver.OverSpeed.AllowedOverSpeed
					};
				}
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

		IAuxiliaryEngineeringInputData IAuxiliariesEngineeringInputData.Auxiliaries =>
			new EngineeringAuxiliaryDataInputData() {
				ElectricPowerDemand = Body["Padd_electric"] != null ? Body.GetEx<double>("Padd_electric").SI<Watt>() : 0.SI<Watt>(),
				ConstantPowerDemand = Body["Padd"] != null ? Body.GetEx<double>("Padd").SI<Watt>() : 0.SI<Watt>(),
				PowerDemandICEOffDriving = Body["Paux_ICEOff_Driving"] != null ? Body.GetEx<double>("Paux_ICEOff_Driving").SI<Watt>() : 0.SI<Watt>(),
				PowerDemandICEOffStandstill = Body["Paux_ICEOff_Standstill"] != null ? Body.GetEx<double>("Paux_ICEOff_Standstill").SI<Watt>() : 0.SI<Watt>()
			};

		public IBusAuxiliariesEngineeringData BusAuxiliariesData
		{
			get
			{
				if (Body["BusAux"] == null) {
					return null;
				}

				return _busAux ?? (_busAux = JSONInputDataFactory.ReadEngineeringBusAuxiliaries(Path.Combine(BasePath, Body.GetEx<string>("BusAux"))));
			}
		}

		IList<IAuxiliaryDeclarationInputData> IAuxiliariesDeclarationInputData.Auxiliaries => AuxData();

		protected virtual IList<IAuxiliaryDeclarationInputData> AuxData()
		{
			var retVal = new List<IAuxiliaryDeclarationInputData>();

			foreach (var aux in Body["Aux"] ?? Enumerable.Empty<JToken>())
			{
				try
				{
					aux.GetEx("Technology").ToObject<List<string>>();
				}
				catch (Exception)
				{
					throw new VectoException(
						"Aux: Technology for aux '{0}' list could not be read. Maybe it is a single string instead of a list of strings?",
						aux.GetEx<string>("ID"));
				}

				var type = AuxiliaryTypeHelper.Parse(aux.GetEx<string>("Type"));

				var auxData = new DeclarationAuxiliaryDataInputData
				{
					ID = aux.GetEx<string>("ID"),
					Type = type,
					Technology = aux.GetEx("Technology").ToObject<List<string>>()
				};


				retVal.Add(auxData);

			}

			return retVal;
		}

		#endregion

	}

	public class JSONInputDataV2 : AbstractJSONInputData
	{
		public JSONInputDataV2(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing)
		{
			Engine = ReadEngine();

			if (Body.GetEx(JsonKeys.Job_EngineOnlyMode).Value<bool>())
			{
				return;
			}

			Gearbox = ReadGearbox();
			AxleGear = Gearbox as IAxleGearInputData;
			TorqueConverter = Gearbox as ITorqueConverterEngineeringInputData;
			GearshiftInputData = Gearbox as IGearshiftEngineeringInputData;

			VehicleData = ReadVehicle();
		}
	}

	public class JSONInputDataV3 : JSONInputDataV2
	{
		public JSONInputDataV3(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) { }


	}


	public class JSONInputDataV4 : JSONInputDataV3
	{
		public JSONInputDataV4(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) { }

		public override TableData PTOCycleWhileDrive => Body["PTOCycleDuringDrive"] != null ? VectoCSVFile.Read(Path.Combine(BasePath, Body.GetEx<string>("PTOCycleDuringDrive"))) : null;

		public override IGearshiftEngineeringInputData GearshiftInputData =>
			Body["TCU"] == null
				? null
				: JSONInputDataFactory.ReadShiftParameters(Path.Combine(BasePath, Body.GetEx<string>("TCU")), false);
	}

	public class JSONVTPInputDataV4 : JSONFile, IVTPEngineeringInputDataProvider, IVTPEngineeringJobInputData,
		IVTPDeclarationInputDataProvider, IManufacturerReport, ICompletedVIF
	{
		private Meter _vehicleLength;
		private string _coolingFanTech;
		private AirdragData _airDragData;
		private VehicleCode _bodyworkCode;

		private IXMLInputDataReader _xmlInputReader;
		private IResultsInputData _manufacturerResults;
		private IDeclarationJobInputData _declarationJobInputData;
		private IXMLMultistageInputDataProvider _completeVifInputData;

		private DigestData _jobDigest;
		private IDictionary<VectoComponents, IList<string>> _componentDigests;

		public JSONVTPInputDataV4(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing)
		{
			VerifyInput();

			var baseFullPath = Path.GetFullPath(BasePath);
			string declPath = Path.Combine(baseFullPath, Body["DeclarationVehicle"].Value<string>());
			string mrfPath  = Path.Combine(baseFullPath, Body["ManufacturerRecord"].Value<string>());
			string cifPath  = Path.Combine(baseFullPath, Body["CustomerInformationFile"]?.Value<string>() ?? string.Empty);
			string vifPath  = Path.Combine(baseFullPath, Body["PrimaryVIF"]?.Value<string>() ?? string.Empty);
			string completedVifPath = Path.Combine(baseFullPath, Body["CompletedVIF"]?.Value<string>() ?? string.Empty);

			_xmlInputReader = new StandardKernel(new VectoNinjectModule()).Get<IXMLInputDataReader>();
			_declarationJobInputData = _xmlInputReader.CreateDeclaration(declPath, true).JobInputData;

			if (Vehicle.VehicleCategory.IsBus())
			{
				VectoPrimaryVIFHash = Body["PrimaryVIF"] != null ? VectoHash.Load(vifPath) : null;
				VectoCompletedVIFHash = Body["CompletedVIF"] != null ? VectoHash.Load(completedVifPath) : null;
				
				_completeVifInputData = (IXMLMultistageInputDataProvider)_xmlInputReader.CreateDeclaration(completedVifPath, true);
			}

			VectoJobHash = VectoHash.Load(declPath);
			VectoManufacturerReportHash = Body["ManufacturerRecord"] != null ? VectoHash.Load(mrfPath) : null;
			VectoCustomerFileHash = Body["CustomerInformationFile"] != null ? VectoHash.Load(cifPath) : null;
			
			_bodyworkCode = VehicleCode.NOT_APPLICABLE;
		}

		public IVTPEngineeringJobInputData JobInputData => this;

		public IManufacturerReport ManufacturerReportInputData => this;

		public ICompletedVIF CompletedVIFInputData => this;

		public IVehicleDeclarationInputData Vehicle => _declarationJobInputData.Vehicle;

		public IVectoHash VectoJobHash { get; }

		public IVectoHash VectoManufacturerReportHash { get; }

		public IVectoHash VectoCustomerFileHash { get; }

		public IVectoHash VectoPrimaryVIFHash { get; }
		
		public IVectoHash VectoCompletedVIFHash { get; }

		public Meter Mileage => Body.GetEx<double>("Mileage").SI(Unit.SI.Kilo.Meter).Cast<Meter>();

		public VTPOBFCMDeclarationData OBFCMDeclarationInputData => new VTPOBFCMDeclarationData(Body);

		string IManufacturerReport.Source => Body["ManufacturerRecord"].Value<string>();

		string ICompletedVIF.Source => Body[JsonKeys.VTP_CompletedVIF]?.Value<string>();

		public IReportFile PrimaryVIFInputData => new ReportFile(Body["PrimaryVIF"]?.Value<string>());

		public IReportFile CIFInputData => new ReportFile(Body["CustomerInformationFile"]?.Value<string>());

		public IBusAuxiliariesDeclarationData BusAuxiliaries => _completeVifInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries;

		public Meter VehicleLength
		{
			get
			{
				if (_vehicleLength == null)
				{
					ReadCompletedVIF();
				}
				return _vehicleLength;
			}
		}

		public VehicleCode BodyworkCode
		{
			get
			{
				if (_bodyworkCode == VehicleCode.NOT_APPLICABLE)
				{
					ReadCompletedVIF();
				}
				return _bodyworkCode;
			}
		}

		public IResultsInputData Results
		{
			get {
				if (_manufacturerResults == null) {
					ReadManufacturerReport();
				}
				return _manufacturerResults;
			}
		}

		public double CoolingFanTechCoefficient 
		{
			get
			{
				if (_manufacturerResults == null)
				{
					ReadManufacturerReport();
				}

				return new FanBusCoolingCoefficient().GetTechnologyCoefficient(_coolingFanTech); 
			}
		}

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

		public Meter FanDiameter => Body.GetEx<double>("FanDiameter").SI<Meter>();

		public IList<IFuelNCVData> FuelNCVs
		{
			get
			{
				var fuelNCVs = new List<IFuelNCVData>();

				if (Body[JsonKeys.Job_FuelNCVs] == null) {
					throw new Exception($"Job input data: missing input field: {JsonKeys.Job_FuelNCVs}");
                }

				foreach (var fuelNCV in Body.GetEx(JsonKeys.Job_FuelNCVs)) {
					var type = fuelNCV.GetEx<string>(JsonKeys.Job_FuelNCV_Type);
					var ncv = fuelNCV.GetEx<double>(JsonKeys.Job_FuelNCV_NCV);

					IEnumerable<FuelType> matches = Enum.GetValues(typeof(FuelType)).Cast<FuelType>().Where(x => x.GetLabel().Equals(type));
					if (matches.Count() == 0) {
						throw new Exception($"Job input data: {JsonKeys.Job_FuelNCVs}: invalid {JsonKeys.Job_FuelNCV_Type}: {type}");
                    }

					fuelNCVs.Add(new FuelNCVData() { Type = matches.First(), NCV = (ncv * Constants.Mega).SI<JoulePerKilogramm>() });
                }

				var fuelsPerMode = JobInputData.Vehicle.Components.EngineInputData.EngineModes.Select(
					x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, JobInputData.Vehicle.TankSystem)));

				foreach (var fuels in fuelsPerMode) {
					foreach (var fuel in fuels) {
						if (fuelNCVs.Count(x => x.Type == fuel.FuelType) == 0) {
							throw new Exception($"Job input data: {JsonKeys.Job_FuelNCVs}: missing {JsonKeys.Job_FuelNCV_Type}: {fuel.FuelType.GetLabel()}");
						}
					}
				}

				return fuelNCVs;
            }
        }

		public NewtonMeter TorqueDriftLeftWheel
		{
			get { return Body.GetEx<double>(JsonKeys.Job_TorqueDriftLeftWheel).SI<NewtonMeter>(); }
        }

		public NewtonMeter TorqueDriftRightWheel
		{
			get { return Body.GetEx<double>(JsonKeys.Job_TorqueDriftRightWheel).SI<NewtonMeter>(); }
		}


        #region Implementation of IVTPDeclarationInputDataProvider

		IVTPDeclarationJobInputData IVTPDeclarationInputDataProvider.JobInputData => JobInputData;

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

		public AirdragData AirDragData
		{
			get
			{
				if (_airDragData == null)
				{
					_airDragData = ReadAirDragComponent();
				}

				return _airDragData;
			}
		}

		public void ValidateSimulationToolVersion()
		{
			var xmlDoc = XMLHelper.SecureLoadXML(Path.Combine(Path.GetFullPath(BasePath), Body["ManufacturerRecord"].Value<string>()));

            string simToolVersionStr = XMLManufacturerReportReader.ReadElementValue(xmlDoc, "SimulationToolVersion");
			string vectoVersionStr = VectoSimulationCore.VersionNumber;

			bool xmlVersionNewer = VersioningUtil.CompareVersions(simToolVersionStr, vectoVersionStr) > 0;

			#if !DEBUG
			if (xmlVersionNewer) {
				throw new VectoException($"Not allowed to run simulation because VECTO version ({vectoVersionStr}) is older than <SimulationToolVersion> in Manufacturer Report ({simToolVersionStr}).");
			}
			#endif
		}

		public void ValidateHash()
		{
			var xmlDoc = XMLHelper.SecureLoadXML(Path.Combine(Path.GetFullPath(BasePath), Body["ManufacturerRecord"].Value<string>()));

            var signatureNode = xmlDoc.SelectSingleNode("//*[local-name()='Signature']");
			var signatureDigest = new DigestData(signatureNode);

			var hash = XMLHashProvider.ComputeHash(xmlDoc, signatureDigest.Reference.Remove(0, 1), signatureDigest.CanonicalizationMethods,
				signatureDigest.DigestMethod);

			if (!hash.InnerText.Equals(signatureDigest.DigestValue)) {
				throw new VectoException($"Manufacturer Report hash: {signatureDigest.DigestValue} differs from calculated hash: {hash.InnerText}");
			}
		}

		#endregion

		private void VerifyInput()
		{
			string missingInputMsg = "Missing JSON parameter: {0}";
			
			string declVeh = Body["DeclarationVehicle"]?.Value<string>() 
				?? throw new ArgumentException(string.Format(missingInputMsg, "DeclarationVehicle"));
			_ = Body["ManufacturerRecord"]		
				?? throw new ArgumentException(string.Format(missingInputMsg, "ManufacturerRecord"));
			_ = Body["CustomerInformationFile"] 
				?? throw new ArgumentException(string.Format(missingInputMsg, "CustomerInformationFile"));

			string declPath = Path.Combine(Path.GetFullPath(BasePath), declVeh);
			var vehicle = new StandardKernel(new VectoNinjectModule())
				.Get<IXMLInputDataReader>()
				.CreateDeclaration(declPath, true)
				.JobInputData
				.Vehicle;

			if (vehicle.VehicleCategory.IsBus())
			{
				_ = Body["PrimaryVIF"]   ?? throw new ArgumentException(string.Format(missingInputMsg, "PrimaryVIF"));
				_ = Body["CompletedVIF"] ?? throw new ArgumentException(string.Format(missingInputMsg, "CompletedVIF"));
			}
		}

		private void ReadCompletedVIF()
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(Path.Combine(Path.GetFullPath(BasePath), Body[JsonKeys.VTP_CompletedVIF].Value<string>()));

			var code = xmlDoc.SelectSingleNode($"//*[local-name()='{XMLNames.Vehicle_BodyworkCode}']").InnerText;
			_bodyworkCode = code.ParseEnum<VehicleCode>();

			var length = xmlDoc.SelectSingleNode($"//*[local-name()='{XMLNames.Bus_VehicleLength}']").InnerText.ToDouble();
			_vehicleLength = (length / 1000).SI<Meter>();
		}

		private void ReadManufacturerReport()
		{
			var xmlDoc = XMLHelper.SecureLoadXML(Path.Combine(Path.GetFullPath(BasePath), Body["ManufacturerRecord"].Value<string>()));

			var components = XMLManufacturerReportReader.GetContainingComponents(xmlDoc).GroupBy(s => s)
														.Select(g => new { Entry = g.Key, Count = g.Count() });
			_componentDigests = new Dictionary<VectoComponents, IList<string>>();

			try {
				foreach (var component in components) {
					if (component.Entry == VectoComponents.Vehicle) {
						continue;
					}

					for (var i = 0; i < component.Count; i++) {
						_componentDigests.GetOrAdd(component.Entry, _ => new List<string>()).Add(
							XMLManufacturerReportReader.GetComponentDataDigestValue(xmlDoc, component.Entry, i));
					}
				}
			} catch (Exception) {
				// todo mk2021-08-26 really suppress all errors?
			}

			try {
				_jobDigest = new DigestData(xmlDoc.SelectSingleNode("//*[local-name()='InputDataSignature']"));
			} catch (Exception) {
				_jobDigest = new DigestData("", new string[] { }, "", "");
			}

			_manufacturerResults = new ManufacturerResults(xmlDoc.SelectSingleNode("//*[local-name() = 'Results']"));

			_coolingFanTech = xmlDoc.SelectSingleNode("//*[local-name() = 'Auxiliaries']//*[local-name()='CoolingFanTechnology']")?.InnerText 
				?? throw new ArgumentException("Auxiliary fan technology is missing. Please provide a Primary Manufacturer Report file.");

		}

		private AirdragData ReadAirDragComponent()
		{
			Segment segment = Vehicle.VehicleCategory.IsBus() ?
				DeclarationData.PrimaryBusSegments.Lookup(
				Vehicle.VehicleCategory,
				Vehicle.AxleConfiguration,
				Vehicle.Articulated) :
				DeclarationData.TruckSegments.Lookup(
				Vehicle.VehicleCategory,
				Vehicle.AxleConfiguration,
				Vehicle.GrossVehicleMassRating,
				Vehicle.CurbMassChassis,
				Vehicle.VocationalVehicle);

			string model = null;
			string certificationNumber = null;
			SquareMeter cdxa = segment.Missions.First().DefaultCDxA;
			CertificationMethod certificationMethod = CertificationMethod.StandardValues;

			var xmlDoc = XMLHelper.SecureLoadXML(Path.Combine(Path.GetFullPath(BasePath), Body["CompletedVIF"].Value<string>()));
			var mrfSteps = xmlDoc
				.SelectNodes($"//*[local-name() = 'VectoOutputMultistep']//*[local-name()='ManufacturingStep']")
				.Cast<XmlNode>()
				.ToList();

			for (int i = 0; i < mrfSteps.Count; i++)
			{
				string airDragXPath =
					"//*[local-name()='Data']" +
					"//*[local-name()='Vehicle']" +
					"//*[local-name()='Components']" +
					"//*[local-name()='AirDrag']" +
					"//*[local-name()='Data']";

				var mrfStep = mrfSteps.SingleOrDefault(s => s.Attributes.GetNamedItem("stepCount").Value.ToInt() == i + 2);
				var airDragComponent = mrfStep.SelectSingleNode(airDragXPath);
				if (airDragComponent != null && airDragComponent.SelectSingleNode($"{airDragXPath}//*[local-name()='CdxA_0']") != null)
				{
					model = airDragComponent.SelectSingleNode($"{airDragXPath}//*[local-name()='Model']").InnerText;
					cdxa = airDragComponent.SelectSingleNode($"{airDragXPath}//*[local-name()='CdxA_0']").InnerText.ToDouble().SI<SquareMeter>();
					certificationNumber = airDragComponent.SelectSingleNode($"{airDragXPath}//*[local-name()='CertificationNumber']").InnerText;
					certificationMethod = CertificationMethod.Measured;
				}
			}

			return new AirdragData()
			{
				ModelName = model,
				CertificationMethod = certificationMethod,
				CertificationNumber = certificationNumber,
				DeclaredAirdragArea = cdxa
			};
		}
	}

	internal class ManufacturerResults : IResultsInputData
	{
		public ManufacturerResults(XmlNode resultNode)
		{
			Status = resultNode.SelectSingleNode("./*[local-name() = 'Status']").InnerText;
			Results = new List<IResult>();
			foreach (XmlNode node in resultNode.SelectNodes("./*[local-name() = 'Result' and @status='success']")) {
				var entry = new Result {
					ResultStatus = node.Attributes.GetNamedItem("status").InnerText.ParseEnum<ResultStatus>(),
					Mission = node.SelectSingleNode("./*[local-name()='Mission']").InnerText.ParseEnum<MissionType>(),
					SimulationParameter = GetSimulationParameter(node.SelectSingleNode("./*[local-name() = 'SimulationParameters' or local-name() = 'SimulationParametersCompletedVehicle']")),
					EnergyConsumption = node.SelectSingleNode("./*[local-name()='Fuel' and FuelConsumption/@unit='MJ/km']")?
											.Cast<XmlNode>().Select(
												x => new KeyValuePair<FuelType, JoulePerMeter>(
													x.Attributes.GetNamedItem(XMLNames.Report_Results_Fuel_Type_Attr).InnerText.ParseEnum<FuelType>(),
													x.SelectSingleNode(
															$"./*[local-name()='{XMLNames.Report_Result_EnergyConsumption}' and @unit='MJ/km']")
													?.InnerText
													.ToDouble().SI(Unit.SI.Mega.Joule.Per.Kilo.Meter).Cast<JoulePerMeter>()))
											.ToDictionary(x => x.Key, x => x.Value),
					CO2 = node.SelectNodes(".//*[local-name()='CO2' and @unit]").Cast<XmlNode>().Select(
								x => new KeyValuePair<string, double>(x.Attributes.GetNamedItem("unit").InnerText, x.InnerText.ToDouble()))
							.ToDictionary(x => x.Key, x => x.Value)

				};
				Results.Add(entry);
			}
		}

		private ISimulationParameter GetSimulationParameter(XmlNode node)
		{
			return new SimulationParameter {
				TotalVehicleMass = (node.SelectSingleNode($"./*[local-name()='{XMLNames.Report_ResultEntry_TotalVehicleMass}']")?.InnerText.ToDouble() ?? 0).SI<Kilogram>(),
				Payload = (node.SelectSingleNode($"./*[local-name()='{XMLNames.Report_Result_Payload}']")?.InnerText.ToDouble() ?? 0).SI<Kilogram>(),
				PassengerCount = node.SelectSingleNode($"./*[local-name()='{XMLNames.Bus_PassengerCount}']")?.InnerText.ToDouble() ?? 0,
				//FuelMode = "" //node.SelectSingleNode($"./*[local-name()='{XMLNames.Report_Result_FuelMode}']").InnerText
			};
		}

		#region Implementation of IResultsInputData

		public string Status { get; }
		public IList<IResult> Results { get; }

		#endregion
	}

	public class VTPOBFCMDeclarationData
	{
		private readonly JObject _body;
		private readonly Dictionary<string, double> _rawLifetimeFuelConsumption;
		private readonly Meter _odometerReading;

		public VTPOBFCMDeclarationData(JObject body)
		{
			_body = body ?? throw new ArgumentNullException(nameof(body));

			var rawOBFCMJson = body["OBFCM"]?.ToString();

			_rawLifetimeFuelConsumption = rawOBFCMJson != null
				? JsonConvert.DeserializeObject<Dictionary<string, double>>(rawOBFCMJson) : null;
			
			_odometerReading = _body["MileageEndOfTest"]?.Value<double>().SI(Unit.SI.Kilo.Meter).Cast<Meter>() ?? null;
		}

		public VTPOBFCMDeclarationData(Dictionary<string, double> rawLifetimeFuelConsumption, Meter odometerReading)
		{
			_odometerReading = odometerReading;
			_rawLifetimeFuelConsumption = rawLifetimeFuelConsumption ?? throw new ArgumentNullException(nameof(rawLifetimeFuelConsumption));
		}

		public Meter OdometerReading => _odometerReading;

		public Dictionary<string, Kilogram> LifetimeFuelConsumptionMass => _rawLifetimeFuelConsumption != null 
			? _rawLifetimeFuelConsumption
				.Where(f => f.Key == "LifetimeFuelConsumptionMassStart" || f.Key == "LifetimeFuelConsumptionMassEnd")?
				.Select(f => new KeyValuePair<string, Kilogram>(f.Key, f.Value.SI<Kilogram>()))
				.ToDictionary(f => f.Key, f => f.Value)
			: null;

		public Dictionary<string, Liter> LifetimeFuelConsumptionVolume => _rawLifetimeFuelConsumption != null 
			? _rawLifetimeFuelConsumption
				.Where(f => f.Key == "LifetimeFuelConsumptionVolStart" || f.Key == "LifetimeFuelConsumptionVolEnd")?
				.Select(f => new KeyValuePair<string, Liter>(f.Key, f.Value.SI<Liter>()))
				.ToDictionary(f => f.Key, f => f.Value)
			: null;

		public Kilogram TotalLifetimeFuelConsumptionMass => !LifetimeFuelConsumptionMass.IsNullOrEmpty()
			? (LifetimeFuelConsumptionMass.Last().Value - LifetimeFuelConsumptionMass.First().Value) 
			: null;

		public Liter TotalLifetimeFuelConsumptionVolume => !LifetimeFuelConsumptionVolume.IsNullOrEmpty() 
			? (LifetimeFuelConsumptionVolume.Last().Value - LifetimeFuelConsumptionVolume.First().Value) 
			: null;
	}

	public class JSONInputDataV5 : JSONInputDataV4
	{
		protected IEngineStopStartEngineeringInputData engineStopStartData;
		protected IEcoRollEngineeringInputData ecoRollData;
		protected IPCCEngineeringInputData pccData;

		public JSONInputDataV5(JObject data, string filename, bool tolerateMissing = false) : base(
			data, filename, tolerateMissing) { }

		#region Overrides of JSONInputDataV2

		public override IEngineStopStartEngineeringInputData EngineStopStartData =>
			engineStopStartData ?? (engineStopStartData = new EngineStopStartInputData {
				MaxEngineOffTimespan = Body["EngineStopStartMaxOffTimespan"] == null
					? null
					: Body.GetEx<double>("EngineStopStartMaxOffTimespan").SI<Second>(),
				UtilityFactorStandstill = Body["EngineStopStartUtilityFactor"] == null
					? DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor
					: Body.GetEx<double>("EngineStopStartUtilityFactor"),
				UtilityFactorDriving = Body["EngineStopStartUtilityFactorDriving"] == null
					? (Body["EngineStopStartUtilityFactor"] == null
						? DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor
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
				AccelerationUpperLimit = Body["EcoRollMaxAcceleration"] == null? null : Body.GetEx<double>("EcoRollMaxAcceleration").SI<MeterPerSquareSecond>()
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

	public class JSONInputDataSingleBusV6 : JSONFile, ISingleBusInputDataProvider, IDeclarationJobInputData
	{
		private readonly IXMLInputDataReader _xmlInputReader;

		public JSONInputDataSingleBusV6(JObject data, string filename, bool tolerateMissing = false) : base(
			data, filename, tolerateMissing)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = kernel.Get<IXMLInputDataReader>();

			var primaryInputData = Path.Combine(BasePath,  Body.GetEx<string>(JsonKeys.PrimaryVehicle));
			var completedInputData = Path.Combine(BasePath,  Body.GetEx<string>("CompletedVehicle"));

			var primaryJob = CreateReader(primaryInputData);
			PrimaryVehicle = primaryJob.JobInputData.Vehicle;
			XMLHash = primaryJob.XMLHash;
			var completedJob = CreateReader(completedInputData);
			CompletedVehicle = completedJob.JobInputData.Vehicle;
			XMLHashCompleted = completedJob.XMLHash;

			JobName = CompletedVehicle.VIN;
		}

		private IDeclarationInputDataProvider CreateReader(string vehicleFileName)
		{
			if (Path.GetExtension(vehicleFileName) != ".xml") {
				throw new VectoException("unsupported vehicle file format {0}", vehicleFileName);
			}

			return _xmlInputReader.CreateDeclaration(vehicleFileName);

		}

		#region Overrides of JSONFile

		public override bool SavedInDeclarationMode => true;

		#endregion

		#region Implementation of ISingleBusInputDataProvider

		public IVehicleDeclarationInputData PrimaryVehicle { get; }
		public IVehicleDeclarationInputData CompletedVehicle { get; }

		#endregion

		#region Implementation of IDeclarationInputDataProvider

		public IDeclarationJobInputData JobInputData => this;
		public virtual IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData => null;
		public XElement XMLHash { get; }
		public XElement XMLHashCompleted { get; }

		#endregion

		#region Implementation of IDeclarationJobInputData

		public IVehicleDeclarationInputData Vehicle => PrimaryVehicle;
		public string JobName { get; }

		public VectoSimulationJobType JobType => VectoSimulationJobType.ConventionalVehicle;

		#endregion
	}

	// --------------------------

	public class JSONInputDataCompletedBusFactorMethodV7 : JSONFile, IMultistageVIFInputData //, IDeclarationInputDataProvider, IDeclarationJobInputData
	{
		private readonly IXMLInputDataReader _xmlInputReader;
		protected internal string PrimaryInputDataFile;
		protected internal string CompletedInputDataFile;
		protected internal bool RunSimulation;

		public JSONInputDataCompletedBusFactorMethodV7(JObject data, string filename, bool tolerateMissing = false) : base(
			data, filename, tolerateMissing)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = kernel.Get<IXMLInputDataReader>();

			PrimaryInputDataFile = Path.Combine(BasePath, Body.GetEx<string>("PrimaryVehicleResults"));
			CompletedInputDataFile = Path.Combine(BasePath, Body.GetEx<string>("CompletedVehicle"));
			RunSimulation = Body.ContainsKey(JsonKeys.BUS_RunSimulation) ? Body.GetEx<bool>(JsonKeys.BUS_RunSimulation) : true;


            //PrimaryVehicle = CreateReader(primaryInputData);

            Vehicle = _xmlInputReader.CreateDeclaration(CompletedInputDataFile).JobInputData.Vehicle;
            PrimaryVehicleData = (_xmlInputReader.Create(PrimaryInputDataFile) as IMultistepBusInputDataProvider);
            //JobName = Vehicle.VIN;
        }


		//private IDeclarationInputDataProvider CreateReader(string vehicleFileName)
		//{
		//	if (Path.GetExtension(vehicleFileName) != ".xml") {
		//		throw new VectoException("unsupported vehicle file format {0}", vehicleFileName);
		//	}

		//	return ;
		//}

		#region Overrides of JSONFile

		public override bool SavedInDeclarationMode => true;

		#endregion

        //#region Implementation of IDeclarationInputDataProvider

        //public IDeclarationJobInputData JobInputData => this;
        public IMultistepBusInputDataProvider PrimaryVehicleData { get; }
        //public XElement XMLHash { get; }

        //#endregion

        //#region Implementation of IDeclarationJobInputData

        public IVehicleDeclarationInputData Vehicle { get; }
        //public string JobName { get; }

        //public VectoSimulationJobType JobType => VectoSimulationJobType.ConventionalVehicle;

        //#endregion

		#region Implementation of IMultistageVIFInputData

		public IVehicleDeclarationInputData VehicleInputData => Vehicle;
		public IMultistepBusInputDataProvider MultistageJobInputData => PrimaryVehicleData;

		public bool SimulateResultingVIF => RunSimulation;

        public string MonitoringData => Vehicle.VehicleMonitoringData;

        #endregion
    }

	// --------------------------

	public class JSONInputDataV8_ParallelHybrid : JSONInputDataV5
	{
		public JSONInputDataV8_ParallelHybrid(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		public override VectoSimulationJobType JobType => VectoSimulationJobType.ParallelHybridVehicle;

		public override IHybridStrategyParameters HybridStrategyParameters =>
			Body["HybridStrategyParams"] == null
				? null : JSONInputDataFactory.ReadHybridStrategyParameters(
					Path.Combine(BasePath, Body.GetEx<string>("HybridStrategyParams")), false);
	}

	// --------------------------

	public class JSONInputDataV9_BEV : AbstractJSONInputData
	{

		public JSONInputDataV9_BEV(JObject data, string filename, bool tolerateMissing = false) : base(data, filename,
			tolerateMissing)
		{
			VehicleData = ReadVehicle();
			if (Body[JsonKeys.Vehicle_GearboxFile] != null) {
				Gearbox = ReadGearbox();
				AxleGear = Gearbox as IAxleGearInputData;
				TorqueConverter = Gearbox as ITorqueConverterEngineeringInputData;
				//GearshiftInputData = Gearbox as IGearshiftEngineeringInputData;
			}
		}

		public override VectoSimulationJobType JobType => VectoSimulationJobType.BatteryElectricVehicle;

		public override IGearshiftEngineeringInputData GearshiftInputData =>
			Body["TCU"] == null
				? null
				: JSONInputDataFactory.ReadShiftParameters(Path.Combine(BasePath, Body.GetEx<string>("TCU")), false);
	}

	// --------------------------

	public class JSONInputDataV11_SerialHybrid : AbstractJSONInputData
	{

		public JSONInputDataV11_SerialHybrid(JObject data, string filename, bool tolerateMissing = false) : base(data, filename,
			tolerateMissing)
		{
			VehicleData = ReadVehicle();
			Engine = ReadEngine();
			if (Body[JsonKeys.Vehicle_GearboxFile] != null) {
				Gearbox = ReadGearbox();
				AxleGear = Gearbox as IAxleGearInputData;
				TorqueConverter = Gearbox as ITorqueConverterEngineeringInputData;
				//GearshiftInputData = Gearbox as IGearshiftEngineeringInputData;
			}
		}

		public override IHybridStrategyParameters HybridStrategyParameters =>
			Body["HybridStrategyParams"] == null
				? null : JSONInputDataFactory.ReadHybridStrategyParameters(
					Path.Combine(BasePath, Body.GetEx<string>("HybridStrategyParams")), false);

		public override VectoSimulationJobType JobType => VectoSimulationJobType.SerialHybridVehicle;

		public override IGearshiftEngineeringInputData GearshiftInputData =>
			Body["TCU"] == null
				? null
				: JSONInputDataFactory.ReadShiftParameters(Path.Combine(BasePath, Body.GetEx<string>("TCU")), false);


	}

	// --------------------------

	public class JSONInputDataV12_IEPC : AbstractJSONInputData
	{
		public JSONInputDataV12_IEPC(JObject data, string filename, bool tolerateMissing = false) : base(data, filename,
			tolerateMissing)
		{
			VehicleData = ReadVehicle();
			if (Body[JsonKeys.Vehicle_EngineFile] != null) {
				Engine = ReadEngine();
			}

			if (Body[JsonKeys.Vehicle_GearboxFile] != null && !string.IsNullOrWhiteSpace(Body[JsonKeys.Vehicle_GearboxFile].Value<string>())) {
				//AxleGear = ReadGearbox() as IAxleGearInputData;
				Gearbox = ReadGearbox();  // gearbox is not used, but required by GUI
				AxleGear = Gearbox as IAxleGearInputData;
			}
		}

		public override IGearshiftEngineeringInputData GearshiftInputData =>
			Body["TCU"] == null
				? null
				: JSONInputDataFactory.ReadShiftParameters(Path.Combine(BasePath, Body.GetEx<string>("TCU")), false);

		public override IHybridStrategyParameters HybridStrategyParameters =>
			Body["HybridStrategyParams"] == null
				? null : JSONInputDataFactory.ReadHybridStrategyParameters(
					Path.Combine(BasePath, Body.GetEx<string>("HybridStrategyParams")), false);

		public override VectoSimulationJobType JobType => VehicleData.VehicleType;
	}

	// --------------------------

	public class JSONInputDataV13_IHPC : JSONInputDataV8_ParallelHybrid
	{
		public JSONInputDataV13_IHPC(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		public override VectoSimulationJobType JobType => VectoSimulationJobType.IHPC;
	}

	public class JSONInputDataV14_FCHybrid : JSONInputDataV9_BEV
	{
		public JSONInputDataV14_FCHybrid(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing)
		{
			//VehicleData = ReadVehicle();
			//Gearbox = ReadGearbox();


		}

		public override VectoSimulationJobType JobType => VectoSimulationJobType.FCHV;
    }

	public class JSONInputDataV15_FCHV_IEPC : JSONInputDataV12_IEPC
	{
		public JSONInputDataV15_FCHV_IEPC(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing)
		{}

		public override VectoSimulationJobType JobType => VectoSimulationJobType.FCHV_IEPC;
	}

	public class JSONInputDataV16_MultiplePowertrains : AbstractJSONInputData
	{
		public JSONInputDataV16_MultiplePowertrains(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing)
		{
			VehicleData = ReadVehicle();

			if (Body[JsonKeys.Vehicle_EngineFile] != null)
			{
				Engine = ReadEngine();
			}

			if (JobType == VectoSimulationJobType.Multiple_SHEV)
			{
				throw new VectoException("SHEV with multiple powertrains are not yet supported.");
			}
		}

		public override VectoSimulationJobType JobType => VehicleData.VehicleType;
		
		public override IHybridStrategyParameters HybridStrategyParameters =>
			(Body[JsonKeys.Vehicle_HybridStrategyParams] == null)
			? null 
			: JSONInputDataFactory.ReadHybridStrategyParameters(
					Path.Combine(BasePath, Body.GetEx<string>(JsonKeys.Vehicle_HybridStrategyParams)), 
					false);
	}


	// --------------------------

	public class JSONInputDataV10_PrimaryAndStageInputBus : JSONFile, IInputDataProvider, IMultistagePrimaryAndStageInputDataProvider
	{
		private readonly IXMLInputDataReader _xmlInputReader;
		private readonly string _primaryVehicleInputDataPath;
		private readonly string _stageInputDataPath;
		private IVehicleDeclarationInputData _stageInputData;
		private IDeclarationInputDataProvider _primaryVehicle;

		public string PrimaryVehicleInputDataPath => _primaryVehicleInputDataPath;
		public string StageInputDataPath => _stageInputDataPath;
		public IDeclarationInputDataProvider PrimaryVehicle =>
			_primaryVehicle ?? (_primaryVehicle =
				_xmlInputReader.CreateDeclaration(_primaryVehicleInputDataPath));

        public IVehicleDeclarationInputData StageInputData =>
			_stageInputDataPath != null
			?
            _stageInputData ?? (_stageInputData =
                _xmlInputReader.CreateDeclaration(_stageInputDataPath).JobInputData.Vehicle)
			:
			null;

		public bool SimulateResultingVIF => _simulateResultingVif;


		private bool? _completed;
		private bool _simulateResultingVif;

		public bool? Completed
		{
			get => _completed;
			set => _completed = value;
		}


		public JSONInputDataV10_PrimaryAndStageInputBus(JObject data, string filename, bool tolerateMissing = false) :
			base(data, filename, tolerateMissing)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = kernel.Get<IXMLInputDataReader>();


			_primaryVehicleInputDataPath = Body.GetEx<string>(JsonKeys.PrimaryVehicle);
			_primaryVehicleInputDataPath = PathHelper.GetAbsolutePath(filename, _primaryVehicleInputDataPath);
			_simulateResultingVif = Body.GetEx<bool>(JsonKeys.BUS_RunSimulation);
			_stageInputDataPath = Body.GetEx<string>(JsonKeys.InterimStep);
			_stageInputDataPath = PathHelper.GetAbsolutePath(filename, _stageInputDataPath);
			_completed = Body.GetValueOrDefault<bool>(JsonKeys.Completed);

		}

		private void checkFileExtension(string path)
		{
			if (Path.GetExtension(path) != ".xml") {
				throw new VectoException("unsupported vehicle file format {0}", path);
			}
		}
	}
}

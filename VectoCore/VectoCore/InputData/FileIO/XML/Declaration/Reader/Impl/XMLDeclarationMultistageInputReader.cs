using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLDeclarationMultistageInputReaderV01 : AbstractComponentReader, IXMLDeclarationMultistageVehicleInputDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistepType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IDeclarationMultistageJobInputData _jobData;

		protected IXMLMultistageInputDataProvider InputData;

		protected XmlNode JobNode;

		public XMLDeclarationMultistageInputReaderV01(IXMLMultistageInputDataProvider inputData, XmlNode baseNode)
			: base(inputData, baseNode)
		{
			JobNode = baseNode;
			InputData = inputData;
		}

		public IDeclarationMultistageJobInputData JobData => _jobData ?? (_jobData = CreateComponent(XMLNames.VectoOutputMultistep, JobCreator));

		protected virtual IDeclarationMultistageJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreateMultiStageJobData(version, BaseNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreateMultistageJobReader(version, job, JobNode);
			return job;

		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLMultistageJobReaderV01 : AbstractComponentReader, IXMLMultistageJobReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistepType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IXMLDeclarationMultistageJobInputData InputData;
		protected IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		protected IList<IManufacturingStageInputData> _manufacturingStages;
		protected ConsolidateManufacturingStages _consolidateManufacturingStages;
		private IList<string> _invalidEntries = new List<string>();

		private XmlNodeList _manufacturingNodeStages;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }



		public XMLMultistageJobReaderV01(IXMLDeclarationMultistageJobInputData inputData, XmlNode baseNode)
			: base(inputData, baseNode)
		{
			InputData = inputData;
			SetManufacturingStageNodes();
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle => _primaryVehicle ?? (_primaryVehicle = CreateComponent(XMLNames.Bus_PrimaryVehicle, PrimaryVehicleCreator));

		protected IPrimaryVehicleInformationInputDataProvider PrimaryVehicleCreator(string version, XmlNode node,
			string arg3)
		{
			var primaryVehicle = Factory.CreatePrimaryMultistageVehicleData(version, node, arg3);
			primaryVehicle.Reader = Factory.CreatePrimaryVehicleBusInputReader(version, primaryVehicle, node.FirstChild);
			return primaryVehicle;
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get
			{
				if (_manufacturingNodeStages is null || _manufacturingNodeStages.Count == 0)
					return null;

				return _manufacturingStages ?? (_manufacturingStages = ManufacturingStagesCreator());
			}
		}

		private IList<IManufacturingStageInputData> ManufacturingStagesCreator()
		{
			var stages = new List<IManufacturingStageInputData>();

			foreach (XmlNode manufacturingNodeStage in _manufacturingNodeStages)
			{
				var version = XMLHelper.GetXsdType(manufacturingNodeStage?.SchemaInfo.SchemaType);
				stages.Add(ManufacturingStageCreator(version, manufacturingNodeStage));
			}
			return stages;
		}

		protected IManufacturingStageInputData ManufacturingStageCreator(string version, XmlNode node)
		{
			var stage = Factory.CreateMultistageData(version, node, null);
			stage.Reader = Factory.CreateMultistageDataReader(version, stage, node);
			return stage;
		}

		public virtual IManufacturingStageInputData ConsolidateManufacturingStage
		{
			get
			{
                //if (ManufacturingStages.IsNullOrEmpty())
                //    return null;

                return _consolidateManufacturingStages ??
						(_consolidateManufacturingStages = GetConsolidateManufacturingStage());
			}
		}

		public VectoSimulationJobType JobType => InputData.JobType;

		public bool InputComplete
		{
			get
			{
				if (ManufacturingStages is null || ManufacturingStages.Count == 0) {
					_invalidEntries.Add("no manufacturing stages");
					return false;
				}

				if (_consolidateManufacturingStages == null)
					_consolidateManufacturingStages = GetConsolidateManufacturingStage();

				return _consolidateManufacturingStages.IsInputDataComplete(JobType);
			}
		}

		public IList<string> InvalidEntries
		{
			get
			{
				var consolidatedInvalidEntries = _consolidateManufacturingStages?.GetInvalidEntries(JobType);
				if (consolidatedInvalidEntries != null) {
					return _invalidEntries.Concat(consolidatedInvalidEntries).ToList();
				} else {
					return _invalidEntries;
				}
				
			}
		}

		private ConsolidateManufacturingStages GetConsolidateManufacturingStage()
		{
			return new ConsolidateManufacturingStages(PrimaryVehicle, ManufacturingStages?.Reverse());
		}


		private void SetManufacturingStageNodes()
		{
			_manufacturingNodeStages = BaseNode.SelectNodes(XMLHelper.QueryLocalName(XMLNames.ManufacturingStep));
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLMultistageEntryReaderV01 : AbstractComponentReader, IXMLMultistageReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ManufacturingStepType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected readonly XmlNode JobNode;
		protected readonly IXMLMultistageEntryInputDataProvider _multistageData;
		protected IApplicationInformation _applicationInformation;
		protected IVehicleDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLMultistageEntryReaderV01(IXMLMultistageEntryInputDataProvider multistageData, XmlNode node) : base(
			multistageData, node)
		{
			JobNode = node;
			_multistageData = multistageData;
		}

		public IVehicleDeclarationInputData Vehicle => _vehicle ?? (_vehicle = CreateComponent(XMLNames.Tag_Vehicle, VehicleCreator));

		private IVehicleDeclarationInputData VehicleCreator(string version, XmlNode node, string arg3)
		{
			var vehicle = Factory.CreateVehicleData(version, null, node, arg3, false);

			if (vehicle.ComponentNode != null)
				vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);

			if (vehicle.ADASNode != null)
				vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);

			return vehicle;
		}

		public IApplicationInformation ApplicationInformation =>
			_applicationInformation ??
			(_applicationInformation = CreateComponent(XMLNames.Tag_ApplicationInformation, ApplicationCreator));

		protected IApplicationInformation ApplicationCreator(string version, XmlNode node, string agr3)
		{
			return Factory.CreateApplicationInformationReader(version, node);
		}

		public DigestData GetDigestData(XmlNode xmlNode)
		{
			return xmlNode == null ? null : new DigestData(xmlNode);
		}
	}

	// ---------------------------------------------------------------------------------------


	public class XMLMultistagePrimaryVehicleReaderV01 : AbstractComponentReader, IXMLDeclarationPrimaryVehicleBusInputDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "PrimaryVehicleDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected XmlNode JobNode;
		protected IDeclarationJobInputData _jobData;
		protected IXMLPrimaryVehicleBusInputData _primaryInputData;
		protected IApplicationInformation _applicationInformation;
		protected IResultsInputData _resultsInputData;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }


		public XMLMultistagePrimaryVehicleReaderV01(IXMLPrimaryVehicleBusInputData inputData, XmlNode baseNode) : base(inputData, baseNode)
		{
			JobNode = baseNode;
			_primaryInputData = inputData;
		}

		public virtual IDeclarationJobInputData JobData => _jobData ?? (_jobData = CreateComponent(XMLNames.Tag_Vehicle, JobCreator));


		protected IDeclarationJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreatePrimaryVehicleJobData(version, BaseNode, _primaryInputData,
				(_primaryInputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreatePrimaryVehicleJobReader(version, job, JobNode);
			return job;
		}

		public IResultsInputData ResultsInputData =>
			_resultsInputData ??
			(_resultsInputData = CreateComponent(XMLNames.Report_Results, ResultsInputDataCreator));

		protected IResultsInputData ResultsInputDataCreator(string version, XmlNode node, string arg3)
		{
			return Factory.CreateResultsInputDataReader(version, node);
		}

		public DigestData GetDigestData(XmlNode xmlNode)
		{
			return xmlNode == null ? null : new DigestData(xmlNode);
		}

		protected IApplicationInformation ApplicationCreator(string version, XmlNode node, string agr3)
		{
			return Factory.CreateApplicationInformationReader(version, node);
		}

		public IApplicationInformation ApplicationInformation =>
			_applicationInformation ??
			(_applicationInformation = CreateComponent(XMLNames.Tag_ApplicationInformation, ApplicationCreator));
	}

	// ---------------------------------------------------------------------------------------


	#region  Generate Consolidated Multistage InputData
	public abstract class ConsolidatedDataBase 
	{
		protected readonly IEnumerable<IManufacturingStageInputData> _manufacturingStages;
		private string _invalidEntry;
		protected IList<string> _invalidEntries = new List<string>();
		protected bool _fullChecked;
		protected bool _checked;
		protected bool _isComplete = true;

		protected string InvalidEntry
		{
			get => _invalidEntry;
			private set
			{
				_invalidEntry = value;

				if (!_invalidEntries.Contains(_invalidEntry)) {
					_invalidEntries.Add(_invalidEntry);
				}
				_isComplete = false;
			} 
		}


		public ConsolidatedDataBase(IEnumerable<IManufacturingStageInputData> manufacturingStages)
		{
			_manufacturingStages = manufacturingStages;
		}

		protected T GetPropertyValue<T>(object obj, string name)
		{
			var propertyValue = GetPropertyValue(obj, name);
			if (propertyValue == null)
				return default;

			return (T)propertyValue;
		}

		protected object GetPropertyValue(object obj, string name)
		{
			foreach (var part in name.Split('.'))
			{
				if (obj == null) { return null; }

				var type = obj.GetType();
				var info = type.GetProperty(part);
				if (info == null) { return null; }

				obj = info.GetValue(obj, null);
			}
			return obj;
		}

		protected abstract bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck);


		public bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			var result = (_checked && _isComplete) || IsInputDataCompleteTemplate(jobType, fullCheck: false);
			_isComplete = result;
			return result;
		}

		public bool IsInputDataCompleteFullCheck(VectoSimulationJobType jobType)
		{
			if (_isComplete && _checked) {
				return true;
			} else {
				var result = IsInputDataCompleteTemplate(jobType, fullCheck: true);
				_isComplete = result;
				_fullChecked = true;
				_checked = true;
				return result;
			}
		}

		
		

		public abstract string GetInvalidEntry();

		protected abstract IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType);

		public IList<string> GetInvalidEntries(VectoSimulationJobType jobType)
		{
			if (_checked && _isComplete) {
				return _invalidEntries; //<- empty
			} else {
				IsInputDataCompleteFullCheck(jobType);
				return GetInvalidEntriesTemplate(jobType);
			}
			
		}

		protected bool MethodComplete(bool result, string methodName)
		{
			if (result)
				return true;

			InvalidEntry = methodName;
			return false;
		}

		protected bool InputComplete<T>(T value, string variableName)
		{
			if (value != null)
				return true;

			InvalidEntry  = variableName; 
			return false;
		}

		protected bool InputComplete<T>(T value, string variableName, bool mandatory)
		{
			if (mandatory) {
				return InputComplete(value, variableName);
			} else {
				return true;
			}
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidateManufacturingStages : ConsolidatedDataBase, IManufacturingStageInputData
	{
		private ConsolidatedVehicleData _consolidatedVehicleData;
		protected IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		
		public ConsolidateManufacturingStages(IPrimaryVehicleInformationInputDataProvider primaryVehicle, 
			IEnumerable<IManufacturingStageInputData> manufacturingStages) : base(manufacturingStages)
		{
			_primaryVehicle = primaryVehicle;
		}

		public DigestData HashPreviousStep => _manufacturingStages?.First().HashPreviousStep;

		public int StepCount => _manufacturingStages?.First().StepCount ?? 0;

		public virtual IVehicleDeclarationInputData Vehicle => GetConsolidatedVehicleData();

		public IApplicationInformation ApplicationInformation => _manufacturingStages?.First().ApplicationInformation;

		public DigestData Signature => _manufacturingStages?.First().Signature;


		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
			return fullCheck
				? GetConsolidatedVehicleData().IsInputDataCompleteFullCheck(jobType)
				: GetConsolidatedVehicleData().IsInputDataComplete(jobType);
		}

		public override string GetInvalidEntry()
		{
			return _consolidatedVehicleData.GetInvalidEntry();
		}

		#region Overrides of ConsolidatedDataBase
		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			return _invalidEntries.Concat(_consolidatedVehicleData.GetInvalidEntries(jobType)).ToList();
		}
		#endregion

		private ConsolidatedVehicleData GetConsolidatedVehicleData()
		{
			return _consolidatedVehicleData ??
					(_consolidatedVehicleData = new ConsolidatedVehicleData(_primaryVehicle, _manufacturingStages));
		}

	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedVehicleData : ConsolidatedDataBase, IVehicleDeclarationInputData
	{
		private ConsolidatedADASData _consolidatedADAS;
		private ConsolidatedComponentData _consolidatedComponents;
		private readonly IPrimaryVehicleInformationInputDataProvider _primaryVehicle;

		public ConsolidatedVehicleData(IPrimaryVehicleInformationInputDataProvider primaryVehicle,
			IEnumerable<IManufacturingStageInputData> manufacturingStages) : base(manufacturingStages)
		{
			_primaryVehicle = primaryVehicle;
		}
		public VectoSimulationJobType VehicleType { get => _primaryVehicle.Vehicle.VehicleType; }

		#region ManufacturingStage mandatory properties

		public string Manufacturer => _manufacturingStages?.First().Vehicle.Manufacturer;

		public string ManufacturerAddress => _manufacturingStages?.First().Vehicle.ManufacturerAddress;

		public DateTime Date => _manufacturingStages?.First().Vehicle.Date ?? DateTime.MinValue;

		public string VIN => _manufacturingStages?.First().Vehicle.VIN;

		public VehicleDeclarationType VehicleDeclarationType => _manufacturingStages?.First().Vehicle.VehicleDeclarationType ?? default(VehicleDeclarationType);


		public IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits => throw new NotImplementedException();
		public TableData BoostingLimitations => throw new NotImplementedException();

		#endregion

		#region ManufacturingStage optional properties

		public string Model => GetVehiclePropertyValue<string>(nameof(Model));

		public LegislativeClass? LegislativeClass => GetVehiclePropertyValue<LegislativeClass?>(nameof(LegislativeClass));

		public Kilogram CurbMassChassis => GetVehiclePropertyValue<Kilogram>(nameof(CurbMassChassis));

		public Kilogram GrossVehicleMassRating => GetVehiclePropertyValue<Kilogram>(nameof(GrossVehicleMassRating)) ?? _primaryVehicle.Vehicle.GrossVehicleMassRating;

		public bool? AirdragModifiedMultistep => GetVehiclePropertyValue<bool?>(nameof(AirdragModifiedMultistep));

		public TankSystem? TankSystem => GetVehiclePropertyValue<TankSystem?>(nameof(TankSystem));

		public RegistrationClass? RegisteredClass => GetVehiclePropertyValue<RegistrationClass?>(nameof(RegisteredClass));


		public int? NumberPassengerSeatsUpperDeck => GetVehiclePropertyValue<int?>(nameof(NumberPassengerSeatsUpperDeck));

		public int? NumberPassengerSeatsLowerDeck => GetVehiclePropertyValue<int?>(nameof(NumberPassengerSeatsLowerDeck));

		public int? NumberPassengersStandingLowerDeck => GetVehiclePropertyValue<int?>(nameof(NumberPassengersStandingLowerDeck));

		public int? NumberPassengersStandingUpperDeck => GetVehiclePropertyValue<int?>(nameof(NumberPassengersStandingUpperDeck));

		public VehicleCode? VehicleCode => GetVehiclePropertyValue<VehicleCode?>(nameof(VehicleCode));

		public string VehicleTypeApprovalNumber => GetVehiclePropertyValue<string>(nameof(VehicleTypeApprovalNumber));
		public bool? LowEntry => GetVehiclePropertyValue<bool?>(nameof(LowEntry));

		public Meter Height => GetVehiclePropertyValue<Meter>(nameof(Height));

		public Meter Length => GetVehiclePropertyValue<Meter>(nameof(Length));

		public Meter Width => GetVehiclePropertyValue<Meter>(nameof(Width));

		public Meter EntranceHeight => GetVehiclePropertyValue<Meter>(nameof(EntranceHeight));

		public ConsumerTechnology? DoorDriveTechnology => GetVehiclePropertyValue<ConsumerTechnology?>(nameof(DoorDriveTechnology));

		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS => GetADAS();

		public IVehicleInMotionChargingDeclaration InMotionCharging => GetVehiclePropertyValue<IVehicleInMotionChargingDeclaration>(nameof(InMotionCharging));

		private IAdvancedDriverAssistantSystemDeclarationInputData GetADAS()
		{
			return _consolidatedADAS
					?? (_consolidatedADAS = new ConsolidatedADASData(
						manufacturingStages: _manufacturingStages, 
						primaryVehicleData: _primaryVehicle.Vehicle,
						usePrimaryData: GetVehiclePropertyValue<IAdvancedDriverAssistantSystemDeclarationInputData>(nameof(ADAS)) == null));
		}


		public IVehicleComponentsDeclaration Components => GetComponents();

		private IVehicleComponentsDeclaration GetComponents()
		{
			if (GetVehiclePropertyValue<IVehicleComponentsDeclaration>(nameof(Components)) == null)
				return null;

			return _consolidatedComponents
					?? (_consolidatedComponents = new ConsolidatedComponentData(_manufacturingStages));
		}

		#endregion

		#region Non set IVehicleDeclarationInputData interface properties

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get; }
		public string CertificationNumber { get; }
		public DigestData DigestValue { get; }
		public string Identifier { get; }
		public string SimulationToolLicenseNumber { get; }
        public string VehicleMonitoringData { get; }

        public bool ExemptedVehicle
		{
			get
			{
				return _primaryVehicle.Vehicle.ExemptedVehicle;
				//if (_primaryVehicle.Vehicle.ExemptedVehicle) {
				//	return true;
				//}
				////return _manufacturingStages.Any(x => x.Vehicle.ExemptedVehicle);
			}
		}
		public VehicleCategory VehicleCategory
		{
			get
			{
				if (ExemptedVehicle) {
					return IsInputDataCompleteExempted(VectoSimulationJobType.ConventionalVehicle, false) 
						? VehicleCategory.HeavyBusCompletedVehicle
						: VehicleCategory.HeavyBusInterimVehicle;
				}
				return VehicleDeclarationType == VehicleDeclarationType.final
					? VehicleCategory.HeavyBusCompletedVehicle
					: VehicleCategory.HeavyBusInterimVehicle;
			}
		}

		#region Properties taken from primary vehicle
		public AxleConfiguration AxleConfiguration => _primaryVehicle.Vehicle.AxleConfiguration;
		public IList<ITorqueLimitInputData> TorqueLimits { get; }
		public bool HybridElectricHDV => _primaryVehicle.Vehicle.HybridElectricHDV;

		public bool ZeroEmissionVehicle => _primaryVehicle.Vehicle.ZeroEmissionVehicle;

		public bool VocationalVehicle => _primaryVehicle.Vehicle.VocationalVehicle;
		public bool? SleeperCab => _primaryVehicle.Vehicle.SleeperCab;

		public bool DualFuelVehicle => _primaryVehicle.Vehicle.DualFuelVehicle;

		public bool OVC => _primaryVehicle.Vehicle.OVC;

		public PerSecond EngineIdleSpeed => _primaryVehicle.Vehicle.EngineIdleSpeed;

		public Watt MaxNetPower1 => _primaryVehicle.Vehicle.MaxNetPower1;
		public string ExemptedTechnology => _primaryVehicle.Vehicle.ExemptedTechnology;

		public ArchitectureID ArchitectureID => _primaryVehicle.Vehicle.ArchitectureID;

		public ArchitectureID ArchitectureIDPwt2 => _primaryVehicle.Vehicle.ArchitectureIDPwt2;

        public Watt MaxChargingPower => _primaryVehicle.Vehicle.MaxChargingPower;

		public Kilogram H2StorageUsableCapacity => _primaryVehicle.Vehicle.H2StorageUsableCapacity;

		public HydrogenStorageTechnology? HydrogenStorageTechnology => _primaryVehicle.Vehicle.HydrogenStorageTechnology;

        public bool BatteryOnlyMode => _primaryVehicle.Vehicle.BatteryOnlyMode;

		public DynamicChargingTechnology DynamicChargingTechnology => _primaryVehicle.Vehicle.DynamicChargingTechnology;

        #endregion








        public CubicMeter CargoVolume { get; }
		public bool Articulated { get; }

		public XmlNode XMLSource { get; }




		#endregion

		private T GetVehiclePropertyValue<T>(string propertyName)
		{
			if (_manufacturingStages != null) {
				foreach (var manufacturingStage in _manufacturingStages)
				{
					var value = GetPropertyValue<T>(manufacturingStage.Vehicle, propertyName);
					if (value != null)
						return value;
				}
			}
			return default;
		}

		private bool PrimaryEngineWithNGTankSystem()
		{
			var enginePrimaryEngine = _primaryVehicle?.Vehicle?.Components?.EngineInputData;
			if (enginePrimaryEngine == null)
				return false;

			foreach (var engineMode in enginePrimaryEngine.EngineModes)
			{
				foreach (var fuel in engineMode.Fuels)
				{
					if (fuel.FuelType == FuelType.NGPI || fuel.FuelType == FuelType.NGCI)
						return true;
				}
			}

			return false;
		}
		

		private bool IsTankSystemValid()
		{
			if (PrimaryEngineWithNGTankSystem())
				return TankSystem != null;
			return true;
		}

		private bool IsAirdragEntriesValid()
		{
			var checkAirdragModified = false;
			var validAirdragEntries = true;
			if (_manufacturingStages == null) {
				return default(bool);
			}

			var stages = _manufacturingStages.Reverse().ToList();
			foreach (var manufacturingStage in stages) {
				if (manufacturingStage.Vehicle?.Components?.AirdragInputData != null && !checkAirdragModified ) {
					checkAirdragModified = true;
					continue;
				}

				if (checkAirdragModified && manufacturingStage.Vehicle?.AirdragModifiedMultistep == null) {
					validAirdragEntries = false;
					break;
				}
			}

			return validAirdragEntries;
		}

        private bool IsInputDataCompleteExempted(VectoSimulationJobType jobType, bool fullCheck)
		{
			if (fullCheck)
			{
				//use Binary AND to execute all Statements and gather information about missing parameters.
				return InputComplete(Model, nameof(Model))
						& InputComplete(LegislativeClass, nameof(LegislativeClass))
						& InputComplete(CurbMassChassis, nameof(CurbMassChassis))
						& InputComplete(GrossVehicleMassRating, nameof(GrossVehicleMassRating))
						& InputComplete(RegisteredClass, nameof(RegisteredClass))
						& InputComplete(NumberPassengerSeatsLowerDeck, nameof(NumberPassengerSeatsLowerDeck))
						& InputComplete(NumberPassengersStandingLowerDeck, nameof(NumberPassengersStandingLowerDeck))
						& InputComplete(NumberPassengerSeatsUpperDeck, nameof(NumberPassengerSeatsUpperDeck))
						& InputComplete(NumberPassengersStandingUpperDeck, nameof(NumberPassengersStandingUpperDeck))
						& InputComplete(VehicleCode, nameof(VehicleCode))
						& InputComplete(LowEntry, nameof(LowEntry))
						& InputComplete(Height, nameof(Height));
			}


			return InputComplete(Model, nameof(Model))
					&& InputComplete(LegislativeClass, nameof(LegislativeClass))
					&& InputComplete(CurbMassChassis, nameof(CurbMassChassis))
					&& InputComplete(GrossVehicleMassRating, nameof(GrossVehicleMassRating))
					&& InputComplete(RegisteredClass, nameof(RegisteredClass))
					&& InputComplete(NumberPassengerSeatsLowerDeck, nameof(NumberPassengerSeatsLowerDeck))
					&& InputComplete(NumberPassengerSeatsUpperDeck, nameof(NumberPassengerSeatsUpperDeck))
					&& InputComplete(VehicleCode, nameof(VehicleCode))
					&& InputComplete(LowEntry, nameof(LowEntry)) && InputComplete(Height, nameof(Height));
		}


		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
			if (ExemptedVehicle) {
				return IsInputDataCompleteExempted(jobType, fullCheck);
			}
			GetADAS();
			GetComponents();
			if (fullCheck) {
				//use Binary AND to execute all Statements and gather information about missing parameters.
				return InputComplete(Model, nameof(Model))
                    & InputComplete(LegislativeClass, nameof(LegislativeClass))
					& InputComplete(CurbMassChassis, nameof(CurbMassChassis))
					& InputComplete(GrossVehicleMassRating, nameof(GrossVehicleMassRating))
					& MethodComplete(IsAirdragEntriesValid(), nameof(IsAirdragEntriesValid))
					& MethodComplete(IsTankSystemValid(), nameof(IsTankSystemValid))
					& InputComplete(RegisteredClass, nameof(RegisteredClass))
					& InputComplete(NumberPassengerSeatsLowerDeck, nameof(NumberPassengerSeatsLowerDeck))
					& InputComplete(NumberPassengerSeatsUpperDeck, nameof(NumberPassengerSeatsUpperDeck))
					& InputComplete(NumberPassengersStandingLowerDeck, nameof(NumberPassengersStandingLowerDeck))
					& InputComplete(NumberPassengersStandingUpperDeck, nameof(NumberPassengersStandingUpperDeck))
					& InputComplete(VehicleCode, nameof(VehicleCode))
					& InputComplete(LowEntry, nameof(LowEntry)) 
					& InputComplete(Height, nameof(Height))
					& InputComplete(Length, nameof(Length)) 
					& InputComplete(Width, nameof(Width))
					& InputComplete(EntranceHeight, nameof(EntranceHeight))
					& InputComplete(DoorDriveTechnology, nameof(DoorDriveTechnology))
					//& InputComplete(VehicleTypeApprovalNumber, nameof(VehicleTypeApprovalNumber))
					& (InputComplete(_consolidatedADAS, nameof(_consolidatedADAS)) && _consolidatedADAS.IsInputDataCompleteFullCheck(jobType))
					& (InputComplete(_consolidatedComponents, nameof(_consolidatedComponents)) && _consolidatedComponents.IsInputDataCompleteFullCheck(jobType));
			}
			
		
			return  InputComplete(Model, nameof(Model))
					&& InputComplete(LegislativeClass, nameof(LegislativeClass)) 
					&& InputComplete(CurbMassChassis, nameof(CurbMassChassis)) 
					&& InputComplete(GrossVehicleMassRating, nameof(GrossVehicleMassRating))
					&& MethodComplete(IsAirdragEntriesValid(), nameof(IsAirdragEntriesValid)) 
					&& MethodComplete(IsTankSystemValid(), nameof(IsTankSystemValid))
					&& InputComplete(RegisteredClass, nameof(RegisteredClass))
					&& InputComplete(NumberPassengerSeatsLowerDeck, nameof(NumberPassengerSeatsLowerDeck))
					&& InputComplete(NumberPassengerSeatsUpperDeck, nameof(NumberPassengerSeatsUpperDeck))
					&& InputComplete(NumberPassengersStandingLowerDeck, nameof(NumberPassengersStandingLowerDeck))
					&& InputComplete(NumberPassengersStandingUpperDeck, nameof(NumberPassengersStandingUpperDeck))
					&& InputComplete(VehicleCode, nameof(VehicleCode))
					&& InputComplete(LowEntry, nameof(LowEntry)) && InputComplete(Height, nameof(Height)) 
					&& InputComplete(Length, nameof(Length)) && InputComplete(Width, nameof(Width)) 
					&& InputComplete(EntranceHeight, nameof(EntranceHeight))  
					&& InputComplete(DoorDriveTechnology, nameof(DoorDriveTechnology)) 
					//&& InputComplete(VehicleTypeApprovalNumber, nameof(VehicleTypeApprovalNumber))
					&& InputComplete(_consolidatedADAS, nameof(_consolidatedADAS)) && _consolidatedADAS.IsInputDataComplete(jobType)
					&& InputComplete(_consolidatedComponents, nameof(_consolidatedComponents)) && _consolidatedComponents.IsInputDataComplete(jobType);
		}


		public override string GetInvalidEntry()
		{
			if (InvalidEntry != null)
				return InvalidEntry;
			
			if (_consolidatedADAS?.GetInvalidEntry() != null)
				return _consolidatedADAS.GetInvalidEntry();

			if (_consolidatedComponents?.GetInvalidEntry() != null)
				return _consolidatedComponents.GetInvalidEntry(); 

			return null;
		}

		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			IEnumerable<string> concatenatedEntries = new List<string>();
			if (_consolidatedComponents != null) {
				concatenatedEntries = concatenatedEntries.Concat(_consolidatedComponents.GetInvalidEntries(jobType));
			}

			if (_consolidatedADAS != null) {
				concatenatedEntries = concatenatedEntries.Concat(_consolidatedADAS.GetInvalidEntries(jobType));
			}


			return _invalidEntries.Concat(concatenatedEntries).ToList();
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedADASData : ConsolidatedDataBase, IAdvancedDriverAssistantSystemDeclarationInputData
	{
		private readonly bool _usePrimaryData;
		private readonly IAdvancedDriverAssistantSystemDeclarationInputData _primaryAdas;
		private readonly IVehicleDeclarationInputData _primaryVehicleData;

		public ConsolidatedADASData(IEnumerable<IManufacturingStageInputData> manufacturingStages,
			IVehicleDeclarationInputData primaryVehicleData, bool usePrimaryData)
			: base(manufacturingStages)
		{
			_usePrimaryData = usePrimaryData;
			_primaryVehicleData = primaryVehicleData;
			_primaryAdas = primaryVehicleData.ADAS;
		}

		public bool EngineStopStart => GetADASPropertyValue<bool>(nameof(EngineStopStart));

		public EcoRollType EcoRoll => GetADASPropertyValue<EcoRollType>(nameof(EcoRoll));

		public PredictiveCruiseControlType PredictiveCruiseControl => GetADASPropertyValue<PredictiveCruiseControlType>(nameof(PredictiveCruiseControl));

		public bool? ATEcoRollReleaseLockupClutch => GetADASPropertyValue<bool?>(nameof(ATEcoRollReleaseLockupClutch));

		public XmlNode XMLSource { get; }


		private T GetADASPropertyValue<T>(string propertyName)
		{
			if (_usePrimaryData) {
				return GetPropertyValue<T>(_primaryAdas, propertyName);
			}

			foreach (var manufacturingStage in _manufacturingStages) {
				var adas = manufacturingStage.Vehicle.ADAS;
				if (adas == null)
					continue;
				var value = GetPropertyValue<T>(adas, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}

		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
            if (fullCheck)
            {
                //use Binary AND to execute all Statements and gather information about missing parameters.
                return InputComplete(EngineStopStart, nameof(EngineStopStart))
                        & InputComplete(EcoRoll, nameof(EcoRoll))
                        & InputComplete(PredictiveCruiseControl, nameof(PredictiveCruiseControl))
						& InputComplete(IsATPEcoRollComplete(), "APTEcoRollReleaseLockupClutch");
            }
            return InputComplete(EngineStopStart, nameof(EngineStopStart))
                    && InputComplete(EcoRoll, nameof(EcoRoll))
                    && InputComplete(PredictiveCruiseControl, nameof(PredictiveCruiseControl))
					&& InputComplete(IsATPEcoRollComplete(), "APTEcoRollReleaseLockupClutch");
		}

		private bool IsATPEcoRollComplete()
		{
			if (PrimaryGearboxIsAT()) {
				return ATEcoRollReleaseLockupClutch != null;
			} else {
				return false;
			}
		}

		private bool PrimaryGearboxIsAT()
		{
			switch (_primaryVehicleData.Components.GearboxInputData?.Type) {
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					return true;
				default: 
					return false;
			}
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}

		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			return _invalidEntries;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedComponentData : ConsolidatedDataBase, IVehicleComponentsDeclaration
	{
		private ConsolidatedAirdragData _consolidateAirdragData;
		private ConsolidatedBusAuxiliariesData _consolidateBusAuxiliariesData;

		public ConsolidatedComponentData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }


		public IAirdragDeclarationInputData AirdragInputData => GetAirdragInputData();

		private IAirdragDeclarationInputData GetAirdragInputData()
		{
			if (GetComponentPropertyValue<IAirdragDeclarationInputData>(nameof(AirdragInputData)) == null)
				return null;

			return _consolidateAirdragData ??
					(_consolidateAirdragData = new ConsolidatedAirdragData(_manufacturingStages));
		}

		public IGearboxDeclarationInputData GearboxInputData => null;

		public ITorqueConverterDeclarationInputData TorqueConverterInputData => null;

		public IAxleGearInputData AxleGearInputData => null;

		public IAngledriveInputData AngledriveInputData => null;

		public IEngineDeclarationInputData EngineInputData => null;

		public IAuxiliariesDeclarationInputData AuxiliaryInputData => null;

		public IRetarderInputData RetarderInputData => null;

		public IPTOTransmissionInputData PTOTransmissionInputData => null;

		public IAxlesDeclarationInputData AxleWheels => null;

		public IBusAuxiliariesDeclarationData BusAuxiliaries => GetBusAuxiliaries();

		private IBusAuxiliariesDeclarationData GetBusAuxiliaries()
		{
			if (GetComponentPropertyValue<IBusAuxiliariesDeclarationData>(nameof(BusAuxiliaries)) == null)
				return null;

			return _consolidateBusAuxiliariesData ??
					(_consolidateBusAuxiliariesData = new ConsolidatedBusAuxiliariesData(_manufacturingStages));
		}

		public IElectricStorageSystemDeclarationInputData ElectricStorage => null;

		public IElectricMachinesDeclarationInputData ElectricMachines => null;
		
		public IIEPCDeclarationInputData IEPC => null;

		public IFuelCellSystemDeclarationInputData FuelCellSystem => null;

		public IList<IAxlePowertrainDeclarationInputData> AxlePowertrainInputData => null;

		public ElectricMachineEntry<IElectricMotorDeclarationInputData> Generator => null;

        private T GetComponentPropertyValue<T>(string propertyName)
		{

			foreach (var manufacturingStage in _manufacturingStages) {
				var component = manufacturingStage.Vehicle?.Components;
				if (component == null)
					continue;
				var value = GetPropertyValue<T>(component, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}


		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
			GetAirdragInputData();
			GetBusAuxiliaries();
			if (fullCheck) {
				//use Binary AND to execute all Statements and gather information about missing parameters.
				return InputComplete(_consolidateBusAuxiliariesData, nameof(_consolidateBusAuxiliariesData))
						& (_consolidateBusAuxiliariesData != null && _consolidateBusAuxiliariesData.IsInputDataCompleteFullCheck(jobType));
			}
			return InputComplete(_consolidateBusAuxiliariesData, nameof(_consolidateBusAuxiliariesData)) 
					&& _consolidateBusAuxiliariesData.IsInputDataComplete(jobType);
		}

		public override string GetInvalidEntry()
		{
			if (InvalidEntry != null)
				return InvalidEntry;
			
			if (_consolidateAirdragData?.GetInvalidEntry() != null)
				return _consolidateAirdragData.GetInvalidEntry();

			if (_consolidateBusAuxiliariesData?.GetInvalidEntry() != null)
				return _consolidateBusAuxiliariesData.GetInvalidEntry(); 

			return InvalidEntry;
		}

		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			if (_consolidateBusAuxiliariesData == null) {
				return new List<string>();
			}
			return _invalidEntries.Concat(_consolidateBusAuxiliariesData.GetInvalidEntries(jobType)).ToList();
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedAirdragData : ConsolidatedDataBase, IAirdragDeclarationInputData
	{
		public IAirdragDeclarationInputData AirdragEntry { private set; get; }

		public ConsolidatedAirdragData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages)
		{
			SetLastValidAirdragEntry();
		}

		public string Manufacturer => AirdragEntry?.Manufacturer;

		public string Model => AirdragEntry?.Model;

		public DateTime Date => AirdragEntry.Date;

		public string AppVersion => AirdragEntry?.AppVersion;

		public CertificationMethod CertificationMethod => AirdragEntry.CertificationMethod;

		public string CertificationNumber => AirdragEntry?.CertificationNumber;

		public DigestData DigestValue => AirdragEntry?.DigestValue;

		public SquareMeter AirDragArea => AirdragEntry?.AirDragArea;

		public SquareMeter TransferredAirDragArea => AirdragEntry?.TransferredAirDragArea;

		public SquareMeter AirDragArea_0 => AirdragEntry.AirDragArea_0;

		public SquareMeter DeltaCdxA_CFD => AirdragEntry?.DeltaCdxA_CFD;

		public SquareMeter DeltaCdxA_declared => AirdragEntry?.DeltaCdxA_declared;

		public SquareMeter DeltaTransferredCdxA => AirdragEntry?.DeltaTransferredCdxA;

		public string LicenseNumberCFDMethod => AirdragEntry?.LicenseNumberCFDMethod;

        public XmlNode XMLSource => AirdragEntry.XMLSource;

		public DataSource DataSource => AirdragEntry?.DataSource;
		public bool SavedInDeclarationMode { get; }

		private void SetLastValidAirdragEntry()
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var airdragData = manufacturingStage.Vehicle?.Components?.AirdragInputData;
				if (airdragData == null)
					continue;

				var value = airdragData.AirDragArea;
				if (value == null)
					continue;

				AirdragEntry = airdragData;
				return;
			}
		}

		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
			return InputComplete(AirdragEntry, nameof(AirdragEntry));
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}

		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			return _invalidEntries;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedBusAuxiliariesData : ConsolidatedDataBase, IBusAuxiliariesDeclarationData
	{
		private ConsolidateElectricConsumerData _consolidateElectricConsumerData;
		private ConsolidatedHVACBusAuxiliariesData _consolidatedHVACBusAuxiliariesData;
		private XmlNode _xmlNode;
		private IList<string> _consolidateSteeringPumpData;


		public ConsolidatedBusAuxiliariesData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }

		public DataSource DataSource
		{
			get => _manufacturingStages.Select(s => s.Vehicle.Components.BusAuxiliaries.DataSource).FirstOrDefault();
		}

		public string FanTechnology => null;

		public IList<string> SteeringPumpTechnology => null;

		public IElectricSupplyDeclarationData ElectricSupply => null;

		public IElectricConsumersDeclarationData ElectricConsumers => GetElectricConsumers();

		private IElectricConsumersDeclarationData GetElectricConsumers()
		{
			if (GetBusAuxPropertyValue<IElectricConsumersDeclarationData>(nameof(ElectricConsumers)) == null)
				return null;

			return _consolidateElectricConsumerData ??
					(_consolidateElectricConsumerData = new ConsolidateElectricConsumerData(_manufacturingStages));
		}

		public IPneumaticSupplyDeclarationData PneumaticSupply => null;

		public IPneumaticConsumersDeclarationData PneumaticConsumers => null;

		public IHVACBusAuxiliariesDeclarationData HVACAux => GetHVACAux();

		private IHVACBusAuxiliariesDeclarationData GetHVACAux()
		{
			if (GetBusAuxPropertyValue<IHVACBusAuxiliariesDeclarationData>(nameof(HVACAux)) == null)
				return null;

			return _consolidatedHVACBusAuxiliariesData ??
					(_consolidatedHVACBusAuxiliariesData = new ConsolidatedHVACBusAuxiliariesData(_manufacturingStages));
		}

		private T GetBusAuxPropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var busAux = manufacturingStage.Vehicle?.Components?.BusAuxiliaries;
				if (busAux == null)
					continue;
				var value = GetPropertyValue<T>(busAux, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}
		

		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
			GetElectricConsumers();
			GetHVACAux();
			if (fullCheck) {
				//use Binary AND to execute all Statements and gather information about missing parameters.
				return InputComplete(_consolidateElectricConsumerData, nameof(_consolidateElectricConsumerData))
					& ((_consolidateElectricConsumerData != null)
                        && _consolidateElectricConsumerData.IsInputDataCompleteFullCheck(jobType))
					& InputComplete(_consolidatedHVACBusAuxiliariesData, nameof(_consolidatedHVACBusAuxiliariesData))
					& ((_consolidatedHVACBusAuxiliariesData != null)
						&& _consolidatedHVACBusAuxiliariesData.IsInputDataCompleteFullCheck(jobType));

			}
			return InputComplete(_consolidateElectricConsumerData, nameof(_consolidateElectricConsumerData)) 
					&& _consolidateElectricConsumerData.IsInputDataComplete(jobType)
					&& InputComplete(_consolidatedHVACBusAuxiliariesData, nameof(_consolidatedHVACBusAuxiliariesData))
					&& (_consolidatedHVACBusAuxiliariesData != null)
					&& _consolidatedHVACBusAuxiliariesData.IsInputDataComplete(jobType);
		}

		public override string GetInvalidEntry()
		{
			if (InvalidEntry != null)
				return InvalidEntry;

			if (_consolidateElectricConsumerData?.GetInvalidEntry() != null)
				return _consolidateElectricConsumerData.GetInvalidEntry(); 
			if (_consolidatedHVACBusAuxiliariesData?.GetInvalidEntry() != null)
				return _consolidatedHVACBusAuxiliariesData.GetInvalidEntry(); 
			
			return null;
		}

		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			return _invalidEntries.Concat(_consolidateElectricConsumerData?.GetInvalidEntries(jobType) 
					?? new List<string>() { XMLNames.BusAux_ElectricSystem })
				.Concat(_consolidatedHVACBusAuxiliariesData?.GetInvalidEntries(jobType) ?? new List<string>()).ToList();
		}

	}
	

	// ---------------------------------------------------------------------------------------

		public class ConsolidateElectricConsumerData : ConsolidatedDataBase, IElectricConsumersDeclarationData
	{
		public ConsolidateElectricConsumerData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }

		public bool? InteriorLightsLED => GetElectricConsumerPropertyValue<bool?>(nameof(InteriorLightsLED));

		public bool? DayrunninglightsLED => GetElectricConsumerPropertyValue<bool?>(nameof(DayrunninglightsLED));

		public bool? PositionlightsLED => GetElectricConsumerPropertyValue<bool?>(nameof(PositionlightsLED));

		public bool? HeadlightsLED => GetElectricConsumerPropertyValue<bool?>(nameof(HeadlightsLED));

		public bool? BrakelightsLED => GetElectricConsumerPropertyValue<bool?>(nameof(BrakelightsLED));


		private T GetElectricConsumerPropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var electricConsumer = manufacturingStage.Vehicle?.Components?.BusAuxiliaries?.ElectricConsumers;
				if (electricConsumer == null)
					continue;
				var value = GetPropertyValue<T>(electricConsumer, propertyName);
				if (value != null)
					return value;
			}
			return default;
		}

		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
			if (fullCheck) {
				return InputComplete(InteriorLightsLED, nameof(InteriorLightsLED))
						& InputComplete(DayrunninglightsLED, nameof(DayrunninglightsLED))
						& InputComplete(PositionlightsLED, nameof(PositionlightsLED))
						& InputComplete(HeadlightsLED, nameof(HeadlightsLED))
						& InputComplete(BrakelightsLED, nameof(BrakelightsLED));
			}
			return InputComplete(InteriorLightsLED, nameof(InteriorLightsLED))
				&& InputComplete(DayrunninglightsLED, nameof(DayrunninglightsLED))
				&& InputComplete(PositionlightsLED, nameof(PositionlightsLED))
				&& InputComplete(HeadlightsLED, nameof(HeadlightsLED))
				&& InputComplete(BrakelightsLED, nameof(BrakelightsLED));
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}

		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			return _invalidEntries;
		}
	}

	// ---------------------------------------------------------------------------------------
	public class ConsolidatedHVACBusAuxiliariesData : ConsolidatedDataBase, IHVACBusAuxiliariesDeclarationData
	{

		public ConsolidatedHVACBusAuxiliariesData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }

		public BusHVACSystemConfiguration? SystemConfiguration => GetHVACBusAuxPropertyValue<BusHVACSystemConfiguration?>(nameof(SystemConfiguration));

		public HeatPumpType? HeatPumpTypeCoolingDriverCompartment => GetHVACBusAuxPropertyValue<HeatPumpType?>(nameof(HeatPumpTypeCoolingDriverCompartment));

		public HeatPumpType? HeatPumpTypeHeatingDriverCompartment => GetHVACBusAuxPropertyValue<HeatPumpType?>(nameof(HeatPumpTypeHeatingDriverCompartment));

		public HeatPumpType? HeatPumpTypeCoolingPassengerCompartment => GetHVACBusAuxPropertyValue<HeatPumpType?>(nameof(HeatPumpTypeCoolingPassengerCompartment));
		public HeatPumpType? HeatPumpTypeHeatingPassengerCompartment => GetHVACBusAuxPropertyValue<HeatPumpType?>(nameof(HeatPumpTypeHeatingPassengerCompartment));


		public Watt AuxHeaterPower => GetHVACBusAuxPropertyValue<Watt>(nameof(AuxHeaterPower));

		public bool? DoubleGlazing => GetHVACBusAuxPropertyValue<bool?>(nameof(DoubleGlazing));

		public bool? AdjustableAuxiliaryHeater => GetHVACBusAuxPropertyValue<bool?>(nameof(AdjustableAuxiliaryHeater));

		public bool? SeparateAirDistributionDucts => GetHVACBusAuxPropertyValue<bool?>(nameof(SeparateAirDistributionDucts));

		public bool? WaterElectricHeater => GetHVACBusAuxPropertyValue<bool?>(nameof(WaterElectricHeater));

		public bool? AirElectricHeater => GetHVACBusAuxPropertyValue<bool?>(nameof(AirElectricHeater));

		public bool? OtherHeatingTechnology => GetHVACBusAuxPropertyValue<bool?>(nameof(OtherHeatingTechnology));

		public bool? AdjustableCoolantThermostat { get; }

		public bool EngineWasteGasHeatExchanger { get; }



		private T GetHVACBusAuxPropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var havacAux = manufacturingStage.Vehicle?.Components?.BusAuxiliaries?.HVACAux;
				if (havacAux == null)
					continue;
				var value = GetPropertyValue<T>(havacAux, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}
		
		private bool IsCorrectSystemConfiguration()
		{
			return SystemConfiguration != null && SystemConfiguration != BusHVACSystemConfiguration.Unknown;
		}
		
		private bool RequiredParametersForJobType(VectoSimulationJobType jobType)
		{
			switch (jobType) {
				case VectoSimulationJobType.ConventionalVehicle:
					return true;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.IHPC:
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
					return WaterElectricHeater != null && AirElectricHeater != null && OtherHeatingTechnology != null;
				default:
					return false;
			}
		}
		
		protected override bool IsInputDataCompleteTemplate(VectoSimulationJobType jobType, bool fullCheck)
		{
			if (fullCheck) {
				return MethodComplete(IsCorrectSystemConfiguration(), nameof(IsCorrectSystemConfiguration))
						& InputComplete(HeatPumpTypeCoolingDriverCompartment, nameof(HeatPumpTypeCoolingDriverCompartment))
						& InputComplete(HeatPumpTypeHeatingDriverCompartment, nameof(HeatPumpTypeHeatingDriverCompartment))
						& InputComplete(HeatPumpTypeCoolingPassengerCompartment, nameof(HeatPumpTypeCoolingPassengerCompartment))
						& InputComplete(HeatPumpTypeHeatingPassengerCompartment, nameof(HeatPumpTypeHeatingPassengerCompartment))
						& InputComplete(AuxHeaterPower, nameof(AuxHeaterPower))
						& InputComplete(DoubleGlazing, nameof(DoubleGlazing))
						& InputComplete(AdjustableAuxiliaryHeater, nameof(AdjustableAuxiliaryHeater))
						& InputComplete(SeparateAirDistributionDucts, nameof(SeparateAirDistributionDucts))
						& MethodComplete(RequiredParametersForJobType(jobType), "Electric Heater");
			}
			return MethodComplete(IsCorrectSystemConfiguration(), nameof(IsCorrectSystemConfiguration))
					&& InputComplete(HeatPumpTypeCoolingDriverCompartment, nameof(HeatPumpTypeCoolingDriverCompartment))
					&& InputComplete(HeatPumpTypeHeatingDriverCompartment, nameof(HeatPumpTypeHeatingDriverCompartment))
					&& InputComplete(HeatPumpTypeCoolingPassengerCompartment, nameof(HeatPumpTypeCoolingPassengerCompartment))
					&& InputComplete(HeatPumpTypeHeatingPassengerCompartment, nameof(HeatPumpTypeHeatingPassengerCompartment))
					&& InputComplete(AuxHeaterPower, nameof(AuxHeaterPower))
					&& InputComplete(DoubleGlazing, nameof(DoubleGlazing))
					&& InputComplete(AdjustableAuxiliaryHeater, nameof(AdjustableAuxiliaryHeater))
					&& InputComplete(SeparateAirDistributionDucts, nameof(SeparateAirDistributionDucts))
					&& MethodComplete(RequiredParametersForJobType(jobType), "Electric Heater");
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}

		protected override IList<string> GetInvalidEntriesTemplate(VectoSimulationJobType jobType)
		{
			return _invalidEntries;
		}
	}

	#endregion
}
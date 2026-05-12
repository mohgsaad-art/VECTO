using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationInputDataProviderMultistageV01 : AbstractXMLResource, IXMLMultistageInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistepType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IDeclarationMultistageJobInputData JobData;
		protected readonly XmlDocument Document;

		public XMLDeclarationInputDataProviderMultistageV01(XmlDocument xmlDoc, string fileName)
			: base(xmlDoc.DocumentElement, fileName)
		{
			Document = xmlDoc;

			var h = VectoHash.Load(xmlDoc);
			XMLHash = h.ComputeXmlHash();
		}

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		public IDeclarationMultistageJobInputData JobInputData => JobData ?? (JobData = Reader.JobData);


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData => throw new NotImplementedException();

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get; }
		public XElement XMLHash { get; }

		public IXMLDeclarationMultistageVehicleInputDataReader Reader { protected get; set; }

	}

    public class XMLDeclarationInputDataProviderMultistageV11 : XMLDeclarationInputDataProviderMultistageV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationInputDataProviderMultistageV11(XmlDocument xmlDoc, string fileName) : base(xmlDoc, fileName) { }
    }

    // ---------------------------------------------------------------------------------------


    public class XMLDeclarationMultistageJobInputDataV01 : AbstractXMLResource, IXMLDeclarationMultistageJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistepType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		private IList<IManufacturingStageInputData> _manufacturingStages;
		private IManufacturingStageInputData _consolidatedManufacturingStage;


		public XMLDeclarationMultistageJobInputDataV01(XmlNode node, IXMLMultistageInputDataProvider inputProvider,
			string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
			SourceType = DataSourceType.XMLFile;
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle => _primaryVehicle ?? (_primaryVehicle = Reader.PrimaryVehicle);

		public IList<IManufacturingStageInputData> ManufacturingStages => _manufacturingStages ?? (_manufacturingStages = Reader.ManufacturingStages);

		public IManufacturingStageInputData ConsolidateManufacturingStage => _consolidatedManufacturingStage ?? (_consolidatedManufacturingStage = Reader.ConsolidateManufacturingStage);

		public VectoSimulationJobType JobType => ConsolidateManufacturingStage.Vehicle.VehicleType;

		public bool InputComplete => Reader.InputComplete;

		public IList<string> InvalidEntries => Reader.InvalidEntries;

		public IXMLMultistageJobReader Reader { protected get; set; }

		public IXMLMultistageInputDataProvider InputData { get; }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }
	}

	public class XMLDeclarationMultistageJobInputDataV11 : XMLDeclarationMultistageJobInputDataV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMultistageJobInputDataV11(XmlNode node, IXMLMultistageInputDataProvider inputProvider, string fileName) : 
			base(node, inputProvider, fileName) 
		{ }
    }

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistagePrimaryVehicleInputDataV01 : AbstractXMLResource,
		IXMLPrimaryVehicleBusInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "PrimaryVehicleDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private readonly XmlNode _signatureNode;
		private IVehicleDeclarationInputData _vehicle;
		private IApplicationInformation _applicationInformation;
		private IResultsInputData _resultsInputData;
		private DigestData _primaryVehicleInputDataHash;
		private DigestData _vehicleSignatureHash;
		private DigestData _manufacturerRecordHash;
		
		public XMLDeclarationMultistagePrimaryVehicleInputDataV01(XmlNode xmlNode, string fileName)
			: base(xmlNode, fileName)
		{
			//var h = VectoHash.Load(xmlNode);
			//XMLHash = h.ComputeXmlHash();

			_signatureNode = xmlNode.LastChild;
		}

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType => DataSourceType.XMLFile;


		public IVehicleDeclarationInputData Vehicle => _vehicle ?? (_vehicle = Reader.JobData.Vehicle);

		public DigestData PrimaryVehicleInputDataHash =>
			_primaryVehicleInputDataHash ?? 
			(_primaryVehicleInputDataHash =  Reader.GetDigestData(GetNode("InputDataSignature")));

		public DigestData VehicleSignatureHash =>
			_vehicleSignatureHash ?? 
			(_vehicleSignatureHash = Reader.GetDigestData(_signatureNode));

		public DigestData ManufacturerRecordHash =>
			_manufacturerRecordHash ??
			(_manufacturerRecordHash =  Reader.GetDigestData(GetNode("ManufacturerRecordSignature")));

		public IResultsInputData ResultsInputData => _resultsInputData ?? (_resultsInputData = Reader.ResultsInputData);

		public IResult GetResult(VehicleClass vehicleClass, MissionType mission, string fuelMode, Kilogram payload,
			OvcHevMode ovcHevMode)
		{
			var matches = ResultsInputData.Results.Where(
				x => x.VehicleGroup == vehicleClass &&
					(x.SimulationParameter.Payload - payload).IsEqual(0, 1) && x.Mission == mission 
			).ToArray();
			if (!matches.Any()) {
				throw new VectoException($"No primary result found for {vehicleClass}, {mission}, {payload}", ErrorCodes.XML_MISSING_NODE);
			}
			if (matches.Length == 1) {
				return matches.First();
			}

			return matches.First(x => x.OvcMode == ovcHevMode);
		}

		public XmlNode ResultsNode => GetNode(XMLNames.Report_Results);

		public IApplicationInformation ApplicationInformation => _applicationInformation ?? (_applicationInformation = Reader.ApplicationInformation);

		public XmlNode ApplicationInformationNode => GetNode(XMLNames.Tag_ApplicationInformation);

		public XElement XMLHash { get; }

		public IXMLDeclarationPrimaryVehicleBusInputDataReader Reader { protected get; set; }

	}

	public class XMLDeclarationMultistagePrimaryVehicleInputDataV11 : XMLDeclarationMultistagePrimaryVehicleInputDataV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMultistagePrimaryVehicleInputDataV11(XmlNode xmlNode, string fileName)
			: base(xmlNode, fileName)
		{ }
    }

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistageTypeInputDataV01 : AbstractXMLResource, IXMLMultistageEntryInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ManufacturingStepType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private readonly XmlNode _signatureXmlNode;
		private IVehicleDeclarationInputData _vehicle;
		private IApplicationInformation _applicationInformation;
		private DigestData _hashPreviousStep;
		private DigestData _signature;


		public XMLDeclarationMultistageTypeInputDataV01(XmlNode xmlNode, string fileName) 
			: base(xmlNode, fileName)
		{
			_signatureXmlNode = xmlNode.LastChild;
		}
		
		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		public DigestData HashPreviousStep =>
			_hashPreviousStep ??
			(_hashPreviousStep = Reader.GetDigestData(GetNode(XMLNames.ManufacturingStep_HashPreviousStep)));

		public int StepCount => Convert.ToInt32(GetAttribute(BaseNode, XMLNames.ManufacturingStep_StepCount));

		public IVehicleDeclarationInputData Vehicle => _vehicle ?? (_vehicle = Reader.Vehicle);

		public IApplicationInformation ApplicationInformation => _applicationInformation ?? (_applicationInformation = Reader.ApplicationInformation);

		public DigestData Signature => _signature ?? (_signature = Reader.GetDigestData(_signatureXmlNode));

		public IXMLMultistageReader Reader { protected get; set; }
	}

	public class XMLDeclarationMultistageTypeInputDataV11 : XMLDeclarationMultistageTypeInputDataV01
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11;

        public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMultistageTypeInputDataV11(XmlNode xmlNode, string fileName) : base(xmlNode, fileName) 
		{ }
    }

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationVIFInputData : IMultistageVIFInputData
	{
		private readonly IMultistepBusInputDataProvider _multistageJobInputData;
		private readonly IVehicleDeclarationInputData _vehicleInput;

		public XMLDeclarationVIFInputData(IMultistepBusInputDataProvider multistageJobInputData,
			IVehicleDeclarationInputData vehicleInput) : this(multistageJobInputData, vehicleInput, false) { }

		public XMLDeclarationVIFInputData(IMultistepBusInputDataProvider multistageJobInputData,
		IVehicleDeclarationInputData vehicleInput, bool runSimulation, string monitoringData = null)
		{
			_multistageJobInputData = multistageJobInputData;
			_vehicleInput = vehicleInput;
			_simulateResultingVif = runSimulation;
			MonitoringData = monitoringData;
		}

		public IVehicleDeclarationInputData VehicleInputData => _vehicleInput;

		public IMultistepBusInputDataProvider MultistageJobInputData => _multistageJobInputData;

		public string MonitoringData {  get; private set; }

		private readonly bool _simulateResultingVif;
		bool IMultistageVIFInputData.SimulateResultingVIF => _simulateResultingVif;

		public DataSource DataSource { get; }
	}

}
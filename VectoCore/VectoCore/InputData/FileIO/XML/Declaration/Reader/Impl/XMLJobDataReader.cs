using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLJobDataReaderV10 : AbstractComponentReader, IXMLJobDataReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected IXMLDeclarationJobInputData JobData;
		protected XmlNode JobNode;
		protected IVehicleDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLJobDataReaderV10(IXMLDeclarationJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode)
		{
			JobNode = jobNode;
			JobData = jobData;
		}

		#region Implementation of IXMLJobDataReader

		public virtual IVehicleDeclarationInputData CreateVehicle
		{
			get { return _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator)); }
		}

		#endregion

		protected virtual IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);
			vehicle.PTOReader = GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);

			return vehicle;
		}

	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV20 : XMLJobDataReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataReaderV20(IXMLDeclarationJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode)
		{ }

		protected override IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = vehicle.ADASNode == null ? null : GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader); //null;
			vehicle.PTOReader = vehicle.PTONode == null ? null : GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);

			return vehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV21 : XMLJobDataReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLJobDataReaderV21(IXMLDeclarationJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode)
		{ }

		protected override IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);

			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);
			vehicle.PTOReader = GetReader(vehicle, vehicle.PTONode, Factory.CreatePTOReader);

			return vehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV27 : AbstractComponentReader, IXMLJobTrailerDataReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IXMLDeclarationTrailerJobInputData JobData;
		protected XmlNode JobNode;
		protected ITrailerDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLJobDataReaderV27(IXMLDeclarationTrailerJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode)
		{
			JobNode = jobNode;
			JobData = jobData;
		}

		public ITrailerDeclarationInputData CreateVehicle => _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, TrailerVehicleCreator));

		protected virtual ITrailerDeclarationInputData TrailerVehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var trailer = Factory.CreateTrailerVehicleData(version, JobData, vehicleNode, sourceFile);

			trailer.AxlesReader = GetReader(trailer, trailer.AxleWheelsNode, Factory.CreateTrailerAxlesReader);
			//trailer.AxlesReader = new XMLComponentReaderV27(trailer, vehicleNode.ChildNodes[12]?.ChildNodes[0]?.FirstChild);

			return trailer;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataPrimaryVehicleReaderV01 : AbstractComponentReader, IXMLJobDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01;

		public const string XSD_TYPE = "PrimaryVehicleHeavyBusDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IVehicleDeclarationInputData _vehicle;

		private readonly IXMLPrimaryVehicleBusJobInputData _primaryBusJobData;

		public XMLJobDataPrimaryVehicleReaderV01(IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode)
			: base(busJobData, jobNode)
		{
			_primaryBusJobData = busJobData;
		}

		public IVehicleDeclarationInputData CreateVehicle
		{
			get { return _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator)); }
		}

		protected IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreatePrimaryVehicleData(version, _primaryBusJobData, vehicleNode, sourceFile);
			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);

			return vehicle;
		}
	}
}
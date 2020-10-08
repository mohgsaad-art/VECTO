using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationJobInputDataProviderV10 : AbstractXMLResource, IXMLDeclarationJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IVehicleDeclarationInputData _vehicle;

		public XMLDeclarationJobInputDataProviderV10(XmlNode node, IXMLDeclarationInputData inputProvider, string fileName) :
			base(node, fileName)
		{
			InputData = inputProvider;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		#endregion

		#region Implementation of IDeclarationJobInputData

		public virtual bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public virtual IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		public virtual string JobName
		{
			get { return Vehicle.Identifier; }
		}

		public virtual string ShiftStrategy { get { return null; } }

		#endregion

		#region Implementation of IXMLDeclarationJobInputData

		public virtual IXMLJobDataReader Reader { protected get; set; }
		public virtual IXMLDeclarationInputData InputData { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationJobInputDataProviderV20 : XMLDeclarationJobInputDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationJobInputDataProviderV20(XmlNode node, IXMLDeclarationInputData inputProvider, string fileName) :
			base(node, inputProvider, fileName) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationJobInputDataProviderV27 : AbstractXMLResource, IXMLDeclarationTrailerJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected ITrailerDeclarationInputData _vehicle;

		public XMLDeclarationJobInputDataProviderV27(XmlNode node, IXMLDeclarationTrailerInputData inputProvider, string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		#endregion
		#region Implementation of IDeclarationJobInputData

		public virtual bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public virtual ITrailerDeclarationInputData Trailer
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		public virtual string JobName
		{
			get { return Trailer.Identifier; }
		}

		public virtual string ShiftStrategy { get { return null; } }

		#endregion

		public virtual IXMLJobTrailerDataReader Reader { protected get; set; }
		public virtual IXMLDeclarationTrailerInputData InputData { get; }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryVehicleBusJobInputDataProviderV01 : AbstractXMLResource, IXMLPrimaryVehicleBusJobInputData
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01;

		public const string XSD_TYPE = "PrimaryVehicleHeavyBusDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected IVehicleDeclarationInputData _vehicle;


		public XMLDeclarationPrimaryVehicleBusJobInputDataProviderV01(XmlNode node, IXMLPrimaryVehicleBusInputData inputProvider,
			string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public string ShiftStrategy
		{
			get { return null; }
		}

		public bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public string JobName
		{
			get { return Vehicle.Identifier; }
		}
		
		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		public IXMLJobDataReader Reader { protected get; set; }
		public IXMLPrimaryVehicleBusInputData InputData { get; }
	}


}

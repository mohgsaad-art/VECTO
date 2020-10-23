using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationTyreDataProviderV10 : AbstractCommonComponentType, IXMLTyreDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "TyreDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected string _dimension;
		protected double? _rrc;
		protected Newton _fzIso;

		public XMLDeclarationTyreDataProviderV10(XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of ITyreDeclarationInputData

		public virtual string Dimension
		{
			get { return _dimension ?? (_dimension = GetString(XMLNames.AxleWheels_Axles_Axle_Dimension)); }
		}

		public virtual double RollResistanceCoefficient
		{
			get { return _rrc ?? (_rrc = GetDouble(XMLNames.AxleWheels_Axles_Axle_RRCDeclared)).Value; }
		}

		public virtual Newton TyreTestLoad
		{
			get { return _fzIso ?? (_fzIso = GetDouble(XMLNames.AxleWheels_Axles_Axle_FzISO).SI<Newton>()); }
		}

		public virtual string FuelEfficiencyClass { get { return DeclarationData.Wheels.TyreClass.Lookup(RollResistanceCoefficient); } }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV20 : XMLDeclarationTyreDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "TyreComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationTyreDataProviderV20(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile) { }



		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV22 : XMLDeclarationTyreDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V22;

		//public new const string XSD_TYPE = "TyreComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationTyreDataProviderV22(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV23 : XMLDeclarationTyreDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;

		//public new const string XSD_TYPE = "TyreComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationTyreDataProviderV23(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile) { }


		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public override string FuelEfficiencyClass { get { return GetString("FuelEfficiencyClass"); } }
	}

}
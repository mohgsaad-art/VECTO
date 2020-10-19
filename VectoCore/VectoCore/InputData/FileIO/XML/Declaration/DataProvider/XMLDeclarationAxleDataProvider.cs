using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAxleDataProviderV10 : AbstractCommonComponentType, IXMLAxleDeclarationInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "AxleDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected ITyreDeclarationInputData _tyre;
		protected bool? _twinTyre;
		protected AxleType? _axleType;

		public XMLDeclarationAxleDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IAxleDeclarationInputData

		public virtual bool TwinTyres
		{
			get {
				return _twinTyre ?? (_twinTyre = XmlConvert.ToBoolean(GetString(XMLNames.AxleWheels_Axles_Axle_TwinTyres))).Value;
			}
		}

		public virtual AxleType AxleType
		{
			get {
				return _axleType ?? (_axleType = GetString(XMLNames.AxleWheels_Axles_Axle_AxleType).ParseEnum<AxleType>()).Value;
			}
		}

		public virtual ITyreDeclarationInputData Tyre
		{
			get { return _tyre ?? (_tyre = Reader.Tyre); }
		}

		#endregion


		#region Implementation of IXMLAxleDeclarationInputData

		public virtual IXMLAxleReader Reader { protected get; set; }

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationAxleDataProviderV20 : XMLDeclarationAxleDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "AxleDataDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		static XMLDeclarationAxleDataProviderV20()
		{
			NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;
		}

		public XMLDeclarationAxleDataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

	}

	// -----------------------------------------------------------------------------------

	public class XMLDeclarationAxleDataProviderV27 : AbstractCommonComponentType, IXMLAxleDeclarationInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

		public const string XSD_TYPE = "AxleDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected ITyreDeclarationInputData _tyre;
		protected bool? _twinTyre;
		protected AxleType? _axleType;

		public XMLDeclarationAxleDataProviderV27(IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IAxleDeclarationInputData

		public virtual bool TwinTyres
		{
			get
			{
				return _twinTyre ?? (_twinTyre = XmlConvert.ToBoolean(GetString(XMLNames.AxleWheels_Axles_Axle_TwinTyres))).Value;
			}
		}

		public virtual AxleType AxleType
		{
			get
			{
				return _axleType ?? (_axleType = GetString(XMLNames.AxleWheels_Axles_Axle_AxleType).ParseEnum<AxleType>()).Value;
			}
		}

		public virtual ITyreDeclarationInputData Tyre
		{
			get { return _tyre ?? (_tyre = Reader.Tyre); }
		}

		#endregion


		#region Implementation of IXMLAxleDeclarationInputData

		public virtual IXMLAxleReader Reader { protected get; set; }

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}
}

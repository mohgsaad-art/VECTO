using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAxlesDataProviderV10 : AbstractXMLType, IXMLAxlesDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "AxleWheelsDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IAxleDeclarationInputData[] _axles;


		public XMLDeclarationAxlesDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode)
		{
			DataSource = new DataSource() {
				SourceType = DataSourceType.XMLFile,
				SourceFile = sourceFile,
				SourceVersion = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace)
			};
		}

		#region Implementation of IAxlesDeclarationInputData

		public virtual IList<IAxleDeclarationInputData> AxlesDeclaration
		{
			get {
				if (_axles != null) {
					return _axles;
				}

				var axleNodes = GetNodes(new[] { XMLNames.AxleWheels_Axles, XMLNames.AxleWheels_Axles_Axle });
				if (axleNodes == null) {
					return new List<IAxleDeclarationInputData>();
				}

				_axles = new IAxleDeclarationInputData[axleNodes.Count];
				foreach (XmlNode axlenode in axleNodes) {
					var axleNumber = GetAttribute(axlenode, XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).ToInt();
					if (axleNumber < 1 || axleNumber > _axles.Length) {
						throw new VectoException("Axle #{0} exceeds axle count", axleNumber);
					}
					if (_axles[axleNumber - 1] != null) {
						throw new VectoException("Axle #{0} defined multiple times!", axleNumber);
					}

					_axles[axleNumber - 1] = Reader.CreateAxle(axlenode);
				}

				return _axles;
			}
		}

		public XmlNode XMLSource { get { return BaseNode; } }

		#endregion

		#region Implementation of IXMLResource

		public virtual DataSource DataSource { get; }

		#endregion

		#region Implementation of IXMLAxlesDeclarationInputData

		public virtual IXMLAxlesReader Reader { protected get; set; }

		#endregion

		protected virtual XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationAxlesDataProviderV20 : XMLDeclarationAxlesDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "AxleWheelsComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		
		public XMLDeclarationAxlesDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	public class XMLDeclarationAxlesDataProviderV27 : AbstractXMLType, IXMLTrailerAxlesDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

		public const string XSD_TYPE = "AxleWheelsDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected ITrailerAxleDeclarationInputData[] _axles;


		public XMLDeclarationAxlesDataProviderV27(
			IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode)
		{
			DataSource = new DataSource()
			{
				SourceType = DataSourceType.XMLFile,
				SourceFile = sourceFile,
				SourceVersion = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace)
			};
		}

		#region Implementation of IAxlesDeclarationInputData

		public virtual IList<ITrailerAxleDeclarationInputData> AxlesDeclaration
		{
			get
			{
				if (_axles != null)
				{
					return _axles;
				}

				var axleNodes = GetNodes(new[] { XMLNames.AxleWheels_Axles, XMLNames.AxleWheels_Axles_Axle });
				if (axleNodes == null)
				{
					return new List<ITrailerAxleDeclarationInputData>();
				}

				_axles = new ITrailerAxleDeclarationInputData[axleNodes.Count];
				foreach (XmlNode axlenode in axleNodes)
				{
					var axleNumber = GetAttribute(axlenode, XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).ToInt();
					if (axleNumber < 1 || axleNumber > _axles.Length)
					{
						throw new VectoException("Axle #{0} exceeds axle count", axleNumber);
					}
					if (_axles[axleNumber - 1] != null)
					{
						throw new VectoException("Axle #{0} defined multiple times!", axleNumber);
					}

					_axles[axleNumber - 1] = Reader.CreateAxle(axlenode);
				}

				return _axles;
			}
		}

		public XmlNode XMLSource { get { return BaseNode; } }

		#endregion

		#region Implementation of IXMLResource

		public virtual DataSource DataSource { get; }

		#endregion

		#region Implementation of IXMLAxlesDeclarationInputData

		public virtual IXMLTrailerAxlesReader Reader { protected get; set; }

		#endregion

		protected virtual XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}

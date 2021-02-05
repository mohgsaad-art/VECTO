using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationComponentInputDataProviderV10 : AbstractXMLResource, IXMLComponentInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_COMPONENT_NAMESPACE_URI_V10;

		//public const string XSD_TYPE = ""; // don't use type here  - bind this 'general' input provider to the namespace only

		public static readonly string QUALIFIED_XSD_TYPE = NAMESPACE_URI.NamespaceName;

		protected readonly XmlDocument Document;
		protected IDeclarationJobInputData JobData;
		private IComponentInputData _component;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLDeclarationComponentInputDataProviderV10(XmlDocument xmlDoc, string fileName) : base(
			xmlDoc.DocumentElement, fileName)
		{
			Document = xmlDoc;
			SourceType = DataSourceType.XMLFile;

			var h = VectoHash.Load(xmlDoc);
			XMLHash = h.ComputeXmlHash();

		}

		protected override XNamespace SchemaNamespace {
			get { return NAMESPACE_URI; }
		}


		protected override DataSourceType SourceType { get; }

		public virtual XElement XMLHash { get; private set; }
		public virtual IComponentInputData Component
		{
			get { return _component ?? (_component = ReadComponent()); }
		}

		protected virtual IComponentInputData ReadComponent()
		{

			var componentNode = Document.DocumentElement?.FirstChild;
			if (componentNode == null) {
				throw new VectoException("Failed to read XML Component {0}", SourceFile);
			}

			var dataNode =
				componentNode?.SelectSingleNode(string.Format("./*[local-name()='{0}']", XMLNames.ComponentDataWrapper));
			var type = (dataNode ?? componentNode).SchemaInfo.SchemaType;
			var version = XMLHelper.GetXsdType(type);

			var componentCreators = new Dictionary<string, Func<string, IXMLDeclarationVehicleData, XmlNode, string, IComponentInputData>>() {
				{XMLNames.Component_Engine, Factory.CreateEngineData},
                {XMLNames.Component_Gearbox, Factory.CreateGearboxData},
                {XMLNames.Component_Axlegear, Factory.CreateAxlegearData},
                {XMLNames.Component_Retarder, Factory.CreateRetarderData},
                {XMLNames.Component_Angledrive, Factory.CreateAngledriveData},
                {XMLNames.Component_TorqueConverter, Factory.CreateTorqueconverterData},
			};

			if (componentCreators.ContainsKey(componentNode.LocalName)) {
				return componentCreators[componentNode.LocalName](version, null, componentNode, SourceFile);
			}

			if (componentNode.LocalName.Equals(XMLNames.AxleWheels_Axles_Axle_Tyre,
				StringComparison.InvariantCulture)) {
				return Factory.CreateTyre(version, componentNode, SourceFile);
			}
			
			throw new VectoException("Unknown component node {}", Document.DocumentElement?.FirstChild.LocalName);
			
		}
	}

	//-------------------------

	public class XMLDeclarationComponentInputDataProviderV20 : XMLDeclarationComponentInputDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_COMPONENT_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = ""; // don't use type here  - bind this 'general' input provider to the namespace only

		public new static readonly string QUALIFIED_XSD_TYPE = NAMESPACE_URI.NamespaceName;

		

		public XMLDeclarationComponentInputDataProviderV20(XmlDocument xmlDoc, string fileName) : base(
			xmlDoc, fileName)
		{
			

		}

		protected override XNamespace SchemaNamespace {
			get { return NAMESPACE_URI; }
		}


		protected override DataSourceType SourceType { get; }

		
	}

}
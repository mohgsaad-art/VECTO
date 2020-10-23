using System;
using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public abstract class AbstractComponentReader
	{
		protected readonly XmlNode BaseNode;
		protected readonly IXMLResource ParentComponent;

		public AbstractComponentReader(IXMLResource parent, XmlNode baseNode)
		{
			BaseNode = baseNode;
			ParentComponent = parent;
		}

		protected virtual T CreateComponent<T>(
			string component, Func<string, XmlNode, string, T> componentCreator, bool createDummy = false)
		{
			var componentNode = BaseNode.LocalName == component
				? BaseNode
				: BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(component));
			var dataNode =
				componentNode?.SelectSingleNode(string.Format("./*[local-name()='{0}']", XMLNames.ComponentDataWrapper));
			if (componentNode != null) {
				var type =  (dataNode ?? componentNode).SchemaInfo.SchemaType;
				var version = XMLHelper.GetXsdType(type);
				if (string.IsNullOrWhiteSpace(version)) {
					version = XMLHelper.GetVersionFromNamespaceUri(((dataNode ?? componentNode).SchemaInfo.SchemaType?.Parent as XmlSchemaElement)?.QualifiedName.Namespace);
				}
				return componentCreator(version, componentNode, ParentComponent.DataSource.SourceFile);
			}

			if (createDummy) {
				try {
					return componentCreator(null, null, null);
				} catch (Exception e) {
					throw new VectoException("failed to create dummy instance for component {0}", e, component);
				}
			}

			throw new VectoException("Component {0} not found!", component);
		}

		protected virtual T GetReader<T>(IXMLDeclarationVehicleData vehicle, XmlNode node, Func<string, IXMLDeclarationVehicleData, XmlNode, T> creator) where T : class 
		{
			if (node == null) {
				return null;
			}
			var version = XMLHelper.GetXsdType(node.SchemaInfo.SchemaType);
			if (string.IsNullOrWhiteSpace(version)) {
				version = XMLHelper.GetVersionFromNamespaceUri((node.SchemaInfo.SchemaType?.Parent as XmlSchemaElement)?.QualifiedName.Namespace);
			}
			return creator(version, vehicle, node);
		}

		protected virtual T GetReader<T>(IXMLTrailerDeclarationVehicleData vehicle, XmlNode node, Func<string, IXMLTrailerDeclarationVehicleData, XmlNode, T> creator) where T : class 
		{
			if (node == null) {
				return null;
			}
			var version = XMLHelper.GetXsdType(node.SchemaInfo.SchemaType);
			if (string.IsNullOrWhiteSpace(version)) {
				version = XMLHelper.GetVersionFromNamespaceUri((node.SchemaInfo.SchemaType?.Parent as XmlSchemaElement)?.QualifiedName.Namespace);
			}
			return creator(version, vehicle, node);
		}

	}
}

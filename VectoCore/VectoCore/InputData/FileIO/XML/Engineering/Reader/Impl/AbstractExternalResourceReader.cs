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

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	internal abstract class AbstractExternalResourceReader
	{
		protected XmlNode BaseNode;
		protected IXMLResource ParentComponent;

		public AbstractExternalResourceReader(IXMLResource parent, XmlNode baseNode)
		{
			BaseNode = baseNode;

			ParentComponent = parent;
		}

		protected virtual T CreateComponent<T>(
			string component, Func<string, XmlNode, string, T> componentCreator, bool allowExternalResource = true,
			bool createDummy = false, bool requireDataNode = true)
		{
			var componentNode = BaseNode.LocalName == component
				? BaseNode
				: BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(component));
			var dataNode = requireDataNode
				? componentNode?.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']")
				: componentNode;
			var componentResourceNode = componentNode?.SelectSingleNode(
											$"./*[local-name()='{XMLNames.ExternalResource}'" +
											$" and @{XMLNames.ExtResource_Type_Attr}='{XMLNames.ExtResource_Type_Value_XML}']") ??
										BaseNode?.SelectSingleNode(
											$"./*[local-name()='{XMLNames.ExternalResource}'" +
											$" and @{XMLNames.ExtResource_Component_Attr}='{component}'" +
											$" and @{XMLNames.ExtResource_Type_Attr}='{XMLNames.ExtResource_Type_Value_XML}']");
			if (dataNode != null && componentResourceNode == null) {
				var type = dataNode.SchemaInfo.SchemaType;
				var version = XMLHelper.GetXsdType(type);

				try {
					return componentCreator(version, componentNode, ParentComponent.DataSource.SourceFile);
				} catch (Exception e) {
					throw new VectoException("Failed to create component {0} version {1}.", e, component, version);
				}
			}

			if (!allowExternalResource && !createDummy) {
				throw new VectoException("Component {0} not found!", component);
			}

			
			if (componentResourceNode != null) {
				try {
					var componentFile = componentResourceNode.Attributes?.GetNamedItem(XMLNames.ExtResource_File_Attr).InnerText;
					var fullFileName = componentFile == null
						? null
						: Path.Combine(ParentComponent.DataSource.SourcePath, componentFile);
					if (componentFile == null || !File.Exists(fullFileName)) {
						throw new VectoException(
							"Referenced component file '{1}' for component '{0}' not found!", component, componentFile);
					}

					var componentDocument = new XmlDocument();
					componentDocument.Load(XmlReader.Create(fullFileName));
					if (componentDocument.DocumentElement == null) {
						throw new VectoException("invalid xml file for component {0}, file {1}", component, componentFile);
					}

					new XMLValidator(componentDocument, null, XMLValidator.CallBackExceptionOnError).ValidateXML(
						XmlDocumentType.EngineeringJobData);

					var docComponentNode = componentDocument.DocumentElement.LocalName == component
						? componentDocument.DocumentElement
						: componentDocument.DocumentElement.SelectSingleNode(XMLHelper.QueryLocalName(component));
					var docDataNode =
						docComponentNode?.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']") ??
						docComponentNode;

					var type = (docDataNode)?.SchemaInfo.SchemaType;
					var version = XMLHelper.GetXsdType(type);
					try {
						return componentCreator(version, componentDocument.DocumentElement, fullFileName);
					} catch (Exception e) {
						throw new VectoException("Failed to create component {0} version {1}.", e, component, version);
					}
				} catch (XmlSchemaValidationException validationException) {
					throw new VectoException("Validation of XML-file for component {0} failed", validationException, component);
				}
			}

			if (createDummy) {
				try {
					return componentCreator(null, null, null);
				} catch (Exception e) {
					throw new VectoException("Failed to create dummy instance for component {0}.", e, component);
				}
			}

			throw new VectoException("Component {0} not found!", component);
		}

		protected virtual T GetReader<T>(
			IXMLEngineeringVehicleData vehicle, XmlNode node, Func<string, IXMLEngineeringVehicleData, XmlNode, T> creator)
			where T : class
		{
			if (node == null) {
				return null;
			}

			var version = XMLHelper.GetXsdType(node.SchemaInfo.SchemaType);
			if (string.IsNullOrWhiteSpace(version)) {
				version = XMLHelper.GetVersionFromNamespaceUri(
					(node.SchemaInfo.SchemaType?.Parent as XmlSchemaElement)?.QualifiedName.Namespace);
			}
			return creator(version, vehicle, node);
		}
	}
}

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
				componentNode?.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']");
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
	}
}

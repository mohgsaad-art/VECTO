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

using System.Collections.Generic;
using System.Linq;
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

		public int? NumSteeredAxles=> AxlesDeclaration.Count(x => x.Steered);
					
		public XmlNode XMLSource => BaseNode;

		#endregion

		#region Implementation of IXMLResource

		public virtual DataSource DataSource { get; }

		#endregion

		#region Implementation of IXMLAxlesDeclarationInputData

		public virtual IXMLAxlesReader Reader { protected get; set; }

		#endregion

		protected virtual XNamespace SchemaNamespace => NAMESPACE_URI;
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

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationAxlesDataProviderV01 : XMLDeclarationAxlesDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "AxleWheelsDataPIFType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationAxlesDataProviderV01(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}

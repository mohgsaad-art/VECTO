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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringAuxiliariesDataProviderV07 : AbstractXMLType, IXMLAuxiliairesData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AuxiliariesDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAuxiliariesDataProviderV07(
			IXMLEngineeringVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		public IXMLAuxiliaryReader Reader { protected get; set; }

		#region Implementation of IAuxiliariesEngineeringInputData

		public virtual IAuxiliaryEngineeringInputData Auxiliaries
		{
			get;
		}

		public IBusAuxiliariesEngineeringData BusAuxiliariesData =>
			// TODO: MQ 20210211 - implement...
			null;

		public Watt ElectricAuxPower { get; }

		#endregion
	}

	internal class XMLEngineeringAuxiliariesDataProviderV10 : XMLEngineeringAuxiliariesDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AuxiliariesDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAuxiliariesDataProviderV10(
			IXMLEngineeringVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }
	}

	internal class XMLAuxiliaryEngineeringDataV07 : AbstractXMLType, IXMLAuxiliaryData
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AuxiliaryEntryEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		protected string BasePath;

		public XMLAuxiliaryEngineeringDataV07(XmlNode node, string basePath) : base(node)
		{
			BasePath = basePath;

		}

		protected virtual XNamespace SchemaNamespace => NAMESPACE_URI;

		#region Implementation of IAuxiliaryEngineeringInputData


		public virtual Watt ConstantPowerDemand
		{
			get;
		}

		public Watt PowerDemandICEOffDriving { get; }
		public Watt PowerDemandICEOffStandstill { get; }
		public Watt ElectricPowerDemand { get; }


		#endregion
	}

	internal class XMLAuxiliaryEngineeringDataV10 : XMLAuxiliaryEngineeringDataV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AuxiliaryEntryEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLAuxiliaryEngineeringDataV10(XmlNode node, string basePath) : base(node, basePath) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}

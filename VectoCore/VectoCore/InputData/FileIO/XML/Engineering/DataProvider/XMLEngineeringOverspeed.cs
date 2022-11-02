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
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringOverspeedV07 : AbstractXMLType, IXMLOverspeedData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "OverspeedEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		protected IXMLEngineeringDriverData DriverData;

		public XMLEngineeringOverspeedV07(IXMLEngineeringDriverData driverData, XmlNode node) : base(node)
		{
			DriverData = driverData;
		}

		#region Implementation of IOverSpeedEcoRollDeclarationInputData

		public virtual bool Enabled => GetNode(XMLNames.DriverModel_Overspeed_Mode, required: false)?.InnerText.Equals("Overspeed", StringComparison.InvariantCultureIgnoreCase) ?? false;

		#endregion

		#region Implementation of IOverSpeedEcoRollEngineeringInputData

		public virtual MeterPerSecond MinSpeed => GetNode(XMLNames.DriverModel_Overspeed_MinSpeed, required: false)?.InnerText.ToDouble().KMPHtoMeterPerSecond();

		public virtual MeterPerSecond OverSpeed => GetNode(XMLNames.DriverModel_Overspeed_AllowedOverspeed, required: false)?.InnerText.ToDouble().KMPHtoMeterPerSecond();

		public virtual MeterPerSecond UnderSpeed => GetNode(XMLNames.DriverModel_Overspeed_AllowedUnderspeed, required: false)?.InnerText.ToDouble().KMPHtoMeterPerSecond();

		#endregion
	}

	internal class XMLEngineeringOverspeedV10 : XMLEngineeringOverspeedV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "OverspeedEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		public XMLEngineeringOverspeedV10(IXMLEngineeringDriverData driverData, XmlNode node) : base(driverData, node) { }

		public override bool Enabled => XmlConvert.ToBoolean(GetNode(XMLNames.DriverModel_Overspeed_Enabled, required: false)?.InnerText ?? "false");
	}
}

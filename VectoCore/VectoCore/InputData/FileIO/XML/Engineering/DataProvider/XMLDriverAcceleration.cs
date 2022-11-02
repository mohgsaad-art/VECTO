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

using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLDriverAccelerationV07 : AbstractXMLType, IXMLDriverAcceleration
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "DriverAccelerationCurveEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		protected IXMLEngineeringDriverData DriverData;
		private IDriverAccelerationData _accelerationCurve;

		public XMLDriverAccelerationV07(IXMLEngineeringDriverData driverData, XmlNode node) : base(node)
		{
			DriverData = driverData;
		}

		public XMLDriverAccelerationV07(TableData accCurve) : base(null)
		{
			_accelerationCurve = new DriverAccelerationInputData() { AccelerationCurve = accCurve };
		}

		public virtual IDriverAccelerationData AccelerationCurve =>
			BaseNode == null
				? _accelerationCurve
				: new DriverAccelerationInputData() {
					AccelerationCurve = XMLHelper.ReadEntriesOrResource(
						BaseNode, DriverData.DataSource.SourcePath, null, XMLNames.DriverModel_DriverAccelerationCurve_Entry,
						AttributeMappings.DriverAccelerationCurveMapping)
				};
	}

	internal class XMLDriverAccelerationV10 : XMLDriverAccelerationV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "DriverAccelerationCurveType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		public XMLDriverAccelerationV10(IXMLEngineeringDriverData driverData, XmlNode node) : base(driverData, node) { }
	}
}

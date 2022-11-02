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
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLTyreEngineeringDataProviderV10 : AbstractEngineeringXMLComponentDataProvider, IXMLTyreData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public const string XSD_TYPE = "TyreDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLTyreEngineeringDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) :
			base(vehicle, baseNode, source)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == source ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		#region Implementation of ITyreDeclarationInputData

		public virtual string Dimension => GetNode(XMLNames.AxleWheels_Axles_Axle_Dimension, required:false)?.InnerText;

		public virtual double RollResistanceCoefficient => GetNode(XMLNames.AxleWheels_Axles_Axle_RRCISO, required: false)?.InnerText.ToDouble() ?? double.NaN;

		public virtual Newton TyreTestLoad => GetNode(XMLNames.AxleWheels_Axles_Axle_FzISO, required: false)?.InnerText.ToDouble().SI<Newton>();

		public virtual string FuelEfficiencyClass => DeclarationData.Wheels.TyreClass.Lookup(RollResistanceCoefficient);

		#endregion

		#region Implementation of ITyreEngineeringInputData

		public virtual KilogramSquareMeter Inertia => GetNode(XMLNames.AxleWheels_Axles_Axle_Inertia, required: false)?.InnerText.ToDouble().SI<KilogramSquareMeter>();

		public virtual Meter DynamicTyreRadius => GetNode(XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius, required: false)?.InnerText.ToDouble().SI(Unit.SI.Milli.Meter).Cast<Meter>();

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLTyreEngineeringDataProviderV10TEST : XMLTyreEngineeringDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		public new const string XSD_TYPE = "TyreDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLTyreEngineeringDataProviderV10TEST(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(vehicle, baseNode, source) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}

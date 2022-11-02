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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringAxlesDataProviderV07 : AbstractEngineeringXMLComponentDataProvider,
		IXMLAxlesData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AxleWheelsDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAxlesDataProviderV07(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source)
			: base(vehicle, baseNode, source)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == source ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		#region Implementation of IAxlesEngineeringInputData

		public virtual IList<IAxleEngineeringInputData> AxlesEngineering
		{
			get {
				var axleNodes = GetNodes(new[] { XMLNames.AxleWheels_Axles, XMLNames.AxleWheels_Axles_Axle });
				if (axleNodes == null || axleNodes.Count == 0) {
					return new List<IAxleEngineeringInputData>();
				}

				var retVal = new IAxleEngineeringInputData[axleNodes.Count];
				foreach (XmlNode axleNode in axleNodes) {
					var axleNumber = GetAttribute(axleNode, XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).ToInt();
					if (axleNumber < 1 || axleNumber > retVal.Length) {
						throw new VectoException("Axle #{0} exceeds axle count", axleNumber);
					}
					if (retVal[axleNumber - 1] != null) {
						throw new VectoException("Axle #{0} defined multiple times!", axleNumber);
					}

					retVal[axleNumber - 1] = Reader.CreateAxle(axleNode);
				}

				return retVal;
			}
		}

		public IXMLAxlesReader Reader { protected get; set; }

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }


		#endregion
	}

	internal class XMLEngineeringAxlesDataProviderV10 : XMLEngineeringAxlesDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AxleWheelsDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		public XMLEngineeringAxlesDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(
			vehicle, baseNode, source) { }

		#region Overrides of XMLEngineeringAxlesDataProviderV07

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#endregion
	}

	internal class XMLAxleEngineeringDataV07 : AbstractEngineeringXMLComponentDataProvider, IXMLAxleEngineeringData,
		ITyreEngineeringInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

				public const string XSD_TYPE = "AxleDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLAxleEngineeringDataV07(XmlNode node, IXMLEngineeringVehicleData vehicle) : base(
			vehicle, node, (vehicle as IXMLResource).DataSource.SourcePath)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IAxleDeclarationInputData

		public virtual bool TwinTyres => XmlConvert.ToBoolean(GetNode(XMLNames.AxleWheels_Axles_Axle_TwinTyres)?.InnerText ?? "");

		public virtual bool Steered => XmlConvert.ToBoolean(GetNode(XMLNames.AxleWheels_Axles_Axle_Steered)?.InnerText ?? "");

		public virtual AxleType AxleType => (GetNode(XMLNames.AxleWheels_Axles_Axle_AxleType)?.InnerText ?? "").ParseEnum<AxleType>();

		public virtual double AxleWeightShare => GetNode(XMLNames.AxleWheels_Axles_Axle_WeightShare)?.InnerText.ToDouble() ?? 0;

		public virtual ITyreEngineeringInputData Tyre => this;

		ITyreDeclarationInputData IAxleDeclarationInputData.Tyre => throw new NotImplementedException();

		#endregion

		#region Implementation of ITyreDeclarationInputData

		public virtual string Dimension => GetNode(XMLNames.AxleWheels_Axles_Axle_Dimension)?.InnerText;

		public virtual double RollResistanceCoefficient => GetNode(XMLNames.AxleWheels_Axles_Axle_RRCISO)?.InnerText.ToDouble() ?? double.NaN;

		public virtual Newton TyreTestLoad => GetNode(XMLNames.AxleWheels_Axles_Axle_FzISO)?.InnerText.ToDouble().SI<Newton>();

		public virtual string FuelEfficiencyClass => DeclarationData.Wheels.TyreClass.Lookup(RollResistanceCoefficient);

		#endregion

		#region Implementation of ITyreEngineeringInputData

		public virtual KilogramSquareMeter Inertia => GetNode(XMLNames.AxleWheels_Axles_Axle_Inertia)?.InnerText.ToDouble().SI<KilogramSquareMeter>();

		public virtual Meter DynamicTyreRadius => GetNode(XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius)?.InnerText.ToDouble().SI(Unit.SI.Milli.Meter).Cast<Meter>();

		#endregion

		public IXMLAxleReader Reader { protected get; set; }

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLAxleEngineeringDataV10 : AbstractEngineeringXMLComponentDataProvider, IXMLAxleEngineeringData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public const string XSD_TYPE = "AxleDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		private ITyreEngineeringInputData _tyre;

		public XMLAxleEngineeringDataV10(XmlNode node, IXMLEngineeringVehicleData vehicle) : base(
			vehicle, node, (vehicle as IXMLResource).DataSource.SourcePath)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IAxleDeclarationInputData

		public virtual bool TwinTyres => XmlConvert.ToBoolean(GetNode(XMLNames.AxleWheels_Axles_Axle_TwinTyres)?.InnerText ?? "");

		public virtual bool Steered => XmlConvert.ToBoolean(GetNode(XMLNames.AxleWheels_Axles_Axle_Steered)?.InnerText ?? "");

		public virtual AxleType AxleType => (GetNode(XMLNames.AxleWheels_Axles_Axle_AxleType)?.InnerText ?? "").ParseEnum<AxleType>();

		public virtual ITyreEngineeringInputData Tyre => _tyre ?? (_tyre = Reader.Tyre);


		public virtual double AxleWeightShare => GetNode(XMLNames.AxleWheels_Axles_Axle_WeightShare)?.InnerText.ToDouble() ?? 0;

		ITyreDeclarationInputData IAxleDeclarationInputData.Tyre => Tyre;

		#endregion

		public IXMLAxleReader Reader { protected get; set; }

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLAxleEngineeringDataV10TEST : XMLAxleEngineeringDataV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		//public new const string XSD_TYPE = "AxleDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLAxleEngineeringDataV10TEST(XmlNode node, IXMLEngineeringVehicleData vehicle) : base(node, vehicle) { }

		public override bool TwinTyres => XmlConvert.ToBoolean(GetNode("TwinTires")?.InnerText ?? "");

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}

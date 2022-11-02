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

using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringAxleWriterV10 : AbstractXMLWriter, IXMLEngineeringAxleWriter
	{
		protected XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringAxleWriterV10() : base("AxleDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#region Overrides of AbstractXMLWriter

		public override object[] WriteXML(IAxleEngineeringInputData axle, int idx, Meter dynamicTyreRadius)
		{
			var tns = ComponentDataNamespace;
			return new object[] {
				new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, idx),
				new XElement(
					tns + XMLNames.AxleWheels_Axles_Axle_AxleType,
					idx == 2 ? AxleType.VehicleDriven.ToString() : AxleType.VehicleNonDriven.ToString()),
				new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres_Attr, axle.TwinTyres),
				new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Steered, idx == 1),
				new XElement(tns + XMLNames.AxleWheels_Axles_Axle_WeightShare, axle.AxleWeightShare),
				CreateTyre(axle.Tyre, idx)
			};
		}

		protected virtual XElement CreateTyre(ITyreEngineeringInputData tyre, int idx)
		{
			var component = XMLNames.AxleWheels_Axles_Axle_Tyre;
			if (Writer.Configuration.SingleFile) {
				var tns = ComponentDataNamespace;
				var tyreWriter = Factory.GetWriter(tyre, Writer);
				return new XElement(
					tns + component,
					new XElement(
						tns + XMLNames.ComponentDataWrapper,
						tyreWriter.WriteXML(tyre)));
			}

			var document = Writer.WriteComponent(tyre);
			return ExtComponent(document, component, Writer.GetFilename(tyre, tyre.Dimension ?? idx.ToString()));
		}

		#endregion

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		#endregion
	}


	internal class XMLEngineeringAxleWriterV10TEST : XMLEngineeringAxleWriterV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		public override object[] WriteXML(IAxleEngineeringInputData axle, int idx, Meter dynamicTyreRadius)
		{
			var tns = ComponentDataNamespace;
			return new object[] {
				new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, idx),
				new XElement(tns + "WeightShare", axle.AxleWeightShare),
				new XElement(tns + "TwinTires", axle.TwinTyres),
				CreateTyre(axle.Tyre, idx)
			};
		}

		protected override XElement CreateTyre(ITyreEngineeringInputData tyre, int idx)
		{
			var component = "Tire";
			if (Writer.Configuration.SingleFile) {
				var tns = ComponentDataNamespace;
				var v10 = Writer.RegisterNamespace(XMLEngineeringAxleWriterV10.NAMESPACE_URI);
				var tyreWriter = Factory.GetWriter(tyre, Writer);
				return new XElement(
					tns + component,
					new XElement(
						v10 + XMLNames.ComponentDataWrapper,
						tyreWriter.WriteXML(tyre)));
			}

			var document = Writer.WriteComponent(tyre);
			return ExtComponent(document, component, Writer.GetFilename(tyre, tyre.Dimension ?? idx.ToString()));
		}
	}
}

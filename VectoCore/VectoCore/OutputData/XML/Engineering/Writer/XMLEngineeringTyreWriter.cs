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
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringTyreWriterV10 : AbstractComponentWriter<ITyreEngineeringInputData>,
		IXmlEngineeringTyreWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;
		public XMLEngineeringTyreWriterV10() : base("TyreDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion

		#region Overrides of AbstractComponentWriter<ITyreEngineeringInputData>

		protected override object[] DoWriteXML(ITyreEngineeringInputData inputData)
		{
			var tns = ComponentDataNamespace;

			//var retVal = new list<object> {
			var tyre = new List<object> { 
				GetXMLTypeAttribute(),
				GetDefaultComponentElements(inputData)
				
			};

			if (string.IsNullOrWhiteSpace(inputData.Dimension)) {
				tyre.Add(new XElement(
							tns + XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius, inputData.DynamicTyreRadius.Value() * 1000));
				tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Inertia, inputData.Inertia.Value()));
			} else {
				tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Dimension, inputData.Dimension));
			}
			tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_RRCISO, inputData.RollResistanceCoefficient.ToXMLFormat(4)));
			tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_FzISO, inputData.TyreTestLoad.Value().ToXMLFormat(0)));

			return tyre.ToArray();
		}

		#endregion
	}
}

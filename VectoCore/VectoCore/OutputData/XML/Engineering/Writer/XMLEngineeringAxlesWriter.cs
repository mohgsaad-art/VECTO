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
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringAxlesWriterV10 : AbstractXMLWriter, IXMLEngineeringAxlesWriter
	{
		private XNamespace _componentDataNamespace;
		private object[] _axles;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringAxlesWriterV10() : base("AxleWheelsDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		#region Overrides of AbstractXMLWriter

		public override object[] WriteXML(IComponentInputData vehicle)
		{
			return _axles ?? (_axles = CreateAxles(vehicle));
		}

		private object[] CreateAxles(IComponentInputData inputData)
		{
			var vehicle = inputData as IVehicleEngineeringInputData;
			if (vehicle == null) {
				return null;
			}
			var retVal = new List<object>();
			var tns = ComponentDataNamespace;
			var idx = 1;
			foreach (var axle in vehicle.Components.AxleWheels.AxlesEngineering) {
				var writer = Factory.GetWriter(axle, Writer, axle.DataSource);
				retVal.Add(
					new XElement(
						tns + XMLNames.AxleWheels_Axles_Axle,
						writer.GetXMLTypeAttribute(),
						writer.WriteXML(axle, idx++, vehicle.DynamicTyreRadius)
					));
			}

			return new object[] { new XElement(tns + XMLNames.AxleWheels_Axles, retVal.ToArray()) };
		}

		#endregion

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		#endregion
	}
}

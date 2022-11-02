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
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	public interface IXMLEngineeringGearboxWriter : IXMLEngineeringComponentWriter { }

	internal class XMLEngineeringGearboxWriterV10 : AbstractComponentWriter<IGearboxEngineeringInputData>,
		IXMLEngineeringGearboxWriter
	{
		private XNamespace _componentNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringGearboxWriterV10() : base("GearboxDataEngineeringType") { }

		#region Overrides of AbstractComponentWriter<IGearboxEngineeringInputData>

		protected override object[] DoWriteXML(IGearboxEngineeringInputData data)
		{
			var tns = Writer.RegisterNamespace(NAMESPACE_URI);
			var gears = new XElement(tns + XMLNames.Gearbox_Gears);
			var i = 1;

			foreach (var gearData in data.Gears) {
				var gearWriter = Factory.GetWriter(gearData, Writer, gearData.DataSource);
				var gear = gearWriter.WriteXML(gearData, i++);

				gears.Add(gear);
			}

			return new object[] {
				GetXMLTypeAttribute(),
				new XAttribute(XMLNames.Component_ID_Attr, $"GBX-{data.Model}"),

				GetDefaultComponentElements(data),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, data.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Gearbox_Inertia, data.Inertia.Value()),
				new XElement(tns + XMLNames.Gearbox_TractionInterruption, data.TractionInterruption.Value()), gears
			};
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentNamespace ?? (_componentNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		#endregion
	}
}

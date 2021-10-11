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
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringDriverDataWriterV10 : AbstractXMLWriter, IXMLEngineeringDriverDataWriter
	{
		private XNamespace _componentDataNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringDriverDataWriterV10() : base("DriverModelEngineeringType") { }

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IEngineeringInputDataProvider inputData)
		{
			var driverData = inputData.DriverInputData;
			var v10 = ComponentDataNamespace;

			var lacWriter = Factory.GetWriter(driverData.Lookahead, Writer, inputData.DataSource);
			var overspeedWriter = Factory.GetWriter(driverData.OverSpeedData, Writer, inputData.DataSource);
			var accWriter = Factory.GetWriter(driverData.AccelerationCurve, Writer, inputData.DataSource);
			var gearshiftWriter = Factory.GetWriter(driverData.GearshiftInputData, Writer, inputData.DataSource);
			return new object[] {
				new XElement(
					v10 + XMLNames.DriverModel_LookAheadCoasting,
					lacWriter.GetXMLTypeAttribute(),
					lacWriter.WriteXML(driverData.Lookahead)
				),
				new XElement(
					v10 + XMLNames.DriverModel_Overspeed,
					overspeedWriter.GetXMLTypeAttribute(),
					overspeedWriter.WriteXML(driverData.OverSpeedData)
				),
				new XElement(
					v10 + XMLNames.DriverModel_DriverAccelerationCurve,
					accWriter.GetXMLTypeAttribute(),
					accWriter.WriteXML(driverData.AccelerationCurve)
				),
				new XElement(
					v10 + XMLNames.DriverModel_ShiftStrategyParameters,
					gearshiftWriter.GetXMLTypeAttribute(),
					gearshiftWriter.WriteXML(driverData.GearshiftInputData)
				)
			};
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion
	}
}

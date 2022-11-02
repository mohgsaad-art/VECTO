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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer.DriverData
{
	internal class XMLShiftParmeterDataWriterV10 : AbstractXMLWriter, IXMLGearshiftDataWriter
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		private XNamespace _componentDataNamespace;
		public XMLShiftParmeterDataWriterV10() : base("ShiftStrategyParametersEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var gearshift = inputData as IGearshiftEngineeringInputData;
			var tns = ComponentDataNamespace;
			if (gearshift == null) {
				return new object[] { };
			}

			return new object[] {
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_UpshiftMinAcceleration,
					gearshift.UpshiftMinAcceleration.Value()),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_DownshiftAfterUpshiftDelay,
					gearshift.DownshiftAfterUpshiftDelay.Value()),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_UpshiftAfterDownshiftDelay,
					gearshift.UpshiftAfterDownshiftDelay.Value()),
				new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_TorqueReserve, gearshift.TorqueReserve),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_TimeBetweenGearshift,
					gearshift.MinTimeBetweenGearshift.Value()),
				new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_StartSpeed, gearshift.StartSpeed.Value()),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_StartAcceleration, gearshift.StartAcceleration.Value()),
				new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_StartTorqueReserve, gearshift.StartTorqueReserve)
			};
		}

		#endregion
	}
}

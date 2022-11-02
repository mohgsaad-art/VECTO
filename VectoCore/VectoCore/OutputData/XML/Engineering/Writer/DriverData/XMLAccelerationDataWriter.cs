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

using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer.DriverData
{
	internal class XMLAccelerationDataWriterV10 : AbstractXMLWriter, IXMLAccelerationDataWriter
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		private XNamespace _componentDataNamespace;
		public XMLAccelerationDataWriterV10() : base("DriverAccelerationCurveEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var acc = inputData as IDriverAccelerationData;
			
			if (acc == null) {
				return new object[] { };
			}

			if (acc.AccelerationCurve.SourceType == DataSourceType.Embedded &&
				acc.AccelerationCurve.Source.StartsWith(DeclarationData.DeclarationDataResourcePrefix)) {

				var filename = acc.AccelerationCurve.Source.Replace(DeclarationData.DeclarationDataResourcePrefix + ".VACC.", "")
								.Replace(Constants.FileExtensions.DriverAccelerationCurve, "");
				var tns = Writer.RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
				return new object[] {
					new XElement(
						tns + XMLNames.ExternalResource,
						new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
						new XAttribute(XMLNames.ExtResource_File_Attr, filename))
				};
			}

			return new object[] {
				Writer.Configuration.SingleFile
					? EmbedDataTable(acc.AccelerationCurve, AttributeMappings.DriverAccelerationCurveMapping)
					: ExtCSVResource(
						acc.AccelerationCurve,
						Path.GetFileName(acc.AccelerationCurve.Source ?? "Driver.vacc"))
			};
		}

		#endregion
	}
}

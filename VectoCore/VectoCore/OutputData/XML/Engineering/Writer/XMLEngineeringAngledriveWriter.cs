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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringAngledriveWriterV10 : AbstractComponentWriter<IAngledriveInputData>,
		IXMLEngineeringAngledriveWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringAngledriveWriterV10() : base("AngledriveDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion

		#region Overrides of AbstractComponentWriter<IAxleGearInputData>

		protected override object[] DoWriteXML(IAngledriveInputData data)
		{
			var tns = ComponentDataNamespace;
			var typeId = $"ANGLDRV-{data.Ratio:0.000}";
			return new object[] {
				GetXMLTypeAttribute(),
				new XAttribute(XMLNames.Component_ID_Attr, typeId),

				GetDefaultComponentElements(data),
				new XElement(tns + XMLNames.Axlegear_Ratio, data.Ratio.ToXMLFormat(3)),
				data.LossMap == null
					? new XElement(
						tns + XMLNames.Axlegear_TorqueLossMap,
						new XElement(tns + XMLNames.Axlegear_Efficiency, data.Efficiency))
					: new XElement(tns + XMLNames.Axlegear_TorqueLossMap, GetTransmissionLossMap(data.LossMap))
			};
		}

		#endregion

		protected virtual object[] GetTransmissionLossMap(TableData lossmap)
		{
			if (Writer.Configuration.SingleFile) {
				return EmbedDataTable(lossmap, AttributeMappings.TransmissionLossmapMapping);
			}

			return ExtCSVResource(lossmap, Path.Combine(Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters(Path.GetFileName(lossmap.Source))));
		}
	}
}

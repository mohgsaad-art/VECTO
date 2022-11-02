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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringAirdragWriterV10 : AbstractComponentWriter<IAirdragEngineeringInputData>,
		IXMLEngineeringAirdragWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;
		public XMLEngineeringAirdragWriterV10() : base("AirDragDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion

		#region Overrides of AbstractComponentWriter<IAirdragEngineeringInputData>

		protected override object[] DoWriteXML(IAirdragEngineeringInputData data)
		{
			var tns = ComponentDataNamespace;
			var id = $"Airdrag-{data.Model}";
			return new object[] {
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetXMLTypeAttribute(),
					GetDefaultComponentElements(data),
					new XElement(tns + XMLNames.Vehicle_CrossWindCorrectionMode, data.CrossWindCorrectionMode.ToXMLFormat()),
					new XElement(tns + XMLNames.Vehicle_AirDragArea, data.AirDragArea.Value().ToXMLFormat(2)),
				GetCrossWindCorrectionData(data)
			};
		}

		#endregion

		protected virtual XElement GetCrossWindCorrectionData(IAirdragEngineeringInputData airdrag)
		{
			if (airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.NoCorrection ||
				airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.DeclarationModeCorrection) {
				return null;
			}

			var correctionMap = new XElement(ComponentDataNamespace + XMLNames.Vehicle_CrosswindCorrectionData);

			if (Writer.Configuration.SingleFile) {
				correctionMap.Add(EmbedDataTable(airdrag.CrosswindCorrectionMap, AttributeMappings.CrossWindCorrectionMapping));
			} else {
				var ext = airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.SpeedDependentCorrectionFactor
					? "vcdv"
					: "vcdb";
				correctionMap.Add(ExtCSVResource(airdrag.CrosswindCorrectionMap, Path.Combine(Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("CrossWindCorrection." + ext))));
			}

			return correctionMap;
		}
	}
}

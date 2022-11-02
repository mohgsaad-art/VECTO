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

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer {
	internal class XMLEngineeringADASWriterV10 : AbstractXMLWriter,
		IXMLEngineeringADASWriter
	{

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringADASWriterV10() : base("AdvancedDriverAssistEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override object[] WriteXML(IAdvancedDriverAssistantSystemsEngineering inputData)
		{
			var adas = inputData;
			if (adas == null) {
				return null;
			}

			var ns = ComponentDataNamespace;
			return new object[] { new XElement(ns + XMLNames.Vehicle_AdvancedDriverAssist, 
				new XElement(ns + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(ns + XMLNames.Vehicle_ADAS_EcoRoll, adas.EcoRoll.ToXMLFormat()),
				new XElement(ns + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()))};
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => Writer.RegisterNamespace(NAMESPACE_URI);

		#endregion
	}
}
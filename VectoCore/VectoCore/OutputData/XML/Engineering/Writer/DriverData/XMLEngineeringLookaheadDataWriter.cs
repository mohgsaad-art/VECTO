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
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer.DriverData
{
	internal class XMLEngineeringLookaheadDataWriterV10 : AbstractXMLWriter, IXMLLookaheadDataWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringLookaheadDataWriterV10() : base("LookAheadCoastingEngineeringType") { }

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var lookahead = inputData as ILookaheadCoastingInputData;
			var ns = ComponentDataNamespace;
			if (lookahead == null) {
				return new object[] { };
			}

			return new object[] {
				new XElement(ns + XMLNames.DriverModel_LookAheadCoasting_Enabled, lookahead.Enabled),
				new XElement(ns + XMLNames.DriverModel_LookAheadCoasting_MinSpeed, lookahead.MinSpeed.AsKmph),
				new XElement(ns + XMLNames.DriverModel_LookAheadCoasting_PreviewDistanceFactor, lookahead.LookaheadDistanceFactor),
				new XElement(
					ns + XMLNames.DriverModel_LookAheadCoasting_DecisionFactorOffset,
					lookahead.CoastingDecisionFactorOffset),
				new XElement(
					ns + XMLNames.DriverModel_LookAheadCoasting_DecisionFactorScaling,
					lookahead.CoastingDecisionFactorScaling),
				lookahead.CoastingDecisionFactorTargetSpeedLookup == null
					? null
					: new XElement(
						ns + XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor,
						Writer.Configuration.SingleFile
							? EmbedDataTable(
								lookahead.CoastingDecisionFactorTargetSpeedLookup,
								AttributeMappings.CoastingDFTargetSpeedLookupMapping)
							: ExtCSVResource(
								lookahead.CoastingDecisionFactorTargetSpeedLookup,
								Path.Combine(
									Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("Driver_LAC_TargetspeedLookup.csv")))),
				lookahead.CoastingDecisionFactorVelocityDropLookup == null
					? null
					: new XElement(
						ns + XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor,
						Writer.Configuration.SingleFile
							? EmbedDataTable(
								lookahead.CoastingDecisionFactorVelocityDropLookup,
								AttributeMappings.CoastingDFVelocityDropLookupMapping)
							: ExtCSVResource(
								lookahead.CoastingDecisionFactorVelocityDropLookup,
								Path.Combine(
									Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("Driver_LAC_VelocityDropLookup.csv"))))
			};
		}


		public override XNamespace ComponentDataNamespace => _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion
	}
}

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
using System.Linq;
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
	internal class XMLEngineeringEngineWriterV10 : AbstractComponentWriter<IEngineEngineeringInputData>,
		IXMLEngineeringEngineWriter
	{
		protected XNamespace _componentNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;


		public XMLEngineeringEngineWriterV10() : base("EngineDataEngineeringType") { }

		#region Overrides of AbstractXMLEngineeringEngineWriter

		protected override object[] DoWriteXML(IEngineEngineeringInputData data)
		{
			var ns = ComponentDataNamespace;
			return new object[] {
				GetXMLTypeAttribute(),
				GetDefaultComponentElements(data),
				new XElement(ns + XMLNames.Engine_Displacement, (data.Displacement.Value() * 1000 * 1000).ToXMLFormat(0)),
				new XElement(ns + XMLNames.Engine_IdlingSpeed, data.EngineModes.First().IdleSpeed.AsRPM.ToXMLFormat(0)),
				data.Inertia != null ? new XElement(ns + XMLNames.Engine_Inertia, data.Inertia.Value()) : null,
				new XElement(ns + XMLNames.Engine_FCCorrection, data.EngineModes.First().Fuels.First().WHTCEngineering.ToXMLFormat(4)),
				new XElement(ns + XMLNames.Engine_FuelType, data.EngineModes.First().Fuels.First().FuelType.ToXMLFormat()),
				new XElement(ns + XMLNames.Engine_FuelConsumptionMap, GetFuelConsumptionMap(ns, data)),
				new XElement(ns + XMLNames.Engine_FullLoadAndDragCurve, GetFullLoadDragCurve(ns, data))
			};
		}

		#endregion

		protected virtual object[] GetFullLoadDragCurve(XNamespace ns, IEngineEngineeringInputData data)
		{
			if (Writer.Configuration.SingleFile) {
				return EmbedDataTable(data.EngineModes.First().FullLoadCurve, AttributeMappings.EngineFullLoadCurveMapping);
			}

			var filename = Path.Combine(
				Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters($"ENG_{data.Model}.vfld"));
			return ExtCSVResource(data.EngineModes.First().FullLoadCurve, filename);
		}

		protected virtual object[] GetFuelConsumptionMap(XNamespace ns, IEngineEngineeringInputData data)
		{
			if (Writer.Configuration.SingleFile) {
				return EmbedDataTable(data.EngineModes.First().Fuels.First().FuelConsumptionMap, AttributeMappings.FuelConsumptionMapMapping);
			}

			var filename = Path.Combine(
				Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters($"ENG_{data.Model}.vmap"));
			return ExtCSVResource(data.EngineModes.First().Fuels.First().FuelConsumptionMap, filename);
		}

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace => _componentNamespace ?? (_componentNamespace = Writer.RegisterNamespace(NAMESPACE_URI));

		#endregion
	}

	internal class XMLEngineeringEngineWriterV10TEST : XMLEngineeringEngineWriterV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		protected override object[] DoWriteXML(IEngineEngineeringInputData data)
		{
			var v10 = Writer.RegisterNamespace(XMLEngineeringEngineWriterV10.NAMESPACE_URI);
			var v11 = ComponentDataNamespace;
			return new object[] {
				GetXMLTypeAttribute(),
				GetDefaultComponentElements(data),
				new XElement(v10 + XMLNames.Engine_Displacement, (data.Displacement.Value() * 1000 * 1000).ToXMLFormat(0)),
				new XElement(v10 + XMLNames.Engine_IdlingSpeed, data.EngineModes.First().IdleSpeed.AsRPM.ToXMLFormat(0)),
				data.Inertia != null ? new XElement(v10 + XMLNames.Engine_Inertia, data.Inertia.Value()) : null,
				new XElement(v10 + XMLNames.Engine_FCCorrection, data.EngineModes.First().Fuels.First().WHTCEngineering.ToXMLFormat(4)),
				new XElement(v10 + XMLNames.Engine_FuelType, data.EngineModes.First().Fuels.First().FuelType.ToXMLFormat()),
				new XElement(v10 + XMLNames.Engine_FuelConsumptionMap, GetFuelConsumptionMap(v10, data)),
				new XElement(v10 + XMLNames.Engine_FullLoadAndDragCurve, GetFullLoadDragCurve(v10, data)),
				new XElement(v11 + XMLNames.Engine_RatedPower, data.RatedPowerDeclared?.Value().ToXMLFormat(0) ?? "xxx kW"),
				new XElement(v11 + XMLNames.Engine_RatedSpeed, data.RatedSpeedDeclared?.AsRPM.ToXMLFormat(0) ?? "yyy rpm")
			};
		}

		public override XNamespace ComponentDataNamespace => _componentNamespace ?? (_componentNamespace = Writer.RegisterNamespace(NAMESPACE_URI));
	}
}

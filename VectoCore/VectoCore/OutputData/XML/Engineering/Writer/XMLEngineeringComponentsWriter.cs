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
using System.Linq;
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
	public interface IXMLEngineeringComponentsWriter : IXMLEngineeringComponentWriter { }

	public class XMLEngineeringComponentsWriterV10 : AbstractComponentWriter<IVehicleEngineeringInputData>,
		IXMLEngineeringComponentsWriter
	{
		private XNamespace _componentDataNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;


		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }


		public XMLEngineeringComponentsWriterV10() : base("VehicleComponentsType") { }

		#region Overrides of AbstractComponentWriter<IVehicleEngineeringInputData>

		protected override object[] DoWriteXML(IVehicleEngineeringInputData inputData)
		{
			var retVal = new List<object>() {
				GetXMLTypeAttribute(),
				CreateComponent(inputData.Components.EngineInputData, XMLNames.Component_Engine),
				CreateComponent(inputData.Components.GearboxInputData, XMLNames.Component_Gearbox),
			};
			var singleFile = Writer.Configuration.SingleFile;
			Writer.Configuration.SingleFile = true;

			if (inputData.Components.AngledriveInputData.Type == AngledriveType.SeparateAngledrive) {
				retVal.Add(CreateComponent(inputData.Components.AngledriveInputData, XMLNames.Component_Angledrive));
			}

			if (inputData.Components.RetarderInputData.Type.IsDedicatedComponent()) {
				retVal.Add(
					CreateComponent(inputData.Components.RetarderInputData, XMLNames.Component_Retarder)
				);
			}

			Writer.Configuration.SingleFile = singleFile;
			retVal.AddRange(new object[] {
				CreateComponent(inputData.Components.AxleGearInputData, XMLNames.Component_Axlegear),
				CreateAxles(inputData),
				CreateAuxiliaries(inputData),
				CreateComponent(inputData.Components.AirdragInputData, XMLNames.Component_AirDrag)
			});
			return retVal.ToArray();
		}

		protected virtual XElement CreateAuxiliaries(IVehicleEngineeringInputData inputData)
		{
			var v10 = ComponentDataNamespace;
			var auxData = inputData.Components.AuxiliaryInputData;
			var componentWriter = Factory.GetWriter(
				auxData, Writer, inputData.DataSource);
			return new XElement(
				v10 + XMLNames.Component_Auxiliaries,
				new XElement(
					v10 + XMLNames.ComponentDataWrapper,
					componentWriter.GetXMLTypeAttribute()
					//auxData.Auxiliaries.Select(a => Factory.GetWriter(a, Writer, a.DataSource).WriteXML(a)).ToArray()
				)
			);
		}

		protected virtual XElement CreateAxles(IVehicleEngineeringInputData inputData)
		{
			var v10 = ComponentDataNamespace;
			var componentWriter = Factory.GetWriter(
				inputData.Components.AxleWheels, Writer, inputData.Components.AxleWheels.DataSource);
			return new XElement(
				v10 + XMLNames.Component_AxleWheels,
				new XElement(
					v10 + XMLNames.ComponentDataWrapper,
					componentWriter.GetXMLTypeAttribute(),
					componentWriter.WriteXML(inputData)
				)
			);
		}


		protected virtual XElement CreateComponent<T>(T componentData, string component) where T : class, IComponentInputData
		{
			if (Writer.Configuration.SingleFile) {
				var v10 = ComponentDataNamespace;
				var componentWriter = Factory.GetWriter(componentData, Writer);
				return new XElement(
					v10 + component,
					new XElement(
						v10 + XMLNames.ComponentDataWrapper,
						componentWriter.WriteXML(componentData)
					)
				);
			}

			var document = Writer.WriteComponent(componentData);
			return ExtComponent(document, component, Writer.GetFilename(componentData));
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

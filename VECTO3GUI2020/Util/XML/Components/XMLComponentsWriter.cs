using System.Xml.Linq;
using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Resources.XML;

namespace VECTO3GUI2020.Util.XML.Components
{


	public abstract class XMLCompletedBusComponentsWriter : IXMLComponentsWriter
	{
		protected IVehicleComponentsDeclaration _inputData;
		protected readonly IXMLWriterFactory _writerFactory;

		public XMLCompletedBusComponentsWriter(IXMLWriterFactory writerFactory)
		{
			_writerFactory = writerFactory;
		}


		#region Implementation of IXMLComponentsWriter

		public XElement GetComponents()
		{
			if (_inputData == null) {
				throw new Exception("Input data not initialized");
			}

			return DoCreateComponents();
		}

		protected abstract XElement DoCreateComponents();

		public IXMLComponentsWriter Init(IVehicleComponentsDeclaration inputData)
		{
			_inputData = inputData;
			return this;
		}

		#endregion
	}

	public class XMLCompletedBusComponentsWriter_xEV : XMLCompletedBusComponentsWriter
	{

		public static (XNamespace ns, string xsdType) VERSION = (
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27,
			XMLTypes.Components_xEV_CompletedBusType);



		public XMLCompletedBusComponentsWriter_xEV(IXMLWriterFactory writerFactory) 
			: base(writerFactory) { }

		#region Overrides of XMLCompletedBusComponentsWriter

		protected override XElement DoCreateComponents()
		{
			var componentElement = new XElement(VERSION.ns + XMLNames.Vehicle_Components,
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, VERSION.xsdType));


			if (_inputData.AirdragInputData != null)
			{
				var airDragElement = _writerFactory.CreateComponentWriter(_inputData.AirdragInputData).GetElement();

				var tempAirDragElement = new XElement(VERSION.ns + XMLNames.Component_AirDrag);

				airDragElement.Name = tempAirDragElement.Name;
				componentElement.Add(airDragElement);
			}

			if (_inputData.BusAuxiliaries != null)
			{
				var busAuxElement = _writerFactory.CreateComponentWriter(_inputData.BusAuxiliaries).GetElement();


				componentElement.Add(busAuxElement);
			}


			return componentElement;
        }

		#endregion
	}

	public class XMLCompletedBusComponentWriter_Conventional : XMLCompletedBusComponentsWriter
	{
		public static (XNamespace ns, string xsdType) VERSION = (
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27, XMLTypes.Components_Conventional_CompletedBusType);

		public XMLCompletedBusComponentWriter_Conventional(IXMLWriterFactory writerFactory) : base(writerFactory) { }

		#region Overrides of XMLCompletedBusComponentsWriter

		protected override XElement DoCreateComponents()
		{
			
			var componentElement = new XElement(VERSION.ns + XMLNames.Vehicle_Components,
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, VERSION.xsdType));
			

			if (_inputData.AirdragInputData != null) {
				var airDragElement = _writerFactory.CreateComponentWriter(_inputData.AirdragInputData).GetElement();
				
				var tempAirDragElement = new XElement(VERSION.ns + XMLNames.Component_AirDrag);

				airDragElement.Name = tempAirDragElement.Name;
				componentElement.Add(airDragElement);
			}

			if (_inputData.BusAuxiliaries != null) {
				var busAuxElement = _writerFactory.CreateComponentWriter(_inputData.BusAuxiliaries).GetElement();
				

				componentElement.Add(busAuxElement);
			}


			return componentElement;

        }

		#endregion
	}
}

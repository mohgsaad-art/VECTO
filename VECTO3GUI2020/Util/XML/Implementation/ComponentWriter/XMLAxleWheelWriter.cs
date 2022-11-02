using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
    public abstract class XMLAxleWheelWriter : IXMLComponentWriter
    {
		protected IXMLWriterFactory _writerFactory;
		protected IAxleDeclarationInputData _inputData;

		protected XNamespace _defaultNamespace;

		protected XElement _xElement;
		public XMLAxleWheelWriter(IAxleDeclarationInputData inputData, IXMLWriterFactory writerFactory)
		{
			_inputData = inputData;
			_writerFactory = writerFactory;
		}

		public XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateElement();
			}

			return _xElement;
		}

		protected abstract void CreateElement();
		protected abstract void Initialize();
	}


	public class XMLAxleWheelWriter_v2_0 : XMLAxleWheelWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(AxleViewModel_v2_0).ToString()
		};
		public XMLAxleWheelWriter_v2_0(IAxleDeclarationInputData inputData, IXMLWriterFactory writerFactory) : base(inputData, writerFactory) { }

		protected override void CreateElement()
		{
			_xElement.Add(new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, 0));//_inputData.AxleNumber));
			_xElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.Axle_Type_Attr_AxleDataDeclarationType));
			_xElement.Add(new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_AxleType, _inputData.AxleType));
			_xElement.Add(new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_TwinTyres, _inputData.TwinTyres));
			_xElement.Add(new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_Steered, false)); //_inputData.Steered));

			_xElement.Add(_writerFactory.CreateComponentWriter(_inputData.Tyre).GetElement());
		}

		protected override void Initialize()
		{

			_defaultNamespace = XML.XMLNamespaces.V20;
			_xElement = new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle);
		}
	}
}

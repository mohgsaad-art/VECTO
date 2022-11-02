using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
    public abstract class XMLAxleWheelsWriter : IXMLComponentWriter
	{
		protected XElement _xElement;
		protected XNamespace _defaultNameSpace;
		protected IAxlesDeclarationInputData _inputData;
		protected IXMLWriterFactory _writerFactory;

		protected XMLAxleWheelsWriter(IAxlesDeclarationInputData inputData, IXMLWriterFactory writerFactory)
		{
			_inputData = inputData;
			_writerFactory = writerFactory;
		}

		public XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateDataElements();
			}

			return _xElement;
		}

		public abstract void CreateDataElements();
		public abstract void Initialize();
	}


	public class XMLAxleWheelsWriter_v2_0 : XMLAxleWheelsWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(AxleWheelsViewModel_v2_0).ToString()
		};
		public override void CreateDataElements()
		{
			_xElement.Add(new XElement(_defaultNameSpace + XMLNames.ComponentDataWrapper));
			var dataElement = _xElement.LastNode as XElement;

			dataElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.AxleWheels_Type_Attr_AxleWheelsDeclarationType));

			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.AxleWheels_Axles));
			var axles = dataElement.LastNode as XElement;
			foreach (var axle in _inputData.AxlesDeclaration) {
				axles.Add(_writerFactory.CreateComponentWriter(axle).GetElement());
			}
		}

		public override void Initialize()
		{
			_defaultNameSpace = XML.XMLNamespaces.V20;
			_xElement = new XElement(_defaultNameSpace + XMLNames.Component_AxleWheels);
		}

		public XMLAxleWheelsWriter_v2_0(IAxlesDeclarationInputData inputData, IXMLWriterFactory writerFactory) : base(inputData, writerFactory) { }
	}
}

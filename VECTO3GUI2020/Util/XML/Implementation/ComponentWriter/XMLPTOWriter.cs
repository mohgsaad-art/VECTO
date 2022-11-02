using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
	public abstract class XMLPTOWriter : IXMLComponentWriter
	{
		protected XElement _xElement;
		protected XNamespace _defaultNamespace;
		private IPTOTransmissionInputData _inputData;

		public XMLPTOWriter(IPTOTransmissionInputData inputData)
		{
			_inputData = inputData;
			Initialize();
			CreateElements();
		}



		public XElement GetElement()
		{
			return _xElement;
		}

		protected abstract void Initialize();
		protected abstract void CreateElements();
	}

	public class XMLPTOWriter_v1_0 : XMLPTOWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(PTOViewModel_V1_0).ToString()
		};

		public XMLPTOWriter_v1_0(IPTOTransmissionInputData inputData) : base(inputData) { }

		protected override void Initialize()
		{
			
			throw new NotImplementedException();
		}

		protected override void CreateElements()
		{
			throw new NotImplementedException();
		}
	}




	public class XMLPTOWriter_v2_0 : XMLPTOWriter_v1_0
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(PTOViewModel_V2_0).ToString()
		};

		public XMLPTOWriter_v2_0(IPTOTransmissionInputData inputData) : base(inputData) { }

		protected override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V20;
			_xElement = new XElement(_defaultNamespace + XMLNames.Vehicle_PTO);
			_xElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, XMLNames.Component_Type_Attr_PTO));
		}

		protected override void CreateElements()
		{
			_xElement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_PTO_ShaftsGearWheels, "none"));
			_xElement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_PTO_OtherElements, "none"));
		}
	}

	
}

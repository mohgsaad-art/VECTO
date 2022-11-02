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
    public abstract class XMLAuxiliariesWriter : IXMLComponentWriter
	{
		protected XNamespace _defaultNamespace;
		protected XElement _xElement;
		protected IAuxiliariesDeclarationInputData _inputData;

		public XMLAuxiliariesWriter(IAuxiliariesDeclarationInputData inputData)
		{
			_inputData = inputData;
		}

		public XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateElements();
			}

			return _xElement;
		}
		public abstract void Initialize();

		public abstract void CreateElements();
	}

	public class XMLAuxiliariesWriter_v2_0 : XMLAuxiliariesWriter
	{

		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(AuxiliariesViewModel_v2_0).ToString()
		};

		public XMLAuxiliariesWriter_v2_0(IAuxiliariesDeclarationInputData inputData) : base(inputData) { }
		
		public override void CreateElements()
		{
			var dataElement = new XElement(_defaultNamespace + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);

			dataElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.Auxiliaries_Type_Attr_DataDeclarationType));

			foreach (var auxiliary in _inputData.Auxiliaries) {
				var auxElement = new XElement(_defaultNamespace + auxiliary.Type.ToString());
				auxElement.Add(new XElement(_defaultNamespace + XMLNames.Auxiliaries_Auxiliary_Technology,
					auxiliary.Technology[0]));
				dataElement.Add(auxElement);
			}
		}

		public override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V20;
			_xElement = new XElement(_defaultNamespace + XMLNames.Component_Auxiliaries);
		}
	}
}

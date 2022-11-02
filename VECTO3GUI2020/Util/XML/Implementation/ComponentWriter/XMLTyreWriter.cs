using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
    public abstract class XMLTyreWriter : IXMLComponentWriter
    {
		protected ITyreDeclarationInputData _inputData;

		protected XNamespace _defaultNamespace;
		protected XElement _xElement;
		protected string _uri = "ToDo-Add-Id";

		public XMLTyreWriter(ITyreDeclarationInputData inputData)
		{
			_inputData = inputData;
		}

		public XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateDataElements();
				_xElement.Add(this.CreateSignatureElement(_defaultNamespace, _uri, _inputData.DigestValue));
			}

			return _xElement;
		}
		protected abstract void Initialize();
		protected abstract void CreateDataElements();





	}

	public class XMLTyreWriter_v2_0 : XMLTyreWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(TyreViewModel_v2_0).ToString()
		};
	
		public XMLTyreWriter_v2_0(ITyreDeclarationInputData inputData) : base(inputData) { }
		protected override void CreateDataElements()
		{
			var dataElement = new XElement(_defaultNamespace + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);

			dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, _uri));
			dataElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.Tyre_Type_Attr_TyreDataDeclarationType));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Date, _inputData.Date));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_Dimension, _inputData.Dimension));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_RRCDeclared, _inputData.RollResistanceCoefficient.ToXMLFormat(4)));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_FzISO, _inputData.TyreTestLoad.ToXMLFormat(0)));
		}

		protected override void Initialize()
		{
			_defaultNamespace = XML.XMLNamespaces.V20;
			_xElement = new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_Tyre);
		}

	}

	public class XMLTyreWriter_v2_3 : XMLTyreWriter_v2_0
	{
		public new static readonly string[] SUPPORTED_VERSIONS = {
			typeof(TyreViewModel_v2_3).ToString()
		};
		public XMLTyreWriter_v2_3(ITyreDeclarationInputData inputData) : base(inputData) { }

		protected override void CreateDataElements()
		{
			base.CreateDataElements();
			
		}

		protected override void Initialize()
		{
			_defaultNamespace = XML.XMLNamespaces.V23;
			_xElement = new XElement(_defaultNamespace + XMLNames.AxleWheels_Axles_Axle_Tyre);
		}
	}

}

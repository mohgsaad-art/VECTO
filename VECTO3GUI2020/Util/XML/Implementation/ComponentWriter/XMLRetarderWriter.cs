using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
	public abstract class XMLRetarderWriter : IXMLComponentWriter
	{
		protected XElement _xElement;
		protected IRetarderInputData _inputData;
		protected XNamespace _defaultNameSpace;
		protected string _uri = "toTo-Add-URI";

		public XMLRetarderWriter(IRetarderInputData inputData)
		{
			_inputData = inputData;
		}

		protected abstract void Initialize();
		protected abstract void CreateDataElements();

		public XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateDataElements();
				_xElement.Add(this.CreateSignatureElement(_defaultNameSpace, _uri, _inputData.DigestValue));
			}
			return _xElement;
		}
	}

	public class XMLRetarderWriter_v1_0 : XMLRetarderWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(RetarderViewModel_v1_0).ToString()
		};

		protected override void Initialize()
		{
			_defaultNameSpace = XML.XMLNamespaces.V10;
			_xElement = new XElement(_defaultNameSpace + XMLNames.Component_Retarder);
		}

		protected override void CreateDataElements()
		{
			var dataElement = new XElement(_defaultNameSpace + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);

			dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, _uri));
			dataElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, XMLNames.Retarder_Attr_DataDeclarationType));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Model, _inputData.Model));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Date, _inputData.Date));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_CertificationMethod, _inputData.CertificationMethod.ToXMLFormat()));

			var lossMapElement = new XElement(_defaultNameSpace + XMLNames.Retarder_RetarderLossMap);
			dataElement.Add(lossMapElement);

			foreach (DataRow lossMapRow in _inputData.LossMap.Rows)
			{
				var entryElement = new XElement(_defaultNameSpace + XMLNames.Retarder_RetarderLossMap_Entry);
				entryElement.Add(new XAttribute(XMLNames.Retarder_RetarderLossmap_RetarderSpeed_Attr, lossMapRow[0].ToString()));
				entryElement.Add(new XAttribute(XMLNames.Retarder_RetarderLossmap_TorqueLoss_Attr, lossMapRow[1].ToString()));
				lossMapElement.Add(entryElement);
			}
		}

		public XMLRetarderWriter_v1_0(IRetarderInputData inputData) : base(inputData) { }
	}



	public class XMLRetarderWriter_v2_0 : XMLRetarderWriter_v1_0
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(RetarderViewModel_v2_0).ToString()
		};


		public XMLRetarderWriter_v2_0(IRetarderInputData inputData) : base(inputData)
		{
			
		}



		protected override void Initialize()
		{
			_defaultNameSpace = XML.XMLNamespaces.V20;
			_xElement = new XElement(_defaultNameSpace + XMLNames.Component_Retarder);
		}
	}
}

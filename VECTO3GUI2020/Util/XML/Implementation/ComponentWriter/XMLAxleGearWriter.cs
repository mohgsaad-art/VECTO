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
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
	public abstract class XMLAxleGearWriter : IXMLComponentWriter
	{
		protected XElement _xElement;
		protected IAxleGearInputData _inputData;
		protected XNamespace _defaultNamespace;
		protected string _uri = "ToDO-ADD_URI";

		public XMLAxleGearWriter(IAxleGearInputData inputData)
		{
			_inputData = inputData;

			
		}

		public abstract void Initialize();
		public abstract void CreateDataElements();

		public XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateDataElements();
				_xElement.Add(this.CreateSignatureElement(_defaultNamespace, _uri, _inputData.DigestValue));
			}
			return _xElement;
		}
	}

	public class XMLAxleGearWriter_v2_0 : XMLAxleGearWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(AxleGearViewModel_v2_0).ToString()
		};
		

		public XMLAxleGearWriter_v2_0(IAxleGearInputData inputData) : base(inputData)
		{
						
		}


		public override void Initialize()
		{
			_defaultNamespace = XML.XMLNamespaces.V20;
			_xElement = new XElement(_defaultNamespace + XMLNames.Component_Axlegear);
		}

		public override void CreateDataElements()
		{
			var dataElement = new XElement(_defaultNamespace + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);

			dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, _uri));
			dataElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.AxleGear_Type_Attr));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Date, _inputData.Date));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Axlegear_LineType, _inputData.LineType.ToXMLFormat()));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.AngleDrive_Ratio, _inputData.Ratio.ToXMLFormat(3)));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_CertificationMethod, _inputData.CertificationMethod.ToXMLFormat()));

			var torqueLossMap = new XElement(_defaultNamespace + XMLNames.Axlegear_TorqueLossMap);
			dataElement.Add(torqueLossMap);
			foreach (DataRow row in _inputData.LossMap.Rows) {
				var entry = new XElement(_defaultNamespace + XMLNames.Axlegear_TorqueLossMap_Entry);
				entry.Add(new XAttribute(XMLNames.TransmissionLossmap_InputSpeed_Attr, row[0].ToString()));
				entry.Add(new XAttribute(XMLNames.TransmissionLossmap_InputTorque_Attr, row[1].ToString()));
				entry.Add(new XAttribute(XMLNames.TransmissionLossmap_TorqueLoss_Attr, row[2].ToString()));
				torqueLossMap.Add(entry);
			}
		}
	}
}

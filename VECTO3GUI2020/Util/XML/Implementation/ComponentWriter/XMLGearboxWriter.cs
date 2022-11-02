using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    public abstract class XMLGearboxWriter : IXMLComponentWriter
    {
		protected IGearboxDeclarationInputData _inputData;
		protected XElement _xElement;
		protected XNamespace _defaultNameSpace;
		protected string _uri = "toDo-Adduri";

		public XMLGearboxWriter(IGearboxDeclarationInputData inputData)
		{

			_inputData = inputData;
		}

		public virtual XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateDataElements();
				_xElement.Add(this.CreateSignatureElement(_defaultNameSpace, _uri, _inputData.DigestValue));
			}
			return _xElement;
		}

		protected abstract void Initialize();
		protected abstract void CreateDataElements();
	}
	public class XMLGearboxWriter_v2_0 : XMLGearboxWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(GearboxViewModel_v2_0).ToString()
		};

		public XMLGearboxWriter_v2_0(IGearboxDeclarationInputData inputData) : base(inputData)
		{
		}

		protected override void Initialize()
		{
			_defaultNameSpace = XML.XMLNamespaces.V20;
			_xElement = new XElement(_defaultNameSpace + XMLNames.Component_Gearbox);
		}

		protected override void CreateDataElements()
		{
			var dataElement = new XElement(_defaultNameSpace + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);
			dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, _uri));
			dataElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.Gearbox_attr_GearboxDataDeclarationType));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Model, _inputData.Model));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Date, _inputData.Date));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Gearbox_TransmissionType, _inputData.Type.ToXMLFormat()));
			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Gearbox_CertificationMethod, _inputData.CertificationMethod.ToXMLFormat()));

			dataElement.Add(new XElement(_defaultNameSpace + XMLNames.Gearbox_Gears));
			var gearsElement = dataElement.LastNode as XElement;
			gearsElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, XMLNames.Gearbox_Gears_Attr_GearsDeclarationType));
			foreach (var gear in _inputData.Gears) {
				gearsElement.Add(new XElement(_defaultNameSpace + XMLNames.Gearbox_Gears_Gear));
				var gearElement = gearsElement.LastNode as XElement;
				//var gearElement = new XElement(_defaultNameSpace + XMLNames.Gearbox_Gears_Gear);
				//gearsElement.Add(gearElement);
				gearElement.Add(new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, gear.Gear));
				gearElement.Add(new XElement(_defaultNameSpace + XMLNames.Gearbox_Gear_Ratio, gear.Ratio.ToXMLFormat(3)));

				if (gear.MaxTorque != null) {
					gearElement.Add(new XElement(_defaultNameSpace + XMLNames.Gearbox_Gears_MaxTorque, gear.MaxTorque));
				}

				if (gear.MaxInputSpeed != null) {
					gearElement.Add(new XElement(_defaultNameSpace + XMLNames.Gearbox_Gear_MaxSpeed, gear.MaxInputSpeed));
				}

				var torqueLossMapElement = new XElement(_defaultNameSpace + XMLNames.Gearbox_Gear_TorqueLossMap);
				gearElement.Add(torqueLossMapElement);
				foreach (DataRow torquelossEntry in gear.LossMap.Rows) {
					var entry = new XElement(_defaultNameSpace + XMLNames.Gearbox_Gear_TorqueLossMap_Entry);
					torqueLossMapElement.Add(entry);
					entry.Add(new XAttribute(XMLNames.TransmissionLossmap_InputSpeed_Attr, torquelossEntry[0].ToString()));
					entry.Add(new XAttribute(XMLNames.TransmissionLossmap_InputTorque_Attr, torquelossEntry[1].ToString()));
					entry.Add(new XAttribute(XMLNames.TransmissionLossmap_TorqueLoss_Attr, torquelossEntry[2].ToString()));
					
				}
			}
			Debug.WriteLine(_xElement.ToString());
		}

		
	}
}

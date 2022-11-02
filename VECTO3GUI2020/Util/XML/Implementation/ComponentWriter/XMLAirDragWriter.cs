using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
    public abstract class XMLAirDragWriter : IXMLComponentWriter
    {
		protected IAirdragDeclarationInputData _inputData;
		protected XNamespace _defaultNamespace;
		protected XElement _xElement;
		protected string _uri = "ToDO-Add-id";

		public XMLAirDragWriter(IAirdragDeclarationInputData inputData)
		{
			_inputData = inputData;
			_uri = inputData.DigestValue?.Reference?.Replace("#","");
			if (_uri == null) {
				_uri = "AirdragComponent" + Guid.NewGuid().ToString("n").Substring(0, 20);
			}
		}

		public XElement GetElement()
		{
			if (_xElement == null) {

				Initialize();
				CreateDataElements();
				var signatureElemnet = this.CreateSignatureElement(_defaultNamespace, _uri, _inputData.DigestValue);
				_xElement.Add(signatureElemnet);
			}

			return _xElement;
		}

		public XElement GetElement(XNamespace wrapperNamespace)
		{
			throw new NotImplementedException();
		}

		protected abstract void Initialize();
		protected abstract void CreateDataElements();


	}

	public class XMLAirDragWriter_v2_0 : XMLAirDragWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(AirDragViewModel_v2_0).ToString(),
			typeof(MultistageAirdragViewModel).ToString()
		};
		public XMLAirDragWriter_v2_0(IAirdragDeclarationInputData inputData) : base(inputData) { }
		protected override void CreateDataElements()
		{
			var dataElement = new XElement(_defaultNamespace + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);

			dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, _uri), new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, "v2.0:" +
			 XMLNames.AirDrag_Data_Type_Attr));

			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_Date, _inputData.Date));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.AirDrag_CdxA_0, _inputData.AirDragArea_0.ToXMLFormat(2)));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.AirDrag_TransferredCDxA, _inputData.TransferredAirDragArea.ToXMLFormat(2)));
			dataElement.Add(new XElement(_defaultNamespace + XMLNames.AirDrag_DeclaredCdxA, _inputData.AirDragArea.ToXMLFormat(2)));


			dataElement.DescendantsAndSelf().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
		}

		protected override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V20;
			_xElement = new XElement(XMLNames.Component_AirDrag);
		}
	}
}

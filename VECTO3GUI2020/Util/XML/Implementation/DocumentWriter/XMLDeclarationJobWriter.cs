using System;
using System.Diagnostics;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit;

namespace VECTO3GUI2020.Util.XML.Implementation.DocumentWriter
{
    public abstract class XMLDeclarationJobWriter : IXMLDeclarationJobWriter
    {

		protected XDocument _xDocument;
		protected string _schemaVersion;
		private IXMLWriterFactory _xmlWriterFactory;

		protected string LocalSchemaLocation = @"V:\VectoCore\VectoCore\Resources\XSD\";

		//Must be overwritten by subclasses;
		protected abstract void Initialize();



		public XMLDeclarationJobWriter(IDeclarationJobInputData inputData, IXMLWriterFactory xmlWriterFactory)
		{
			_xmlWriterFactory = xmlWriterFactory;
			Initialize();

			(_xDocument.FirstNode as XElement)?.Add(_xmlWriterFactory.CreateVehicleWriter(inputData.Vehicle).GetElement());
			var rootElement = _xDocument.FirstNode as XElement;
			
		}





        public XDocument GetDocument()
        {
            return _xDocument;
        }
    }


    public class XMLDeclarationJobWriter_v1_0 : XMLDeclarationJobWriter
	{
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(XMLDeclarationJobInputDataProviderV10).ToString()
		};
		public XMLDeclarationJobWriter_v1_0(IDeclarationJobInputData inputData, IXMLWriterFactory xmlWriterFactory) : base(inputData, xmlWriterFactory)
        {
            
        }
		protected override void Initialize()
		{
			throw new NotImplementedException();
		}


    }

    public class XMLDeclarationJobWriter_v2_0 : XMLDeclarationJobWriter
	{

		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(DeclarationJobEditViewModel_v2_0).ToString(),
            typeof(XMLDeclarationJobInputDataProviderV20).ToString()
		};

		public XMLDeclarationJobWriter_v2_0(IDeclarationJobInputData inputData, IXMLWriterFactory xmlWriterFactory) : base(inputData, xmlWriterFactory)
        {

        }

        protected override void Initialize()
		{
			_schemaVersion = "2.0";
			_xDocument = new XDocument();

			var xElement = new XElement(XMLNamespaces.Tns_v20 + XMLNames.VectoInputDeclaration);
			_xDocument.Add(xElement);

			xElement.Add(new XAttribute("schemaVersion", _schemaVersion));
			xElement.Add(new XAttribute("xmlns", XMLNamespaces.DeclarationDefinition + ":v" + _schemaVersion));
			xElement.Add(new XAttribute(XNamespace.Xmlns + "xsi", XMLNamespaces.Xsi.NamespaceName));
			xElement.Add(new XAttribute(XNamespace.Xmlns + "tns", XMLNamespaces.Tns_v20));
			xElement.Add(new XAttribute(XMLNamespaces.Xsi + "schemaLocation",
				$"{XMLNamespaces.DeclarationRootNamespace} {LocalSchemaLocation}VectoDeclarationJob.xsd"));

			Debug.WriteLine(_xDocument.ToString());
		}
    }

}

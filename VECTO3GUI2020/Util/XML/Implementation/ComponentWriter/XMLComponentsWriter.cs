using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using EnvironmentVariableTarget = System.EnvironmentVariableTarget;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
    public abstract class XMLComponentsWriter : IXMLComponentsWriter
    {
		protected IXMLWriterFactory _xMLWriterFactory;

		protected XElement _xElement;

		protected static string _name = "Components";
		protected static readonly string _declarationDefinition = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions";
		protected static readonly XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected static readonly XNamespace _v10 = _declarationDefinition + ":v1.0";
		protected static readonly XNamespace _v20 = _declarationDefinition + ":v2.0";
		protected XNamespace _defaultNamespace;
		protected IVehicleComponentsDeclaration _inputData;


		public XMLComponentsWriter(IVehicleComponentsDeclaration inputData, IXMLWriterFactory xMLWriterFactory)
		{
			_inputData = inputData;
			_xMLWriterFactory = xMLWriterFactory;
			Initialize();
			CreateComponents();
		}


		protected abstract void CreateComponents();
		public abstract XElement GetComponents();
		public abstract void Initialize();


	}

	public class XMLComponentsWriter_v1_0 : XMLComponentsWriter
	{

		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(ComponentsViewModel_v1_0).ToString()
		};
		public XMLComponentsWriter_v1_0(IVehicleComponentsDeclaration inputData, IXMLWriterFactory xMLWriterFactory) :
			base(inputData, xMLWriterFactory)
		{
			
		}

		protected override void CreateComponents()
		{
			throw new NotImplementedException();
		}

		public override XElement GetComponents()
		{
			throw new NotImplementedException();
		}

		public override void Initialize()
		{
			_defaultNamespace = _v10;
		}
	}


	public class XMLComponentsWriter_v2_0 : XMLComponentsWriter_v1_0
	{
		public new static readonly string[] SUPPORTED_VERSIONS = {
			typeof(ComponentsViewModel_v2_0).ToString()
		};

		public XMLComponentsWriter_v2_0(IVehicleComponentsDeclaration inputData, IXMLWriterFactory xMLWriterFactory) :
			base(inputData, xMLWriterFactory)
		{

		}


		protected override void CreateComponents()
		{
			Debug.Assert(_inputData is IComponentsViewModel);
			IXMLComponentWriter writer;
			if (_inputData.EngineInputData != null) {
				writer = _xMLWriterFactory.CreateComponentWriter(_inputData.EngineInputData);
				_xElement.Add(writer.GetElement());
			}

			if (_inputData.GearboxInputData != null) {
				writer = _xMLWriterFactory.CreateComponentWriter(_inputData.GearboxInputData);
				_xElement.Add(writer.GetElement());
			}

			if (_inputData.RetarderInputData != null) {
				writer = _xMLWriterFactory.CreateComponentWriter(_inputData.RetarderInputData);
				_xElement.Add(writer.GetElement());
			}

			if (_inputData.AxleGearInputData != null) {
				writer = _xMLWriterFactory.CreateComponentWriter(_inputData.AxleGearInputData);
				_xElement.Add(writer.GetElement());
			}

			if (_inputData.AxleWheels != null) {
				writer = _xMLWriterFactory.CreateComponentWriter(_inputData.AxleWheels);
				_xElement.Add(writer.GetElement());
			}

			if (_inputData.AuxiliaryInputData != null) {
				writer = _xMLWriterFactory.CreateComponentWriter(_inputData.AuxiliaryInputData);
				_xElement.Add(writer.GetElement());
			}

			if (_inputData.AirdragInputData != null) {
				writer = _xMLWriterFactory.CreateComponentWriter(_inputData.AirdragInputData);
				_xElement.Add(writer.GetElement());
			}
		}

		public override XElement GetComponents()
		{
			return _xElement;
		}

		public override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V20;
			_xElement = new XElement(_defaultNamespace + XMLNames.Vehicle_Components);
			_xElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, XMLNames.Components_type_attr));
		}
	}
}

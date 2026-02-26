using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.Util.XML.Vehicle;
using Vecto3GUI2020Test.MockInput;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.XML.XMLWritingTest;

[TestFixture]
public class XMLCompletedBusVehicleWriterTest
{
	private MockDialogHelper _mockDialogHelper;
	private IKernel _kernel;
	private IXMLWriterFactory _xmlWriterFactory;


	

    [OneTimeSetUp()]
	public void Setup()
	{
		_kernel = TestHelper.GetKernel(out _mockDialogHelper, out _);
		_xmlWriterFactory = _kernel.Get<IXMLWriterFactory>();
		_xmlInputDataReader = _kernel.Get<IXMLInputDataReader>();
	}

	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE, typeof(XMLCompletedBusVehicleWriterConventional))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE, typeof(XMLCompletedBusVehicleWriterExempted))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE, typeof(XMLCompletedBusVehicleWriterHEV))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE, typeof(XMLCompletedBusVehicleWriterPEV))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE, typeof(XMLCompletedBusVehicleWriterIEPC))]
    public void WriteVehicle(string version, string type, Type expectedWriter)
	{

		var vehicleData = MockVehicle.GetMockVehicle(type, version);
		var writer = _xmlWriterFactory.CreateVehicleWriter(vehicleData);
		Assert.NotNull(writer);
		Assert.AreEqual(expectedWriter, writer.GetType());

		var xElement = writer.GetElement();





		var doc = CreateDocumentWithVehicle(xElement);
		WriteDocument(doc, out _);

    }
	protected string LocalSchemaLocation = @"V:\VectoCore\VectoCore\Resources\XSD\";
	private IXMLInputDataReader _xmlInputDataReader;

	public XDocument CreateDocumentWithVehicle(XElement vehicleElement)
	{

		var schemaVersion = "2.0";
		var xDocument = new XDocument();

		var xElement = new XElement(XMLNamespaces.Tns_v20 + XMLNames.VectoInputDeclaration);
		xDocument.Add(xElement);

		xElement.Add(new XAttribute("schemaVersion", schemaVersion));
		xElement.Add(new XAttribute(XNamespace.Xmlns + "xsi", XMLNamespaces.Xsi.NamespaceName));
		xElement.Add(new XAttribute(XNamespace.Xmlns + "tns", XMLNamespaces.Tns_v20));
		xElement.Add(new XAttribute(XNamespace.Xmlns + "v2.0", XMLNamespaces.V20));
		xElement.Add(new XAttribute(XMLNamespaces.Xsi + "schemaLocation",
			$"{XMLNamespaces.DeclarationRootNamespace} {LocalSchemaLocation}VectoDeclarationJob.xsd"));
		xElement.Add(vehicleElement);
		return xDocument;
	}


	public static List<string> GetXMLJobs(params string[] path)
	{
		List<string> files = new List<string>();
		foreach (var pathItem in path) {
			files.AddRange(GetFiles(pathItem, "*.xml"));
		}

		return files;
	}

	public static List<string> GetFiles(string path, string searchPattern)
	{
		var dirPath = Path.GetFullPath(path);
		List<string> vectoJobs = new List<string>();
		foreach (var fileName in Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.AllDirectories))
		{
			vectoJobs.Add(fileName);
		}
		return vectoJobs;
	}



	[TestCaseSource(nameof(GetDistributedCompletedVehicleFiles))]
	[TestCaseSource(nameof(GetCompletedBusDistributedSchema24))]
	[TestCase("TestData\\XML\\SchemaVersion2.4\\Distributed\\CompletedBus\\Conventional_completedBus_1.xml", TestName="Conventional")]
    public void LoadAndWriteFiles(string file)
	{
		var stepInput = _xmlInputDataReader.CreateDeclaration(file);
		Assert.NotNull(stepInput);

		var writer = _xmlWriterFactory.CreateVehicleWriter(stepInput.JobInputData.Vehicle);
		Assert.NotNull(writer);
		

		var document = CreateDocumentWithVehicle(writer.GetElement());
		WriteDocument(document, out var result);

		IDeclarationInputDataProvider writtenInput;
		using (var tr = new StringReader(result)) {
			using (var xmlReader = new XmlTextReader(tr))
				writtenInput = _xmlInputDataReader.CreateDeclaration(xmlReader);
		}
		

		Compare(stepInput, writtenInput);
    }

	public void Compare(IDeclarationInputDataProvider a, IDeclarationInputDataProvider b)
	{
		var vehA = a.JobInputData.Vehicle;
		var vehB = b.JobInputData.Vehicle;
		Assert.AreEqual(vehA.Manufacturer, vehB.Manufacturer);
		Assert.AreEqual(vehA.ManufacturerAddress, vehB.ManufacturerAddress);
		//TODO: add all relevant properties


		Assert.AreEqual(vehA.Components?.AirdragInputData?.DigestValue.DigestValue, vehB.Components?.AirdragInputData?.DigestValue.DigestValue);


		CompareBusAux(vehA.Components?.BusAuxiliaries, vehB.Components?.BusAuxiliaries);
	}

	private void CompareBusAux(IBusAuxiliariesDeclarationData busAuxA, IBusAuxiliariesDeclarationData busAuxB)
	{
		if (busAuxA == null && busAuxB == null) {
			return;
		}

		Assert.NotNull(busAuxB);
		Assert.NotNull(busAuxA);

		CompareHVAC(busAuxA?.HVACAux, busAuxB?.HVACAux);
		CompareElectricConsumers( busAuxA?.ElectricConsumers, busAuxB?.ElectricConsumers);
	}

	private static void CompareElectricConsumers(IElectricConsumersDeclarationData consumerA, IElectricConsumersDeclarationData consumerB)
	{
		if (consumerA == null && consumerB == null) {
			return;
		}
		Assert.AreEqual(consumerA.BrakelightsLED, consumerB.BrakelightsLED);
		Assert.AreEqual(consumerA.DayrunninglightsLED, consumerB.DayrunninglightsLED);
		Assert.AreEqual(consumerA.HeadlightsLED, consumerB.HeadlightsLED);
		Assert.AreEqual(consumerA.InteriorLightsLED, consumerB.InteriorLightsLED);
		Assert.AreEqual(consumerA.PositionlightsLED, consumerB.PositionlightsLED);
	}

	private static void CompareHVAC(IHVACBusAuxiliariesDeclarationData hvacA, IHVACBusAuxiliariesDeclarationData hvacB)
	{
		if (hvacA == null && hvacB == null) {
			return;
		}

		Assert.AreEqual(hvacA.SystemConfiguration, hvacB.SystemConfiguration);
		Assert.AreEqual(hvacA.HeatPumpTypeCoolingDriverCompartment, hvacB.HeatPumpTypeCoolingDriverCompartment);
		Assert.AreEqual(hvacA.HeatPumpTypeHeatingDriverCompartment, hvacB.HeatPumpTypeHeatingDriverCompartment);
		Assert.AreEqual(hvacA.HeatPumpTypeCoolingPassengerCompartment, hvacB.HeatPumpTypeCoolingPassengerCompartment);
		Assert.AreEqual(hvacA.HeatPumpTypeHeatingPassengerCompartment, hvacB.HeatPumpTypeHeatingPassengerCompartment);
		Assert.AreEqual(hvacA.AuxHeaterPower, hvacB.AuxHeaterPower);
		Assert.AreEqual(hvacA.DoubleGlazing, hvacB.DoubleGlazing);
		Assert.AreEqual(hvacA.AdjustableAuxiliaryHeater, hvacB.AdjustableAuxiliaryHeater);
		Assert.AreEqual(hvacA.SeparateAirDistributionDucts, hvacB.SeparateAirDistributionDucts);
    }


	public static void WriteDocument(XDocument document, out string writtenXml)
	{
		using (var memoryWriter = new MemoryStream())
		{
			using (var xmlWriter = new XmlTextWriter(memoryWriter, Encoding.UTF8))
			{
				xmlWriter.Formatting = Formatting.Indented;
				xmlWriter.Namespaces = true;
				document.WriteTo(xmlWriter);

				xmlWriter.Flush();

				memoryWriter.Seek(0, SeekOrigin.Begin);
				using (var streamReader = new StreamReader(memoryWriter)) {
					writtenXml = streamReader.ReadToEnd();
					TestContext.WriteLine(writtenXml);
				}

				xmlWriter.Close();

			}

		}
    }


	public static string[] GetDistributedCompletedVehicleFiles()
	{
		var _completedBusDistributed = "TestData\\XML\\SchemaVersion2.4\\Distributed\\CompletedBus";
		return GetFiles(Path.GetFullPath(_completedBusDistributed), "*.xml").ToArray();
	}
	[DebuggerStepThrough]
	public static string[] GetCompletedBusDistributedSchema24()
	{
		var _completedBusDistributed = "TestData\\XML\\SchemaVersion2.4";
		var inputDataReader = TestHelper.GetKernel().Get<IXMLInputDataReader>();
		var files = GetFiles(Path.GetFullPath(_completedBusDistributed), "*.xml").ToArray();
		var completedBusFiles = new List<string>();
		foreach (var file in files) {
			try {
				var completedInput = inputDataReader.CreateDeclaration(file);
				if(completedInput.JobInputData.Vehicle is AbstractXMLDeclarationCompletedBusDataProviderV24)
				{
					completedBusFiles.Add(file);
				}
			} catch (Exception) {
				//ignore;
			}
		}

		return completedBusFiles.ToArray();
	}


    [TearDown]
	public void AfterTestChecks()
	{
		_mockDialogHelper.AssertNoErrorDialogs();
	}


	




}
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.InputData.XML.DataProvider.v2_3;

public class CapacitorDataProviderTests
{
	private StandardKernel _kernel;
	private IDeclarationInjectFactory _declarationFactory;

	[SetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
	}

	IXMLSuperCapDeclarationInputData CreateElectricMotorInputData(XmlDocument document)
	{
		var (version, componentNode) = XMLTestHelper.GetComponent(document, XMLNames.Component_CapacitorSystem);
		var input = _declarationFactory.CreateSuperCapDeclarationInputData(version, componentNode, "");
		Assert.NotNull(input);
		return input;

	}

	[TestCase()]
	public void TestCapacitor_v23_Measured()
	{
		var doc = XMLTestHelper.LoadAndValidate(Capacitor_Measured);
		var inputData = CreateElectricMotorInputData(doc);

		Assert.AreEqual("2.3", (inputData as IXMLResource).DataSource.SourceVersion);

        Assert.AreEqual(CertificationMethod.Measured, inputData.CertificationMethod);

        Assert.AreEqual(1.0, inputData.Capacity.Value());
        Assert.AreEqual(23.45, inputData.InternalResistance.Value(), 1e-3);
        Assert.AreEqual(3.55, inputData.MinVoltage.Value(), 1e-3);
        Assert.AreEqual(30.45, inputData.MaxVoltage.Value(), 1e-3);
        Assert.AreEqual(100, inputData.MaxCurrentCharge.Value(), 1e-3);
        Assert.AreEqual(99.45, inputData.MaxCurrentDischarge.Value(), 1e-3);
        Assert.AreEqual(30, inputData.TestingTemperature.AsDegCelsius, 1e-3);
    }

	[TestCase()]
	public void TestCapacitor_v23_StandardValues()
	{
		var doc = XMLTestHelper.LoadAndValidate(Capacitor_StandardValues);
		var inputData = CreateElectricMotorInputData(doc);

		Assert.AreEqual("2.3", (inputData as IXMLResource).DataSource.SourceVersion);

		Assert.AreEqual(CertificationMethod.StandardValues, inputData.CertificationMethod);

		Assert.AreEqual(1.0, inputData.Capacity.Value());
		Assert.AreEqual(0.51982, inputData.InternalResistance.Value(), 1e-3); // correction applied!
		Assert.AreEqual(3.55, inputData.MinVoltage.Value(), 1e-3);
		Assert.AreEqual(30.45, inputData.MaxVoltage.Value(), 1e-3);
		Assert.AreEqual(100, inputData.MaxCurrentCharge.Value(), 1e-3);
		Assert.AreEqual(99.45, inputData.MaxCurrentDischarge.Value(), 1e-3);
		Assert.AreEqual(30, inputData.TestingTemperature.AsDegCelsius, 1e-3);
	}


    private const string Capacitor_Measured = @"<?xml version=""1.0"" encoding=""utf-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.1"" 
xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" 
xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent v:/VectoCore/VectoCore/Resources/XSD/VectoDeclarationComponent.xsd"">
    <tns:CapacitorSystem xsi:type=""CapacitorSystemComponentDeclarationType"">
        <Data xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xsi:type=""CapacitorSystemDataType"" id=""CAP-asdf"">
            <Manufacturer>a</Manufacturer>
            <Model>a</Model>
            <CertificationNumber>token</CertificationNumber>
            <Date>2021-11-18T14:14:06.0791626Z</Date>
            <AppVersion>aaaaa</AppVersion>
            <CertificationMethod>Measured</CertificationMethod>
            <Capacitance>1.00</Capacitance>
            <InternalResistance>23.45</InternalResistance>
            <MinVoltage>3.55</MinVoltage>
            <MaxVoltage>30.45</MaxVoltage>
            <MaxChargingCurrent>100.00</MaxChargingCurrent>
            <MaxDischargingCurrent>99.45</MaxDischargingCurrent>
            <TestingTemperature>30</TestingTemperature>
        </Data>
        <v2.3:Signature>
            <di:Reference URI=""#CAP-asdf"">
                <di:Transforms>
                    <di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization"" />
                    <di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
                </di:Transforms>
                <di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
                <di:DigestValue>dBadIN60l8Iqcanj/nrx1EbD+KixtDxLAusUcutITk8=</di:DigestValue>
            </di:Reference>
        </v2.3:Signature>
    </tns:CapacitorSystem>
</tns:VectoInputDeclaration>";

    private const string Capacitor_StandardValues = @"<?xml version=""1.0"" encoding=""utf-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.1"" 
xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" 
xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent v:/VectoCore/VectoCore/Resources/XSD/VectoDeclarationComponent.xsd"">
    <tns:CapacitorSystem xsi:type=""CapacitorSystemComponentDeclarationType"">
        <Data xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xsi:type=""CapacitorSystemDataType"" id=""CAP-asdf"">
            <Manufacturer>a</Manufacturer>
            <Model>a</Model>
            <CertificationNumber>token</CertificationNumber>
            <Date>2021-11-18T14:14:06.0791626Z</Date>
            <AppVersion>aaaaa</AppVersion>
            <CertificationMethod>Standard values</CertificationMethod>
            <Capacitance>1.00</Capacitance>
            <InternalResistance>23.45</InternalResistance>
            <MinVoltage>3.55</MinVoltage>
            <MaxVoltage>30.45</MaxVoltage>
            <MaxChargingCurrent>100.00</MaxChargingCurrent>
            <MaxDischargingCurrent>99.45</MaxDischargingCurrent>
            <TestingTemperature>30</TestingTemperature>
        </Data>
        <v2.3:Signature>
            <di:Reference URI=""#CAP-asdf"">
                <di:Transforms>
                    <di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization"" />
                    <di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
                </di:Transforms>
                <di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
                <di:DigestValue>dBadIN60l8Iqcanj/nrx1EbD+KixtDxLAusUcutITk8=</di:DigestValue>
            </di:Reference>
        </v2.3:Signature>
    </tns:CapacitorSystem>
</tns:VectoInputDeclaration>";
}
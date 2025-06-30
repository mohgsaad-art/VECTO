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

public class ADCDataProviderTests
{
	private StandardKernel _kernel;
	private IDeclarationInjectFactory _declarationFactory;

	[SetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
	}

	IXMLADCDeclarationInputData CreateElectricMotorInputData(XmlDocument document)
	{
		var (version, componentNode) = XMLTestHelper.GetComponent(document, XMLNames.Component_ADC);
		var input = _declarationFactory.CreateADCDeclarationInputData(version, componentNode, "");
		Assert.NotNull(input);
		return input;

	}

	[TestCase()]
	public void TestADC_v23_Measured()
	{
		var doc = XMLTestHelper.LoadAndValidate(ADC_Measured);
		var inputData = CreateElectricMotorInputData(doc);

		Assert.AreEqual("2.3", (inputData as IXMLResource).DataSource.SourceVersion);

        Assert.AreEqual(CertificationMethod.Option1, inputData.CertificationMethod);

        Assert.AreEqual(0.035, inputData.Ratio, 1e-3);
        Assert.AreEqual(10, inputData.LossMap.Rows.Count);
    }

	private const string ADC_Measured = @"<?xml version=""1.0"" encoding=""utf-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.1"" 
xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" 
xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent v:/VectoCore/VectoCore/Resources/XSD/VectoDeclarationComponent.xsd"">
    <tns:ADC xsi:type=""v2.3:ADCComponentDeclarationType"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"">
        <Data xsi:type=""v2.3:ADCDataDeclarationType"" id=""ADC-123"" xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"">
            <Manufacturer>Generic Vendor</Manufacturer>
            <Model>ADC 20</Model>
            <CertificationNumber>ADC-20-CERT</CertificationNumber>
            <Date>2021-11-18T14:09:17.2810263Z</Date>
            <AppVersion>VectoCore</AppVersion>
            <Ratio>0.035</Ratio>
            <CertificationMethod>Option 1</CertificationMethod>
            <TorqueLossMap>
                <Entry inputSpeed=""0.00"" inputTorque=""-50000.00"" torqueLoss=""2000.00"" />
                <Entry inputSpeed=""0.00"" inputTorque=""-125.00"" torqueLoss=""5.00"" />
                <Entry inputSpeed=""0.00"" inputTorque=""0.00"" torqueLoss=""5.00"" />
                <Entry inputSpeed=""0.00"" inputTorque=""125.00"" torqueLoss=""5.00"" />
                <Entry inputSpeed=""0.00"" inputTorque=""50000.00"" torqueLoss=""2000.00"" />
                <Entry inputSpeed=""5000.00"" inputTorque=""-50000.00"" torqueLoss=""2000.00"" />
                <Entry inputSpeed=""5000.00"" inputTorque=""-780.00"" torqueLoss=""31.00"" />
                <Entry inputSpeed=""5000.00"" inputTorque=""0.00"" torqueLoss=""31.00"" />
                <Entry inputSpeed=""5000.00"" inputTorque=""780.00"" torqueLoss=""31.00"" />
                <Entry inputSpeed=""5000.00"" inputTorque=""50000.00"" torqueLoss=""2000.00"" />
            </TorqueLossMap>
        </Data>
        <v2.3:Signature>
            <di:Reference URI=""#ADC-123"">
                <di:Transforms>
                    <di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization"" />
                    <di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
                </di:Transforms>
                <di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
                <di:DigestValue>NiyH2Xp0rQswwXIOf52Jm0wvK4Yc2/PL/T+zQCWQGFo=</di:DigestValue>
            </di:Reference>
        </v2.3:Signature>
    </tns:ADC>
</tns:VectoInputDeclaration>";
}
using System.Xml;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.UnitTests.TestCases.InputData.XML;

public class WheelComponentTests
{
    [TestCaseSource(nameof(GetAllowedWheelsDimensions))]
    public void TestWheelsSupportedInXML(string tyreDimension)
    {
        var modified = XmlReader.Create(GetTyreComponent(tyreDimension).ToStream());
        var validator = new XMLValidator(modified, null, XMLValidator.CallBackExceptionOnError);
        var valid = validator.ValidateXML(XmlDocumentType.DeclarationComponentData);
        Assert.IsTrue(valid, $"error validating XML with dimension {tyreDimension}");
    }

    private static string[] GetAllowedWheelsDimensions()
    {
        return DeclarationData.Wheels.GetWheelsDimensions();

    }

    [
    TestCase("285/55R16C"), // invalid space
                            //TestCase("285/55  R16C"), // allowed, as xs:token already combines multiple whitespaces
    TestCase("85/55 R16C"), // invalid section width
    TestCase("285/1231 R16C"), // invalid aspect ratio width
    TestCase("285/55 X16C"), // invalid construction type
    TestCase("85/55 R16112C"), // invalid rim diameter
    TestCase("85/55 R16x"), // invalid suffix
    TestCase("1.0001 R12"), // 
    TestCase("9 R12111"), // invalid rim diameter
    TestCase("9 R12x"), // invalid suffix
    TestCase("9R12"), // invalid space
                      //TestCase("9  R12"), // allowed, as xs:token already combines multiple whitespaces
    ]
    public void TestInvalidWheelsDimensionString(string dim)
    {
        var modified = XmlReader.Create(GetTyreComponent(dim).ToStream());
        var validator = new XMLValidator(modified, null, XMLValidator.CallBackExceptionOnError);
        AssertHelper.Exception<VectoException>(() => validator.ValidateXML(XmlDocumentType.DeclarationComponentData), messageContains: "Validation error:");
    }

    private string GetTyreComponent(string dim)
    {
        return TyreComonentData.Replace("###DIMENSION###", dim);
    }

    const string TyreComonentData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<tns:VectoInputDeclaration 
	xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.5""
	xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0""
	xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.0""
	xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0""
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<tns:Tyre>
		<v2.0:Data id=""TYR-gooZah3D"" xsi:type=""TyreDataDeclarationType"">
			<Manufacturer>Generic Tyre Manufacturer</Manufacturer>
			<Model>Generic Tyre Model</Model>
			<CertificationNumber>e12*0815/8051*2017/05T0000*00</CertificationNumber>
			<Date>2017-02-15T11:00:00Z</Date>
			<AppVersion>DemoTyreApp 1.0</AppVersion>
			<Dimension>###DIMENSION###</Dimension>
			<TyreClass>C1</TyreClass>
			<FuelEfficiencyClass>A</FuelEfficiencyClass>
			<RRCDeclared>0.0055</RRCDeclared>
			<FzISO>33500</FzISO>
		</v2.0:Data>
		<v2.0:Signature>
			<di:Reference URI=""#TYR-gooZah3D"">
				<di:Transforms>
					<di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization"" />
					<di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
				</di:Transforms>
				<di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
				<di:DigestValue>JWEwzKSP0lXvwRgQZTiWJm9dpdtQ72FOX0CC5Vy6f2Y=</di:DigestValue>
			</di:Reference>
		</v2.0:Signature>
	</tns:Tyre>
</tns:VectoInputDeclaration>";
}
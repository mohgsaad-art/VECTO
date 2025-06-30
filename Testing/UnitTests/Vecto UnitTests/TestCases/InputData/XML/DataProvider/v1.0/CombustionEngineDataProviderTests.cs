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

public class CombustionEngineDataProvider_v10_Tests
{
	private StandardKernel _kernel;
	private IDeclarationInjectFactory _declarationFactory;

	[SetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
	}

	[TestCase()]
	public void TestEngineDataProvider_v10()
	{
		var doc = XMLTestHelper.LoadAndValidate(CombustionEngine_v10);
		var inputData = CreateEngineInputData(doc);

		Assert.AreEqual("1.0", (inputData as IXMLResource).DataSource.SourceVersion);

		Assert.AreEqual(12.730e-3, inputData.Displacement.Value(), 1e-3);
		Assert.AreEqual(2300, inputData.MaxTorqueDeclared.Value(), 1e-3);
		Assert.AreEqual(380000, inputData.RatedPowerDeclared.Value(), 1e-3);
		Assert.AreEqual(2200, inputData.RatedSpeedDeclared.AsRPM, 1e-3);
		Assert.AreEqual(WHRType.None, inputData.WHRType);
		
		Assert.AreEqual(1, inputData.EngineModes.Count);
		var mode = inputData.EngineModes.First();

		Assert.AreEqual(1, mode.Fuels.Count);
		var fuel = mode.Fuels.First();

		Assert.AreEqual(560, mode.IdleSpeed.AsRPM);
		Assert.IsNull(mode.WasteHeatRecoveryDataElectrical);
		Assert.IsNull(mode.WasteHeatRecoveryDataMechanical);
		Assert.AreEqual(10, mode.FullLoadCurve.Rows.Count);

		Assert.AreEqual(FuelType.DieselCI, fuel.FuelType);
		Assert.AreEqual(1.0097, fuel.WHTCUrban, 1e-5);
		Assert.AreEqual(1.0035, fuel.WHTCRural, 1e-5);
		Assert.AreEqual(1.020, fuel.WHTCMotorway, 1e-5);

		Assert.AreEqual(1.0, fuel.CorrectionFactorRegPer, 1e-5);
		Assert.AreEqual(1.0, fuel.ColdHotBalancingFactor, 1e-5);

		Assert.AreEqual(6, fuel.FuelConsumptionMap.Rows.Count);
    }


    IXMLEngineDeclarationInputData CreateEngineInputData(XmlDocument document)
	{
		var (version, componentNode) = XMLTestHelper.GetComponent(document, XMLNames.Component_Engine);
		var input = _declarationFactory.CreateEngineData(version, null, componentNode, "");
		Assert.NotNull(input);
		return input;

	}

    private const string CombustionEngine_v10 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0""
													xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v1.0""
													xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""1.0""
													xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
													xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v1.0 v:\VectoCore/Vectocore/Resources/XSD/VectoComponent.1.0.xsd"">
	<tns:Engine>
		<Data id=""ENG-gooZah3D"">
			<Manufacturer>Generic Engine Manufacturer</Manufacturer>
			<Model>Generic 40t Long Haul Truck Engine</Model>
			<CertificationNumber>e12*0815/8051*2017/05E0000*00</CertificationNumber>
			<Date>2017-02-15T11:00:00Z</Date>
			<AppVersion>VectoEngine x.y</AppVersion>
			<Displacement>12730</Displacement>
			<IdlingSpeed>560</IdlingSpeed>
			<RatedSpeed>2200</RatedSpeed>
			<RatedPower>380000</RatedPower>
			<MaxEngineTorque>2300</MaxEngineTorque>
			<WHTCUrban>1.0097</WHTCUrban>
			<WHTCRural>1.0035</WHTCRural>
			<WHTCMotorway>1.0200</WHTCMotorway>
			<BFColdHot>1.0000</BFColdHot>
			<CFRegPer>1.0000</CFRegPer>
			<CFNCV>1.0000</CFNCV>
			<FuelType>Diesel CI</FuelType>
			<FuelConsumptionMap>
				<Entry engineSpeed=""560.00"" torque=""-149.00"" fuelConsumption=""0.00"" />
				<Entry engineSpeed=""560.00"" torque=""0.00"" fuelConsumption=""1256.00"" />
				<Entry engineSpeed=""560.00"" torque=""1180.00"" fuelConsumption=""12869.00"" />
				
				<Entry engineSpeed=""2100.00"" torque=""-320.00"" fuelConsumption=""0.00"" />
				<Entry engineSpeed=""2100.00"" torque=""0.00"" fuelConsumption=""10470.00"" />
				<Entry engineSpeed=""2100.00"" torque=""1100.00"" fuelConsumption=""50653.00"" />
			</FuelConsumptionMap>
			<FullLoadAndDragCurve>
				<Entry engineSpeed=""560.00"" maxTorque=""1180.00"" dragTorque=""-149.00"" />
				<Entry engineSpeed=""600.00"" maxTorque=""1282.00"" dragTorque=""-148.00"" />
				<Entry engineSpeed=""800.00"" maxTorque=""1791.00"" dragTorque=""-149.00"" />
				<Entry engineSpeed=""1000.00"" maxTorque=""2300.00"" dragTorque=""-160.00"" />
				<Entry engineSpeed=""1200.00"" maxTorque=""2300.00"" dragTorque=""-179.00"" />
				<Entry engineSpeed=""1400.00"" maxTorque=""2300.00"" dragTorque=""-203.00"" />
				<Entry engineSpeed=""1600.00"" maxTorque=""2079.00"" dragTorque=""-235.00"" />
				<Entry engineSpeed=""1800.00"" maxTorque=""1857.00"" dragTorque=""-264.00"" />
				<Entry engineSpeed=""2000.00"" maxTorque=""1352.00"" dragTorque=""-301.00"" />
				<Entry engineSpeed=""2100.00"" maxTorque=""1100.00"" dragTorque=""-320.00"" />
			</FullLoadAndDragCurve>
		</Data>
		<Signature>
			<di:Reference URI=""#ENG-gooZah3D"">
				<di:Transforms>
					<di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization"" />
					<di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
				</di:Transforms>
				<di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
				<di:DigestValue>JWEwzKSP0lXvwRgQZTiWJm9dpdtQ72FOX0CC5Vy6f2Y=</di:DigestValue>
			</di:Reference>
		</Signature>
	</tns:Engine>
</tns:VectoInputDeclaration>";
}
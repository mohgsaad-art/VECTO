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

public class IEPCDataProviderTests
{
	private StandardKernel _kernel;
	private IDeclarationInjectFactory _declarationFactory;

	[SetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
	}

	IXMLIEPCInputData CreateIEPCInputData(XmlDocument document)
	{
		var (version, componentNode) = XMLTestHelper.GetComponent(document, XMLNames.Component_IEPC);
		var input = _declarationFactory.CreateIEPCData(version, null, componentNode, "");
		Assert.NotNull(input);
		return input;

	}


	[TestCase()]
	public void TestIECP_v23_Measured()
	{
		var doc = XMLTestHelper.LoadAndValidate(IECP_Measured);
		var inputData = CreateIEPCInputData(doc);

		Assert.AreEqual("2.3", (inputData as IXMLResource).DataSource.SourceVersion);

		Assert.AreEqual(CertificationMethod.Measured, inputData.CertificationMethod);

		Assert.IsFalse(inputData.DifferentialIncluded);
		Assert.IsTrue(inputData.DesignTypeWheelMotor);

		Assert.AreEqual(60587.858, inputData.TotalRatedPowerCalculated.Value(), 1e-3);

        Assert.AreEqual(2, inputData.VoltageLevels.Count);

		Assert.AreEqual(2, inputData.Gears.Count);
		Assert.AreEqual(1, inputData.Gears[0].GearNumber);
		Assert.AreEqual(3, inputData.Gears[0].Ratio, 1e-3);
		Assert.AreEqual(2, inputData.Gears[1].GearNumber);
		Assert.AreEqual(1, inputData.Gears[1].Ratio, 1e-3);

        var v1 = inputData.VoltageLevels[0];
		Assert.AreEqual(400, v1.VoltageLevel.Value(), 1e-3);
		Assert.AreEqual(200, v1.ContinuousTorque.Value(), 1e-3);
		Assert.AreEqual(2000, v1.ContinuousTorqueSpeed.AsRPM, 1e-3);
		Assert.AreEqual(400, v1.OverloadTorque.Value(), 1e-3);
		Assert.AreEqual(2000, v1.OverloadTestSpeed.AsRPM, 1e-3);
		Assert.AreEqual(30, v1.OverloadTime.Value());
		Assert.AreEqual(2, v1.FullLoadCurve.First().LoadCurve.Rows.Count);
		Assert.AreEqual(2, v1.PowerMap.Count);

		var v1p1 = v1.PowerMap[0];
		Assert.AreEqual(4, v1p1.PowerMap.Rows.Count);
		Assert.AreEqual(1, v1p1.Gear);

		var v1p2 = v1.PowerMap[1];
		Assert.AreEqual(4, v1p2.PowerMap.Rows.Count);
		Assert.AreEqual(2, v1p2.Gear);

		var v2 = inputData.VoltageLevels[1];
		Assert.AreEqual(600, v2.VoltageLevel.Value(), 1e-3);
		Assert.AreEqual(200, v2.ContinuousTorque.Value(), 1e-3);
		Assert.AreEqual(2000, v2.ContinuousTorqueSpeed.AsRPM, 1e-3);
		Assert.AreEqual(400, v2.OverloadTorque.Value(), 1e-3);
		Assert.AreEqual(2000, v2.OverloadTestSpeed.AsRPM, 1e-3);
		Assert.AreEqual(30, v2.OverloadTime.Value());
		Assert.AreEqual(2, v2.FullLoadCurve.First().LoadCurve.Rows.Count);
		Assert.AreEqual(2, v2.PowerMap.Count);

		var v2p1 = v1.PowerMap[0];
		Assert.AreEqual(4, v2p1.PowerMap.Rows.Count);
		Assert.AreEqual(1, v2p1.Gear);

		var v2p2 = v1.PowerMap[1];
		Assert.AreEqual(4, v2p2.PowerMap.Rows.Count);
		Assert.AreEqual(2, v2p2.Gear);

		Assert.AreEqual(2, inputData.DragCurves.Count);
		Assert.AreEqual(2, inputData.DragCurves[0].DragCurve.Rows.Count);
		Assert.AreEqual(1, inputData.DragCurves[0].Gear);
		Assert.AreEqual(2, inputData.DragCurves[1].DragCurve.Rows.Count);
		Assert.AreEqual(2, inputData.DragCurves[1].Gear);

        Assert.AreEqual(1, inputData.Conditioning.Rows.Count);
	}

    [TestCase()]
    public void TestIECP_v23_StandardValues()
    {

        var doc = XMLTestHelper.LoadAndValidate(IEPC_StandardValues);
        var inputData = CreateIEPCInputData(doc);

        Assert.AreEqual("2.3", (inputData as IXMLResource).DataSource.SourceVersion);

		Assert.AreEqual(CertificationMethod.StandardValues, inputData.CertificationMethod);

        Assert.IsFalse(inputData.DifferentialIncluded);
		Assert.IsTrue(inputData.DesignTypeWheelMotor);

		Assert.AreEqual(60587.858, inputData.TotalRatedPowerCalculated.Value(), 1e-3);

		Assert.AreEqual(2, inputData.Gears.Count);
		Assert.AreEqual(1, inputData.Gears[0].GearNumber);
		Assert.AreEqual(3, inputData.Gears[0].Ratio, 1e-3);
		Assert.AreEqual(2, inputData.Gears[1].GearNumber);
		Assert.AreEqual(1, inputData.Gears[1].Ratio, 1e-3);

        Assert.AreEqual(1, inputData.VoltageLevels.Count);

        var v1 = inputData.VoltageLevels[0];
        Assert.IsNull(v1.VoltageLevel);
        Assert.AreEqual(200, v1.ContinuousTorque.Value(), 1e-3);
        Assert.AreEqual(2000, v1.ContinuousTorqueSpeed.AsRPM, 1e-3);
        Assert.AreEqual(400, v1.OverloadTorque.Value(), 1e-3);
        Assert.AreEqual(2000, v1.OverloadTestSpeed.AsRPM, 1e-3);
        Assert.AreEqual(30, v1.OverloadTime.Value());
        Assert.AreEqual(2, v1.FullLoadCurve.First().LoadCurve.Rows.Count);
        Assert.AreEqual(2, v1.PowerMap.Count);

        var v1p1 = v1.PowerMap[0];
        Assert.AreEqual(4, v1p1.PowerMap.Rows.Count);
        Assert.AreEqual(1, v1p1.Gear);

        var v1p2 = v1.PowerMap[1];
        Assert.AreEqual(4, v1p2.PowerMap.Rows.Count);
        Assert.AreEqual(2, v1p2.Gear);

		Assert.AreEqual(1, inputData.DragCurves.Count);
        Assert.AreEqual(2, inputData.DragCurves[0].DragCurve.Rows.Count);
        Assert.IsNull(inputData.DragCurves[0].Gear);
		
        Assert.IsNull(inputData.Conditioning);
    }


    private const string IECP_Measured = @"<?xml version=""1.0"" encoding=""utf-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.1"" xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent v:/VectoCore/VectoCore/Resources/XSD/VectoDeclarationComponent.xsd"">
	<tns:IEPC xsi:type=""v2.3:IEPCComponentDeclarationType"">
		<Data xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xsi:type=""IEPCMeasuredDataDeclarationType"" id=""IEPC-asdf"" xmlns:v2.9=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.9"">
			<Manufacturer>a</Manufacturer>
			<Model>a</Model>
			<CertificationNumber>token</CertificationNumber>
			<Date>2021-11-18T14:31:52.4460619Z</Date>
			<AppVersion>aaaaa</AppVersion>
			<ElectricMachineType>ASM</ElectricMachineType>
			<CertificationMethod>Measured for complete component</CertificationMethod>
			<R85RatedPower>50000</R85RatedPower>
			<RotationalInertia>0.10</RotationalInertia>
			<DifferentialIncluded>false</DifferentialIncluded>
			<DesignTypeWheelMotor>true</DesignTypeWheelMotor>
			<Gears xsi:type=""v2.3:IEPCGearsDeclarationType"">
				<Gear number=""1"">
					<Ratio>3.000</Ratio>
				</Gear>
				<Gear number=""2"">
					<Ratio>1.000</Ratio>
					<MaxOutShaftTorque>2000</MaxOutShaftTorque>
				</Gear>
			</Gears>
			<VoltageLevel>
				<Voltage>400</Voltage>
				<ContinuousTorque>200.00</ContinuousTorque>
				<TestSpeedContinuousTorque>2000.00</TestSpeedContinuousTorque>
				<OverloadTorque>400.00</OverloadTorque>
				<TestSpeedOverloadTorque>2000.00</TestSpeedOverloadTorque>
				<OverloadDuration>30.00</OverloadDuration>
				<MaxTorqueCurve>
					<Entry outShaftSpeed=""0.00"" maxTorque=""450.00"" minTorque=""-450.00""/>
					<Entry outShaftSpeed=""4000.00"" maxTorque=""100.00"" minTorque=""-100.00""/>
				</MaxTorqueCurve>
				<PowerMap gear=""1"">
					<Entry outShaftSpeed=""0.00"" torque=""-400.00"" electricPower=""-1000.00""/>
					<Entry outShaftSpeed=""0.00"" torque=""400.00"" electricPower=""1000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""-4000.00"" electricPower=""-20000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""4000.00"" electricPower=""20000.00""/>
				</PowerMap>
				<PowerMap gear=""2"">
					<Entry outShaftSpeed=""0.00"" torque=""-500.00"" electricPower=""-1500.00""/>
					<Entry outShaftSpeed=""0.00"" torque=""500.00"" electricPower=""1500.00""/>
					<Entry outShaftSpeed=""5000.00"" torque=""-5000.00"" electricPower=""-25000.00""/>
					<Entry outShaftSpeed=""5000.00"" torque=""5000.00"" electricPower=""25000.00""/>
				</PowerMap>
			</VoltageLevel>
			<VoltageLevel>
				<Voltage>600</Voltage>
				<ContinuousTorque>200.00</ContinuousTorque>
				<TestSpeedContinuousTorque>2000.00</TestSpeedContinuousTorque>
				<OverloadTorque>400.00</OverloadTorque>
				<TestSpeedOverloadTorque>2000.00</TestSpeedOverloadTorque>
				<OverloadDuration>30.00</OverloadDuration>
				<MaxTorqueCurve>
					<Entry outShaftSpeed=""0.00"" maxTorque=""450.00"" minTorque=""-450.00""/>
					<Entry outShaftSpeed=""4000.00"" maxTorque=""100.00"" minTorque=""-100.00""/>
				</MaxTorqueCurve>
				<PowerMap gear=""1"">
					<Entry outShaftSpeed=""0.00"" torque=""-400.00"" electricPower=""-1000.00""/>
					<Entry outShaftSpeed=""0.00"" torque=""400.00"" electricPower=""1000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""-4000.00"" electricPower=""-20000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""4000.00"" electricPower=""20000.00""/>
				</PowerMap>
				<PowerMap gear=""2"">
					<Entry outShaftSpeed=""0.00"" torque=""-500.00"" electricPower=""-1500.00""/>
					<Entry outShaftSpeed=""0.00"" torque=""500.00"" electricPower=""1500.00""/>
					<Entry outShaftSpeed=""5000.00"" torque=""-5000.00"" electricPower=""-25000.00""/>
					<Entry outShaftSpeed=""5000.00"" torque=""5000.00"" electricPower=""25000.00""/>
				</PowerMap>
			</VoltageLevel>
			<DragCurve gear=""1"">
				<Entry outShaftSpeed=""0.00"" dragTorque=""10.00""/>
				<Entry outShaftSpeed=""4000.00"" dragTorque=""30.00""/>
			</DragCurve>
			<DragCurve gear=""2"">
				<Entry outShaftSpeed=""0.00"" dragTorque=""15.00""/>
				<Entry outShaftSpeed=""4500.00"" dragTorque=""35.00""/>
			</DragCurve>
			<Conditioning>
				<Entry coolantTempInlet=""30"" coolingPower=""5000""/>
			</Conditioning>
		</Data>
		<v2.3:Signature>
			<di:Reference URI=""#IEPC-asdf"">
				<di:Transforms>
					<di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization""/>
					<di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>
				</di:Transforms>
				<di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""/>
				<di:DigestValue>3L/fYxKTdIwzADHQMnUBPxcNwZNEHM+sKEC2M32UnEA=</di:DigestValue>
			</di:Reference>
		</v2.3:Signature>
	</tns:IEPC>
</tns:VectoInputDeclaration>
";

	private const string IEPC_StandardValues = @"<?xml version=""1.0"" encoding=""utf-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.1"" xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent v:/VectoCore/VectoCore/Resources/XSD/VectoDeclarationComponent.xsd"">
	<tns:IEPC xsi:type=""v2.3:IEPCComponentDeclarationType"">
		<Data xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xsi:type=""IEPCStandardValuesDataDeclarationType"" id=""IEPC-asdf"" xmlns:v2.9=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.9"">
			<Manufacturer>a</Manufacturer>
			<Model>a</Model>
			<CertificationNumber>token</CertificationNumber>
			<Date>2021-11-18T14:33:55.6801441Z</Date>
			<AppVersion>aaaaa</AppVersion>
			<ElectricMachineType>ASM</ElectricMachineType>
			<CertificationMethod>Standard values for all components</CertificationMethod>
			<R85RatedPower>50000</R85RatedPower>
			<RotationalInertia>0.10</RotationalInertia>
			<DifferentialIncluded>false</DifferentialIncluded>
			<DesignTypeWheelMotor>true</DesignTypeWheelMotor>
			<Gears xsi:type=""v2.3:IEPCGearsDeclarationType"">
				<Gear number=""1"">
					<Ratio>3.000</Ratio>
				</Gear>
				<Gear number=""2"">
					<Ratio>1.000</Ratio>
					<MaxOutShaftTorque>2000</MaxOutShaftTorque>
				</Gear>
			</Gears>
			<VoltageLevel>
				<ContinuousTorque>200.00</ContinuousTorque>
				<TestSpeedContinuousTorque>2000.00</TestSpeedContinuousTorque>
				<OverloadTorque>400.00</OverloadTorque>
				<TestSpeedOverloadTorque>2000.00</TestSpeedOverloadTorque>
				<OverloadDuration>30.00</OverloadDuration>
				<MaxTorqueCurve>
					<Entry outShaftSpeed=""0.00"" maxTorque=""450.00"" minTorque=""-450.00""/>
					<Entry outShaftSpeed=""4000.00"" maxTorque=""100.00"" minTorque=""-100.00""/>
				</MaxTorqueCurve>
				<PowerMap gear=""1"">
					<Entry outShaftSpeed=""0.00"" torque=""-400.00"" electricPower=""-1000.00""/>
					<Entry outShaftSpeed=""0.00"" torque=""400.00"" electricPower=""1000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""-4000.00"" electricPower=""-20000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""4000.00"" electricPower=""20000.00""/>
				</PowerMap>
				<PowerMap gear=""2"">
					<Entry outShaftSpeed=""0.00"" torque=""-400.00"" electricPower=""-1000.00""/>
					<Entry outShaftSpeed=""0.00"" torque=""400.00"" electricPower=""1000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""-4000.00"" electricPower=""-20000.00""/>
					<Entry outShaftSpeed=""4000.00"" torque=""4000.00"" electricPower=""20000.00""/>
				</PowerMap>
			</VoltageLevel>
			<DragCurve>
				<Entry outShaftSpeed=""0.00"" dragTorque=""10.00""/>
				<Entry outShaftSpeed=""4000.00"" dragTorque=""30.00""/>
			</DragCurve>
		</Data>
		<v2.3:Signature>
			<di:Reference URI=""#IEPC-asdf"">
				<di:Transforms>
					<di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization""/>
					<di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>
				</di:Transforms>
				<di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""/>
				<di:DigestValue>BTHs/Hh2SgycIwU5OSuTgU/2SptMvmRFvPXr2X1Y7XQ=</di:DigestValue>
			</di:Reference>
		</v2.3:Signature>
	</tns:IEPC>
</tns:VectoInputDeclaration>
";
}
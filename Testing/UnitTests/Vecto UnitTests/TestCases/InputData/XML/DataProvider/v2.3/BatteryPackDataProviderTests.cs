using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;
using TestContext = NUnit.Framework.TestContext;

namespace TUGraz.Vecto.UnitTests.TestCases.InputData.XML.DataProvider.v2_3;

public class BatteryPackDataProviderTests
{
	private StandardKernel _kernel;
	private IDeclarationInjectFactory _declarationFactory;

	[SetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		var inputDataFactory = _kernel.Get<IXMLInputDataReader>();
		_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
	}

	[TestCase(BatterySystemComponentDataMeasured_v23, typeof(XMLBatteryPackDeclarationInputDataMeasuredV23),
		TestName = "XMLBatterySystemData InternalResistance Measured")]
	[TestCase(BatterySystemComponentDataStdVal_v23, typeof(XMLBatteryPackDeclarationInputDataStandardV23),
		TestName = "XMLBatterySystemData InternalResistance StandardValues")]
	public void BatterySystemInternalResistanceTest(string xmlData, Type expectedType)
	{
		var document = XMLTestHelper.LoadAndValidate(xmlData);
		Assert.IsNotNull(document);
		TestContext.WriteLine(document);

		var reessReader = CreateBatterySystemReader(document);
		Assert.AreEqual(reessReader.GetType(), expectedType);
		switch (reessReader) {
			case XMLBatteryPackDeclarationInputDataMeasuredV23 m:
				CheckInternalResistanceMeasured(m, document);
				break;
			case XMLBatteryPackDeclarationInputDataStandardV23 s:
				CheckInternalResistanceStandard(s, document);
				break;
			default:
				Assert.Fail("unexpected type");
				break;
		}

	}

	[TestCase(BatterySystemComponentDataMeasured_v23, typeof(XMLBatteryPackDeclarationInputDataMeasuredV23),
		TestName = "XMLBatterySystemData MaxCurrent Measured")]
	[TestCase(BatterySystemComponentDataStdVal_v23, typeof(XMLBatteryPackDeclarationInputDataStandardV23),
		TestName = "XMLBatterySystemData MaxCurrent StandardValues")]
	public void BatterySystemMaxCurrentTest(string xmlData, Type expectedType)
	{
		var document = XMLTestHelper.LoadAndValidate(xmlData);
		Assert.IsNotNull(document);
		TestContext.WriteLine(document);

		var reessReader = CreateBatterySystemReader(document);
		Assert.AreEqual(reessReader.GetType(), expectedType);
		switch (reessReader) {
			case XMLBatteryPackDeclarationInputDataMeasuredV23 m:
				var expectedM = new[] {
					Tuple.Create(0.0, 50.0, 0.0),
					Tuple.Create(1.0, 0.0, -50.0)
				};
				CheckMaxCurrent(reessReader, expectedM);
				break;
			case XMLBatteryPackDeclarationInputDataStandardV23 s:
				var expectedStd = new[] {
					Tuple.Create(0.0, 50.0 * 0.9, 0.0 * 5.0),
					Tuple.Create(0.3, 50.0 * 0.9, -50.0 * 5.0),
					Tuple.Create(0.8, 50.0 * 0.9, -50.0 * 5.0),
					Tuple.Create(1.0, 0.0 * 0.9, -50.0 * 5.0)
				};
				CheckMaxCurrent(reessReader, expectedStd);
				break;
			default:
				Assert.Fail("unexpected type");
				break;
		}

	}

	private void CheckMaxCurrent(IXMLBatteryPackDeclarationInputData m,
		Tuple<double, double, double>[] expected)
	{
		var maxCurrentMap = BatteryMaxCurrentReader.Create(m.MaxCurrentMap);

		foreach (var e in expected) {
			Assert.AreEqual(e.Item2, maxCurrentMap.LookupMaxChargeCurrent(e.Item1).Value(),
				"max charge current for SoC {0} diverges. expected: {1} actual: {2}", e.Item1, e.Item2,
				maxCurrentMap.LookupMaxChargeCurrent(e.Item1));
			Assert.AreEqual(e.Item3, maxCurrentMap.LookupMaxDischargeCurrent(e.Item1).Value(),
				"max charge current for SoC {0} diverges. expected: {1} actual: {2}", e.Item1, e.Item3,
				maxCurrentMap.LookupMaxDischargeCurrent(e.Item1));
		}
	}

	public void CheckInternalResistanceMeasured(IXMLBatteryPackDeclarationInputData m, XmlDocument document)
	{
		var resistanceCurve = m.InternalResistanceCurve;
		var resistanceMap = BatteryInternalResistanceReader.Create(resistanceCurve, true);
		//from input file "BatterySystem_StdValues.xml"

		var uncorrected = ReadInternalResistanceFromFile(document);
		for (var rowIdx = 0; rowIdx < uncorrected.Rows.Count; rowIdx++) {
			for (var colIdx = 0; colIdx < uncorrected.Columns.Count; colIdx++) {
				if (colIdx == 0) {
					//SOC no change
					Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx),
						resistanceCurve.Rows[rowIdx].ParseDouble(colIdx));
					continue;
				}

				Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx),
					resistanceCurve.Rows[rowIdx].ParseDouble(colIdx), 10e-3); //mOhm

				var soc = uncorrected.Rows[rowIdx].ParseDouble(0) / 100.0;
				var entry = resistanceMap.Lookup(soc, 0.SI<Second>());
				Assert.AreEqual(entry.AsMilliOhm, uncorrected.Rows[rowIdx].ParseDouble(1));
			}
		}
	}

	public void CheckInternalResistanceStandard(IXMLBatteryPackDeclarationInputData s, XmlDocument document)
	{
		var resistanceCurve = s.InternalResistanceCurve;
		var resistanceMap = BatteryInternalResistanceReader.Create(resistanceCurve, true);
		//from input file "BatterySystem_StdValues.xml"
		var nominalVoltage = BatterySOCReader.Create(s.VoltageCurve).Lookup(0.5); //630

		var uncorrected = ReadInternalResistanceFromFile(document);
		//dcir = 630/3.3 = 190.90909090
		var dcir = 190.90909090909090;
		for (var rowIdx = 0; rowIdx < uncorrected.Rows.Count; rowIdx++) {
			for (var colIdx = 0; colIdx < uncorrected.Columns.Count; colIdx++) {
				if (colIdx == 0) {
					//SOC no change
					Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx),
						resistanceCurve.Rows[rowIdx].ParseDouble(colIdx));
					continue;
				}

				Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx) * dcir,
					resistanceCurve.Rows[rowIdx].ParseDouble(colIdx), 10e-3); //mOhm
				var soc = uncorrected.Rows[rowIdx].ParseDouble(0) / 100.0;
				var entry = resistanceMap.Lookup(soc, 0.SI<Second>());
				Assert.AreEqual(entry.AsMilliOhm, uncorrected.Rows[rowIdx].ParseDouble(1) * dcir, 1e-2);
			}
		}
	}

	IXMLBatteryPackDeclarationInputData CreateBatterySystemReader(XmlDocument document)
	{
		var (version, componentNode) = XMLTestHelper.GetComponent(document, XMLNames.Component_BatterySystem);
		var input = _declarationFactory.CreateBatteryPackDeclarationInputData(version, null, componentNode, "");
		Assert.NotNull(input);
		return input;

	}


	

    public TableData ReadInternalResistanceFromFile(XmlDocument document)
	{
		var irc = document.GetElementsByTagName(XMLNames.REESS_InternalResistanceCurve)[0];

		var entries = irc?.ChildNodes;
		if (entries is { Count: > 0 }) {
			return XMLHelper.ReadTableData(AttributeMappings.InternalResistanceMap, entries);
		}

		Assert.Fail();
		return null;
	}

	private const string BatterySystemComponentDataMeasured_v23 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.1"" xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent v:/VectoCore/VectoCore/Resources/XSD/VectoDeclarationComponent.xsd"">
    <tns:BatterySystem xsi:type=""v2.3:BatteryComponentDeclarationType"">
        <Data xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xsi:type=""BatterySystemDataType"" id=""BAT-asdf"" xmlns:v2.9=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.9"">
            <Manufacturer>a</Manufacturer>
            <Model>a</Model>
            <CertificationNumber>token</CertificationNumber>
            <Date>2021-11-18T14:11:51.3452323Z</Date>
            <AppVersion>aaaaa</AppVersion>
            <CertificationMethod>Measured</CertificationMethod>
            <BatteryType>HPBS</BatteryType>
            <RatedCapacity>72.00</RatedCapacity>
            <ConnectorsSubsystemsIncluded>true</ConnectorsSubsystemsIncluded>
            <JunctionboxIncluded>true</JunctionboxIncluded>
            <TestingTemperature>20</TestingTemperature>
            <OCV>
                <Entry SoC=""0"" OCV=""620.00"" />
                <Entry SoC=""100"" OCV=""640.00"" />
            </OCV>
            <InternalResistance>
                <Entry SoC=""0"" R_2=""10.00"" R_10=""11.00"" R_20=""12.00"" />
                <Entry SoC=""100"" R_2=""12.00"" R_10=""14.00"" R_20=""16.00"" />
            </InternalResistance>
            <CurrentLimits>
                <Entry SoC=""0"" maxChargingCurrent=""50.00"" maxDischargingCurrent=""0.00"" />
                <Entry SoC=""100"" maxChargingCurrent=""0.00"" maxDischargingCurrent=""50.00"" />
            </CurrentLimits>
        </Data>
        <v2.3:Signature>
            <di:Reference URI=""#BAT-asdf"">
                <di:Transforms>
                    <di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization"" />
                    <di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
                </di:Transforms>
                <di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
                <di:DigestValue>tam1LGpdznHGFGo+rp0WVr0/6+F2yU2Kv4G4tYvAe+Y=</di:DigestValue>
            </di:Reference>
        </v2.3:Signature>
    </tns:BatterySystem>
</tns:VectoInputDeclaration>";

	private const string BatterySystemComponentDataStdVal_v23 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<tns:VectoInputDeclaration xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:tns=""urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.1"" xmlns:v2.0=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0"" xmlns:v2.3=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xmlns:di=""http://www.w3.org/2000/09/xmldsig#"" schemaVersion=""2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:tugraz:ivt:VectoAPI:DeclarationComponent v:\VectoCore\VectoCore\Resources\XSD/VectoDeclarationComponent.xsd"">
    <tns:BatterySystem xsi:type=""v2.3:BatteryComponentDeclarationType"">
        <Data xmlns=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3"" xsi:type=""BatterySystemStandardValuesDataType"" id=""BAT-asdf"" xmlns:v2.9=""urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.9"">
            <Manufacturer>a</Manufacturer>
            <Model>a</Model>
            <CertificationNumber>token</CertificationNumber>
            <Date>2021-11-18T14:11:51.3452323Z</Date>
            <AppVersion>aaaaa</AppVersion>
            <CertificationMethod>Standard values</CertificationMethod>
            <BatteryType>HPBS</BatteryType>
            <RatedCapacity>72.00</RatedCapacity>
            <OCV>
                <Entry SoC=""0"" OCV=""620.00"" />
                <Entry SoC=""100"" OCV=""640.00"" />
            </OCV>
            <InternalResistance>
                <Entry SoC=""0"" R_2=""10.00"" R_10=""11.00"" R_20=""12.00"" />
                <Entry SoC=""100"" R_2=""12.00"" R_10=""14.00"" R_20=""16.00"" />
            </InternalResistance>
            <CurrentLimits>
                <Entry SoC=""0"" maxChargingCurrent=""50.00"" maxDischargingCurrent=""0.00"" />
                <Entry SoC=""100"" maxChargingCurrent=""0.00"" maxDischargingCurrent=""50.00"" />
            </CurrentLimits>
        </Data>
        <v2.3:Signature>
            <di:Reference URI=""#BAT-asdf"">
                <di:Transforms>
                    <di:Transform Algorithm=""urn:vecto:xml:2017:canonicalization"" />
                    <di:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
                </di:Transforms>
                <di:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
                <di:DigestValue>tam1LGpdznHGFGo+rp0WVr0/6+F2yU2Kv4G4tYvAe+Y=</di:DigestValue>
            </di:Reference>
        </v2.3:Signature>
    </tns:BatterySystem>
</tns:VectoInputDeclaration>";

}
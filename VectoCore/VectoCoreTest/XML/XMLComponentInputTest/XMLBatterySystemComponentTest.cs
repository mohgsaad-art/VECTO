using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML.XMLComponentInputTest
{
    [TestFixture]
    public class XMLBatterySystemComponentTest
    {
		private StandardKernel _kernel;

		private string BASEDIRComponent = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/ComponentData";

		private string BASEDIRVehicle = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry";

        private IDeclarationInjectFactory _declarationFactory;
		private IXMLInputDataReader _inputDataFactory;

		[SetUp]
		public void Setup()
		{
			_kernel = new StandardKernel(new VectoNinjectModule());
			_inputDataFactory = _kernel.Get<IXMLInputDataReader>();
			_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
		}

		[TestCase("BatterySystem_1.xml", typeof(XMLBatteryPackDeclarationInputDataMeasuredV23), TestName ="XMLBatterySystemData InternalResistance Measured")]
		[TestCase("BatterySystem_StdValues.xml", typeof(XMLBatteryPackDeclarationInputDataStandardV23), TestName = "XMLBatterySystemData InternalResistance StandardValues")]
		[Category(Definitions.TESTCASE_MIGRATED)]
        public void BatterySystemInternalResistanceTest(string fileName, Type expectedType)
		{
			var path = GetFullPath(BASEDIRComponent, fileName);
			var document = LoadAndValidate(path);
			Assert.IsNotNull(document);
			TestContext.WriteLine(document);

			var reessReader = CreateBatterySystemReader(document, path);
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

		[TestCase("BatterySystem_1.xml", typeof(XMLBatteryPackDeclarationInputDataMeasuredV23), TestName = "XMLBatterySystemData MaxCurrent Measured")]
		[TestCase("BatterySystem_StdValues.xml", typeof(XMLBatteryPackDeclarationInputDataStandardV23), TestName = "XMLBatterySystemData MaxCurrent StandardValues")]
		[Category(Definitions.TESTCASE_MIGRATED)]
		public void BatterySystemMaxCurrentTest(string fileName, Type expectedType)
		{
			var path = GetFullPath(BASEDIRComponent, fileName);
			var document = LoadAndValidate(path);
			Assert.IsNotNull(document);
			TestContext.WriteLine(document);

			var reessReader = CreateBatterySystemReader(document, path);
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
						Tuple.Create(0.0, 50.0 * 0.9,   0.0 * 5.0),
						Tuple.Create(0.3, 50.0 * 0.9, -50.0 * 5.0),
						Tuple.Create(0.8, 50.0 * 0.9, -50.0 * 5.0),
						Tuple.Create(1.0,  0.0 * 0.9, -50.0 * 5.0)
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
				Assert.AreEqual(e.Item2, maxCurrentMap.LookupMaxChargeCurrent(e.Item1).Value(), "max charge current for SoC {0} diverges. expected: {1} actual: {2}", e.Item1, e.Item2, maxCurrentMap.LookupMaxChargeCurrent(e.Item1));
				Assert.AreEqual(e.Item3, maxCurrentMap.LookupMaxDischargeCurrent(e.Item1).Value(), "max charge current for SoC {0} diverges. expected: {1} actual: {2}", e.Item1, e.Item3, maxCurrentMap.LookupMaxDischargeCurrent(e.Item1));
			}
        }

		public void CheckInternalResistanceMeasured(IXMLBatteryPackDeclarationInputData m, XmlDocument document)
		{
			var resistanceCurve = m.InternalResistanceCurve;
			var resistanceMap = BatteryInternalResistanceReader.Create(resistanceCurve, true);
			//from input file "BatterySystem_StdValues.xml"

			var uncorrected = ReadInternalResistanceFromFile(document);
			for (var rowIdx = 0; rowIdx < uncorrected.Rows.Count; rowIdx++)
			{
				for (var colIdx = 0; colIdx < uncorrected.Columns.Count; colIdx++)
				{
					if (colIdx == 0)
					{
						//SOC no change
						Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx), resistanceCurve.Rows[rowIdx].ParseDouble(colIdx));
						continue;
					}
					Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx), resistanceCurve.Rows[rowIdx].ParseDouble(colIdx), 10e-3);  //mOhm
					
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
						Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx), resistanceCurve.Rows[rowIdx].ParseDouble(colIdx));
						continue;
					}
					Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx) * dcir, resistanceCurve.Rows[rowIdx].ParseDouble(colIdx), 10e-3);  //mOhm
					var soc = uncorrected.Rows[rowIdx].ParseDouble(0) / 100.0;
					var entry = resistanceMap.Lookup(soc, 0.SI<Second>());
					Assert.AreEqual(entry.AsMilliOhm, uncorrected.Rows[rowIdx].ParseDouble(1) * dcir);
                }
			}
		}

		public TableData ReadInternalResistanceFromFile(XmlDocument document)
		{
			var irc = document.GetElementsByTagName(XMLNames.REESS_InternalResistanceCurve)[0];

			var entries = irc?.ChildNodes;
            if (entries is { Count: > 0 })
			{
				return XMLHelper.ReadTableData(AttributeMappings.InternalResistanceMap, entries);
            }

			Assert.Fail();
			return null;
		}




    [TestCase(XMLBatteryPackDeclarationInputDataMeasuredV23.XSD_TYPE,
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23,
			typeof(XMLBatteryPackDeclarationInputDataMeasuredV23))]
		[TestCase(XMLBatteryPackDeclarationInputDataStandardV23.XSD_TYPE,
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23,
			typeof(XMLBatteryPackDeclarationInputDataStandardV23))]
		[Category(Definitions.TESTCASE_MIGRATED)]
        public void FactoryTest(string type, string ns, Type expType)
		{
			XNamespace nameSpace = ns;
			var provider = _declarationFactory.CreateBatteryPackDeclarationInputData(XMLHelper.CombineNamespace(nameSpace, type), null, null, null);

			Assert.AreEqual(expType, provider.GetType());
			Assert.NotNull(provider);

		}

		[DebuggerStepThrough]
		public string GetFullPath(string baseDir, string fileName)
		{
			return Path.GetFullPath((Path.Combine(baseDir, fileName)));
        }

		[TestCase("PEV_heavyLorry_E3.xml")]
		public void CreateFromVehicle(string vehicleName)
		{
			var inputData = _inputDataFactory.CreateDeclaration(GetFullPath(BASEDIRVehicle, vehicleName));
			var ressPack = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements[0].REESSPack;

		}

		IXMLBatteryPackDeclarationInputData CreateBatterySystemReader(XmlDocument document, string source)
		{
			var componentNode = document.FirstChild.NextSibling.SelectSingleNode($"./*[local-name()='{XMLNames.Component_BatterySystem}']");
			Assert.NotNull(componentNode);
            var dataNode = componentNode.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']");
			Assert.NotNull(dataNode);
			var version = XMLHelper.GetXsdType(dataNode.SchemaInfo.SchemaType);
			var input = _declarationFactory.CreateBatteryPackDeclarationInputData(version, null, componentNode,  source);
            Assert.NotNull(input);
			return input;

		}


		public XmlDocument LoadAndValidate(string filePath)
		{
			Assert.IsTrue(File.Exists(filePath), $"{filePath} missing");
			var document = new XmlDocument();
			using (var reader = XmlReader.Create(filePath)) {
				document.Load(reader);
			}
			var xmlValidator = new XMLValidator(document, null, XMLValidator.CallBackExceptionOnError);
			Assert.IsTrue(xmlValidator.ValidateXML(XmlDocumentType.DeclarationComponentData));
			return document;
		}

		//var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
		//	try {
		//	var input = DeclarationFactory.CreateInputProvider(versionNumber, xmlDoc, source);
		//	input.Reader = DeclarationFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement);
		//	return input;
		//} catch (Exception e) {
		//throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
	}
}

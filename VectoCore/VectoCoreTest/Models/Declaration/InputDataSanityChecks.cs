using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class InputDataSanityChecks
	{
		public const string HeavyLorryConventional = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/HeavyLorry/Conventional_heavyLorry_AMT.xml";
		public const string MediumLorryConventional = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/MediumLorry/Conventional_mediumLorry_AMT.xml";
		public const string PrimaryBusConventional = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/PrimaryBus/Conventional_primaryBus_AMT.xml";

		private IXMLInputDataReader _xmlInputReader;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			var kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = kernel.Get<IXMLInputDataReader>();
		}

		[
			TestCase(HeavyLorryConventional, 1, "Fixed displacement"),
			TestCase(HeavyLorryConventional, 2, "Fixed displacement", "Dual displacement"),
			TestCase(HeavyLorryConventional, 2, "Full electric steering gear", "Electric driven pump"),
			TestCase(HeavyLorryConventional, 1, "Full electric steering gear"),

			TestCase(MediumLorryConventional, 1, "Fixed displacement"),
			TestCase(MediumLorryConventional, 2, "Fixed displacement", "Dual displacement"),
			TestCase(MediumLorryConventional, 2, "Full electric steering gear", "Electric driven pump"),
			TestCase(MediumLorryConventional, 1, "Full electric steering gear"),

			TestCase(PrimaryBusConventional, 1, "Fixed displacement"),
			TestCase(PrimaryBusConventional, 2, "Fixed displacement", "Dual displacement"),
			TestCase(PrimaryBusConventional, 2, "Full electric steering gear", "Electric driven pump"),
			TestCase(PrimaryBusConventional, 1, "Full electric steering gear"),
			Category(Definitions.TESTCASE_MIGRATED),
		]
		public void TestCorrectNumberSteeredAxles(string jobFile, int numStreeredAxles, params string[] steeringPumpTechnologies)
		{
			var modified = GetModifiedXML(jobFile, numStreeredAxles, steeringPumpTechnologies);

			var writer = new FileOutputWriter("SanityCheckTest");

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, modified, writer);
			factory.WriteModalResults = true;
			factory.Validate = false;

			var runs = factory.RunDataFactory.NextRun().ToList();
			
			Assert.IsTrue(runs.Count > 0);
		}

		[
			TestCase(HeavyLorryConventional, 2, "Fixed displacement"),
			TestCase(HeavyLorryConventional, 1, "Fixed displacement", "Dual displacement"),
			TestCase(HeavyLorryConventional, 1, "Full electric steering gear", "Electric driven pump"),
			TestCase(HeavyLorryConventional, 1, "Fixed displacement", "Dual displacement", "Full electric steering gear"),

			TestCase(MediumLorryConventional, 2, "Fixed displacement"),
			TestCase(MediumLorryConventional, 1, "Fixed displacement", "Dual displacement"),
			TestCase(MediumLorryConventional, 1, "Full electric steering gear", "Electric driven pump"),
			TestCase(MediumLorryConventional, 1, "Fixed displacement", "Dual displacement", "Full electric steering gear"),

			TestCase(PrimaryBusConventional, 2, "Fixed displacement"),
			TestCase(PrimaryBusConventional, 1, "Fixed displacement", "Dual displacement"),
			TestCase(PrimaryBusConventional, 1, "Full electric steering gear", "Electric driven pump"),
			TestCase(PrimaryBusConventional, 1, "Fixed displacement", "Dual displacement", "Full electric steering gear"),
			Category(Definitions.TESTCASE_MIGRATED),
		]
		public void TestWrongNumberSteeredAxles(string jobFile, int numStreeredAxles, params string[] steeringPumpTechnologies)
		{
			var modified = GetModifiedXML(jobFile, numStreeredAxles, steeringPumpTechnologies);

			var writer = new FileOutputWriter("SanityCheckTest");

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, modified, writer);
			factory.WriteModalResults = true;
			factory.Validate = false;

			AssertHelper.Exception<VectoException>(() => {
				var runs = factory.RunDataFactory.NextRun().ToList();
			}, messageContains: $"Number of steering pump technologies does not match number of steered axles ({numStreeredAxles}, {steeringPumpTechnologies.Length})");

		}

		private IInputDataProvider GetModifiedXML(string jobFile, int numStreeredAxles, string[] steeringPumpTechnologies)
		{
			var inputXml = new XmlDocument();
			inputXml.Load(jobFile);

			var axleNodesSteered = inputXml.SelectNodes("//*[local-name()='Axle']/*[local-name()='Steered']");

			var cnt = 0;
			foreach (XmlNode steered in axleNodesSteered) {
				steered.InnerText = cnt++ < numStreeredAxles ? "true" : "false";
			}

			var steeringPumpNode = inputXml.SelectSingleNode("//*[local-name()='SteeringPump']");
			steeringPumpNode.RemoveAll();
			cnt = 1;
			foreach (var steeringPumpTechnology in steeringPumpTechnologies) {
				var tech = inputXml.CreateElement("Technology", "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4");
				var val = inputXml.CreateTextNode(steeringPumpTechnology);
				var attr = inputXml.CreateAttribute("axleNumber");
				var attrVal = inputXml.CreateTextNode(cnt.ToString());
				attr.AppendChild(attrVal);
				tech.Attributes.Append(attr);
				tech.AppendChild(val);

				steeringPumpNode.AppendChild(tech);
				cnt++;
			}

			var modified = XmlReader.Create(new StringReader(inputXml.OuterXml));

			return _xmlInputReader.CreateDeclaration(modified);
		}
	}
}
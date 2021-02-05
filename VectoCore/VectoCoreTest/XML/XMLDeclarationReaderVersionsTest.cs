using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLDeclarationReaderVersionsTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(@"SchemaVersion1.0\Tractor_4x2_vehicle-class-5_5_t_0.xml")]
		public void TestReadingJobVersion_V10(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"SchemaVersion2.0\Tractor_4x2_vehicle-class-5_5_t_0.xml")]
		public void TestReadingJobVersion_V20(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}


		[TestCase(@"SchemaVersion2.0\vecto_vehicle-components_1.0.xml")]
		public void TestReadingJobVersion_V20_ComponentsV10(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"SchemaVersion2.1\Tractor_4x2_vehicle-class-5_5_t_0.xml")]
		public void TestReadingJobVersion_V21(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"SchemaVersion2.1\vecto_vehicle-exempted-sample.xml")]
		public void TestReadingJobVersion_V21_Exempted(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"SchemaVersion2.1\vecto_vehicle-components_1.0.xml")]
		public void TestReadingJobVersion_V21_ComponentsV10(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"SchemaVersion2.1\vecto_vehicle-components_2.0.xml")]
		public void TestReadingJobVersion_V21_ComponentsV20(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"SchemaVersion2.2\Tractor_4x2_vehicle-class-5_5_t_0.xml")]
		public void TestReadingJobVersion_V22(string jobFile)
		{
			// does not work as the new tire dimension is not allowed in VECTO at the moment
			//var runs = ReadDeclarationJob(jobFile);

			//Assert.AreEqual("235/60 R17 C", runs[0].GetContainer().RunData.VehicleData.AxleData[1].WheelsDimension);

			var filename = Path.Combine(@"TestData\XML\XMLReaderDeclaration", jobFile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.AreEqual("235/60 R17C", dataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration[1].Tyre.Dimension);

		}


		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.2\vecto_tyre-sample.xml", typeof(ITyreDeclarationInputData), TestName = "TestReadingXmlComponent Tyre 2.2"),
		TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion1.0\vecto_engine-sample.xml",typeof(IEngineDeclarationInputData), TestName = "TestReadingXmlComponent Engine 1.0")]
		public void TestReadingXMLComponent(string filename, Type expectedType)
		{
			var component = xmlInputReader.CreateComponent(XmlReader.Create(filename));

			Assert.IsTrue(expectedType.IsInstanceOfType(component));
		}
		
		[TestCase(@"TestData\Integration\MediumLorries\vecto_vehicle-medium_lorry-Van.xml", TestName = "Medium Lorry Van"),
		TestCase(@"TestData\Integration\MediumLorries\vecto_vehicle-medium_lorry.xml", TestName = "Medium Lorry"),
		TestCase(@"TestData\Integration\MediumLorries\vecto_vehicle-medium_lorryFWD.xml", TestName = "Medium Lorry FWD"),
        ]
		public void CreateRunDataMediumLorry(string jobFile)
		{
			var runIdx = 0;
			//var jobFile = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.6_Buses\vecto_vehicle-medium_lorry-sample.xml";

			var writer = new FileOutputWriter(jobFile);
			var inputData = xmlInputReader.CreateDeclaration(jobFile);
			
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			//var runs = factory.SimulationRuns().ToArray();
			//jobContainer.AddRun(runs[runIdx]);
			//runs[runIdx].Run();

			Assert.IsTrue(jobContainer.AllCompleted);
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));

			var xml = new XmlDocument();
			xml.Load(writer.XMLCustomerReportName);
			var volumeResults =
				xml.SelectNodes(
					".//*[local-name()='Result']/*[local-name()='Fuel']/*[local-name()='FuelConsumption' and @unit='l/m³-km']");
			Assert.IsTrue(volumeResults.Count > 0);
		}


		public IVectoRun[] ReadDeclarationJob(string jobfile)
		{
			var filename = Path.Combine(@"TestData\XML\XMLReaderDeclaration", jobfile);

			var fileWriter = new FileOutputWriter(filename);
			//var sumWriter = new SummaryDataContainer(fileWriter);
			//var jobContainer = new JobContainer(sumWriter);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = false,
				ActualModalData = false,
				Validate = false,
			};

			var runs = runsFactory.SimulationRuns().ToArray();
			Assert.IsTrue(runs.Length > 0);

			return runs;

			//var customerRecord = fileWriter.XMLCustomerReportName;
			//var manufacturerRecord = fileWriter.XMLFullReportName;

			//var validationMsg1 = new List<string> { customerRecord };

			//var validator1 = new XMLValidator(XmlReader.Create(customerRecord), validationErrorAction: (s, e) => {
			//	validationMsg1.Add(e.ValidationEventArgs.Message);
			//});
			//Assert.IsTrue(validator1.ValidateXML(VectoCore.Utils.XmlDocumentType.CustomerReport), string.Join("\n", validationMsg1));

			//var validationMsg2 = new List<string> { manufacturerRecord };
			//var validator2 = new XMLValidator(XmlReader.Create(manufacturerRecord), validationErrorAction: (s, e) => {
			//	validationMsg2.Add(e.ValidationEventArgs.Message);
			//});
			//Assert.IsTrue(validator2.ValidateXML(VectoCore.Utils.XmlDocumentType.ManufacturerReport), string.Join("\n", validationMsg2));
		}
	}
}

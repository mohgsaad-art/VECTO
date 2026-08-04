using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.XML
{
    [TestFixture]
    public class XMLDeclarationInputv27WheelBearings
    {
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		private const string BASE_DIR = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.7/WheelBearings/";
		
        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
            xmlInputReader = _kernel.Get<IXMLInputDataReader>();
        }

		[
		TestCase(@"Conventional_heavyLorry_AMT.xml", BASE_DIR, 3.2, double.NaN, -3.2, TestName="WheelBearings_ConventionalLorry"),
		TestCase(@"No_wheelBearings_Conventional_heavyLorry_AMT.xml", BASE_DIR, double.NaN, double.NaN, 0, TestName="WheelBearings_None"),
		TestCase(@"HEV_heavyLorry_AMT_Px.xml", BASE_DIR, 3.1, double.NaN, -3.4, TestName="WheelBearings_HEVLorry"),
		TestCase(@"PEV_heavyLorry_AMT_E2.xml", BASE_DIR, 3.0, double.NaN, -3.6, TestName="WheelBearings_PEVLorry"),
		TestCase(@"Conventional_primaryBus_AMT.xml", BASE_DIR, 2.5, double.NaN, -4.6, TestName="WheelBearings_PrimaryBus")
		]
		public void TestWheelBearingsInput(string jobfile, string testDir, double friction0, double friction1, double delta)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			
			Assert.NotNull(dataProvider);

			var axlesDec = dataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration;

			Assert.AreEqual(friction0, axlesDec[0].WheelEndFriction?.Value() ?? double.NaN);
			Assert.AreEqual(friction1, axlesDec[1].WheelEndFriction?.Value() ?? double.NaN);

			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, null, null, null, false);
            var deltaFriction = runsFactory.RunDataFactory.NextRun().First().WheelEndData.DeltaFrictionTorque;
			
			Assert.AreEqual(delta, deltaFriction.Value(), 1E-03);
		}

		[
		TestCase(@"VehicleDrivenWrong_Conventional_heavyLorry_AMT.xml", BASE_DIR, "VehicleDriven", TestName="WheelBearings_VehicleDriven"),
		TestCase(@"NegativeFriction_Conventional_heavyLorry_AMT.xml", BASE_DIR, "MinInclusive", TestName="WheelBearings_NegativeFriction"),
		TestCase(@"FrictionTooBig_Conventional_heavyLorry_AMT.xml", BASE_DIR, "greater than", TestName="WheelBearings_FrictionTooBig"),
		]
		public void TestBadWheelBearingsInput(string jobfile, string testDir, string keyword)
		{
			var filename = Path.Combine(testDir, jobfile);
			
			var exception = Assert.Throws<VectoException>(
				() => { 
					var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
					var axlesDec = dataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration;
					var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, null, null, null, false);
                    runsFactory.RunDataFactory.NextRun().First();
				});
				
			TestContext.WriteLine(exception.Message);
		 	Assert.IsTrue(exception.Message.Contains(keyword));
		}

		[
		TestCase(@"Conventional_heavyLorry_AMT.xml", BASE_DIR, -1.0842, TestName = "WheelBearings_FullRun_ConventionalLorry"),
		TestCase(@"disallowed_class_vehicle.xml", BASE_DIR, 0, TestName = "WheelBearings_Disallowed_class")
		]
		public void TestWheelEndEnergySaved(string jobfile, string testDir, double energy)
		{ 
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			string outputFile = InputDataHelper.CreateUniqueSubfolder(filename);
			var writer = new FileOutputWriter(outputFile);

			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, writer, null, null, false);
            runsFactory.WriteModalResults = false;
			runsFactory.SumData = new SummaryDataContainer(writer);

			var jobContainer = new JobContainer(runsFactory.SumData);
			jobContainer.AddRuns(runsFactory);
				
			jobContainer.Execute();
			jobContainer.WaitFinished();
		
			Assert.IsTrue(jobContainer.AllCompleted);
			Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));

			AssertHelper.AssertSumFromSummaryFileData(writer.SumFileName, SumDataFields.E_WHEELEND_SAVED, energy);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
		}

	}
}

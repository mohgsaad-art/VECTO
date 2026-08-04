using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.Multistage
{
	[TestFixture]
	[NonParallelizable]
	public class MultistageMultipleRunsTest
	{
		private const string TestDataDir = "TestData//Integration//Multistage//";

		private const string CompletedDiesel = TestDataDir + "newVifCompletedConventional.vecto";
		private const string CompletedExempted = TestDataDir + "newVifExempted.vecto";
		private const string CompletedExemptedWithoutTPMLM = TestDataDir + "newVifExempted-noTPMLM.vecto";
		private string CompletedWithoutADAS = TestDataDir + "newVifCompletedConventional-noADAS.vecto";






		private const string InterimExempted = TestDataDir + "newVifExemptedIncomplete.vecto";
		private const string InterimDiesel = TestDataDir + "newVifInterimDiesel.vecto";

		private FileOutputWriter _sumFileWriter;
		private SummaryDataContainer _sumContainer;
		private JobContainer _jobContainer;

		private string _outputDirectory;

		private Stopwatch _stopWatch;

		private IKernel _kernel;
		private ISimulatorFactoryFactory _simFactoryFactory;


		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

		}

		[SetUp]
		public void SetUp()
		{
			_stopWatch = new Stopwatch();
			_stopWatch.Start();
			_outputDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory,TestContext.CurrentContext.Test.Name);
			_sumFileWriter = new FileOutputWriter(_outputDirectory);
			_sumContainer = new SummaryDataContainer(_sumFileWriter);
			_jobContainer = new JobContainer(_sumContainer);
			_kernel = new StandardKernel(new VectoNinjectModule());
			_simFactoryFactory = _kernel.Get<ISimulatorFactoryFactory>();
		}

		[TearDown]
		public void TearDown()
		{
			_stopWatch.Stop();
			TestContext.WriteLine($"Execution time: {_stopWatch.Elapsed}");
		}

		[NonParallelizable]
		[Test]//, Timeout(3000)]
		public void ExemptedPrimaryAndCompletedTest()
		{
			var fileOutputWriter = new FileOutputWriter(_outputDirectory);
			var tempFileOutputWriter = new TempFileOutputWriter(fileOutputWriter);
            StartSimulation(CompletedExempted, tempFileOutputWriter, fileOutputWriter);

            var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLMultistageReportFileName));
		}

		[NonParallelizable]
		[Test]
		public void ExemptedPrimaryAndCompletedWithoutTPMLMTest()
		{
			var fileOutputWriter = new FileOutputWriter(_outputDirectory);
			var tempFileOutputWriter = new TempFileOutputWriter(fileOutputWriter);
			StartSimulation(CompletedExemptedWithoutTPMLM, tempFileOutputWriter, fileOutputWriter);

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLMultistageReportFileName));
		}

		[NonParallelizable]
		[Test]//, Timeout(3000)]
		public void ExemptedPrimaryAndInterimTest()
		{
			var fileOutputWriter = new FileOutputWriter(_outputDirectory);
			var tempFileOutputWriter = new TempFileOutputWriter(fileOutputWriter);
            StartSimulation(InterimExempted, tempFileOutputWriter, fileOutputWriter); 
			
			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(tempFileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(fileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(fileOutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLMultistageReportFileName));

		}

		private IList<string> GetWrittenFiles()
		{
			var files = new List<string>();
			var outputWriters = _jobContainer.GetOutputDataWriters();
			foreach (var outputDataWriter in outputWriters) {
				files.AddRange(outputDataWriter.GetWrittenFiles().Values);
			}

			return files;
		}

		//SpecialCase II
		[NonParallelizable]
		[Test]
		public void PrimaryAndCompletedTest()
		{
			var fileOutputWriter = new FileOutputWriter(_outputDirectory);
			var tempFileOutputWriter = new TempFileOutputWriter(fileOutputWriter);
			StartSimulation(CompletedDiesel, tempFileOutputWriter, fileOutputWriter);

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLMultistageReportFileName));


		}

		[NonParallelizable]
		[Test]
		public void PrimaryAndCompletedWithoutADASAndTPMLM()
		{
			var fileOutputWriter = new FileOutputWriter(_outputDirectory);
			var tempFileOutputWriter = new TempFileOutputWriter(fileOutputWriter);
            StartSimulation(CompletedWithoutADAS, tempFileOutputWriter, fileOutputWriter);

            var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLMultistageReportFileName));

		}

		//SpecialCase I
		[NonParallelizable]
		[Test]
		public void PrimaryAndInterimTest()
		{

			var fileOutputWriter = new FileOutputWriter(_outputDirectory);
			var tempFileOutputWriter = new TempFileOutputWriter(fileOutputWriter);
			var input = JSONInputDataFactory.ReadJsonJob(InterimDiesel) as JSONInputDataV10_PrimaryAndStageInputBus;
			Assert.IsTrue(File.Exists(input.PrimaryVehicleInputDataPath));
			Assert.IsTrue(File.Exists(input.StageInputDataPath));



            StartSimulation(InterimDiesel, tempFileOutputWriter, fileOutputWriter);


			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);
			Assert.IsTrue(writtenFiles.Contains(tempFileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(fileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(fileOutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(fileOutputWriter.XMLMultistageReportFileName));
		}

		private void StartSimulation(string path, TempFileOutputWriter tempFileOutputWriter, FileOutputWriter fileOutputWriter, bool multithreaded = true)
		{
			var inputFile = Path.GetFullPath(path);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);
			
			
			StartSimulation(input, tempFileOutputWriter, fileOutputWriter, multithreaded:multithreaded);

			_jobContainer.WaitFinished();
		}


		private void StartSimulation(IInputDataProvider input,  TempFileOutputWriter tempFileOutputWriter, FileOutputWriter fileOutputWriter, bool multithreaded = true)
		{
			var runsFactory = _simFactoryFactory.Factory(ExecutionMode.Declaration, input, fileOutputWriter, null, null, true);
			
			runsFactory.WriteModalResults = true;
			runsFactory.ModalResults1Hz = true;
			runsFactory.Validate = true;
			runsFactory.ActualModalData = true;
			runsFactory.SerializeVectoRunData = true;

			_jobContainer.AddRuns(runsFactory);
			_jobContainer.Execute(multithreaded);
		}

		private void ShowWrittenFiles(IList<string> writtenFiles)
		{
			TestContext.WriteLine("Written Files:");
			if (writtenFiles.Count == 0)
			{
				TestContext.WriteLine("No Files Written");
			}
			foreach (var fileName in writtenFiles)
			{
				TestContext.WriteLine(fileName);
			}
		}



	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.Multistage
{
	[TestFixture]
	public class MultistageMultipleRunsTest
	{
		private const string TestDataDir = "TestData\\Integration\\Multistage\\";

		private const string CompletedDiesel = TestDataDir + "newVifCompletedConventional.vecto";
		private const string CompletedExempted = TestDataDir + "newVifExempted.vecto";
		private const string CompletedExemptedWithoutTPMLM = TestDataDir + "newVifExempted-noTPMLM.vecto";
		private string CompletedWithoutADAS = TestDataDir + "newVifCompletedConventional-noADAS.vecto";






		private const string InterimExempted = TestDataDir + "newVifExemptedIncomplete.vecto";
		private const string InterimDiesel = TestDataDir + "newVifInterimDiesel.vecto";

		private FileOutputWriter _sumFileWriter;
		private SummaryDataContainer _sumContainer;
		private JobContainer _jobContainer;

		private FileOutputWriter _fileoutputWriter;
		private TempFileOutputWriter _tempFileOutputWriter;
		private ExecutionMode _mode = ExecutionMode.Declaration;

		private string _outputDirectory;

		private Stopwatch _stopWatch;

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
		}

		[TearDown]
		public void TearDown()
		{
			_stopWatch.Stop();
			TestContext.WriteLine($"Execution time: {_stopWatch.Elapsed}");
		}

		[Test]//, Timeout(3000)]
		public void ExemptedPrimaryAndCompletedTest()
		{

			StartSimulation(CompletedExempted);

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));
		}

		[Test]
		public void ExemptedPrimaryAndCompletedWithoutTPMLMTest()
		{

			StartSimulation(CompletedExemptedWithoutTPMLM);

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));
		}

		[Test]//, Timeout(3000)]
		public void ExemptedPrimaryAndInterimTest()
		{
			StartSimulation(InterimExempted);
			
			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));

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
		[Test, Timeout(1000 * 20 * 60)]
		public void PrimaryAndCompletedTest()
		{
			StartSimulation(CompletedDiesel);

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));


		}

		[Test]
		public void PrimaryAndCompletedWithoutADASAndTPMLM()
		{
			StartSimulation(CompletedWithoutADAS);

			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));

		}

		//SpecialCase I
		[Test, Timeout(1000 * 10 * 60)]
		public void PrimaryAndInterimTest()
		{
			StartSimulation(InterimDiesel);


			var writtenFiles = GetWrittenFiles();
			ShowWrittenFiles(writtenFiles);

			Assert.IsTrue(writtenFiles.Contains(_tempFileOutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLFullReportName));
			Assert.IsFalse(writtenFiles.Contains(_fileoutputWriter.XMLCustomerReportName));
			Assert.IsTrue(writtenFiles.Contains(_fileoutputWriter.XMLMultistageReportFileName));
		}

		private void StartSimulation(string path)
		{
			var inputFile = Path.GetFullPath(path);
			var input = JSONInputDataFactory.ReadJsonJob(inputFile);


			StartSimulation(input);

			_jobContainer.WaitFinished();
		}


		private void StartSimulation(IInputDataProvider input)
		{
			_fileoutputWriter = new FileOutputWriter(_outputDirectory);
			_tempFileOutputWriter = new TempFileOutputWriter(_fileoutputWriter);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(_mode, input, _fileoutputWriter);
			runsFactory.WriteModalResults = true;
			runsFactory.ModalResults1Hz = true;
			runsFactory.Validate = true;
			runsFactory.ActualModalData = true;
			runsFactory.SerializeVectoRunData = true;

			var timeout = 1000;


			_jobContainer.AddRuns(runsFactory);
			_jobContainer.Execute();
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.Integration
{
    /// <summary>
    /// Make sure all shipped generic vehicles actually can be simulated
    /// </summary>
	[Ignore("New solution provided by GenericVehiclesDeclarationTests.")]
	[TestFixture]
	//[Parallelizable]
	internal class GenericVehicleTest
    {
		private static string BASE_DIR = "TestData/Shipped_Generic";
		private StandardKernel _kernel;
		private IXMLInputDataReader _xmlReader;

		private bool _simulate = false;
		private static HashSet<string> _ignoredFiles = new HashSet<string>()
		{
			@"Engineering Mode\GenericVehicleE2\BEV_ENG_PTO_invalid.vecto", //INVALID Input file 
			//@"Declaration Mode\CompletedBus 31b2\airdrag.xml" //no job
        };

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_kernel = new StandardKernel(new VectoNinjectModule());
			_xmlReader = _kernel.Get<IXMLInputDataReader>();
			
		}


		[Test, TestCaseSource(nameof(GetJSONEngineering))]
		public void GenericVehiclesEngineering(string path)
		{
			PrepareJSONSimulation(path, ExecutionMode.Engineering, out _, out _);
		}


		private void PrepareJSONSimulation(string path, ExecutionMode executionMode, out JobContainer jobContainer,
			out ISimulatorFactory runsFactory)
		{
			TestContext.Progress.WriteLine($"Preparing {path} ...");
			if (IgnoreFiles(path, out var reason)) {
				Assert.Ignore(reason);
			}
			var writeReports = true;
			var inputData = JSONInputDataFactory.ReadJsonJob(path, false);

			var fileWriter = new FileOutputWriter(path);
			runsFactory = SimulatorFactory.CreateSimulatorFactory(executionMode, inputData, fileWriter,
				writeReports ? null : new NullDeclarationReport()); //, writeReports ? null : new NullDeclarationReport());
			//DisableIterativeRuns(runsFactory);
			runsFactory.WriteModalResults = false;
			var sumWriter = new SummaryDataContainer(fileWriter); //new MockSumWriter();

			jobContainer = new JobContainer(sumWriter);
			runsFactory.SumData = sumWriter;
			//var sumDataContainer = sumWriter;

			jobContainer.AddRuns(runsFactory);


        }


		[Test, TestCaseSource(nameof(GetJSONEngineering))]
        public void SimulateEngineering(string path)
		{
			PrepareJSONSimulation(path, ExecutionMode.Engineering, out var jobContainer, out var runsFactory);


			jobContainer.Execute(true);
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success));
		}

		[Test, TestCaseSource(nameof(GetJSONDeclaration)), TestCaseSource(nameof(GetXMLDeclaration))]
        public void SimulateDeclaration(string path)
		{
			PrepareDeclarationSimulation(path, out var jobContainer, out _);
			jobContainer.Execute(true);
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success));

        }

		[Test, TestCaseSource(nameof(GetJSONDeclaration)), TestCaseSource(nameof(GetXMLDeclaration))]
		public void GenericVehiclesDeclaration(string path)
		{
			PrepareDeclarationSimulation(path, out _, out _);
		}

		public void PrepareDeclarationSimulation(string path, out JobContainer jobContainer, out ISimulatorFactory runsFactory)
		{
			if (IgnoreFiles(path, out var reason))
			{
				Assert.Ignore(reason);
			}
			if (path.EndsWith(".vecto"))
			{
				PrepareJSONSimulation(path, ExecutionMode.Declaration, out jobContainer, out runsFactory);
			}
			else
			{
				TestContext.Progress.WriteLine($"Running {path} ...");
				var writeReports = true;



				var inputData = _xmlReader.CreateDeclaration(path);
				var fileWriter = new FileOutputWriter(path);

				if (IgnoreInputData(inputData, out reason))
				{
					Assert.Ignore(reason);
				};

				if (inputData.DataSource.Type == "VectoOutputMultistepType")
				{
					var busInputData = inputData as IMultistepBusInputDataProvider;
					var multistepInputData = new XMLDeclarationVIFInputData(busInputData, null);
					fileWriter = new FileOutputVIFWriter(path, busInputData.JobInputData.ManufacturingStages?.Count ?? 0);
					runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, multistepInputData, fileWriter, null, null, true);
				}
				else
				{

					runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter, writeReports ? null : new NullDeclarationReport()); 
					//, writeReports ? null : new NullDeclarationReport());
				}

				var sumWriter = new SummaryDataContainer(fileWriter); //new MockSumWriter();
				runsFactory.WriteModalResults = false;
				jobContainer = new JobContainer(sumWriter);
				runsFactory.SumData = sumWriter;
				//var sumDataContainer = sumWriter;

				jobContainer.AddRuns(runsFactory);
				if (_simulate)
				{
					jobContainer.Execute(true);
					jobContainer.WaitFinished();
					Assert.IsTrue(jobContainer.Runs.All(r => r.Success));
				}
			}
        }


		public static List<string> GetJSONEngineering()
		{
			return GetFiles("Engineering Mode", "*.vecto");
		}

		public static List<string> GetJSONDeclaration()
		{
			return GetFiles("Declaration Mode", "*.vecto");
        }

		public static List<string> GetXMLDeclaration()
		{
			return GetFiles("Declaration Mode", "*.xml");
		}
		private static List<string> GetFiles(string path, string searchPattern)
		{
			var dirPath = Path.Combine(BASE_DIR, path);
			List<string> vectoJobs = new List<string>();
			foreach (var fileName in Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.AllDirectories))
			{
				if (IgnoreFiles(path, out _)) {
					continue;
				}
			
				vectoJobs.Add(fileName);
			}


			return vectoJobs;
		}

		/// <summary>
		/// Check if a file should be ignored based on path of the file
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		static bool IgnoreFiles(string path, out string reason)
		{
			reason = null;
			if (_ignoredFiles.Contains(Path.GetRelativePath(BASE_DIR, path))) {
				reason = "File ignored";
				return true;

			}

			if (path.EndsWith(".xml")) {
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(path);

				var documentType = XMLHelper.GetDocumentTypeFromRootElement(xmlDoc.DocumentElement.LocalName);
				if (documentType == null) {
					reason = "File ignored - No document type set";
					return true;
				}

				if (xmlDoc.DocumentElement.NamespaceURI == "urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.0") {
					reason = $"Ignoring {xmlDoc.DocumentElement.NamespaceURI}";
					return true;
				}

				switch (documentType.Value) {
					case XmlDocumentType.DeclarationJobData:
					case XmlDocumentType.MultistepOutputData:
						break;

					case XmlDocumentType.EngineeringJobData:
						throw new Exception("Engineering xml still a thing?");
						break;
					case XmlDocumentType.EngineeringComponentData:
					case XmlDocumentType.DeclarationComponentData:
                    case XmlDocumentType.ManufacturerReport:
					case XmlDocumentType.CustomerReport:
					case XmlDocumentType.MonitoringReport:
					case XmlDocumentType.VTPReport:
						reason = $"{documentType} ignored";
						return true;
					default:
						throw new ArgumentOutOfRangeException();
				}

			}

            //if (path.Contains("RSLT_MANUFACTURER") || path.Contains("RSLT_CUSTOMER")) {
            //	return true;
            //}


            return false;
		}

		static bool IgnoreInputData(IDeclarationInputDataProvider inputData, out string reason)
		{
			reason = null;



			if (inputData.DataSource.Type != "VectoOutputMultistepType" && inputData.JobInputData?.Vehicle is IXMLResource xmlResource) {
				if (xmlResource.DataSource.Type.Contains("CompletedBus", StringComparison.InvariantCultureIgnoreCase)) {
					reason = $"Ignore xml type {xmlResource.DataSource.Type}";
					return true;
                };
			}



			return false;
		}
    }
}

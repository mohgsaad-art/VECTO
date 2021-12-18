using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = System.Xml.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML
{

	public class TrailerReferenceFactors
	{
		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		private const string SampleTrailer = @"TestData\Trailer\trailerSample.xml";

		private IDeclarationTrailerInputDataProvider inputDataProvider;


		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();

			//var reader = XmlReader.Create(SampleTrailer);
			//inputDataProvider = XMLInputReader.CreateDeclarationTrailer(reader);
		}

		public void RunTrailerDeclarationJob()
		{
			System.Environment.SetEnvironmentVariable("referenceFactorRun", "true");

			var trailerList = DeclarationData.TrailerSegments.GetActiveVehicleGroups();
			foreach (var group in trailerList) {

				System.Environment.SetEnvironmentVariable("vehicleGroup", group.ToString());

				var fileWriter = new FileOutputWriter(SampleTrailer);
				var sumWriter = new SummaryDataContainer(fileWriter);
				var jobContainer = new JobContainer(sumWriter);
				var dataProvider = XMLInputReader.CreateDeclarationTrailer(XmlReader.Create(SampleTrailer));
				var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
					ModalResults1Hz = false,
					WriteModalResults = false,
					ActualModalData = false,
					Validate = false,
				};
				jobContainer.AddRuns(runsFactory);
				jobContainer.Execute();
				jobContainer.WaitFinished();

				//var customerRecord = fileWriter.XMLCustomerReportName;
				//var manufacturerRecord = fileWriter.XMLFullReportName;

				//TODO: Calculate factors and write to csv 
			}
		}

		[TestCase]
		public void CreateReferenceFactors()
		{
			//TODO: Create loop for all active vehicle groups and foreach create factors:
			RunBeforeAnyTests();
			RunTrailerDeclarationJob();
		}

	}
}

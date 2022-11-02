/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
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

		
		[TestCase(@"TestData\Integration\MediumLorries\vecto_vehicle-medium_lorry-Van.xml", TestName = "Medium Lorry Van"),
		TestCase(@"TestData\Integration\MediumLorries\vecto_vehicle-medium_lorry.xml", TestName = "Medium Lorry"),
		//TestCase(@"TestData\Integration\MediumLorries\vecto_vehicle-medium_lorryFWD.xml", TestName = "Medium Lorry FWD"), // FWD no longer supported / required
        ]
		public void CreateRunDataMediumLorry(string jobFile)
		{
			//var jobFile = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.6_Buses\vecto_vehicle-medium_lorry-sample.xml";

			var writer = new FileOutputWriter(jobFile);
			var inputData = xmlInputReader.CreateDeclaration(jobFile);
			
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;
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
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = false;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

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

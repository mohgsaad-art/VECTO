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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.Integration.VTP
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class VTPTest
	{
		[OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[
			TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle.vecto"),
			TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle_noGear.vecto")
		 //TestCase(@"TestData\Integration\VTPMode\HeavyBus\VTP_PrimaryBus_ENG.vecto", TestName = "RunVTPHeavyPrimaryBus Engineering")
		]
		[Ignore("Fix this when testing VTP mode.")]
		public void RunVTP(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(1, jobContainer.Runs.Count);
			//var i = 0;
			//jobContainer.Runs[i].Run.Run();
			//Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[
			TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle_DECL.vecto", 45.6, 0.8972, TestName = "RunVTPHeavyLorry_Declaration"),
			TestCase(@"TestData\Integration\VTPMode\MediumLorry\VTP_MediumLorry.vecto", 400.0, 1.06, TestName = "RunVTPMediumLorry_Declaration"),
			TestCase(@"TestData\Integration\VTPMode\DualFuelVehicle\VTP_DualFuel.vecto", 43.5, 1.0107, TestName = "RunVTPDualFuel_Declaration"),
			TestCase(@"TestData\Integration\VTPMode\HeavyBus\VTP_PrimaryBus.vecto", 14.2, 1.1413, TestName = "RunVTPHeavyPrimaryBus")
		]
		[Ignore("Fix this when testing VTP mode.")]
		public void RunVTP_Declaration(string jobFile, double expectedDeclaredCO2, double expectedCVTP)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(1, jobContainer.Runs.Count);
			//var i = 0;
			//jobContainer.Runs[i].Run.Run();
			//Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var vtpReport = fileWriter.XMLVTPReportName;
			var validator = new XMLValidator(XmlReader.Create(vtpReport));
			validator.ValidateXML(XmlDocumentType.VTPReport);
			Assert.IsNull(validator.ValidationError);

			var vtpXml = XDocument.Load(vtpReport);

			Assert.AreEqual(expectedDeclaredCO2, vtpXml.Document?.XPathSelectElement("//*[local-name()='Declared']")?.Value.ToDouble(), 1e-1);
			Assert.AreEqual(expectedCVTP, vtpXml.Document?.XPathSelectElement("//*[local-name()='C_VTP']")?.Value.ToDouble(), 1e-4);
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle_CNG\class_5_generic vehicle_DECL.vecto")]
		[Ignore("Fix this when testing VTP mode.")]
		public void RunVTP_Declaration_NCVCorrection_CNG(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(1, jobContainer.Runs.Count);
			//var i = 0;
			//jobContainer.Runs[i].Run.Run();
			//Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var vtpReport = XDocument.Load(XmlReader.Create(fileWriter.XMLVTPReportName));
			var vtpFactor = vtpReport.XPathSelectElement("//*[local-name() = 'Results']/*[local-name() = 'C_VTP']")?.Value.ToDouble(0);

			Assert.AreEqual(0.9549, vtpFactor);
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle\VTP_AT-gbx.vecto")]
		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle\VTP_AT-gbx_2TC.vecto")]
		[Ignore("Fix this when testing VTP mode.")]
		public void RunVTPWithAT(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(1, jobContainer.Runs.Count);
			var i = 0;
			jobContainer.Runs[i].Run.Run();
			Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);
		}
	}
}
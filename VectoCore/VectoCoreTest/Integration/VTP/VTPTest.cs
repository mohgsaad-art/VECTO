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

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
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
        private StandardKernel _kernel;

        [OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            _kernel = new StandardKernel(new VectoNinjectModule());
        }

		[TestCase(@"TestData/Integration/VTPMode/GenericVehicle/class_5_generic vehicle.vecto"),
		 TestCase(@"TestData/Integration/VTPMode/GenericVehicle/class_5_generic vehicle_noGear.vecto"),
		 TestCase(@"TestData/Integration/VTPMode/HeavyBus/VTP_Simulation_ENG.vecto", TestName = "RunVTPHeavyBus Engineering")
		]
		public void RunVTP(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, dataProvider, fileWriter, null, null, false);
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

		private const string STOP_START_JOB = @"TestData\Integration\VTPMode\Group2_RigidTruck_4x2\Class2_RigidTruck_DECL_SS_VTP.vecto";
		private const string POLLUTANTS_JOB = @"TestData\Integration\VTPMode\Group2_RigidTruck_4x2\Class2_RigidTruck_VTP_pollutants.vecto";
		private const string PEL_FAN_JOB = @"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle_DECL_FanPel.vecto";
		private const string DUAL_FUEL_JOB = @"TestData\Integration\VTPMode\DualFuelVehicle\VTP_DualFuel.vecto";
		private const string TORQUE_DRIFT_JOB = @"TestData\Integration\VTPMode\Group2_RigidTruck_4x2\Class2_RigidTruck_VTP_TorqueDrift.vecto";
		private const string ARCHIVE_BUILDER_JOB = @"TestData\Integration\VTPMode\JobArchiveBuilder\VTP_MediumLorry.vecto";
		private const string MRF_v09_JOB = @"TestData\Integration\VTPMode\MRF_v0.9\VTP_DualFuel.vecto";

		[Category("LongRunning")]
		[Category("Integration")]
		[
			TestCase(@"TestData/Integration/VTPMode/GenericVehicle/class_5_generic vehicle_DECL.vecto", 45.6, 0.8615, TestName = "RunVTPHeavyLorry_Declaration"),
			TestCase(@"TestData/Integration/VTPMode/MediumLorry/VTP_MediumLorry.vecto", 400.0, 1.0735, TestName = "RunVTPMediumLorry_Declaration"),
			TestCase(@"TestData/Integration/VTPMode/HeavyBus/VTP_simulation.vecto", 12, 1.0026, TestName = "RunVTPHeavyBus"),
			TestCase(@"TestData/Integration/VTPMode/GenericVehicle XMLJob PTO/class_5_generic vehicle_DECL.vecto", 45.6, 0.8592, TestName = "Generic Group 5 VTP Test Declaration Mode with PTO"),
			TestCase(@"TestData/Integration/VTPMode/GenericVehicle/class_3_generic vehicle_DECL.vecto", 126, 0.9668, TestName = "Generic Group 3 VTP Test Declaration Mode"),
			TestCase(STOP_START_JOB, 188, 1.0099, TestName = "VTP StopStart"),
			TestCase(PEL_FAN_JOB, 45.6, 0.8968, TestName = "VTP Fan Electrical Power"),
			TestCase(POLLUTANTS_JOB, 188, 1.0082, TestName = "VTP Pollutants"),
			TestCase(DUAL_FUEL_JOB, 43.5, 1.0148, TestName = "VTP Dual Fuel"),
			TestCase(MRF_v09_JOB, 42.4, 1.0148, TestName = "VTP MRF v09"),
			TestCase(TORQUE_DRIFT_JOB, 188, 1.0082, TestName = "VTP Torque Drift")
		]
		public void RunVTP_Declaration(string jobFile, double expectedDeclaredCO2, double expectedCVTP)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null, false);
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

			var declared = vtpXml.Document?.XPathSelectElement("//*[local-name()='Declared']")?.Value.ToDouble();
			var cvtp = vtpXml.Document?.XPathSelectElement("//*[local-name()='C_VTP']")?.Value.ToDouble();
			var status = vtpXml.Document?.XPathSelectElement("//*[local-name()='Status']")?.Value;

			TestContext.WriteLine($"declared CO2 = {declared}, cvtp = {cvtp}");

			Assert.That(declared, Is.EqualTo(expectedDeclaredCO2).Within(1e-8));
            Assert.That(cvtp, Is.EqualTo(expectedCVTP).Within(1e-4));
            Assert.AreEqual(status, (cvtp < 1.075) ? "Passed" : "Failed");
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(ARCHIVE_BUILDER_JOB, TestName = "VTP Job Archive creation")]
		public void TestVTPJobArchiveCreation(string jobFile)
		{ 
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter, new JobArchiveBuilder());
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null, false);
            runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var archivePath = Path.Combine(Path.GetDirectoryName(jobFile), "JobArchive_VTP_MediumLorry.zip");
			Assert.IsTrue(File.Exists(archivePath));

			var fileNames = new List<string>() {
				"VTP_MediumLorry.VTP_Report.xml",
				"VTP_MediumLorry.vsum",
				"standard values_VTP Testzyklus.vmod",
				"VTP_MediumLorry.vecto",
				"vecto_vehicle-medium_lorry_4x2.RSLT_MANUFACTURER.xml",
				"vecto_vehicle-medium_lorry_4x2.xml",
				"VTP Testzyklus.vdri"
			};

			var zipFiles = new List<string>();

			using (ZipArchive archive = ZipFile.OpenRead(archivePath))
			{
				foreach (ZipArchiveEntry entry in archive.Entries) { zipFiles.Add(entry.FullName); }
			} 

			Assert.IsTrue(fileNames.Count == zipFiles.Count);
			Assert.IsTrue(fileNames.All(x => zipFiles.Contains(x)));
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(@"TestData/Integration/VTPMode/GenericVehicle_CNG/class_5_generic vehicle_DECL.vecto")]
		public void RunVTP_Declaration_NCVCorrection_CNG(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null, false);
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

			Assert.AreEqual(0.8157, vtpFactor);
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(@"TestData/Integration/VTPMode/GenericVehicle/VTP_AT-gbx.vecto")]
		[TestCase(@"TestData/Integration/VTPMode/GenericVehicle/VTP_AT-gbx_2TC.vecto")]
		public void RunVTPWithAT(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null, false);
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
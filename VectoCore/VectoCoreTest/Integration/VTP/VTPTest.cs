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
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;

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

		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle.vecto"),
		 TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle_noGear.vecto")]
		public void RunVTP(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
			};
			
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
		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle_DECL.vecto", 0.8972, TestName = "Generic Group 5 VTP Test Declaration Mode"),
		 TestCase(@"TestData\Integration\VTPMode\GenericVehicle XMLJob PTO\class_5_generic vehicle_DECL.vecto", 0.8925, TestName = "Generic Group 5 VTP Test Declaration Mode with PTO"),
		TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_3_generic vehicle_DECL.vecto", 1.0068, TestName = "Generic Group 3 VTP Test Declaration Mode"),
		TestCase(@"TestData\Integration\VTPMode\Group2_RigidTruck_4x2\Class2_RigidTruck_DECL_SS_VTP.vecto", 1.0099, TestName = "Class 2 RigidTruck VTP StopStart"),
		TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle_DECL_FanPel.vecto", 0.8968, TestName = "Generic Group 5 VTP Test Declaration Mode Fan Electrical Power"),
		TestCase(@"TestData\Integration\VTPMode\GenericVehicle\class_5_generic vehicle_DECL_pollutants.vecto", 0.8972, TestName = "Generic Group 5 VTP Test Declaration Mode Pollutnats")
		]
		public void RunVTP_Declaration(string jobFile, double expectedVTPFactor)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
				ValidateComponentHashes = false
			};

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(2, jobContainer.Runs.Count);
			//var i = 0;
			//jobContainer.Runs[i].Run.Run();
			//Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var vtpReport = XDocument.Load(XmlReader.Create(fileWriter.XMLVTPReportName));
			var vtpFactor = vtpReport.XPathSelectElement("//*[local-name() = 'Results']/*[local-name() = 'VTRatio']")?.Value.ToDouble(0);

			Assert.AreEqual(expectedVTPFactor, vtpFactor);
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle_CNG\class_5_generic vehicle_DECL.vecto")]
		public void RunVTP_Declaration_NCVCorrection_CNG(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
				ValidateComponentHashes = false
			};

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(2, jobContainer.Runs.Count);
			//var i = 0;
			//jobContainer.Runs[i].Run.Run();
			//Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var vtpReport = XDocument.Load(XmlReader.Create(fileWriter.XMLVTPReportName));
			var vtpFactor = vtpReport.XPathSelectElement("//*[local-name() = 'Results']/*[local-name() = 'VTRatio']")?.Value.ToDouble(0);

			Assert.AreEqual(0.8972, vtpFactor);
		}



		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle\VTP_AT-gbx.vecto")]
		[TestCase(@"TestData\Integration\VTPMode\GenericVehicle\VTP_AT-gbx_2TC.vecto")]
		public void RunVTPWithAT(string jobFile)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
				ValidateComponentHashes = false
			};

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(2, jobContainer.Runs.Count);
			var i = 0;
			jobContainer.Runs[i].Run.Run();
			Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);
		}
	}
}
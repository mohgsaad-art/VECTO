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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using NUnit.Framework;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class SimulationTests
	{
		private const string EngineOnlyJob = @"TestData\Jobs\EngineOnlyJob.vecto";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase]
		public void TestSimulationEngineOnly()
		{
			var resultFileName = "TestEngineOnly-result.vmod";
			var job = CreateRun(resultFileName);

			var container = job.GetContainer();

			Assert.AreEqual(560.RPMtoRad(), container.EngineInfo.EngineSpeed);
		}

		[TestCase]
		public void TestEngineOnly_JobRun()
		{
			var actual = @"TestData\Jobs\EngineOnlyJob_Coach Engine Only short.vmod";
			var expected = @"TestData\Results\EngineOnlyCycles\EngineOnlyJob_Coach Engine Only short.vmod";

			var job = CreateRun(actual);
			job.Run();

			Assert.IsTrue(job.FinishedWithoutErrors);

			ResultFileHelper.TestModFile(expected, actual);
		}

		[TestCase]
		public void TestEngineOnly_SimulatorRun()
		{
			var actual = @"TestData\Jobs\EngineOnlyJob_Coach Engine Only short.vmod";
			var expected = @"TestData\Results\EngineOnlyCycles\EngineOnlyJob_Coach Engine Only short.vmod";

			var run = CreateRun(actual);

			var jobContainer = new JobContainer(new MockSumWriter());
			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			foreach (var r in jobContainer.Runs) {
				Assert.IsTrue(r.Run.FinishedWithoutErrors, string.Format("{0}", r.ExecException));
			}

			ResultFileHelper.TestModFile(expected, actual);
		}

		public IVectoRun CreateRun(string resultFileName)
		{
			var fileWriter = new FileOutputWriter(resultFileName);
			var sumWriter = new SummaryDataContainer(fileWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineOnlyJob);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter) {
				SumData = sumWriter
			};

			return factory.SimulationRuns().First();
		}

		[TestCase]
		public void Test_VectoJob()
		{
			var jobFile = @"TestData\Jobs\24t Coach EngineOnly.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			foreach (var run in jobContainer.Runs) {
				Assert.IsTrue(run.Run.FinishedWithoutErrors, string.Format("{0}", run.ExecException));
			}

			ResultFileHelper.TestSumFile(@"TestData\Results\EngineOnlyCycles\24t Coach EngineOnly.vsum",
				@"TestData\Jobs\24t Coach EngineOnly.vsum");

			ResultFileHelper.TestModFiles(new[] {
					@"TestData\Results\EngineOnlyCycles\24t Coach EngineOnly_Engine Only1.vmod",
					@"TestData\Results\EngineOnlyCycles\24t Coach EngineOnly_Engine Only2.vmod",
					@"TestData\Results\EngineOnlyCycles\24t Coach EngineOnly_Engine Only3.vmod"
				}, new[] {
					@"TestData\Jobs\24t Coach EngineOnly_Engine Only1.vmod",
					@"TestData\Jobs\24t Coach EngineOnly_Engine Only2.vmod",
					@"TestData\Jobs\24t Coach EngineOnly_Engine Only3.vmod"
				})
				;
		}
	}
}
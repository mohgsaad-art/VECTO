using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class TorqueConverterTest
    {
        public const String P1SerialJob = @"TestData/Integration/EngineeringMode/CityBus_AT/CityBus_AT_Ser-TC_all_gears.vecto";

		[OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

        [Category("Integration")]
        [
        TestCase(P1SerialJob, 0, TestName = "Torque Converter active for all gears")
        ]
        public void RunCycle(string jobFile, int cycleIdx)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(jobFile);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer, validate: false);
			factory.WriteModalResults = false;
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
        }
    }
}

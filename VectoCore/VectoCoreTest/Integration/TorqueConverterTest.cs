using System;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class TorqueConverterTest
    {
        private StandardKernel _kernel;

        public const String P1SerialJob = @"TestData/Integration/EngineeringMode/CityBus_AT/CityBus_AT_Ser-TC_all_gears.vecto";

		[OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            _kernel = new StandardKernel(new VectoNinjectModule());
        }

        [Category("Integration")]
        [
        TestCase(P1SerialJob, 0, TestName = "Torque Converter active for all gears")
        ]
        public void RunCycle(string jobFile, int cycleIdx)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(jobFile);

			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputProvider, writer, null, null, false);
            factory.WriteModalResults = false;
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
        }
    }
}

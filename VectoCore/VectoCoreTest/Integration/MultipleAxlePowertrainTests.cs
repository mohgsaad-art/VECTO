using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MultipleAxlePowertrainTests
    {
        private StandardKernel _kernel;
        
        private const string EngineeringBasePath = @"TestData/Generic Vehicles/Engineering Mode/MultiplePowertrains";
        private const string DeclarationBasePath = @"TestData/Generic Vehicles/Declaration Mode/MultiplePowertrains";

        [OneTimeSetUp]
        public void TestInitialize()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            _kernel = new StandardKernel(new VectoNinjectModule());
        }

        [Category("Integration")]
        [
            // Engineering mode is not very reliable
            //TestCase(@$"{EngineeringBasePath}/MultipleBEV_E2_E2/MultipleBEV.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleBEV_E2_E3/MultipleBEV.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleBEV_E3_E3/MultipleBEV.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleBEV_E3_E4/MultipleBEV.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleBEV_E4_E4/BEV_ENG.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleBEV_IEPC_IEPC/Multiple_IEPC.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleFuelCellBEV_E4_E4/BEV_ENG.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleSHEV_IEPC-S_IEPC-S/Multiple_IEPC-S.vecto", ExecutionMode.Engineering),
            //TestCase(@$"{EngineeringBasePath}/MultipleSHEV_S4_S4/Multiple.vecto", ExecutionMode.Engineering),

            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E2_E2_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E2_E3_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E2_E4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E2_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E3_E3_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E3_E4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E3_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E4_E4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E4_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_IEPC_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_IEPC_IEPC_PrimaryBus_SingleGear.xml", ExecutionMode.Declaration),

            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F2_F2_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F2_F3_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F2_F4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F2_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F3_F3_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F3_F4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F3_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F4_F4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F4_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_IEPC_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),

            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S2_S2_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S2_S3_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S2_S4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S2_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S3_S3_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S3_S4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S3_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S4_S4_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_S4_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_SHEV_IEPC_IEPC_HeavyLorry.xml", ExecutionMode.Declaration),

            TestCase($@"{DeclarationBasePath}/Multiple_FCHV_F4_F4_PrimaryBus.xml", ExecutionMode.Declaration),
            TestCase($@"{DeclarationBasePath}/Multiple_PEV_E4_E4_PrimaryBus.xml", ExecutionMode.Declaration),
        ]
        public void RunDistanceJob(string jobFile, ExecutionMode mode)
        {
            var inputProvider = (mode == ExecutionMode.Engineering) 
                ? JSONInputDataFactory.ReadJsonJob(jobFile)
                : _kernel.Get<IXMLInputDataReader>().Create(jobFile);

            var fileWriter = new FileOutputWriter(jobFile);

            var factory = _kernel
                .Get<ISimulatorFactoryFactory>()
                .Factory(mode, inputProvider, fileWriter, null, null, false);
            factory.Validate = false;

            var sumWriter = new SummaryDataContainer(fileWriter);
            var jobContainer = new JobContainer(sumWriter);
            jobContainer.AddRuns(factory);

            foreach(var cycleToRun in factory.SimulationRuns())
            {
                cycleToRun.Run();

                Assert.IsTrue(cycleToRun.FinishedWithoutErrors);
            }
        }

    }
}

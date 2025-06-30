using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData;
using System.Data;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using System.Collections.Generic;
using Ninject;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Tests.Integration
{
	

    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class Auxiliaries
    {
		private IPowertrainBuilder PowertrainBuilder;
		public IModalDataFactory ModDataFactory;
        private IShiftStrategyFactory ShiftStrategyFactory;

		private const string TRACTOR_AT_JOB = @"TestData/Integration/ConventionalTimeruns/Class5_Tractor_4x2/Class5_Tractor_ENG_Aux.vecto";

        private const string E2_JOB = @"TestData\Integration\Auxiliaries\GenericVehicleE2\BEV_ENG.vecto";
        private const string IEPC_GBX3_JOB = @"TestData\Integration\Auxiliaries\GenericIEPC\IEPC_Gbx3Speed\IEPC_ENG_Gbx3.vecto";
        private const string HYBRID_IEPC_S_GBX3 = @"TestData\Integration\Auxiliaries\GenericIEPC-S\IEPC-S_Gbx3Speed\IEPC-S_ENG_Gbx3.vecto";
        private const string HYBRID_IHPC_G5_12SPEED = @"TestData\Integration\Auxiliaries\GenericIHPC\12SpeedGbx\IHPC Group 5.vecto";
        private const string HYBRID_P2_Group_5 = @"TestData\Integration\Auxiliaries\GenericVehicle_Group5_P2\P2 Group 5.vecto";
        private const string HYBRID_SerialHybrid_S2 = @"TestData\Integration\Auxiliaries\GenericVehicle_S2_Job\SerialHybrid_S2.vecto";
        private const string HYBRID_SerialHybrid_S4 = @"TestData\Integration\Auxiliaries\GenericVehicle_S4_Job\SerialHybrid_S4.vecto";

        [OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			var kernel = new StandardKernel(new VectoNinjectModule());
			PowertrainBuilder = kernel.Get<IPowertrainBuilder>();
			ModDataFactory = kernel.Get<IModalDataFactory>();
			ShiftStrategyFactory = kernel.Get<IShiftStrategyFactory>();
		}


		[Category("Integration")]
		[Ignore("Temporarily disabling fix for codeu issue 15 because it causes distance-based testcases to fail")]
        [
			TestCase(TRACTOR_AT_JOB, TestName = "Tractor AT distance Padd cycle")
        ]
        public void TestAuxiliaryLoadFromCycle(String jobFile)
        {
            var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;

			jobContainer.AddRuns(runsFactory);
			
			var modData = (jobContainer.Runs[0].Run.GetContainer().ModalData as ModalDataContainer).Data;
			
			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var row = modData.Rows.Cast<DataRow>().First();
			Assert.AreEqual(9330.SI<Watt>(), row[ModalResultField.P_aux_mech.GetName()]);
        }

        [Category("Integration")]
        [
            TestCase(E2_JOB, TestName = "GenericVehicle E2 Padd_hv"),
            TestCase(IEPC_GBX3_JOB, TestName = "IEPC Gearbox 3 speed Padd_hv"),
        ]
        public void HighVoltageAuxiliaries_IncreaseConsumption_ForPEV(string jobFile)
        {
            // Arrange.
            var fileWriter = new FileOutputWriter(jobFile);
            var sumWriter = new SummaryDataContainer(fileWriter);
            var jobContainer = new JobContainer(sumWriter);
            var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;

            jobContainer.AddRuns(runsFactory);

            // Act.
            jobContainer.Execute();
            jobContainer.WaitFinished();

            // Assert.
            Assert.AreEqual(true, jobContainer.AllCompleted);

            int cycleNameEntry = 3;
            var summaryData = sumWriter.Table.Rows.Cast<DataRow>().ToDictionary(r => r[cycleNameEntry]);

            string distanceRunCycle = "DistanceRun";
            AssertHVElectricAxuliaries(summaryData, distanceRunCycle);

            string measuredSpeedCycle = "MeasuredSpeed";
            AssertHVElectricAxuliaries(summaryData, measuredSpeedCycle);

            string measuredSpeedGearCycle = "MeasuredSpeedGear";
            AssertHVElectricAxuliaries(summaryData, measuredSpeedGearCycle);

            string pWheelCycle = "PWheel";
            AssertHVElectricAxuliaries(summaryData, pWheelCycle);
        }

        [Category("Integration")]
        [
			TestCase(HYBRID_P2_Group_5, TestName = "GenericVehicle Group5 P2 Padd_hv"),
			TestCase(HYBRID_SerialHybrid_S2, TestName = "GenericVehicle S2 Job Padd_hv"),
			TestCase(HYBRID_SerialHybrid_S4, TestName = "GenericVehicle S4 Job Padd_hv"),
			TestCase(HYBRID_IEPC_S_GBX3, TestName = "IEPC-S 3 Speed Gearbox Padd_hv"),
			TestCase(HYBRID_IHPC_G5_12SPEED, TestName = "IHPC Group 5 12 Speed Gearbox Padd_hv"),
        ]
        public void HighVoltageAuxiliaries_IncreaseConsumption_ForHybrid(string jobFile)
        {
            // Arrange.
            var fileWriter = new FileOutputWriter(jobFile);
            var sumWriter = new SummaryDataContainer(fileWriter);
            var jobContainer = new JobContainer(sumWriter);
            var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = true;
			runsFactory.ActualModalData = false;

            jobContainer.AddRuns(runsFactory);

            // Act.
            jobContainer.Execute();
            jobContainer.WaitFinished();

            // Assert.
            Assert.AreEqual(true, jobContainer.AllCompleted);

            int cycleNameEntry = 3;
            var summaryData = sumWriter.Table.Rows.Cast<DataRow>().ToDictionary(r => r[cycleNameEntry]);

            string distanceRunCycle = "DistanceRun";
            AssertHVElectricAxuliariesForHybrids(summaryData, distanceRunCycle);

            string measuredSpeedCycle = "MeasuredSpeed";
            AssertHVElectricAxuliariesForHybrids(summaryData, measuredSpeedCycle);

            string measuredSpeedGearCycle = "MeasuredSpeedGear";
            AssertHVElectricAxuliariesForHybrids(summaryData, measuredSpeedGearCycle);

            string pWheelCycle = "PWheel";
            AssertHVElectricAxuliariesForHybrids(summaryData, pWheelCycle);
		}

		private void AssertHVElectricAxuliaries(Dictionary<object, DataRow> summaryData, string drivingCycle)
        {
            string extendedSuffix = "Extended";
            string cycle = $"{drivingCycle}.vdri";
            string extendedCycle = $"{drivingCycle}_{extendedSuffix}.vdri";

            if (!summaryData.TryGetValue(cycle, out _))
            {
                return;
            }

            TestContext.WriteLine($"Tested cycle: {cycle}");

            var absoluteTime = (ConvertedSI)summaryData[extendedCycle][SumDataFields.TIME];

            var batteryLossDiff = DiffWithBaseSummaryData(summaryData, cycle, extendedCycle, SumDataFields.E_REESS_LOSS);
            var auxEnergyConsumptionDiff = DiffWithBaseSummaryData(summaryData, cycle, extendedCycle, SumDataFields.E_AUX_EL_HV);
            var ecSocDiff = DiffWithBaseSummaryData(summaryData, cycle, extendedCycle, SumDataFields.EC_el_SOC);

            // energy_consumption = aux_power * time_in_hours = aux_power * time_s/3600
            // ec_Soc = batteryLoss + auxEnergyConsumption 
            AssertSummaryValues(auxEnergyConsumptionDiff, absoluteTime, 3600, (v1, v2) => v1 / v2);
            AssertSummaryValues(ecSocDiff, batteryLossDiff, auxEnergyConsumptionDiff, (v1, v2) => v1 + v2);
        }

        private void AssertHVElectricAxuliariesForHybrids(Dictionary<object, DataRow> summaryData, string drivingCycle)
        {
            string extendedSuffix = "Extended";
            string cycle = $"{drivingCycle}.vdri";
            string extendedCycle = $"{drivingCycle}_{extendedSuffix}.vdri";

            if (!summaryData.TryGetValue(cycle, out _))
            {
                return;
            }

            TestContext.WriteLine($"Tested cycle: {cycle}");
            
            var absoluteTime = (ConvertedSI)summaryData[extendedCycle][SumDataFields.TIME];

            var auxEnergyConsumptionDiff = DiffWithBaseSummaryData(summaryData, cycle, extendedCycle, SumDataFields.E_AUX_EL_HV);

            // energy_consumption = aux_power * time_in_hours = aux_power * time_s/3600
            AssertSummaryValues(auxEnergyConsumptionDiff, absoluteTime, 3600, (v1, v2) => v1 / v2);
        }

        /// <summary>
        /// Diffs non optional vdri (base) simulation with optional values vdri simulation (extended).
        /// </summary>
        /// <param name="summaryData">Summary data to evaluate.</param>
        /// <param name="baseCycle">Summary data cycle with non optional vdri values (columns).</param>
        /// <param name="extendedCycle">Summary data cycle with optional vdri values (columns).</param>
        private static double DiffWithBaseSummaryData(Dictionary<object, DataRow> summaryData, string baseCycle, string extendedCycle, string fieldToCompare)
        {
            var baseSummaryData = summaryData[baseCycle];
            var extendedSummaryData = summaryData[extendedCycle];

            return Math.Abs((ConvertedSI)extendedSummaryData[fieldToCompare] - (ConvertedSI)baseSummaryData[fieldToCompare]);
        }

        private static void AssertSummaryValues(
            double valueToAssert,
            double value1,
            double value2,
            Func<double, double, double> computeExpectedValue,
            double tolerance = 0.00000000001)
        {
            double expected = computeExpectedValue(value1, value2);

            Assert.IsTrue(Math.Abs(expected - valueToAssert) < tolerance);
        }
    }
}

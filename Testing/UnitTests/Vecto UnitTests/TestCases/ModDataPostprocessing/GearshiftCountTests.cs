using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class GearshiftCountTest
    {
		private StandardKernel _kernel;

		[OneTimeSetUp]
		public void Setup()
		{
			_kernel = new StandardKernel(new VectoNinjectModule());
		}

        [TestCase()]
        public void TestGearshiftCountTractionInterruptionShiftup()
        {
            var rundata = new VectoRunData() {
                JobName = "GearshiftRun",
                GearboxData = new GearboxData()
            };
			var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
			Assert.IsNotNull(modData);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
            modData.Data.CreateColumns(ModalResults.GearboxSignals);

            var entries = new[] {
                new DummyEntry { v = 34, gear = 4u },
                new DummyEntry { v = 34.5, gear = 4u },
                new DummyEntry { v = 33.3, gear = 0 },
                new DummyEntry { v = 33.2, gear = 0 },
                new DummyEntry { v = 32, gear = 5 }
            };
            foreach (var dummyEntry in entries) {
                modData[ModalResultField.v_act] = dummyEntry.v.KMPHtoMeterPerSecond();
                modData[ModalResultField.Gear] = dummyEntry.gear;
                modData.CommitSimulationStep();
            }

            var gearshifts = modData.GearshiftCount();
            Assert.AreEqual(1, gearshifts.Value());
        }

        [TestCase()]
        public void TestGearshiftCountTractionInterruption()
        {
            var rundata = new VectoRunData() {
                JobName = "GearshiftRun",
                GearboxData = new GearboxData()
            };
			var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
			Assert.IsNotNull(modData);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
            modData.Data.CreateColumns(ModalResults.GearboxSignals);

            var entries = new[] {
                new DummyEntry { v = 34, gear = 4u },
                new DummyEntry { v = 34.5, gear = 4u },
                new DummyEntry { v = 33.3, gear = 0 },
                new DummyEntry { v = 33.2, gear = 0 },
                new DummyEntry { v = 32, gear = 4 }
            };
            foreach (var dummyEntry in entries) {
                modData[ModalResultField.v_act] = dummyEntry.v.KMPHtoMeterPerSecond();
                modData[ModalResultField.Gear] = dummyEntry.gear;
                modData.CommitSimulationStep();
            }

            var gearshifts = modData.GearshiftCount();
            Assert.AreEqual(0, gearshifts.Value());
        }

        [TestCase()]
        public void TestGearshiftCountTractionInterruptionShiftDown()
        {
            var rundata = new VectoRunData() {
                JobName = "GearshiftRun",
                GearboxData = new GearboxData()
            };
			var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
			Assert.IsNotNull(modData);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
            modData.Data.CreateColumns(ModalResults.GearboxSignals);

            var entries = new[] {
                new DummyEntry { v = 34, gear = 4u },
                new DummyEntry { v = 34.5, gear = 4u },
                new DummyEntry { v = 33.3, gear = 0 },
                new DummyEntry { v = 33.2, gear = 0 },
                new DummyEntry { v = 32, gear = 3 }
            };
            foreach (var dummyEntry in entries) {
                modData[ModalResultField.v_act] = dummyEntry.v.KMPHtoMeterPerSecond();
                modData[ModalResultField.Gear] = dummyEntry.gear;
                modData.CommitSimulationStep();
            }

            var gearshifts = modData.GearshiftCount();
            Assert.AreEqual(1, gearshifts.Value());
        }


        [TestCase()]
        public void TestGearshiftCountTractionInterruptionStop()
        {
            var rundata = new VectoRunData() {
                JobName = "GearshiftRun",
                GearboxData = new GearboxData()
            };
			var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
			Assert.IsNotNull(modData);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
            modData.Data.CreateColumns(ModalResults.GearboxSignals);

            var entries = new[] {
                new DummyEntry { v = 4, gear = 4u },
                new DummyEntry { v = 3.5, gear = 4u },
                new DummyEntry { v = 0, gear = 0 },
                new DummyEntry { v = 0, gear = 0 },
                new DummyEntry { v = 0, gear = 0 }
            };
            foreach (var dummyEntry in entries) {
                modData[ModalResultField.v_act] = dummyEntry.v.KMPHtoMeterPerSecond();
                modData[ModalResultField.Gear] = dummyEntry.gear;
                modData.CommitSimulationStep();
            }

            var gearshifts = modData.GearshiftCount();
            Assert.AreEqual(1, gearshifts.Value());
        }

        [TestCase()]
        public void TestGearshiftCountTractionInterruptionStopDriveOff()
        {
            var rundata = new VectoRunData() {
                JobName = "GearshiftRun",
                GearboxData = new GearboxData()
            };
			var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
			Assert.IsNotNull(modData);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
            modData.Data.CreateColumns(ModalResults.GearboxSignals);

            var entries = new[] {
                new DummyEntry { v = 4, gear = 4u },
                new DummyEntry { v = 3.5, gear = 4u },
                new DummyEntry { v = 0, gear = 0 },
                new DummyEntry { v = 0, gear = 0 },
                new DummyEntry { v = 0, gear = 0 },
                new DummyEntry { v = 3, gear = 2 }
            };
            foreach (var dummyEntry in entries) {
                modData[ModalResultField.v_act] = dummyEntry.v.KMPHtoMeterPerSecond();
                modData[ModalResultField.Gear] = dummyEntry.gear;
                modData.CommitSimulationStep();
            }

            var gearshifts = modData.GearshiftCount();
            Assert.AreEqual(2, gearshifts.Value());
        }


        [TestCase()]
        public void TestGearshiftCountTractionInterruptionShiftupAT()
        {
            var rundata = new VectoRunData() {
                JobName = "GearshiftRun",
                GearboxData = new GearboxData()
            };
			var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
			Assert.IsNotNull(modData);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
            modData.Data.CreateColumns(ModalResults.GearboxSignals);

            var entries = new[] {
                new DummyEntry { v = 34, gear = 4u },
                new DummyEntry { v = 34.5, gear = 4u },
                new DummyEntry { v = 33.3, gear = 5 },
                new DummyEntry { v = 33.2, gear = 5 },
                new DummyEntry { v = 32, gear = 5 }
            };
            foreach (var dummyEntry in entries) {
                modData[ModalResultField.v_act] = dummyEntry.v.KMPHtoMeterPerSecond();
                modData[ModalResultField.Gear] = dummyEntry.gear;
                modData.CommitSimulationStep();
            }

            var gearshifts = modData.GearshiftCount();
            Assert.AreEqual(1, gearshifts.Value());
        }


        [TestCase()]
        public void TestGearshiftCountTractionInterruptionShiftDownAT()
        {
            var rundata = new VectoRunData() {
                JobName = "GearshiftRun",
                GearboxData = new GearboxData()
            };
			var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(rundata, null, null, null) as ModalDataContainer;
			Assert.IsNotNull(modData);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
            modData.Data.CreateColumns(ModalResults.GearboxSignals);

            var entries = new[] {
                new DummyEntry { v = 34, gear = 4u },
                new DummyEntry { v = 34.5, gear = 4u },
                new DummyEntry { v = 33.3, gear = 4 },
                new DummyEntry { v = 33.2, gear = 3 },
                new DummyEntry { v = 32, gear = 3 }
            };
            foreach (var dummyEntry in entries) {
                modData[ModalResultField.v_act] = dummyEntry.v.KMPHtoMeterPerSecond();
                modData[ModalResultField.Gear] = dummyEntry.gear;
                modData.CommitSimulationStep();
            }

            var gearshifts = modData.GearshiftCount();
            Assert.AreEqual(1, gearshifts.Value());
        }


        public class DummyEntry
        {
            public double v;
            public uint gear;
        }
    }
}

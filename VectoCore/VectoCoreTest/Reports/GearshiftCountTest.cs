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

using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Reports
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class GearshiftCountTest
	{
		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearshiftCountTractionInterruptionShiftup()
		{
			var rundata = new VectoRunData() {
				JobName = "GearshiftRun",
				GearboxData = new GearboxData()
            };
			var modData = new ModalDataContainer(rundata, null, null);
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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearshiftCountTractionInterruption()
		{
			var rundata = new VectoRunData()
			{
				JobName = "GearshiftRun",
				GearboxData = new GearboxData()
			};
			var modData = new ModalDataContainer(rundata, null, null);
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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearshiftCountTractionInterruptionShiftDown()
		{
			var rundata = new VectoRunData()
			{
				JobName = "GearshiftRun",
				GearboxData = new GearboxData()
			};
			var modData = new ModalDataContainer(rundata, null, null);
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


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearshiftCountTractionInterruptionStop()
		{
			var rundata = new VectoRunData()
			{
				JobName = "GearshiftRun",
				GearboxData = new GearboxData()
			};
			var modData = new ModalDataContainer(rundata, null, null);
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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearshiftCountTractionInterruptionStopDriveOff()
		{
			var rundata = new VectoRunData()
			{
				JobName = "GearshiftRun",
				GearboxData = new GearboxData()
			};
			var modData = new ModalDataContainer(rundata, null, null);
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


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearshiftCountTractionInterruptionShiftupAT()
		{
			var rundata = new VectoRunData()
			{
				JobName = "GearshiftRun",
				GearboxData = new GearboxData()
			};
			var modData = new ModalDataContainer(rundata, null, null);
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


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearshiftCountTractionInterruptionShiftDownAT()
		{
			var rundata = new VectoRunData()
			{
				JobName = "GearshiftRun",
				GearboxData = new GearboxData()
			};
			var modData = new ModalDataContainer(rundata, null, null);
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

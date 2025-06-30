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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DistanceCycleDataTest
	{
//		public readonly string CycleFile = @"TestData/Cycles/";
		private const string ResourceNamespace = "TUGraz.VectoCore.Resources.Declaration.";


		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void FilterRedundantEntries()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				" 1, 20, 0, 0",
				" 2, 20, 0, 0",
				" 3, 20, 0, 0",
				" 4, 20, 0, 0",
				" 5, 20, 1, 0",
				" 6, 20, 1, 0",
				" 6, 20, 1, 0",
				" 8, 20, 1, 0",
				" 9, 20, 1, 0",
			};

			var cycleData = SimpleDrivingCycles.CreateCycleData(data);
			Assert.AreEqual(3, cycleData.Entries.Count);

			Assert.AreEqual(0, cycleData.Entries[0].Distance.Value());
			Assert.AreEqual(5, cycleData.Entries[1].Distance.Value());
			Assert.AreEqual(9, cycleData.Entries[2].Distance.Value());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HandleStopTimes()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				" 1, 20, 0, 0",
				" 2, 20, 0, 0",
				" 3, 20, 0, 0",
				" 4, 20, 0, 0",
				" 5,  0, 0, 5",
				" 6, 20, 0, 0",
				" 6, 20, 0, 0",
				" 8, 20, 0, 0",
				" 9, 20, 0, 0",
			};

			var cycleData = SimpleDrivingCycles.CreateCycleData(data);
			Assert.AreEqual(4, cycleData.Entries.Count);

			Assert.AreEqual(0, cycleData.Entries[0].Distance.Value());
			Assert.AreEqual(5, cycleData.Entries[1].Distance.Value());
			Assert.AreEqual(5, cycleData.Entries[2].Distance.Value());
			Assert.AreEqual(9, cycleData.Entries[3].Distance.Value());

			Assert.AreEqual(5, cycleData.Entries[1].StoppingTime.Value());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void StopTimeWhenVehicleSpeedIsNotZero()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				"20, 20, 0, 5",
				"90, 20, 0, 0"
			};

			AssertHelper.Exception<VectoException>(() => SimpleDrivingCycles.CreateCycleData(data));
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void DistanceNotStrictlyIncreasing()
		{
			var data = new[] {
				" 0, 20, 0, 0",
				"90, 20, 0, 0",
				"80, 20, 0, 0"
			};

			AssertHelper.Exception<VectoException>(() => SimpleDrivingCycles.CreateCycleData(data));
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void CycleAltitudeTest()
		{
			var missionType = "LongHaul";
			var stream =
				RessourceHelper.ReadStream(ResourceNamespace + "MissionCycles." + missionType + Constants.FileExtensions.CycleFile);

			var cycleData = DrivingCycleDataReader.ReadFromStream(stream, CycleType.DistanceBased, "", false);
			foreach (var entry in cycleData.Entries) {
				var tmp = entry.Altitude;
			}
		}
	}
}
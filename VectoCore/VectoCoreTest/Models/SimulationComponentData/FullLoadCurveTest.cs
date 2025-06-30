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
using NLog;
using NLog.Config;
using NLog.Targets;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using NUnit.Framework;
using System.IO;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FullLoadCurveTest
	{
		private const string CoachEngineFLD = @"TestData/Components/24t Coach.vfld";
		private const double Tolerance = 0.0001;

		public static List<string> LogList = new List<string>();

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestFullLoadStaticTorque()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(1180, fldCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1352, fldCurve.FullLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1231, fldCurve.FullLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestFullLoadEngineSpeedRated()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);
			Assert.AreEqual(181.8444, fldCurve.RatedSpeed.Value(), Tolerance);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestFullLoadStaticPower()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(69198.814183, fldCurve.FullLoadStationaryPower(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(283162.218372, fldCurve.FullLoadStationaryPower(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(74767.810760, fldCurve.FullLoadStationaryPower(580.RPMtoRad()).Value(), Tolerance);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestDragLoadStaticTorque()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(-149, fldCurve.DragLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-301, fldCurve.DragLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-148.5, fldCurve.DragLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-150, fldCurve.DragLoadStationaryTorque(520.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-339, fldCurve.DragLoadStationaryTorque(2200.RPMtoRad()).Value(), Tolerance);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestDragLoadStaticPower()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(-8737.81636, fldCurve.DragLoadStationaryPower(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-63041.29254, fldCurve.DragLoadStationaryPower(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-9019.51251, fldCurve.DragLoadStationaryPower(580.RPMtoRad()).Value(), Tolerance);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestPT1()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(0.6, fldCurve.PT1(560.RPMtoRad()).Value.Value(), Tolerance);
			Assert.AreEqual(0.25, fldCurve.PT1(2000.RPMtoRad()).Value.Value(), Tolerance);
			Assert.AreEqual(0.37, fldCurve.PT1(1700.RPMtoRad()).Value.Value(), Tolerance);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestPreferredSpeed()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);
			fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };
			AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
			var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.N95hSpeed);
			Assert.AreEqual((0.51 * totalArea).Value(),
				fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
			AssertHelper.AreRelativeEqual(194.515816596908.SI<PerSecond>(), fldCurve.N95hSpeed);
			AssertHelper.AreRelativeEqual(94.24639.SI<PerSecond>(), fldCurve.LoSpeed);
			//AssertHelper.AreRelativeEqual(219.084329211505.SI<PerSecond>(), fldCurve.HiSpeed);
			AssertHelper.AreRelativeEqual(2300.SI<NewtonMeter>(), fldCurve.MaxTorque);
			AssertHelper.AreRelativeEqual(-320.SI<NewtonMeter>(), fldCurve.MaxDragTorque);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestP99HighSpeed()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);
			fldCurve.EngineData = new CombustionEngineData() {IdleSpeed = 560.RPMtoRad()};

			var nP99h = fldCurve.NP99hSpeed;

			Assert.IsTrue(nP99h > fldCurve.PreferredSpeed);
			Assert.AreEqual(fldCurve.MaxPower.Value() * 0.99, (fldCurve.FullLoadStationaryTorque(nP99h) * nP99h).Value(), 1e-3);
			Assert.AreEqual(1810.67898, nP99h.AsRPM, 1e-3);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestTq99HighSpeed()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);
			fldCurve.EngineData = new CombustionEngineData() { IdleSpeed = 560.RPMtoRad() };

			var nTq99h = fldCurve.NTq99hSpeed;

			Assert.IsTrue(nTq99h > fldCurve.PreferredSpeed);
			Assert.AreEqual(fldCurve.MaxTorque.Value() * 0.99, fldCurve.FullLoadStationaryTorque(nTq99h).Value(), 1e-3);
			Assert.AreEqual(1420.8144, nTq99h.AsRPM, 1e-3);
		}


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestTq99LowSpeed()
		{
			var fldCurve = FullLoadCurveReader.ReadFromFile(CoachEngineFLD);
			fldCurve.EngineData = new CombustionEngineData() { IdleSpeed = 560.RPMtoRad() };

			var nTq99l = fldCurve.NTq99lSpeed;

			Assert.IsTrue(nTq99l < fldCurve.PreferredSpeed);
			Assert.AreEqual(fldCurve.MaxTorque.Value() * 0.99, fldCurve.FullLoadStationaryTorque(nTq99l).Value(), 1e-3);
			Assert.AreEqual(990.9626, nTq99l.AsRPM, 1e-3);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestPreferredSpeed2()
		{
			var fldData = new[] {
				"560,1180,-149,0.6",
				"600,1282,-148,0.6",
				"800,1791,-149,0.6",
				"1000,2300,-160,0.6",
				"1200,2400,-179,0.6",
				"1400,2300,-203,0.6",
				"1600,2079,-235,0.49",
				"1800,1857,-264,0.25",
				"2000,1352,-301,0.25",
				"2100,1100,-320,0.25",
			};
			var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);
			var fldCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldEntries));
			fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };

			var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.N95hSpeed);
			Assert.AreEqual((0.51 * totalArea).Value(),
				fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
			//AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestN95hSpeedInvalid()
		{
			var fldData = new[] {
				"600,539.8228,-59.02274, 1.0",
				"821,673.5694587,-62.77795, 1.0",
				"1041,1102.461949,-68.37734, 1.0",
				"1262,1112.899122,-76.0485, 1.0",
				"1482,1098.632364,-85.00573, 1.0",
				"1606,1093.403667,-90.9053, 1.0",
				"1800,1058.081866,-100.937, 1.0",
				"1995,992.0155535,-112.1166, 1.0",
				"2189,926.7779212,-124.9432, 1.0",
				"4000,811.7189964,-138.7132, 1.0",
			};
			var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);
			var fldCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldEntries));
			fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };

			AssertHelper.Exception<VectoException>(() => { var tmp = fldCurve.N95hSpeed; });
			//var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, );
			//Assert.AreEqual((0.51 * totalArea).Value(),
			//	fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
			//AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
		}

		/// <summary>
		///		[VECTO-679]
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestN95hComputation()
		{
			var fldData = new[] {
				"590,486,-44,0.60	",
				"600,486,-44,0.60	",
				"800,755,-54,0.60	",
				"1000,883,-62,0.60	",
				"1200,899,-74,0.60	",
				"1300,899,-80,0.60	",
				"1400,899,-87,0.60	",
				"1500,899,-92,0.60	",
				"1600,899,-97,0.60	",
				"1700,890,-100,0.60	",
				"1800,881,-103,0.60	",
				"1900,867,-107,0.60	",
				"2000,853,-111,0.43	",
				"2150,811,-118,0.29	",
				"2200,802,-125,0.25	",
				"2300,755,-130,0.25	",
				"2400,705,-135,0.25	",
				"2500,644,-140,0.25	",
				"2600,479,-145,0.25	",
				"2700,0,-149,0.25	",
			};
			var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);
			var fldCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldEntries));
			fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };

			Assert.AreEqual(2420.5, fldCurve.N95hSpeed.AsRPM, 1);

			
		}

		/// <summary>
		///     [VECTO-78]
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_FileRead_WrongFileFormat_InsufficientColumns()
		{
			AssertHelper.Exception<VectoException>(
				() => FullLoadCurveReader.ReadFromFile(@"TestData/Components/FullLoadCurve insufficient columns.vfld"),
				"ERROR while reading FullLoadCurve File: Engine FullLoadCurve Data File must consist of at least 3 columns.");
		}

		/// <summary>
		/// [VECTO-78]
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_FileRead_HeaderColumnsNotNamedCorrectly()
		{
			LogList.Clear();
			var target = new MethodCallTarget {
				ClassName = typeof(FullLoadCurveTest).AssemblyQualifiedName,
				MethodName = "LogMethod_Test_FileRead_HeaderColumnsNotNamedCorrectly"
			};
			target.Parameters.Add(new MethodCallParameter("${level}"));
			target.Parameters.Add(new MethodCallParameter("${message}"));
			SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Warn);
			FullLoadCurveReader.ReadFromFile(@"TestData/Components/FullLoadCurve wrong header.vfld");
			Assert.IsTrue(
				LogList.Contains(
					"FullLoadCurve: Header Line is not valid. Expected: 'engine speed, full load torque, motoring torque', Got: 'n, Mfull, Mdrag, PT1'. Falling back to column index."),
				string.Join("\n", LogList));
			LogList.Clear();
		}

		public static void LogMethod_Test_FileRead_HeaderColumnsNotNamedCorrectly(string level, string message)
		{
			LogList.Add(message);
		}

		/// <summary>
		///     [VECTO-78]
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_FileRead_NoHeader()
		{
			var curve = FullLoadCurveReader.ReadFromFile(@"TestData/Components/FullLoadCurve no header.vfld");
			var result = curve.FullLoadStationaryTorque(1.SI<PerSecond>());
			Assert.AreNotEqual(result.Value(), 0.0);
		}

		/// <summary>
		///     [VECTO-78]
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_FileRead_InsufficientEntries()
		{
			AssertHelper.Exception<VectoException>(
				() => FullLoadCurveReader.ReadFromFile(@"TestData/Components/FullLoadCurve insufficient entries.vfld"),
				"ERROR while reading FullLoadCurve File: FullLoadCurve must consist of at least two lines with numeric values (below file header)");
		}

		[TestCase,
		Ignore(Definitions.DEPRECATED),
		Category(Definitions.DEPRECATED)]
		// validation of run data produces many more errors, does not check for engine data nor loss map
		public void FullLoad_LossMap_Test()
		{
			var engineData = new CombustionEngineData {
				FullLoadCurves =
					new Dictionary<uint, EngineFullLoadCurve>() {
						{ 0, FullLoadCurveReader.ReadFromFile(@"TestData/Components/12t Delivery Truck.vfld") },
						{ 1, FullLoadCurveReader.ReadFromFile(@"TestData/Components/12t Delivery Truck.vfld") }
					},
				IdleSpeed = 560.RPMtoRad()
			};

			var gearboxData = new GearboxData();
			gearboxData.Gears[1] = new GearData {
				LossMap = TransmissionLossMapReader.ReadFromFile(@"TestData/Components/limited.vtlm", 1, "1"),
				Ratio = 1
			};

			var axleGearData = new AxleGearData() {
				AxleGear = new GearData {
					Ratio = 1,
					LossMap = TransmissionLossMapReader.ReadFromFile(@"TestData/Components/limited.vtlm", 1, "1"),
				}
			};

			var runData = new VectoRunData { GearboxData = gearboxData, EngineData = engineData, AxleGearData = axleGearData };
			Assert.IsFalse(runData.IsValid());
		}

		/// <summary>
		///     [VECTO-190]
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestSortingFullLoadEntries()
		{
			var fldEntries = new[] {
				"600,1282,-148,0.6			 ",
				"799.9999999,1791,-149,0.6	 ",
				"560,1180,-149,0.6			 ",
				"1000,2300,-160,0.6			 ",
				"1599.999999,2079,-235,0.49	 ",
				"1200,2300,-179,0.6			 ",
				"1800,1857,-264,0.25		 ",
				"1400,2300,-203,0.6			 ",
				"2000.000001,1352,-301,0.25	 ",
				"2100,1100,-320,0.25		 ",
			};

			var fldCurve =
				FullLoadCurveReader.Create(
					VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s]", fldEntries)));

			Assert.AreEqual(1180, fldCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1352, fldCurve.FullLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1231, fldCurve.FullLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestDuplicateEntries()
		{
			var fldData = new[] {
				"560,1180,-149,0.6",
				"600,1282,-148,0.6",
				"800,1791,-149,0.6",
				"1000,2300,-160,0.6",
				"1200,2400,-179,0.6",
				"1400,2300,-203,0.6",
				"1600,2079,-235,0.49",
				"1800,1857,-264,0.25",
				"2000,1352,-301,0.25",
				"2100,1100,-320,0.25",
				"1200,2410,-180,0.6",
			};
			var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);

			AssertHelper.Exception<VectoException>(
				() => {
					var fldCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldEntries));
				}, messageContains: "Error reading full-load curve: multiple entries for engine speeds 1200");
		}
	}
}
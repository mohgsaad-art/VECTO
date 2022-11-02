using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	public class GearshiftLinesVoithTest
	{
		public const string File = @"TestData\Components\GearshiftLinesVoith.vgsv";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[Ignore("VOITH SHIFT LINES")]
		[TestCase(File, 1, 1, 0.0, 0.0, 680),
		TestCase(File, 1, 1, 0.0, 1.0, 680),
		TestCase(File, 1, 1, 0.0, 0.5, 680),

		TestCase(File, 1, 1, 5.0, 0.0, 725),
		TestCase(File, 1, 1, 5.0, 1.0, 725),
		TestCase(File, 1, 1, 5.0, 0.5, 725),


		TestCase(File, 1, 1, -5.0, 0.0, 650),
		TestCase(File, 1, 1, -5.0, 1.0, 650),
		TestCase(File, 1, 1, -5.0, 0.5, 650),

		TestCase(File, 1, 1, 2.5, 0.0, 702.5),
		TestCase(File, 1, 1, 2.5, 1.0, 702.5),
		TestCase(File, 1, 1, 2.5, 0.5, 702.5),

		TestCase(File, 1, 2, 0.0, 1.0, 680),
		TestCase(File, 1, 2, 5.0, 1.0, 725),
		TestCase(File, 1, 2, -5.0, 1.0, 650),
		TestCase(File, 1, 2, 2.5, 1.0, 702.5),

		TestCase(File, 1, 4, 0.0, 0.5, 690),
		TestCase(File, 1, 4, 5.0, 0.5, 735),
		TestCase(File, 1, 4, -5.0, 0.5, 660),
		TestCase(File, 1, 4, 2.5, 0.0, 722.5),
		TestCase(File, 1, 4, 2.5, 0.5, 712.5),
		TestCase(File, 1, 4, 2.5, 1.0, 702.5),

		TestCase(File, 1, 5, 0.0, 0.5, 700),
		TestCase(File, 1, 5, 5.0, 0.5, 745),
		TestCase(File, 1, 5, -5.0, 0.5, 670),
		TestCase(File, 1, 5, 2.5, 0.0, 732.5),
		TestCase(File, 1, 5, 2.5, 0.5, 722.5),
		TestCase(File, 1, 5, 2.5, 1.0, 712.5),

		TestCase(File, 1, 6, 0.0, 0.5, 710),
		TestCase(File, 1, 6, 5.0, 0.5, 755),
		TestCase(File, 1, 6, -5.0, 0.5, 680),
		TestCase(File, 1, 6, 2.5, 0.0, 742.5),
		TestCase(File, 1, 6, 2.5, 0.5, 732.5),
		TestCase(File, 1, 6, 2.5, 1.0, 722.5),

		// Testcases from debugging Matlab model
		TestCase(File, 1, 6, 1.5846, 0.47923, 734.26, 0.61774, 0.839467),
		TestCase(File, 2, 5, 2.8, 1.0, 767.09, 0.98723, 1.341563),
		TestCase(File, 3, 5, 2.8, 1.0, 814.67, 0.98698, 1.341227)

			]
		public void TestShiftlinesLookupUpshift(string filename, int gear, int loadStage, double gradient, double acc, double expectedRpm, double amin = 0.1, double amax = 0.9)
		{

			var data = VectoCSVFile.Read(filename);

			var runData = GetVectoRunData(data);

			//var strategy = new ATShiftStrategyVoith(new SimplePowertrainContainer(runData));
			var slope = VectoMath.InclinationToAngle(gradient / 100.0);

			//var upshiftSpeed = strategy.UpshiftLines[gear].LookupShiftSpeed(
			//	loadStage, slope, acc.SI<MeterPerSquareSecond>(), amin.SI<MeterPerSquareSecond>(), amax.SI<MeterPerSquareSecond>());

			//Assert.AreEqual(expectedRpm, upshiftSpeed.AsRPM, 0.1);
		}

		[Ignore("VOITH SHIFT LINES")]
		[TestCase(File, 3, 5, 2.8, 1.0, 731.2)]
		public void TestShiftlinesLookupDownshift(string filename, int gear, int loadStage, double gradient, double acc, double expectedRpm, double amin = -0.2, double amax = -0.4)
		{
			var data = VectoCSVFile.Read(filename);

			var runData = GetVectoRunData(data);

			//var strategy = new ATShiftStrategyVoith(new SimplePowertrainContainer(runData));
			var slope = VectoMath.InclinationToAngle(gradient / 100.0);

			//var upshiftSpeed = strategy.DownshiftLines[gear].LookupShiftSpeed(
			//	loadStage, slope, acc.SI<MeterPerSquareSecond>(), amin.SI<MeterPerSquareSecond>(), amax.SI<MeterPerSquareSecond>());

			//Assert.AreEqual(expectedRpm, upshiftSpeed.AsRPM, 0.1);
		}

		private static VectoRunData GetVectoRunData(TableData data)
		{
			var loadStageThresoldsUp = "19.7;36.34;53.01;69.68;86.35".Split(';').Select(x => x.ToDouble()).ToList();
			var loadStageThresoldsDown = "13.7;30.34;47.01;63.68;80.35".Split(';').Select(x => x.ToDouble()).ToList();
			var runData = new VectoRunData() {
				EngineData = new CombustionEngineData() {
					FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
						{ 0, new EngineFullLoadCurve(new List<EngineFullLoadCurve.FullLoadCurveEntry>(), new PT1()) }
					}
				},
				VehicleData = new VehicleData() {
					GrossVehicleMass = 18000.SI<Kilogram>()
				},
				GearshiftParameters = new ShiftStrategyParameters() {
					GearshiftLines = data,
					LoadstageThresholds = loadStageThresoldsUp.Zip(loadStageThresoldsDown, Tuple.Create)
				},
				GearboxData = new GearboxData() {
					Gears = new Dictionary<uint, GearData>() {
						{1, new GearData() },
						{2,new GearData() }
					}
				}
			};
			return runData;
		}
	}
}

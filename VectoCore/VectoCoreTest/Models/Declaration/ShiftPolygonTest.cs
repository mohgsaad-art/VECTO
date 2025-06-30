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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Point = TUGraz.VectoCommon.Utils.Point;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ShiftPolygonTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}


		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void IntersectShiftLines1()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 8),
				new Point(18, 22)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void IntersectShiftLines2()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 6),
				new Point(18, 14)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 7.6),
				new Point(20, 15.6),
			};

			var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}

			result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(transformed, upShift);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void IntersectShiftLines3()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 4),
				new Point(18, 22)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 7.6),
				new Point(13, 13),
				new Point(20, 20),
			};

			var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}

			result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(transformed, upShift);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void IntersectShiftLines4()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 12),
				new Point(18, 16)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(14.6666, 14.6666),
				new Point(20, 16.8),
			};

			var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}

			result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(transformed, upShift);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void LimitShiftlines1()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var limit = new[] {
				new Point(8, 0),
				new Point(8, 20)
			};

			var expected = new[] {
				new Point(8, 0),
				new Point(8, 20)
			};

			var result = DeclarationData.Gearbox.IntersectTakeLowerShiftLine(upShift, limit);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void LimitShiftlines2()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var limit = new[] {
				new Point(15, 0),
				new Point(15, 20)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(15, 15),
				new Point(15, 20),
			};

			var result = DeclarationData.Gearbox.IntersectTakeLowerShiftLine(upShift, limit);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void LimitShiftlines3()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var limit = new[] {
				new Point(25, 0),
				new Point(25, 20)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var result = DeclarationData.Gearbox.IntersectTakeLowerShiftLine(upShift, limit);

			Assert.AreEqual(expected.Length, result.Length);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ShiftPolygonFldMarginTest()
		{
			var engineFld = new[] {
				new Point(8, 10),
				new Point(9, 11),
				new Point(10, 11.5),
				new Point(11, 12),
				new Point(12, 12.5),
				new Point(13, 13),
				new Point(14, 13.5),
				new Point(15, 14),
				new Point(16, 14.5),
				new Point(17, 15),
				new Point(18, 16.5),
				new Point(19, 18),
				new Point(20, 19.5),
				new Point(21, 21),
				new Point(22, 22.5),
			};

			var expected = new[] {
				new Point(8, 9.8),
				new Point(9, 10.78),
				new Point(10, 11.27),
				new Point(11, 11.76),
				new Point(12, 12.25),
				new Point(13, 12.74),
				new Point(14, 13.23),
				new Point(15, 13.72),
				new Point(16, 14.21),
				new Point(17, 14.7),
				new Point(18, 16.17),
				new Point(19, 17.64),
				new Point(20, 19.11),
				new Point(21, 20.58),
				new Point(22, 22.05),
			};

			var result =
				DeclarationData.Gearbox.ShiftPolygonFldMargin(
					engineFld.Select(
						p =>
							new EngineFullLoadCurve.FullLoadCurveEntry() {
								EngineSpeed = p.X.SI<PerSecond>(),
								TorqueFullLoad = p.Y.SI<NewtonMeter>()
							}).ToList(),
					23.SI<PerSecond>());

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void CorrectDownShiftByEngineFldTest()
		{
			var downshift = Edge.Create(new Point(10, 10), new Point(22, 20));
			var engineFldCorr = new[] {
				new Point(8, 9.8),
				new Point(9, 10.78),
				new Point(10, 11.27),
				new Point(11, 11.76),
				new Point(12, 12.25),
				new Point(13, 12.74),
				new Point(14, 13.23),
				new Point(15, 13.72),
				new Point(16, 14.21),
				new Point(17, 14.7),
				new Point(18, 16.17),
				new Point(19, 17.64),
				new Point(20, 19.11),
				new Point(21, 20.58),
				new Point(22, 22.05),
			};

			var corrected = DeclarationData.Gearbox.MoveDownshiftBelowFld(downshift, engineFldCorr, 20.SI<NewtonMeter>());

			Assert.AreEqual(10, corrected.P1.X, 1e-3);
			Assert.AreEqual(8.86666, corrected.P1.Y, 1e-3);
			Assert.AreEqual(23.36, corrected.P2.X, 1e-3);
			Assert.AreEqual(20, corrected.P2.Y, 1e-3);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ComputeShiftPolygonDeclarationTest()
		{
			var engineFile = @"TestData/Components/40t_Long_Haul_Truck.veng";
			var gearboxFile = @"TestData/Components/40t_Long_Haul_Truck.vgbx";

			var rdyn = 0.4882675.SI<Meter>();
			var axlegearRatio = 2.59;

			var expectedDownshift = new[] {
				// Gear 1
				new[] {
					new Point(64.50736915, -352),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
				// Gear 2
				new[] {
					new Point(64.50736915, -352),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
				// Gear 3
				new[] {
					new Point(64.50736915, -352),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
			};
			var expectedUpshift = new[] {
				// Gear 1
				new[] {
					new Point(136.9338946, -352),
					new Point(136.9338946, 1281.30911),
					new Point(201.7326, 2427.6748),
				},
				// Gear 2
				new[] {
					new Point(136.9338946, -352),
					new Point(136.9338946, 1281.30911),
					new Point(203.9530, 2466.9558),
				},
				// Gear 3
				new[] {
					new Point(136.9338946, -352),
					new Point(136.9338946, 1281.30911),
					//new Point(153.606666, 1893.19273),
					new Point(201.3375, 2420.6842),
				},
			};

			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
			var gearboxData = new JSONGearboxDataV5(JSONInputDataFactory.ReadFile(gearboxFile), gearboxFile);
			var engineInput = new JSONEngineDataV3(JSONInputDataFactory.ReadFile(engineFile), engineFile);
			var vehicle = new MockDeclarationVehicleInputData() {
				EngineInputData = engineInput,
				GearboxInputData = gearboxData
			};
			var engineData = dao.CreateEngineData(vehicle, engineInput.EngineModes.First(), new Mission() {MissionType = MissionType.LongHaul});

			var shiftPolygons = new List<ShiftPolygon>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(i,
					engineData.FullLoadCurves[(uint)(i + 1)],
					gearboxData.Gears,
					engineData, axlegearRatio, rdyn));
			}

			for (var i = 0; i < Math.Min(gearboxData.Gears.Count, expectedDownshift.Length); i++) {
				foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				}

				foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				}
			}

			Assert.AreEqual(0, shiftPolygons.First().Downshift.Count);
			Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ComputeShiftPolygonATDeclarationTest()
		{
			var engineFile = @"TestData/Components/40t_Long_Haul_Truck.veng";
			var gearboxFile = @"TestData/Components/40t_Long_Haul_Truck.vgbx";

			var rdyn = 0.4882675.SI<Meter>();
			var axlegearRatio = 2.59;

			var expectedDownshift = new[] {
				new Point(73.3038, -352),
				new Point(73.3038, 2530),
			};
			var expectedUpshift = new[] {
				new Point(94.2478, -352),
				new Point(94.2478, 0),
				new Point(123.0457, 2530),
			};

			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
			var gearboxData = new JSONGearboxDataV5(JSONInputDataFactory.ReadFile(gearboxFile), gearboxFile);
			var engineInput = new JSONEngineDataV3(JSONInputDataFactory.ReadFile(engineFile), engineFile);
			var vehicle = new MockDeclarationVehicleInputData() {
				EngineInputData = engineInput,
				GearboxInputData = gearboxData
			};
			var engineData = dao.CreateEngineData(vehicle, engineInput.EngineModes.First(), new Mission() { MissionType = MissionType.LongHaul });

			var shiftPolygons = new List<ShiftPolygon>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(DeclarationData.Gearbox.ComputeShiftPolygon(GearboxType.ATSerial, i,
					engineData.FullLoadCurves[(uint)(i + 1)],
					gearboxData.Gears,
					engineData, axlegearRatio, rdyn, null));
			}

			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				foreach (var tuple in expectedDownshift.Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				}

				foreach (var tuple in expectedUpshift.Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				}
			}

			Assert.AreEqual(0, shiftPolygons.First().Downshift.Count);
			Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);
		}

		[TestCase,
		Category(Definitions.DEPRECATED)]
		public void ComputeShiftPolygonDeclarationTestConfidentialEngine()
		{
			//var engineFldFile = @"E:/QUAM/Downloads/EngineFLD/Map_375c_BB1390_modTUG_R49_375c_BB1386.vfld";
			//var engineFldFile = @"E:/QUAM/tmp/scania_fullload_shiftpolygon-test.csv";
			//var gearboxFile = @"E:/QUAM/Downloads/TUG_dev_gbx/TUG_dev/GRS905R.vgbx";
			var engineFldFile = @"E:/QUAM/Downloads/attachment/Models_Declaration-mode/Overdrive/text/VENG_330kW_GENERIC.vfld";
			var gearboxFile = @"E:/QUAM/Downloads/attachment/Models_Declaration-mode/Overdrive/text/VGBX_AMT_12_overdr_DECL.vgbx";

			//@"TestData/Components/40t_Long_Haul_Truck.vgbx";

			if (!File.Exists(engineFldFile)) {
				Assert.Inconclusive("Confidential File not found. Test cannot run without file.");
			}

			var rdyn = DeclarationData.Wheels.Lookup("315/70 R22.5").DynamicTyreRadius; //0.4882675.SI<Meter>();
			//var axlegearRatio = 3.71; //2.31; // 3.71; //2.59;

			var gearboxData = new JSONGearboxDataV6(JSONInputDataFactory.ReadFile(gearboxFile), gearboxFile);

			var axlegearRatio = gearboxData.Ratio;

			var engineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad(),
			};

			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
			fullLoadCurves[0] = FullLoadCurveReader.ReadFromFile(engineFldFile, true);
			fullLoadCurves[0].EngineData = engineData;
			for (uint i = 1; i <= gearboxData.Gears.Count; i++) {
				fullLoadCurves[i] = AbstractSimulationDataAdapter.IntersectFullLoadCurves(fullLoadCurves[0],
					gearboxData.Gears[(int)(i - 1)].MaxTorque);
			}
			engineData.FullLoadCurves = fullLoadCurves;

			var shiftPolygons = new List<ShiftPolygon>();
			var downshiftTransformed = new List<List<Point>>();
			var downshiftOrig = new List<List<Point>>();
			var upshiftOrig = new List<List<Point>>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(DeclarationData.Gearbox.ComputeShiftPolygon(GearboxType.AMT, i, fullLoadCurves[(uint)(i + 1)],
					gearboxData.Gears,
					engineData, axlegearRatio, rdyn, null));

				ShiftPolygonComparison.ComputShiftPolygonPoints(i, fullLoadCurves[(uint)(i + 1)], gearboxData.Gears,
					engineData, axlegearRatio, rdyn, out var tmp1, out var tmp2, out var tmp3);
				upshiftOrig.Add(tmp1);
				downshiftTransformed.Add(tmp2);
				downshiftOrig.Add(tmp3);
			}

			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(gearboxFile), fullLoadCurves, shiftPolygons,
				Path.Combine(Path.GetDirectoryName(gearboxFile), "Shiftlines.png"),
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / rdyn * axlegearRatio * gearboxData.Gears.Last().Ratio, upshiftOrig,
				downshiftTransformed, downshiftOrig);

			var shiftLines = "";
			var gear = 1;
			foreach (var shiftPolygon in shiftPolygons) {
				shiftLines += "Gear " + gear + "\n";
				shiftLines += "Upshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Upshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
				shiftLines += "Downshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Downshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
			}
		}

		[TestCase(@"E:/QUAM/Downloads/upshifts-missing_over-revving/FL_curve_orig.xml"),
			TestCase(@"E:/QUAM/Downloads/VECTO_821/VECTO_lorry_6x2_13L_276kW.vecto")]
		public void ComputeShiftPolygonDeclarationTestConfidentialXMLJob(string jobFile)
		{
			if (!File.Exists(jobFile)) {
				Assert.Inconclusive("Confidential File not found. Test cannot run without file.");
			}

			IDeclarationInputDataProvider job = (IDeclarationInputDataProvider)(Path.GetExtension(jobFile) == ".vecto" ? JSONInputDataFactory.ReadJsonJob(jobFile) : xmlInputReader.CreateDeclaration(jobFile));

			var gearboxData = job.JobInputData.Vehicle.Components.GearboxInputData;
			var idlespeed = VectoMath.Max(
				job.JobInputData.Vehicle.EngineIdleSpeed, job.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().IdleSpeed);
			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
			var engineData = dao.CreateEngineData(job.JobInputData.Vehicle, job.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First(), new Mission() {MissionType = MissionType.LongHaul});
			var axlegearRatio = job.JobInputData.Vehicle.Components.AxleGearInputData.Ratio;
			var rdyn = job.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration.Where(x => x.AxleType == AxleType.VehicleDriven)
						.Select(x => DeclarationData.Wheels.Lookup(x.Tyre.Dimension)).Average(x => x.DynamicTyreRadius.Value())
						.SI<Meter>();

			var fullLoadCurves = engineData.FullLoadCurves;
			var gearboxFile = jobFile;

			var shiftPolygons = new List<ShiftPolygon>();
			var downshiftTransformed = new List<List<Point>>();
			var downshiftOrig = new List<List<Point>>();
			var upshiftOrig = new List<List<Point>>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(DeclarationData.Gearbox.ComputeShiftPolygon(GearboxType.AMT, i, fullLoadCurves[(uint)(i + 1)],
					gearboxData.Gears,
					engineData, axlegearRatio, rdyn, null));

				ShiftPolygonComparison.ComputShiftPolygonPoints(i, fullLoadCurves[(uint)(i + 1)], gearboxData.Gears,
					engineData, axlegearRatio, rdyn, out var tmp1, out var tmp2, out var tmp3);
				upshiftOrig.Add(tmp1);
				downshiftTransformed.Add(tmp2);
				downshiftOrig.Add(tmp3);
			}

			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(gearboxFile), fullLoadCurves, shiftPolygons,
				Path.Combine(Path.GetDirectoryName(gearboxFile), "Shiftlines.png"),
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / rdyn * axlegearRatio * gearboxData.Gears.Last().Ratio, upshiftOrig,
				downshiftTransformed, downshiftOrig);

			var shiftLines = "";
			var gear = 1;
			foreach (var shiftPolygon in shiftPolygons) {
				shiftLines += "Gear " + gear + "\n";
				shiftLines += "Upshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Upshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
				shiftLines += "Downshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Downshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
			}
		}

	}

	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ShiftPolygonComparison
	{
		const string BasePath = @"E:/QUAM/Workspace/Daten_INTERN/Testfahrzeuge/";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[Category("LongRunning")]
		[Ignore("No Assertions - only plotting shift polygons")]
		[
			TestCase(@"class2_12t_baseline/175kW_Diesel_example.vfld", @"class2_12t_baseline/delivery_12t_example.vgbx", 0.421,
				4.18, 600),
			TestCase(@"class2_12t_iaxle_long/175kW_Diesel_example.vfld", @"class2_12t_iaxle_long/delivery_12t_example.vgbx",
				0.421, 2.85, 600),
			TestCase(@"class2_12t_iaxle_short/175kW_Diesel_example.vfld", @"class2_12t_iaxle_short/delivery_12t_example.vgbx",
				0.421, 5.33, 600),
			TestCase(@"class2_12t_Pmax_high/220kW_Diesel_example.vfld", @"class2_12t_Pmax_high/delivery_12t_example_220kW.vgbx",
				0.421, 4.18, 600),
			TestCase(@"class2_12t_Pmax_low/130kW_Diesel_example.vfld", @"class2_12t_Pmax_low/delivery_12t_example.vgbx", 0.421,
				4.18, 600),
			TestCase(@"class5_40t_baseline/12L-324kW.vfld", @"class5_40t_baseline/tractor_12gear_example.vgbx", 0.421, 2.64,
				600),
			TestCase(@"class5_40t_iaxle_long/12L-324kW.vfld", @"class5_40t_iaxle_long/tractor_12gear_example.vgbx", 0.421, 2.31,
				600),
			TestCase(@"class5_40t_iaxle_short/12L-324kW.vfld", @"class5_40t_iaxle_short/tractor_12gear_example.vgbx", 0.421,
				3.71,
				600),
			TestCase(@"class5_40t_Pmax_high/13-9-L-375kW.vfld", @"class5_40t_Pmax_high/tractor_12gear_example.vgbx", 0.421,
				2.64, 600),
			TestCase(@"class5_40t_Pmax_low/9-6-L_260kW.vfld", @"class5_40t_Pmax_low/tractor_12gear_example.vgbx", 0.421, 2.64,
				600),
			Category(Definitions.DEPRECATED),
		]
		public void ComputeShiftPolygon(string engineFldFile, string gearboxFile, double rdyn, double axlegearRatio,
			double idlingSpeed)
		{
			if (!Directory.Exists(BasePath)) {
				Assert.Inconclusive("Confidential File not found. Test cannot run without file.");
			}

			var gearboxData = new JSONGearboxDataV6(JSONInputDataFactory.ReadFile(Path.Combine(BasePath, gearboxFile)),
				Path.Combine(BasePath, gearboxFile));
			var engineData = new CombustionEngineData() {
				IdleSpeed = idlingSpeed.RPMtoRad(),
			};

			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
			fullLoadCurves[0] = FullLoadCurveReader.ReadFromFile(Path.Combine(BasePath, engineFldFile), true);
			fullLoadCurves[0].EngineData = engineData;
			for (uint i = 1; i <= gearboxData.Gears.Count; i++) {
				fullLoadCurves[i] = AbstractSimulationDataAdapter.IntersectFullLoadCurves(fullLoadCurves[0],
					gearboxData.Gears[(int)(i - 1)].MaxTorque);
			}
			engineData.FullLoadCurves = fullLoadCurves;

			var shiftPolygons = new List<ShiftPolygon>();
			var downshiftTransformed = new List<List<Point>>();
			var upshiftOrig = new List<List<Point>>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(
					DeclarationData.Gearbox.ComputeShiftPolygon(gearboxData.Type, i, fullLoadCurves[(uint)(i + 1)], gearboxData.Gears,
						engineData, axlegearRatio, rdyn.SI<Meter>(), null)
					);
				ComputShiftPolygonPoints(i, fullLoadCurves[(uint)(i + 1)], gearboxData.Gears,
					engineData, axlegearRatio, rdyn.SI<Meter>(), out var tmp1, out var tmp2, out var tmp3);
				upshiftOrig.Add(tmp1);
				downshiftTransformed.Add(tmp2);
			}

			var imageFile = Path.GetDirectoryName(gearboxFile) + "_" + Path.GetFileNameWithoutExtension(gearboxFile) + "_" +
							Path.GetFileNameWithoutExtension(engineFldFile) +
							".png";
			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(gearboxFile), fullLoadCurves, shiftPolygons,
				imageFile,
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / rdyn.SI<Meter>() * axlegearRatio * gearboxData.Gears.Last().Ratio,
				upshiftOrig, downshiftTransformed);
			var str = "";
			var g = 1;
			foreach (var shiftPolygon in shiftPolygons) {
				str += "Gear " + g + "\n";
				str += "downshift\n";
				foreach (var entry in shiftPolygon.Downshift) {
					str += $"{entry.AngularSpeed.AsRPM} {entry.Torque.Value()}\n";
				}
				str += "upshift\n";
				foreach (var entry in shiftPolygon.Upshift) {
					str += $"{entry.AngularSpeed.AsRPM} {entry.Torque.Value()}\n";
				}
				g++;
			}
		}

		public static void ComputShiftPolygonPoints(int gear, EngineFullLoadCurve fullLoadCurve,
			IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius,
			out List<Point> upshiftOrig, out List<Point> downshiftTransformed, out List<Point> downshiftOrig)
		{
			var engineSpeed85kmhLastGear = DeclarationData.Gearbox.TruckMaxAllowedSpeed / dynamicTyreRadius * axlegearRatio *
											gears[gears.Count - 1].Ratio;
			//var engineSpeed85kmhSecondToLastGear = ComputeEngineSpeed85kmh(gears[gears.Count - 2], axlegearRatio,
			//	dynamicTyreRadius, engine);

			var nVHigh = VectoMath.Min(engineSpeed85kmhLastGear, engine.FullLoadCurves[0].RatedSpeed);

			var diffRatio = gears[gears.Count - 2].Ratio / gears[gears.Count - 1].Ratio - 1;

			var maxDragTorque = fullLoadCurve.MaxDragTorque * 1.1;

			var p1 = new Point(engine.IdleSpeed.Value() / 2, 0);
			var p2 = new Point(engine.IdleSpeed.Value() * 1.1, 0);
			var p3 = new Point(nVHigh.Value() * 0.9,
				fullLoadCurve.FullLoadStationaryTorque(nVHigh * 0.9).Value());

			var p4 =
				new Point((nVHigh * (1 + diffRatio / 3)).Value(), 0);
			var p5 = new Point(fullLoadCurve.N95hSpeed.Value(), fullLoadCurve.MaxTorque.Value());

			var p6 = new Point(p2.X, VectoMath.Interpolate(p1, p3, p2.X));
			var p7 = new Point(p4.X, VectoMath.Interpolate(p2, p5, p4.X));

			downshiftOrig = new[] { p2, p6, p3 }.ToList();

			//var fldMargin = ShiftPolygonFldMargin(fullLoadCurve.FullLoadEntries, nVHigh * 0.95);
			//var downshiftCorr = MoveDownshiftBelowFld(Edge.Create(p6, p3), fldMargin, 1.1 * fullLoadCurve.MaxTorque);
			upshiftOrig = new[] { p4, p7, p5 }.ToList();
			downshiftTransformed = new List<Point>();

			if (gear >= gears.Count - 1) {
				return;
			}
			var gearRatio = gears[gear].Ratio / gears[gear + 1].Ratio;
			var rpmMarginFactor = 1 + DeclarationData.Gearbox.ShiftPolygonRPMMargin / 100.0;

			var p2p = new Point(p2.X * gearRatio * rpmMarginFactor, p2.Y / gearRatio);
			var p3p = new Point(p3.X * gearRatio * rpmMarginFactor, p3.Y / gearRatio);
			var p6p = new Point(p6.X * gearRatio * rpmMarginFactor, p6.Y / gearRatio);
			var edgeP6pP3p = new Edge(p6p, p3p);
			var p3pExt = new Point((1.1 * p5.Y - edgeP6pP3p.OffsetXY) / edgeP6pP3p.SlopeXY, 1.1 * p5.Y);

			downshiftTransformed = new[] { p2p, p6p, p3pExt }.ToList();
		}

		/// <summary>
		/// VECTO-517 Shiftpolygon is considered invalid
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ShiftCurve_ShiftPolygon_Validation_Test()
		{
			var vgbs = new[] {
				"-50,685,1537",
				"550,685,1537",
				"678,763,1537",
				"1080,1008,2092",
				"1200,1081,2092",
				"1200,1081,2092",
				"3000,1081,2092"
			};

			var shiftPolygon =
				ShiftPolygonReader.Create(
					VectoCSVFile.ReadStream(
						InputDataHelper.InputDataAsStream("engine torque,downshift rpm [rpm],upshift rpm [rpm]	", vgbs)));

			var results = shiftPolygon.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.MT, false);
			Assert.IsFalse(results.Any(), results.Select(r => r.ErrorMessage).Join("\n"));
		}

		[
			TestCase(false, 650, 400),
			TestCase(true, 400, 500),
			TestCase(false, 900, 400),
			TestCase(false, 1200, 400),
			TestCase(true, 600, 900),
			TestCase(false, 1000, 900),
			TestCase(false, 1200, 900),
			TestCase(true, 300, 1300),
			TestCase(true, 900, 1300),
			TestCase(false, 1200, 1250),
			TestCase(false, 1200, 1600),
		]
		public void IsLeftOf_Test(bool result, double speed, double torque)
		{
			var segment = (new ShiftPolygon.ShiftPolygonEntry(550.SI<NewtonMeter>(), 685.RPMtoRad()), 
					new ShiftPolygon.ShiftPolygonEntry(1200.SI<NewtonMeter>(), 1080.RPMtoRad()));

			Assert.AreEqual(result, ShiftPolygon.IsLeftOf(speed.RPMtoRad(), torque.SI<NewtonMeter>(), segment));
		}


        [TestCase(@"E:/QUAM/Workspace/VECTO-Bugreports/BugReportTests/Bugreport Jobs/20190307_VECTO-904_Extrapolation/OM-18173493.xml")]
        //[TestCase(@"E:/QUAM/Workspace/VECTO_DEV_Hybrid/Generic Vehicles/Declaration Mode/Group5_Tractor_4x2/Class5_Tractor_DECL.xml")]
        public void ComputeShiftPolygonXML(string xmlJob)
		{
			if (!File.Exists(xmlJob)) {
				Assert.Inconclusive("Confidential File not found. Test cannot run without file.");
			}
			var inputData = xmlInputReader.CreateDeclaration(xmlJob);
			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();

			var gearboxData = inputData.JobInputData.Vehicle.Components.GearboxInputData;
			var engineData = dao.CreateEngineData(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First(), new Mission() {MissionType = MissionType.LongHaul});

			var fullLoadCurves = engineData.FullLoadCurves;

			var axlegearRatio = inputData.JobInputData.Vehicle.Components.AxleGearInputData.Ratio;
			var vehicle = inputData.JobInputData.Vehicle;
			var segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis,
				false);
			var vehicleData = dao.CreateVehicleData(inputData.JobInputData.Vehicle, segment, segment.Missions.First(),
				new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.LowLoading,
					new Tuple<Kilogram, double?>(0.SI<Kilogram>(), null)), true);
			var rdyn = vehicleData.DynamicTyreRadius;

			var shiftPolygons = new List<ShiftPolygon>();
			var downshiftTransformed = new List<List<Point>>();
			var upshiftOrig = new List<List<Point>>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(
					DeclarationData.Gearbox.ComputeShiftPolygon(gearboxData.Type, i, fullLoadCurves[(uint)(i + 1)], gearboxData.Gears,
						engineData, axlegearRatio, rdyn, null)
					);
				ComputShiftPolygonPoints(i, fullLoadCurves[(uint)(i + 1)], gearboxData.Gears,
					engineData, axlegearRatio, rdyn, out var tmp1, out var tmp2, out var tmp3);
				upshiftOrig.Add(tmp1);
				downshiftTransformed.Add(tmp2);
			}

			var imageFile = Path.Combine(Path.GetDirectoryName(xmlJob), Path.GetFileNameWithoutExtension(xmlJob) + "_shiftlines.png");
			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(xmlJob), fullLoadCurves, shiftPolygons,
				imageFile,
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / rdyn * axlegearRatio * gearboxData.Gears.Last().Ratio,
				upshiftOrig, downshiftTransformed);
			var str = "";
			var g = 1;
			foreach (var shiftPolygon in shiftPolygons) {
				str += "Gear " + g + "\n";
				str += "downshift\n";
				foreach (var entry in shiftPolygon.Downshift) {
					str += $"{entry.AngularSpeed.AsRPM} {entry.Torque.Value()}\n";
				}
				str += "upshift\n";
				foreach (var entry in shiftPolygon.Upshift) {
					str += $"{entry.AngularSpeed.AsRPM} {entry.Torque.Value()}\n";
				}
				g++;
			}
		}

		[TestCase(@"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG.vecto", null),
		TestCase(@"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_Cont30kW.vecto", null),
		TestCase(@"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_Cont30kW.vecto", 0.9),
		TestCase(@"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_Cont30kW.vecto", 1.1),
		]
		public void ComputePEVShiftLines(string pevE2Job, double? factorDownshiftSpeed)
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(pevE2Job) as IEngineeringInputDataProvider;
			var gearboxData = inputData.JobInputData.Vehicle.Components.GearboxInputData;
			var dao = new EngineeringDataAdapter();
			var emData = dao.CreateElectricMachines(inputData.JobInputData.Vehicle.Components.ElectricMachines,
				null, null).FirstOrDefault()?.Item2;
			var axlegearRatio = inputData.JobInputData.Vehicle.Components.AxleGearInputData.Ratio;
			var vehicleData = dao.CreateVehicleData(inputData.JobInputData.Vehicle);
			var r_dyn = vehicleData.DynamicTyreRadius;

			var fullLoadCurve = emData.EfficiencyData.VoltageLevels.First().FullLoadCurve.FullLoadEntries.Select(x =>
				new EngineFullLoadCurve.FullLoadCurveEntry() {
					EngineSpeed = x.MotorSpeed,
					TorqueFullLoad = -x.FullDriveTorque,
					TorqueDrag = -x.FullGenerationTorque
				}).ToList();
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
			var engineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad()
			};
			fullLoadCurves[(uint)(0)] = new EngineFullLoadCurve(fullLoadCurve, null) { EngineData = engineData};
			var shiftPolygons = new List<ShiftPolygon>();

			var runData = new VectoRunData() {
				GearshiftParameters = new ShiftStrategyParameters(),
				ExecutionMode = ExecutionMode.Engineering,
            };
			if (factorDownshiftSpeed.HasValue) {
				runData.GearshiftParameters.PEV_DownshiftSpeedFactor = factorDownshiftSpeed.Value;
			}
			var shiftStrategy = new PEVAMTShiftStrategyPolygonCreator(runData.GearshiftParameters);
			
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(shiftStrategy.ComputeDeclarationShiftPolygon(GearboxType.AMT, i, null, gearboxData.Gears,
					null, axlegearRatio, r_dyn, emData));
				//shiftPolygons.Add(deRatedShiftLines[(uint)(i + 1)]);
				fullLoadCurves[(uint)(i + 1)] = new EngineFullLoadCurve(fullLoadCurve, null) { EngineData = engineData };
			}

			var shiftLines = "";
			var gear = 1;
			foreach (var shiftPolygon in shiftPolygons) {
				shiftLines += "Gear " + gear++ + "\n";
				shiftLines += "Upshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Upshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
				shiftLines += "Downshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Downshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
			}

			TestContext.WriteLine(shiftLines);

            var suffix = factorDownshiftSpeed.HasValue ? $"_{factorDownshiftSpeed.Value}" : "";
			var imageFile = Path.Combine(Path.GetDirectoryName(pevE2Job), Path.GetFileNameWithoutExtension(pevE2Job) + $"_shiftlines{suffix}.png");

			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(pevE2Job), fullLoadCurves, shiftPolygons,
				imageFile,
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / r_dyn * axlegearRatio * gearboxData.Gears.Last().Ratio);
		}

		[TestCase(null),
		TestCase(0.9)]
		public void ComputePEVShiftLinesDeRated(double? factorDownshiftSpeed)
		{
			var pevE2Job = @"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_Cont30kW.vecto";

			var inputData = JSONInputDataFactory.ReadJsonJob(pevE2Job) as IEngineeringInputDataProvider;
			var gearboxData = inputData.JobInputData.Vehicle.Components.GearboxInputData;
			var dao = new EngineeringDataAdapter();
			var emData = dao.CreateElectricMachines(inputData.JobInputData.Vehicle.Components.ElectricMachines,
				null, null).FirstOrDefault()?.Item2;
			
			var axlegearRatio = inputData.JobInputData.Vehicle.Components.AxleGearInputData.Ratio;
			var vehicleData = dao.CreateVehicleData(inputData.JobInputData.Vehicle);
			var r_dyn = vehicleData.DynamicTyreRadius;

            //emData.ContinuousTorque = 500.SI<NewtonMeter>();
            var contTqFld = new ElectricMotorFullLoadCurve(new List<ElectricMotorFullLoadCurve.FullLoadEntry>() {
				new ElectricMotorFullLoadCurve.FullLoadEntry() {
					MotorSpeed = 0.RPMtoRad(),
					FullDriveTorque = -emData.Overload.ContinuousTorque,
					FullGenerationTorque = emData.Overload.ContinuousTorque
				},
				new ElectricMotorFullLoadCurve.FullLoadEntry() {
					MotorSpeed = 1.1 * emData.EfficiencyData.VoltageLevels.First().FullLoadCurve.MaxSpeed,
					FullDriveTorque = -emData.Overload.ContinuousTorque,
					FullGenerationTorque = emData.Overload.ContinuousTorque
				}
			});
			var limitedFld = AbstractSimulationDataAdapter.IntersectEMFullLoadCurves(emData.EfficiencyData.VoltageLevels.First().FullLoadCurve, contTqFld);

			var fullLoadCurve = limitedFld.FullLoadEntries.Select(x =>
				new EngineFullLoadCurve.FullLoadCurveEntry() {
					EngineSpeed = x.MotorSpeed,
					TorqueFullLoad = -x.FullDriveTorque,
					TorqueDrag = -x.FullGenerationTorque
				}).ToList();
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
			var engineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad()
			};
			fullLoadCurves[(uint)(0)] = new EngineFullLoadCurve(fullLoadCurve, null) { EngineData = engineData };
			
			var shiftPolygons = new List<ShiftPolygon>();
			var runData = new VectoRunData() { GearshiftParameters = new ShiftStrategyParameters(), ExecutionMode = ExecutionMode.Engineering, };
			if (factorDownshiftSpeed.HasValue) {
				runData.GearshiftParameters.PEV_DeRatedDownshiftSpeedFactor = factorDownshiftSpeed.Value;
			}
			var shiftStrategy = new PEVAMTShiftStrategyPolygonCreator(runData.GearshiftParameters);
			//var deRatedShiftLines = shiftStrategy.ComputeElectricMotorDeclarationShiftPolygon(emData, gearboxData.Gears,
			//	r_dyn, axlegearRatio, gearboxData.Type);

            for (var i = 0; i < gearboxData.Gears.Count; i++) {
				var deratedShiftLine = shiftStrategy.ComputeElectricMotorDeclarationShiftPolygon(GearboxType.APTN, i,
					gearboxData.Gears, axlegearRatio, r_dyn, emData, emData);
				shiftPolygons.Add(deratedShiftLine);
				fullLoadCurves[(uint)(i + 1)] = new EngineFullLoadCurve(fullLoadCurve, null) { EngineData = engineData };
			}

			var shiftLines = "";
			var gear = 1;
			foreach (var shiftPolygon in shiftPolygons) {
				shiftLines += "Gear " + gear++ + "\n";
				shiftLines += "Upshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Upshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
				shiftLines += "Downshift\n";
				foreach (var shiftPolygonEntry in shiftPolygon.Downshift) {
					shiftLines += $"{shiftPolygonEntry.AngularSpeed.AsRPM} {shiftPolygonEntry.Torque.Value()}\n";
				}
			}
			TestContext.WriteLine(shiftLines);

            var suffix = factorDownshiftSpeed.HasValue ? $"_{factorDownshiftSpeed.Value}" : "";
			var imageFile = Path.Combine(Path.GetDirectoryName(pevE2Job), Path.GetFileNameWithoutExtension(pevE2Job) + $"_shiftlines_DeRated{suffix}.png");

			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(pevE2Job), fullLoadCurves, shiftPolygons,
				imageFile,
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / r_dyn * axlegearRatio * gearboxData.Gears.Last().Ratio);
		}
	}
}
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
using System.Data;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using System.IO;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class GearboxDataTest
	{
		protected const string GearboxFile = @"TestData/Components/24t Coach.vgbx";

		protected const string EngineFile = @"TestData/Components/24t Coach.veng";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGearboxDataReadTest()
		{
			var axleData = MockSimulationDataFactory.CreateAxleGearDataFromFile(GearboxFile);
			Assert.AreEqual(3.240355, axleData.AxleGear.Ratio, 0.0001);

			var gbxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxFile, EngineFile, false);
			Assert.AreEqual(GearboxType.AMT, gbxData.Type);
			Assert.AreEqual(1.0, gbxData.TractionInterruption.Value(), 0.0001);
			Assert.AreEqual(8, gbxData.Gears.Count);

			Assert.AreEqual(1.0, gbxData.Gears[7].Ratio, 0.0001);

			Assert.AreEqual(-400, gbxData.Gears[1].ShiftPolygon.Downshift[0].Torque.Value(), 0.0001);
			Assert.AreEqual(560.RPMtoRad().Value(), gbxData.Gears[1].ShiftPolygon.Downshift[0].AngularSpeed.Value(), 0.0001);
			Assert.AreEqual(1289.RPMtoRad().Value(), gbxData.Gears[1].ShiftPolygon.Upshift[0].AngularSpeed.Value(), 0.0001);

			Assert.AreEqual(200.RPMtoRad().Value(), gbxData.Gears[1].LossMap[26].InputSpeed.Value(), 0.0001);
			Assert.AreEqual(-350, gbxData.Gears[1].LossMap[35].InputTorque.Value(), 0.0001);
			Assert.AreEqual(13.072, gbxData.Gears[1].LossMap[35].TorqueLoss.Value(), 0.0001);
		}

		[TestCase("Test1", @"TestData/Components/24t Coach.vgbx", 520, 20.320, "A", 279698.4, 9401.44062)]
		[TestCase("Test2", @"TestData/Components/24t Coach.vgbx", 520, 0.5858335, "A", 17173.5, 409.773677587509)]
		[TestCase("Test3", @"TestData/Components/24t Coach.vgbx", 520, 0.3996113, "A", 292.5253, 118.282541632652)]
		[TestCase("Test4", @"TestData/Components/24t Coach.vgbx", 520, 5.327739, "A", 57431.12, 2222.78785705566)]
		[TestCase("Test5", @"TestData/Components/24t Coach.vgbx", 520, 5.661779, "A", 73563.93, 2553.00283432007)]
		[TestCase("Test6", @"TestData/Components/24t Coach.vgbx", 520, 14.15156, "A", 212829.5, 6822.16882705688)]
		[TestCase("Test7", @"TestData/Components/24t Coach.vgbx", 520, 14.55574, "A", 15225.52, 4308.41207504272)]
		[TestCase("Test8", @"TestData/Components/24t Coach.vgbx", 520, 4.601774, "A", -1240.225, 1362.09738254547)]
		[TestCase("Test9", @"TestData/Components/24t Coach.vgbx", 520, 3.934339, "A", -698.5989, 1164.5405292511)]
		[Category(Definitions.TESTCASE_MIGRATED)]
		public void TestInterpolation(string testName, string gearboxDataFile, double rDyn, double v, string gear,
			double powerGbxOut, double gbxPowerLoss)
		{
			var rdyn = rDyn;

			var angSpeed = (60 * v / (2 * rdyn * Math.PI / 1000)).RPMtoRad();

			var axleData = MockSimulationDataFactory.CreateAxleGearDataFromFile(gearboxDataFile);

			var pvD = powerGbxOut.SI<Watt>();

			var torqueToWheels = Formulas.PowerToTorque(pvD, angSpeed);
			var torqueFromEngine = 0.SI<NewtonMeter>();

			if (gear == "A") {
				torqueFromEngine = torqueToWheels / axleData.AxleGear.Ratio;
				torqueFromEngine += axleData.AxleGear.LossMap.GetTorqueLoss(angSpeed, torqueToWheels).Value;
			}

			var powerEngine = Formulas.TorqueToPower(torqueFromEngine, angSpeed * axleData.AxleGear.Ratio);
			var loss = powerEngine - pvD;

			Assert.AreEqual(gbxPowerLoss, loss.Value(), 0.1, testName);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestLossMap_IN_10_CONST_Interpolation_Extrapolation()
		{
			var data = new DataTable();
			data.Columns.Add("");
			data.Columns.Add("");
			data.Columns.Add("");
			data.Rows.Add("0", "0", "10"); //         (0,100):10 --  (100,100):10
			data.Rows.Add("0", "100", "10"); //        |      \          |
			data.Rows.Add("100", "0", "10"); //        |       \         |
			data.Rows.Add("100", "100", "10"); //    (0,0):10  ----- (100,10):10

			var map = TransmissionLossMapReader.Create(data, 1.0, "1");

			// test inside the triangles
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(25.RPMtoRad(), 25.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(75.RPMtoRad(), 50.SI<NewtonMeter>()).Value);

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), -5.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 45.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 40.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 75.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), 25.SI<NewtonMeter>()).Value);

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()).Value);

			// test outside the corners
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), 120.SI<NewtonMeter>()).Value);

			// test outside the edges
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
		}

		//[TestCase]
		//public void TestLossMap_OUT_10_CONST_Interpolation_Extrapolation()
		//{
		//	var data = new DataTable();
		//	data.Columns.Add("");
		//	data.Columns.Add("");
		//	data.Columns.Add("");
		//	data.Rows.Add("0", "0", "10"); //         (0,100):10 --  (100,100):10
		//	data.Rows.Add("0", "100", "10"); //        |      \          |
		//	data.Rows.Add("100", "0", "10"); //        |       \         |
		//	data.Rows.Add("100", "100", "10"); //    (0,0):10  ----- (100,10):10

		//	var map = TransmissionLossMapReader.Create(data, 1.0, "1");

		//	// test inside the triangles
		//	AssertHelper.AreRelativeEqual(15, map.GetOutTorque(25.RPMtoRad(), 25.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(40, map.GetOutTorque(75.RPMtoRad(), 50.SI<NewtonMeter>(), true));

		//	// test interpolation on edges
		//	AssertHelper.AreRelativeEqual(-15, map.GetOutTorque(50.RPMtoRad(), -5.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(35, map.GetOutTorque(0.RPMtoRad(), 45.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(30, map.GetOutTorque(50.RPMtoRad(), 40.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(65, map.GetOutTorque(50.RPMtoRad(), 75.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(15, map.GetOutTorque(100.RPMtoRad(), 25.SI<NewtonMeter>(), true));

		//	// test interpolation on corner points
		//	AssertHelper.AreRelativeEqual(-10, map.GetOutTorque(0.RPMtoRad(), 0.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(80, map.GetOutTorque(0.RPMtoRad(), 90.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(-20, map.GetOutTorque(100.RPMtoRad(), -10.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(50, map.GetOutTorque(100.RPMtoRad(), 60.SI<NewtonMeter>(), true));

		//	// test outside the corners
		//	AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(-20.RPMtoRad(), -20.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(110, map.GetOutTorque(-20.RPMtoRad(), 120.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(120.RPMtoRad(), -20.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(110, map.GetOutTorque(120.RPMtoRad(), 120.SI<NewtonMeter>(), true));

		//	// test outside the edges
		//	AssertHelper.AreRelativeEqual(40, map.GetOutTorque(-20.RPMtoRad(), 50.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(110, map.GetOutTorque(50.RPMtoRad(), 120.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(50.RPMtoRad(), -20.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(40, map.GetOutTorque(120.RPMtoRad(), 50.SI<NewtonMeter>(), true));
		//}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestLossMap_IN_Interpolation_Extrapolation()
		{
			var data = new DataTable();
			data.Columns.Add("");
			data.Columns.Add("");
			data.Columns.Add("");
			data.Rows.Add("0", "0", "0"); //         (0,110):10 --  (100,140):40
			data.Rows.Add("0", "110", "10"); //        |      \         |
			data.Rows.Add("100", "10", "10"); //        |       \       |
			data.Rows.Add("100", "140", "40"); //    (0,0):0 ----- (100,10):10

			var map = TransmissionLossMapReader.Create(data, 1.0, "1");

			// test inside the triangles
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(25.RPMtoRad(), 25.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(75.RPMtoRad(), 50.SI<NewtonMeter>()).Value);

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(50.RPMtoRad(), -5.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(4.5, map.GetTorqueLoss(0.RPMtoRad(), 45.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(9, map.GetTorqueLoss(50.RPMtoRad(), 40.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(50.RPMtoRad(), 75.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(100.RPMtoRad(), 25.SI<NewtonMeter>()).Value);

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(9, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(28, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()).Value);

			// test outside the corners
			AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(-20.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(40, map.GetTorqueLoss(120.RPMtoRad(), 120.SI<NewtonMeter>()).Value);

			// test outside the edges
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(-20.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(25, map.GetTorqueLoss(50.RPMtoRad(), 120.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(50.RPMtoRad(), -20.SI<NewtonMeter>()).Value);
			AssertHelper.AreRelativeEqual(25, map.GetTorqueLoss(120.RPMtoRad(), 50.SI<NewtonMeter>()).Value);
		}

		//[TestCase]
		//public void TestLossMap_OUT_Interpolation_Extrapolation()
		//{
		//	var data = new DataTable();
		//	data.Columns.Add("");
		//	data.Columns.Add("");
		//	data.Columns.Add("");
		//	data.Rows.Add("0", "0", "0"); //         (0,100):10 -- (100,100):40
		//	data.Rows.Add("0", "100", "10"); //        |      \        |
		//	data.Rows.Add("100", "0", "10"); //        |       \       |
		//	data.Rows.Add("100", "100", "40"); //    (0,0):0 ----- (100,0):10

		//	var map = TransmissionLossMapReader.Create(data, 1.0, "1");

		//	// test inside the triangles
		//	AssertHelper.AreRelativeEqual(20, map.GetOutTorque(25.RPMtoRad(), 25.SI<NewtonMeter>()));
		//	AssertHelper.AreRelativeEqual(32.5, map.GetOutTorque(75.RPMtoRad(), 50.SI<NewtonMeter>()));

		//	// test interpolation on edges
		//	AssertHelper.AreRelativeEqual(-5, map.GetOutTorque(50.RPMtoRad(), 0.SI<NewtonMeter>()));
		//	AssertHelper.AreRelativeEqual(40.5, map.GetOutTorque(0.RPMtoRad(), 45.SI<NewtonMeter>()));
		//	AssertHelper.AreRelativeEqual(31, map.GetOutTorque(50.RPMtoRad(), 40.SI<NewtonMeter>()));
		//	AssertHelper.AreRelativeEqual(57.5, map.GetOutTorque(50.RPMtoRad(), 75.SI<NewtonMeter>()));
		//	AssertHelper.AreRelativeEqual(7.5, map.GetOutTorque(100.RPMtoRad(), 25.SI<NewtonMeter>()));

		//	// test interpolation on corner points
		//	AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()).Value);
		//	AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()).Value);
		//	AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()).Value);
		//	AssertHelper.AreRelativeEqual(40, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()).Value);

		//	// test outside the corners
		//	AssertHelper.AreRelativeEqual(-20, map.GetOutTorque(-20.RPMtoRad(), -20.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(110, map.GetOutTorque(-20.RPMtoRad(), 120.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(120.RPMtoRad(), -20.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(80, map.GetOutTorque(120.RPMtoRad(), 120.SI<NewtonMeter>(), true));

		//	// test outside the edges
		//	AssertHelper.AreRelativeEqual(45, map.GetOutTorque(-20.RPMtoRad(), 50.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(95, map.GetOutTorque(50.RPMtoRad(), 120.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(-25, map.GetOutTorque(50.RPMtoRad(), -20.SI<NewtonMeter>(), true));
		//	AssertHelper.AreRelativeEqual(25, map.GetOutTorque(120.RPMtoRad(), 50.SI<NewtonMeter>(), true));

		//	// test extrapolation not allowed
		//	AssertHelper.Exception<VectoException>(() => { map.GetOutTorque(120.RPMtoRad(), 50.SI<NewtonMeter>()); });
		//}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestFullLoadCurveIntersection()
		{
			var engineFldString = new[] {
				"560, 1180, -149",
				"600, 1282, -148",
				"800, 1791, -149",
				"1000, 2300, -160",
				"1200, 2300, -179",
				"1400, 2300, -203",
				"1600, 2079, -235",
				"1800, 1857, -264",
				"2000, 1352, -301",
				"2100, 1100, -320",
			};
			var dataEng =
				VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm]", engineFldString));
			var engineFld = FullLoadCurveReader.Create(dataEng, true);


			var fullLoadCurve = AbstractSimulationDataAdapter.IntersectFullLoadCurves(engineFld, 2500.SI<NewtonMeter>());

			Assert.AreEqual(10, fullLoadCurve.FullLoadEntries.Count);

			Assert.AreEqual(1180.0, fullLoadCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value());
			Assert.AreEqual(1100.0, fullLoadCurve.FullLoadStationaryTorque(2100.RPMtoRad()).Value());
		}

		/// <summary>
		///		VECTO-190
		/// </summary>
		[TestCase, 
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestFullLoadSorting()
		{
			var gbxFldString = new[] {
				"600, 1000, -100",
				"2400, 2000, -120",
				"1000, 500, -110"
			};

			var dataGbx =
				VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm], Mdrag [Nm]", gbxFldString));
			var gbxFld = FullLoadCurveReader.Create(dataGbx, true);

			var maxTorque = gbxFld.FullLoadStationaryTorque(800.RPMtoRad());
			Assert.AreEqual(750, maxTorque.Value());
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestLossMapExtension()
		{
			var gbxFile = @"TestData/Components/Indirect Gear.vtlm";
			
			var lossMapOrig = TransmissionLossMapReader.ReadFromFile(gbxFile, 1.0, "origLossMap");
			var extendedMap = TransmissionLossMapReader.ReadFromFile(gbxFile, 1.0, "origLossMap", true);

			var rpm = 100.RPMtoRad();
			var tq = -3000.SI<NewtonMeter>();
			var lookupOrig = lossMapOrig.GetTorqueLoss(rpm, tq);
			var lookupExt = extendedMap.GetTorqueLoss(rpm, tq);

			//foreach (var entry in gearboxDataExt.Gears[7].LossMap._entries) {
			//	Console.WriteLine(string.Format("{0},{1},{2}", entry.InputSpeed.AsRPM, entry.InputTorque.Value(), entry.TorqueLoss.Value()));
			//}

			Assert.IsTrue(lookupOrig.Extrapolated);
			Assert.IsFalse(lookupExt.Extrapolated);

			rpm = 1200.RPMtoRad();
			lookupOrig = lossMapOrig.GetTorqueLoss(rpm, tq);
			lookupExt = extendedMap.GetTorqueLoss(rpm, tq);

			Assert.IsTrue(lookupOrig.Extrapolated);
			Assert.IsFalse(lookupExt.Extrapolated);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestAxlegearLossMapExtension()
		{
			var lossMapFile = @"TestData/Components/Axle.vtlm";

			var origLossMap = TransmissionLossMapReader.ReadFromFile(lossMapFile, 3.240355, "AxleOrig");
			var extendedLossMap = TransmissionLossMapReader.ReadFromFile(lossMapFile, 3.240355, "AxleExtended", true);

			//foreach (var entry in gearboxDataExt.AxleGear.LossMap._entries) {
			//	Console.WriteLine(string.Format("{0},{1},{2}", entry.InputSpeed.AsRPM, entry.InputTorque.Value(), entry.TorqueLoss.Value()));
			//}

			var rpm = 100.RPMtoRad();
			var tq = 80000.SI<NewtonMeter>();
			var lookupOrig = origLossMap.GetTorqueLoss(rpm, tq);
			var lookupExt = extendedLossMap.GetTorqueLoss(rpm, tq);

			Assert.IsTrue(lookupOrig.Extrapolated);
			Assert.IsFalse(lookupExt.Extrapolated);

			rpm = 1000.RPMtoRad();
			lookupOrig = origLossMap.GetTorqueLoss(rpm, tq);
			lookupExt = extendedLossMap.GetTorqueLoss(rpm, tq);

			Assert.IsTrue(lookupOrig.Extrapolated);
			Assert.IsFalse(lookupExt.Extrapolated);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestLossMapExtensionNegativeRegressionSlope()
		{
			var entries = new[] {
				"Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm] # this is a comment",
				"200,-2500,60",
				"200,0,30",
				"200,100,35",
				"200,200,40",
				"200,500,32",
				"200,1000,30",
				"200,2000,25",
				"200,2500,23",
				"500,-2500,60",
				"500,0,35",
				"500,2500,23"
			};
			var mstream = new MemoryStream();
			var writer = new StreamWriter(mstream);
			
			foreach (var entry in entries) {
				writer.WriteLine(entry);
			}
			writer.Flush();
			mstream.Flush();
			mstream.Seek(0, SeekOrigin.Begin);
			
			var lossMap = TransmissionLossMapReader.Create(VectoCSVFile.ReadStream(mstream), 1.0, "TestGear", true);

			var validation = lossMap.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.AMT, false);

			Assert.AreEqual(0, validation.Count);

			var lookup1 = lossMap.GetTorqueLoss(200.RPMtoRad(), 500.SI<NewtonMeter>());
			Assert.IsFalse(lookup1.Extrapolated);
			Assert.AreEqual(31.8725, lookup1.Value.Value(), 1e-3);

			var lookup2 = lossMap.GetTorqueLoss(200.RPMtoRad(), 7000.SI<NewtonMeter>());
			Assert.IsFalse(lookup2.Extrapolated);
			Assert.AreEqual(23, lookup2.Value.Value());
		}

	}

}
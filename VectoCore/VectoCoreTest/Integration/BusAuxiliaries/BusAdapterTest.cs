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

using System.Data;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class BusAdapterTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(12000, 1256, 148, 148, 5649.8149)]
		[TestCase(12000, 1256, -48, -148, 5649.8149)]
		[TestCase(12000, 1256, 48, -148, 5649.8149)]
		[TestCase(12000, 800, 148, 148, 5939.985)]
		[TestCase(12000, 800, -48, -148, 5939.985)]
		[TestCase(12000, 800, 48, -148, 5939.985),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestNoSmartAuxDuringDrive(double vehicleWeight, double engineSpeedRpm, double driveLinePower,
			double internalPower, double expectedPowerDemand)
		{
			var busAux = AuxDemandTest.CreateBusAuxAdapterForTesting(vehicleWeight, out var driver);

			driver.DriverBehavior = DrivingBehavior.Driving;
			driver.DrivingAction = DrivingAction.Accelerate;

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-2);
		}

		[TestCase(12000, 1256, 148, 148, 5649.814)]
		[TestCase(12000, 1256, -28, -27, 5649.814)]
		[TestCase(12000, 1256, -28, -29, 5649.814)]
		[TestCase(12000, 1256, -128, -28, 5649.814)]
		[TestCase(12000, 1256, 28, -28, 5649.814)]
		[TestCase(12000, 800, 148, 148, 5939.9854)]
		[TestCase(12000, 800, -14, -13, 5939.9854)]
		[TestCase(12000, 800, -14, -15, 5939.9854)]
		[TestCase(12000, 800, -35, -14, 5939.9854)]
		[TestCase(12000, 800, 35, -14, 5939.9854),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestNoSmartAuxDuringCoasting(double vehicleWeight, double engineSpeedRpm, double driveLinePower,
			double internalPower, double expectedPowerDemand)
		{
			// this test is to make sure that the aux power-demand does not jump between average and smart power demand
			// when searching for the operating point for coasting (i.e. power demand (internal Power) is close to the motoring curve, 
			// intependent of power demand of power train)
			var busAux = AuxDemandTest.CreateBusAuxAdapterForTesting(vehicleWeight, out var driver);

			driver.DriverBehavior = DrivingBehavior.Coasting;
			driver.DrivingAction = DrivingAction.Coast;

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-2);
		}

		[TestCase(12000, 1256, -48, -28, 8516.9257)] // smart PS active - power demand below engine drag
		[TestCase(12000, 1256, 48, -28, 5649.8149)] // no smart aux active - positive power demand
		[TestCase(12000, 800, -48, -28, 7844.2956)] // smart PS active - power demand below engine drag
		[TestCase(12000, 800, 48, -28, 5939.985),
		Category(Definitions.TESTCASE_MIGRATED)] // no smart aux active - positive power demand
		public void TestSmartAuxDuringBrake(double vehicleWeight, double engineSpeedRpm, double driveLinePower,
			double internalPower, double expectedPowerDemand)
		{
			var busAux = AuxDemandTest.CreateBusAuxAdapterForTesting(vehicleWeight, out var driver);

			driver.DriverBehavior = DrivingBehavior.Braking;
			driver.DrivingAction = DrivingAction.Brake;

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-3);
		}

		[Test,
		TestCase(19000),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void AuxDemandContinuityTest(double vehicleWeight)
		{
			var busAux = AuxDemandTest.CreateBusAuxAdapterForTesting(vehicleWeight, out var driver);

			driver.DriverBehavior = DrivingBehavior.Driving;

			//var engineSpeedRpm = 1375.1014;

			var table = new DataTable();
			table.Columns.Add("engineSpeed", typeof(double));
			table.Columns.Add("auxPowerDemand", typeof(double));

			for (var engineSpeedRpm = 1370.0; engineSpeedRpm < 1380; engineSpeedRpm += 1e-3) {
				var driveLinePower = 2200.0;
				var internalPower = driveLinePower + 43;

				var engineDrivelinePower = driveLinePower.SI<Watt>();
				var engineSpeed = engineSpeedRpm.RPMtoRad();
				busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

				var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);

				var row = table.NewRow();
				row["engineSpeed"] = engineSpeed.Value() / Constants.RPMToRad;
				row["auxPowerDemand"] = (torque * engineSpeed).Value();
				table.Rows.Add(row);
			}
			var pt1 = new Point((double)(table.Rows[0]["engineSpeed"]), (double)(table.Rows[0]["auxPowerDemand"]));
			var pt2 = new Point((double)(table.Rows[table.Rows.Count - 1]["engineSpeed"]),
				(double)(table.Rows[table.Rows.Count - 1]["auxPowerDemand"]));
			var edge = new Edge(pt1, pt2);
			var slope = edge.SlopeXY;
			var offset = edge.OffsetXY;
			foreach (DataRow row in table.Rows) {
				var expected = (double)row["engineSpeed"] * slope + offset;
				Assert.AreEqual(expected, (double)row["auxPowerDemand"], 0.1);
			}

			//VectoCSVFile.Write("auxPowerDemand_EngineSpeed.csv", table);
		}
	}
}

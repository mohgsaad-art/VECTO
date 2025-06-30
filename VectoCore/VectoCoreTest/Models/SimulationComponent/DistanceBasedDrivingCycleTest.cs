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

using System.Globalization;
using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using MockDriver = TUGraz.VectoCore.Tests.Utils.MockDriver;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DistanceBasedDrivingCycleTest
	{
		public const string ShortCycle = @"TestData/Cycles/Coach_24t_xshort.vdri";

		public const double Tolerance = 0.0001;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestLimitRequst()
		{
			var data = new string[] {
				// <s>,<v>,<grad>,<stop>
				"  0,  20,  0,     0",
				"  1,  20, -0.1,   0",
				"  2,  20, -0.3,   0",
				" 10,  40, -0.3,   0",
				" 19,  40, -0.2,   0",
				" 20,  30, -0.1,   0"
			};
			var cycleData = SimpleDrivingCycles.CreateCycleData(data);
			var container = VehicleContainer.CreateVehicleContainer(new VectoRunData() {
				GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				},
			}, null, null);
			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var gbx = new MockGearbox(container);
			new ZeroMileageCounter(container);
			new DummyVehicleInfo(container);
			var driver = new MockDriver(container);
			cycle.InPort().Connect(driver.OutPort());

			cycle.OutPort().Initialize();

			// just in test mock driver
			driver.VehicleStopped = false;

			var absTime = 0.SI<Second>();

			// a request up to 10m succeeds, no speed change for the next 10m

			var response = cycle.OutPort().Request(absTime, 0.3.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);

			response = cycle.OutPort().Request(absTime, 1.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);

			response = cycle.OutPort().Request(absTime, 1.3.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);

			response = cycle.OutPort().Request(absTime, 2.7.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);

			response = cycle.OutPort().Request(absTime, 3.5.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);

			// a request with 12m exceeds the speed change at 10m -> maxDistance == 10m

			response = cycle.OutPort().Request(absTime, 12.SI<Meter>());
			Assert.IsInstanceOf<ResponseDrivingCycleDistanceExceeded>(response);
			Assert.AreEqual(10, ((ResponseDrivingCycleDistanceExceeded)response).MaxDistance.Value());

			response = cycle.OutPort().Request(absTime, 10.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);

			// drive 10m
			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			// - - - - - - - - 
			// request with 8m succeeds

			response = cycle.OutPort().Request(absTime, 8.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);

			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			// - - - - - - - - 
			// request with 3m more -> distance exceeded. maxDistance == 2m (approach next speed change, we are within 5m radius)

			response = cycle.OutPort().Request(absTime, 3.SI<Meter>());
			Assert.IsInstanceOf<ResponseDrivingCycleDistanceExceeded>(response);
			Assert.AreEqual(2, ((ResponseDrivingCycleDistanceExceeded)response).MaxDistance.Value());

			// - - - - - - - - 
			// request with 1m (18 -> 19m) => response exceeded, drive up to next sample point (at least 5m)
			response = cycle.OutPort().Request(absTime, 1.SI<Meter>());
			Assert.IsInstanceOf<ResponseDrivingCycleDistanceExceeded>(response);
			Assert.AreEqual(Constants.SimulationSettings.BrakeNextTargetDistance.Value(),
				((ResponseDrivingCycleDistanceExceeded)response).MaxDistance.Value(), 1e-6);

			// next request with 5m, as suggested => distance exceeded. maxDistance == 2m (next speed change)....
			response = cycle.OutPort().Request(absTime, ((ResponseDrivingCycleDistanceExceeded)response).MaxDistance);
			Assert.IsInstanceOf<ResponseDrivingCycleDistanceExceeded>(response);
			Assert.AreEqual(2, ((ResponseDrivingCycleDistanceExceeded)response).MaxDistance.Value());

			// ok
			response = cycle.OutPort().Request(absTime, ((ResponseDrivingCycleDistanceExceeded)response).MaxDistance);
			Assert.IsInstanceOf<ResponseSuccess>(response);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestDistanceRequest()
		{
			var cycleData = DrivingCycleDataReader.ReadFromFile(ShortCycle, CycleType.DistanceBased, false);

			var container = VehicleContainer.CreateVehicleContainer(new VectoRunData() {
				GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				}
			}, null, null);
		
		
			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var gbx = new MockGearbox(container);
			new ZeroMileageCounter(container);
			new DummyVehicleInfo(container);

			var driver = new MockDriver(container);
			cycle.InPort().Connect(driver.OutPort());

			cycle.OutPort().Initialize();

			// just in test mock driver
			driver.VehicleStopped = false;

			var startDistance = container.DrivingCycleInfo.CycleStartDistance.Value();
			var absTime = 0.SI<Second>();


			IResponse response;

			// waiting 40s in 1s steps
			for (var i = 0; i < 40; i++) {
				response = cycle.OutPort().Request(absTime, 1.SI<Meter>());
			Assert.IsInstanceOf<ResponseSuccess>(response);
				Assert.AreEqual(0, driver.LastRequest.TargetVelocity.Value(), Tolerance);
				Assert.AreEqual(0.028416069495827, driver.LastRequest.Gradient.Value(), 1E-12);
				Assert.AreEqual(1, driver.LastRequest.dt.Value(), Tolerance);
				container.CommitSimulationStep(absTime, response.SimulationInterval);
				absTime += response.SimulationInterval;
			}

			response = cycle.OutPort().Request(absTime, 1.SI<Meter>());

			Assert.IsInstanceOf<ResponseSuccess>(response);

			Assert.AreEqual(5.SI<MeterPerSecond>().Value(), driver.LastRequest.TargetVelocity.Value(), Tolerance);
			Assert.AreEqual(0.0284160694958265, driver.LastRequest.Gradient.Value(), 1E-12);
			Assert.AreEqual(1 + startDistance, cycle.CurrentState.Distance.Value(), Tolerance);

			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			response = cycle.OutPort().Request(absTime, 1.SI<Meter>());

			Assert.IsInstanceOf<ResponseSuccess>(response);

			Assert.AreEqual(5.SI<MeterPerSecond>().Value(), driver.LastRequest.TargetVelocity.Value(), Tolerance);
			Assert.AreEqual(0.0284160694958265, driver.LastRequest.Gradient.Value(), 1E-12);
			Assert.AreEqual(2 + startDistance, cycle.CurrentState.Distance.Value(), Tolerance);

			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;


			var exceeded = (ResponseDrivingCycleDistanceExceeded)cycle.OutPort().Request(absTime, 1000.SI<Meter>());
			Assert.AreEqual(811, exceeded.MaxDistance.Value(), Tolerance);

			AssertHelper.Exception<VectoSimulationException>(() => {
				container.CommitSimulationStep(absTime, exceeded.SimulationInterval);
				absTime += exceeded.SimulationInterval;
			}, "Previous request did not succeed!");

			response = cycle.OutPort().Request(absTime, exceeded.MaxDistance);

			Assert.AreEqual(5.SI<MeterPerSecond>().Value(), driver.LastRequest.TargetVelocity.Value(), Tolerance);
			Assert.AreEqual(0.020140043264606885, driver.LastRequest.Gradient.Value(), 1E-12);
			Assert.AreEqual(813 + startDistance, cycle.CurrentState.Distance.Value(), Tolerance);


			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;
		}

		[TestCase]
		public void CycleStartsWithNoStopTime()
		{
			var data = new string[] {
				// <s>,<v>,<grad>,<stop>
				"  0,   0,  0,   0",
				" 10,  50,  0,   0",
				"100,  50,  0,   0"
			};
			var cycleData = SimpleDrivingCycles.CreateCycleData(data);

			var modFileName = string.Format(CultureInfo.InvariantCulture, "CyccleStartWithoutStop.vmod");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFileName);

			
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors, "Cycle start witout stoptime FAILED");
		}

		[TestCase]
		public void CycleWithZeroStopTime()
		{
			var data = new string[] {
				// <s>,<v>,<grad>,<stop>
				"  0,   0,  0,   2",
				"100,  50,  0,   0",
				"120,   0,  0,   0",
				"200,  50,  0,   0"
			};
			var cycleData = SimpleDrivingCycles.CreateCycleData(data);

			var modFileName = string.Format(CultureInfo.InvariantCulture, "CyccleWithZeroStoptime.vmod");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFileName);


			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors, "Cycle start witout stoptime FAILED");
		}
	}

	public class DummyVehicleInfo : VectoSimulationComponent, IVehicleInfo
	{
		public DummyVehicleInfo(IVehicleContainer container) : base(container)
		{
			
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion

		#region Implementation of IVehicleInfo

		public MeterPerSecond VehicleSpeed => 0.SI<MeterPerSecond>();

		public bool VehicleStopped => throw new System.NotImplementedException();

		public Kilogram VehicleMass => throw new System.NotImplementedException();

		public Kilogram VehicleLoading => throw new System.NotImplementedException();

		public Kilogram TotalMass => throw new System.NotImplementedException();

		public CubicMeter CargoVolume => throw new System.NotImplementedException();

		public AirDragLossResult AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			throw new System.NotImplementedException();
		}

		public Newton RollingResistance(Radian gradient)
		{
			throw new System.NotImplementedException();
		}

		public Newton SlopeResistance(Radian gradient)
		{
			throw new System.NotImplementedException();
		}

		public MeterPerSecond MaxVehicleSpeed => throw new System.NotImplementedException();

		#endregion
		protected override bool DoUpdateFrom(object other) => false;

	}
}

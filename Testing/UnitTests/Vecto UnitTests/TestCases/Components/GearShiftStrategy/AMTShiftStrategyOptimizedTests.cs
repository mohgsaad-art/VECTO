using System.ComponentModel.Design.Serialization;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;
using Range = System.Range;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy;

[TestFixture]
public class AMTShiftStrategyOptimizedTests
{
    [TestCase(8, 7, 1800, 750, true),
TestCase(7, 6, 1800, 750, true),
TestCase(6, 5, 1800, 750, true),
TestCase(5, 4, 1800, 750, true),
TestCase(4, 3, 1800, 750, true),
TestCase(3, 2, 1800, 750, true),
TestCase(2, 1, 1900, 750, true),
TestCase(1, 1, 1200, 700, false),
TestCase(8, 4, 15000, 200, true),]
    public void Gearbox_ShiftDown_ACEA_Shiftlines(int gear, int newGear, double t, double n, bool shiftExpected)
    {
		// the first element 0.0 is just a placeholder for axlegear, not used in this test
		var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };

		//Get Containers and MockData
		var vehicleContainer = GetMocks(ratios, out var runData, out var testPowertrain);


		var shiftStrategy = GetShiftStrategyAndGearbox(vehicleContainer, out var gbx);
		vehicleContainer.Setup(c => c.EngineInfo.EngineSpeed).Returns(() => n.RPMtoRad());
		vehicleContainer.Setup(c => c.EngineInfo.EngineN95hSpeed).Returns(() => 2000.RPMtoRad());


        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];

		
		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

        var expectedT = t.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];

		var response = new ResponseSuccess(this);
		response.Engine.TorqueOutDemand = expectedT;
		response.Engine.EngineSpeed = expectedN;
		
		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), response);
        Assert.AreEqual(shiftExpected, shiftRequired);
        Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
    }


	[TestCase(7, 8, 1000, 1400, true),
	TestCase(6, 8, 800, 1400, true),
	TestCase(5, 6, 1000, 1400, true),
	TestCase(4, 5, 1000, 1400, true),
	TestCase(3, 4, 1000, 1400, true),
	TestCase(2, 4, 800, 1400, true),
	TestCase(1, 2, 1000, 1400, true),
	TestCase(8, 8, 1000, 1400, false),
	TestCase(1, 6, 200, 9000, true),]
    public void Gearbox_ShiftUp(int gear, int newGear, double tq, double n, bool shiftExpected)
    {
		// the first element 0.0 is just a placeholder for axlegear, not used in this test
		var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };


		var container = GetMocks(ratios, out var runData, out _);
		var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gbx);

		container.Setup(c => c.EngineInfo.EngineSpeed).Returns(() => n.RPMtoRad());
		container.Setup(c => c.EngineInfo.EngineN95hSpeed).Returns(2000.RPMtoRad());


		var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];

		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

        absTime += dt;

        var expectedT = tq.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];

		var response = new ResponseSuccess(this);
		//Setup Response
		response.Engine.TorqueOutDemand = expectedT;
		response.Engine.EngineSpeed = expectedN;


		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), response);



        Assert.AreEqual(shiftExpected, shiftRequired);
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
    }


	// [TestCase(2, 3, 800, 1400, 20_000, true, Description = "A gear would be skipped, but due to the uphill driving conditions, the next gear should be used")]
	// [TestCase(3, 3, 1000, 1400, 30_000, false, Description = "No upshifting because acceleration would be to low")]
 //    public void Gearbox_ShiftUpUphill(int gear, int newGear, double tq, double n, double slopeResistance, bool shiftExpected)
	// {
	// 	// the first element 0.0 is just a placeholder for axlegear, not used in this test
	// 	var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
	// 	var container = GetMocks(ratios, out var runData, out _);
 //
	// 	var accEstimationLookAhead = Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation;
 //
	// 	container.Setup(c => c.VehicleInfo.SlopeResistance(It.IsAny<Radian>())).Returns(slopeResistance.SI<Newton>());
	// 	container.Setup(c => c.DrivingCycleInfo.CycleLookAhead(accEstimationLookAhead))
	// 		.Returns(new DrivingCycleData.DrivingCycleEntry() {
	// 			Altitude = 100.SI<Meter>()
	// 		});
	// 	container.Setup(c => c.DrivingCycleInfo.Altitude).Returns(0.SI<Meter>());
 //
 //
	// 	
 //
 //
 //
 //  //       var testPt = GetMockTestPowertrain(runData, out var simplePt);
 //  //
	// 	// var ptBuilder = new Mock<ISimplePowertrainBuilder>();
	// 	// ptBuilder.Setup(p => p.CreateTestPowertrain<Gearbox>(It.IsAny<ISimpleVehicleContainer>(), It.IsAny<IDataBus>()))
	// 	// 	.Returns(testPt.Object);
 //  //
	// 	// ptBuilder.Setup(p => p.BuildSimplePowertrain(It.IsAny<VectoRunData>())).Returns(simplePt.Object);
	// 	// container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);
 //  //
	// 	// var gbx = GetMockGearbox(container);
	// 	// Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => n.RPMtoRad());
	// 	// Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineN95hSpeed).Returns(2000.RPMtoRad());
 //
	// 	var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gbx);
	// 	// var shiftStrategy = new AMTShiftStrategyOptimized(container.Object);
	// 	// shiftStrategy.Gearbox = gbx.Object;
 //
	// 	var absTime = 0.SI<Second>();
	// 	var dt = 2.SI<Second>();
 //
	// 	var expectedN = n.RPMtoRad();
	// 	var angularVelocity = expectedN / ratios[gear];
 //
	// 	var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
	// 		angularVelocity);
 //
	// 	absTime += dt;
 //
	// 	var expectedT = tq.SI<NewtonMeter>();
	// 	var torque = expectedT * ratios[gear];
 //
 //
	// 	var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
	// 		new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));
 //
	// 	Assert.AreEqual(shiftExpected, shiftRequired);
	// 	Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
 //    }
 //
 //
	// [TestCase]
	// public void GetMocksTest()
	// {
	// 	// the first element 0.0 is just a placeholder for axlegear, not used in this test
	// 	var ratios = new[] { 0.0, 6.38, 4.63, 3.84, 2.59, 1.86, 1.35, 1, 0.76 };
	//
	// 	var mockContainer = GetMocks(ratios,
	// 		out var runData,
	// 		out var testPowertrain);
	//
	// 	var container = mockContainer.Object;
	// 	var createdTestPowertrain = container.SimplePowertrainBuilder.CreateTestPowertrain(container, false);
	// 	
	// 	Assert.NotNull(createdTestPowertrain.Container.GearboxOutPort);
	// 	Assert.NotNull(createdTestPowertrain);
	// }


	[TestCase(1, 2, 100, 900, true)]
	[TestCase(4, 5, 100, 900, true)]
	[TestCase(5, 6, 100, 900, true)]

    public void Gearbox_EarlyUpShift(int gear, int newGear, double tq, double n, bool shiftExpected)
    {
        // the first element 0.0 is just a placeholder for axlegear, not used in this test
        var ratios = new[] { 0.0, 6.38, 4.63, 3.84, 2.59, 1.86, 1.35, 1, 0.76 };

		var container = GetMocks(ratios,
			out var runData,
			out var testPowertrain);


		
		
		runData.GearshiftParameters.RatioEarlyUpshiftFC = 10;
		runData.GearshiftParameters.MinEngineSpeedPostUpshift = 1.RPMtoRad();
		runData.GearshiftParameters.TorqueReserve = 0.1;
		runData.GearshiftParameters.RatingFactorCurrentGear = 0.97;
		var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gbx);
		
		
		container.Setup(c => c.EngineInfo.EngineSpeed).Returns(() => n.RPMtoRad());
		container.Setup(c => c.EngineInfo.EngineN95hSpeed).Returns(() => 2000.RPMtoRad());
		

        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];

        var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
            angularVelocity);
		
		
		
        absTime += dt;

        var expectedT = tq.SI<NewtonMeter>();
        var torque = expectedT * ratios[gear];

		var response = new ResponseSuccess(this);
		response.Engine.TorqueOutDemand = expectedT;
		response.Engine.EngineSpeed = expectedN;



        var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
            new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), response);

		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
        Assert.AreEqual(shiftExpected, shiftRequired);
    }

	

    [TestCase(7, 1, 1000, 1400, true)]
	[TestCase(7, 2, 400, 200, true)]
    public void InitStartGear(int gear, int newGear, double tq, double n, bool shiftExpected)
	{
		// the first element 0.0 is just a placeholder for axlegear, not used in this test
		var ratios = new[] { 0.0, 6.38, 5.2, 4.3, 3.2, 2.5, 1.8, 1, 0.76 };
		var container = GetMocks(ratios, out var runData, out var testPt);
		
		container.Setup(c => c.VehicleInfo.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());

		var gbx = GetMockGearbox();
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => n.RPMtoRad());
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineN95hSpeed).Returns(2000.RPMtoRad());

		var shiftStrategy = new AMTShiftStrategyOptimized(container.Object);
		shiftStrategy.Gearbox = gbx.Object;

		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();

		var expectedN = n.RPMtoRad();
		var angularVelocity = expectedN / ratios[gear];


		testPt.Setup(t => t.Gearbox.Request(
			It.IsAny<Second>(),
			It.IsAny<Second>(),
			It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(),
			true)).Returns(new ResponseDryRun(null)
		{
			Engine = {
				TotalTorqueDemand = 2.SI<NewtonMeter>(),
				DynamicFullLoadTorque = 4.SI<NewtonMeter>(),
				EngineSpeed = 600.RPMtoRad(),
			}
		});

		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

		absTime += dt;

		var expectedT = tq.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];


		//var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
		//	new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));

		//Assert.AreEqual(shiftExpected, shiftRequired);
		Assert.GreaterOrEqual(shiftStrategy.MaxStartGear.Gear, newGear);
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
    }


		[TestCase(2, 1, 2, 1000, 300)]
		[TestCase(3, 2, 2, 1000, 1400)]
		[TestCase(8, 7, 2, 1800, 750)]
		[TestCase(7, 6, 2, 1800, 750)]
		[TestCase(6, 5, 2, 1800, 750)]
		[TestCase(5, 4, 2, 1800, 750)]
		[TestCase(4, 3, 2, 1800, 750)]
		[TestCase(3, 2, 2, 1800, 750)]
		[TestCase(2, 2, 2, 1900, 750)]
		[TestCase(1, 2, 2, 1200, 700)]
		[TestCase(8, 4, 2, 15000, 200)]
		[TestCase(2, 2, 2, 300, 1000)]
    public void Gearbox_PTO(int gear, int newGear, int ptoGear, double tq, double n)
	{
		var shiftExpected = gear != newGear;
        // the first element 0.0 is just a placeholder for axlegear, not used in this test
        var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };

		var container = GetMocks(ratios, out VectoRunData runData, out _);

		runData.DriverData = new DriverData() {
			PTODriveRoadsweepingGear = new GearshiftPosition((uint)ptoGear)
		};

		container.Setup(c => c.DrivingCycleInfo.CycleData).Returns(
			new CycleData() {
				LeftSample = new DrivingCycleData.DrivingCycleEntry() {
					PTOActive = PTOActivity.PTOActivityRoadSweeping,
					RoadGradient = 0.SI<Radian>(),
				}
			}
		);

		var shiftStrategy = GetShiftStrategyAndGearbox(container, out var gbx);



        ////Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => n.RPMtoRad());
        ////Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineN95hSpeed).Returns(2000.RPMtoRad());



        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];

        var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
            angularVelocity);

        absTime += dt;

        var expectedT = tq.SI<NewtonMeter>();
        var torque = expectedT * ratios[gear];

		var response = new ResponseSuccess(this) {

		};
        var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
            new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), response);

        Assert.AreEqual(shiftExpected, shiftRequired);
        Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
    }


	private Mock<IVehicleContainer> GetMocks(double[] ratios,
		out VectoRunData runData,
		out Mock<ITestPowertrain> testPowertrain)
	{
		runData = GetRunData(ratios);

		var container = GetMockVehicleContainer(runData);

		
		//Use simple powertrain to create testpowertrain
		testPowertrain = GetMockTestPowertrain(runData, out var simpleContainer);
		
		
		var ptBuilder = new Mock<ISimplePowertrainBuilder>();

		//TestPowertrain
		ptBuilder.Setup(p => p.CreateTestPowertrain(
				It.IsAny<IVehicleContainer>(),
				It.IsAny<bool>()))
			.Returns(testPowertrain.Object);

		ptBuilder.Setup(p => p.CreateTestPowertrain(
			It.IsAny<IVehicleContainer>(),
			It.IsAny<bool>(), It.IsAny<VectoSimulationJobType>())).Throws(new NotImplementedException());
		
		container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);
		


		return container;
	}




    private Mock<ITestPowertrain> GetMockTestPowertrain(VectoRunData runData, out Mock<ISimpleVehicleContainer> simpleContainer)
    {
        var testPt = new Mock<ITestPowertrain>();
		
		simpleContainer = GetSimplePowertrain(runData, out var testGearbox);
		testPt.Setup(t => t.Container).Returns(simpleContainer.Object);

		testPt.Setup(t => t.Gearbox).Returns(testGearbox.Object);
  //       tGbx.Setup(g => g.Initialize(It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>()))
  //           .Returns((NewtonMeter t, PerSecond n) => new ResponseSuccess(this) {
  //               Engine = { PowerRequest = n * t, 
		// 			EngineSpeed = n },
  //               Clutch = { PowerRequest = n * t }
  //           });
  //       tGbx.Setup(g => g.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(),
  //               It.IsAny<bool>()))
  //           .Returns((Second absTime, Second dt, NewtonMeter t, PerSecond n, bool dryRun) => new ResponseSuccess(this) {
  //               Engine = { PowerRequest = n * t, EngineSpeed = n },
  //               Clutch = { PowerRequest = n * t }
  //           });
		// tGbx.Setup(g => g.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(),
		// 		It.IsAny<bool>()))
		// 	.Returns((Second absTime, Second dt, NewtonMeter t, PerSecond n, bool dryRun) => dryRun ? 
		// 	new ResponseDryRun(this) {
		// 		Engine = { 
		// 			PowerRequest = n * t, 
		// 			EngineSpeed = n,
		// 			TotalTorqueDemand = t,
		// 		},
		// 		Clutch = { PowerRequest = n * t },
		// 		DeltaFullLoad = n*t / 2 *(-1)
		// 	}: new ResponseSuccess(this) {
		// 		Engine = { PowerRequest = n * t, EngineSpeed = n },
		// 		Clutch = { PowerRequest = n * t }
		// 	});
        var tEng = new Mock<ITestpowertrainCombustionEngine>();
        testPt.Setup(t => t.CombustionEngine).Returns(tEng.Object);
        tEng.Setup(e => e.EngineStationaryFullPower(It.IsAny<PerSecond>()))
            .Returns((PerSecond n) => runData.EngineData.FullLoadCurves[0].FullLoadStationaryPower(n));
        return testPt;
    }

	private static Mock<IPowertainInfo> GetPowertrainInfo()
	{
		var tPi = new Mock<IPowertainInfo>();
		tPi.Setup(p => p.HasCombustionEngine).Returns(true);
		return tPi;
	}

	private static Mock<IVehicleContainer> GetMockVehicleContainer(VectoRunData runData)
    {
        var container = new Mock<IVehicleContainer>();
        container.Setup(c => c.RunData).Returns(runData);
        var veh = new Mock<IVehicleInfo>();
        veh.Setup(v => v.VehicleSpeed).Returns(10.SI<MeterPerSecond>());
        container.Setup(c => c.VehicleInfo).Returns(veh.Object);
        var eng = new Mock<IEngineInfo>();
        container.Setup(c => c.EngineInfo).Returns(eng.Object);
        eng.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);
        eng.Setup(e => e.EngineRatedSpeed).Returns(runData.EngineData.FullLoadCurves.First().Value.RatedSpeed);
        eng.Setup(e => e.EngineN95hSpeed).Returns(runData.EngineData.FullLoadCurves.First().Value.N95hSpeed);
		eng.Setup(e => e.EngineStationaryFullPower(It.IsAny<PerSecond>()))
			.Returns((PerSecond n) => runData.EngineData.FullLoadCurves[0].FullLoadStationaryPower(n));
        var ci = new Mock<IDrivingCycleInfo>();
        container.Setup(c => c.DrivingCycleInfo).Returns(ci.Object);
        var di = new Mock<IDriverInfo>();
		di.Setup(d => d.DriverBehavior).Returns(DrivingBehavior.Accelerating);
		di.Setup(d => d.DrivingAction).Returns(DrivingAction.Accelerate);
        container.Setup(c => c.DriverInfo).Returns(di.Object);
        ci.Setup(c => c.CycleData).Returns(new CycleData() { LeftSample = new DrivingCycleData.DrivingCycleEntry() { PTOActive = PTOActivity.Inactive } });
		ci.Setup(c => c.CycleLookAhead(It.IsAny<Meter>())).Returns(new DrivingCycleData.DrivingCycleEntry() {
			Altitude = 0.SI<Meter>()
		});
		ci.Setup(c => c.Altitude).Returns(0.SI<Meter>());
		var pi = new Mock<IPowertainInfo>();
        pi.Setup(p => p.HasCombustionEngine).Returns(true);
        container.Setup(c => c.PowertrainInfo).Returns(pi.Object);
        var vi = new Mock<IVehicleInfo>();
		vi.Setup(v => v.AirDragResistance(It.IsAny<MeterPerSecond>(), It.IsAny<MeterPerSecond>()))
			.Returns(new AirDragLossResult(0.SI<Watt>(), 0.SI<SquareMeter>(), 0.SI<MeterPerSecond>()));
		vi.Setup(v => v.RollingResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vi.Setup(v => v.SlopeResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vi.Setup(v => v.VehicleSpeed).Returns(30.KMPHtoMeterPerSecond());
		vi.Setup(v => v.TotalMass).Returns(12000.SI<Kilogram>());
		container.Setup(c => c.VehicleInfo).Returns(vi.Object);
		var wi = new Mock<IWheelsInfo>();
		container.Setup(c => c.WheelsInfo).Returns(wi.Object);
		wi.Setup(w => w.ReducedMassWheels).Returns(0.SI<Kilogram>());
		var axli = new Mock<IAxlegearInfo>();
		container.Setup(c => c.AxlegearInfo).Returns(axli.Object);
		axli.Setup(a => a.AxlegearLoss()).Returns(0.SI<Watt>());
        return container;
    }

	
	private AMTShiftStrategyOptimized GetShiftStrategyAndGearbox(Mock<IVehicleContainer> vehicleContainer, out Mock<IGearbox> gbx)
	{
		var shiftStrategy = new AMTShiftStrategyOptimized(vehicleContainer.Object);

		gbx = GetMockGearbox();
		shiftStrategy.Gearbox = gbx.Object;

		SetVelocityDropLookupData(shiftStrategy);
        return shiftStrategy;
	}

	
	

    private Mock<ISimpleVehicleContainer> GetSimplePowertrain(VectoRunData runData, out Mock<ITestPowertrainTransmission> testGearbox)
	{
		var simplePt = new Mock<ISimpleVehicleContainer>();
		simplePt.Setup(s => s.RunData).Returns(runData);
		simplePt.Setup(s => s.AddComponent(It.IsAny<VectoSimulationComponent>()));
		simplePt.Setup(s => s.PowertrainInfo).Returns(GetPowertrainInfo().Object);
		simplePt.Setup(s => s.IsTestPowertrain).Returns(true);

		var gbx = GetMockTestGearbox(runData.GearboxData.Gears);
		simplePt.Setup(s => s.GearboxInfo).Returns(gbx.Object);
		simplePt.Setup(s => s.GearboxCtl).Returns(gbx.Object);
		simplePt.Setup(s => s.GearboxOutPort).Returns(gbx.Object);
		//Vehicle Info
		var vehicleInfo = new Mock<IVehicleInfo>();
		simplePt.Setup(c => c.VehicleInfo).Returns(vehicleInfo.Object);
		vehicleInfo.Setup(v => v.VehicleSpeed).Returns(1.KMPHtoMeterPerSecond());

		//VehiclePort
		var vehiclePort = new Mock<IDriverDemandOutPort>();
		vehiclePort.Setup(port => port.Initialize(
			It.IsAny<MeterPerSecond>(), It.IsAny<Radian>())).Returns(new ResponseSuccess(this));

		// simplePt.Setup(c => c).Returns(vehiclePort.Object);


		//GearboxOutPort


  //       var mockPort = new Mock<ITnOutPort>();
		// mockPort.Name = "MockPort1";
		// mockPort.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(),
		// 	It.IsAny<PerSecond>())).Returns((NewtonMeter tq, PerSecond rpm) => {
		// 	return new ResponseSuccess(this)
		// 	{
		// 		Engine = {
		// 			EngineSpeed = rpm,
		// 			PowerRequest = tq * rpm,
		// 		},
		// 	};
		// });
		// mockPort.Setup(p => p.Request(
		// 		It.IsAny<Second>(),
		// 		It.IsAny<Second>(),
		// 		It.IsAny<NewtonMeter>(),
		// 		It.IsAny<PerSecond>(),
		// 		true)).Returns((
		// 		Second absTime,
		// 		Second dt,
		// 		NewtonMeter t,
		// 		PerSecond n,
		// 		bool dryRun) => {
  //
		// 	var ratio = 
		// 		gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;
		// 	return dryRun
  //
		// 		? new ResponseDryRun(this)
		// 		{
		// 			Engine = {
		// 				PowerRequest = n * t, EngineSpeed = n * ratio,
		// 				DynamicFullLoadPower = (t / ratio + 2300.SI<NewtonMeter>()) * n * ratio,
		// 				TotalTorqueDemand = t,
		// 			},
		// 			Clutch = { PowerRequest = n * t },
		// 			DeltaFullLoad = n*t / 2 * (-1)
		// 		}
		// 		: new ResponseSuccess(this)
		// 		{
		// 			Engine = { 
		// 				PowerRequest = n * t, 
		// 				EngineSpeed = n * ratio
  //
		// 			},
		// 			Clutch = { PowerRequest = n * t }
		// 		};
  //               });

		// simplePt.Setup(c => c.GearboxOutPort).Returns(mockPort.Object);

		testGearbox = gbx;
		return simplePt;
	}
	
	private Mock<IGearbox> GetMockGearbox()
	{
		var amtGearbox = new Mock<IAMTGearbox>(MockBehavior.Strict);
		amtGearbox.Name = "AMT_Gearbox";
		var gbx = amtGearbox.As<IGearbox>();
		gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
		gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());
		return gbx;
	} 
	
	
    private Mock<ITestPowertrainTransmission> GetMockTestGearbox(Dictionary<uint, GearData> ratios)
	{
		
		Mock<IAMTGearbox> amtGearbox = new Mock<IAMTGearbox>();
		amtGearbox.Name = "AMT_TestGearbox";
		Mock<ITestPowertrainTransmission> gbx = amtGearbox.As<ITestPowertrainTransmission>();

		
		
		gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
		gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());

		GearshiftPosition gear = null;
		gbx.SetupGet(g => g.Gear).Returns(() => {
			
			return gear;
		});
		gbx.SetupSet(g => g.SetGear = It.IsAny<GearshiftPosition>())
			.Callback<GearshiftPosition>(p => {
				gear = p;
			});


		GearshiftPosition nextGear = null;
		gbx.SetupGet(g => g.NextGear).Returns(() => nextGear);
		gbx.SetupSet(g => g.SetNextGear = It.IsAny<GearshiftPosition>())
			.Callback<GearshiftPosition>(p => nextGear = p);
			
		gbx.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>())).Returns((NewtonMeter tq, PerSecond rpm) => {
			return new ResponseSuccess(this)
			{
				Engine = {
					EngineSpeed = rpm,
					PowerRequest = tq * rpm,
				},
			};
		});
		gbx.Setup(p => p.Request(
			It.IsAny<Second>(),
			It.IsAny<Second>(),
			It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(),
			true)).Returns((
			Second absTime,
			Second dt,
			NewtonMeter t,
			PerSecond n,
			bool dryRun) => {

			var ratio =
				gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;
			return dryRun
				? new ResponseDryRun(this) {
					Engine = {
						PowerRequest = n * t, EngineSpeed = n * ratio,
						DynamicFullLoadPower = (t / ratio + 2300.SI<NewtonMeter>()) * n * ratio,
						TotalTorqueDemand = t,
					},
					Clutch = { PowerRequest = n * t },
					DeltaFullLoad = n * t / 2 * (-1)
				}
				: new ResponseSuccess(this) {
					Engine = {
						PowerRequest = n * t,
						EngineSpeed = n * ratio

					},
					Clutch = { PowerRequest = n * t }
				};
		});
		return gbx;
	}
	

    private void SetVelocityDropLookupData(AMTShiftStrategyOptimized shiftStrategy)
    {
        //"StartVelocity [km/h], Gradient [-], EndVelocity [km/h]"
        var data = new[] {
            new[] { 5.0, -0.0997, 9.061522965237558 },
            new[] { 5.0, -0.0798, 7.76698080079411 },
            new[] { 5.0, -0.0599, 6.46583777913701 },
            new[] { 5.0, -0.0400, 5.1596021788785045 },
            new[] { 5.0, -0.0200, 3.8498121019126907 },
            new[] { 5.0, 0.0000, 2.5380265159468918 },
            new[] { 5.0, 0.0200, 1.2258160477557427 },
            new[] { 5.0, 0.0400, 0.0 },
            new[] { 5.0, 0.0599, 0.0 },
            new[] { 5.0, 0.0798, 0.0 },
            new[] { 5.0, 0.0997, 0.0 },
            new[] { 10.0, -0.0997, 14.807789640888302 },
            new[] { 10.0, -0.0798, 13.474253785811362 },
            new[] { 10.0, -0.0599, 12.133880027250777 },
            new[] { 10.0, -0.0400, 10.788221550084467 },
            new[] { 10.0, -0.0200, 9.438862432338068 },
            new[] { 10.0, 0.0000, 8.087408432114643 },
            new[] { 10.0, 0.0200, 6.735477499784416 },
            new[] { 10.0, 0.0400, 5.384690105076845 },
            new[] { 10.0, 0.0599, 4.036659549786269 },
            new[] { 10.0, 0.0798, 2.6929823705748137 },
            new[] { 10.0, 0.0997, 1.3552289800549435 },
            new[] { 20.0, -0.0997, 25.061542153872097 },
            new[] { 20.0, -0.0798, 23.72002987685605 },
            new[] { 20.0, -0.0599, 22.371630788642932 },
            new[] { 20.0, -0.0400, 21.017907257116025 },
            new[] { 20.0, -0.0200, 19.660452757813403 },
            new[] { 20.0, 0.0000, 18.30088261992287 },
            new[] { 20.0, 0.0200, 16.94082447048767 },
            new[] { 20.0, 0.0400, 15.581908518759507 },
            new[] { 20.0, 0.0599, 14.225757795590708 },
            new[] { 20.0, 0.0798, 12.873978508374211 },
            new[] { 20.0, 0.0997, 11.528150617530041 },
            new[] { 30.000000000000004, -0.0997, 35.091774208140095 },
            new[] { 30.000000000000004, -0.0798, 33.750904327320896 },
            new[] { 30.000000000000004, -0.0599, 32.40315158162777 },
            new[] { 30.000000000000004, -0.0400, 31.050077596965064 },
            new[] { 30.000000000000004, -0.0200, 29.69327509188113 },
            new[] { 30.000000000000004, 0.0000, 28.33435862498606 },
            new[] { 30.000000000000004, 0.0200, 26.974955046566407 },
            new[] { 30.000000000000004, 0.0400, 25.616693776603913 },
            new[] { 30.000000000000004, 0.0599, 24.261197065863925 },
            new[] { 30.000000000000004, 0.0798, 22.91007034016412 },
            new[] { 30.000000000000004, 0.0997, 21.56489279686667 },
            new[] { 40.0, -0.0997, 45.024018797189854 },
            new[] { 40.0, -0.0798, 43.68636622428101 },
            new[] { 40.0, -0.0599, 42.34185052441486 },
            new[] { 40.0, -0.0400, 40.99202962224952 },
            new[] { 40.0, -0.0200, 39.63849244500093 },
            new[] { 40.0, 0.0000, 38.282849690992855 },
            new[] { 40.0, 0.0200, 36.926724305651554 },
            new[] { 40.0, 0.0400, 35.57174178507285 },
            new[] { 40.0, 0.0599, 34.21952045137207 },
            new[] { 40.0, 0.0798, 32.87166183129923 },
            new[] { 40.0, 0.0997, 31.5297412694979 },
            new[] { 50.0, -0.0997, 54.652157586769704 },
            new[] { 50.0, -0.0798, 53.319266704395716 },
            new[] { 50.0, -0.0599, 51.97954186606304 },
            new[] { 50.0, -0.0400, 50.634535516787736 },
            new[] { 50.0, -0.0200, 49.28583097196263 },
            new[] { 50.0, 0.0000, 47.93503321632867 },
            new[] { 50.0, 0.0200, 46.583759417454765 },
            new[] { 50.0, 0.0400, 45.23362925944123 },
            new[] { 50.0, 0.0599, 43.88625525588574 },
            new[] { 50.0, 0.0798, 42.54323315977748 },
            new[] { 50.0, 0.0997, 41.20613261641401 },
            new[] { 60.00000000000001, -0.0997, 64.2503607025971 },
            new[] { 60.00000000000001, -0.0798, 62.91932863058169 },
            new[] { 60.00000000000001, -0.0599, 61.58156853535776 },
            new[] { 60.00000000000001, -0.0400, 60.23862904729555 },
            new[] { 60.00000000000001, -0.0200, 58.89369896300112 },
            new[] { 60.00000000000001, 0.0000, 57.547040626925885 },
            new[] { 60.00000000000001, 0.0200, 56.199911833784675 },
            new[] { 60.00000000000001, 0.0400, 54.85392730356455 },
            new[] { 60.00000000000001, 0.0599, 53.510694584916905 },
            new[] { 60.00000000000001, 0.0798, 52.17180450267632 },
            new[] { 60.00000000000001, 0.0997, 50.83882182989668 },
            new[] { 70.0, -0.0997, 73.78388063954164 },
            new[] { 70.0, -0.0798, 72.45700102808648 },
            new[] { 70.0, -0.0599, 71.12341092304165 },
            new[] { 70.0, -0.0400, 69.78459543842656 },
            new[] { 70.0, -0.0200, 68.44188526693415 },
            new[] { 70.0, 0.0000, 67.09719334997524 },
            new[] { 70.0, 0.0200, 65.75212749798493 },
            new[] { 70.0, 0.0400, 64.40829754257321 },
            new[] { 70.0, 0.0599, 63.06730571969745 },
            new[] { 70.0, 0.0798, 61.73073716025881 },
            new[] { 70.0, 0.0997, 60.40015062055851 },
            new[] { 80.0, -0.0997, 83.25457689135129 },
            new[] { 80.0, -0.0798, 81.93182852399568 },
            new[] { 80.0, -0.0599, 80.60238880559001 },
            new[] { 80.0, -0.0400, 79.2676325992058 },
            new[] { 80.0, -0.0200, 77.92916666616652 },
            new[] { 80.0, 0.0000, 76.58872134590663 },
            new[] { 80.0, 0.0200, 75.24790011046437 },
            new[] { 80.0, 0.0400, 73.90830846424944 },
            new[] { 80.0, 0.0599, 72.57154434889891 },
            new[] { 80.0, 0.0798, 71.23918865436896 },
            new[] { 80.0, 0.0997, 69.91277394305271 },
        };
        var entries = new List<VelocitySpeedGearshiftPreprocessor.Entry>();
        foreach (var d in data)
        {
            entries.Add(new VelocitySpeedGearshiftPreprocessor.Entry()
            {
                StartVelocity = d[0].KMPHtoMeterPerSecond(),
                Gradient = d[1].SI<Radian>(),
                EndVelocity = d[2].KMPHtoMeterPerSecond(),
            });
        }

        shiftStrategy.VelocityDropData.Data = entries.ToArray();

    }

    private static VectoRunData GetRunData(double[] ratios)
    {
        var gearboxData = new GearboxData {
            Gears = new Dictionary<uint, GearData>()
        };
        for (uint i = 1; i < ratios.Length; i++) {
            gearboxData.Gears[i] = new GearData {
                Ratio = ratios[i],
                LossMap = TransmissionLossMapReader.Create(0.96, ratios[i], $"Gear {i}")
            };
        }

		IEnumerable<(uint key, EngineFullLoadCurve fld)> fldCurves =
			ratios.Select(
				(_, i) => ((uint)i,
						FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHdr, EngineFldData))
					));


		List<CombustionEngineFuelData> fuels = new List<CombustionEngineFuelData>() {
			new CombustionEngineFuelData() {
				ColdHotCorrectionFactor = 1,
				ConsumptionMap = FuelConsumptionMapReader.Create(InputDataHelper.InputDataAsTableData(
					EngineFcMapHdr, 
					EngineFcMapData)),
				FuelData = FuelData.Diesel,
			}
		};
		var engineData = new CombustionEngineData() {
            IdleSpeed = 560.RPMtoRad(),
            FullLoadCurves = fldCurves.ToDictionary((x) => x.key, (x) => x.fld),
			Fuels = fuels,
        };

        var axlRatio = 3.240355;
        var gearsInput = gearboxData.Gears.Select(x => {
            var r = new Mock<ITransmissionInputData>();
            r.Setup(g => g.Ratio).Returns(x.Value.Ratio);
            return r.Object;
        }).ToList();
        foreach (var entry in gearboxData.Gears) {
            entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
                (int)(entry.Key - 1), engineData.FullLoadCurves.First().Value,
                gearsInput, engineData, axlRatio, 0.5.SI<Meter>());
        }


		var mockCycle = new Mock<IDrivingCycleData>();
		mockCycle.Setup(cd => cd.Entries).Returns(
			new List<DrivingCycleData.DrivingCycleEntry>() {
				new DrivingCycleData.DrivingCycleEntry() {
					RoadGradient = 0.SI<Radian>()
				}
			});









        var runData = new VectoRunData() {
            GearboxData = gearboxData,
            GearshiftParameters = new ShiftStrategyParameters() {
                StartSpeed = 2.SI<MeterPerSecond>(),
                TimeBetweenGearshifts = 6.SI<Second>(),
                DownshiftAfterUpshiftDelay = 2.SI<Second>(),
                UpshiftAfterDownshiftDelay = 2.SI<Second>(),
                UpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>(),
				StartTorqueReserve = 0.2,
            },
            EngineData = engineData,
            AxleGearData = new AxleGearData() {
                AxleGear = new TransmissionData() {
                    Ratio = 3.240355
                }
            },
            VehicleData = new VehicleData() {
                DynamicTyreRadius = 0.492.SI<Meter>(),
            },
			Cycle = mockCycle.Object,
		};
        return runData;
    }

    const string EngineFldHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";
    
    static readonly string[] EngineFldData = new[] {
		"560,1180,-149,0.6		   ",
		"600,1282,-148,0.6		   ",
		"799.9999999,1791,-149,0.6 ",
		"1000,2300,-160,0.6		   ",
		"1200,2300,-179,0.6		   ",
		"1400,2300,-203,0.6		   ",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25	   ",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25	   ",
	};


	const string EngineFcMapHdr = "engine speed [rpm], torque [Nm], fuel consumption [g/h]";
	static readonly string[] EngineFcMapData = new[] {
		"500,-235.5,0",
"500,-135.5,0",
"500,0,1355",
"500,213.4,3412.291",
"500,426.8,5830.1",
"500,640.2,8316.426",
"500,853.6,10439.87",
"500,1067,12823.69",
"500,1188,14228.79",
"500,1401.4,16628.66",
"600,-238,0",
"600,-138,0",
"600,0,1355",
"600,213.4,3412.291",
"600,426.8,5830.1",
"600,640.2,8316.426",
"600,853.6,10439.87",
"600,1067,12823.69",
"600,1188,14228.79",
"600,1401.4,16628.66",
"751,-241.775,0",
"751,-141.775,0",
"750.9,0,1649.255",
"750.9,213.4,4157.795",
"750.9,426.8,7149.494",
"750.9,640.2,10037.08",
"750.9,853.6,12957.07",
"750.9,1067,16055.22",
"750.9,1280.4,19231.36",
"750.9,1493.8,22400.17",
"750.9,1544.879,23213.92",
"751,1758.279,26392.93",
"902,-247.59,0",
"902,-147.59,0",
"901.8,0,2210.735",
"901.8,213.4,5204.867",
"901.8,426.8,8515.462",
"901.8,640.2,11804.75",
"901.8,853.6,15410.55",
"901.8,1067,19081.7",
"901.8,1280.4,22742.96",
"901.8,1493.8,26543.87",
"901.8,1707.2,30534.68",
"901.8,1901.757,34352.75",
"902,2115.157,38403.27",
"1053,-255.445,0",
"1053,-155.445,0",
"1052.7,0,2768.035",
"1052.7,213.4,6228.407",
"1052.7,426.8,9836.041",
"1052.7,640.2,13624.5",
"1052.7,853.6,17854.95",
"1052.7,1067,22072.71",
"1052.7,1280.4,26161.13",
"1052.7,1493.8,30525.55",
"1052.7,1707.2,35019.18",
"1052.7,1920.6,39913.3",
"1052.7,2134,45438.16",
"1053,2347.4,50542.53",
"1204,-265.44,0",
"1203.6,0,3086.704",
"1203.6,213.4,6943.027",
"1203.6,426.8,11040.37",
"1203.6,640.2,15504.65",
"1203.6,853.6,20335.89",
"1203.6,1067,25176.6",
"1203.6,1280.4,29782.22",
"1203.6,1493.8,34642.24",
"1203.6,1707.2,39786.14",
"1203.6,1920.6,45254.8",
"1203.6,2134,51129.03",
"1204,2347.4,56732.88",
"1367,-283.37,0",
"1367,-183.37,0",
"1367.1,0,3845.344",
"1367.1,213.4,7981.742",
"1367.1,426.8,12796.69",
"1367.1,640.2,17789.2",
"1367.1,853.6,22854.21",
"1367.1,1067,28302.84",
"1367.1,1280.4,33739.91",
"1367.1,1493.8,39393.87",
"1367.1,1707.2,45836.33",
"1367.1,1920.6,52078.71",
"1367.1,2134,58296.41",
"1367,2347.4,64530.56",
"1490,-300.5,0",
"1490,-200.5,0",
"1489.6,0,4373.424",
"1489.6,213.4,8861.484",
"1489.6,426.8,14090.86",
"1489.6,640.2,19518.29",
"1489.6,853.6,25092.8",
"1489.6,1067,30873.69",
"1489.6,1280.4,36865.42",
"1489.6,1493.8,43095.57",
"1489.6,1707.2,50249.81",
"1489.6,1920.6,57035.25",
"1489.6,2041.712,60609.5",
"1490,2255.112,67311.83",
"1612,-318.62,0",
"1612,-218.62,0",
"1612.2,0,4904.015",
"1612.2,213.4,9810.482",
"1612.2,426.8,15403.9",
"1612.2,640.2,21301.35",
"1612.2,853.6,27492.32",
"1612.2,1067,33580.96",
"1612.2,1280.4,40114.61",
"1612.2,1493.8,46914.77",
"1612.2,1707.2,54666.14",
"1612.2,1915.434,61862.91",
"1612,2128.834,69491.99",
"1735,-335.225,0",
"1735,-235.225,0",
"1734.7,0,5586.953",
"1734.7,213.4,11041.15",
"1734.7,426.8,16949.24",
"1734.7,640.2,23500.23",
"1734.7,853.6,30159.59",
"1734.7,1067,36741.18",
"1734.7,1280.4,43923.85",
"1734.7,1493.8,51295.21",
"1734.7,1707.2,59469.31",
"1734.7,1789.259,62731.31",
"1735,2002.659,70935.23",
"1857,-353.69,0",
"1857,-253.69,0",
"1857.3,0,6673.839",
"1857.3,213.4,12518.56",
"1857.3,426.8,18687.88",
"1857.3,640.2,25652.39",
"1857.3,853.6,33003.08",
"1857.3,1067,40438.09",
"1857.3,1280.4,48117.52",
"1857.3,1493.8,55848.59",
"1857.3,1587.631,59434.17",
"1857,1801.031,67215.39",
"1957,-370.69,0",
"1957,-270.69,0",
"1957.3,0,6673.839",
"1957.3,213.4,12518.56",
"1957.3,426.8,18687.88",
"1957.3,640.2,25652.39",
"1957.3,853.6,33003.08",
"1957.3,1067,40438.09",
"1957.3,1280.4,48117.52",
"1957.3,1493.8,55848.59",
"1957.3,1587.631,59434.17",
"1957,1801.031,67215.39",
	};
		
		
}
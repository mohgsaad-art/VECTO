using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
// using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;
using Constants = TUGraz.VectoCore.Configuration.Constants;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy;

public class AMTShiftStrategyTests
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
        var runData = GetRunData(ratios);

		var container = GetMockVehicleContainer(runData);

		var testPt = GetMockTestPowertrain(runData);

		var ptBuilder = new Mock<ISimplePowertrainBuilder>();
		ptBuilder.Setup(p => p.CreateTestPowertrain(It.IsAny<IVehicleContainer>(), It.IsAny<bool>()))
			.Returns(testPt.Object);
		container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);

        var gbx = GetMockGearbox(container);


		var shiftStrategy = new AMTShiftStrategy(container.Object);
		shiftStrategy.Gearbox = gbx.Object;

        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];

		
		shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

        var expectedT = t.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];

		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));

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
		var runData = GetRunData(ratios);

		var container = GetMockVehicleContainer(runData);

		var testPt = GetMockTestPowertrain(runData);

		var ptBuilder = new Mock<ISimplePowertrainBuilder>();
		ptBuilder.Setup(p => p.CreateTestPowertrain(It.IsAny<IVehicleContainer>(), 
				It.IsAny<bool>()))
			.Returns(testPt.Object);
		container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);

		var gbx = GetMockGearbox(container);
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => n.RPMtoRad());
        Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineN95hSpeed).Returns(2000.RPMtoRad());

        var shiftStrategy = new AMTShiftStrategy(container.Object);
		shiftStrategy.Gearbox = gbx.Object;

        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];

		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

        absTime += dt;

        var expectedT = tq.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];


		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));

        Assert.AreEqual(shiftExpected, shiftRequired);
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
    }


	[TestCase(2, 3, 800, 1400, 20_000, true, Description = "A gear would be skipped, but due to the uphill driving conditions, the next gear should be used")]
	[TestCase(3, 3, 1000, 1400, 30_000, false, Description = "No upshifting because acceleration would be to low")]
    public void Gearbox_ShiftUpUphill(int gear, int newGear, double tq, double n, double slopeResistance, bool shiftExpected)
	{
		// the first element 0.0 is just a placeholder for axlegear, not used in this test
		var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
		var runData = GetRunData(ratios);

		var container = GetMockVehicleContainer(runData);

		var accEstimationLookAhead = Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation;

		container.Setup(c => c.VehicleInfo.SlopeResistance(It.IsAny<Radian>())).Returns(slopeResistance.SI<Newton>());

		
		//container.Setup(c => c.DrivingCycleInfo.CycleLookAhead(accEstimationLookAhead))
		//	.Returns(new DrivingCycleData.DrivingCycleEntry() {
		//		Altitude = 100.SI<Meter>()
		//	});
		//container.Setup(c => c.DrivingCycleInfo.Altitude).Returns(0.SI<Meter>());






        var testPt = GetMockTestPowertrain(runData);

		var ptBuilder = new Mock<ISimplePowertrainBuilder>();
		ptBuilder.Setup(p => p.CreateTestPowertrain(
				It.IsAny<IVehicleContainer>(), 
				It.IsAny<bool>()))
			.Returns(testPt.Object);
		container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);

		var gbx = GetMockGearbox(container);
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => n.RPMtoRad());
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineN95hSpeed).Returns(2000.RPMtoRad());

		var shiftStrategy = new AMTShiftStrategy(container.Object);
		shiftStrategy.Gearbox = gbx.Object;

		var absTime = 0.SI<Second>();
		var dt = 2.SI<Second>();

		var expectedN = n.RPMtoRad();
		var angularVelocity = expectedN / ratios[gear];

		var gearShiftPosition = shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

		absTime += dt;

		var expectedT = tq.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];


		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));

		Assert.AreEqual(shiftExpected, shiftRequired);
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
    }



    [TestCase(7, 1, 1000, 1400, true)]
	[TestCase(7, 2, 400, 200, true)]
    public void InitStartGear(int gear, int newGear, double tq, double n, bool shiftExpected)
	{
		// the first element 0.0 is just a placeholder for axlegear, not used in this test
		var ratios = new[] { 0.0, 6.38, 5.2, 4.3, 3.2, 2.5, 1.8, 1, 0.76 };
		var runData = GetRunData(ratios);

		var container = GetMockVehicleContainer(runData);
		container.Setup(c => c.VehicleInfo.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());
		var testPt = GetMockTestPowertrain(runData);

		var ptBuilder = new Mock<ISimplePowertrainBuilder>();
		ptBuilder.Setup(p => p.CreateTestPowertrain(It.IsAny<IVehicleContainer>(), It.IsAny<bool>()))
			.Returns(testPt.Object);
		container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);

		var gbx = GetMockGearbox(container);
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => n.RPMtoRad());
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineN95hSpeed).Returns(2000.RPMtoRad());

		var shiftStrategy = new AMTShiftStrategy(container.Object);
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
        var runData = GetRunData(ratios);

		runData.DriverData = new DriverData() {
			PTODriveRoadsweepingGear = new GearshiftPosition((uint)ptoGear)
		};

        var container = GetMockVehicleContainer(runData);

		container.Setup(c => c.DrivingCycleInfo.CycleData).Returns(
			new CycleData() {
				LeftSample = new DrivingCycleData.DrivingCycleEntry() {
					PTOActive = PTOActivity.PTOActivityRoadSweeping,
					RoadGradient = 0.SI<Radian>(),
				}
			}
		);


        var testPt = GetMockTestPowertrain(runData);

        var ptBuilder = new Mock<ISimplePowertrainBuilder>();
		ptBuilder.Setup(p => p.CreateTestPowertrain(It.IsAny<IVehicleContainer>(), It.IsAny<bool>()))
			.Returns(testPt.Object);
        container.Setup(c => c.SimplePowertrainBuilder).Returns(ptBuilder.Object);

		var gbx = GetMockGearbox(container);
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => n.RPMtoRad());
        Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineN95hSpeed).Returns(2000.RPMtoRad());

        var shiftStrategy = new AMTShiftStrategy(container.Object);
		shiftStrategy.Gearbox = gbx.Object;

        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();

        var expectedN = n.RPMtoRad();
        var angularVelocity = expectedN / ratios[gear];

		shiftStrategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, 1.SI<NewtonMeter>(),
			angularVelocity);

        absTime += dt;

        var expectedT = tq.SI<NewtonMeter>();
		var torque = expectedT * ratios[gear];


		var shiftRequired = shiftStrategy.ShiftRequired(absTime, dt, torque, angularVelocity, expectedT, expectedN,
			new GearshiftPosition((uint)gear), -double.MaxValue.SI<Second>(), new ResponseSuccess(this));

		Assert.AreEqual(shiftExpected, shiftRequired);
		Assert.AreEqual(newGear, shiftStrategy.NextGear.Gear);
    }

	private Mock<ISimpleVehicleContainer> GetSimpleVehicleContainer(VectoRunData runData, out Mock<ITestPowertrainTransmission> gbx)
	{
		var simpleContainer = new Mock<ISimpleVehicleContainer>();

		simpleContainer.Setup(c => c.RunData).Returns(runData);
		simpleContainer.Setup(c => c.PowertrainInfo.HasCombustionEngine).Returns(true);

		gbx = GetTestGearbox(runData.GearboxData.Gears);
		
		simpleContainer.Setup(c => c.GearboxOutPort).Returns(gbx.Object);

		return simpleContainer;
	}

	private Mock<ITestPowertrainTransmission> GetTestGearbox(Dictionary<uint, GearData> ratios)
	{
		var gbx = new Mock<ITestPowertrainTransmission>();
		
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
		
		
		gbx.Setup(g => g.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(),
				It.IsAny<bool>()))
			.Returns((Second absTime, Second dt, NewtonMeter t, PerSecond n, bool dryRun) => {
				var ratio = gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;
				return dryRun
					? new ResponseDryRun(this) {
						Engine = {
							PowerRequest = n * t, EngineSpeed = n * ratio,
							DynamicFullLoadPower = (t / ratio + 2300.SI<NewtonMeter>()) * n * ratio,
						},
						Clutch = { PowerRequest = n * t }
					}
					: new ResponseSuccess(this) {
						Engine = { PowerRequest = n * t, EngineSpeed = n * ratio },
						Clutch = { PowerRequest = n * t }
					};
			});
		return gbx;
	}
    private Mock<ITestPowertrain> GetMockTestPowertrain(VectoRunData runData)
    {
        var testPt = new Mock<ITestPowertrain>();

		var simpleContainer = GetSimpleVehicleContainer(runData, out var gbx);
        // var tCnt = new Mock<ISimpleVehicleContainer>();
		testPt.Setup(t => t.Container).Returns(simpleContainer.Object);
		testPt.Setup(t => t.Gearbox).Returns(gbx.Object);
		
        var tEng = new Mock<ITestpowertrainCombustionEngine>();
        testPt.Setup(t => t.CombustionEngine).Returns(tEng.Object);
        tEng.Setup(e => e.EngineStationaryFullPower(It.IsAny<PerSecond>()))
            .Returns((PerSecond n) => runData.EngineData.FullLoadCurves[0].FullLoadStationaryPower(n));
        return testPt;
    }

    private static Mock<IVehicleContainer> GetMockVehicleContainer(VectoRunData runData)
    {
        var container = new Mock<IVehicleContainer>();
        container.Setup(c => c.RunData).Returns(runData);
        var veh = new Mock<IVehicleInfo>();
        veh.Setup(v => v.VehicleSpeed).Returns(10.SI<MeterPerSecond>());
        container.Setup(c => c.VehicleInfo).Returns(veh.Object);
		
		//EngineInfo
        var eng = GetEngineInfo(runData);
		container.Setup(c => c.EngineInfo).Returns(eng.Object);
		
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
		
        var vi = GetVehicleInfo();
		container.Setup(c => c.VehicleInfo).Returns(vi.Object);
		
		
		var wi = new Mock<IWheelsInfo>();
		container.Setup(c => c.WheelsInfo).Returns(wi.Object);
		wi.Setup(w => w.ReducedMassWheels).Returns(0.SI<Kilogram>());
		var axli = new Mock<IAxlegearInfo>();
		container.Setup(c => c.AxlegearInfo).Returns(axli.Object);
		axli.Setup(a => a.AxlegearLoss()).Returns(0.SI<Watt>());
        return container;
    }

	private static Mock<IVehicleInfo> GetVehicleInfo()
	{
		var vi = new Mock<IVehicleInfo>();
		vi.Setup(v => v.AirDragResistance(It.IsAny<MeterPerSecond>(), It.IsAny<MeterPerSecond>()))
			.Returns(new AirDragLossResult(0.SI<Watt>(), 0.SI<SquareMeter>(), 0.SI<MeterPerSecond>()));
		vi.Setup(v => v.RollingResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vi.Setup(v => v.SlopeResistance(It.IsAny<Radian>())).Returns(0.SI<Newton>());
		vi.Setup(v => v.VehicleSpeed).Returns(30.KMPHtoMeterPerSecond());
		vi.Setup(v => v.TotalMass).Returns(12000.SI<Kilogram>());
		return vi;
	}

	private static Mock<IEngineInfo> GetEngineInfo(VectoRunData runData)
	{
		var eng = new Mock<IEngineInfo>();

		eng.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);
		eng.Setup(e => e.EngineRatedSpeed).Returns(runData.EngineData.FullLoadCurves.First().Value.RatedSpeed);
		eng.Setup(e => e.EngineN95hSpeed).Returns(runData.EngineData.FullLoadCurves.First().Value.N95hSpeed);
		eng.Setup(e => e.EngineStationaryFullPower(It.IsAny<PerSecond>()))
			.Returns((PerSecond n) => runData.EngineData.FullLoadCurves[0].FullLoadStationaryPower(n));
		return eng;
	}

	private Mock<ITestPowertrainTransmission> GetMockTestGearbox(Dictionary<uint, GearData> ratios)
	{
		var gbx = new Mock<ITestPowertrainTransmission>();
		
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
		
		
		gbx.Setup(g => g.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(),
				It.IsAny<bool>()))
			.Returns((Second absTime, Second dt, NewtonMeter t, PerSecond n, bool dryRun) => {
				var ratio = gbx.Object.Gear == null ? 1.0 : ratios[gbx.Object.Gear.Gear].Ratio;
				return dryRun
					? new ResponseDryRun(this) {
						Engine = {
							PowerRequest = n * t, EngineSpeed = n * ratio,
							DynamicFullLoadPower = (t / ratio + 2300.SI<NewtonMeter>()) * n * ratio,
						},
						Clutch = { PowerRequest = n * t }
					}
					: new ResponseSuccess(this) {
						Engine = { PowerRequest = n * t, EngineSpeed = n * ratio },
						Clutch = { PowerRequest = n * t }
					};
			});
		return gbx;
	}
	

	private Mock<IAMTGearbox> GetMockGearbox(Mock<IVehicleContainer> container)
	{
		//Make sure no method that is not explicitly mocked is called
		var gbx = new Mock<IAMTGearbox>(MockBehavior.Strict);
		gbx.Setup(g => g.LastUpshift).Returns(-double.MaxValue.SI<Second>());
		gbx.Setup(g => g.LastDownshift).Returns(-double.MaxValue.SI<Second>());
		return gbx;
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

        var engineData = new CombustionEngineData() {
            IdleSpeed = 560.RPMtoRad(),
            FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
                { 0u, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHdr, EngineFldData)) }
            }
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

        var runData = new VectoRunData() {
            GearboxData = gearboxData,
            GearshiftParameters = new ShiftStrategyParameters() {
                StartSpeed = 2.SI<MeterPerSecond>(),
                TimeBetweenGearshifts = 6.SI<Second>(),
                DownshiftAfterUpshiftDelay = 2.SI<Second>(),
                UpshiftAfterDownshiftDelay = 2.SI<Second>(),
                UpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>()
            },
            EngineData = engineData,
            AxleGearData = new AxleGearData() {
                AxleGear = new TransmissionData() {
                    Ratio = 3.240355
                }
            },
            VehicleData = new VehicleData() {
                DynamicTyreRadius = 0.492.SI<Meter>(),
            }
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
}
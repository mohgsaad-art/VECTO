using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.ClutchTests;

public class ClutchRequestTests
{
	[Test,
	// clutch slipping
	TestCase(DrivingBehavior.Driving, 100, 0, 3, 0, 65.6889),
	TestCase(DrivingBehavior.Driving, 100, 5, 1, 100, 65.6889), // would cause clutch losses!
	TestCase(DrivingBehavior.Braking, 100, 80, 1, 100, 80),
	// clutch opened - would cause neg. clutch losses (which is not possible), torque is adapted
	TestCase(DrivingBehavior.Halted, 100, 30, 0, 51.1569, 58.643062),
	// clutch closed
	TestCase(DrivingBehavior.Driving, 100, 80, 3, 100, 80),
	TestCase(DrivingBehavior.Braking, 100, 80, 3, 100, 80),
	TestCase(DrivingBehavior.Driving, 100, 30, 3, 100, 30),
		// clutch opened due to braking
		//TestCase(DrivingBehavior.Braking, 0, 55, null, null),
	]
	public void TestClutch(DrivingBehavior drivingBehavior, double torque, double angularSpeed, int gear, double expectedTorque,
		double expectedEngineSpeed)
	{
		var container = GetMockVehicleContainer(gear, drivingBehavior);

		var mockIdleController = new Mock<IIdleController>();
		var engineData = new CombustionEngineData() {
			IdleSpeed = 560.RPMtoRad(),
			FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
				{0u, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFLDHdr, EngineFldData))}
			}
		};

		var outPort = new Mock<ITnOutPort>();
		NewtonMeter reqTq = null;
		PerSecond reqN = null;
		outPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>(), It.IsAny<bool>()))
			.Returns((Second absTime, Second dt, NewtonMeter t, PerSecond n, bool dryRun) => {
				reqTq = t;
				reqN = n;
				return new ResponseSuccess(this) { };
			});

		var clutch = new Clutch(container.Object, engineData) { IdleController = mockIdleController.Object };

        var inPort = clutch.InPort();
		inPort.Connect(outPort.Object);

		var clutchOutPort = clutch.OutPort();

		clutchOutPort.Request(0.SI<Second>(), 0.SI<Second>(), torque.SI<NewtonMeter>(), angularSpeed.SI<PerSecond>(), false);

		Assert.AreEqual(expectedTorque, reqTq.Value(), 0.001);
		Assert.AreEqual(expectedEngineSpeed, reqN.Value(), 0.001);
	}

	private static Mock<IVehicleContainer> GetMockVehicleContainer(int gear, DrivingBehavior drivingBehavior)
	{
		var container = new Mock<IVehicleContainer>();
		var gi = new Mock<IGearboxInfo>();
		container.Setup(c => c.GearboxInfo).Returns(gi.Object);
		gi.Setup(g => g.GearEngaged(It.IsAny<Second>())).Returns(true);
		gi.Setup(g => g.Gear).Returns(new GearshiftPosition((uint)gear));
		var ci = new Mock<IClutchInfo>();
		container.Setup(c => c.ClutchInfo).Returns(ci.Object);
		ci.Setup(c => c.ClutchClosed(It.IsAny<Second>())).Returns(true);
		var vi = new Mock<IVehicleInfo>();
		container.Setup(c => c.VehicleInfo).Returns(vi.Object);
		vi.Setup(v => v.VehicleStopped).Returns(false);
		var eng = new Mock<IEngineInfo>();
		container.Setup(c => c.EngineInfo).Returns(eng.Object);
		eng.Setup(e => e.EngineSpeed).Returns(800.RPMtoRad());
		eng.Setup(e => e.EngineOn).Returns(true);
		var driver = new Mock<IDriverInfo>();
		container.Setup(c => c.DriverInfo).Returns(driver.Object);
		driver.Setup(d => d.DriverBehavior).Returns(drivingBehavior);

		var cyi = new Mock<IDrivingCycleInfo>();
		container.Setup(c => c.DrivingCycleInfo).Returns(cyi.Object);
		var runData = new VectoRunData() {
			GearshiftParameters = new ShiftStrategyParameters() {
				StartAcceleration = 0.8.SI<MeterPerSquareSecond>()
			},
			Aux = new List<VectoRunData.AuxData>()
		};
		container.Setup(c => c.RunData).Returns(runData);

        return container;
	}

	const string EngineFLDHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	private static readonly string[] EngineFldData = new[] {
		"560,1180,-149,0.6",
		"600,1282,-148,0.6",
		"799.9999999,1791,-149,0.6",
		"1000,2300,-160,0.6",
		"1200,2300,-179,0.6",
		"1400,2300,-203,0.6",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25",
	};
}
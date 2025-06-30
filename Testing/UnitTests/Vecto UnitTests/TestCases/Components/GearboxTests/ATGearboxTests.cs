using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;
using IIdleController = TUGraz.VectoCore.Models.SimulationComponent.IIdleController;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearboxTests;

public class ATGearboxTests
{
    [Test,
		TestCase(200, 2u, 3u, 562, 600, 19.7908, double.NaN),
		TestCase(200, 2u, 3u, 562, 620, 19.5023, double.NaN),
        TestCase(400, 2u, 3u, 562, 600, 39.5816, double.NaN),
		TestCase(400, 2u, 3u, 562, 620, 39.0046, 3183.0086),
		TestCase(400, 3u, 2u, 500, 490, 30.7900, double.NaN),
		TestCase(400, 3u, 2u, 550, 490, 32.4643, 2328.0855),
		TestCase(600, 2u, 3u, 562, 600, 59.3723, 4774.5128),
		TestCase(600, 2u, 3u, 562, 620, 58.5069, double.NaN),
		TestCase(600, 3u, 2u, 550, 490, 48.6965, double.NaN),
		TestCase(800, 2u, 3u, 562, 600, 79.1632, double.NaN),
        TestCase(800, 2u, 3u, 562, 620, 78.0092, double.NaN),
        ]
    public void TestShiftLossComputation(double torqueDemand, uint gear, uint nextGear, double preShiftRpm,
            double postShiftRpm, double expectedShiftLoss, double expectedShiftLossEnergy)
    {
        var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
        var container = CreateVehicle(ratios);

		var shiftStrategy = new Mock<IShiftStrategy>();
		
		var port = new Mock<ITnOutPort>();
		port.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.IsAny<bool>())).Returns(new ResponseSuccess(this));

		var idleCtl = new Mock<IIdleController>();

		// Setup DUT:
        var gbx = new APTGearbox(container.Object, shiftStrategy.Object);
		gbx.Connect(port.Object);
		gbx.IdleController = idleCtl.Object;

		// mock shift strategy behavior
		shiftStrategy
			.Setup(s => s.InitGear(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>())).Returns(() => {
				gbx.Disengaged = false;
				return new GearshiftPosition(gear, true);
			});
		bool gearshiftDone = false;
		shiftStrategy.Setup(s => s.ShiftRequired(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), It.IsAny<GearshiftPosition>(),
			It.IsAny<Second>(), It.IsAny<IResponse>())).Returns(
			(Second t, Second dt, NewtonMeter to, PerSecond no, NewtonMeter ti, PerSecond ni, GearshiftPosition g,
				Second lst, IResponse resp) => {
				var retVal = no.IsEqual(preShiftRpm.RPMtoRad()) || gearshiftDone ? false : true;
				gearshiftDone = gearshiftDone || retVal;
				return retVal;
			});
		shiftStrategy.Setup(s => s.NextGear).Returns(new GearshiftPosition(nextGear, true));
		shiftStrategy
			.Setup(s => s.Engage(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>())).Returns(new GearshiftPosition(nextGear, true));

		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(preShiftRpm.RPMtoRad() * ratios[gear - 1]);
		container.Setup(c => c.CommitSimulationStep(It.IsAny<Second>(), It.IsAny<Second>()))
			.Callback((Second t, Second dt) => gbx.CommitSimulationStep(t, dt, null));

        
        var absTime = 20.SI<Second>();
        var dt = 0.5.SI<Second>();

		var init = gbx.Initialize(0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
        var response = gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
		
        Assert.IsInstanceOf<ResponseSuccess>(response);
        container.Object.CommitSimulationStep(absTime, dt);
        absTime += dt;

        response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());
        Assert.IsInstanceOf<ResponseFailTimeInterval>(response);

        dt = ((ResponseFailTimeInterval)response).DeltaT;
        response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

        Assert.IsInstanceOf<ResponseSuccess>(response);
        Assert.AreEqual(expectedShiftLoss, gbx.CurrentState.PowershiftLoss.Value(), 1e-3);
        Assert.AreEqual(gear + (postShiftRpm > preShiftRpm ? 1 : -1), gbx.Gear.Gear);

        if (!double.IsNaN(expectedShiftLossEnergy)) {
            var modData = new MockModalDataContainer();
            gbx.CommitSimulationStep(absTime, dt, modData);
            var shiftLossE = (Watt)modData[ModalResultField.P_gbx_shift_loss] * dt;
            Assert.AreEqual(expectedShiftLossEnergy, shiftLossE.Value(), 1e-3);
        }
    }

    [Test,
        //TestCase(200, 2u, 562, 620, 19.5023),
        TestCase(400, 2u, 3u, 562, 620, 39.0046, 3183.0086),
        //TestCase(600, 2u, 562, 620, 58.5069),
        //TestCase(800, 2u, 562, 620, 78.0092),
        //TestCase(200, 2u, 562, 600, 19.7908),
        //TestCase(400, 2u, 562, 600, 39.5816),
        TestCase(600, 2u, 3u, 562, 600, 59.3723, 4774.5128),
        //TestCase(800, 2u, 562, 600, 79.1632),
        //TestCase(400, 3u, 500, 490, 30.7900),
        TestCase(400, 3u, 2u, 550, 490, 32.4643, 2328.0855),
        //TestCase(600, 3u, 550, 490, 48.6965),
        ]
    public void TestSplittingShiftLossesTwoIntervals(double torqueDemand, uint gear, uint nextGear, double preShiftRpm,
            double postShiftRpm, double expectedShiftLoss, double expectedShiftLossEnergy)
    {
		var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
		var container = CreateVehicle(ratios);

		var shiftStrategy = new Mock<IShiftStrategy>();

		var port = new Mock<ITnOutPort>();
		port.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.IsAny<bool>())).Returns(new ResponseSuccess(this));

		var idleCtl = new Mock<IIdleController>();

		// Setup DUT:
		var gbx = new APTGearbox(container.Object, shiftStrategy.Object);
		gbx.Connect(port.Object);
		gbx.IdleController = idleCtl.Object;

		// mock shift strategy behavior
		shiftStrategy
			.Setup(s => s.InitGear(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>())).Returns(() => {
				gbx.Disengaged = false;
				return new GearshiftPosition(gear, true);
			});
		bool gearshiftDone = false;
		shiftStrategy.Setup(s => s.ShiftRequired(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), It.IsAny<GearshiftPosition>(),
			It.IsAny<Second>(), It.IsAny<IResponse>())).Returns(
			(Second t, Second dt, NewtonMeter to, PerSecond no, NewtonMeter ti, PerSecond ni, GearshiftPosition g,
				Second lst, IResponse resp) => {
				var retVal = no.IsEqual(preShiftRpm.RPMtoRad()) || gearshiftDone ? false : true;
				gearshiftDone = gearshiftDone || retVal;
				return retVal;
			});
		shiftStrategy.Setup(s => s.NextGear).Returns(new GearshiftPosition(nextGear, true));
		shiftStrategy
			.Setup(s => s.Engage(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
				It.IsAny<PerSecond>())).Returns(new GearshiftPosition(nextGear, true));

		var engineSpeed = preShiftRpm.RPMtoRad() * ratios[gear - 1];
		Mock.Get(container.Object.EngineInfo).Setup(e => e.EngineSpeed).Returns(() => engineSpeed);

		var modData = new MockModalDataContainer();

        gbx.Gear = new GearshiftPosition(gear, true);

        var absTime = 20.SI<Second>();
        var dt = 0.5.SI<Second>();
		var init = gbx.Initialize(0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
        var response = gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
        gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad(), false);


        Assert.IsInstanceOf<ResponseSuccess>(response);
        gbx.CommitSimulationStep(absTime, dt, modData);
        absTime += dt;

        response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());
        Assert.IsInstanceOf<ResponseFailTimeInterval>(response);

        var splitFactor = 0.75;
        var shiftTime = ((ResponseFailTimeInterval)response).DeltaT;
        dt = shiftTime * splitFactor;
        response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

        Assert.IsInstanceOf<ResponseSuccess>(response);
        Assert.AreEqual(expectedShiftLoss, gbx.CurrentState.PowershiftLoss.Value(), 1e-3);
        Assert.AreEqual(gear + (postShiftRpm > preShiftRpm ? 1 : -1), gbx.Gear.Gear);

		gbx.CommitSimulationStep(absTime, dt, modData);
        var shiftLoss1 = (Watt)modData[ModalResultField.P_gbx_shift_loss] * dt;
        Assert.AreEqual(expectedShiftLossEnergy * splitFactor, shiftLoss1.Value(), 1e-3);
        gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad(), false);

        absTime += dt;
        dt = 0.5.SI<Second>();
		engineSpeed = postShiftRpm.RPMtoRad() * ratios[nextGear - 1];


        response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

        Assert.IsInstanceOf<ResponseSuccess>(response);
        gbx.CommitSimulationStep(absTime, dt, modData);
        var shiftLoss2 = (Watt)modData[ModalResultField.P_gbx_shift_loss] * dt;
        Console.WriteLine("expected shiftloss energy: {0}, sum of shift loss energy: {1} ({2} + {3})", expectedShiftLossEnergy, shiftLoss1 + shiftLoss2, shiftLoss1, shiftLoss2);
        Assert.AreEqual(expectedShiftLossEnergy * (1 - splitFactor), shiftLoss2.Value(), 1e-3);

        Assert.AreEqual(expectedShiftLossEnergy, (shiftLoss1 + shiftLoss2).Value(), 1e-3);
    }

    private Mock<IVehicleContainer> CreateVehicle(double[] ratios)
	{
		var runData = new VectoRunData() {
			EngineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad(),
				//Inertia = 5.SI<KilogramSquareMeter>()
			},
			GearboxData = new GearboxData() {
				Type = GearboxType.ATSerial,
				Gears = ratios.Select((ratio, i) =>
						Tuple.Create((uint)i,
							new GearData {
								LossMap = ratio.IsEqual(1)
									? TransmissionLossMapReader.Create(0.96, ratio, $"Gear {i}")
									: TransmissionLossMapReader.Create(0.98, ratio, $"Gear {i}"),
								Ratio = ratio,
								ShiftPolygon = ShiftPolygonReader.Create(InputDataHelper.InputDataAsTableData(ShiftLineHdr, ShiftLineData)),
								TorqueConverterRatio = i == 0 ? ratio : double.NaN,
								TorqueConverterGearLossMap = i == 0
									? TransmissionLossMapReader.Create(0.98, ratio,
										$"Gear {i}")
									: null,
								TorqueConverterShiftPolygon =
									i == 0 ? ShiftPolygonReader.Create(InputDataHelper.InputDataAsTableData(ShiftLineHdr, ShiftLineData)) : null
							}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2),

				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 0.SI<Second>(),

				PowershiftShiftTime = 0.8.SI<Second>(),
				TorqueConverterData = TorqueConverterDataReader.Create(
					InputDataHelper.InputDataAsTableData(TcCharHdr, TcCharData), 1000.RPMtoRad(), 1500.RPMtoRad(),
					ExecutionMode.Declaration, 1, DeclarationData.Gearbox.UpshiftMinAcceleration,
					DeclarationData.Gearbox.UpshiftMinAcceleration)
			},
			GearshiftParameters = new ShiftStrategyParameters() {
				TimeBetweenGearshifts = 1.SI<Second>(),
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
				StartTorqueReserve = 0.2,
				TorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
			}
		};

		var container = new Mock<IVehicleContainer>();

		container.Setup(c => c.RunData).Returns(runData);
		var ci = new Mock<IClutchInfo>();
		container.Setup(c => c.ClutchInfo).Returns(ci.Object);
		ci.Setup(c => c.ClutchClosed(It.IsAny<Second>())).Returns(true);

		var vi = new Mock<IVehicleInfo>();
		container.Setup(c => c.VehicleInfo).Returns(vi.Object);
		vi.Setup(v => v.VehicleStopped).Returns(false);

		var di = new Mock<IDriverInfo>();
		container.Setup(c => c.DriverInfo).Returns(di.Object);
		di.Setup(d => d.DriverAcceleration).Returns(0.SI<MeterPerSquareSecond>());
		di.Setup(d => d.DriverBehavior).Returns(DrivingBehavior.Driving);

		var ei = new Mock<IEngineInfo>();
		var eCtl = ei.As<IEngineControl>();
		container.Setup(c => c.EngineInfo).Returns(ei.Object);
		container.Setup(c => c.EngineCtl).Returns(eCtl.Object);
		ei.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);
		eCtl.Setup(e => e.CombustionEngineOn).Returns(true);

        return container;
	}

	const string ShiftLineHdr = "engine torque [Nm],downshift rpm [1/min],upshift rpm [1/min]";

	private static readonly string[] ShiftLineData = new[] {
		"-200,700,800",
		"0,700,800",
		"3000,700,800",
	};

	const string TcCharHdr = "Speed Ratio, Torque Ratio,MP1000";

	private static readonly string[] TcCharData = new[] {
		"0.0,1.80,377.80",
		"0.1,1.71,365.21",
		"0.2,1.61,352.62",
		"0.3,1.52,340.02",
		"0.4,1.42,327.43",
		"0.5,1.33,314.84",
		"0.6,1.23,302.24",
		"0.7,1.14,264.46",
		"0.8,1.04,226.68",
		"0.9,1.02,188.90",
		"1.0,1.0,0.00",
		"1.100,0.999,-40.34",
		"1.222,0.998,-80.34",
		"1.375,0.997,-136.11",
		"1.571,0.996,-216.52",
		"1.833,0.995,-335.19",
		"2.200,0.994,-528.77",
		"2.750,0.993,-883.40",
		"4.400,0.992,-2462.17",
		"11.000,0.991,-16540.98",
	};
}
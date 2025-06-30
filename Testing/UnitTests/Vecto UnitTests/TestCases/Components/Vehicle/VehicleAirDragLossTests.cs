using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Vehicle;

public class VehicleAirDragLossTests
{
	public static readonly double Tolerance = 0.001;


    [
            TestCase(0, 0, 0.5, 3.0, 0),
            TestCase(0, 1, 0.5, 3.0, 0.5657),
            TestCase(60, 0, 0.5, 3.0, 1257.2212),
            TestCase(60, 1, 0.5, 3.0, 1291.6202),
            TestCase(60, 0.5, 0.5, 3.0, 1274.3082),
            TestCase(72, 0.5, 0.5, 3.0, 1765.8214),
            TestCase(72, 1, 3, 3.0, 2001.6463)
        ]
    public void VehicleAirResistanceTest(double vehicleSpeed, double acceleration, double dt, double height,
            double expected)
    {
        var container = GetMockContainer();

		var vehicleData = GetVehicleData();
		var airdragData = GetAirdragData("TractorSemitrailer",
			6.46.SI<SquareMeter>(), height.SI<Meter>()); 
        var vehicle = new VectoCore.Models.SimulationComponent.Impl.Vehicle(container.Object, vehicleData, airdragData);
        //new DummyCycle(container);

		var mockPort = new Mock<IFvOutPort>();
		vehicle.InPort().Connect(mockPort.Object);

        // ====================

        vehicle.Initialize(vehicleSpeed.KMPHtoMeterPerSecond(), 0.SI<Radian>());

        var nextSpeed = vehicleSpeed.KMPHtoMeterPerSecond() + acceleration.SI<MeterPerSquareSecond>() * dt.SI<Second>();
        var avgForce = vehicle.AirDragResistance(vehicleSpeed.KMPHtoMeterPerSecond(), nextSpeed);
        Assert.AreEqual(expected, avgForce.AirdragForce.Value(), Tolerance);
    }


	[TestCase]
    public void VehicleAirDragPowerLossDeclarationTest()
    {
		var container = GetMockContainer();

        var vehicleData = GetVehicleData();
        var airdragData = GetAirdragData("TractorSemitrailer",
			6.2985.SI<SquareMeter>(), 3.SI<Meter>());

        var vehicle = new VectoCore.Models.SimulationComponent.Impl.Vehicle(container.Object, vehicleData, airdragData);

		var mockPort = new Mock<IFvOutPort>();
		mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<Newton>(),
				It.IsAny<MeterPerSecond>(), It.IsAny<bool>()))
			.Returns(new ResponseSuccess(this));
        vehicle.InPort().Connect(mockPort.Object);

        var writer = new MockModalDataContainer();

        vehicle.Initialize(80.KMPHtoMeterPerSecond(), 0.SI<Radian>());

        var absTime = 0.SI<Second>();
        var dt = 0.5.SI<Second>();

        vehicle.Request(absTime, dt, 0.SI<MeterPerSquareSecond>(), 0.SI<Radian>());
        vehicle.CommitSimulationStep(absTime, dt, writer);

        Assert.AreEqual(45956.3024, ((SI)writer[ModalResultField.P_air]).Value(), 0.1);

        vehicle.Request(absTime, dt, 1.SI<MeterPerSquareSecond>(), 0.SI<Radian>());
        vehicle.CommitSimulationStep(absTime, dt, writer);
        Assert.AreEqual(47448.0989, ((SI)writer[ModalResultField.P_air]).Value(), 0.1);
    }

	private static Mock<IVehicleContainer> GetMockContainer()
	{
		var container = new Mock<IVehicleContainer>();
		var runData = new VectoRunData() {
			ExecutionMode = ExecutionMode.Declaration,
			ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
		};
		var driver = new Mock<IDriverInfo>();
		driver.Setup(d => d.DriverBehavior).Returns(DrivingBehavior.Driving);
		var cycle = new Mock<IDrivingCycleInfo>();
		cycle.Setup(c => c.CycleStartDistance).Returns(0.SI<Meter>());
		cycle.Setup(c => c.CycleData).Returns(new CycleData() { });
		container.Setup(c => c.DriverInfo).Returns(driver.Object);
		container.Setup(c => c.DrivingCycleInfo).Returns(cycle.Object);
		container.Setup(c => c.RunData).Returns(runData);
		var pi = new Mock<IPowertainInfo>();
		pi.Setup(p => p.VehicleArchitecutre).Returns(VectoSimulationJobType.ConventionalVehicle);
		pi.Setup(p => p.HasCombustionEngine).Returns(true);
		container.Setup(c => c.PowertrainInfo).Returns(pi.Object);
		var eng = new Mock<IEngineInfo>();
		eng.Setup(e => e.EngineN95hSpeed).Returns(2500.RPMtoRad());
		container.Setup(c => c.EngineInfo).Returns(eng.Object);
		var axl = new Mock<IAxlegearInfo>();
		axl.Setup(a => a.Ratio).Returns(1);
		container.Setup(c => c.AxlegearInfo).Returns(axl.Object);
		var whl = new Mock<IWheelsInfo>();
		whl.Setup(w => w.DynamicTyreRadius).Returns(0.5.SI<Meter>());
		container.Setup(c => c.WheelsInfo).Returns(whl.Object);
		var gbx = new Mock<IGearboxInfo>();
		gbx.Setup(g => g.GetGearData(It.IsAny<uint>())).Returns(new GearData() { Ratio = 1 });
		container.Setup(c => c.GearboxInfo).Returns(gbx.Object);

		return container;
	}

    private AirdragData GetAirdragData(string airdragParams, SquareMeter cdxA, Meter height)
	{
		var dataAdapter = new AirdragDataAdapter();
		var airdragData = dataAdapter.GetDeclarationAirResistanceCurve(airdragParams, cdxA, height);

		return new AirdragData() {
			CrossWindCorrectionCurve =
				new CrosswindCorrectionCdxALookup(cdxA, 0.SI<SquareMeter>(), airdragData, CrossWindCorrectionMode.DeclarationModeCorrection),
			
		};
	}

	private VehicleData GetVehicleData()
	{
		return new VehicleData() {
			CurbMass = 7100.SI<Kilogram>(),
			DynamicTyreRadius = 0.4882675.SI<Meter>(),
            AirDensity = DeclarationData.AirDensity,
            AxleData = new List<Axle>() {
                new Axle() {
                    AxleWeightShare = 0.5,
                    Inertia = 14.9.SI<KilogramSquareMeter>(),
                    TyreTestLoad = 31300.SI<Newton>(),
                    TwinTyres = false
				},
				new Axle() {
					AxleWeightShare = 0.5,
					Inertia = 14.9.SI<KilogramSquareMeter>(),
					TyreTestLoad = 31300.SI<Newton>(),
					TwinTyres = false
				},
            },
		};
	}
}
using Moq;
using NUnit.Framework;
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
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Vehicle;

public class VehicleTests
{
	[TestCase(17.210535, 0.00366547048, -0.256231159, -2332.5362, 16.954303841)]
	public void VehicleRequestTest(double velocityMps, double grad, double acc, double expectedForce, double expectedVelocity)
	{
		var container = GetMockContainer();

		var vehicleData = GetVehicleData();
		var airdragData = GetAirdragData("CoachBus",
			3.2634.SI<SquareMeter>(), 4.SI<Meter>());

        var vehicle = new VectoCore.Models.SimulationComponent.Impl.Vehicle(container.Object, vehicleData, airdragData);

		var mockPort = new Mock<IFvOutPort>();
		Newton force = null;
		MeterPerSecond velocity = null;
		mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<Newton>(),
				It.IsAny<MeterPerSecond>(), It.IsAny<bool>()))
			.Returns((Second _, Second _, Newton f, MeterPerSecond v, bool _) => {
				force = f;
				velocity = v;
				return new ResponseSuccess(this);
			});
        vehicle.InPort().Connect(mockPort.Object);

		vehicle.Initialize(velocityMps.SI<MeterPerSecond>(), 0.SI<Radian>());

		var requestPort = vehicle.OutPort();

		var absTime = 0.SI<Second>();
		var dt = 1.SI<Second>();

		var accell = acc.SI<MeterPerSquareSecond>();
		var gradient = Math.Atan(grad).SI<Radian>();

		requestPort.Request(absTime, dt, accell, gradient, false);

		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<Newton>(),
			It.IsAny<MeterPerSecond>(), It.IsAny<bool>()), Times.Once);
		Assert.AreEqual(expectedForce, force.Value(), 0.0001);
		Assert.AreEqual(expectedVelocity, velocity.Value(), 0.0001);
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
		cycle.Setup(c => c.CycleData).Returns(new CycleData());
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
			CurbMass = 15700.SI<Kilogram>(),
			Loading = 3300.SI<Kilogram>(),
			DynamicTyreRadius = 0.520.SI<Meter>(),
			AirDensity = DeclarationData.AirDensity,
			AxleData = new List<Axle>() {
				new Axle() {
					AxleWeightShare = 0.4375,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					TyreTestLoad = 62538.75.SI<Newton>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false
				},
				new Axle() {
					AxleWeightShare = 0.375,
					Inertia = 10.83333.SI<KilogramSquareMeter>(),
					TyreTestLoad = 52532.55.SI<Newton>(),
					RollResistanceCoefficient = 0.0065,
					TwinTyres = true
				},
				new Axle() {
					AxleWeightShare = 0.1875,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					TyreTestLoad = 62538.75.SI<Newton>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false
				},
            },
		};
	}
}
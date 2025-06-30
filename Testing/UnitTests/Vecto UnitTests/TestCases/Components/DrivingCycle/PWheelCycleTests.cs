using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.DrivingCycle;

public class PWheelCycleTests
{
	[TestCase]
	public void PWheelDrivingCycleTest()
	{
		var runData = GetDummyRunData();

		var cycleFile = InputDataHelper.InputDataAsStream("<t>,<Pwheel>,<gear>,<n>,<Padd>", new[] {
			"1,89,2,1748,1.300",
			"2,120,2,1400,0.4"
		});
		var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycleFile, CycleType.PWheel, "", false);

		var container = new Mock<IVehicleContainer>().Object;
		Mock.Get(container).Setup(c => c.RunData).Returns(runData);

        var cycle = new PWheelCycle(container, drivingCycle);
		//Mock.Get(container).Setup(c => c.DrivingCycleInfo).Returns(cycle);

        var port = new Mock<ITnOutPort>();
		port.Setup(p => p.Initialize(It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>()))
			.Returns(new ResponseSuccess(this));

		cycle.Connect(port.Object);

		cycle.Initialize();

		Assert.AreEqual(cycle.CycleData.LeftSample.Time, 1.SI<Second>());
		Assert.AreEqual(cycle.CycleData.RightSample.Time, 2.SI<Second>());

		Assert.AreEqual(1748.RPMtoRad() / (2.3 * 3.5), cycle.CycleData.LeftSample.WheelAngularVelocity);
		Assert.AreEqual(1400.RPMtoRad() / (2.3 * 3.5), cycle.CycleData.RightSample.WheelAngularVelocity);

		Assert.AreEqual(89.SI(Unit.SI.Kilo.Watt), cycle.CycleData.LeftSample.PWheel);
		Assert.AreEqual(120.SI(Unit.SI.Kilo.Watt), cycle.CycleData.RightSample.PWheel);

		Assert.AreEqual(2u, cycle.CycleData.LeftSample.Gear);
		Assert.AreEqual(2u, cycle.CycleData.RightSample.Gear);

		Assert.AreEqual(1300.SI<Watt>(), cycle.CycleData.LeftSample.AdditionalAuxPowerDemand);
		Assert.AreEqual(400.SI<Watt>(), cycle.CycleData.RightSample.AdditionalAuxPowerDemand);

		Assert.AreEqual(89.SI(Unit.SI.Kilo.Watt) / (1748.RPMtoRad() / (2.3 * 3.5)), cycle.CycleData.LeftSample.Torque);
		Assert.AreEqual(120.SI(Unit.SI.Kilo.Watt) / (1400.RPMtoRad() / (2.3 * 3.5)), cycle.CycleData.RightSample.Torque);

    }

	private VectoRunData GetDummyRunData()
	{
		return new VectoRunData() {
			GearboxData = new GearboxData {
				Gears = new Dictionary<uint, GearData> {
					{ 1, new GearData { Ratio = 2.0 } },
					{ 2, new GearData { Ratio = 3.5 } }
				}
			},
			VehicleData = new VehicleData {
				DynamicTyreRadius = 0.5.SI<Meter>()
			},
			AxleGearData = new AxleGearData {
				AxleGear = new TransmissionData {
					Ratio = 2.3
				}
			},
			ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>()
		};
    }
}
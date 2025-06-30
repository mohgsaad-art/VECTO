using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Supercap;

public class SuperCapTests
{
	[
		TestCase(0.5, 0.5, -500, 0.499875863),
		TestCase(0.5, 1, -14000, 0.4929779240),
		TestCase(0.5, 1, 7000, 0.503456863),
		TestCase(0.35, 0.5, -200, 0.3499290696),
		TestCase(0.35, 0.5, -16000, 0.3441842440),
		TestCase(0.75, 0.5, -300, 0.7499503587),
		TestCase(0.75, 0.5, -14500, 0.74758944005),
	]

	public void SuperCapRequestTest(double initialSoC, double simInterval, double powerDemand, double expectedSoC)
	{
		var batteryData = new SuperCapData() {
			Capacity = 37.SI<Farad>(),
			InternalResistance = 0.02.SI<Ohm>(),
			MinVoltage = 0.SI<Volt>(),
			MaxVoltage = 330.SI<Volt>(),
			MaxCurrentDischarge = -200.SI<Ampere>(),
			MaxCurrentCharge = 200.SI<Ampere>()
		};

		var container = new MockVehicleContainer();
		var superCap = new SuperCap(container, batteryData);
		var modData = new MockModalDataContainer();
		superCap.Initialize(initialSoC);

		//Assert.AreEqual(3.2*200, bat.Voltage.Value());

		var absTime = 0.SI<Second>();
		var dt = simInterval.SI<Second>();
		var response = superCap.Request(absTime, dt, powerDemand.SI<Watt>(), false);
		Assert.IsInstanceOf<RESSResponseSuccess>(response);
		superCap.CommitSimulationStep(absTime, dt, modData);

		Assert.AreEqual(expectedSoC, superCap.StateOfCharge, 1e-9);
	}
}
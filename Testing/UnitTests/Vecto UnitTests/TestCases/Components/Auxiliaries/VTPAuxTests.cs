using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Auxiliaries;

public class VTPAuxTests
{
	[
		TestCase(200, 53.998),
		TestCase(500, 843.713),
		TestCase(1000, 6749.700),
		TestCase(1300, 14829.091),
		TestCase(1500, 22780.238)
	]
	public void TestEngineFanPowerDemand(double fanSpeedRPM, double expectedPowerDemand)
	{
		var engineFan = new EngineFanAuxiliary(new[] { 5.5e-7, 14.62, 108.5 }, 0.225.SI<Meter>());

		Assert.AreEqual(expectedPowerDemand, engineFan.PowerDemand(
			new DrivingCycleData.DrivingCycleEntry() { FanSpeed = fanSpeedRPM.RPMtoRad() }).Value(), 1e-3);
	}

	[
		TestCase(14, 20),
		TestCase(28, 40),
		TestCase(7, 10)
	]
	public void TestEngineFanPowerDemandWithElectricalPower(double fanElectricalPower, double expectedPowerDemand)
	{
		var engineFan = new EngineFanAuxiliary(new[] { 5.5e-7, 14.62, 108.5 }, 0.225.SI<Meter>());

		Assert.AreEqual(expectedPowerDemand, engineFan.PowerDemand(
			new DrivingCycleData.DrivingCycleEntry() { FanElectricalPower = fanElectricalPower.SI<Watt>() }).Value(), 1e-3);
	}
}
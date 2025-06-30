using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.CombustionEngine;

[TestFixture]
public class CombustionEngineInertiaTests
{
	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, 0.002, null, 0.8),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 0.0032, null, 1.28),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 0.0033, GearboxType.ATPowerSplit, 1.3719),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 0.0033, GearboxType.MT, 1.3787),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 0.005, GearboxType.MT, 3.06),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 0.006, GearboxType.MT, 3.33),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 0.006, GearboxType.ATPowerSplit, 3.23),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, 0.006, null, 2.03)
	]
	public void EngineInertiaTest(VectoSimulationJobType jobType, double displacement, GearboxType? gbxType,
		double inertia)
	{
		var result = DeclarationData.Engine.EngineInertia(
			jobType,
			displacement.SI<CubicMeter>(),
			gbxType);

		Assert.AreEqual(inertia, result.Value(), 1e-4);
	}

}
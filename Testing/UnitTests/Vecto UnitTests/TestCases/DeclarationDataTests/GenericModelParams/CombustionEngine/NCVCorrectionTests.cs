using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.CombustionEngine;

public class NCVCorrectionTests
{
	[
		TestCase("Diesel CI", null, 1.0),
		TestCase("Ethanol CI", null, 1.011811),
		TestCase("Petrol PI", null, 1.0),
		TestCase("Ethanol PI", null, 0.993174),
		TestCase("NG PI", TankSystem.Liquefied, 0.918533),
		TestCase("NG PI", TankSystem.Compressed, 0.939583)
	]
	public void TestNCVCorrection(string fuelTypeStr, TankSystem? tankSystem, double expectedCorrectionFactor)
	{
		var fuelType = fuelTypeStr.ParseEnum<FuelType>();
		var cf = DeclarationData.FuelData.Lookup(fuelType, tankSystem).HeatingValueCorrection;

		Assert.AreEqual(expectedCorrectionFactor, cf, 1e-6);
	}
}
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration.Auxiliaries;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils;

public class AuxiliaryTypeHelperTests
{
	
	[TestCase(AuxiliaryType.Fan, "Fan")]
	[TestCase(AuxiliaryType.SteeringPump,"Steering pump")]
	[TestCase(AuxiliaryType.ElectricSystem, "Electric System")]
	[TestCase(AuxiliaryType.HVAC, "HVAC")]
	[TestCase(AuxiliaryType.PneumaticSystem, "Pneumatic System")]
	public void TestParseAuxiliaryType(AuxiliaryType expectedAuxType, string auxString)
	{
		Assert.AreEqual(expectedAuxType, AuxiliaryTypeHelper.Parse(auxString));
		
	}

	[TestCase(Constants.Auxiliaries.Names.Fan, AuxiliaryType.Fan)]
	[TestCase(Constants.Auxiliaries.Names.SteeringPump, AuxiliaryType.SteeringPump)]
	[TestCase(Constants.Auxiliaries.Names.HeatingVentilationAirCondition, AuxiliaryType.HVAC)]
	[TestCase(Constants.Auxiliaries.Names.PneumaticSystem, AuxiliaryType.PneumaticSystem)]
	[TestCase(Constants.Auxiliaries.Names.ElectricSystem, AuxiliaryType.ElectricSystem)]
	public void TestToString(string expectedStr, AuxiliaryType auxType)
	{
		Assert.AreEqual(expectedStr, AuxiliaryTypeHelper.ToString(auxType));
	}
}
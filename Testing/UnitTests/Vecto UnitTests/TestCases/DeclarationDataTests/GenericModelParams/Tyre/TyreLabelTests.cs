using NUnit.Framework;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Tyre;

public class TyreLabelTests
{
	[
		TestCase(0.0030, "A"),
		TestCase(0.0040, "A"),
		TestCase(0.0050, "B"),
		TestCase(0.0060, "C"),
		TestCase(0.0070, "D"),
		TestCase(0.0080, "E"),

		TestCase(0.0041, "B"),
		TestCase(0.0051, "C"),
		TestCase(0.0061, "D"),
		TestCase(0.0071, "E"),
		TestCase(0.0081, "F"),

		TestCase(0.00402, "A"),
		TestCase(0.004049, "A"),
		TestCase(0.00405, "B"),
		TestCase(0.00407, "B"),

		TestCase(0.00502, "B"),
		TestCase(0.005049, "B"),
		TestCase(0.00505, "C"),
		TestCase(0.00507, "C"),
	]
	public void TestTyreLabelLookup(double rrc, string expectedClass)
	{
		var tyreClass = DeclarationData.Wheels.TyreClass.Lookup(rrc);
		//Assert.IsTrue(expectedClass.Equals(tyreClass, StringComparison.InvariantCultureIgnoreCase));
		Assert.AreEqual(expectedClass, tyreClass);
	}
}
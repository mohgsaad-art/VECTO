using NUnit.Framework;
using TUGraz.VectoCommon.Models;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils;

public class AlternatorTypeHelperTests
{


	[TestCase("conventional", AlternatorType.Conventional)]
	[TestCase("no alternator", AlternatorType.None)]
	[TestCase("smart", AlternatorType.Smart)]
	public void ParseAlternatorTypeEnum(string xmlValue, AlternatorType expected)
	{
		var enumResult = AlternatorTypeHelper.Parse(xmlValue);
		Assert.AreEqual(expected, enumResult);
	}
}
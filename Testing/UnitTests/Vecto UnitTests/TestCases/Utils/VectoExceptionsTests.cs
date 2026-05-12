using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils;

public class VectoExceptionsTests
{
	[TestCase]
	public void Test_VectoExceptions()
	{
		new VectoException("Test", ErrorCodes.EXCEPTION_CANNOT_INFER_ERROR_TYPE, new Exception("Inner"));
		new VectoSimulationException("Test", ErrorCodes.EXCEPTION_CANNOT_INFER_ERROR_TYPE);
		new VectoSimulationException("Test", ErrorCodes.EXCEPTION_CANNOT_INFER_ERROR_TYPE, new Exception("Inner"));
	}
}
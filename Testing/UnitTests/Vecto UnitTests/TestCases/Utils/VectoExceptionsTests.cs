using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils;

public class VectoExceptionsTests
{
	[TestCase]
	public void Test_VectoExceptions()
	{
		new CSVReadException("Test");
		new CSVReadException("Test", new Exception("Inner"));
		new InvalidFileFormatException("Test");
		new CSVReadException("Test", new Exception("Inner"));
		new UnsupportedFileVersionException("Test");
		new UnsupportedFileVersionException("Test", new Exception("Inner"));
		new InvalidFileFormatException("Test");
		new InvalidFileFormatException("Test", new Exception("Inner"));
		new VectoException("Test");
		new VectoException("Test", new Exception("Inner"));

		new VectoSimulationException("Test");
		new VectoSimulationException("Test", new Exception("Inner"));
	}
}
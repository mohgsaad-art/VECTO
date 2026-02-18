using System.Runtime.CompilerServices;

namespace TUGraz.Vecto.UnitTests.Utils;

internal static class ProjectSource
{
	private static string CallerFilePath([CallerFilePath] string? callerFilePath = null) =>
		callerFilePath ?? throw new ArgumentNullException(nameof(callerFilePath));

	public static string ProjectDirectory() => Path.Combine(Path.GetDirectoryName(CallerFilePath())!, "..", "..", "..", "..");
}
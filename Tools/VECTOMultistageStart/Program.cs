using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main()
		{
			var version = GetHighestNETVersion();
			Process.Start(new ProcessStartInfo($"{version}\\{Assembly.GetExecutingAssembly().GetName().Name}.exe") {
				WorkingDirectory = Directory.GetCurrentDirectory()
			});
		}

		private static string GetHighestNETVersion()
		{
			if (SupportsNet60()) {
				return "net60";
			}

			return "net48";
		}

		private static bool SupportsNet60()
		{
			try {
				var p = Process.Start(new ProcessStartInfo("dotnet", "--list-runtimes") {
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				});

				p.WaitForExit();
				var output = p.StandardOutput.ReadToEnd();
				return output.Contains("Microsoft.WindowsDesktop.App 6");
			} catch (Exception e) {
				Console.WriteLine(e);
			}

			return false;
		}
	}
}

using System;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.IO;

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

			if (SupportsNet48()) {
				return "net48";
			}

			return "net45";
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

		private static bool SupportsNet48()
		{
			const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
			using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)) {
				if (ndpKey != null && ndpKey.GetValue("Release") != null) {
					var releaseKey = (int)ndpKey.GetValue("Release");
					return releaseKey >= 528040;
				}

				return false;
			}
		}
	}
}

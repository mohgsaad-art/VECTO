using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace VECTOStart
{
	public class StarterHelper
	{
		public const string NET48 = "net48";
		public const string NET60 = "net60";
		public const string NET80 = "net80";
		public const string NET100 = "net100";
		/// <summary>
		/// Supported .NET versions of the StartHelper
		/// </summary>
		private readonly HashSet<string> _supportedVersions = new HashSet<string>() {
			NET48,NET60,NET80,NET100
		};

		/// <summary>
		/// Supported versions of the program that should be executed
		/// </summary>
		private readonly IList<string> _targetVersions;

		private readonly Assembly _entryAssembly;
		private readonly string _targetProgramName;

		private bool IsConsole { get; set; }
		private bool IsGui => !IsConsole;
		/// <summary>
		///
		/// </summary>
		/// <param name="isConsoleApp">true for console apps</param>
		/// <param name="targetVersions">A list of supported .NET versions of the target application</param>
		/// <exception cref="Exception"></exception>
		public StarterHelper(bool isConsoleApp, params string[] targetVersions)
		{
			_targetVersions = targetVersions;
			var unsupported = targetVersions.Where(v => !_supportedVersions.Contains(v)).ToList();
			if (unsupported.Any()) {
				throw new Exception("Unsupported versions: " + string.Join(", ", unsupported));
			}
			IsConsole = isConsoleApp;

			_entryAssembly = Assembly.GetEntryAssembly();
			_targetProgramName = _entryAssembly.GetName().Name;
		}

        public void Start(string[] cmdArguments)
		{
			var path = "No path found.";
			string version = "No version found.";


			//Get the highest .NET version on this computer
			try {
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {version}\n");
                version = GetNetVersion(desktop: IsGui);
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {version}\n");
                path = $"{version}\\{_targetProgramName}.exe";


				string argumentsString = "";
				if (cmdArguments.Length > 0) {
					foreach (var cmdArgument in cmdArguments) {
						argumentsString += "\"" +  cmdArgument + "\" ";
					}
				}

				var processInfo = new ProcessStartInfo(path) {
					WorkingDirectory = Directory.GetCurrentDirectory(),
					Arguments = argumentsString,
				};


				if (IsConsole) {
					processInfo.UseShellExecute = false;
                    Process.Start(processInfo)?.WaitForExit();
                } else {
					Process.Start(processInfo);
				}

				ValidateVersion(version);
			} catch (Exception e) {
				var message = $"Error during starting {_targetProgramName}.\nDetected .NET version: {version}\nTried to open path: {path}\n{e.Message}";
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {message}\n");
				Console.WriteLine(message);
				throw new Exception(message);
			}
		}

		private void ValidateVersion(string version)
		{
			if (!_targetVersions.Contains(version))
			{
				throw new Exception($"Invalid .NET Version supplied. Only the following values are valid: {string.Join(", ", _targetVersions)}");
			}
		}

		private string GetNetVersion(bool desktop)
		{
			if (_targetVersions.Contains(NET100)
					&& SupportsNet100(desktop)) {
				return NET100;
			}

			if (_targetVersions.Contains(NET80)
					&& SupportsNet80(desktop)) {
				return NET80;
			}

			if (_targetVersions.Contains(NET60)
					&& SupportsNet60(desktop)) {
				return NET60;
			}

			//Should be installed by default on windows 10 and 11 machines
			return NET48;
		}

		private bool SupportsNet100(bool desktop)
		{
			try {
				var output = GetDotnetRuntimes();
				return output.Contains(desktop
					? "Microsoft.WindowsDesktop.App 10"
					: "Microsoft.NETCore.App 10");
			} catch (Exception e) {
				Console.WriteLine(e);
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {e.Message}\n");
			}

			return false;
		}

		private bool SupportsNet80(bool desktop)
		{
			try {
				var output = GetDotnetRuntimes();
				return output.Contains(desktop
					? "Microsoft.WindowsDesktop.App 8"
					: "Microsoft.NETCore.App 8");
			} catch (Exception e) {
				Console.WriteLine(e);
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {e.Message}\n");
			}

			return false;
		}

		private bool SupportsNet60(bool desktop)
		{
			try {
				var output = GetDotnetRuntimes();
				return output.Contains(desktop
					? "Microsoft.WindowsDesktop.App 6"
					: "Microsoft.NETCore.App 6");
			} catch (Exception e) {
				Console.WriteLine(e);
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {e.Message}\n");
			}

			return false;
		}

		private static string GetDotnetRuntimes()
		{
			var p = Process.Start(new ProcessStartInfo("dotnet", "--list-runtimes") {
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			});

			p.WaitForExit();
			var output = p.StandardOutput.ReadToEnd();
			return output;
		}


		private bool SupportsNet48()
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
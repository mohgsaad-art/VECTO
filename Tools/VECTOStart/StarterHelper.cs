using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace VECTOStart
{
	public class StarterHelper
	{
		public const string NET48 = "net48";
		public const string NET80 = "net80";
		/// <summary>
		/// Supported .NET versions of the StartHelper
		/// </summary>
		private readonly HashSet<string> _supportedVersions = new HashSet<string>()
		{
			NET48,
			NET80
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
			if (_targetVersions.Contains(NET80) && SupportsNet80(desktop))
			{
				return NET80;
			}
			
			//Should be installed by default on windows 10 and 11 machines
			return NET48;
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
	}
}
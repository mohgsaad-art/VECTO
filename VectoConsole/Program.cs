/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NLog;
using NLog.Config;
using NLog.Targets;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using LogManager = NLog.LogManager;

namespace VectoConsole
{
	internal static class Program
	{
		public static List<string> WarningMessages = new List<string>();

		private static int _numLines;
		private static int ProgessCounter { get; set; }

		private const string Usage = @"Usage: vectocmd.exe [-h] [-v] FILE1.vecto [FILE2.vecto ...]";

		private const string Help = @"
Commandline Interface for Vecto.

Synopsis:
	vectocmd.exe [-h] [-v] FILE1.(vecto|xml) [FILE2.(vecto|xml) ...]

Description:
	FILE1.vecto [FILE2.vecto ...]: A list of vecto-job files (with the 
	   extension: .vecto). At least one file must be given. Delimited by 
	   whitespace.

	-t: output information about execution times
	-mod: write mod-data in addition to sum-data
	-1Hz: convert mod-data to 1Hz resolution
	-eng: switch to engineering mode (implies -mod)
	-q: quiet - disables console output unless verbose information is enabled
	-nv: skip validation of internal data structure before simulation
	-v: Shows verbose information (errors and warnings will be displayed)
	-vv: Shows more verbose information (infos will be displayed)
	-vvv: Shows debug messages (slow!)
	-vvvv: Shows all verbose information (everything, slow!)
	-V: show version information
	-h: Displays this help.
	
Examples:
	vecto.exe ""12t Delivery Truck.vecto"" 40t_Long_Haul_Truck.vecto
	vecto.exe 24tCoach.vecto 40t_Long_Haul_Truck.vecto
	vecto.exe -v 24tCoach.vecto
	vecto.exe -v jobs\40t_Long_Haul_Truck.vecto
	vecto.exe -h
";

		private static JobContainer _jobContainer;
		private static bool _quiet;
		private static bool _debugEnabled;
		private static IKernel _kernel;

		private static int Main(string[] args)
		{

			_kernel = new StandardKernel(new VectoNinjectModule());
			try {
				// on -h display help and terminate.
				if (args.Contains("-h")) {
					ShowVersionInformation();
					Console.Write(Help);
					return 0;
				}

				if (args.Contains("-q")) {
					_quiet = true;
				}

				// on -v: activate verbose console logger
				var logLevel = LogLevel.Fatal;

				// Fatal > Error > Warn > Info > Debug > Trace

				if (args.Contains("-v")) {
					// display errors, warnings
					logLevel = LogLevel.Warn;
					_debugEnabled = true;
				} else if (args.Contains("-vv")) {
					// also display info
					logLevel = LogLevel.Info;
					_debugEnabled = true;
				} else if (args.Contains("-vvv")) {
					// display debug messages
					logLevel = LogLevel.Debug;
					_debugEnabled = true;
				} else if (args.Contains("-vvvv")) {
					// display everything!
					logLevel = LogLevel.Trace;
					_debugEnabled = true;
				}

				var config = LogManager.Configuration;
				if (config is null) {
					// in .net6.0 the app is exported as dll, therefore the default config lookup of nlog doesn't work.
					LogManager.LoadConfiguration($"{Assembly.GetExecutingAssembly().GetName().Name}.dll.config");
					config = LogManager.Configuration;
				}

				config.LoggingRules.Add(new LoggingRule("*", logLevel, config.FindTargetByName("LogFile")));

				if (logLevel > LogLevel.Warn) {
					var methodCallTarget = new MethodCallTarget {
						ClassName = "VectoConsole.Program, vectocmd",
						MethodName = "LogWarning",
						Name = "WarningLogger"
					};
					methodCallTarget.Parameters.Add(new MethodCallParameter("${level}"));
					methodCallTarget.Parameters.Add(new MethodCallParameter("${message}"));
					config.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, methodCallTarget));
				}
				LogManager.Configuration = config;

				if (args.Contains("-V") || _debugEnabled) {
					ShowVersionInformation();
				}

				var fileList =
					args.Except(new[] { "-v", "-vv", "-vvv", "-vvvv", "-V", "-nv", "-mod", "-eng", "-t", "-1Hz", "-q", "-act" })
						.ToArray();
				var jobFiles =
					fileList.Where(
						f =>
							Path.GetExtension(f) == Constants.FileExtensions.VectoJobFile ||
							Path.GetExtension(f) == Constants.FileExtensions.VectoXMLDeclarationFile).ToList();

				// if no other arguments given: display usage and terminate
				if (!args.Any()) {
					Console.Write(Usage);
					return 1;
				}

				var stopWatch = new Stopwatch();
				var timings = new Dictionary<string, double>();

				// process the file list and start simulation
				var fileWriter = new FileOutputWriter(fileList.First());
				var sumWriter = new SummaryDataContainer(fileWriter);
				_jobContainer = new JobContainer(sumWriter);

				var mode = ExecutionMode.Declaration;
				if (args.Contains("-eng")) {
					mode = ExecutionMode.Engineering;
					WriteLine(@"Switching to Engineering Mode. Make sure the job-file is saved in engineering mode!",
						ConsoleColor.White);
				}

				stopWatch.Start();

				if (!jobFiles.Any()) {
					WriteLine(@"No Job files found. Please restart the application with a valid '.vecto' file.", ConsoleColor.Red);
					return 1;
				}

				var inputReader = _kernel.Get<IXMLInputDataReader>();

				foreach (var file in jobFiles) {
					WriteLine(@"Reading job: " + file);
					var extension = Path.GetExtension(file);
					IInputDataProvider dataProvider = null;
					switch (extension) {
						case Constants.FileExtensions.VectoJobFile:
							dataProvider = JSONInputDataFactory.ReadJsonJob(file);
							break;
						case ".xml":
							var xDocument = XDocument.Load(file);
							var rootNode = xDocument?.Root.Name.LocalName ?? "";
							switch (rootNode) {
								case "VectoInputEngineering":
									dataProvider = inputReader.CreateEngineering(file);
									break;
								case "VectoInputDeclaration":
									dataProvider = inputReader.CreateDeclaration(XmlReader.Create(file));
									break;
							}
							break;
					}

					if (dataProvider == null) {
						WriteLine($@"failed to read job: '{file}'");
						continue;
					}

					fileWriter = new FileOutputWriter(file);
					var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(mode, dataProvider, fileWriter);
					//var runsFactory = SimulatorFactory.CreateSimulatorFactory(mode, dataProvider, fileWriter);
					runsFactory.ModalResults1Hz = args.Contains("-1Hz");
					runsFactory.WriteModalResults = args.Contains("-mod");
					runsFactory.ActualModalData = args.Contains("-act");
					runsFactory.Validate = !args.Contains("-nv");

					_jobContainer.AddRuns(runsFactory);
				}

				WriteLine();
				WriteLine(@"Detected cycles:", ConsoleColor.White);

				foreach (var cycle in _jobContainer.GetCycleTypes()) {
					WriteLineStdOut($@"  {cycle.Name}: {cycle.CycleType}");
				}
				WriteLine();

				stopWatch.Stop();
				timings.Add("Reading input files", stopWatch.Elapsed.TotalMilliseconds);
				stopWatch.Reset();

				WriteLine(@"Starting simulation runs", ConsoleColor.White);
				if (_debugEnabled) {
					WriteLine(@"Debug-Output is enabled, executing simulation runs sequentially", ConsoleColor.Yellow);
				}
				WriteLine();

				DisplayWarnings();
				WriteLine();
				stopWatch.Start();
				_jobContainer.Execute(!_debugEnabled);

				Console.CancelKeyPress += (sender, e) => {
					if (e.SpecialKey == ConsoleSpecialKey.ControlC) {
						e.Cancel = true;
						_jobContainer.CancelCurrent();
					}
				};

				while (!_jobContainer.AllCompleted) {
					PrintProgress(_jobContainer.GetProgress());
					Thread.Sleep(100);
				}
				stopWatch.Stop();
				timings.Add("Simulation runs", stopWatch.Elapsed.TotalMilliseconds);

				PrintProgress(_jobContainer.GetProgress(), args.Contains("-t"), force: true);

				if (args.Contains("-t")) {
					PrintTimings(timings);
				}

				DisplayWarnings();
			} catch (Exception e) {
				if (!_quiet) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Error.WriteLine(e.Message);
					Console.ResetColor();

					Console.Error.WriteLine("Please see log-file for further details (logs/log.txt)");
				}
				Environment.ExitCode = Environment.ExitCode != 0 ? Environment.ExitCode : 1;
			}

#if DEBUG
			Console.Error.WriteLine("done.");

			if (!Console.IsInputRedirected) {
				Console.ReadKey();
			}
#endif
			return Environment.ExitCode;
		}

		private static void WriteLine()
		{
			if (_quiet && !_debugEnabled) {
				return;
			}
			Console.Error.WriteLine();
		}

		private static void WriteLine(string message, ConsoleColor foregroundColor = ConsoleColor.Gray)
		{
			if (_quiet && !_debugEnabled) {
				return;
			}
			Console.ForegroundColor = foregroundColor;
			Console.Error.WriteLine(message);
			Console.ResetColor();
		}

		private static void WriteLineStdOut(string message, ConsoleColor foregroundColor = ConsoleColor.Gray)
		{
			if (_quiet && !_debugEnabled) {
				return;
			}
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		private static void DisplayWarnings()
		{
			if (_quiet) {
				return;
			}

			if (WarningMessages.Any()) {
				Console.ForegroundColor = ConsoleColor.Yellow;
				foreach (var message in WarningMessages) {
					Console.Error.WriteLine(message);
				}
				Console.ResetColor();
			}
			WarningMessages.Clear();
		}

		public static void LogWarning(string level, string message)
		{
			if (level == "Warn") {
				WarningMessages.Add(message);
			}
		}

		private static void ShowVersionInformation()
		{
			WriteLine($@"VectoConsole: {Assembly.GetExecutingAssembly().GetName().Version}");
			WriteLine($@"VectoCore: {VectoSimulationCore.VersionNumber}");
		}

		private static void PrintProgress(IDictionary<int, JobContainer.ProgressEntry> progessData,
			bool showTiming = true, bool force = false)
		{
			try {
				if (_quiet || (Console.IsOutputRedirected && !force)) {
					return;
				}

				if (!Console.IsOutputRedirected) {
					Console.SetCursorPosition(0, Console.CursorTop - _numLines);
				}

				_numLines = 0;
				var sumProgress = 0.0;
				foreach (var progressEntry in progessData) {
					if (progressEntry.Value.Success) {
						Console.ForegroundColor = ConsoleColor.Green;
					} else if (progressEntry.Value.Error != null) {
						Console.ForegroundColor = ConsoleColor.Red;
					}
					var timingString = "";
					if (showTiming && progressEntry.Value.ExecTime > 0) {
						timingString = $"{progressEntry.Value.ExecTime / 1000.0,9:F2}s";
					}
					var runName = $"{progressEntry.Value.RunName} {progressEntry.Value.CycleName} {progressEntry.Value.RunSuffix}";
					Console.WriteLine(@"{0,-60} {1,8:P}{2}", runName, progressEntry.Value.Progress, timingString);
					Console.ResetColor();
					sumProgress += progressEntry.Value.Progress;
					_numLines++;
				}
				sumProgress /= _numLines;
				var spinner = "/-\\|"[ProgessCounter++ % 4];
				var bar = new string('#', (int)(sumProgress * 100.0 / 2));
				Console.WriteLine(@"   {2}   [{1,-50}]  [{0,7:P}]", sumProgress, bar, spinner);

				if (WarningMessages.Any()) {
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Error.WriteLine(@"Warnings: {0,5}", WarningMessages.Count);
					Console.ResetColor();
				} else {
					Console.WriteLine("");
				}

				_numLines += 2;
			} catch (Exception e) {
				throw new VectoException("Error during writing progress to output: " + e.Message);
			}
		}

		private static void PrintTimings(Dictionary<string, double> timings)
		{
			Console.Error.WriteLine();
			Console.Error.WriteLine(@"---- timing information ----");
			foreach (var timing in timings) {
				Console.Error.WriteLine(@"{0,-20}: {1:F2}s", timing.Key, timing.Value / 1000);
			}
		}
	}
}

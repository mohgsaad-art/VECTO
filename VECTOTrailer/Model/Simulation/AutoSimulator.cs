using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using ControlzEx.Standard;
using NLog;
using NLog.Config;
using NLog.Targets;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using VECTOTrailer.Helper;
using VECTOTrailer.ViewModel.Impl;
using LogManager = NLog.LogManager;

namespace VECTOTrailer.Model.Simulation
{
	public interface IBackGroundSimulator
	{
		string InputDirectory { get; set; }
		string OutputDirectory { get; set; }

		ObservableCollection<MessageEntry> Messages { get; }
		bool WriteModData { get; set; }
		//bool WriteModData1Hz { get; set; }
		//bool ValidateData { get; set; }
		//bool WriteActualModData { get; set; }
		//bool WriteModelData { get; set; }
		bool AutoSimBusy { get; }
		bool AutoSimActive { get; }

		bool WritePdfReports { get; set; }


		void ToggleSimulationBackgroundWorker();

	}
    public class BackGroundSimulator : ObservableObject, IBackGroundSimulator
	{
		private ILogger _logger;
		private BackgroundWorker worker;
		private ReaderWriterLockSlim _simSettingsRwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		public BackGroundSimulator(IXMLInputDataReader inputDataReader, IPdfCreator pdfCreator, ITrailerToolSettings settings)
		{
			_pdfCreator = pdfCreator;
			_inputDataReader = inputDataReader;
			worker = new BackgroundWorker() {
				WorkerSupportsCancellation = true,
				WorkerReportsProgress = true
			};

			ConfigureNlog();
			worker.DoWork += DoWork;
			worker.RunWorkerCompleted += VectoSimulationCompleted;
			worker.ProgressChanged += WorkerOnProgressChanged;

			_trailerToolSettings = settings;


			InputDirectory = _trailerToolSettings.GeneralSettings.BackgroundInputPath;
			OutputDirectory = _trailerToolSettings.GeneralSettings.BackgroundOutputPath;
			WritePdfReports = _trailerToolSettings.SimulationSettings.BackgroudWritePDF;
			WriteModData = _trailerToolSettings.SimulationSettings.BackgroundWriteModData;
			settings.PropertyChanged += ((s, e) => {
				var trailerToolSettings = s as TrailerToolSettings;
				var generalSettings = trailerToolSettings?.GeneralSettings;
				var simulationSettings = trailerToolSettings?.SimulationSettings;
				switch (e.PropertyName) {
					case nameof(generalSettings.BackgroundInputPath):
						InputDirectory = generalSettings?.BackgroundInputPath ?? InputDirectory;
						break;
					case nameof(generalSettings.BackgroundOutputPath):
						OutputDirectory = generalSettings?.BackgroundOutputPath ?? OutputDirectory;
						break;
					case nameof(simulationSettings.BackgroundWriteModData):
						WriteModData = simulationSettings?.BackgroundWriteModData ?? WriteModData;
						break;
					case nameof(simulationSettings.BackgroudWritePDF):
						WritePdfReports = simulationSettings?.BackgroudWritePDF ?? WritePdfReports;
						break;
					default:
						break;
				}
			});
		}

		private ObservableCollection<MessageEntry> _messages = new ObservableCollection<MessageEntry>();
		public ObservableCollection<MessageEntry> Messages => _messages;


		#region Logging

		private void ConfigureNlog()
		{
			var target = new FileTarget(nameof(BackGroundSimulator)) {
				FileName = LogFileHelper.GetFileNameWithBasePath("BackgroundWorker.log"),
				Layout = "${longdate} ${level} ${message}  ${exception} ${event-properties:myProperty}"
			};
			var loggingRule = new LoggingRule($"{nameof(BackGroundSimulator)}", LogLevel.Trace, target);
			NLog.LogManager.Configuration.AddTarget(target); 
			NLog.LogManager.Configuration.LoggingRules.Add(loggingRule);
			NLog.LogManager.Configuration.Reload();
			NLog.LogManager.ReconfigExistingLoggers();
			_logger = NLog.LogManager.GetLogger(nameof(BackGroundSimulator));
		}
		#endregion	

		#region SimulationSettings


		private bool _writeModData;
		public bool WriteModData
		{
			get
			{
				try {
					_simSettingsRwLock.EnterReadLock();
					return _writeModData;
				} finally {
					_simSettingsRwLock.ExitReadLock();
				}
			
			}
			set
			{
				try {
					_simSettingsRwLock.EnterWriteLock();
					if (SetProperty(ref _writeModData, value)) {
						_trailerToolSettings.SimulationSettings.WriteModData = value;
					}
				} finally {
					_simSettingsRwLock.ExitWriteLock();
				}
			
			}
		}

		//private bool _writeModData1Hz;

		//public bool WriteModData1Hz
		//{
		//	get
		//	{
		//		try {
		//			_simSettingsRwLock.EnterReadLock();
		//			return _writeModData1Hz;
		//		} finally {
		//			_simSettingsRwLock.ExitReadLock();
		//		}
				
		//	}
		//	set
		//	{
		//		try {
		//			_simSettingsRwLock.EnterWriteLock();
		//			SetProperty(ref _writeModData1Hz, value);
		//		} finally {
		//			_simSettingsRwLock.ExitWriteLock();
		//		}
		//	}
		//}

		//public bool ValidateData { get; set; }
		//public bool WriteActualModData { get; set; }
		//public bool WriteModelData { get; set; }

		private bool _writePdfReports;
		private readonly IPdfCreator _pdfCreator;

		public bool WritePdfReports
		{
			get
			{
				try
				{
					_simSettingsRwLock.EnterReadLock();
					return _writePdfReports && _pdfCreator.Initialized;
				}
				finally
				{
					_simSettingsRwLock.ExitReadLock();
				}
				
			}
			set
			{
				try {
					_simSettingsRwLock.EnterWriteLock();
					if (SetProperty(ref _writePdfReports, value)) {
						_trailerToolSettings.SimulationSettings.BackgroudWritePDF = value;
					}
				} finally {
					_simSettingsRwLock.ExitWriteLock();
				}
				
			}
		}

		public bool AutoSimBusy
		{
			get => _autoSimBusy;
			private set => SetProperty(ref _autoSimBusy, value);
		}

		#endregion

		private object dirLock = new object();

		#region Implementation of IAutoSimulator

		private string _inputDirectory;
		public string InputDirectory
		{
			get
			{
				lock (dirLock) {
					return _inputDirectory;
				}
			}
			set
			{
				lock (dirLock) {
					if (SetProperty(ref _inputDirectory, value)) {
						_trailerToolSettings.GeneralSettings.BackgroundInputPath = value;
					}
				}
			}
		}

		private string _outputDirectory;
		private readonly IXMLInputDataReader _inputDataReader;
		private bool _autoSimBusy;
		private bool _autoSimActive;
		private readonly ITrailerToolSettings _trailerToolSettings;


		public string OutputDirectory
		{
			get
			{
				lock (dirLock) {
					return _outputDirectory;
				}
				
			}
			set
			{
				lock (dirLock) {
					if (SetProperty(ref _outputDirectory, value)) {
						_trailerToolSettings.GeneralSettings.BackgroundOutputPath = value;
					};
				}
				
			}
		}

		public bool AutoSimActive
		{
			get
			{
				return _autoSimActive;
			}
			private set
			{
				SetProperty(ref _autoSimActive, value);
			}
		}


		public void ToggleSimulationBackgroundWorker()
		{
			if (AutoSimActive) {
				StopAutoSimulator();
			} else {
				StartAutoSimulator();
			}	
			OnPropertyChanged(nameof(AutoSimActive)); //<- DO not remove important to sync the state of the toggle button
		}

		private bool StartAutoSimulator()
		{
			if (!worker.IsBusy) {
				worker.RunWorkerAsync();
				var msg = string.Format("Starting Background Worker \n Input directory: {0} \n Output directory{1} \n",
					InputDirectory, OutputDirectory);
				_logger.Log(LogLevel.Info, msg);
				Messages.Add(new MessageEntry()
				{
					Message = msg,
				});
				return true;
			}

			return false;
		}

		private bool StopAutoSimulator()
		{
			if (worker.WorkerSupportsCancellation) {
				worker.CancelAsync();
				var msg = "Stopping Background Worker\n";
				_logger.Log(LogLevel.Info, msg);
				Messages.Add(new MessageEntry() {
					Message = msg,
				});
				return true;
			}

			return false;
		}


		#endregion

		private void DoWork(object theSender, DoWorkEventArgs e)
		{
			var backgroundWorker = theSender as BackgroundWorker;
			if (backgroundWorker == null) {
				throw new ArgumentException();
			}

		

			AutoSimActive = true;
			
			while (!backgroundWorker.CancellationPending) {
				if (!CheckDirectories(progress => backgroundWorker.ReportProgress(0, progress))) {
					break;
				}

				AutoSimBusy = true;
				string outputDirectory = null;
				IList<string> filesToSimulate = null;
				lock (dirLock) {
					filesToSimulate = CopyFiles(progressAction: (progress => backgroundWorker.ReportProgress(0, progress)));
					outputDirectory = _outputDirectory;
				}

				if (filesToSimulate.Count > 0) {
					RunSimulation(filesToSimulate, outputDirectory, backgroundWorker.ReportProgress, () => backgroundWorker.CancellationPending);
				} else {
					Thread.Sleep(2000);
					AutoSimBusy = false;
					Thread.Sleep(2000);
				}
			}

			AutoSimBusy = false;
		}

		private bool CheckDirectories(Action<VectoSimulationProgress> errorAction)
		{
			var errorMsgs = new List<string>();
			var directoriesValid = true;
			lock (dirLock) {

				if (InputDirectory.IsNullOrEmpty()) {
					directoriesValid = false;
					errorMsgs.Add("Input directory not set");
				}else if (!Directory.Exists(_inputDirectory)) {
					directoriesValid = false;
					errorMsgs.Add($"Input directory {_inputDirectory} doesn't exist");
				}

				if (OutputDirectory.IsNullOrEmpty()) {
					directoriesValid = false;
					errorMsgs.Add("Output directory not set");
				}
			}

			if (!directoriesValid) {
				errorAction.Invoke(new VectoSimulationProgress()
				{
					Target = VectoSimulationProgress.MsgTarget.MessageWindow | VectoSimulationProgress.MsgTarget.NLog,
					Message = String.Join("\n", errorMsgs),
					Type = VectoSimulationProgress.MsgType.LogError,
				});
			}



			return directoriesValid;
		}
		private IList<string> CopyFiles(Action<VectoSimulationProgress> progressAction)
		{
			var copiedFiles = new List<string>();
	
			foreach (var file in Directory.GetFiles(_inputDirectory))
			{
				progressAction.Invoke(new VectoSimulationProgress()
				{
					Message = string.Format("[Background Worker]: Copy {0} \n to {1} \n", file, OutputDirectory),
					Type = VectoSimulationProgress.MsgType.InfoMessage,
					Target = VectoSimulationProgress.MsgTarget.MessageWindow | VectoSimulationProgress.MsgTarget.NLog
				});

				if (CopyTo(file, _outputDirectory, out var destFile, true, progressAction))
				{
					copiedFiles.Add(destFile);
				}
			}
				

			return copiedFiles;
		}

		private bool CopyTo(string source, string destDirectory, out string destFileName, bool overwrite, Action<VectoSimulationProgress> errorAction)
		{
			destFileName = Path.Combine(destDirectory, Path.GetFileName(source));
			if (!Directory.Exists(destDirectory)) {
				errorAction.Invoke(new VectoSimulationProgress() {
					Type = VectoSimulationProgress.MsgType.InfoMessage,
					Message = $"Created directory {destDirectory}",
					Target = VectoSimulationProgress.MsgTarget.MessageWindow | VectoSimulationProgress.MsgTarget.NLog,
				});
				Directory.CreateDirectory(destDirectory);
			}
	
			try {
				//TODO: Can be replaced in .NET5 with a single call to move;
				File.Copy(source, destFileName, overwrite);
				File.Delete(source);
			} catch (Exception ex) {
				errorAction.Invoke(new VectoSimulationProgress() {
					Type = VectoSimulationProgress.MsgType.LogError,
					Message = "Error while copying files: " + ex.Message,
					Target = VectoSimulationProgress.MsgTarget.MessageWindow | VectoSimulationProgress.MsgTarget.NLog
				});
				return false;
			}
			return true;
		}

		private void RunSimulation(IList<string> jobFileNames,
			string outputDirectory, 
			Action<int, VectoSimulationProgress> reportProgress, 
			Func<bool> cancel)
		{
			var finishedRuns = new List<int>();
			var jobContainer = ReadJobFiles(jobFileNames, outputDirectory, reportProgress, cancel);
			if (cancel()) {
				return;
			}
			var start = Stopwatch.StartNew();

			jobContainer.Execute(true);

			while (!jobContainer.AllCompleted)
			{
				if (cancel())
				{
					jobContainer.Cancel();
					return;
				}

				var progress = jobContainer.GetProgress();
				var sumProgress = progress.Sum(x => x.Value.Progress);
				var duration = start.Elapsed.TotalSeconds;

				reportProgress(
					Convert.ToInt32(sumProgress * 100.0 / progress.Count), new VectoSimulationProgress()
					{
						Type = VectoSimulationProgress.MsgType.Progress,
						Message = string.Format(
							"Duration: {0:F1}s, Current Progress: {1:P} ({2})", duration, sumProgress / progress.Count,
							string.Join(", ", progress.Select(x => string.Format("{0,4:P}", x.Value.Progress)))),
						Target = VectoSimulationProgress.MsgTarget.NLog
					});
				var justFinished = progress.Where(x => x.Value.Done & !finishedRuns.Contains(x.Key))
					.ToDictionary(x => x.Key, x => x.Value);
				//PrintRuns(justFinished, fileWriters);
				finishedRuns.AddRange(justFinished.Select(x => x.Key));
				Thread.Sleep(100);
			}
			start.Stop();
			finishedRuns.Clear();

			HashSet<string> runNames = new HashSet<string>();
			foreach (var progressEntry in jobContainer.GetProgress())
			{
				reportProgress(100, new VectoSimulationProgress()
				{
					Type = VectoSimulationProgress.MsgType.StatusMessage,
					Message =
						string.Format("{0,-60} {1,8:P} {2,10:F2}s - {3}",
							$"{progressEntry.Value.RunName} {progressEntry.Value.CycleName} {progressEntry.Value.RunSuffix}",
							progressEntry.Value.Progress,
							progressEntry.Value.ExecTime / 1000.0,
							progressEntry.Value.Success ? "Success" : "Aborted"),
					Target = VectoSimulationProgress.MsgTarget.NLog
				});
				if (!progressEntry.Value.Success)
				{
					reportProgress(
						100,
						new VectoSimulationProgress()
						{
							Type = VectoSimulationProgress.MsgType.StatusMessage,
							Message = progressEntry.Value.Error.Message,
							Target = VectoSimulationProgress.MsgTarget.NLog | VectoSimulationProgress.MsgTarget.MessageWindow
						}
					);
				}

				//if (File.Exists(sumFileWriter.SumFileName))
				//{
				//	sender.ReportProgress(100, new VectoSimulationProgress()
				//	{
				//		Type = VectoSimulationProgress.MsgType.StatusMessage,
				//		Message = string.Format("Sum file written to {0}", sumFileWriter.SumFileName),
				//		Link = "<CSV>" + sumFileWriter.SumFileName
				//	});
				//}


				if (!runNames.Contains(progressEntry.Value.RunName)) {
					reportProgress(100, new VectoSimulationProgress()
					{
						Type = VectoSimulationProgress.MsgType.StatusMessage,
						Message = $"Background Simulation for {progressEntry.Value.RunName} {progressEntry.Value.CycleName} finished in {start.Elapsed.TotalSeconds}s",
						Target = VectoSimulationProgress.MsgTarget.NLog
					});
					runNames.Add(progressEntry.Value.RunName);
				}
			}

			foreach (var jobFileName in jobFileNames) {
				reportProgress(100, new VectoSimulationProgress()
				{
					Type = VectoSimulationProgress.MsgType.StatusMessage,
					Message = $"Background Simulation for {Path.GetFileNameWithoutExtension(jobFileName)} finished in {start.Elapsed.TotalSeconds}s",
					Target = VectoSimulationProgress.MsgTarget.MessageWindow
				});
			}

			reportProgress(100, new VectoSimulationProgress() {
				Type = VectoSimulationProgress.MsgType.StatusMessage,
				Message = string.Format("Background Simulation finished in {0:F1}s", start.Elapsed.TotalSeconds),
				Target = VectoSimulationProgress.MsgTarget.NLog | VectoSimulationProgress.MsgTarget.MessageWindow,
			});

			if(WritePdfReports) {
				foreach (var jobFileName in jobFileNames) {
					var jobPath = Path.Combine(outputDirectory, jobFileName);
					var w = new FileOutputWriter(jobPath);
					_pdfCreator.CreatePDFFromXml(w.XMLCustomerReportName, w.XMLCustomerReportName.Replace(".xml", ".pdf"));
					_pdfCreator.CreatePDFFromXml(w.XMLFullReportName, w.XMLFullReportName.Replace(".xml", ".pdf"));
				}
			}
		}

		private JobContainer ReadJobFiles(IList<string> jobFileNames, string outputDirectory, Action<int, VectoSimulationProgress> reportProgress, Func<bool> cancel)
		{
			var mode = ExecutionMode.Declaration;
			var sumFileWriter = new FileOutputWriter(jobFileNames.First());
			var sumContainer = new SummaryDataContainer(sumFileWriter);
			var jobContainer = new JobContainer(sumContainer);

			var fileWriters = new Dictionary<int, FileOutputWriter>();

			var xmlReader = _inputDataReader;

			foreach (var jobFileName in jobFileNames) {
				if (cancel()) {
					reportProgress(0, new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.StatusMessage,
						Message = "Simulation canceled",
						Target = VectoSimulationProgress.MsgTarget.MessageWindow | VectoSimulationProgress.MsgTarget.NLog,
					});
					return null;
				}

				try {
					var fullFileName = Path.GetFullPath(jobFileName);
					if (!File.Exists(fullFileName)) {
						reportProgress.Invoke(
							0, new VectoSimulationProgress() {
								Type = VectoSimulationProgress.MsgType.StatusMessage,
								Message =
									$"File {fullFileName} not found!",
								Target = VectoSimulationProgress.MsgTarget.NLog
							});
						continue;
					}

					reportProgress.Invoke(
						0,
						new VectoSimulationProgress() {
							Type = VectoSimulationProgress.MsgType.StatusMessage,
							Message = $"Reading file {Path.GetFileName(fullFileName)}",
							Target = VectoSimulationProgress.MsgTarget.NLog | VectoSimulationProgress.MsgTarget.MessageWindow
						});
					var extension = Path.GetExtension(fullFileName);
					IInputDataProvider input = null;
					switch (extension) {
						case Constants.FileExtensions.VectoJobFile:
							input = JSONInputDataFactory.ReadJsonJob(fullFileName);
							var tmp = input as IDeclarationTrailerInputDataProvider;
							mode = tmp?.JobInputData.SavedInDeclarationMode ?? false
								? ExecutionMode.Declaration
								: ExecutionMode.Engineering;
							break;
						case ".xml":
							var xdoc = XDocument.Load(fullFileName);
							var rootNode = xdoc.Root?.Name.LocalName ?? "";
							if (XMLNames.VectoInputEngineering.Equals(rootNode, StringComparison.InvariantCultureIgnoreCase)) {
								input = xmlReader.CreateEngineering(fullFileName);
								mode = ExecutionMode.Engineering;
							} else if (XMLNames.VectoInputDeclaration.Equals(rootNode,
								StringComparison.InvariantCultureIgnoreCase)) {
								using (var reader = XmlReader.Create(fullFileName)) {
									input = xmlReader.Create(reader);
								}

								mode = ExecutionMode.Declaration;
							}

							break;
					}

					if (input == null) {
						reportProgress(
							0,
							new VectoSimulationProgress() {
								Type = VectoSimulationProgress.MsgType.StatusMessage,
								Message = $"No input provider for job {Path.GetFileName(fullFileName)}"
							});
						continue;
					}

					var fileWriter = new FileOutputWriter(fullFileName);
					var runsFactory = new SimulatorFactory(mode, input, fileWriter) {
						WriteModalResults = WriteModData,
						ModalResults1Hz = false,//;WriteModData1Hz,
						Validate = false, //ValidateData,
						ActualModalData = false, //WriteActualModData,
						SerializeVectoRunData = false, // WriteModelData
					};
					foreach (var runId in jobContainer.AddRuns(runsFactory)) {
						fileWriters.Add(runId, fileWriter);
					}

					// TODO MQ-20200525: Remove the following loop in production (or after evaluation of LAC!!
					/*if (!string.IsNullOrWhiteSpace(LookAheadMinSpeedOverride)) {
						foreach (var run in jobContainer.Runs) {
							var tmpDriver = ((VectoRun)run.Run).GetContainer().RunData.DriverData;
							tmpDriver.LookAheadCoasting.Enabled = true;
							tmpDriver.LookAheadCoasting.MinSpeed = LookAheadMinSpeedOverride.ToDouble().KMPHtoMeterPerSecond();
						}
					}*/

					reportProgress(0, new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.StatusMessage,
						Message = $"Finished reading data for job {Path.GetFileName(fullFileName)}"
					});
				} catch (Exception ex) {
					reportProgress(
						0,
						new VectoSimulationProgress()
						{
							Type = VectoSimulationProgress.MsgType.LogError, Message = ex.Message,
							Target = VectoSimulationProgress.MsgTarget.MessageWindow | VectoSimulationProgress.MsgTarget.NLog,
						});
				}
			}

			return jobContainer;
		}
		private void VectoSimulationCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			AutoSimActive = false;
			AutoSimBusy = false;
		}

		private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			var progress = e.UserState as VectoSimulationProgress;
			if (progress == null) {
				return;
			}

			if (progress.Target.HasFlag(VectoSimulationProgress.MsgTarget.NLog)) {
				_logger.Log(progress.Type.ToLogLevel(), progress.Message);
			}

			if (progress.Target.HasFlag(VectoSimulationProgress.MsgTarget.MessageWindow)) {
				Messages.Add(new MessageEntry() {
					Message = progress.Message,
					Type = progress.Type.ToMessageType(),
				});
			}
			
		}


	}


}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using ControlzEx.Standard;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.Model.Simulation
{
	public interface IAutoSimulator
	{
		string InputDirectory { get; set; }
		string OutputDirectory { get; set; }


		bool WriteModData { get; set; }
		bool WriteModData1Hz { get; set; }
		bool ValidateData { get; set; }
		bool WriteActualModData { get; set; }
		bool WriteModelData { get; set; }
		bool AutoSimBusy { get; }
		bool AutoSimActive { get; }

		void ToggleAutoSimulator();

	}
    public class AutoSimulator : ObservableObject, IAutoSimulator
	{

		private BackgroundWorker worker;
		public AutoSimulator(IXMLInputDataReader inputDataReader)
		{
			_inputDataReader = inputDataReader;
			worker = new BackgroundWorker() {
				WorkerSupportsCancellation = true,
				WorkerReportsProgress = true
			};
			worker.DoWork += DoWork;
			worker.RunWorkerCompleted += VectoSimulationCompleted;
		}
		#region SimulationSettings

		public bool WriteModData { get; set; }
		public bool WriteModData1Hz { get; set; }
		public bool ValidateData { get; set; }
		public bool WriteActualModData { get; set; }
		public bool WriteModelData { get; set; }

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
					SetProperty(ref _inputDirectory, value);
				}
			}
		}

		private string _outputDirectory;
		private readonly IXMLInputDataReader _inputDataReader;
		private bool _autoSimBusy;
		private bool _autoSimActive;

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
					SetProperty(ref _outputDirectory, value);
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

		public void ToggleAutoSimulator()
		{
			if (AutoSimActive) {
				StopAutoSimulator();
			} else {
				StartAutoSimulator();
			}	
			OnPropertyChanged(nameof(AutoSimActive)); //<- DO not remove
		}

		private bool StartAutoSimulator()
		{
			if (!worker.IsBusy) {
				worker.RunWorkerAsync();
				return true;
			}

			return false;
		}

		private bool StopAutoSimulator()
		{
			if (worker.WorkerSupportsCancellation) {
				worker.CancelAsync();
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
				AutoSimBusy = true;
				string outputDirectory = null;
				IList<string> filesToSimulate = null;
				lock (dirLock) {
					filesToSimulate = CheckDirectoryAndCopyFiles(progressAction: (progress => backgroundWorker.ReportProgress(0, progress)));
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

		private IList<string> CheckDirectoryAndCopyFiles(Action<VectoSimulationProgress> progressAction)
		{
			var copiedFiles = new List<string>();
			lock (dirLock) {
				if (Directory.Exists(_inputDirectory) && _outputDirectory != null)
				{
					foreach (var file in Directory.GetFiles(_inputDirectory))
					{

						if (_outputDirectory == _inputDirectory)
						{
							throw new Exception("Input and output directory must not be the same directory");
						}
						if (CopyTo(file, _outputDirectory, out var destFile, true, progressAction))
						{
							copiedFiles.Add(destFile);
						}
					}
				}
				else
				{
					progressAction.Invoke(new VectoSimulationProgress()
					{
						Message = $"Directory {_inputDirectory} does not exist",
						Type = VectoSimulationProgress.MsgType.LogError
					});
				}

			}

			return copiedFiles;
		}

		private bool CopyTo(string source, string destDirectory, out string destFileName, bool overwrite, Action<VectoSimulationProgress> errorAction)
		{
			destFileName = Path.Combine(destDirectory, Path.GetFileName(source));
	
			try {
				//TODO: Can be replaced in .NET5 with a single call to move;
				File.Copy(source, destFileName, overwrite);
				File.Delete(source);
			} catch (Exception ex) {
				errorAction.Invoke(new VectoSimulationProgress() {
					Type = VectoSimulationProgress.MsgType.LogError,
					Message = "Error while copying files: " + ex.Message
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
							string.Join(", ", progress.Select(x => string.Format("{0,4:P}", x.Value.Progress))))
					});
				var justFinished = progress.Where(x => x.Value.Done & !finishedRuns.Contains(x.Key))
					.ToDictionary(x => x.Key, x => x.Value);
				//PrintRuns(justFinished, fileWriters);
				finishedRuns.AddRange(justFinished.Select(x => x.Key));
				Thread.Sleep(100);
			}
			start.Stop();
			finishedRuns.Clear();


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
							progressEntry.Value.Success ? "Success" : "Aborted")
				});
				if (!progressEntry.Value.Success)
				{
					reportProgress(
						100,
						new VectoSimulationProgress()
						{
							Type = VectoSimulationProgress.MsgType.StatusMessage,
							Message = progressEntry.Value.Error.Message
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

				reportProgress(100, new VectoSimulationProgress()
				{
					Type = VectoSimulationProgress.MsgType.StatusMessage,
					Message = string.Format("Simulation finished in {0:F1}s", start.Elapsed.TotalSeconds)
				});
			}
		}

		private JobContainer ReadJobFiles(IList<string> jobFileNames, string outputDirectory, Action<int, VectoSimulationProgress> reportProgress, Func<bool> cancel)
		{
			var mode = ExecutionMode.Declaration;
			var sumFileWriter = new FileOutputWriter(Path.Combine(outputDirectory, $"AutoSim"));
			var sumContainer = new SummaryDataContainer(sumFileWriter);
			var jobContainer = new JobContainer(sumContainer);

			var fileWriters = new Dictionary<int, FileOutputWriter>();

			var xmlReader = _inputDataReader;

			foreach (var jobFileName in jobFileNames) {
				if (cancel()) {
					reportProgress(0, new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.StatusMessage,
						Message = "Simulation canceled"
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
									$"File {fullFileName} not found!"
							});
						continue;
					}

					reportProgress.Invoke(
						0,
						new VectoSimulationProgress() {
							Type = VectoSimulationProgress.MsgType.StatusMessage,
							Message = $"Reading file {Path.GetFileName(fullFileName)}"
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
						ModalResults1Hz = WriteModData1Hz,
						Validate = ValidateData,
						ActualModalData = WriteActualModData,
						SerializeVectoRunData = WriteModelData
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
							Type = VectoSimulationProgress.MsgType.LogError, Message = ex.Message
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
	}


}

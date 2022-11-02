using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Annotations;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Implementation.Document;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using IDocumentViewModel = VECTO3GUI2020.ViewModel.Interfaces.Document.IDocumentViewModel;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VECTO3GUI2020.ViewModel.Implementation
{
    public class JobListViewModel : ViewModelBase, IJobListViewModel
    {
        #region Members and Properties
        private readonly Settings _settings = Settings.Default;
		private bool _simulationLoggingEnabled = true; //Enabled and Disable NLOG Messages



		private BackgroundWorker fileReadingBackgroundWorker;

		private object _jobsLock = new Object();
        private ObservableCollection<IDocumentViewModel> _jobs = new ObservableCollection<IDocumentViewModel>();
        public ObservableCollection<IDocumentViewModel> Jobs{ get => _jobs; set => SetProperty(ref _jobs, value);}

		public IDocumentViewModel SelectedJob
		{
			get => _selectedJob;
			set
			{
				if(SetProperty(ref _selectedJob, value)) {
					RemoveJob.NotifyCanExecuteChanged();
					_openSourceFileCommand?.NotifyCanExecuteChanged();
					_showSourceFileInExplorerCommand?.NotifyCanExecuteChanged();
				};
			}
		}



		private IDialogHelper _dialogHelper;
        private IWindowHelper _windowHelper;
        private IDocumentViewModelFactory _documentViewModelFactory;

		private IMultiStageViewModelFactory _multiStageViewModelFactory;
		private readonly IXMLInputDataReader _inputDataReader;
		private IOutputViewModel _outputViewModel;



		private readonly string StoredJobsFileName = "storedJobs.json";

		
        
		#endregion



        

        private JobListViewModel()
        {
			BindingOperations.EnableCollectionSynchronization(Jobs, _jobsLock);
		}


        public JobListViewModel(IDocumentViewModelFactory documentViewModelFactory,
            IXMLInputDataReader inputDataReader,
            IDialogHelper dialogHelper,
            IWindowHelper windowHelper,
			IMultiStageViewModelFactory multiStageViewModelFactory,
			ISimulatorFactoryFactory simulatorFactoryFactory,
			IOutputViewModel outputViewModel) : this()
        {
            _documentViewModelFactory = documentViewModelFactory;
            _dialogHelper = dialogHelper;
            _windowHelper = windowHelper;
			_inputDataReader = inputDataReader;
			_multiStageViewModelFactory = multiStageViewModelFactory;
			_outputViewModel = outputViewModel;
			_simFactoryFactory = simulatorFactoryFactory;

			_outputMessage = new Progress<MessageEntry>((message) => {
				_outputViewModel.AddMessage(message);
			});
			_progress = new Progress<int>((i) => {
				_outputViewModel.Progress = i;
			});
			_status = new Progress<string>((msg) => {
				_outputViewModel.StatusMessage = msg;
			});


			//configure Nlog
			var target = new MethodCallTarget("VectoGuiTarget", (evtInfo, obj) => LogMethod(evtInfo, obj));
			NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);


			if(System.Windows.Application.Current != null){
				System.Windows.Application.Current.Exit += new ExitEventHandler(this.OnApplicationExit);
				LoadFiles();
			}
		}


		private void LogMethod(LogEventInfo evtInfo, object[] objects)
		{
			if (!SimulationRunning || !_simulationLoggingEnabled)
			{
				return;
			}
			if (evtInfo.Level == LogLevel.Error || evtInfo.Level == LogLevel.Warn || evtInfo.Level == LogLevel.Fatal)
				_outputMessage.Report(new MessageEntry()
				{
					Type = evtInfo.Level == LogLevel.Warn ? MessageType.WarningMessage : MessageType.ErrorMessage,
					Message = evtInfo.FormattedMessage,
					Source = evtInfo.CallerMemberName,
				});
		}



		#region JobList
		#region Store and Restore JobList
		private void LoadFiles()
		{
			var filesToRead = ReadFileNamesFromFile();
			if (filesToRead != null)
			{
				foreach (var fileName in filesToRead)
				{
					Task.Run(() => AddJobAsync(fileName));
				}
			}
		}


		private void OnApplicationExit(object sender, EventArgs e)
		{
			SaveFileNamesToFile();
		}


		private string[] ReadFileNamesFromFile()
		{
			try {
				var filesJson = File.ReadAllText(StoredJobsFileName);
				IList<string> filesToRead = JsonConvert.DeserializeObject<List<string>>(filesJson);
				return filesToRead.ToArray();
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}

			return null;
		}

		private void SaveFileNamesToFile()
		{
			var filesToStore = Jobs.Where(job => job.DataSource?.SourceFile != null).Select(job => job.DataSource.SourceFile).ToList();
			string jsonString = JsonConvert.SerializeObject(filesToStore);
			Debug.WriteLine(jsonString);
			File.WriteAllText(StoredJobsFileName, jsonString);
		}
		#endregion

		public void AddJob(IDocumentViewModel jobToAdd)
		{
			lock (_jobsLock) {
				_jobs.Add(jobToAdd);
			}
		}


		public async Task<IDocumentViewModel> AddJobExecuteAsync()
		{
			var fileName = _dialogHelper.OpenXMLAndVectoFileDialog();
			if (fileName != null)
			{
				return await AddJobAsync(fileName);
			}

			return null;

		}

		public async Task<IDocumentViewModel> AddJobAsync(string fileName, bool runSimulationAfterAdding = false)
		{
			if (fileName != null)
			{
				try
				{
					var result = await LoadFileAsync(fileName);
					lock (_jobsLock)
					{
						Jobs.Add(result);
					}
					if (runSimulationAfterAdding) {
						if (result.CanBeSimulated) {
							await RunSimulationExecute(result);
						}
					}

					return result;
				}
				catch (Exception e)
				{
					var errorString = "";
					errorString = $"{fileName}\n";
					errorString += e.Message;
					_dialogHelper.ShowMessageBox(errorString, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}


			return null;
		}

		private Task<IDocumentViewModel> LoadFileAsync([NotNull] string fileName)
		{
			var extension = Path.GetExtension(fileName);
			switch (extension) {
				case Constants.FileExtensions.VectoXMLDeclarationFile:
					return LoadXMLFile(fileName);
				case Constants.FileExtensions.VectoJobFile:
					return LoadJsonFile(fileName);
				default: 
					throw new VectoException($"{extension} not supported!");
			}
			
		}

		private Task<IDocumentViewModel> LoadJsonFile([NotNull] string fileName)
		{
			IDocumentViewModel result = null;
			try {
				var inputData = JSONInputDataFactory.ReadJsonJob(fileName, true);
				return Task.FromResult(_multiStageViewModelFactory.CreateDocumentViewModel(inputData));
			} catch (Exception ex) {
				return Task.FromException<IDocumentViewModel>(ex);
				//_dialogHelper.ShowErrorMessage(ex.Message));
			}
		}

		private Task<IDocumentViewModel> LoadXMLFile([NotNull] string fileName)
		{
			var xElement = new System.Xml.XmlDocument();
			xElement.Load(fileName);

			var documentType = XMLHelper.GetDocumentType(xElement?.DocumentElement?.LocalName);
			if (documentType == XmlDocumentType.MultistepOutputData)
			{
				var inputDataProvider = _inputDataReader.Create(fileName) as IMultistageBusInputDataProvider;
				return Task.FromResult(_multiStageViewModelFactory.GetMultiStageJobViewModel(inputDataProvider) as IDocumentViewModel);
			}
			else if (documentType == XmlDocumentType.DeclarationJobData)
			{
				//Remove
				var inputDataProvider = _inputDataReader.CreateDeclaration(fileName);
				IDocumentViewModel result;
				try
				{
					result = _multiStageViewModelFactory.CreateDocumentViewModel(inputDataProvider);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.GetInnerExceptionMessages());
					result = new SimulationOnlyDeclarationJob(inputDataProvider.DataSource, inputDataProvider.JobInputData.JobName, XmlDocumentType.DeclarationJobData) as IDocumentViewModel;
				}
				return Task.FromResult(result);
			}
			else {
				return Task.FromException<IDocumentViewModel>(
					new VectoXMLException($"{documentType.ToString()} not supported"));
				//throw new VectoXMLException($"{documentType.ToString()} not supported");
			}

			return null;



		}

		#endregion

		private bool _newFilePopUpIsOpen = false;
		public bool NewFilePopUpIsOpen
		{
			get => _newFilePopUpIsOpen;
			set => SetProperty(ref _newFilePopUpIsOpen, value);
		}
		



		#region Simulation
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private bool _simulationRunning = false;

		public bool SimulationRunning
		{
			get => _simulationRunning;
			set
			{
				SetProperty(ref _simulationRunning, value);
				SimulationCommand?.NotifyCanExecuteChanged();
				
				(_cancelSimulationCommand as RelayCommand)?.NotifyCanExecuteChanged();
			}
		}

		private ICommand _cancelSimulationCommand;

		private IProgress<MessageEntry> _outputMessage;
		private IProgress<int> _progress;
		private IProgress<string> _status;


		public async Task RunSimulationExecute(IDocumentViewModel jobToSimulate = null)
		{
			if (SimulationRunning) {
				return;
			}
			SimulationRunning = true;
			try {
                await Task.Run(() => RunSimulationAsync(_cancellationTokenSource.Token,
                    outputMessages: _outputMessage,
                    progress: _progress,
                    status: _status,
                    jobToSimulate: jobToSimulate));
                //await Task.Factory.StartNew(() => RunSimulationAsync(_cancellationTokenSource.Token,
                //	outputMessages: _outputMessage,
                //	progress: _progress,
                //	status: _status,
                //	jobToSimulate: jobToSimulate),
                //		TaskCreationOptions.LongRunning).Unwrap();

            }
			catch (Exception ex) {
				_outputViewModel.AddMessage(new MessageEntry() {
					Type = MessageType.ErrorMessage,
					Message = ex.Message
				});
			} finally {
				_cancellationTokenSource = new CancellationTokenSource();
				_simulationLoggingEnabled = true;
				SimulationRunning = false;
				_outputViewModel.Progress = 0;
			}
		}

		private async Task RunSimulationAsync(CancellationToken ct, 
			IProgress<MessageEntry> outputMessages,
			IProgress<int> progress, 
			IProgress<string> status, 
			IDocumentViewModel jobToSimulate = null)
		{
			if (Thread.CurrentThread.Name == null) {
				Thread.CurrentThread.Name = "JobListThread";
			};


            progress.Report(0);
			status.Report("starting...");
			
			IDocumentViewModel[] jobs;
			if (jobToSimulate == null) {
				lock (_jobsLock)
				{
					jobs = Jobs.Where(x => x.Selected).ToArray();
					if (jobs.Length == 0)
					{
						outputMessages.Report(new MessageEntry()
						{
							Message = "No Jobs Selected",
							Time = DateTime.Now,
							Type = MessageType.InfoMessage,
						});
						status.Report("No Jobs selected");
						return;
					}
				}
			} else {
				jobs = new IDocumentViewModel[] {
					jobToSimulate
				};
				
			}

			var sumFileWriter = new FileOutputWriter(GetOutputDirectory(jobs.First().DataSource.SourceFile));
			var sumContainer = new SummaryDataContainer(sumFileWriter);
			var jobContainer = new JobContainer(sumContainer);
			var mode = ExecutionMode.Declaration;

			var fileWriters = new Dictionary<int, FileOutputWriter>();
			var finishedRuns = new List<int>();

			var xmlReader = _inputDataReader;

			foreach (var jobEntry in jobs) {
				try
				{
					var fullFileName = Path.GetFullPath(jobEntry.DataSource.SourceFile);
					if (!File.Exists(fullFileName))
					{
						outputMessages.Report(new MessageEntry()
							{
								Type = MessageType.ErrorMessage,
								Message =
									$"File {Path.GetFileName(jobEntry.DataSource.SourceFile)} not found!"
							});
						continue;
					}

					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.StatusMessage,
							Message = $"Reading file {Path.GetFileName(fullFileName)}"
						});



					var extension = Path.GetExtension(jobEntry.DataSource.SourceFile);
					IInputDataProvider input = null;
					IXMLMultistageInputDataProvider multistageInput = null;

					var FileWriter = new FileOutputWriter(fullFileName);
					switch (extension) {
						case Constants.FileExtensions.VectoJobFile:
							input = JSONInputDataFactory.ReadJsonJob(fullFileName);
							switch (input) {
								case IDeclarationInputDataProvider declInput:
									mode = declInput.JobInputData.SavedInDeclarationMode
										? ExecutionMode.Declaration
										: ExecutionMode.Engineering;
									break;
								case IMultistagePrimaryAndStageInputDataProvider primaryAndStage:
									mode = ExecutionMode.Declaration;
									break;
								default:
									input = null;

									break;
							}
							break;
						case ".xml":
							var xdoc = XDocument.Load(fullFileName);
							var rootNode = xdoc.Root?.Name.LocalName ?? "";
							if (XMLNames.VectoInputEngineering.Equals(rootNode,
								StringComparison.InvariantCultureIgnoreCase)) {
								input = xmlReader.CreateEngineering(fullFileName);
								mode = ExecutionMode.Engineering;
							} else if (XMLNames.VectoInputDeclaration.Equals(rootNode,
								StringComparison.InvariantCultureIgnoreCase)) {
								using (var reader = XmlReader.Create(fullFileName)) {
									input = xmlReader.CreateDeclaration(reader);
								}

								mode = ExecutionMode.Declaration;
							} else if (XMLNames.VectoOutputMultistep.Equals(rootNode,
								StringComparison.InvariantCultureIgnoreCase)) {
								using (var reader = XmlReader.Create(fullFileName)) {
									input = new XMLDeclarationVIFInputData(xmlReader.Create(fullFileName) as IMultistageBusInputDataProvider, null);
									FileWriter = new FileOutputVIFWriter(fullFileName,
										(jobEntry as MultiStageJobViewModel_v0_1).ManufacturingStages?.Count ?? 0);
								}

								mode = ExecutionMode.Declaration;
							}

							break;
					}

					if (input == null && multistageInput == null)
					{
						outputMessages.Report(
							new MessageEntry()
							{
								Type = MessageType.ErrorMessage,
								Message = $"No input provider for job {Path.GetFileName(fullFileName)}"
							});
						continue;
					}

					var fileWriter = new FileOutputWriter(GetOutputDirectory(fullFileName));
					var runsFactory = _simFactoryFactory.Factory(mode, input, fileWriter);
					//var runsFactory = SimulatorFactory.CreateSimulatorFactory(mode, input, fileWriter);
					runsFactory.WriteModalResults = Settings.Default.WriteModalResults;
					runsFactory.ModalResults1Hz = Settings.Default.ModalResults1Hz;
					runsFactory.Validate = Settings.Default.Validate;
					runsFactory.ActualModalData = Settings.Default.ActualModalData;
					runsFactory.SerializeVectoRunData = Settings.Default.SerializeVectoRunData;

					var stopwatch = new Stopwatch();
					stopwatch.Start();
					foreach (var runId in jobContainer.AddRuns(runsFactory))
					{
						if (ct.IsCancellationRequested) {
							outputMessages.Report(new MessageEntry()
							{
								Message = "Simulation canceled",
								Type = MessageType.InfoMessage,
							});
							return;
						}
						fileWriters.Add(runId, fileWriter);
					}
					stopwatch.Stop();


					// TODO MQ-20200525: Remove the following loop in production (or after evaluation of LAC!!

					
					//if (!string.IsNullOrWhiteSpace(LookAheadMinSpeedOverride))
					//{
					//	foreach (var run in jobContainer.Runs)
					//	{
					//		var tmpDriver = ((VectoRun)run.Run).GetContainer().RunData.DriverData;
					//		tmpDriver.LookAheadCoasting.Enabled = true;
					//		tmpDriver.LookAheadCoasting.MinSpeed = LookAheadMinSpeedOverride.ToDouble().KMPHtoMeterPerSecond();
					//	}
					//}
					

					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.StatusMessage,
							Message = $"Finished reading data for job {Path.GetFileName(fullFileName)}"
						});
				}
				catch (Exception ex)
				{
					/*
					MessageBox.Show(
						$"ERROR running job {Path.GetFileName(jobEntry.DataSource.SourceFile)}: {ex.Message}", "Error", MessageBoxButton.OK,
						MessageBoxImage.Exclamation);
					*/
					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.ErrorMessage, 
							Message = ex.Message
						});
				}
			}
			foreach (var cycle in jobContainer.GetCycleTypes())
			{
				outputMessages.Report(new MessageEntry()
				{
					Type = MessageType.StatusMessage, Message = $"Detected cycle {cycle.Name}: {cycle.CycleType}"
				});
			}

			outputMessages.Report(new MessageEntry() {
				Type = MessageType.StatusMessage,
				Message = $"Starting simulation ({jobs.Length} jobs, {jobContainer.GetProgress().Count} runs)",
			});

			var start = Stopwatch.StartNew();
			jobContainer.Execute(true);
			
			while (!jobContainer.AllCompleted)
			{
				if (ct.IsCancellationRequested)
				{
					try {
						await Task.Run(() => jobContainer.Cancel());
					} catch (Exception e) {
						Debug.WriteLine(e.Message);
					}

					outputMessages.Report(new MessageEntry()
					{
						Message = "Simulation canceled",
						Type = MessageType.InfoMessage,
					});

					return;
				}
				Debug.WriteLine(Thread.CurrentThread.Name);
				var jobProgress = jobContainer.GetProgress();
				var sumProgress = jobProgress.Sum(x => x.Value.Progress);
				var duration = start.Elapsed.TotalSeconds;
				//jobProgress.Select(x => x.Value.Progress);

				
				
				progress.Report(Convert.ToInt32(sumProgress * 100 / jobProgress.Count));
				status.Report(string.Format(
					"Duration: {0:F1}s, Current Progress: {1:P} ({2})", duration, sumProgress / jobProgress.Count,
					string.Join(", ", jobProgress.Select(x => string.Format("{0,4:P}", x.Value.Progress)))));
                
				var justFinished = jobProgress.Where(x => x.Value.Done & !finishedRuns.Contains(x.Key))
					.ToDictionary(x => x.Key, x => x.Value);
				PrintRuns(justFinished, fileWriters, outputMessages);
				finishedRuns.AddRange(justFinished.Select(x => x.Key));

				var delayMs = 100;
				Task.Delay(delayMs, ct).Wait(delayMs);
			}
			start.Stop();

			var remainingRuns = jobContainer.GetProgress().Where(x => x.Value.Done && !finishedRuns.Contains(x.Key))
				.ToDictionary(x => x.Key, x => x.Value);
			PrintRuns(remainingRuns, fileWriters, outputMessages);

			finishedRuns.Clear();
			fileWriters.Clear();
			foreach (var progressEntry in jobContainer.GetProgress())
			{
				outputMessages.Report(new MessageEntry()
				{
					Type = MessageType.StatusMessage,
					Message =
						string.Format("{0,-60} {1,8:P} {2,10:F2}s - {3}",
							$"{progressEntry.Value.RunName} {progressEntry.Value.CycleName} {progressEntry.Value.RunSuffix}",
							progressEntry.Value.Progress,
							progressEntry.Value.ExecTime / 1000.0,
							progressEntry.Value.Success ? "Success" : "Aborted")
				});
				if (!progressEntry.Value.Success)
				{
					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.StatusMessage,
							Message = progressEntry.Value.Error.Message
						}
					);
				}
			}

			var outputWriters = jobContainer.GetOutputDataWriters();
			var sortedOutputWriters = outputWriters.OrderBy(ow => ow.JobFile);
			foreach (var outputDataWriter in sortedOutputWriters) {
				var writtenFiles = outputDataWriter.GetWrittenFiles();
				if (writtenFiles is null || writtenFiles.Count == 0) {
					continue;
				}
				var jobFileName = outputDataWriter.JobFile;
				foreach (var entry in new Dictionary<ReportType, string> {
					{ ReportType.DeclarationReportPrimaryVehicleXML, "Primary Vehicle Information File" },
					{ ReportType.DeclarationReportManufacturerXML, "XML Manufacturer Report" },
					{ ReportType.DeclarationReportCustomerXML, "XML Customer Report" },
					{ ReportType.DeclarationVTPReportXML, "VTP Report" },
					{ ReportType.DeclarationReportMultistageVehicleXML, "VIF File" },
				}) {
					if (writtenFiles.ContainsKey(entry.Key)) {
						outputMessages.Report(
							new MessageEntry()
							{
								Type = MessageType.StatusMessage,
								Message = $"{entry.Value} for \n '{jobFileName}' \nwritten to \n '{writtenFiles[entry.Key]}'", 
								Link = writtenFiles[entry.Key],
							}
						);
					}
				}
			}
			if (sumFileWriter.SumFileWritten)
			{
				outputMessages.Report(new MessageEntry()
				{
					Type = MessageType.StatusMessage,
					Message = $"Sum file written to {sumFileWriter.SumFileName}",
					Link = sumFileWriter.SumFileName,
				});
			}

			outputMessages.Report(new MessageEntry()
			{
				Type = MessageType.StatusMessage,
				Message = string.Format("Simulation finished in {0:F1}s", start.Elapsed.TotalSeconds)
			});

			status.Report($"Simulation finished in {start.Elapsed.TotalSeconds,0:F1} s");

		}
		private void PrintRuns(Dictionary<int, JobContainer.ProgressEntry> progress, Dictionary<int, FileOutputWriter> fileWriters, IProgress<MessageEntry> outputMessages)
		{ 
			foreach (var p in progress) {
				var modFilename = "Add modFileName";
				if (fileWriters.ContainsKey(p.Key)) {
					modFilename = fileWriters[p.Key]
						.GetModDataFileName(p.Value.RunName, p.Value.CycleName, p.Value.RunSuffix);
					
				}
				var runName = string.Format("{0} {1} {2}", p.Value.RunName, p.Value.CycleName, p.Value.RunSuffix);

				if (p.Value.Error != null)
                {
                    outputMessages.Report(new MessageEntry()
                    {
                        Type = MessageType.StatusMessage,
                        Message = string.Format("Finished Run {0} with ERROR: {1}", runName,
                            p.Value.Error.Message),
                        //Link = modFilename
                        //Link = modFilename
						//Link = "<CSV>" + modFilename
					});
                }
                else
                {
					outputMessages.Report(new MessageEntry()
					{
                        Type = MessageType.StatusMessage,
                        Message = string.Format("Finished run {0} successfully.", runName)
                    });
                }
                if (File.Exists(modFilename))
                {
                    outputMessages.Report(new MessageEntry()
                    {
                        Type = MessageType.StatusMessage,
                        Message = string.Format("Run {0}: Modal results written to {1}", runName, modFilename),
                        Link = modFilename,
						//Link = "<CSV>" + modFilename
					});
                }
            }
		}


		private string GetOutputDirectory(string jobFilePath)
		{
			var outFile = jobFilePath;
			var outputDirectory = Settings.Default.DefaultOutputPath;
			if (string.IsNullOrWhiteSpace(outputDirectory)) {
				return outFile;
			}

			outFile = Path.IsPathRooted(outputDirectory)
				? Path.Combine(outputDirectory, Path.GetFileName(jobFilePath) ?? "")
				: Path.Combine(Path.GetDirectoryName(jobFilePath) ?? "", outputDirectory,
					Path.GetFileName(jobFilePath) ?? "");
			if (!Directory.Exists(Path.GetDirectoryName(outFile))) {
				Directory.CreateDirectory(Path.GetDirectoryName(outFile));
			}

			return outFile;
		}

		#endregion
		#region Commands
		private ICommand _editJobCommand;
		private IAsyncRelayCommand _removeJobCommand;
		private ICommand _moveJobUpCommand;
		private ICommand _moveJobDownCommand;
		private ICommand _viewXMLCommand;
		private IDocumentViewModel _selectedJob;
		private IAsyncRelayCommand _addJobAsync;
		private IAsyncRelayCommand<IDocumentViewModel> _simulationCommand;
		private IRelayCommand<bool> _newVifCommand;
		private ICommand _newMultiStageFileCommand;
		private ICommand _openNewFilePopUpCommand;
		private ICommand _newCompletedInputCommand;
		private ICommand _newExemptedCompletedInputCommand;
		private ICommand _openAdditionalJobInformationCommand;
		private IRelayCommand _openSourceFileCommand;
		private IRelayCommand _showSourceFileInExplorerCommand;
		private readonly ISimulatorFactoryFactory _simFactoryFactory;


		public ICommand OpenSourceFileCommand
		{
			get
			{
				return _openSourceFileCommand ?? (_openSourceFileCommand =
					new RelayCommand(() => { ProcessHelper.OpenFile(_selectedJob?.DataSource?.SourceFile); },
						() => _selectedJob != null));
			}
		}

		public ICommand ShowSourceFileCommand
		{
			get
			{
				return _showSourceFileInExplorerCommand ?? (_showSourceFileInExplorerCommand = 
					new RelayCommand(() => { ProcessHelper.OpenFolder(_selectedJob?.DataSource?.SourceFile); },
					() => _selectedJob != null));
			}
		}


		public ICommand OpenPopUpCommand
		{
			get => _openNewFilePopUpCommand ??
					(_openNewFilePopUpCommand = new RelayCommand(() => {
						if (NewFilePopUpIsOpen == false) {
							NewFilePopUpIsOpen = true;
						}
						
					}));
		}



		public ICommand CancelSimulation
		{
			get
			{
				return _cancelSimulationCommand ?? (_cancelSimulationCommand = new RelayCommand(() => {
						_outputViewModel.AddMessage(new MessageEntry() {
							Message="Canceling Simulation - this operation can take some time",
							Type=MessageType.InfoMessage,
						});
						_simulationLoggingEnabled = false;
						_cancellationTokenSource.Cancel();
						_status.Report("");
						
						
					},
					() => SimulationRunning));
			}            
		}

		public ICommand NewCompletedInputCommand
		{
			get
			{
				return _newCompletedInputCommand ?? (_newCompletedInputCommand = new RelayCommand(() => {
					NewCompletedInputCommandExecute(false);
				}));
			}
		}

		private void NewCompletedInputCommandExecute(bool exempted)
		{
			var stageInputVm = _multiStageViewModelFactory.GetStageInputViewModel(exempted);

			AddJob(stageInputVm);

			_windowHelper.ShowWindow(stageInputVm);
		}

		public ICommand NewExemptedCompletedInputCommand
		{
			get
			{
				return _newExemptedCompletedInputCommand ?? (_newExemptedCompletedInputCommand = new RelayCommand(() => {
					NewCompletedInputCommandExecute(true);
				}));
			}
		}

		public IRelayCommand<bool> NewVifCommand
		{
			get
			{
				return _newVifCommand ?? (_newVifCommand = new Microsoft.Toolkit.Mvvm.Input.RelayCommand<bool>((b) => {
					var newVifViewModel = _multiStageViewModelFactory.GetCreateNewVifViewModel(b);
					lock (_jobsLock) {
						_jobs.Add(newVifViewModel);
					}
					_windowHelper.ShowWindow(newVifViewModel);
				}, b => true));
			}
		}

		public IAsyncRelayCommand<IDocumentViewModel> SimulationCommand
		{
			get
			{
				return _simulationCommand ?? (_simulationCommand = 
						new AsyncRelayCommand<IDocumentViewModel>(RunSimulationExecute, (d) => !SimulationRunning));
			}
		}

		public ICommand NewManufacturingStageFileCommand
		{
			get
			{
				return _newMultiStageFileCommand ?? new RelayCommand(NewManufacturingStageFileExecute, () => { return true; });
			}
		}

		private void NewManufacturingStageFileExecute()
		{
            _windowHelper.ShowWindow(_multiStageViewModelFactory.GetNewMultistageJobViewModel());
		}

		public IAsyncRelayCommand AddJobAsyncCommand
		{
			get
			{
				return _addJobAsync ?? new AsyncRelayCommand(AddJobExecuteAsync
					, () => true);
			}
		}

		

		public ICommand EditDocument
        {
            get
			{
				return _editJobCommand ?? (_editJobCommand = new Util.RelayCommand<IDocumentViewModel>(EditDocumentExecute,
					(IDocumentViewModel jobentry) => {
						var canExecute = jobentry != null && jobentry.EditViewModel != null;
						return canExecute;
					}));
			}
            set
            {
                _editJobCommand = value;
                OnPropertyChanged();
            }
        }

        private void EditDocumentExecute(IDocumentViewModel selectedJob)
        {
			if (selectedJob == null) {
				return;
			}
            _windowHelper.ShowWindow(selectedJob.EditViewModel);
        }

        public ICommand ViewXMLFile
        {
            get
            {
                return _viewXMLCommand ?? new Util.RelayCommand<IJobViewModel>(ViewXMLFileExecute,
                    (IJobViewModel jobentry) =>
                    {
                        return (jobentry != null);
                    });
            }
            set
            {
                _viewXMLCommand = value;
                OnPropertyChanged();
            }
        }

        private void ViewXMLFileExecute(IJobViewModel selectedJob)
        {
            if (selectedJob == null) return;

 //TODO implement using WindowHelper.
            Debug.WriteLine("open XML File Viewer");
            //_kernel.Get<XMLViewer>().Show();


        }


        public IAsyncRelayCommand RemoveJob
        {
            get
            {
                return _removeJobCommand ?? (_removeJobCommand = new AsyncRelayCommand<IDocumentViewModel>(RemoveJobExecute, 
					(IDocumentViewModel jobEntry) => {
						return (SelectedJob != null);
					}));
            }
            set
            {
                _removeJobCommand = value;
                OnPropertyChanged();
            }
        }

		public void OnJobSelectionChanged()
		{
			RemoveJob.NotifyCanExecuteChanged();
		}


        private async Task RemoveJobExecute(IDocumentViewModel selectedDocument)
        {
			if (selectedDocument == null) {
				return;
			}

			await Task.Run(() => {
				lock (_jobsLock) {
					Jobs.Remove(selectedDocument);
				}
			});
			
          
            OnPropertyChanged();

		}

        public ICommand moveJobUp
        {
            get
            {
                return _moveJobUpCommand ?? new Util.RelayCommand<IDocumentViewModel>(MoveJobUpExecute, (IDocumentViewModel jobentry) =>
                {
                    return (jobentry != null && Jobs.Count > 1 && Jobs.IndexOf(jobentry) != 0);
                });
            }
            set
            {
                _moveJobUpCommand = value;
                OnPropertyChanged();
            }

        }

        private void MoveJobUpExecute(IDocumentViewModel selectedJob)
        {
            if (selectedJob == null) return;
            var index = Jobs.IndexOf(selectedJob);
            if (index > 0)
                Jobs.Move(index, index - 1);

        }

        public ICommand moveJobDown
        {
            get
            {
                return _moveJobDownCommand ?? new Util.RelayCommand<IDocumentViewModel>(MoveJobDownExecute, (IDocumentViewModel jobentry) =>
                {
                    return (jobentry != null && Jobs.Count > 1 && Jobs.IndexOf(jobentry) != Jobs.Count - 1);
                });
            }
            set
            {
                _moveJobDownCommand = value;
                OnPropertyChanged();
            }

        }

		public IOutputViewModel OutputViewModel
		{
			get => _outputViewModel;
			set => SetProperty(ref _outputViewModel, value);
		}

		public ICommand OpenAdditionalJobInformationCommand
		{
			get
			{
				return _openAdditionalJobInformationCommand ?? (_openAdditionalJobInformationCommand = new RelayCommand<IDocumentViewModel>(
					(docVm) => {
						_windowHelper.ShowWindow(docVm.AdditionalJobInfoVm);
					}));
			}
		}

		private void MoveJobDownExecute(IDocumentViewModel selectedJob)
        {
            Debug.WriteLine("move down command");

            if (selectedJob == null) return;
            var index = Jobs.IndexOf(selectedJob);
            if (index != Jobs.Count - 1)
                Jobs.Move(index, index + 1);

        }

        #endregion Commands
	}
}

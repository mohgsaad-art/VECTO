using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;
using NLog;
using NLog.Targets;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Helper;
using VECTO3GUI.Model;
using VECTO3GUI.Views;
using LogManager = TUGraz.VectoCommon.Models.LogManager;
using MessageBox = System.Windows.MessageBox;


namespace VECTO3GUI.ViewModel.Impl
{
	public class JoblistViewModel : ObservableObject, IJoblistViewModel
	{
		protected const string SETTINGS_FILE = "Config/Settings3.json";

		#region Members

		protected ObservableCollectionEx<JobEntry> _jobs;
		protected readonly ObservableCollection<MessageEntry> _messages = new ObservableCollection<MessageEntry>();
		private readonly SettingsModel _settings;
		private string _firstContextMenu;

		private JobEntry _selectedJobEntry;
		private JobListModel _jobListModel;
		private bool _visibilityFirstView;
		private bool _visibilitySecView;

		private ICommand _newJobCommand;
		private ICommand _editJobCommand;
		private ICommand _removeJobCommand;
		private ICommand _removeAllJobCommand;
		private ICommand _addJobCommand;
		private ICommand _openJobCommand;
		private ICommand _openSettingsCommand;
		private ICommand _exitMainCommand;
		private ICommand _addBusJobCommand;
		private ICommand _editCompletedFileCommand;
		private ICommand _moveJobUpCommand;
		private ICommand _moveJobDownCommand;
		private ICommand _openInFolderCommand;
		private ICommand _doubleClickCommand;
		private ICommand _runSimulationCommand;
		private BackgroundWorker SimulationWorker;
		private string _status;
		private double _progress;
		private ICommand _stopSimulationCommand;
		private bool _canRunSimulation = true;
		private bool _canStopSimulation = false;
		private bool _writeModData;
		private bool _writeModData1Hz;
		private bool _validateData;
		private bool _writeActualModData;
		private string _outputDirectory;
		private ICommand _browseOutputDirectory;
		private bool _writeModelData;
		private ICommand _aboutViewCommand;
		private string _lookAheadMinSpeedOverride;

		#endregion

		#region Properties

		public JobEntry SelectedJobEntry
		{
			get { return _selectedJobEntry; }
			set
			{
				if (SetProperty(ref _selectedJobEntry, value)) {
					if(_selectedJobEntry == null)
						return;
					SetXmlViewBehavior();
				}
			}
		}

		public ObservableCollectionEx<JobEntry> Jobs
		{
			get { return _jobs; }
			set { SetProperty(ref _jobs, value); }
		}

		public ObservableCollection<MessageEntry> Messages
		{
			get { return _messages; }
		}

		public string FirstContextMenu
		{
			get { return _firstContextMenu; }
			set { SetProperty(ref _firstContextMenu, value); }
		}

		public bool VisibilityFirstView
		{
			get { return _visibilityFirstView; }
			set { SetProperty(ref _visibilityFirstView, value); }
		}

		public bool VisibilitySecView
		{
			get { return _visibilitySecView; }
			set { SetProperty(ref _visibilitySecView, value); }
		}

		public string Status
		{
			get { return _status; }
			set { SetProperty(ref _status, value); }
		}

		public double Progress
		{
			get { return _progress; }
			set { SetProperty(ref _progress, value); }
		}

		public string LookAheadMinSpeedOverride
		{
			get { return _lookAheadMinSpeedOverride; }
			set { SetProperty(ref _lookAheadMinSpeedOverride, value); }
		}

		#endregion


		public JoblistViewModel()
		{
			_settings = new SettingsModel();
			LoadOptions();
			SetJobEntries();

			SimulationWorker = new BackgroundWorker();
			SimulationWorker.DoWork += RunVectoSimulation;
			SimulationWorker.ProgressChanged += VectoSimulationProgressChanged;
			SimulationWorker.RunWorkerCompleted += VectoSimulationCompleted;
			SimulationWorker.WorkerReportsProgress = true;
			SimulationWorker.WorkerSupportsCancellation = true;

			var target = new MethodCallTarget("VectoGuiTarget", (evtInfo, obj) => LogMethod(evtInfo, obj));
			NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);
		}

		private void LoadOptions()
		{
			if (!File.Exists(SETTINGS_FILE)) {
				WriteModData = true;
				ValidateData = true;
				return;
			}
			using (var reader = File.OpenText(SETTINGS_FILE)) {
				var content = JToken.ReadFrom(new JsonTextReader(reader));
				var body = content["Body"];
				if (body == null) {
					return;
				}

				WriteModData = body.GetValueOrDefault<bool>("WriteModData") ?? true;
				ValidateData = body.GetValueOrDefault<bool>("ValidateRunData") ?? true;
				WriteModData1Hz = body.GetValueOrDefault<bool>("WriteModData1Hz") ?? false;
				WriteActualModData = body.GetValueOrDefault<bool>("WriteActualModData") ?? false;
				OutputDirectory = body["OutputDirectory"] == null ? "" : body["OutputDirectory"].Value<string>();
			}
		}

		private void SaveOptions()
		{
			var header = new Dictionary<string, object>();
			header.Add("Date", DateTime.Now.ToUniversalTime().ToString("o"));
			header.Add("AppVersion", "4");
			header.Add("FileVersion", "4");

			var body = new Dictionary<string, object>();
			body.Add("WriteModData", WriteModData);
			body.Add("ValidateRunData", ValidateData);
			body.Add("WriteModData1Hz", WriteModData1Hz);
			body.Add("WriteActualModData", WriteActualModData);
			body.Add("OutputDirectory", OutputDirectory);

			JSONFileWriter.WriteFile(new Dictionary<string, object>() { { "Header", header }, { "Body", body } }, SETTINGS_FILE);
		}

		private void LogMethod(LogEventInfo evtInfo, object[] objects)
		{
			if (!SimulationWorker.IsBusy || SimulationWorker.CancellationPending) {
				return;
			}

			//if (evtInfo.Level == LogLevel.Warn) {
			//	SimulationWorker.ReportProgress(
			//		0,
			//		new VectoSimulationProgress() {
			//			Type = VectoSimulationProgress.MsgType.LogWarning,
			//			Message = evtInfo.FormattedMessage
			//		});
			//} else
			if (evtInfo.Level == LogLevel.Error || evtInfo.Level == LogLevel.Fatal) {
				SimulationWorker.ReportProgress(
					0,
					new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.LogError,
						Message = evtInfo.FormattedMessage
					});
			}
		}

		private void SetJobEntries()
		{
			_jobListModel = new JobListModel();
			_jobs = new ObservableCollectionEx<JobEntry>(_jobListModel.GetJobEntries());
			_jobs.CollectionChanged += JobsCollectionChanged;
			_jobs.CollectionItemChanged += JobItemChanged;
			VisibilityFirstView = true;
			VisibilitySecView = true;
		}

		private void JobItemChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Selected" && sender is JobEntry)
				UpdateJobListEntry((JobEntry)sender);
		}

		private void JobsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Reset:
					_jobListModel.SaveJobList(_jobs);
					break;
			}
		}

		#region Commands


		public ICommand RunSimulation
		{
			get { return _runSimulationCommand ?? (_runSimulationCommand = new RelayCommand(DoRunSimulation, CanRunSimulationCmd)); }
		}

		public ICommand StopSimulation
		{
			get {
				return _stopSimulationCommand ?? (_stopSimulationCommand = new RelayCommand(DoStopSimulation, CanStopSimulationCmd));
			}
		}

		public bool CanRunSimulation
		{
			get { return _canRunSimulation; }
			set { SetProperty(ref _canRunSimulation, value); }
		}

		public bool CanStopSimulation
		{
			get { return _canStopSimulation; }
			set { SetProperty(ref _canStopSimulation, value); }
		}

		public bool WriteModData
		{
			get { return _writeModData; }
			set { SetProperty(ref _writeModData, value); }
		}

		public bool WriteModData1Hz
		{
			get { return _writeModData1Hz; }
			set { SetProperty(ref _writeModData1Hz,value); }
		}

		public bool ValidateData
		{
			get { return _validateData; }
			set { SetProperty(ref _validateData, value); }
		}

		public bool WriteActualModData
		{
			get { return _writeActualModData; }
			set { SetProperty(ref _writeActualModData, value); }
		}

		public string OutputDirectory
		{
			get { return _outputDirectory; }
			set { SetProperty(ref _outputDirectory, value); }
		}

		public ICommand BrowseOutputDirectory
		{
			get { return _browseOutputDirectory ?? (_browseOutputDirectory = new RelayCommand(DoBrowseOutputDirectory)); }
		}

		public bool WriteModelData
		{
			get { return _writeModelData; }
			set { SetProperty(ref _writeModelData, value); }
		}
		
		private void DoBrowseOutputDirectory()
		{
			var filePath = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.JobFilter);
			if (filePath.IsNullOrEmpty())
				return;

			OutputDirectory = filePath[0];
		}


		public ICommand DoubleClickCommand
		{
			get { return _doubleClickCommand ?? (_doubleClickCommand = new RelayCommand<JobEntry>(DoDoubleClick)); }
		}

		private void DoDoubleClick(JobEntry jobEntry)
		{
			if (jobEntry == null || !IsJobFile(jobEntry.JobEntryFilePath))
				return;
			DoEditJob(jobEntry);
		}


		public ICommand RemoveJob
		{
			get
			{
				return _removeJobCommand ?? (_removeJobCommand = new RelayCommand<JobEntry>(DoRemoveJob, CanRemoveJob));
			}
		}
		private void DoRemoveJob(JobEntry jobEntry)
		{
			_jobs.Remove(jobEntry);
		}
		private bool CanRemoveJob(JobEntry jobEntry)
		{
			return jobEntry != null;
		}


		public ICommand RemoveAllJobs
		{
			get
			{
				return _removeAllJobCommand ?? (_removeAllJobCommand = new RelayCommand(DoRemoveAllJobs));
			}
		}
		private void DoRemoveAllJobs()
		{
			_jobs.Clear();
			SelectedJobEntry = null;
		}


		public ICommand EditJob
		{
			get
			{
				return _editJobCommand ?? (_editJobCommand = new RelayCommand<JobEntry>(DoEditJob, CanEditJob));
			}
		}
		private bool CanEditJob(JobEntry jobEntry)
		{
			return jobEntry != null && IsJobEntry(jobEntry);
		}
		private void DoEditJob(JobEntry jobEntry)
		{
			var viewModel = GetBusJobViewModel(jobEntry.Header.JobType, jobEntry);
			var window = CreateBusJobOutputWindow(viewModel, jobEntry.Header.JobType);
			if (window.ShowDialog() != true)
				ResetBusJobEntries(jobEntry);
			else
				UpdateJobEntry(((IBusJobViewModel)viewModel).SavedJobEntry);
		}

		public ICommand EditCompletedFile
		{
			get
			{
				return _editCompletedFileCommand ??
						(_editCompletedFileCommand =
							new RelayCommand<JobEntry>(DoEditCompletedFile, CanEditCompletedFile));
			}
		}
		private bool CanEditCompletedFile(JobEntry jobEntry)
		{
			return jobEntry != null && !jobEntry.Missing &&
					(IsJobEntry(jobEntry) || jobEntry.Header.JobType == JobType.CompletedXml);

		}
		private void DoEditCompletedFile(JobEntry jobEntry)
		{
			var viewModel = ReadCompletedXmlFile(jobEntry);
			if (viewModel == null)
				return;

			var window = OutputWindowHelper.CreateOutputWindow(Kernel, viewModel);
			window.Show();
		}
		

		public ICommand CreateNewJob
		{
			get
			{
				return _newJobCommand ?? (_newJobCommand = new RelayCommand(DoNewJobCommand));
			}
		}
		private void DoNewJobCommand()
		{
			var jobEditView = new CompleteVehicleBusJobViewModel(Kernel, null);
			var window = OutputWindowHelper.CreateOutputWindow(Kernel, jobEditView, "New File");
			window.Show();
		}


		public ICommand OpenJob
		{
			get
			{
				return _openJobCommand ?? (_openJobCommand = new RelayCommand<JobFileType>(DoOpenJobCommand));
			}
		}
		private void DoOpenJobCommand(JobFileType jobFileType)
		{
			if (SelectedJobEntry == null)
				return;

			XMLViewModel xmlViewModel = null;
			var jobType = _selectedJobEntry.Header.JobType;

			switch (jobFileType)
			{
				case JobFileType.PIFBusFile:
				case JobFileType.PrimaryBusFile:
					if (jobType == JobType.CompletedXml || jobType == JobType.Unknown &&
						IsXmlFile(_selectedJobEntry.JobEntryFilePath))
					{
						xmlViewModel = new XMLViewModel(SelectedJobEntry.JobEntryFilePath);
					}
					else if (IsJobFile(_selectedJobEntry.JobEntryFilePath)) {
						var filePath = SelectedJobEntry.Body.PrimaryVehicle ?? SelectedJobEntry.Body.PrimaryVehicleResults;
						xmlViewModel = new XMLViewModel(SelectedJobEntry.GetAbsoluteFilePath(filePath));
					}
					break;
				case JobFileType.CompletedBusFile:
					xmlViewModel = new XMLViewModel(
						SelectedJobEntry.GetAbsoluteFilePath(SelectedJobEntry.Body.CompletedVehicle));
					break;
			}

			if (xmlViewModel != null)
			{
				var window = OutputWindowHelper.CreateOutputWindow(Kernel, xmlViewModel, xmlViewModel.FileName);
				window.Show();
			}
		}


		public ICommand AddJob
		{
			get
			{
				return _addJobCommand ?? (_addJobCommand = new RelayCommand(DoAddJob));
			}
		}
		private void DoAddJob()
		{
			var filePath = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.JobFilter);
			if (filePath.IsNullOrEmpty() || !IsNewJobFile(filePath.First()))
				return;

			HandleFileOpen(filePath.First());
		}

		public void HandleFileOpen(string filePath)
		{
			JobEntry jobEntry = null;
			if (IsJobFile(filePath)) {
				jobEntry = SerializeHelper.DeserializeToObject<JobEntry>(filePath);
				jobEntry.JobEntryFilePath = filePath;
				jobEntry.Selected = true;
				//_jobs.Add(jobEntry);
			} else if (IsXmlFile(filePath)) {
				jobEntry = GetAdditionalJobEntry(filePath);
				jobEntry.Selected = true;
				//_jobs.Add(jobEntry);
			}
			if (jobEntry == null) {
				return;
			}

			var newJob = Path.GetFullPath(jobEntry.JobEntryFilePath);
			var existing = _jobs.Where(x => Path.GetFullPath(x.JobEntryFilePath).Equals(newJob, StringComparison.InvariantCultureIgnoreCase)).ToArray();
			if (existing.Length == 0) {
				_jobs.Add(jobEntry);
				return;
			}

			SelectedJobEntry = existing.First();
		}

		private bool IsJobFile(string filePath)
		{
			var extension = Path.GetExtension(filePath)?.ToLower();

			if (extension == FileDialogHelper.JobFileExtension)
				return true;
			if (extension == FileDialogHelper.XMLExtension)
				return false;

			return false;
		}
		

		public ICommand OpenSettings
		{
			get
			{
				return _openSettingsCommand ?? (_openSettingsCommand = new RelayCommand(DoOpenSettingsCommand));
			}
		}
		private void DoOpenSettingsCommand()
		{
			var viewModel = new SettingsViewModel();
			var window = OutputWindowHelper.CreateOutputWindow(Kernel, viewModel, "Settings", 440, 200,
				ResizeMode.NoResize);
			window.ShowDialog();
		}


		public ICommand ExitMainCommand
		{
			get
			{
				return _exitMainCommand ?? (_exitMainCommand = new RelayCommand<Window>(DoCloseMainCommand));
			}
		}
		private void DoCloseMainCommand(Window window)
		{
			window?.Close();
		}


		public ICommand AddBusJob
		{
			get
			{
				return _addBusJobCommand ?? (_addBusJobCommand = new RelayCommand<JobType>(DoAddBusJobCommand));
			}
		}
		private void DoAddBusJobCommand(JobType jobType)
		{
			var viewModel = GetBusJobViewModel(jobType);
			var window = CreateBusJobOutputWindow(viewModel, jobType);
			if (window.ShowDialog() == true)
				AddBusJobEntry(((IBusJobViewModel)viewModel)?.SavedJobEntry);
		}


		public ICommand MoveJobUp
		{
			get { return _moveJobUpCommand ?? (_moveJobUpCommand = new RelayCommand<JobEntry>(DoMoveJobUpCommand)); }
		}
		private void DoMoveJobUpCommand(JobEntry jobEntry)
		{
			if (jobEntry == null)
				return;
			var index = _jobs.IndexOf(jobEntry);
			if (index - 1 >= 0)
				_jobs.Move(index, index - 1);
		}

		public ICommand MoveJobDown
		{
			get { return _moveJobDownCommand ?? (_moveJobDownCommand = new RelayCommand<JobEntry>(DoMoveJobDownCommand)); }
		}
		private void DoMoveJobDownCommand(JobEntry jobEntry)
		{
			if (jobEntry == null)
				return;
			var index = _jobs.IndexOf(jobEntry);
			if (index + 1 < _jobs.Count)
				_jobs.Move(index, index + 1);
		}

		public ICommand OpenInFolder
		{
			get { return _openInFolderCommand ?? (_openInFolderCommand = new RelayCommand<JobEntry>(DoOpenInFolderCommand)); }
		}
		private void DoOpenInFolderCommand(JobEntry jobEntry)
		{
			if (jobEntry != null)
			{
				var dirPath = Path.GetDirectoryName(jobEntry.JobEntryFilePath);
				if (Directory.Exists(dirPath))
				{
					Process.Start("explorer.exe", dirPath);
				}
			}
		}

		public ICommand AboutViewCommand
		{
			get { return _aboutViewCommand ?? (_aboutViewCommand = new RelayCommand(DoAboutViewCommand)); }
		}
		private void DoAboutViewCommand()
		{

			var viewModel  = new AboutViewModel();

			var window = OutputWindowHelper.CreateOutputWindow(Kernel, viewModel, "About VECTO", 507, 395, ResizeMode.NoResize);
			window.Show();
		}

		#endregion

		private object GetBusJobViewModel(JobType jobType, JobEntry jobEntry = null)
		{
			var currentJobType = jobEntry?.Header.JobType ?? jobType;

			object viewModel = null;

			switch (currentJobType)
			{
				case JobType.SingleBusJob:
					viewModel = jobEntry == null
						? new SingleBusJobViewModel(Kernel, jobType)
						: new SingleBusJobViewModel(Kernel, jobEntry);
					break;
				case JobType.CompletedBusJob:
					viewModel = jobEntry == null
						? new CompletedBusJobViewModel(Kernel, jobType)
						: new CompletedBusJobViewModel(Kernel, jobEntry);
					break;
			}

			return viewModel;
		}

		private JobEntry GetAdditionalJobEntry(string filePath)
		{
			var isCompletedBusXml = IsCompletedBusXml(filePath);

			return new JobEntry
			{
				JobEntryFilePath = filePath,
				Header = new JobHeader
				{
					JobType = isCompletedBusXml ? JobType.CompletedXml : JobType.Unknown
				},
				Body = new JobBody
				{
					CompletedVehicle = isCompletedBusXml ? filePath : null
				}
			};
		}


		private bool IsCompletedBusXml(string filePath)
		{
			if (!File.Exists(filePath))
				return false;

			var xmlInputReader = Kernel.Get<IXMLInputDataReader>();

			using (var reader = XmlReader.Create(filePath))
			{

				var readerResult = xmlInputReader.Create(reader);
				if (readerResult is IDeclarationInputDataProvider)
				{

					var inputData = readerResult as IDeclarationInputDataProvider;
					if (inputData.JobInputData.Vehicle is XMLDeclarationCompletedBusDataProviderV26)
					{
						return true;
					}
				}
			}

			return false;
		}


		private OutputWindow CreateBusJobOutputWindow(object viewModel, JobType jobType)
		{
			return OutputWindowHelper.CreateOutputWindow(Kernel, viewModel, $"Create {jobType.GetLabel()}",
				460, 200, ResizeMode.NoResize);
		}

		private void AddBusJobEntry(JobEntry jobEntry)
		{
			if (jobEntry == null)
				return;
			_jobs.Add(jobEntry);
		}

		private void ResetBusJobEntries(JobEntry jobEntry)
		{
			SerializeHelper.DeserializeToObject<JobEntry>(jobEntry.JobEntryFilePath);
		}

		private void UpdateJobEntry(JobEntry jobEntry)
		{
			SerializeHelper.SerializeToFile(jobEntry.JobEntryFilePath, jobEntry);
		}

		private void UpdateJobListEntry(JobEntry jobEntry)
		{
			for (int i = 0; i < Jobs.Count; i++)
			{
				if (Jobs[i].JobEntryFilePath == jobEntry.JobEntryFilePath)
				{
					Jobs[i].Selected = jobEntry.Selected;
				}
			}
			_jobListModel.SaveJobList(_jobs);
		}

		private IJobEditViewModel ReadCompletedXmlFile(JobEntry jobEntry)
		{
			var xmlInputReader = Kernel.Get<IXMLInputDataReader>();
			using (var reader = XmlReader.Create(jobEntry.GetAbsoluteFilePath(jobEntry.Body.CompletedVehicle)))
			{
				var readerResult = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
				return CreateCompleteBusVehicleViewModel(readerResult);
			}
		}

		private IJobEditViewModel CreateCompleteBusVehicleViewModel(IDeclarationInputDataProvider dataProvider)
		{
			return dataProvider == null ? null : new CompleteVehicleBusJobViewModel(Kernel, dataProvider);
		}

		private bool IsNewJobFile(string filePath)
		{
			if (!_jobs.IsNullOrEmpty())
			{
				for (int i = 0; i < _jobs.Count; i++)
				{
					if (_jobs[i].JobEntryFilePath == filePath)
						return false;
				}
			}

			return true;
		}

		private bool IsJobEntry(JobEntry jobEntry)
		{
			return !jobEntry.Missing && (jobEntry.Header.JobType == JobType.CompletedBusJob ||
					jobEntry.Header.JobType == JobType.SingleBusJob);
		}

		private bool IsXmlFile(string filePath)
		{
			if (!File.Exists(filePath))
				return false;

			return Path.GetExtension(filePath)?.ToLower() == FileDialogHelper.XMLExtension;
		}

		private void SetXmlViewBehavior()
		{
			var isJobEntry = IsJobEntry(_selectedJobEntry);
			var contextMenuText = string.Empty;

			VisibilityFirstView = true;

			if (isJobEntry)
			{
				VisibilitySecView = true;

				contextMenuText = _selectedJobEntry.Header.JobType == JobType.SingleBusJob
					? JobFileType.PrimaryBusFile.GetLable()
					: JobFileType.PIFBusFile.GetLable();
			}
			else if (IsXmlFile(_selectedJobEntry.JobEntryFilePath))
			{

				VisibilitySecView = false;
				contextMenuText = _selectedJobEntry.Header.JobType == JobType.CompletedXml
					? JobType.CompletedXml.GetLabel() : "XML-File";
			}
			
			FirstContextMenu = $"View {contextMenuText}";
		}

		#region RunVECTOSimulation

		private bool CanRunSimulationCmd()
		{
			return !SimulationWorker.IsBusy;
		}

		private void DoRunSimulation()
		{
			if (!SimulationWorker.IsBusy) {
				// change button to stop button
				Messages.Clear();
				SimulationWorker.RunWorkerAsync();
			}
		}

		private bool CanStopSimulationCmd()
		{
			return SimulationWorker.IsBusy;
		}

		private void DoStopSimulation()
		{
			SimulationWorker.CancelAsync();
		}

		

		private void RunVectoSimulation(object theSender, DoWorkEventArgs e)
		{
			SaveOptions();
			var sender = theSender as BackgroundWorker;
			if (sender == null) {
				return;
			}

			CanRunSimulation = false;
			CanStopSimulation = true;

			var jobs = Jobs.Where(x => x.Selected).ToArray();
			if (jobs.Length == 0) {
				sender.ReportProgress(100, new VectoSimulationProgress() {Type = VectoSimulationProgress.MsgType.StatusMessage, Message = "No jobs selected for simulation"});
				return;
			}
			var sumFileWriter = new FileOutputWriter(GetOutputDirectory(jobs.First().JobEntryFilePath));
			var sumContainer = new SummaryDataContainer(sumFileWriter);
			var jobContainer = new JobContainer(sumContainer);

			var mode = ExecutionMode.Declaration;

			var fileWriters = new Dictionary<int, FileOutputWriter>();
			var finishedRuns = new List<int>();

			
			var kernel = new StandardKernel(new VectoNinjectModule());
			var xmlReader = kernel.Get<IXMLInputDataReader>();

			foreach (var jobEntry in jobs) {
				try {
					var fullFileName = Path.GetFullPath(jobEntry.JobEntryFilePath);
					if (!File.Exists(fullFileName)) {
						sender.ReportProgress(
							0, new VectoSimulationProgress() {
								Type = VectoSimulationProgress.MsgType.StatusMessage,
								Message =
									$"File {jobEntry.JobEntryFilePath} not found!"
							});
						continue;
					}

					sender.ReportProgress(
						0,
						new VectoSimulationProgress() {
							Type = VectoSimulationProgress.MsgType.StatusMessage,
							Message = $"Reading file {Path.GetFileName(fullFileName)}"
						});
					var extension = Path.GetExtension(jobEntry.JobEntryFilePath);
					IInputDataProvider input = null;
					switch (extension) {
						case Constants.FileExtensions.VectoJobFile:
							input = JSONInputDataFactory.ReadJsonJob(fullFileName);
							var tmp = input as IDeclarationInputDataProvider;
							mode = tmp?.JobInputData.SavedInDeclarationMode ?? false ? ExecutionMode.Declaration : ExecutionMode.Engineering;
							break;
						case ".xml":
							var xdoc = XDocument.Load(fullFileName);
							var rootNode = xdoc.Root?.Name.LocalName ?? "";
							if (XMLNames.VectoInputEngineering.Equals(rootNode, StringComparison.InvariantCultureIgnoreCase)) {
								input = xmlReader.CreateEngineering(fullFileName);
								mode = ExecutionMode.Engineering;
							} else if (XMLNames.VectoInputDeclaration.Equals(rootNode, StringComparison.InvariantCultureIgnoreCase)) {
								using (var reader = XmlReader.Create(fullFileName)) {
									input = xmlReader.CreateDeclaration(reader);
								}
								mode = ExecutionMode.Declaration;
							}
							break;
					}

					if (input == null) {
						sender.ReportProgress(
							0,
							new VectoSimulationProgress() {
								Type = VectoSimulationProgress.MsgType.StatusMessage,
								Message = $"No input provider for job {Path.GetFileName(fullFileName)}"
							});
						continue;
					}

					var fileWriter = new FileOutputWriter(GetOutputDirectory(fullFileName));
					var runsFactory = SimulatorFactory.CreateSimulatorFactory(mode, input, fileWriter);
					runsFactory.WriteModalResults = WriteModData;
					runsFactory.ModalResults1Hz = WriteModData1Hz;
					runsFactory.Validate = ValidateData;
					runsFactory.ActualModalData = WriteActualModData;
					runsFactory.SerializeVectoRunData = WriteModelData;
					foreach (var runId in jobContainer.AddRuns(runsFactory)) {
						fileWriters.Add(runId, fileWriter);
					}

					// TODO MQ-20200525: Remove the following loop in production (or after evaluation of LAC!!
					if (!string.IsNullOrWhiteSpace(LookAheadMinSpeedOverride)) {
						foreach (var run in jobContainer.Runs) {
							var tmpDriver = ((VectoRun)run.Run).GetContainer().RunData.DriverData;
							tmpDriver.LookAheadCoasting.Enabled = true;
							tmpDriver.LookAheadCoasting.MinSpeed = LookAheadMinSpeedOverride.ToDouble().KMPHtoMeterPerSecond();
						}
					}

					sender.ReportProgress(
						0,
						new VectoSimulationProgress() {
							Type = VectoSimulationProgress.MsgType.StatusMessage,
							Message = $"Finished reading data for job {Path.GetFileName(fullFileName)}"
						});
				} catch (Exception ex) {
					MessageBox.Show(
						$"ERROR running job {Path.GetFileName(jobEntry.JobEntryFilePath)}: {ex.Message}", "Error", MessageBoxButton.OK,
						MessageBoxImage.Exclamation);
					sender.ReportProgress(
						0, new VectoSimulationProgress() { Type = VectoSimulationProgress.MsgType.StatusMessage, Message = ex.Message });
				}
			}

			foreach (var cycle in jobContainer.GetCycleTypes()) {
				sender.ReportProgress(0, new VectoSimulationProgress() {Type = VectoSimulationProgress.MsgType.StatusMessage, Message = $"Detected cycle {cycle.Name}: {cycle.CycleType}"});
			}

			sender.ReportProgress(0, new VectoSimulationProgress() {Type = VectoSimulationProgress.MsgType.StatusMessage, Message = $"Starting simulation ({jobs.Length} jobs, {jobContainer.GetProgress().Count} runs)"});

			var start = Stopwatch.StartNew();

			jobContainer.Execute(true);

			while (!jobContainer.AllCompleted) {
				if (sender.CancellationPending) {
					jobContainer.Cancel();
					return;
				}

				var progress = jobContainer.GetProgress();
				var sumProgress = progress.Sum(x => x.Value.Progress);
				var duration = start.Elapsed.TotalSeconds;

				sender.ReportProgress(
					Convert.ToInt32(sumProgress * 100.0 / progress.Count), new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.Progress,
						Message = string.Format(
							"Duration: {0:F1}s, Curernt Progress: {1:P} ({2})", duration, sumProgress / progress.Count,
							string.Join(", ", progress.Select(x => string.Format("{0,4:P}", x.Value.Progress))))
					});
				var justFinished = progress.Where(x => x.Value.Done & !finishedRuns.Contains(x.Key))
											.ToDictionary(x => x.Key, x => x.Value);
				PrintRuns(justFinished, fileWriters);
				finishedRuns.AddRange(justFinished.Select(x => x.Key));
				Thread.Sleep(100);
			}
			start.Stop();

			var remainingRuns = jobContainer.GetProgress().Where(x => x.Value.Done && !finishedRuns.Contains(x.Key))
											.ToDictionary(x => x.Key, x => x.Value);
			PrintRuns(remainingRuns, fileWriters);

			finishedRuns.Clear();
			fileWriters.Clear();

			foreach (var progressEntry in jobContainer.GetProgress()) {
				sender.ReportProgress(100, new VectoSimulationProgress() {
					Type = VectoSimulationProgress.MsgType.StatusMessage, Message = 
						string.Format("{0,-60} {1,8:P} {2,10:F2}s - {3}",
									$"{progressEntry.Value.RunName} {progressEntry.Value.CycleName} {progressEntry.Value.RunSuffix}",
						progressEntry.Value.Progress,
						progressEntry.Value.ExecTime / 1000.0,
						progressEntry.Value.Success ? "Success" : "Aborted")
				});
				if (!progressEntry.Value.Success) {
					sender.ReportProgress(
						100,
						new VectoSimulationProgress() {
							Type = VectoSimulationProgress.MsgType.StatusMessage,
							Message = progressEntry.Value.Error.Message
						}
					);
				}
			}

			foreach (var jobEntry in jobs) {
				var w = new FileOutputWriter(GetOutputDirectory(jobEntry.JobEntryFilePath));
				foreach (var entry in new Dictionary<string, string>() { {w.XMLFullReportName,  "XML ManufacturereReport"}, {w.XMLCustomerReportName, "XML Customer Report"}, { w.XMLVTPReportName, "VTP Report"}, {w.XMLPrimaryVehicleReportName, "Primary Vehicle Information File"}  }) {
					if (File.Exists(entry.Key)) {
						sender.ReportProgress(
							100,
							new VectoSimulationProgress() {
								Type = VectoSimulationProgress.MsgType.StatusMessage,
								Message = string.Format(
									"{2} for '{0}' written to {1}", Path.GetFileName(jobEntry.JobEntryFilePath), entry.Key, entry.Value),
								Link = "<XML>" + entry.Key
							});
					}	
				}
			}

			if (File.Exists(sumFileWriter.SumFileName)) {
				sender.ReportProgress(100, new VectoSimulationProgress() {
					Type = VectoSimulationProgress.MsgType.StatusMessage,
					Message = string.Format("Sum file written to {0}", sumFileWriter.SumFileName),
					Link = "<CSV>" + sumFileWriter.SumFileName
				});
			}

			sender.ReportProgress(100, new VectoSimulationProgress() {
				Type = VectoSimulationProgress.MsgType.StatusMessage,
				Message = string.Format("Simulation finished in {0:F1}s", start.Elapsed.TotalSeconds)
			});
		}

		private string GetOutputDirectory(string jobFilePath)
		{
			var outFile = jobFilePath;
			if (!string.IsNullOrWhiteSpace(OutputDirectory)) {
				if (Path.IsPathRooted(OutputDirectory)) {
					outFile = Path.Combine(OutputDirectory, Path.GetFileName(jobFilePath) ?? "");
				} else {
					outFile = Path.Combine(Path.GetDirectoryName(jobFilePath) ?? "", OutputDirectory, Path.GetFileName(jobFilePath) ?? "");
				}
				if (!Directory.Exists(Path.GetDirectoryName(outFile))) {
					Directory.CreateDirectory(Path.GetDirectoryName(outFile));
				}
			}

			return outFile;
		}

		private void PrintRuns(Dictionary<int, JobContainer.ProgressEntry> progress, Dictionary<int, FileOutputWriter> fileWriters)
		{
			foreach (var p in progress) {
				var modFilename = fileWriters[p.Key].GetModDataFileName(p.Value.RunName, p.Value.CycleName, p.Value.RunSuffix);
				var runName = string.Format("{0} {1} {2}", p.Value.RunName, p.Value.CycleName, p.Value.RunSuffix);

				if (p.Value.Error != null) {
					SimulationWorker.ReportProgress(0, new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.StatusMessage,
						Message = string.Format("Finished Run {0} with ERROR: {1}", runName,
												p.Value.Error.Message),
						Link = "<CSV>" + modFilename
					});
				} else {
					SimulationWorker.ReportProgress(0, new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.StatusMessage,
						Message = string.Format("Finished run {0} successfully.", runName)
					});
				}
				if (File.Exists(modFilename)) {
					SimulationWorker.ReportProgress(0, new VectoSimulationProgress() {
						Type = VectoSimulationProgress.MsgType.StatusMessage,
						Message = string.Format("Run {0}: Modal results written to {1}", runName, modFilename),
						Link = "<CSV>" + modFilename
					});
				}
			}
		}


		private void VectoSimulationCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Progress = 0;
			Status = "";
			CanRunSimulation = true;
			CanStopSimulation = false;
		}

		private void VectoSimulationProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			var progress = e.UserState as VectoSimulationProgress;
			if (progress == null) {
				return;
			}

			switch (progress.Type) {
				case VectoSimulationProgress.MsgType.LogError:
				case VectoSimulationProgress.MsgType.LogWarning:
				case VectoSimulationProgress.MsgType.InfoMessage:
				case VectoSimulationProgress.MsgType.StatusMessage: {
					//if (progress.Link == null) {
					Messages.Add(new MessageEntry() { Message = progress.Message, Type = progress.Type.ToMessageType() });

					//}
					break;
				}
				case VectoSimulationProgress.MsgType.Progress:
					Progress = e.ProgressPercentage;
					Status = progress.Message;
					break;
				default: throw new ArgumentOutOfRangeException();
			}
		}


		public class VectoSimulationProgress
		{
			public enum MsgType
			{
				StatusMessage,
				InfoMessage,
				Progress,
				LogError,
				LogWarning,
				
			}

			public string Message { get; set; }

			public MsgType Type { get; set; }

			public string Link { get; set; }
		}

		
#endregion

		#region Implementation of IMainView

		public void Closing(object sender, CancelEventArgs e)
		{
			SaveOptions();
		}

		#endregion
	}
	public static class MsgTypeExtensions
	{
		public static MessageType ToMessageType(this JoblistViewModel.VectoSimulationProgress.MsgType mt)
		{
			switch (mt) {
				case JoblistViewModel.VectoSimulationProgress.MsgType.StatusMessage: return MessageType.StatusMessage;
				case JoblistViewModel.VectoSimulationProgress.MsgType.InfoMessage: return MessageType.InfoMessage;
				case JoblistViewModel.VectoSimulationProgress.MsgType.Progress: return MessageType.StatusMessage;
				case JoblistViewModel.VectoSimulationProgress.MsgType.LogError: return MessageType.ErrorMessage;
				case JoblistViewModel.VectoSimulationProgress.MsgType.LogWarning: return MessageType.WarningMessage;
				default: throw new ArgumentOutOfRangeException(nameof(mt), mt, null);
			}
		}
	}

}

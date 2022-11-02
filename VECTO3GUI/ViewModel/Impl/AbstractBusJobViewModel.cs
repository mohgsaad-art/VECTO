using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI.Helper;
using VECTO3GUI.Model;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{

	public enum JobFileType
	{
		CompletedBusFile,
		PrimaryBusFile,
		PIFBusFile,
	}

	public static class JobFileTypeHelper
	{
		public static string GetLable(this JobFileType fileType)
		{
			switch (fileType)
			{
				case JobFileType.CompletedBusFile:
					return "Completed Bus File";
				case JobFileType.PrimaryBusFile:
					return "Primary Bus File";
				case JobFileType.PIFBusFile:
					return "PIF Bus File";
				default:
					return string.Empty;
			}

		}
	}



	public abstract class AbstractBusJobViewModel : ViewModelBase, IBusJobViewModel
	{
		#region Members

		private JobFileType _firstFileType;
		private JobFileType _secondFileType;

		private string _firstLabelText;
		private string _secondLabelText;
		private string _firstFilePath;
		private string _secondFilePath;

		private ICommand _selectFirstFileCommand;
		private ICommand _selectSecondFileCommand;
		private ICommand _cancelCommand;
		private ICommand _saveCommand;

		protected SettingsModel Settings { get; private set; }

		protected JobType JobType;
		protected JobEntry JobEntry;
		private readonly bool _editJob;

		#endregion

		#region Properties

		public JobEntry SavedJobEntry { get; private set; }

		public string FirstFilePath
		{
			get { return _firstFilePath; }
			set { SetProperty(ref _firstFilePath, value); }
		}

		public string SecondFilePath
		{
			get { return _secondFilePath; }
			set { SetProperty(ref _secondFilePath, value); }
		}

		public string FirstLabelText
		{
			get { return _firstLabelText; }
			set { SetProperty(ref _firstLabelText, value); }
		}

		public string SecondLabelText
		{
			get { return _secondLabelText; }
			set { SetProperty(ref _secondLabelText, value); }
		}

		public JobFileType FirstFileType
		{
			get { return _firstFileType; }
			set { SetProperty(ref _firstFileType, value); }
		}
		public JobFileType SecondFileType
		{
			get { return _secondFileType; }
			set { SetProperty(ref _secondFileType, value); }
		}

		#endregion

		protected AbstractBusJobViewModel(IKernel kernel, JobType jobType)
		{
			Init(kernel, jobType);
			_editJob = false;
		}

		protected AbstractBusJobViewModel(IKernel kernel, JobEntry jobEntry)
		{
			Init(kernel, jobEntry.Header.JobType);
			SetJobEntryData(jobEntry);
			SavedJobEntry = jobEntry;
			_editJob = true;
		}

		private void Init(IKernel kernel, JobType jobType)
		{
			SecondLabelText = $"Select {JobFileType.CompletedBusFile.GetLable()}";
			Settings = new SettingsModel();
			SetFileTypes(jobType);
			Kernel = kernel;
		}

		private void SetFileTypes(JobType jobType)
		{
			JobType = jobType;

			_firstFileType = jobType == JobType.SingleBusJob
				? JobFileType.PrimaryBusFile
				: JobFileType.PIFBusFile;

			_secondFileType = JobFileType.CompletedBusFile;
		}

		private void SetJobEntryData(JobEntry jobEntry)
		{
			JobEntry = jobEntry;

			FirstFilePath = jobEntry.Header.JobType == JobType.SingleBusJob
				? jobEntry.Body.PrimaryVehicle
				: jobEntry.Body.PrimaryVehicleResults;
			SecondFilePath = jobEntry.Body.CompletedVehicle;
		}

		protected abstract void SetFirstFileLabel();

		#region Commands

		public ICommand SelectFirstFileCommand
		{
			get
			{
				return _selectFirstFileCommand ??
					  (_selectFirstFileCommand = new RelayCommand<JobFileType>(DoSelectFirstFileCommand));
			}
		}
		private void DoSelectFirstFileCommand(JobFileType jobFileType)
		{
			FirstFilePath = OpenFileSelector(jobFileType, nameof(FirstFilePath), FirstFilePath);
		}

		public ICommand SelectSecondFileCommand
		{
			get
			{
				return _selectSecondFileCommand ??
						(_selectSecondFileCommand = new RelayCommand<JobFileType>(DoSelectSecondFileCommand));
			}
		}
		private void DoSelectSecondFileCommand(JobFileType jobFileType)
		{
			SecondFilePath = OpenFileSelector(jobFileType, nameof(SecondFilePath), SecondFilePath);
		}

		public ICommand CancelCommand
		{
			get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<Window>(DoCancelCommand)); }
		}
		private void DoCancelCommand(Window window)
		{
			window.Close();
		}

		public ICommand SaveCommand
		{
			get { return _saveCommand ?? (_saveCommand = new RelayCommand<Window>(DoSaveCommand, CanSaveCommand)); }
		}
		private bool CanSaveCommand(Window window)
		{
			return !HasErrors && !FirstFilePath.IsNullOrEmpty() && !SecondFilePath.IsNullOrEmpty();
		}
		private void DoSaveCommand(Window window)
		{
			window.DialogResult = true;
			if (!_editJob)
				SaveJob(window);
			else
				UpdateJobData();
		}

		public ICommand CloseWindowCommand
		{
			get { return null; }
		}

		#endregion


		private void SaveJob(Window window)
		{
			var jobFilePath = FileDialogHelper.SaveJobFileToDialog(Settings.XmlFilePathFolder);
			if (jobFilePath == null)
				return;

			var job = new JobEntry
			{
				JobEntryFilePath = jobFilePath,

				Header = new JobHeader {
					JobType = JobType,
					FileVersion = JobType.GetJobTypeNumberByJobType(),
					AppVersion = JobEntry.APP_VERSION,
					CreatedBy = Environment.UserName,
					Date = DateTime.UtcNow
				}
			};

			var jobBody = new JobBody {
				CompletedVehicle = SecondFilePath
			};
			jobBody.PrimaryVehicle = JobType.SingleBusJob == JobType ? FirstFilePath : null; 
			jobBody.PrimaryVehicleResults = JobType.CompletedBusJob == JobType ? FirstFilePath : null;

			job.Body = jobBody;


			SerializeHelper.SerializeToFile(jobFilePath, job);
			SavedJobEntry = job;
			DoCancelCommand(window);

		}

		private void UpdateJobData()
		{
			SavedJobEntry.Body.PrimaryVehicle = JobType.SingleBusJob == JobType ? FirstFilePath : null; ;
			SavedJobEntry.Body.PrimaryVehicleResults = JobType.CompletedBusJob == JobType ? FirstFilePath : null;
			SavedJobEntry.Body.CompletedVehicle = SecondFilePath;
		}


		private string OpenFileSelector(JobFileType jobFileType, string textPropertyName, string filePath)
		{
			var folderPath = GetFolderPath(filePath);

			var dialogResult = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.XMLFilter, folderPath);
			if (dialogResult != null) {

				filePath = dialogResult.FirstOrDefault();
				var validationResult = IsValideXml(jobFileType, filePath);

				if (!validationResult)
					AddPropertyError(textPropertyName, $"Selected XML-File is not a valid {jobFileType.GetLable()}!");
				else
					RemovePropertyError(textPropertyName);

				return !validationResult ? null : filePath;
			}
			
			return filePath;
		}

		private string GetFolderPath(string filePath)
		{
			if (!_editJob || filePath.IsNullOrEmpty())
				return Settings.XmlFilePathFolder;
			
			if (IsFileName(filePath)) {
				return !JobEntry.JobEntryFilePath.IsNullOrEmpty()
					? Path.GetDirectoryName(JobEntry.JobEntryFilePath)
					: Path.GetDirectoryName(Settings.XmlFilePathFolder);
			}

			return filePath;
		}
		
		private bool IsFileName( string filePath)
		{
			return !Directory.Exists(filePath);
		}
		
		private bool IsValideXml(JobFileType jobFileType, string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return false;

			var xmlInputReader = Kernel.Get<IXMLInputDataReader>();

			using (var reader = XmlReader.Create(filePath))
			{

				var readerResult = xmlInputReader.Create(reader);
				if (readerResult is IDeclarationInputDataProvider)
				{

					var inputData = readerResult as IDeclarationInputDataProvider;
					if (jobFileType == JobFileType.CompletedBusFile &&
						inputData.JobInputData.Vehicle is XMLDeclarationCompletedBusDataProviderV26)
					{
						return true;
					}
					if (jobFileType == JobFileType.PrimaryBusFile &&
						inputData.JobInputData.Vehicle is XMLDeclarationPrimaryBusVehicleDataProviderV26)
					{
						return true;
					}
				}
				else if (readerResult is IPrimaryVehicleInformationInputDataProvider)
				{
					var inputData = readerResult as IPrimaryVehicleInformationInputDataProvider;
					if (jobFileType == JobFileType.PIFBusFile &&
						inputData.Vehicle is XMLDeclarationPrimaryVehicleBusDataProviderV01)
						return true;
				}
				return false;
			}
		}
	}
}

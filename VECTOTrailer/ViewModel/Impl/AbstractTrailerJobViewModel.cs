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
using VECTOTrailer.Helper;
using VECTOTrailer.Model;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Impl
{

	public enum JobFileType
	{
		CompletedTrailerFile,
	}

	public static class JobFileTypeHelper
	{
		public static string GetLabel(this JobFileType fileType)
		{
			return "Completed Trailer File";
		}
	}



	public abstract class AbstractTrailerJobViewModel : ViewModelBase, ITrailerJobViewModel
	{
		#region Members

		private JobFileType _firstFileType;
		private JobFileType _secondFileType;
		private JobFileType _thirdFileType;

		private string _firstLabelText;
		private string _secondLabelText;
		private string _thirdLabelText;

		private string _firstFilePath;
		private string _secondFilePath;
		private string _thirdFilePath;

		private ICommand _selectFirstFileCommand;
		private ICommand _selectSecondFileCommand;
		private ICommand _selectThirdFileCommand;

		private ICommand _cancelCommand;
		private ICommand _saveCommand;


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

		public string ThirdFilePath
		{
			get { return _thirdFilePath; }
			set { SetProperty(ref _thirdFilePath, value); }
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

		public string ThirdLabelText
		{
			get { return _thirdLabelText; }
			set { SetProperty(ref _thirdLabelText, value); }
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
		public JobFileType ThirdFileType
		{
			get { return _thirdFileType; }
			set { SetProperty(ref _thirdFileType, value); }
		}

		#endregion

		protected AbstractTrailerJobViewModel(IKernel kernel, JobType jobType)
		{
			Init(kernel, jobType);
			_editJob = false;
		}

		protected AbstractTrailerJobViewModel(IKernel kernel, JobEntry jobEntry)
		{
			Init(kernel, jobEntry.Header.JobType);
			SetJobEntryData(jobEntry);
			SavedJobEntry = jobEntry;
			_editJob = true;
		}

		private void Init(IKernel kernel, JobType jobType)
		{
			SecondLabelText = $"Select {JobFileType.CompletedTrailerFile.GetLabel()}";
			SetFileTypes(jobType);
			Kernel = kernel;
		}

		private void SetFileTypes(JobType jobType)
		{
			JobType = jobType;

			_firstFileType = JobFileType.CompletedTrailerFile;
			_secondFileType = JobFileType.CompletedTrailerFile;
			_thirdFileType = JobFileType.CompletedTrailerFile;
		}

		private void SetJobEntryData(JobEntry jobEntry)
		{
			JobEntry = jobEntry;

			FirstFilePath = jobEntry.Body.CompletedVehicle;
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

		public ICommand SelectThirdFileCommand
		{
			get
			{
				return _selectThirdFileCommand ??
						(_selectThirdFileCommand = new RelayCommand<JobFileType>(DoSelectThirdFileCommand));
			}
		}
		private void DoSelectThirdFileCommand(JobFileType jobFileType)
		{
			ThirdFilePath = OpenFileSelector(jobFileType, nameof(ThirdFilePath), ThirdFilePath);
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
			var jobFilePath = FileDialogHelper.SaveJobFileToDialog();
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

			job.Body = jobBody;


			SerializeHelper.SerializeToFile(jobFilePath, job);
			SavedJobEntry = job;
			DoCancelCommand(window);

		}

		private void UpdateJobData()
		{
			SavedJobEntry.Body.CompletedVehicle = SecondFilePath;
		}


		private string OpenFileSelector(JobFileType jobFileType, string textPropertyName, string filePath)
		{
			//var folderPath = GetFolderPath(filePath);

			var dialogResult = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.XMLFilter, null);
			if (dialogResult != null) {

				filePath = dialogResult.FirstOrDefault();
				var validationResult = IsValideXml(jobFileType, filePath);

				if (!validationResult)
					AddPropertyError(textPropertyName, $"Selected XML-File is not a valid {jobFileType.GetLabel()}!");
				else
					RemovePropertyError(textPropertyName);

				return !validationResult ? null : filePath;
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

				var readerResult = xmlInputReader.CreateDeclarationTrailer(reader); //Create
				if (readerResult is IDeclarationTrailerInputDataProvider)
				{

					var inputData = readerResult;; //as IDeclarationTrailerInputDataProvider;
					if (jobFileType == JobFileType.CompletedTrailerFile &&
						inputData.JobInputData.Trailer is XMLDeclarationVehicleDataProviderV27)
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}

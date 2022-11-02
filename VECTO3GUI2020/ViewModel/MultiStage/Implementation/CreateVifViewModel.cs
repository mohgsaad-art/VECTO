using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Model.Multistage;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public interface ICreateVifViewModel: IDocumentViewModel, IEditViewModel
	{
		bool LoadStageInput(string fileName);
		bool LoadPrimaryInput(string fileName);
		bool? IsPrimaryExempted { get; set; }
		bool? IsStageInputExempted { get; set; }
		string PrimaryInputPath { get; set; }
		string StageInputPath { get; set; }
		IRelayCommand RemoveStageInputCommand { get; }
		IRelayCommand RemovePrimaryCommand { get; }
		string SaveJob(string path);
	}
    public class CreateVifViewModel : ViewModelBase, ICreateVifViewModel
	{
		private string _primaryInputPath;
		private string _stageInputPath;
		private readonly IDialogHelper _dialogHelper;
		private readonly IXMLInputDataReader _inputDataReader;
		private static uint _newVifCounter = 0;
		private readonly uint _newVifCount;

		private bool? _isPrimaryExempted;

		public bool? IsPrimaryExempted
		{
			get => _isPrimaryExempted;
			set => SetProperty(ref _isPrimaryExempted, value);
		}

		private bool? _isStageInputExempted;

		public bool? IsStageInputExempted
		{
			get => _isStageInputExempted;
			set => SetProperty(ref _isStageInputExempted, value);
		}

		private bool _completed;
		public bool Completed
		{
			get => _completed;
			set => SetProperty(ref _completed, value);
		}

		#region Labeling

		private  string _vifType;

		private string VifType
		{
			get
			{
				return Completed ? "Completed Job" : "Primary Job with Interim Input";
			}
			set => SetProperty(ref _vifType, value);
		}

		#endregion

		public bool UnsavedChanges => _backingStorage.UnsavedChanges;


		public CreateVifViewModel(IDialogHelper dialogHelper, 
			IXMLInputDataReader inputDataReader, 
			IAdditionalJobInfoViewModel additionalJobInfo)
		{
			SizeToContent = SizeToContent.WidthAndHeight;

			_newVifCount = ++_newVifCounter;
			_dialogHelper = dialogHelper;
			_inputDataReader = inputDataReader;
			_additionalJobInfo = additionalJobInfo;
			SetupBackingStorage();
			additionalJobInfo.SetParent(this);


			UpdateTitleAndDocumentName();
			(this as INotifyPropertyChanged).PropertyChanged += CreateVifViewModel_PropertyChanged;
		}

		private void SetupBackingStorage()
		{
			_backingStorage = new BackingStorage<CreateVifViewModel>(this,
				nameof(this.PrimaryInputPath),
				nameof(this.StageInputPath));
			_backingStorage.PropertyChanged += (object s, PropertyChangedEventArgs e) => {
				OnPropertyChanged(nameof(UnsavedChanges));
			};
		}

		private void CreateVifViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if (e.PropertyName != nameof(CanBeSimulated)) {
				OnPropertyChanged(nameof(CanBeSimulated));
			}
			switch (e.PropertyName) {
				case nameof(DataSource):
				case nameof(Completed):
				case nameof(UnsavedChanges):
					UpdateTitleAndDocumentName();
					break;
				default:
					break;
			}
        }

        public CreateVifViewModel(IInputDataProvider inputData, 
			IDialogHelper dialogHelper, 
			IXMLInputDataReader inputDataReader, 
			IAdditionalJobInfoViewModel additionalJobInfo) : this(dialogHelper, inputDataReader, additionalJobInfo)
		{
			SetInputData(inputData);
			_backingStorage.SaveChanges();
		}

		public CreateVifViewModel(bool completed, IDialogHelper dialogHelper, IXMLInputDataReader inputDataReader,
			IAdditionalJobInfoViewModel additionalJobInfo) : this(dialogHelper, inputDataReader, additionalJobInfo)
		{
			_completed = completed;
		}


		private void SetInputData(IInputDataProvider inputData)
		{
			var inputDataProvider = inputData as JSONInputDataV10_PrimaryAndStageInputBus;
			Debug.Assert(inputDataProvider != null);

			try {
				if (inputDataProvider.StageInputData != null && (inputDataProvider.StageInputData.ExemptedVehicle !=
																inputDataProvider.PrimaryVehicle.JobInputData.Vehicle
																	.ExemptedVehicle)) {
					throw new VectoException("Can't combine exempted and non-exempted input data");
				}

				StageInputPath = inputDataProvider.StageInputData?.DataSource?.SourceFile;
				PrimaryInputPath = inputDataProvider.PrimaryVehicle?.DataSource?.SourceFile;
			} catch (Exception ex) {
				_dialogHelper.ShowErrorMessage(ex.Message);
			}
			

			Completed = inputDataProvider.Completed ?? false;

			DataSource = inputData.DataSource;
			UpdateTitleAndDocumentName();
		}

		private void UpdateTitleAndDocumentName()
		{
			var titleStringBuilder = new StringBuilder();
			titleStringBuilder.Append("Create ").Append(VifType);
			if (DataSource != null) {
				titleStringBuilder.Append(" - ").Append(Path.GetFileName(DataSource.SourceFile));
			}

			titleStringBuilder.Append(UnsavedChanges ? "*" : "");
			Title = titleStringBuilder.ToString();
			DocumentName = Path.GetFileNameWithoutExtension(_dataSource?.SourceFile) ?? $"New {VifType} {_newVifCount}";
		}


		public string PrimaryInputPath
		{
			get => _primaryInputPath;
			set
			{
				if (SetProperty(ref _primaryInputPath, value)) {
					OnPropertyChanged(nameof(CanBeSimulated));
					_removePrimaryCommand?.NotifyCanExecuteChanged();
				}
			}
		}

		public string StageInputPath
		{
			get => _stageInputPath;
			set
			{
				if (SetProperty(ref _stageInputPath, value))
				{
					OnPropertyChanged(nameof(CanBeSimulated));
					_removeStageInputCommand?.NotifyCanExecuteChanged();
				}
			}
		}

		#region Commands

		private ICommand _selectPrimaryInputFileCommand;
		private ICommand _selectCompletedInputFileCommand;
		public ICommand SelectCompletedInputFileCommand
		{
			get => _selectCompletedInputFileCommand ?? (_selectCompletedInputFileCommand = new RelayCommand(() => {
				var selectedFile = _dialogHelper.OpenXMLFileDialog();
				LoadStageInput(selectedFile);


			}));
		}

		public ICommand SelectPrimaryInputFileCommand
		{
			get => _selectPrimaryInputFileCommand ?? (_selectPrimaryInputFileCommand = new RelayCommand(() => {
				var selectedFilePath = _dialogHelper.OpenXMLFileDialog();
				LoadPrimaryInput(selectedFilePath);

			}));
		}

		private IRelayCommand _saveJobCommand;

		public ICommand SaveJobCommand
		{
			get => _saveJobCommand ?? (_saveJobCommand = new RelayCommand(() => {
				if (_dataSource.SourceFile != null) {
					if (CanBeSaved()) {
						SaveJob(_dataSource.SourceFile);
					}
				}
			}, () => DataSource != null));
		}
		public IRelayCommand _removeStageInputCommand;

		public IRelayCommand RemoveStageInputCommand =>
			_removeStageInputCommand ?? (_removeStageInputCommand = new RelayCommand(() => {
				StageInputPath = null;
				IsStageInputExempted = null;

			}, () => StageInputPath != null));




		public IRelayCommand _removePrimaryCommand;

		public IRelayCommand RemovePrimaryCommand => 
			_removePrimaryCommand ?? (_removePrimaryCommand = new RelayCommand(() => {
				PrimaryInputPath = null;
				IsPrimaryExempted = null;

			}, () => PrimaryInputPath != null));


		private bool CanBeSaved()
		{
			if (_primaryInputPath == null) {
				_dialogHelper.ShowMessageBox("At least Primary Vehicle has to be provided", "Info", MessageBoxButton.OK,
					MessageBoxImage.Information);
				return false;
			}

			return true;
		}

		public string SaveJob(string path)
		{
			if (path == null) {
				return null;
			}
			
			
			var jsonJob = new JSONJob() {
				Header = new JSONJobHeader() {
					AppVersion = "Vecto3GUI2020",
					CreatedBy = Environment.UserName,
					Date = DateTime.Today,
					FileVersion = JSONJobHeader.PrimaryAndInterimVersion
				},
				Body = new JSONJobBody() {
					PrimaryVehicle = PathHelper.GetRelativePath(path, PrimaryInputPath),
					InterimStep = PathHelper.GetRelativePath(path, StageInputPath)
				}
			};

			string jsonString = JsonConvert.SerializeObject(jsonJob, Formatting.Indented);

			
			Debug.WriteLine(jsonString);
			File.WriteAllText(path, jsonString);
			SetInputData(JSONInputDataFactory.ReadJsonJob(path));
			_backingStorage.SaveChanges();
			return path;
		}

		private IRelayCommand _saveJobAsCommand;

		public IRelayCommand SaveJobAsCommand
		{
			get => _saveJobAsCommand ?? (_saveJobAsCommand = new RelayCommand(() => {
				if (CanBeSaved()) {
					var path = _dialogHelper.SaveToVectoJobDialog();
					SaveJob(path);
				}
			}));
		}

		public bool LoadStageInput(string fileName)
		{
			if (fileName == null)
			{
				return false;
			}

			var valid = true;
			IVehicleDeclarationInputData vehicleInputData = null;
			try
			{
				var inputData = _inputDataReader.Create(fileName) as IDeclarationInputDataProvider;
				vehicleInputData = inputData.JobInputData.Vehicle;
				valid = (inputData != null) && (vehicleInputData is XMLDeclarationConventionalCompletedBusDataProviderV24) || (vehicleInputData is XMLDeclarationExemptedCompletedBusDataProviderV24);
			}
			catch (Exception e)
			{
				valid = false;
			}

			valid = valid && SetStageInputExempted(vehicleInputData.ExemptedVehicle);

			if (valid)
			{
				StageInputPath = fileName;
			}
			else
			{
				_dialogHelper.ShowMessageBox("Invalid File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}




			return valid;
		}


		public bool LoadPrimaryInput(string fileName)
		{
			if (fileName == null)
			{
				return false;
			}

			var valid = true;
			IDeclarationInputDataProvider inputData = null;
			try
			{
				inputData = _inputDataReader.Create(fileName) as IDeclarationInputDataProvider;
				valid = inputData != null && inputData.JobInputData.Vehicle.VehicleCategory.IsBus();
			}
			catch (Exception ex)
			{
				valid = false;
			}

			valid = valid && SetPrimaryInputExempted(inputData.JobInputData.Vehicle.ExemptedVehicle);



			if (valid)
			{
				PrimaryInputPath = fileName;
			}
			else
			{
				_dialogHelper.ShowMessageBox("Invalid File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}


			return valid;
		}

		private bool SetPrimaryInputExempted(bool primaryExempted)
		{
			var valid = IsStageInputExempted == null || IsStageInputExempted == primaryExempted;

			if (valid)
			{
				IsPrimaryExempted = primaryExempted;
			}
			else
			{
				_dialogHelper.ShowMessageBox(
					caption: "Error",
					button: MessageBoxButton.OK,
					icon: MessageBoxImage.Error,
					messageBoxText: (primaryExempted
						? "Exempted primary vehicle not allowed for non-exempted interim/completed input"
						: "Only exempted input allowed for exempted interim/completed input"));
			}
			return valid;
		}

		private bool SetStageInputExempted(bool stageInputExempted)
		{
			var valid = IsPrimaryExempted == null || IsPrimaryExempted == stageInputExempted;

			if (valid)
			{
				IsStageInputExempted = stageInputExempted;
			}
			else
			{
				_dialogHelper.ShowMessageBox(
					caption: "Error",
					button: MessageBoxButton.OK,
					icon: MessageBoxImage.Error,
					messageBoxText: (stageInputExempted
						? "Exempted interim/complete input is invalid for non-exempted primary vehicle"
						: "Only exempted input allowed for selected primary vehicle"));
			}
			return valid;
		}


		#endregion

		#region Implementation of IDocumentViewModel
		private bool _selected;
		private string _documentName;
		private DataSource _dataSource;
		private IAdditionalJobInfoViewModel _additionalJobInfo;
		private BackingStorage<CreateVifViewModel> _backingStorage;


		public string DocumentName
		{
			get => _documentName;
			set => SetProperty(ref _documentName, value);
		}

		//Remove this from
		public XmlDocumentType? DocumentType => null;

		public string DocumentTypeName => "New VIF";

		public DataSource DataSource
		{
			get => _dataSource;
			set
			{
				if (SetProperty(ref _dataSource, value)) {
					_saveJobCommand?.NotifyCanExecuteChanged();
				}
			}
		}

		public IEditViewModel EditViewModel => this;

		public bool Selected
		{
			get => _selected && CanBeSimulated;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeSimulated
		{
			get => PrimaryInputPath != null && StageInputPath != null && !UnsavedChanges;
			set => throw new NotImplementedException();
		}

		public IAdditionalJobInfoViewModel AdditionalJobInfoVm
		{
			get => _additionalJobInfo;
			set => SetProperty(ref _additionalJobInfo, value);
		}

		#endregion

		#region Implementation of IEditViewModel

		public string Name => DocumentName;



		#endregion
	}
}

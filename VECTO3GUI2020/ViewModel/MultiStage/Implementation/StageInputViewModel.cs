using System.IO;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class StageInputViewModel : StageViewModelBase, IDocumentViewModel, IJobEditViewModel
	{
		private bool _canBeEdited;
		private DataSource _dataSource;
		private readonly XmlDocumentType _documentType;
		private string _documentName;
		private bool _selected;
		private static uint _newDocumentCounter = 0;

		private StageInputViewModel(IMultiStageViewModelFactory multistageViewModelFactory,
			IAdditionalJobInfoViewModel additionalJobInfoViewModel) : base(multistageViewModelFactory)
		{
			_documentType = XmlDocumentType.DeclarationJobData;
			_additionalJobInfoViewModel = additionalJobInfoViewModel;
			_additionalJobInfoViewModel.SetParent(this);
		}


		public StageInputViewModel(bool exemptedVehicle, IMultiStageViewModelFactory multiStageViewModelFactory, IAdditionalJobInfoViewModel additionalJobInfoViewModel) : this(multiStageViewModelFactory, additionalJobInfoViewModel)
		{
			_vehicleViewModel = _viewModelFactory.CreateStageInputVehicleViewModel(
				exemptedVehicle
				? InterimStageBusVehicleViewModel_v2_8.VERSION_EXEMPTED
				: InterimStageBusVehicleViewModel_v2_8.VERSION) as IMultistageVehicleViewModel;

			Title = $"{GUILabels.Edit_step_input} - New file";

			_documentName = $"New {(exemptedVehicle ? "exempted " : "")}step input {++_newDocumentCounter}";
			Init();
		}

		public StageInputViewModel(IDeclarationInputDataProvider inputData, IMultiStageViewModelFactory multiStageViewModelFactory, IAdditionalJobInfoViewModel additionalJobInfoViewModel) : this(multiStageViewModelFactory,additionalJobInfoViewModel)
		{
			_documentName = inputData.JobInputData.JobName;
			_vehicleViewModel =
				_viewModelFactory.CreateStageInputVehicleViewModel(inputData.JobInputData.Vehicle) as IMultistageVehicleViewModel;
			(_vehicleViewModel as InterimStageBusVehicleViewModel_v2_8).ShowConsolidatedData = false;

			_dataSource = inputData.DataSource;
			VehicleInputDataFilePath = _dataSource.SourceFile;

			Title = $"{GUILabels.Edit_step_input} - {Path.GetFileName(_dataSource.SourceFile)}";
			Init();
		}

		#region Overrides of StageViewModelBase
		/// <summary>
		/// Called in base class after input data is loaded.
		/// </summary>
		/// <param name="loadedInputData"></param>
		protected override void LoadStageInputDataFollowUp(IDeclarationInputDataProvider loadedInputData)
		{
			DataSource = loadedInputData.DataSource;
			VehicleInputDataFilePath = DataSource.SourceFile;
			UpdateTitle();			
			DocumentName = loadedInputData.JobInputData.JobName;
		}

		#endregion

		private void UpdateTitle()
		{
			Title = GUILabels.Edit_step_input + " - " + ((_dataSource?.SourceFile != null)
				? Path.GetFileName(_dataSource.SourceFile)
				: "New file");
		}

		private void Init()
		{
			UpdateTitle();
			Components.Add("vehicle", VehicleViewModel as IViewModelBase);
			Components.Add("auxiliaries", VehicleViewModel.MultistageAuxiliariesViewModel as IViewModelBase);
			Components.Add("airdrag", VehicleViewModel.MultistageAirdragViewModel as IViewModelBase);
			CurrentView = VehicleViewModel as IViewModelBase;

			ShowSaveAndCloseButtons = true;
		}

		#region Implementation of IDocumentViewModel

		public string DocumentName
		{
			get => _documentName;
			set => SetProperty(ref _documentName, value);
		}

		public XmlDocumentType? DocumentType => _documentType;

		public string DocumentTypeName => "Step input";

		public DataSource DataSource
		{
			get => _dataSource;
			set
			{
				SetProperty(ref _dataSource, value);
				UpdateTitle();
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
			get => false;
			set => throw new System.NotImplementedException();
		}
		private IAdditionalJobInfoViewModel _additionalJobInfoViewModel;
		public IAdditionalJobInfoViewModel AdditionalJobInfoVm
		{
			get => _additionalJobInfoViewModel;
			set => SetProperty(ref _additionalJobInfoViewModel, value);
		}

		#endregion

		#region Implementation of IEditViewModel

		public string Name => "Edit Stage Input";

		#endregion
	}
}
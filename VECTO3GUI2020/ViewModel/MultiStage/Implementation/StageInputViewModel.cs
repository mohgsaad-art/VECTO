using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
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
		private DataSource _dataSource;
        private readonly XmlDocumentType _documentType;
        private string _documentName;
        private bool _selected;
        private static uint _newDocumentCounter = 0;


        private CompletedBusArchitecture _architecture;
        private ObservableCollection<CompletedBusArchitecture> _architectureItems =
            new ObservableCollection<CompletedBusArchitecture>(Enum.GetValues(typeof(CompletedBusArchitecture)).Cast<CompletedBusArchitecture>());

        public ObservableCollection<CompletedBusArchitecture> ArchitectureItems
        {
            get => _architectureItems;
            set => SetProperty(ref _architectureItems, value);
        }

        public CompletedBusArchitecture Architecture
        {
            get => _architecture;
            set
            {
                if (SetProperty(ref _architecture, value))
                {
                    UpdateVehicleViewModel();
                    OnPropertyChanged(nameof(SwitchComponentViewCommand));
                }
            }
        }

        private void UpdateVehicleViewModel()
        {
			if (VehicleViewModel == null) {
				return;
			}
            if (VehicleViewModel.ShowConsolidatedData)
            {
                throw new VectoException("This is only intended on \"standalone\" step inputs");
            }
            var oldVm = _vehicleViewModel;
            var newVm = _viewModelFactory.GetInterimStageVehicleViewModel(Architecture);
            newVm.SetVehicleInputData(oldVm,false);
            VehicleViewModel = newVm;
			UpdateViewModelsAndTitle();
		}


        private StageInputViewModel(IMultiStageViewModelFactory multistageViewModelFactory,
            IAdditionalJobInfoViewModel additionalJobInfoViewModel) : base(multistageViewModelFactory)
        {
            _documentType = XmlDocumentType.DeclarationJobData;
            _additionalJobInfoViewModel = additionalJobInfoViewModel;
            _additionalJobInfoViewModel.SetParent(this);
        }


        public StageInputViewModel(bool exemptedVehicle, IMultiStageViewModelFactory multiStageViewModelFactory, IAdditionalJobInfoViewModel additionalJobInfoViewModel) : this(multiStageViewModelFactory, additionalJobInfoViewModel)
        {
            Architecture = exemptedVehicle
                ? CompletedBusArchitecture.Exempted
                : CompletedBusArchitecture.Conventional;


            _vehicleViewModel = multiStageViewModelFactory.GetInterimStageVehicleViewModel(Architecture);



            Title = $"{GUILabels.Edit_step_input} - New file";

            _documentName = $"New {(exemptedVehicle ? "exempted " : "")}step input {++_newDocumentCounter}";

            UpdateViewModelsAndTitle();
            return;
        }

        public StageInputViewModel(IDeclarationInputDataProvider inputData, IMultiStageViewModelFactory multiStageViewModelFactory, IAdditionalJobInfoViewModel additionalJobInfoViewModel) : this(multiStageViewModelFactory, additionalJobInfoViewModel)
        {
            _documentName = inputData.JobInputData.JobName;
            
			_vehicleViewModel = _viewModelFactory.GetInterimStageVehicleViewModel(inputData.JobInputData.Vehicle);
            //_vehicleViewModel =
            //	_viewModelFactory.CreateStageInputVehicleViewModel(inputData.JobInputData.Vehicle) as IMultistageVehicleViewModel;
            //	(_vehicleViewModel as InterimStageBusVehicleViewModel).ShowConsolidatedData = false;
			_architecture = _vehicleViewModel.Architecture;
            _dataSource = inputData.DataSource;
            VehicleInputDataFilePath = _dataSource.SourceFile;

            Title = $"{GUILabels.Edit_step_input} - {Path.GetFileName(_dataSource.SourceFile)}";
            UpdateViewModelsAndTitle();
            return;
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
            Title = GUILabels.Edit_step_input + $" - {Architecture} - " + ((_dataSource?.SourceFile != null)
                ? Path.GetFileName(_dataSource.SourceFile)
                : $"New file");
        }

        private void UpdateViewModelsAndTitle()
        {
            UpdateTitle();
			var prevKey = Components.FirstOrDefault(kv => kv.Value == CurrentView).Key;
            Components["vehicle"] = VehicleViewModel as IViewModelBase;
            Components["auxiliaries"] = VehicleViewModel.MultistageAuxiliariesViewModel as IViewModelBase;
            Components["airdrag"] = VehicleViewModel.MultistageAirdragViewModel as IViewModelBase;


			CurrentView = null;
			if (prevKey != null) {
				CurrentView = Components[prevKey];
			}
			CurrentView = CurrentView ?? VehicleViewModel as IViewModelBase;
            
            


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

	public enum CompletedBusArchitecture
	{
		Conventional,
		HEV,
		PEV,
		IEPC,
		FCHV,
		Exempted
	}

	public static class CompleteBusArchitectureHelper
	{
		public static CompletedBusArchitecture GetCompletedBusArchitecture(this VectoSimulationJobType jobType,
			bool exempted = false)
		{
			if (exempted)
			{
				return CompletedBusArchitecture.Exempted;
			}
			switch (jobType)
			{
				case VectoSimulationJobType.ConventionalVehicle:
					return CompletedBusArchitecture.Conventional;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IHPC:
					return CompletedBusArchitecture.HEV;
				case VectoSimulationJobType.BatteryElectricVehicle:
					return CompletedBusArchitecture.PEV;
				case VectoSimulationJobType.IEPC_E:
				case VectoSimulationJobType.IEPC_S:
					return CompletedBusArchitecture.IEPC;
				case VectoSimulationJobType.FCHV:
                case VectoSimulationJobType.FCHV_IEPC:
					return CompletedBusArchitecture.FCHV;
				case VectoSimulationJobType.EngineOnlySimulation:
				default:
					throw new ArgumentOutOfRangeException(nameof(jobType), jobType, null);
			}
        }
	}
}
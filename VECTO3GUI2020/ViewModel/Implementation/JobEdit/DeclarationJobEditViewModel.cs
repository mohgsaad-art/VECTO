using Ninject;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit
{

    /// <summary>
    /// ViewModel for Declaration Jobs. 
    /// </summary>
    public abstract class DeclarationJobEditViewModel : ViewModelBase, IJobEditViewModel, IDeclarationJobInputData
    {
		public string Name => _jobInputData.JobName;
		public override string Title => Name;

		private IComponentViewModel _currentDetailView;
        public IComponentViewModel CurrentDetailView 
        { 
            get 
            { 
                return _currentDetailView; 
            } 
            set 
            {
                _currentDetailView = value;
                OnPropertyChanged();
            }
        }

        protected IDeclarationJobInputData _jobInputData;
		protected IDeclarationInputDataProvider _inputData;
		private IXMLWriterFactory _xmlWriterFactory;

        protected IComponentViewModelFactory _componentViewModelFactory;

        
		public ObservableCollection<IComponentViewModel> ComponentViewModels { get; protected set; } = new ObservableCollection<IComponentViewModel>();

        protected IVehicleViewModel _vehicleViewModel;
		public IVehicleViewModel VehicleViewModel
		{
			get => _vehicleViewModel;
			set => SetProperty(ref _vehicleViewModel, value);
		}

		public DeclarationJobEditViewModel(IDeclarationInputDataProvider inputData,
			IComponentViewModelFactory componentViewModelFactory,
			IXMLWriterFactory xmlWriterFactory,
			IDialogHelper dialogHelper)
        {
			_xmlWriterFactory = xmlWriterFactory;
            _componentViewModelFactory = componentViewModelFactory;
			_dialogHelper = dialogHelper;

			_jobInputData = inputData.JobInputData;
			_inputData = inputData;

			DataSource = inputData.DataSource;


			VehicleViewModel = _componentViewModelFactory.CreateVehicleViewModel(_jobInputData.Vehicle);
            CurrentDetailView = VehicleViewModel;

			ComponentViewModels.Add(VehicleViewModel);
			AddVehicleComponentsToCollection();
		}
		
		protected void AddVehicleComponentsToCollection()
        {
			
			foreach(var component in VehicleViewModel.ComponentViewModels)
            {
                ComponentViewModels.Add(component);
            }
        }


        #region Commands

        private ICommand _saveCommand;
		private ICommand _saveAsCommand;
		private DataSource _dataSource;
		private IDialogHelper _dialogHelper;
		private VectoSimulationJobType _jobType;


		private void UpdateDataSource(string filename)
		{
			DataSource.SourceFile = filename;
			OnPropertyChanged(nameof(DataSource.SourceFile));
		}

		public ICommand SaveCommand =>
			_saveCommand ?? new RelayCommand(
				SaveExecute, () => true);

		public ICommand SaveAsCommand => _saveAsCommand ?? new RelayCommand(
			SaveAsExecute, () => true);

		private void SaveAsExecute()
		{
			var filename = _dialogHelper.SaveToXMLDialog(DataSource.SourcePath);
			Save(filename);
			UpdateDataSource(filename);
		}


		private void SaveExecute()
		{
			var filename = _dataSource.SourceFile;
			var dialogResult = _dialogHelper.ShowMessageBox(Strings.SaveExecute_Do_you_want_to_overwrite + filename + "?", "Save",
				MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (dialogResult == MessageBoxResult.No) {
				return;
			}

			Save(filename);
		}


		private void Save(string filename)
		{
			if (filename == null) {
				return;
			}
			var document = _xmlWriterFactory.CreateJobWriter(this).GetDocument();

			document.Save(filename, SaveOptions.OmitDuplicateNamespaces);
		}
		
		#endregion

		public DataSource DataSource
		{
			get => _dataSource;
			set => SetProperty(ref _dataSource, value);
		}

		public bool SavedInDeclarationMode => _jobInputData.SavedInDeclarationMode;

		public IVehicleDeclarationInputData Vehicle => _vehicleViewModel;

		public string JobName => Name;

		public string ShiftStrategy => throw new NotImplementedException();

		public VectoSimulationJobType JobType => _jobType;
	}

    public class DeclarationJobEditViewModel_v1_0 : DeclarationJobEditViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationInputDataProviderV10).FullName;
        
        public DeclarationJobEditViewModel_v1_0
		(IDeclarationInputDataProvider inputData,
			IComponentViewModelFactory componentViewModelFactory,
			IXMLWriterFactory xmlWriterFactory,
			IDialogHelper dialogHelper):
			base(inputData, 
                componentViewModelFactory,
				xmlWriterFactory,
				dialogHelper)
        {
			//ComponentViewModels.Add(VehicleViewModel.PTOViewModel);
		}
    }

    public class DeclarationJobEditViewModel_v2_0 : DeclarationJobEditViewModel_v1_0
    {
        public new static readonly string VERSION = typeof(XMLDeclarationInputDataProviderV20).FullName;

        public DeclarationJobEditViewModel_v2_0(
            IDeclarationInputDataProvider inputData, 
            IComponentViewModelFactory componentViewModelFactory,
			IXMLWriterFactory xmlWriterFactory,
            IDialogHelper dialogHelper) : 
            base(inputData, 
                componentViewModelFactory,
				xmlWriterFactory,
				dialogHelper)
        {
            //ComponentViewModels.Add(VehicleViewModel.PTOViewModel);
        }
    }

}

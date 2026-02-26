using System;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class NewMultiStageJobViewModel : ViewModelBase
	{
		private readonly Settings _settings = Settings.Default;
		private string _vifPath;
		private IDialogHelper _dialogHelper;
		private IXMLInputDataReader _inputDataReader;
		private IMultiStageViewModelFactory _vmFactory;
		private IMultiStageJobViewModel _multistageJobViewModel;
		public string VifPath { get => _vifPath; set => SetProperty(ref _vifPath, value); }


		public IMultiStageJobViewModel MultiStageJobViewModel
		{
			get { return _multistageJobViewModel; }
			set
			{
				OnPropertyChanged(nameof(MultiStageJobViewModel));
				SetProperty(ref _multistageJobViewModel, value);
			}
		}



		public NewMultiStageJobViewModel(IDialogHelper dialogHelper, 
			IXMLInputDataReader inputDataReader, 
			IMultiStageViewModelFactory vmFactory,
			IJobListViewModel jobListViewModel)
		{
			_inputDataReader = inputDataReader;
			_dialogHelper = dialogHelper;
			_vmFactory = vmFactory;
			_jobListViewModel = jobListViewModel;
			Title = GUILabels.New_Multistep_File;
			VifPath = "Select VIF File";
		}



		#region AddVifCommand

		public ICommand AddVifFileCommand
		{
			get => new RelayCommand(AddVifFileExecute, () => true);
		}

		private void AddVifFileExecute()
		{
			
			var fileName = _dialogHelper.OpenXMLFileDialog();
			AddVifFile(fileName);
		}

		internal void AddVifFile(string fileName)
		{
			if (fileName == null)
			{
				return;
			}
			IMultistepBusInputDataProvider inputDataProvider = null;
			try
			{
				inputDataProvider = _inputDataReader.Create(fileName) as IMultistepBusInputDataProvider;
			}
			catch (Exception e)
			{
				_dialogHelper.ShowMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			if (inputDataProvider == null)
			{
				_dialogHelper.ShowMessageBox("invalid input file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			MultiStageJobViewModel = null;
			MultiStageJobViewModel =
				_vmFactory.GetMultiStageJobViewModel(inputDataProvider);
			_jobListViewModel.AddJob(MultiStageJobViewModel);
			VifPath = fileName;
		}


		private readonly IJobListViewModel _jobListViewModel;

		public new ICommand CloseWindow
		{
			get => new RelayCommand<Window>(window => base.CloseWindow(window, _dialogHelper, false),
				(window) => MultiStageJobViewModel == null);
		}



		#endregion

	}
}

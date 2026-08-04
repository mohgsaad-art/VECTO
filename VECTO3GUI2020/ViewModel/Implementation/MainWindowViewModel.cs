using System;
using System.Collections.Generic;
using VECTO3GUI2020.ViewModel.Interfaces;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using TUGraz.VectoCore.Utils;

namespace VECTO3GUI2020.ViewModel.Implementation
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        #region Members
        private IJobListViewModel _jobListVm;
        
		private Dictionary<string, IMainViewModel> _viewModels = new Dictionary<string, IMainViewModel>(StringComparer.InvariantCultureIgnoreCase);
		private readonly AboutViewModel _aboutViewModel;
		private IWindowHelper _windowHelper;
		private ISettingsViewModel _settingsViewModel;
		private IMainViewModel _currentViewModelTop;
		#endregion

		public MainWindowViewModel(IWindowHelper windowHelper, ISettingsViewModel settingsViewModel, IJobListViewModel jobListViewModel, AboutViewModel aboutVm)
		{
			_windowHelper = windowHelper;
			_settingsViewModel = settingsViewModel;
			_jobListVm = jobListViewModel;
			_aboutViewModel = aboutVm;
			_currentViewModelTop = _jobListVm;

			//_bottomView = new TestViewModel();
            _viewModels.Add("Jobs", _jobListVm);
            _viewModels.Add("Settings", _settingsViewModel);
			_viewModels.Add("About", _aboutViewModel);
		}


		#region Properties
		public void SwitchTopViewModel(string key)
		{
			CurrentViewModel = _viewModels[key];
		}

		public bool JobsSelected => CurrentViewModel == _jobListVm;

		public bool SettingsSelected => CurrentViewModel == _settingsViewModel;

		public bool AboutSelected => CurrentViewModel == _aboutViewModel;

		public IMainViewModel CurrentViewModel
        {
            get { 
				return _currentViewModelTop;
			}
			set {
				if (SetProperty(ref _currentViewModelTop, value)) {
					OnPropertyChanged(nameof(JobsSelected));
					OnPropertyChanged(nameof(SettingsSelected));
					OnPropertyChanged(nameof(AboutSelected));
				}
			}
		}
		public IJobListViewModel JobListVm
		{
			get => _jobListVm;
			set => SetProperty(ref _jobListVm, value);
		}


		public string Version => "VECTO Multistep " + VectoSimulationCore.VersionNumber;

		#endregion

		#region Commands

		private ICommand _switchTopView;
		public ICommand OpenSettings
        {
            get{
                return (ICommand)new RelayCommand(OpenSettingsExecute);
            }
            private set
            {

            }
        }

        private void OpenSettingsExecute()
		{
            _windowHelper.ShowWindow(_settingsViewModel);
		}

		public ICommand SwitchTopView
		{
			get
			{
				return _switchTopView ?? (_switchTopView = new RelayCommand<string>(SwitchTopViewModel, (s) => true));
			}
		}



#endregion

	}
}
	
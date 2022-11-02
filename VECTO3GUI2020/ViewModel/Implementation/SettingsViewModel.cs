using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace VECTO3GUI2020.ViewModel.Implementation
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private Settings _settings;

        private ICommand _changePath;
		private ICommand _changeOutPath;

		private IDialogHelper _dialogHelper;


		private string _defaultFilePath;
		private string _defaultOutputPath;
		private bool _writeModalResults;
		private bool _modalResults1Hz;
		private bool _validate;
		private bool _actualModalData;
		private bool _serializeVectoRunData;

		public string DefaultFilePath
        {
            get => _defaultFilePath;
            set
            {
                _settings.DefaultFilePath = value;
				_settings.Save();
                SetProperty(ref _defaultFilePath, value, "DefaultFilePath");
            }
        }

		public string DefaultOutputPath {
			get => _defaultOutputPath;
			set {
				_settings.DefaultOutputPath = value;
				_settings.Save();
				SetProperty(ref _defaultOutputPath, value, "DefaultOutputPath");
			}
		}

		public SettingsViewModel(IDialogHelper dialogHelper)
		{
			base.Title = "Settings";
            _settings = Settings.Default;
            _defaultFilePath = _settings.DefaultFilePath;
			_writeModalResults = _settings.WriteModalResults;
			_modalResults1Hz = _settings.ModalResults1Hz;
			_validate = _settings.Validate;
			_actualModalData = _settings.ActualModalData;
			_serializeVectoRunData = _settings.SerializeVectoRunData;
			_dialogHelper = dialogHelper;
			_defaultOutputPath = _settings.DefaultOutputPath;
		}

		private ICommand _closeWindowCommand;

		public ICommand CloseWindowCommand
		{
			get
			{
				return _closeWindowCommand ?? new RelayCommand<Window>(window => CloseWindow(window, _dialogHelper, false), window => true);
			}
		}

		public ICommand ChangeFilePath
        {
            get
            {
                return _changePath ?? new RelayCommand(()=>
                {


                    //C:\Users\Harry\source\repos\vecto-gui\VectoCore\VectoCoreTest\TestData\XML\XMLReaderDeclaration

					
                    var new_path = _dialogHelper.OpenFolderDialog(DefaultFilePath);

                    if (new_path != null)
                    {
						DefaultFilePath = new_path;
                    }


                }, () => { return true; });
            }
            private set
            {
                _changePath = value;
                OnPropertyChanged();
            }
        }

		public ICommand ChangeOutputPath {
			get {
				return _changeOutPath ?? new RelayCommand(() => {
					var new_path = _dialogHelper.OpenFolderDialog(DefaultFilePath);
					if (new_path != null) {
						DefaultOutputPath = new_path;
					}
				}, () => { return true; });
			}
			private set {
				_changeOutPath = value;
				OnPropertyChanged();
			}
		}

		public bool SerializeVectoRunData
		{
			get => _serializeVectoRunData;
			set
			{
				if (SetProperty(ref _serializeVectoRunData, value)) {
					_settings.SerializeVectoRunData = value;
					_settings.Save();
				}
			}
		}

		public bool ActualModalData
		{
			get => _actualModalData;
			set
			{
				if (SetProperty(ref _actualModalData, value)) {
					_settings.ActualModalData = value;
					_settings.Save();

				}
			}
		}

		public bool Validate
		{
			get => _validate;
			set
			{
				if (SetProperty(ref _validate, value))
				{
					_settings.Validate = value;
					_settings.Save();

				}
			}
		}

		public bool ModalResults1Hz
		{
			get => _modalResults1Hz;
			set
			{
				if (SetProperty(ref _modalResults1Hz, value))
				{
					_settings.ModalResults1Hz = value;
					_settings.Save();

				}
			}
		}

		public bool WriteModalResults
		{
			get => _writeModalResults;
			set
			{
				if (SetProperty(ref _writeModalResults, value))
				{
					_settings.WriteModalResults = value;
					_settings.Save();

				}
			}
		}


	}
}

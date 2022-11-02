using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TUGraz.VectoCore.Configuration;
using VECTO3GUI.Helper;
using VECTO3GUI.Model;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Impl
{
	public class SettingsViewModel : ObservableObject
	{
		#region Members

		private readonly SettingsModel _settings;
		private string _savePathFolder;
		private string _xmlFilePath;

		private ICommand _saveCommand;
		private ICommand _cancelCommand;
		private ICommand _resetCommand;
		private ICommand _openSaveFolderCommand;
		private ICommand _openXMLFolderCommand;

		#endregion


		#region Properties

		public string SavePathFolder
		{
			get { return _savePathFolder; }
			set { SetProperty(ref _savePathFolder, value); }
		}

		public string XMLFilePath
		{
			get { return _xmlFilePath; }
			set { SetProperty(ref _xmlFilePath, value); }
		}

		#endregion


		public SettingsViewModel()
		{
			_settings = new SettingsModel();
			SavePathFolder = _settings.SavePathFolder;
			XMLFilePath = _settings.XmlFilePathFolder;
		}

		#region Commands
		public ICommand SaveCommand
		{
			get
			{
				return _saveCommand ??
					  (_saveCommand = new RelayCommand(DoSaveCommand, CanSaveCommand));
			}
		}

		private bool CanSaveCommand()
		{
			return IsDataChanged();
		}

		private void DoSaveCommand()
		{
			_settings.SavePathFolder = SavePathFolder;
			_settings.XmlFilePathFolder = XMLFilePath;
		}

		public ICommand CancelCommand
		{
			get
			{
				return _cancelCommand ??
					  (_cancelCommand = new RelayCommand<Window>(DoCancelCommand));
			}
		}


		private void DoCancelCommand(Window window)
		{
			window?.Close();
		}

		public ICommand ResetCommand
		{
			get
			{
				return _resetCommand ??
					  (_resetCommand = new RelayCommand(DoResetCommand));
			}
		}

		private void DoResetCommand()
		{
			SavePathFolder = _settings.SavePathFolder;
			XMLFilePath = _settings.XmlFilePathFolder;
		}


		public ICommand OpenSaveFolderCommand
		{
			get
			{
				return _openSaveFolderCommand ??
						(_openSaveFolderCommand = new RelayCommand<string>(DoOpenSaveFolderCommand));
			}
		}

		private void DoOpenSaveFolderCommand(string path)
		{
			var pathResult = FileDialogHelper.ShowSelectDirectoryDialog(path);
			if (pathResult != null)
				SavePathFolder = pathResult;
		}

		public ICommand OpenXMLFolderCommand
		{
			get
			{
				return _openXMLFolderCommand ??
						(_openXMLFolderCommand = new RelayCommand<string>(DoOpenXMLFolderCommand));
			}
		}

		private void DoOpenXMLFolderCommand(string path)
		{
			var pathResult = FileDialogHelper.ShowSelectDirectoryDialog(path);
			if (pathResult != null)
				XMLFilePath = pathResult;
		}

		#endregion

		private bool IsDataChanged()
		{
			return SavePathFolder != _settings.SavePathFolder ||
					XMLFilePath != _settings.XmlFilePathFolder;
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Toolkit.Mvvm.Input;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using RelayCommand = VECTO3GUI2020.Util.RelayCommand;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public interface IStageViewModelBase
	{
		string VehicleInputDataFilePath { get; set; }
		IMultistageVehicleViewModel VehicleViewModel { get; set; }
		ICommand SwitchComponentViewCommand { get; }
		IRelayCommand SaveInputDataCommand { get; }
		ICommand SaveInputDataAsCommand { get; }
		ICommand LoadVehicleDataCommand { get; }
		bool ShowSaveAndCloseButtons { get; set; }
		void SaveInputDataExecute(string filename);
		bool LoadStageInputData(string fileName);
	}

	public class StageViewModelBase : ViewModelBase, IStageViewModelBase
	{
		protected Dictionary<string, IViewModelBase> Components =
			new Dictionary<string, IViewModelBase>(StringComparer.CurrentCultureIgnoreCase);

		protected IMultistageVehicleViewModel _vehicleViewModel;
		protected IMultiStageViewModelFactory _viewModelFactory;
		private IViewModelBase _currentview;
		private ICommand _switchComponentViewCommand;

		[Inject]
		public IMultistageDependencies MultistageDependencies
		{
			get => _multistageDependencies;
			set => _multistageDependencies = value;
		}

		[Inject]
		public IXMLInputDataReader InputDataReader
		{
			get => _inputDataReader;
			set => _inputDataReader = value;
		}

		public StageViewModelBase(IMultiStageViewModelFactory viewModelFactory)
		{
			_viewModelFactory = viewModelFactory;
		}

		public IViewModelBase CurrentView
		{
			get => _currentview;
			set => SetProperty(ref _currentview, value);
		}

		public IMultistageVehicleViewModel VehicleViewModel
		{
			get => _vehicleViewModel;
			set => SetProperty(ref _vehicleViewModel, value);
		}

		private bool _showSaveAndCloseButtons = false;

		public bool ShowSaveAndCloseButtons
		{
			get => _showSaveAndCloseButtons;
			set => SetProperty(ref _showSaveAndCloseButtons, value);
		}

		#region Commands

		public ICommand SwitchComponentViewCommand
		{
			get
			{
				return _switchComponentViewCommand ??
						new Util.RelayCommand<string>(SwitchViewExecute, (string s) => SwitchViewCanExecute(s));
			}
		}

		private void SwitchViewExecute(string viewToShow)
		{
			IViewModelBase newView;
			var success = Components.TryGetValue(viewToShow, out newView);
			if (success) {
				CurrentView = newView;
			}
		}

		private bool SwitchViewCanExecute(string viewToShow)
		{
			return Components[viewToShow] != null;
		}

		private IRelayCommand _saveInputDataCommand;
		private ICommand _saveInputDataAsCommand;
		private IMultistageDependencies _multistageDependencies;
		private IXMLInputDataReader _inputDataReader;

		public IRelayCommand SaveInputDataCommand =>
			_saveInputDataCommand ??
			new Microsoft.Toolkit.Mvvm.Input.RelayCommand(() => { SaveInputDataExecute(filename: _vehicleInputDataFilePath); },
				() => _vehicleInputDataFilePath != null);

		public ICommand SaveInputDataAsCommand =>
			_saveInputDataAsCommand ?? new RelayCommand(() => { SaveInputDataExecute(filename: null); }, () => true);


		private ICommand _loadVehicleDataCommand;
		private string _vehicleInputDataFilePath;


		public ICommand LoadVehicleDataCommand
		{
			get { return _loadVehicleDataCommand ?? new RelayCommand(LoadVehicleDataExecute, () => true); }
		}

		#endregion Commands

		#region File I/O

		private void LoadVehicleDataExecute()
		{
			var fileName = _multistageDependencies.DialogHelper.OpenXMLFileDialog();
			if (fileName == null) {
				return;
			}

			LoadStageInputData(fileName);
			return;
		}



		public void SaveInputDataExecute(string filename)
		{
			var dialogHelper = _multistageDependencies.DialogHelper;
			if (VehicleViewModel.HasErrors) {
				var errorMessage = "Vehicle:\n";
				var vehicleErrorInfo = VehicleViewModel as IDataErrorInfo;
				errorMessage += vehicleErrorInfo.Error.Replace(",", "\n");


				if (VehicleViewModel.MultistageAuxiliariesViewModel is IDataErrorInfo auxiliariesErrorInfo &&
					!string.IsNullOrEmpty(auxiliariesErrorInfo.Error)) {
					errorMessage += "\nAuxiliaries:\n";
					errorMessage += auxiliariesErrorInfo.Error.Replace(",", "\n");
				}

				dialogHelper.ShowMessageBox(errorMessage, "Error", MessageBoxButton.OK,
					MessageBoxImage.Error);
				return;
			}


			if (filename == null) {
				filename = dialogHelper.SaveToXMLDialog(Settings.Default.DefaultFilePath);
				if (filename == null) {
					return;
				}
			}

			var vehicleWriter =
				_multistageDependencies.XMLWriterFactory.CreateVehicleWriter(VehicleViewModel);



			var xElement = vehicleWriter.GetElement();
			var xDoc = xElement.CreateWrapperDocument(XMLNamespaces.v2_10_2);
			Debug.WriteLine(xElement.CreateWrapperDocument(XMLNamespaces.v2_10_2).ToString());


			var valid = false;
			var validationError = "";
			try {
				var validator = new XMLValidator(xDoc.ToXmlDocument());
				valid = validator.ValidateXML(XmlDocumentType.DeclarationJobData);
				validationError = validator.ValidationError;
			} catch (Exception e) {
				dialogHelper.ShowMessageBox(messageBoxText: (e.Message + "\n" + e.InnerException),
					caption: "Error saving File");
			}

			if (!valid) {
				dialogHelper.ShowMessageBox($"Invalid Document: {validationError}", "Error");
				var tempFile = Path.GetTempFileName();
				try {
					xDoc.Save(tempFile, SaveOptions.OmitDuplicateNamespaces);
					LoadStageInputData(tempFile);

				} catch (Exception e) {
					dialogHelper.ShowMessageBox(e.Message, "Error");
					throw;
				} finally {
					if (File.Exists(tempFile)) {
						File.Delete(tempFile);
					}

					;
				}


			} else {
				xDoc.Save(filename, SaveOptions.OmitDuplicateNamespaces);
				LoadStageInputData(filename);
			}
		}

		public bool LoadStageInputData(string fileName)
		{
			try
			{
				var inputData = (IDeclarationInputDataProvider)_inputDataReader.Create(fileName);
				var vehicleInputData = inputData.JobInputData.Vehicle;
				VehicleViewModel.SetVehicleInputData(vehicleInputData);
				VehicleInputDataFilePath = inputData.DataSource.SourceFile;
				LoadStageInputDataFollowUp(inputData);


				return true;
			}
			catch (Exception e)
			{
				_multistageDependencies.DialogHelper.ShowMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			return true;
		}

		protected virtual void LoadStageInputDataFollowUp(IDeclarationInputDataProvider loadedInputData)
		{

		}

		public string VehicleInputDataFilePath
		{
			get => _vehicleInputDataFilePath;
			set
			{
				SetProperty(ref _vehicleInputDataFilePath, value);
				_saveInputDataCommand?.NotifyCanExecuteChanged();
			}
		}

		#endregion

	}


}
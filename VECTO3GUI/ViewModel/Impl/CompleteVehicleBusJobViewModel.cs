using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using Castle.Core.Internal;
using MahApps.Metro.Controls.Dialogs;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationJobs;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;
using VECTO3GUI.Util.XML;
using Component = VECTO3GUI.Util.Component;


namespace VECTO3GUI.ViewModel.Impl
{
	public class CompleteVehicleBusJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{
		#region Members

		private readonly XMLCompletedBus _xmlCompletedBus;
		private readonly XMLCompletedBusWriter _xmlCompletedBusWriter;

		private ICommand _resetComponentCommand;
		private ICommand _commitComponentCommand;

		private bool _saveAsButtonVisible;
		private bool _saveButtonVisibility;

		private Dictionary<string, string> _errors;
		private ICommand _validateInputCommand;
		private ICommand _validationErrorsCommand;
		private ICommand _removeValidationErrorsCommand;

		private bool _validationErrorsDisplayed;

		#endregion

		#region Properties

		public Dictionary<Component, object> CompleteVehicleBusData { get; private set; }

		public bool SaveAsButtonVisible
		{
			get { return _saveAsButtonVisible; }
			set { SetProperty(ref _saveAsButtonVisible, value); }
		}

		public bool SaveButtonVisibility
		{
			get { return _saveButtonVisibility; }
			set { SetProperty(ref _saveButtonVisibility, value); }
		}
		#endregion

		public CompleteVehicleBusJobViewModel(IKernel kernel, IDeclarationInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			IsNewJob = inputData == null;
			JobViewModel = this;
			CreateComponentModel(Component.CompleteBusVehicle);
			CreateComponentModel(Component.Airdrag);
			CreateComponentModel(Component.Auxiliaries);
			CurrentComponent = GetComponentViewModel(Component.CompleteBusVehicle);

			SetXmlFilePath(inputData?.JobInputData.Vehicle.XMLSource.BaseURI);

			_xmlCompletedBus = new XMLCompletedBus();
			_xmlCompletedBusWriter = new XMLCompletedBusWriter();

			SetVisibilityOfSaveButtons();

			_errors = new Dictionary<string, string>();
		}

		private void SetVisibilityOfSaveButtons()
		{
			SaveAsButtonVisible = IsNewJob;
			SaveButtonVisibility = !IsNewJob;
		}

		#region Commands

		protected override bool CanSaveJob(Window window)
		{
			return !IsNewJob;
		}
		protected override void DoSaveJob(Window window)
		{
			var dialogSettings = new MetroDialogSettings
			{
				AffirmativeButtonText = "Yes",
				NegativeButtonText = "Cancel",
				AnimateShow = true,
				AnimateHide = true
			};

			var dialogResult = MetroDialogHelper.GetModalDialogBox(this, "Save",
				"The existing file will be overwritten, do you want to continue?",
				MessageDialogStyle.AffirmativeAndNegative, dialogSettings);

			if (dialogResult == MessageDialogResult.Affirmative)
			{
				SetCurrentDataToSave();
				var xDoc = _xmlCompletedBus.GenerateCompletedBusDocument(CompleteVehicleBusData);

				if (XmlHelper.ValidateXDocument(xDoc, null, ValidationErrorAction)) {
					_xmlCompletedBusWriter.WriteCompletedBusXml(XmlFilePath, xDoc);
					CloseWindow(window);
				} else {
					ValidationResultDialogWindow(false);
				}
			}
		}


		protected override void DoCloseJob(Window window)
		{
			if (CloseWindowDialog())
			{
				CloseWindow(window);
			}
		}

		protected override void DoSaveAsJob(Window window)
		{
			var filePath = FileDialogHelper.SaveXmlFileToDialog(SettingsModel.XmlFilePathFolder);
			if (filePath == null)
				return;

			SetCurrentDataToSave();
			var xDocument = _xmlCompletedBus.GenerateCompletedBusDocument(CompleteVehicleBusData);

			if (XmlHelper.ValidateXDocument(xDocument, null, ValidationErrorAction))
			{
				_xmlCompletedBusWriter.WriteCompletedBusXml(filePath, xDocument);
				CloseWindow(window);
			} else {
				ValidationResultDialogWindow(false);
			}
		}

		public ICommand CommitComponent
		{
			get
			{
				return _commitComponentCommand ??
						(_commitComponentCommand = new RelayCommand<Component>(DoCommitComponent, CanCommitComponent));
			}
		}

		private bool CanCommitComponent(Component component)
		{
			return ComponentsChanged(component);
		}

		private void DoCommitComponent(Component component)
		{
			switch (component)
			{
				case Component.CompleteBusVehicle:
					_subModels[Component.CompleteBusVehicle].CommitComponentData();
					break;
				case Component.Airdrag:
					_subModels[Component.Airdrag].CommitComponentData();
					break;
				case Component.Auxiliaries:
					_subModels[Component.Auxiliaries].CommitComponentData();
					break;
			}
		}

		public ICommand ResetComponent
		{
			get
			{
				return _resetComponentCommand ??
					  (_resetComponentCommand = new RelayCommand<Component>(DoResetComponent, CanResetComponent));
			}
		}
		private bool CanResetComponent(Component component)
		{
			return ComponentsChanged(component);
		}
		private void DoResetComponent(Component component)
		{
			switch (component)
			{
				case Component.CompleteBusVehicle:
					_subModels[Component.CompleteBusVehicle].ResetComponentData();
					break;
				case Component.Airdrag:
					_subModels[Component.Airdrag].ResetComponentData();
					break;
				case Component.Auxiliaries:
					_subModels[Component.Auxiliaries].ResetComponentData();
					break;
			}
		}

		public ICommand RemoveValidationErrors
		{
			get
			{
				return _removeValidationErrorsCommand ??
						(_removeValidationErrorsCommand = new RelayCommand(DoRemoveValidationErrors, CanRemoveValidationErrors));
			}
		}
		private bool CanRemoveValidationErrors()
		{
			return _validationErrorsDisplayed && !_errors.IsNullOrEmpty();
		}
		private void DoRemoveValidationErrors()
		{
			ClearValidationErrors();
		}

		#endregion

		private void SetCurrentDataToSave()
		{
			CompleteVehicleBusData = new Dictionary<Component, object> {
				{ Component.CompleteBusVehicle, _subModels[Component.CompleteBusVehicle].CommitComponentData()},
				{ Component.Airdrag, _subModels[Component.Airdrag].CommitComponentData()},
				{ Component.Auxiliaries, _subModels[Component.Auxiliaries].CommitComponentData()}
			};
		}

		private bool ComponentsChanged(Component component)
		{
			switch (component)
			{
				case Component.CompleteBusVehicle:
					return _subModels[Component.CompleteBusVehicle].IsComponentDataChanged();
				case Component.Airdrag:
					return _subModels[Component.Airdrag].IsComponentDataChanged();
				case Component.Auxiliaries:
					return _subModels[Component.Auxiliaries].IsComponentDataChanged();
				default:
					return false;
			}
		}

		private void CloseWindow(Window window)
		{
			WindowAlreadyClosed = true;
			window?.Close();
		}

		public ICommand ValidateInput
		{
			get { return _validateInputCommand ?? (_validateInputCommand = new RelayCommand(DoValidateInput)); }
		}

		private void DoValidateInput()
		{
			ClearValidationErrors();

			_errors = new Dictionary<string, string>();

			SetCurrentDataToSave();
			var xDoc = _xmlCompletedBus.GenerateCompletedBusDocument(CompleteVehicleBusData);

			if (XmlHelper.ValidateXDocument(xDoc, null, ValidationErrorAction)) {
				ValidationResultDialogWindow(true);
			} else {
				ValidationResultDialogWindow(false);
			}
		}

		public ICommand ShowValidationErrors
		{
			get
			{
				return _validationErrorsCommand ?? (_validationErrorsCommand = new RelayCommand(DoShowValidationErrors, CanShowValidationErrors));
			}
		}
		private bool CanShowValidationErrors()
		{
			return !_validationErrorsDisplayed && !_errors.IsNullOrEmpty();
		}
		private void DoShowValidationErrors()
		{
			ClearValidationErrors();

			var completedBusViewModel = _subModels[Component.CompleteBusVehicle] as CompleteVehicleBusViewModel;
			var auxiliaryViewModel = _subModels[Component.Auxiliaries] as AuxiliariesViewModel;

			completedBusViewModel?.ShowValidationErrors(_errors);
			auxiliaryViewModel?.ShowValidationErrors(_errors);

			_validationErrorsDisplayed = true;
		}


		private void ValidationErrorAction(XmlSeverityType arg1, ValidationEvent arg2)
		{
			var xmlException = arg2?.ValidationEventArgs?.Exception as XmlSchemaValidationException;
			if (xmlException != null)
			{
				var message = xmlException.InnerException;
				var sourceObject = xmlException.SourceObject as XmlElement;
				var localName = sourceObject?.LocalName;

				if (sourceObject != null)
					_errors.Add(localName, message?.Message);
			}
		}

		private void ClearValidationErrors()
		{
			if (_errors.IsNullOrEmpty())
				return;


			var completedBusViewModel = _subModels[Component.CompleteBusVehicle] as CompleteVehicleBusViewModel;
			var auxiliaryViewModel = _subModels[Component.Auxiliaries] as AuxiliariesViewModel;

			completedBusViewModel?.RemoveValidationErrors(_errors);
			auxiliaryViewModel?.RemoveValidationErrors(_errors);

			_validationErrorsDisplayed = false;
		}

		private void ValidationResultDialogWindow(bool validationResult)
		{
			if (validationResult) { 
				MetroDialogHelper.GetModalDialogBox(this, "Input Validation",
					"No validation errors were found, the entered data is valid!", MessageDialogStyle.Affirmative);
			} else {
				MetroDialogHelper.GetModalDialogBox(this, "Input Validation",
					"There are some input validation errors, the data must be valid for saving!", MessageDialogStyle.Affirmative);
			}
		}


		public string JobFile { get; }
		public IInputDataProvider InputDataProvider { get; set; }


	}
}

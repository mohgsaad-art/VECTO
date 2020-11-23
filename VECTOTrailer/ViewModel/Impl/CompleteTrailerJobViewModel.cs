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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationJobs;
using TUGraz.VectoCore.Utils;
using VECTOTrailer.Helper;
using VECTOTrailer.ViewModel.Interfaces;
using VECTOTrailer.Util;
using VECTOTrailer.Util.XML;
using Component = VECTOTrailer.Util.Component;


namespace VECTOTrailer.ViewModel.Impl
{
	public class CompleteTrailerJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{
		#region Members

		private readonly XMLCompletedTrailer _xmlCompletedTrailer;
		private readonly XMLCompletedTrailerWriter _xmlCompletedTrailerWriter;

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

		public Dictionary<Component, object> CompleteVehicleTrailerData { get; private set; }

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

		public CompleteTrailerJobViewModel(IKernel kernel, IDeclarationTrailerInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			IsNewJob = inputData == null;
			JobViewModel = this;
			CreateComponentModel(Component.CompleteTrailer);

			CurrentComponent = GetComponentViewModel(Component.CompleteTrailer);

			SetXmlFilePath(inputData?.JobInputData.Trailer.XMLSource.BaseURI);

			_xmlCompletedTrailer = new XMLCompletedTrailer();
			_xmlCompletedTrailerWriter = new XMLCompletedTrailerWriter();

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

			var dialogResult = MessageBox.Show(
				"The existing file will be overwritten, do you want to continue?", "Save",
				MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (dialogResult == MessageBoxResult.Yes)
			{
				SetCurrentDataToSave();
				var xDoc = _xmlCompletedTrailer.GenerateCompletedTrailerDocument(CompleteVehicleTrailerData);

				if (XmlHelper.ValidateXDocument(xDoc, null, ValidationErrorAction)) {
					_xmlCompletedTrailerWriter.WriteCompletedTrailerXml(XmlFilePath, xDoc);
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
			var xDocument = _xmlCompletedTrailer.GenerateCompletedTrailerDocument(CompleteVehicleTrailerData);

			_xmlCompletedTrailerWriter.WriteCompletedTrailerXml(filePath, xDocument);
			/*
			if (XmlHelper.ValidateXDocument(xDocument, null, ValidationErrorAction)) {
				_xmlCompletedTrailerWriter.WriteCompletedTrailerXml(filePath, xDocument);
				CloseWindow(window);
			} else {
				ValidationResultDialogWindow(false);
			}*/
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
			_subModels[Component.CompleteTrailer].CommitComponentData();
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
			_subModels[Component.CompleteTrailer].ResetComponentData();
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
			CompleteVehicleTrailerData = new Dictionary<Component, object> {
				{ Component.CompleteTrailer, _subModels[Component.CompleteTrailer].CommitComponentData()},
			};
		}

		private bool ComponentsChanged(Component component)
		{
			return _subModels[Component.CompleteTrailer].IsComponentDataChanged();
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
			var xDoc = _xmlCompletedTrailer.GenerateCompletedTrailerDocument(CompleteVehicleTrailerData);

			// Aero features combination validation -----------------------------
			var aeroFeatures = new List<AeroFeatureTechnology>();
			if (((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault()).SideSkirtsShort) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsShort);
			if (((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault()).SideSkirtsLong) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsLong);
			if (((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault()).BoatTailShort) aeroFeatures.Add(AeroFeatureTechnology.BoatTailShort);
			if (((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault()).BoatTailLong) aeroFeatures.Add(AeroFeatureTechnology.BoatTailLong);

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);
			if (combination < 0)
				ValidationResultDialogWindow(false);
			// -----------------------------------------------------------------

			// Axles -----------------------------------------------------------
			if (((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.NumberOfAxles == NumberOfTrailerAxles.Two &&
				(((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.Axle2FilePath == null))
				ValidationResultDialogWindow(false);

			else if (((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.NumberOfAxles == NumberOfTrailerAxles.Three &&
				(((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.Axle2FilePath == null ||
				((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.Axle3FilePath == null))
				ValidationResultDialogWindow(false);
			// -----------------------------------------------------------------

			if (((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.TrailerType == TypeTrailer.DA &&
			(((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.TPLMAxleAssembly == null ||
			((Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault())?.TPLMAxleAssembly?.Value() == 0))
			ValidationResultDialogWindow(false);

			else if (XmlHelper.ValidateXDocument(xDoc, null, ValidationErrorAction)) {
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

			var completedTrailerViewModel = _subModels[Component.CompleteTrailer] as CompleteTrailerViewModel;

			completedTrailerViewModel?.ShowValidationErrors(_errors);

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

			var completedTrailerViewModel = _subModels[Component.CompleteTrailer] as CompleteTrailerViewModel;

			completedTrailerViewModel?.RemoveValidationErrors(_errors);

			_validationErrorsDisplayed = false;
		}

		private void ValidationResultDialogWindow(bool validationResult)
		{
			if (validationResult) {
				MessageBox.Show("No validation errors were found, the entered data is valid!", "Input Validation", MessageBoxButton.OK);

			} else {
				MessageBox.Show("There are some input validation errors, the data must be valid for saving!", "Input Validation", MessageBoxButton.OK);
			}
		}


		public string JobFile { get; }
		public IDeclarationTrailerInputDataProvider InputDataProvider { get; set; }


	}
}

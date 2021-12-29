using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using Castle.Core.Internal;
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
			
			if (XmlHelper.ValidateXDocument(xDocument, null, ValidationErrorAction)) {
				_xmlCompletedTrailerWriter.WriteCompletedTrailerXml(filePath, xDocument);
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

		private static bool CheckAeroFeatures(Model.TempDataObject.TrailerComponentData trailer)
		{
			if (trailer.IsCertified &&
				(trailer.YawAngle0 < 0 || 
				trailer.YawAngle3 < 0 ||
				trailer.YawAngle6 < 0 ||
				trailer.YawAngle9 < 0))
				return false;

			var aeroFeatures = new List<AeroFeatureTechnology>();
			if (trailer.SideSkirtsShort) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsShort);
			if (trailer.SideSkirtsLong) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsLong);
			if (trailer.BoatTailShort) aeroFeatures.Add(AeroFeatureTechnology.BoatTailShort);
			if (trailer.BoatTailLong) aeroFeatures.Add(AeroFeatureTechnology.BoatTailLong);

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);
			if (combination < 0 && trailer.IsStandard)
				return false;

			if (trailer.SideSkirtsShort && trailer.SideSkirtsLong)
				return false;
			if (trailer.BoatTailShort && trailer.BoatTailLong)
				return false;

			return true;
		}

		private static bool CheckAxles(Model.TempDataObject.TrailerComponentData trailer)
		{
			var liftableCount = 0;
			if (trailer.Axle1IsLiftable)
				liftableCount++;
			if (trailer.Axle2IsLiftable)
				liftableCount++;
			if (trailer.Axle3IsLiftable)
				liftableCount++;

			var steerCount = 0;
			if (trailer.Axle1IsSteered)
				steerCount++;
			if (trailer.Axle2IsSteered)
				steerCount++;
			if (trailer.Axle3IsSteered)
				steerCount++;

			//if (trailer.Axle1IsLiftable && trailer.Axle1IsSteered) return false;
			//if (trailer.Axle2IsLiftable && trailer.Axle2IsSteered) return false;
			//if (trailer.Axle3IsLiftable && trailer.Axle3IsSteered) return false;

			switch (trailer.NumberOfAxles)
			{
				case NumberOfTrailerAxles.One when trailer.Axle1IsLiftable: //one axle cant be liftable
				case NumberOfTrailerAxles.One when trailer.TrailerType == TypeTrailer.DC && trailer.Axle1IsSteered:
				case NumberOfTrailerAxles.Two when trailer.Axle2FilePath == null: //missing axle
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DB && steerCount > 0:
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DB && liftableCount > 0:
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DC && trailer.Axle1IsSteered:
				case NumberOfTrailerAxles.Two when trailer.TrailerType == TypeTrailer.DA && trailer.Axle1IsSteered:
				case NumberOfTrailerAxles.Two when liftableCount > 1: //too many liftable
				case NumberOfTrailerAxles.Two when trailer.Axle3IsLiftable || trailer.Axle3IsSteered: //no axle 3 allowed
				case NumberOfTrailerAxles.Three when trailer.TrailerType == TypeTrailer.DB && liftableCount > 1: //too many liftable
				case NumberOfTrailerAxles.Three when trailer.TrailerType == TypeTrailer.DB && (trailer.Axle1IsLiftable || trailer.Axle1IsSteered || trailer.Axle2IsSteered): //for For 3 axle DBs it should not be possible to declare a lift or a steered axle on the first axle
				case NumberOfTrailerAxles.Three when trailer.TrailerType == TypeTrailer.DC && (trailer.Axle2IsLiftable || trailer.Axle1IsSteered || trailer.Axle2IsSteered):
				case NumberOfTrailerAxles.Three when liftableCount > 2: //too many liftable
				case NumberOfTrailerAxles.Three when steerCount > 2: //too many liftable
				case NumberOfTrailerAxles.Three when (trailer.Axle2FilePath == null || trailer.Axle3FilePath == null):  //missing axles
					return false;
			}

			return true;
		}

		private static bool CheckTrailerType(Model.TempDataObject.TrailerComponentData trailer)
		{
			if (trailer.TrailerType != TypeTrailer.DB &&
				(trailer.TPLMAxleAssembly == null || trailer.TPLMAxleAssembly.Value() == 0))
				return false;

			if (trailer.TrailerType != TypeTrailer.DB &&
				((trailer.Axle1IsLiftable || trailer.Axle1IsSteered) || (trailer.Axle2IsLiftable || trailer.Axle2IsSteered)))
				return false;

			if ((trailer.TrailerType != TypeTrailer.DC || trailer.TrailerType != TypeTrailer.DA) &&
				trailer.NumberOfAxles == NumberOfTrailerAxles.One && trailer.LegislativeCategory.ToUpper() != "O3")
				return false;

			if ((trailer.TrailerType != TypeTrailer.DC || trailer.TrailerType != TypeTrailer.DA) &&
				trailer.NumberOfAxles == NumberOfTrailerAxles.One && trailer.Axle1IsLiftable)
				return false;

			if (trailer.TrailerType != TypeTrailer.DC && trailer.TPLMAxleAssembly > trailer.TPLMTotalTrailer)
				return false;

			if (trailer.TrailerType != TypeTrailer.DA && trailer.TPLMAxleAssembly >= trailer.TPLMTotalTrailer)
				return false;

			return trailer.TrailerType == TypeTrailer.DC || trailer.TrailerCouplingPoint == TrailerCouplingPoint.Unknown;
		}

		private static bool GeneralCheck(Model.TempDataObject.TrailerComponentData trailer)
		{
			if (trailer.VIN.Length != 17)
				return false;

			if (trailer.ExternalHeightOfTheBody >= trailer.TotalHeightOfTheTrailer)
				return false;

			if (trailer.NumberOfAxles == NumberOfTrailerAxles.One && trailer.LengthBetweenCentersOfAxles.Value() > 0)
				return false;

			if (trailer.NumberOfAxles == NumberOfTrailerAxles.One && trailer.TrailerType == TypeTrailer.DB)
				return false;

			if (trailer.ExternalWidthOfTheBody < 1 || trailer.ExternalWidthOfTheBody > 2.6)
				return false;

			if (trailer.TotalHeightOfTheTrailer < 1 || trailer.TotalHeightOfTheTrailer > 4)
				return false;

			if (trailer.ExternalWidthOfTheBody >= trailer.TotalHeightOfTheTrailer)
				return false;

			if (trailer.TrailerType == TypeTrailer.DA && trailer.ExternalWidthOfTheBody > 14)
				return false;

			if (trailer.TrailerType == TypeTrailer.DC && trailer.ExternalWidthOfTheBody > 12)
				return false;

			if (trailer.TrailerType == TypeTrailer.DB && trailer.ExternalWidthOfTheBody > 12)
				return false;

			return trailer.TPLMAxleAssembly <= trailer.TPLMTotalTrailer;
		}

		private void DoValidateInput()
		{
			ClearValidationErrors();

			_errors = new Dictionary<string, string>();

			SetCurrentDataToSave();
			var xDoc = _xmlCompletedTrailer.GenerateCompletedTrailerDocument(CompleteVehicleTrailerData);

			var trailer = (Model.TempDataObject.TrailerComponentData)CompleteVehicleTrailerData.Values.FirstOrDefault();

			//Form Input Validation
			if (!CheckAeroFeatures(trailer)) {
				ValidationResultDialogWindow(false);
				return;
			}

			if (!CheckAxles(trailer)){
				ValidationResultDialogWindow(false);
				return;
			}
			if (!CheckTrailerType(trailer)){
				ValidationResultDialogWindow(false);
				return;
			}
			if (!GeneralCheck(trailer)){
				ValidationResultDialogWindow(false);
				return;
			}

			//XML Schema Validation
			ValidationResultDialogWindow(XmlHelper.ValidateXDocument(xDoc, null, ValidationErrorAction));
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

				if (sourceObject != null && !_errors.ContainsKey(localName))
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

		private static void ValidationResultDialogWindow(bool validationResult)
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationJobs;
using TUGraz.VectoCore.Utils;
using VECTOTrailer.Helper;
using VECTOTrailer.ViewModel.Interfaces;
using VECTOTrailer.Util;
using VECTOTrailer.Util.XML;
using VECTOTrailer.ViewModel.Adapter.Declaration;
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

		private IXMLInputDataReader _xmlReader;
		private readonly IJoblistViewModel _jobListViewModel;

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

		public CompleteTrailerJobViewModel(IKernel kernel, IDeclarationTrailerInputDataProvider inputData, ITrailerToolSettings settings) : base()
		{
			Kernel = kernel;
			_xmlReader = kernel.Get<IXMLInputDataReader>();
			_jobListViewModel = kernel.Get<IJoblistViewModel>();
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
				Save(window, XmlFilePath);
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
			var filePath = FileDialogHelper.SaveXmlFileToDialog();
			if (filePath == null) {
				return;
			}

			if (Save(window, filePath)) {

			}
		}

		private bool Save(Window window, string filePath)
		{
			SetCurrentDataToSave();

			IList<string> errors = new List<string>();
			

			var xDocument = _xmlCompletedTrailer.GenerateCompletedTrailerDocument(CompleteVehicleTrailerData);

			var trailerData = _xmlReader.CreateDeclarationTrailer(xDocument.CreateReader(new ReaderOptions()));
			var valid = DeclarationTrailerDataAdapter.ValidateTrailerInput(trailerData.JobInputData.Trailer, ref errors);


			if (valid && XmlHelper.ValidateXDocument(xDocument, null, ValidationErrorAction)) {
				_xmlCompletedTrailerWriter.WriteCompletedTrailerXml(filePath, xDocument);
				if (filePath != XmlFilePath) {
					_jobListViewModel.HandleFileOpen(filePath);
					//SetXmlFilePath(filePath);
				}
				CloseWindow(window);
				return true;
			} else {
				ValidationResultDialogWindow(false, errors);
				DoShowValidationErrors();
				return false;
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
			get { throw new NotImplementedException($"Moved validation to {nameof(DeclarationTrailerDataAdapter)}"); }
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
			
			if (xmlException != null) {
				Debug.WriteLine(arg2?.ValidationEventArgs.Message);
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
			ValidationResultDialogWindow(validationResult, null);
		}
		private static void ValidationResultDialogWindow(bool validationResult, IList<string> errors)
		{
			errors = errors ?? new List<string>();
			if (validationResult) {
				MessageBox.Show("No validation errors were found, the entered data is valid!", "Input Validation", MessageBoxButton.OK, MessageBoxImage.Information);
			} else {
				MessageBox.Show($"There are some input validation errors, the data must be valid for saving! \n \n- {string.Join("\n- ", errors)}", "Input Validation", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		public string JobFile { get; }
		public IDeclarationTrailerInputDataProvider InputDataProvider { get; set; }


	}
}

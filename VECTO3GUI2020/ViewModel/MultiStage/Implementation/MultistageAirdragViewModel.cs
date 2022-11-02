using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using ViewModelBase = VECTO3GUI2020.ViewModel.Implementation.Common.ViewModelBase;
using XmlDocumentType = System.Xml.XmlDocumentType;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class MultistageAirdragViewModel : ViewModelBase, IMultistageAirdragViewModel
	{
		private IAirDragViewModel _airdragViewModel;

		private void OnAirdragViewModelChanged()
		{
			AirdragViewModelChanged?.Invoke(this, EventArgs.Empty);
		}
		public string AirdragFilePath
		{
			get
			{
				return AirDragViewModel?.DataSource.SourceFile.ToString();
			} 
			set => SetProperty(ref _airdragFilePath, value);
		}

		public EventHandler AirdragViewModelChanged { get; set; }

		public IAirDragViewModel AirDragViewModel
		{
			get => _airdragViewModel;
			set
			{
				StoreAirdragViewModel();
				if (SetProperty(ref _airdragViewModel, value)) {
					OnAirdragViewModelChanged();
				};
			}
		}

		public bool ShowConsolidatedData
		{
			get => _showConsolidatedData;
			set => SetProperty(ref _showConsolidatedData, value);
		}

		private void StoreAirdragViewModel()
		{
			if (AirDragViewModel != null && StoredAirdragViewModel != AirDragViewModel) {
				StoredAirdragViewModel = AirDragViewModel;
			}
		}

		public void RestoreAirdragViewModel()
		{
			if (AirDragViewModel == null) {
				AirDragViewModel = StoredAirdragViewModel;
			}
		}

		private IAirDragViewModel StoredAirdragViewModel { get; set; } = null;


		public void SetAirdragInputData(IAirdragDeclarationInputData airdragInputData)
		{
			Debug.WriteLine("[MultistageAirdragViewModel] loaded AirdragInputData");
			if (airdragInputData == null) {
				AirDragViewModel = null;
				return;
				
			}
			AirDragViewModel = _dependencies.ComponentViewModelFactory.CreateComponentViewModel(airdragInputData) as IAirDragViewModel;
			if (AirDragViewModel != null) {
				AirDragViewModel.LabelVisible = false;
				AirDragViewModel.IsReadOnly = true;
			}
		}

		public IAirdragDeclarationInputData ConsolidatedAirdragData
		{
			get => _consolidatedAirdragInputData;
			set => SetProperty(ref _consolidatedAirdragInputData, value);
		}


		#region Commands



		private ICommand _loadAirdragFileCommand;
		private ICommand _removeAirdragDataCommand;



		private Dictionary<string, string> _validationErrors;

		private IAirdragDeclarationInputData _consolidatedAirdragInputData;
		private string _airdragFilePath;
		private readonly IMultistageDependencies _dependencies;
		private bool _showConsolidatedData = true;

		public ICommand LoadAirdragFileCommand
		{
			get => _loadAirdragFileCommand ?? new RelayCommand(LoadAirdragFileCommandExecute, () => true);
		}

		public void LoadAirdragFileCommandExecute()
		{
			var fileName =_dependencies.DialogHelper.OpenXMLFileDialog();
			if (fileName == null) {
				return;
			}
			var success = LoadAirdragFile(fileName);

			if (success) {
				AirdragFilePath = fileName;
			} else {
				_dependencies.DialogHelper.ShowMessageBox("Invalid input file", "Error", MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
			
		}

		public bool LoadAirdragFile(string fileName)
		{
			var success = true;
			var errorStringBuilder = new StringBuilder();
			try {
				var xDoc = XDocument.Load(fileName);
				var doc = new XmlDocument();
				doc.Load(fileName);

				var airdragElements = xDoc.Descendants().Where(e => e.Name.LocalName == XMLNames.Component_AirDrag);
				if (airdragElements.Count() == 1) {
					//GET FROM FILE
					var dataProviderVersion = XMLDeclarationAirdragDataProviderV20.QUALIFIED_XSD_TYPE;


					var validator = new XMLValidator(doc);
					var valid = validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType
						.DeclarationComponentData);
					if (!valid) {
						throw new VectoException("Invalid input file");
					}

					//dataProviderVersion = XMLHelper.GetVersion(doc.Node);

					XElement airdragElement = airdragElements.First();
					XmlNode airdragNode = airdragElement.ToXmlNode();

					var airDragInputData =
						_dependencies.InjectFactory.CreateAirdragData(dataProviderVersion, null, airdragNode, fileName);
					AirDragViewModel =
						_dependencies.ComponentViewModelFactory.CreateComponentViewModel(airDragInputData) as IAirDragViewModel;
					AirDragViewModel.IsReadOnly = true;
					AirDragViewModel.LabelVisible = false;
					success = true;
				} else {
					success = false;
				}
			} catch (Exception e) {
				_dependencies.DialogHelper.ShowMessageBox(e.Message,
					"Invalid File",
					MessageBoxButton.OK,
					MessageBoxImage.Error);
				success = false;
			}

			return success;
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
					_validationErrors.Add(localName, message?.Message);
			}
		}

		public ICommand RemoveAirdragDataCommand{
			get => _removeAirdragDataCommand ?? new RelayCommand(() => {
				RemoveAirdragComponent();
				OnPropertyChanged(nameof(AirdragFilePath));
			},  () => AirDragViewModel != null);
		}

		#endregion

		public void RemoveAirdragComponent()
		{
			AirDragViewModel = null;
			StoredAirdragViewModel = null;
            OnPropertyChanged(nameof(AirdragFilePath));

		}



		public MultistageAirdragViewModel(IMultistageDependencies dependencies)
		{
			_dependencies = dependencies;
			_airdragFilePath = "Select Airdrag File - if no file is selected a default Airdrag Component is loaded";
		}

		public MultistageAirdragViewModel(IAirdragDeclarationInputData consolidatedAirdragInputData,
			IMultistageDependencies multistageDependencies) : this(multistageDependencies)
		{
			ConsolidatedAirdragData = consolidatedAirdragInputData;
		}

		public DataSource DataSource => _airdragViewModel?.DataSource;

		public bool SavedInDeclarationMode => _airdragViewModel?.SavedInDeclarationMode ?? false;

		public string Manufacturer => _airdragViewModel?.Manufacturer;

		public string Model => _airdragViewModel?.Model;

		public DateTime Date => _airdragViewModel.Date;

		public string AppVersion => _airdragViewModel.AppVersion;

		public CertificationMethod CertificationMethod => _airdragViewModel.CertificationMethod;

		public string CertificationNumber => _airdragViewModel.CertificationNumber;

		public DigestData DigestValue => _airdragViewModel.DigestValue;

		public SquareMeter AirDragArea => _airdragViewModel.AirDragArea;

		public SquareMeter TransferredAirDragArea => _airdragViewModel.TransferredAirDragArea;

		public SquareMeter AirDragArea_0 => _airdragViewModel.AirDragArea_0;
	}
}
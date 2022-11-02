using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{

	public interface IMultiStageJobViewModel : IDeclarationMultistageJobInputData, IMultistageVIFInputData, IMultistageBusInputDataProvider, IJobViewModel, IEditViewModel
	{
		IManufacturingStageViewModel ManufacturingStageViewModel { get; }
		bool Exempted { get; }

		/// <summary>
		/// Creates a new VIF file
		/// </summary>
		/// <param name="outputFile"></param>
		/// <returns>Name of the created File</returns>
		string SaveVif(string outputFile);
	}


	public class MultiStageJobViewModel_v0_1 : ViewModelBase, IMultiStageJobViewModel
	{
		private IDeclarationMultistageJobInputData _jobInputData;

		private IManufacturingStageViewModel _manufacturingStageViewModel;
		private IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		private IList<IManufacturingStageInputData> _manufacturingStages;
		private IManufacturingStageInputData _consolidateManufacturingStage;
		private VectoSimulationJobType _jobType;
		private bool _inputComplete;
		private readonly IMultiStageViewModelFactory _vmFactory;
		private IAdditionalJobInfoViewModel _additionalJobInfoVm;



		public IManufacturingStageViewModel ManufacturingStageViewModel
		{
			get => _manufacturingStageViewModel;
			set => SetProperty(ref _manufacturingStageViewModel, value);
		}

		public MultiStageJobViewModel_v0_1(IMultistageBusInputDataProvider inputData, 
			IMultiStageViewModelFactory vmFactory, 
			IMultistageDependencies multistageDependencies,
			IXMLInputDataReader inputDataReader, 
			IJobListViewModel jobListViewModel,
			IAdditionalJobInfoViewModel additionalJobInfo,
			ISimulatorFactoryFactory simulatorFactoryFactory)
		{
			
			_dataSource = inputData.DataSource;
			Title = GUILabels.Edit_Multistep_Job + $" - {Path.GetFileName(_dataSource.SourceFile)}";
			_jobInputData = inputData.JobInputData;
			_jobListViewModel = jobListViewModel;
			_inputData = inputData;
			_vmFactory = vmFactory;
			_simFactoryFactory = simulatorFactoryFactory;
			_consolidateManufacturingStage = _jobInputData.ConsolidateManufacturingStage;
			_manufacturingStages = _jobInputData.ManufacturingStages;
			_primaryVehicle = _jobInputData.PrimaryVehicle;
			_dialogHelper = multistageDependencies.DialogHelperLazy;
			_inputComplete = inputData.JobInputData.InputComplete;
			_invalidEntries = inputData.JobInputData?.InvalidEntries?.Distinct().ToList();
			_additionalJobInfoVm = additionalJobInfo;
			_additionalJobInfoVm.SetParent(this);
			

			_exempted = PrimaryVehicle.Vehicle.ExemptedVehicle;

			_manufacturingStageViewModel =
				vmFactory.GetManufacturingStageViewModel(_consolidateManufacturingStage, _exempted);

			// QUESTION: HEV/PEV ?
			//var hybridElectric = inputData.PrimaryVehicleData.Vehicle.HybridElectricHDV;
			//_manufacturingStageViewModel.VehicleViewModel.PrimaryVehicleHybridElectric = hybridElectric;
			_multistageDependencies = multistageDependencies;


            (_manufacturingStageViewModel as INotifyPropertyChanged).PropertyChanged += MultiStageJobViewModel_v0_1_PropertyChanged;
		}

		public IAdditionalJobInfoViewModel AdditionalJobInfoVm
		{
			get => _additionalJobInfoVm;
			set => SetProperty(ref _additionalJobInfoVm, value);
		}


		private void MultiStageJobViewModel_v0_1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			
			if (e.PropertyName == nameof(VehicleInputDataFilePath)) {
				OnPropertyChanged(nameof(VehicleInputDataFilePath));
			}
        }





        #region Commands

		private ICommand _saveVifCommand;

		public ICommand SaveVIFCommand
		{
			get
			{
				return _saveVifCommand ?? new RelayCommand(() => {
					if (_manufacturingStageViewModel.Vehicle is IMultistageVehicleViewModel vehicleViewModel)
					{
						if (vehicleViewModel.HasErrors) {
							var errorMessage = "Vehicle\n";
							var vehicleErrorInfo = vehicleViewModel as IDataErrorInfo;
							errorMessage += vehicleErrorInfo.Error.Replace(",", "\n");

							var auxiliariesErrorInfo =
								vehicleViewModel.MultistageAuxiliariesViewModel as IDataErrorInfo;
							if (auxiliariesErrorInfo != null && !string.IsNullOrEmpty(auxiliariesErrorInfo.Error)) {
								errorMessage += "Auxiliaries\n";
								errorMessage += auxiliariesErrorInfo.Error.Replace(",", "\n");
							}

							_dialogHelper.Value.ShowMessageBox(errorMessage, "Error", MessageBoxButton.OK,
									MessageBoxImage.Error);
							return;
						}
					} else {
						throw new NotImplementedException();
					}


					var outputFile = _multistageDependencies.DialogHelperLazy.Value.SaveToXMLDialog(Settings.Default.DefaultFilePath);
					if (outputFile == null) {
						return;
					}
					SaveVif(outputFile:outputFile);
				}, () => true);
			}
		}
		/// <summary>
		/// Creates a new VIF file
		/// </summary>
		/// <param name="outputFile"></param>
		/// <returns>Name of the created File</returns>
		public string SaveVif(string outputFile)
		{
			return SaveVif(vifData:this, outputFile:outputFile, dialogHelper:_dialogHelper.Value);
		}

		public void SaveVif(IMultistageVIFInputData vifData, FileOutputVIFWriter writer, IDialogHelper dialogHelper = null)
		{
			SaveVif(vifData, null, writer, dialogHelper);
		}


		/// <summary>
		/// Creates a new VIF file
		/// </summary>
		/// <param name="vifData"></param>
		/// <param name="outputFile"></param>
		/// <param name="writer"></param>
		/// <param name="dialogHelper"></param>
		/// <returns>Name of the created file</returns>
		private string SaveVif(IMultistageVIFInputData vifData, string outputFile,
			FileOutputVIFWriter writer = null, IDialogHelper dialogHelper = null)
		{
			try {
				
				if (writer == null) {
					var numberOfManufacturingStages =
						vifData.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? 0;
					writer = new FileOutputVIFWriter(outputFile, numberOfManufacturingStages);
				}

				var inputData = new XMLDeclarationVIFInputData(vifData.MultistageJobInputData, vifData.VehicleInputData);


				var factory = _simFactoryFactory.Factory(ExecutionMode.Declaration, inputData, writer);
				//var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
					FileHelper.CreateDirectory(outputFile);

					var jobContainer = new JobContainer(new NullSumWriter());

					jobContainer.AddRuns(factory);
					jobContainer.Execute();
					jobContainer.WaitFinished();

					dialogHelper?.ShowMessageBox($"Written to {writer.XMLMultistageReportFileName}", "Info",
						MessageBoxButton.OK, MessageBoxImage.Information);

					var runSimulation = vifData.VehicleInputData.VehicleDeclarationType == VehicleDeclarationType.final &&
										(_dialogHelper.Value.ShowMessageBox("Do you want to start the simulation?",
											"Run Simulation",
											MessageBoxButton.YesNo,
											MessageBoxImage.Question) == MessageBoxResult.Yes);
					_jobListViewModel.AddJobAsync(writer.XMLMultistageReportFileName, runSimulation);

					Debug.WriteLine($"Written to {writer.XMLMultistageReportFileName}");
					return writer.XMLMultistageReportFileName;
                
			}catch (Exception e) {
				dialogHelper?.ShowMessageBox($"{e.GetInnerExceptionMessages()}", "Error writing VIF", MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
			return null;
		}

  //      private bool WriteTempVIFAndValidate(XMLDeclarationVIFInputData inputData, FileOutputVIFWriter writer, IDialogHelper dialogHelper)
  //      {
		//	var tempWriter = new TempFileOutputWriter(writer);
		//	var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, tempWriter);

		//	var jobContainer = new JobContainer(new NullSumWriter());

		//	jobContainer.AddRuns(factory);
		//	jobContainer.Execute();
		//	jobContainer.WaitFinished();

		//	var resultXDoc = tempWriter.GetDocument(ReportType.DeclarationReportMultistageVehicleXML);


		//	using (var reader = resultXDoc.Root.CreateReader())
		//	{
		//		var validator = new XMLValidator(reader);
		//		var valid = validator.ValidateXML(XmlDocumentType.MultistepOutputData);
		//		if (!valid)
		//		{
		//			dialogHelper?.ShowMessageBox($"Error writing VIF {validator.ValidationError}", "Error",
		//				MessageBoxButton.OK, MessageBoxImage.Error);
		//			Debug.WriteLine("Invalid Outputfile");
		//			return false;
		//		}
		//	}
		//	using (var reader = resultXDoc.Root.CreateReader())
  //          {

		//		if (inputData.VehicleInputData.VehicleDeclarationType == VehicleDeclarationType.final)
		//		{
		//			var inputDataProvider = _inputDataReader.Create(reader) as IMultistageBusInputDataProvider;
		//			if (!inputDataProvider.JobInputData.InputComplete)
		//			{
		//				var errorCaption = "Step marked as final with incomplete/invalid input";
		//				var errorStringBuilder = new StringBuilder();
		//				errorStringBuilder.AppendLine("The following parameters are invalid:\n");
		//				var converter = new PropertyNameToLabelTextConverter();
		//				var resourceManagers = new List<ResourceManager>()
		//				{
		//					GUILabels.ResourceManager,
		//					BusStrings.ResourceManager,
		//					Strings.ResourceManager
		//				};
		//				foreach (var invalidEntry in inputDataProvider.JobInputData.InvalidEntries)
		//				{
		//					string name;
		//					var conversionResult = converter.Convert(invalidEntry,
		//						typeof(string),
		//						resourceManagers,
		//						System.Globalization.CultureInfo.CurrentCulture);
		//					if (conversionResult is string convString)
		//					{
		//						name = convString;
		//					}
		//					else
		//					{
		//						name = invalidEntry;
		//					}
		//					errorStringBuilder.AppendLine(name);
		//				}
		//				dialogHelper?.ShowErrorMessage(errorStringBuilder.ToString(), errorCaption);
		//				return false;
		//			}
				
		//		}
		//	}

			



		//	return true;
		//}

        private readonly Lazy<IDialogHelper> _dialogHelper;
		private readonly IMultistageDependencies _multistageDependencies;
		private readonly DataSource _dataSource;
		private readonly IMultistageBusInputDataProvider _inputData;
		private bool _selected;
		private readonly bool _exempted;
		private readonly IJobListViewModel _jobListViewModel;
		private readonly IList<string> _invalidEntries;
		private readonly ISimulatorFactoryFactory _simFactoryFactory;


		public string VehicleInputDataFilePath
		{
			get => ManufacturingStageViewModel.VehicleInputDataFilePath;
			set
			{
				ManufacturingStageViewModel.VehicleInputDataFilePath = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Implementation of IInputDataProvider

		public string DocumentName => Path.GetFileNameWithoutExtension(_inputData.DataSource.SourceFile);

		public XmlDocumentType? DocumentType => XmlDocumentType.MultistepOutputData;

		public string DocumentTypeName => DocumentType?.GetName();

		public DataSource DataSource => _dataSource;

		public IEditViewModel EditViewModel => this;

		public bool Selected
		{
			get => _selected && CanBeSimulated;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeSimulated {
			get {
				var declType = _inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleDeclarationType;
				return InputComplete && (declType == VehicleDeclarationType.final || Exempted);
			}
			set => throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMultistageVIFInputData

		public IVehicleDeclarationInputData VehicleInputData => _manufacturingStageViewModel.Vehicle;

		public IMultistageBusInputDataProvider MultistageJobInputData => this;


		#endregion

		#region Implementation of IDeclarationInputDataProvider


		public IDeclarationMultistageJobInputData JobInputData => _jobInputData;

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData => throw new NotImplementedException();

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData => _primaryVehicle;

		public XElement XMLHash => throw new NotImplementedException();

		#endregion

		#region Implementation of IDeclarationMultistageJobInputData

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get => _primaryVehicle;
			set => _primaryVehicle = value;
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get => _manufacturingStages;
			set => _manufacturingStages = value;
		}

		public IManufacturingStageInputData ConsolidateManufacturingStage
		{
			get => _consolidateManufacturingStage;
			set => _consolidateManufacturingStage = value;
		}

		public VectoSimulationJobType JobType
		{
			get => _jobType;
			set => _jobType = value;
		}

		public bool InputComplete
		{
			get => _inputComplete;
			set => _inputComplete = value;
		}

		public IList<string> InvalidEntries => _invalidEntries;

		#endregion

		#region Implementation of IEditViewModel

		public string Name => "Multistage";

		public bool Exempted => _exempted;



		#endregion
	}

	public class NullSumWriter : SummaryDataContainer
	{
		public override void Write(IModalDataContainer modData, int jobNr, int runNr, VectoRunData runData) { }

		public override void Finish() { }
	}
}
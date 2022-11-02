using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using EnumHelper = VECTO3GUI2020.Helper.EnumHelper;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public enum AIRDRAGMODIFIED
	{
		[GuiLabel("")]
		UNKNOWN = 0,
		[GuiLabel("True")]
		TRUE = 1,
		[GuiLabel("False")]
		FALSE = 2,

	}

	public static class AirdragModifiedEnumHelper
	{
		public static AIRDRAGMODIFIED toAirdragModifiedEnum(this bool? nullableBool)
		{
			if (nullableBool.HasValue) {
				return nullableBool.Value == true ? AIRDRAGMODIFIED.TRUE : AIRDRAGMODIFIED.FALSE;

			}
			return AIRDRAGMODIFIED.UNKNOWN;
		}


		public static bool? toNullableBool(this AIRDRAGMODIFIED airdragModified)
		{
			switch (airdragModified) {
				case AIRDRAGMODIFIED.TRUE:
					return true;
				case AIRDRAGMODIFIED.FALSE:
					return false;
				default:
					return null;
			}
		}
	}

	public interface IMultistageVehicleViewModel : IVehicleViewModel, INotifyPropertyChanged
	{
		bool HasErrors { get; }
		Dictionary<string, string> Errors { get; }
		IMultistageAirdragViewModel MultistageAirdragViewModel { get; set; }
		IMultistageAuxiliariesViewModel MultistageAuxiliariesViewModel { get; set; }
		bool PrimaryVehicleHybridElectric { get; set; }
		void SetVehicleInputData(IVehicleDeclarationInputData vehicleInputData);
	}


	public class InterimStageBusVehicleViewModel_v2_8 : ViewModelBase, IMultistageVehicleViewModel,
		IVehicleComponentsDeclaration, IAdvancedDriverAssistantSystemDeclarationInputData, IDataErrorInfo
	{
		public static readonly Type INPUTPROVIDERTYPE = typeof(XMLDeclarationConventionalCompletedBusDataProviderV24);
		public static readonly Type INPUTPROVIDERTYPEEXEMPTED = typeof(XMLDeclarationExemptedCompletedBusDataProviderV24);
		public static string VERSION = INPUTPROVIDERTYPE.ToString();
		public static string VERSION_EXEMPTED = INPUTPROVIDERTYPEEXEMPTED.ToString();


		private readonly IMultiStageViewModelFactory _multiStageViewModelFactory;

		#region Subcomponents
		private IMultistageAirdragViewModel _multistageAirdragViewModel;
		private IMultistageAuxiliariesViewModel _multistageAuxiliariesViewModel;
		public IMultistageAirdragViewModel MultistageAirdragViewModel
		{
			get => _multistageAirdragViewModel;
			set => SetProperty(ref _multistageAirdragViewModel, value);
		}

		public IMultistageAuxiliariesViewModel MultistageAuxiliariesViewModel
		{
			get => _multistageAuxiliariesViewModel;
			set => SetProperty(ref _multistageAuxiliariesViewModel, value);
		}

		private bool _primaryVehicleHybridElectric;
		public bool PrimaryVehicleHybridElectric
		{
			get => _primaryVehicleHybridElectric;
			set
			{
				SetProperty(ref _primaryVehicleHybridElectric, value);
				MultistageAuxiliariesViewModel.PrimaryVehicleHybridElectric = value;
			}
		}

		private Dictionary<string, MultistageParameterViewModel> _parameterViewModels;
		public Dictionary<string, MultistageParameterViewModel> ParameterViewModels
		{
			get => _parameterViewModels;
			set => _parameterViewModels = value;
		}

		#endregion



		public string Name => "Vehicle";

		public bool IsPresent => true;

		public DataSource DataSource => throw new NotImplementedException();

		public bool SavedInDeclarationMode => true;

		public ObservableCollection<IComponentViewModel> ComponentViewModels
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		private bool _showConsolidatedData;

		public bool ShowConsolidatedData
		{
			get => _showConsolidatedData;
			set
			{
				SetProperty(ref _showConsolidatedData, value);
				if (MultistageAuxiliariesViewModel != null) {
					MultistageAuxiliariesViewModel.ShowConsolidatedData = value;
				}

				if (MultistageAirdragViewModel != null) {
					MultistageAirdragViewModel.ShowConsolidatedData = value;
				}
				foreach (var multistageParameterViewModel in _parameterViewModels) {
					multistageParameterViewModel.Value.ShowConsolidatedData = value;
				}
			}
		}

		public InterimStageBusVehicleViewModel_v2_8(string inputProviderType, IMultiStageViewModelFactory multiStageViewModelFactory)
		{
			if (inputProviderType == VERSION_EXEMPTED)
			{
				_exemptedVehicle = true;
			}

			_multiStageViewModelFactory = multiStageViewModelFactory;

			if (!_exemptedVehicle)
			{
				MultistageAirdragViewModel = _multiStageViewModelFactory.GetMultistageAirdragViewModel();
				MultistageAuxiliariesViewModel = _multiStageViewModelFactory.GetAuxiliariesViewModel();
			}

			CreateParameterViewModels();
			ShowConsolidatedData = false;
		}
		public InterimStageBusVehicleViewModel_v2_8(IVehicleDeclarationInputData inputData, IMultiStageViewModelFactory multiStageViewModelFactory) : 
			this(inputData.GetType().ToString(), multiStageViewModelFactory)
		{
			SetVehicleInputData(inputData);
		}

		public InterimStageBusVehicleViewModel_v2_8(IVehicleDeclarationInputData consolidatedVehicleData,
			IMultiStageViewModelFactory multistageViewModelFactory, bool exempted)
		{
			ConsolidatedVehicleData = consolidatedVehicleData;
			_exemptedVehicle = exempted;
			_multiStageViewModelFactory = multistageViewModelFactory;


			if (!exempted) {
				MultistageAirdragViewModel = _multiStageViewModelFactory.GetMultistageAirdragViewModel(consolidatedVehicleData?.Components?.AirdragInputData);

				MultistageAirdragViewModel.AirdragViewModelChanged += ((sender, args) => {
					if (sender is IMultistageAirdragViewModel vm)
					{
						if (AirdragModifiedMultistepMandatory)
						{
							if (vm.AirDragViewModel != null)
							{
								AirdragModifiedMultistep = true;
							}
						}
					}
				});

				MultistageAuxiliariesViewModel =
					_multiStageViewModelFactory.GetAuxiliariesViewModel(consolidatedVehicleData?.Components?
						.BusAuxiliaries);
			}


			CreateParameterViewModels();


			if (consolidatedVehicleData?.AirdragModifiedMultistep != null)
			{
				AirdragModifiedMultistepMandatory = true;
				AirdragModifiedMultistepEditingEnabled = true;
			}

			if (consolidatedVehicleData?.Components?.AirdragInputData != null)
			{
				AirdragModifiedMultistepMandatory = true;
				AirdragModifiedMultistepEditingEnabled = true;
			}
		}

		private void CreateParameterViewModels()
		{
			_parameterViewModels = new Dictionary<string, MultistageParameterViewModel>();
			var properties = this.GetType().GetProperties();
			var backedUpParameters = new HashSet<string>() {
				nameof(Manufacturer),
				nameof(ManufacturerAddress),
				nameof(VIN),
				nameof(Model),
				nameof(LegislativeClass),
				nameof(CurbMassChassis),
				nameof(GrossVehicleMassRating),
				nameof(TankSystem),
				nameof(AirdragModifiedEnum),
				nameof(RegisteredClass),
				nameof(NumberPassengerSeatsUpperDeck),
				nameof(NumberPassengerSeatsLowerDeck),
				nameof(NumberPassengersStandingLowerDeck),
				nameof(NumberPassengersStandingUpperDeck),
				nameof(VehicleCode),
				nameof(LowEntry),
				nameof(HeightInMm),
				nameof(WidthInMm),
				nameof(LengthInMm),
				nameof(EntranceHeightInMm),
				nameof(DoorDriveTechnology),
				nameof(VehicleDeclarationType),
				nameof(EngineStopStartNullable),
				nameof(EcoRollTypeNullable),
				nameof(PredictiveCruiseControlNullable),
				nameof(ATEcoRollReleaseLockupClutch),
				nameof(VehicleTypeApprovalNumber),
			};

			foreach (var property in properties) {
				if (!backedUpParameters.Contains(property.Name)) {
					continue;
				}


				object previousInputData = null;
				try {
					previousInputData = ConsolidatedVehicleData?.GetType().GetProperty(property.Name)?
						.GetValue(ConsolidatedVehicleData);
				} catch (Exception e) {
					Debug.WriteLine(e.Message);
				}

				_parameterViewModels.Add(property.Name, new MultistageParameterViewModel(property.Name, previousInputData, this,
					resourceManagers: new ResourceManager[] { BusStrings.ResourceManager, Strings.ResourceManager }
				));
			}

			_parameterViewModels[nameof(WidthInMm)].PreviousContent = ConsolidatedWidthInMm;
			_parameterViewModels[nameof(WidthInMm)].DummyContent = ConvertedSIDummyCreator.CreateMillimeterDummy();
			_parameterViewModels[nameof(HeightInMm)].PreviousContent = ConsolidatedHeightInMm;
			_parameterViewModels[nameof(HeightInMm)].DummyContent = ConvertedSIDummyCreator.CreateMillimeterDummy();
			_parameterViewModels[nameof(LengthInMm)].PreviousContent = ConsolidatedLengthInMm;
			_parameterViewModels[nameof(LengthInMm)].DummyContent = ConvertedSIDummyCreator.CreateMillimeterDummy();
			_parameterViewModels[nameof(EntranceHeightInMm)].PreviousContent = ConsolidatedEntranceHeightInMm;
			_parameterViewModels[nameof(EntranceHeightInMm)].DummyContent = ConvertedSIDummyCreator.CreateMillimeterDummy();

			_parameterViewModels[nameof(AirdragModifiedEnum)].PreviousContent = ConsolidatedAirdragModifiedEnum;


			///Set up editing groups

			Action<MultistageParameterViewModel> MeasureMentsEditingGroupCallback = (MultistageParameterViewModel param) => {
				MeasurementsGroupEditingEnabled = param.EditingEnabled;
			};
			_parameterViewModels[nameof(WidthInMm)].EditingChangedCallback = MeasureMentsEditingGroupCallback;
			_parameterViewModels[nameof(LengthInMm)].EditingChangedCallback = MeasureMentsEditingGroupCallback;
			_parameterViewModels[nameof(HeightInMm)].EditingChangedCallback = MeasureMentsEditingGroupCallback;
			_parameterViewModels[nameof(EntranceHeightInMm)].EditingChangedCallback = MeasureMentsEditingGroupCallback;

			Action<MultistageParameterViewModel> ADASGroupEditingCallback = (MultistageParameterViewModel param) => {
				AdasEditingEnabled = param.EditingEnabled;
			};

			_parameterViewModels[nameof(EngineStopStartNullable)].EditingChangedCallback = ADASGroupEditingCallback;
			_parameterViewModels[nameof(EcoRollTypeNullable)].EditingChangedCallback = ADASGroupEditingCallback;
			_parameterViewModels[nameof(PredictiveCruiseControlNullable)].EditingChangedCallback = ADASGroupEditingCallback;
			_parameterViewModels[nameof(ATEcoRollReleaseLockupClutch)].EditingChangedCallback = ADASGroupEditingCallback;

			_parameterViewModels[nameof(ATEcoRollReleaseLockupClutch)].AllowNullValue = true;

			Action<MultistageParameterViewModel> PassengerGroupEditingCallback = (MultistageParameterViewModel param) => {
				NumberOfPassengersEditingEnabled = param.EditingEnabled;
			};

			_parameterViewModels[nameof(NumberPassengerSeatsUpperDeck)].EditingChangedCallback =
				PassengerGroupEditingCallback;
			_parameterViewModels[nameof(NumberPassengerSeatsLowerDeck)].EditingChangedCallback =
				PassengerGroupEditingCallback;
			_parameterViewModels[nameof(NumberPassengersStandingLowerDeck)].EditingChangedCallback =
				PassengerGroupEditingCallback;
			_parameterViewModels[nameof(NumberPassengersStandingUpperDeck)].EditingChangedCallback =
				PassengerGroupEditingCallback;

			_parameterViewModels[nameof(AirdragModifiedEnum)].EditingChangedCallback = model => {
				AirdragModifiedMultistepEditingEnabled = model.EditingEnabled;
			};

			//Setup allowed values

			_parameterViewModels[nameof(VehicleCode)].AllowedItems =
				EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, VehicleCode>((TUGraz.VectoCommon.Models.VehicleCode
					.NOT_APPLICABLE));

			_parameterViewModels[nameof(LegislativeClass)].AllowedItems =
				EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, LegislativeClass>((TUGraz.VectoCommon.Models
					.LegislativeClass.M3));

			_parameterViewModels[nameof(RegisteredClass)].AllowedItems =
				EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, RegistrationClass>(RegistrationClass.unknown);

			_parameterViewModels[nameof(DoorDriveTechnology)].AllowedItems =
				EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, ConsumerTechnology>(
					ConsumerTechnology.Unknown);

			//Setup additional consolidatedVehicleData
			_parameterViewModels[nameof(EngineStopStartNullable)].PreviousContent =
				ConsolidatedVehicleData?.ADAS?.EngineStopStart;
			_parameterViewModels[nameof(EcoRollTypeNullable)].PreviousContent =
				ConsolidatedVehicleData?.ADAS?.EcoRoll;
			_parameterViewModels[nameof(PredictiveCruiseControlNullable)].PreviousContent =
				ConsolidatedVehicleData?.ADAS?.PredictiveCruiseControl;
			_parameterViewModels[nameof(ATEcoRollReleaseLockupClutch)].PreviousContent =
				ConsolidatedVehicleData?.ADAS?.ATEcoRollReleaseLockupClutch;

			//Set Mandatory Fields

			_parameterViewModels[nameof(Manufacturer)].Mandatory = true;
			_parameterViewModels[nameof(ManufacturerAddress)].Mandatory = true;
			_parameterViewModels[nameof(VIN)].Mandatory = true;
		}

		#region Overrides of ViewModelBase

		protected override bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			var propertyChanged = base.SetProperty(ref field, value, propertyName);

			if (propertyChanged && _parameterViewModels != null && _parameterViewModels.ContainsKey(propertyName)) {
				_parameterViewModels[propertyName].CurrentContent = value;
			}
			return propertyChanged;
		}



		#endregion

		public IVehicleDeclarationInputData ConsolidatedVehicleData
		{
			get { return _consolidatedVehicleData; }
			set { SetProperty(ref _consolidatedVehicleData, value); }
		}

		public void SetVehicleInputData(IVehicleDeclarationInputData vehicleInputData)
		{
			if (vehicleInputData.ExemptedVehicle != ExemptedVehicle) {
				throw new VectoException(ExemptedVehicle ? "Only exempted stage inputs are allowed" : "Exempted Vehicle not allowed");
			}

			if (ExemptedVehicle) {
				SetExemptedVehicleInputData(vehicleInputData);
				return;
			}


			Manufacturer = vehicleInputData.Manufacturer;
			Identifier = vehicleInputData.Identifier;
			ManufacturerAddress = vehicleInputData.ManufacturerAddress;
			VIN = vehicleInputData.VIN;
			Model = vehicleInputData.Model;
			LegislativeClass = vehicleInputData.LegislativeClass;
			CurbMassChassis = vehicleInputData.CurbMassChassis;
			GrossVehicleMassRating = vehicleInputData.GrossVehicleMassRating;
			AirdragModifiedMultistep = vehicleInputData.AirdragModifiedMultistep;
			TankSystem = vehicleInputData.TankSystem;
			RegisteredClass = vehicleInputData.RegisteredClass;
			NumberPassengerSeatsUpperDeck = vehicleInputData.NumberPassengerSeatsUpperDeck;
			NumberPassengerSeatsLowerDeck = vehicleInputData.NumberPassengerSeatsLowerDeck;
			NumberPassengersStandingLowerDeck = vehicleInputData.NumberPassengersStandingLowerDeck;
			NumberPassengersStandingUpperDeck = vehicleInputData.NumberPassengersStandingUpperDeck;
			VehicleCode = vehicleInputData.VehicleCode;
			LowEntry = vehicleInputData.LowEntry;
			MeasurementsGroupEditingEnabled =
				vehicleInputData.Height != null || 
				vehicleInputData.Width != null || 
				vehicleInputData.Length != null || 
				vehicleInputData.EntranceHeight != null;
			Height = vehicleInputData.Height;
			Width = vehicleInputData.Width;
			Length = vehicleInputData.Length;
			EntranceHeight = vehicleInputData.EntranceHeight;
			DoorDriveTechnology = vehicleInputData.DoorDriveTechnology;
			VehicleDeclarationType = vehicleInputData.VehicleDeclarationType;
			VehicleTypeApprovalNumber = vehicleInputData.VehicleTypeApprovalNumber;
			AdasEditingEnabled = vehicleInputData.ADAS != null;
			EngineStopStartNullable = vehicleInputData.ADAS?.EngineStopStart;
			EcoRollTypeNullable = vehicleInputData.ADAS?.EcoRoll;
			PredictiveCruiseControlNullable = vehicleInputData.ADAS?.PredictiveCruiseControl;
			ATEcoRollReleaseLockupClutch = vehicleInputData.ADAS?.ATEcoRollReleaseLockupClutch;
			AirdragModifiedMultistep = vehicleInputData.AirdragModifiedMultistep;
			foreach (var multistageParameterViewModel in _parameterViewModels.Values)
			{
				multistageParameterViewModel.UpdateEditingEnabled();
			}
			MultistageAirdragViewModel.SetAirdragInputData(vehicleInputData?.Components?.AirdragInputData);
			MultistageAuxiliariesViewModel.SetAuxiliariesInputData(vehicleInputData?.Components?.BusAuxiliaries);
			OnPropertyChanged(string.Empty);
		}

		private void SetExemptedVehicleInputData(IVehicleDeclarationInputData vehicleInputData)
		{
			Debug.Assert(vehicleInputData.ExemptedVehicle);
			Manufacturer = vehicleInputData.Manufacturer;
			Identifier = vehicleInputData.Identifier;
			ManufacturerAddress = vehicleInputData.ManufacturerAddress;
			VIN = vehicleInputData.VIN;
			Model = vehicleInputData.Model;
			LegislativeClass = vehicleInputData.LegislativeClass;
			CurbMassChassis = vehicleInputData.CurbMassChassis;
			GrossVehicleMassRating = vehicleInputData.GrossVehicleMassRating;
			RegisteredClass = vehicleInputData.RegisteredClass;
			NumberPassengerSeatsUpperDeck = vehicleInputData.NumberPassengerSeatsUpperDeck;
			NumberPassengerSeatsLowerDeck = vehicleInputData.NumberPassengerSeatsLowerDeck;
			VehicleCode = vehicleInputData.VehicleCode;
			Height = vehicleInputData.Height;
		}



		#region Implementation used fields in IVehicleInputData

		private string _manufacturer;
		private string _model;
		private string _vin;
		private string _manufacturerAddress;

		private bool _measurementsGroupEditingEnabled = false;
		private bool _numberOfPassengersEditingEnabled = false;
		private int? _numberOfPassengersUpperDeck;
		private int? _numberOfPassengersLowerDeck;
		private Kilogram _grossVehicleMassRating;

		private bool? _lowEntry;
		private VehicleCode? _vehicleCode;
		private RegistrationClass? _registeredClass;
		private bool? _airdragModifiedMultistep;
		private bool _airdragModifiedEditingEnabled = false;
		private LegislativeClass? _legislativeClass;
		private ConsumerTechnology? _doorDriveTechnology;
		private TankSystem? _tankSystem;
		private Kilogram _curbMassChassis;
		private ConvertedSI _lengthInMm;
		private ConvertedSI _heightMm;
		private ConvertedSI _widthInMm;
		private ConvertedSI _entranceHeightInMm;



		public string Manufacturer
		{
			get { return _manufacturer; }
			set
			{
				SetProperty(ref _manufacturer, value);
			}
		}


		public string Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}

		public string VIN
		{
			get { return _vin; }
			set { SetProperty(ref _vin, value); }
		}

		public string ManufacturerAddress
		{
			get { return String.IsNullOrEmpty(_manufacturerAddress) ? null : _manufacturerAddress; }
			set { SetProperty(ref _manufacturerAddress, value); }
		}

		#region Measurements
		public bool MeasurementsGroupEditingEnabled
		{
			get { return _measurementsGroupEditingEnabled; }
			set
			{
				if (SetProperty(ref _measurementsGroupEditingEnabled, value)) {
					_parameterViewModels[nameof(HeightInMm)].EditingEnabled = value;
					_parameterViewModels[nameof(LengthInMm)].EditingEnabled = value;
					_parameterViewModels[nameof(WidthInMm)].EditingEnabled = value;
					_parameterViewModels[nameof(EntranceHeightInMm)].EditingEnabled = value;
				}
			}
		}

		public ConvertedSI HeightInMm
		{
			get => _heightMm;
			set => SetProperty(ref _heightMm, value);
		}

		public ConvertedSI ConsolidatedHeightInMm
		{
			get => ConsolidatedVehicleData?.Height?.ConvertToMilliMeter();
			set => throw new NotImplementedException();
		}

		public Meter Height
		{
			get => HeightInMm?.ConvertToMeter();
			set => HeightInMm = value?.ConvertToMilliMeter();
		}

		public ConvertedSI LengthInMm
		{
			get { return _lengthInMm; }
			set => SetProperty(ref _lengthInMm, value);
		}

		public ConvertedSI ConsolidatedLengthInMm
		{
			get { return ConsolidatedVehicleData?.Length?.ConvertToMilliMeter(); }
			set { throw new NotImplementedException(); }
		}


		public Meter Length
		{
			get => LengthInMm?.ConvertToMeter();
			set => LengthInMm = value?.ConvertToMilliMeter();
		}

		public ConvertedSI WidthInMm
		{
			get => _widthInMm;
			set => SetProperty(ref _widthInMm, value);
		}

		public ConvertedSI ConsolidatedWidthInMm
		{
			get { return ConsolidatedVehicleData?.Width?.ConvertToMilliMeter(); }
			set { throw new NotImplementedException(); }
		}


		public Meter Width
		{
			get => WidthInMm?.ConvertToMeter();
			set => WidthInMm = value?.ConvertToMilliMeter();
		}

		public ConvertedSI ConsolidatedEntranceHeightInMm
		{
			get { return ConsolidatedVehicleData?.EntranceHeight?.ConvertToMilliMeter(); }
			set { throw new NotImplementedException(); }
		}

		public ConvertedSI EntranceHeightInMm
		{
			get => _entranceHeightInMm;
			set => SetProperty(ref _entranceHeightInMm, value);
		}


		public Meter EntranceHeight
		{
			get => EntranceHeightInMm?.ConvertToMeter();
			set => EntranceHeightInMm = value?.ConvertToMilliMeter();
		}

		#endregion
		public Kilogram CurbMassChassis //Corrected Actual Mass
		{
			get => _curbMassChassis;
			set => SetProperty(ref _curbMassChassis, value);
		}

		public bool NumberOfPassengersEditingEnabled
		{
			get => _numberOfPassengersEditingEnabled;
			set
			{
				if (SetProperty(ref _numberOfPassengersEditingEnabled, value)) {
					_parameterViewModels[nameof(NumberPassengerSeatsUpperDeck)].EditingEnabled = value;
					_parameterViewModels[nameof(NumberPassengerSeatsLowerDeck)].EditingEnabled = value;
					_parameterViewModels[nameof(NumberPassengersStandingUpperDeck)].EditingEnabled = value;
					_parameterViewModels[nameof(NumberPassengersStandingLowerDeck)].EditingEnabled = value;
				}
				
			}
		}


		public int? NumberPassengerSeatsUpperDeck
		{
			get => _numberOfPassengersUpperDeck;
			set => SetProperty(ref _numberOfPassengersUpperDeck, value);
		}

		public int? NumberPassengerSeatsLowerDeck
		{
			get => _numberOfPassengersLowerDeck;
			set => SetProperty(ref _numberOfPassengersLowerDeck, value);
		}

		public int? NumberPassengersStandingLowerDeck
		{
			get => _numberPassengersStandingLowerDeck;
			set => SetProperty(ref _numberPassengersStandingLowerDeck, value);
		}

		public int? NumberPassengersStandingUpperDeck
		{
			get => _numberPassengersStandingUpperDeck;
			set => SetProperty(ref _numberPassengersStandingUpperDeck, value);
		}


		public TankSystem? TankSystem
		{
			get { return _tankSystem; }
			set { SetProperty(ref _tankSystem, value); }
		}

		public MultistageParameterViewModel TankSystemVM
		{
			get => _parameterViewModels[nameof(TankSystem)];
		}

		public Kilogram GrossVehicleMassRating //Technical Permissible Maximum Laden Mass
		{
			get => _grossVehicleMassRating;
			set => SetProperty(ref _grossVehicleMassRating, value);
		}

		public ConsumerTechnology? DoorDriveTechnology
		{
			get => _doorDriveTechnology;
			set => SetProperty(ref _doorDriveTechnology, value);
		}

		public LegislativeClass? LegislativeClass
		{
			get => _legislativeClass;
			set => SetProperty(ref _legislativeClass, value);
		}

        #region AirdragModified

		private AIRDRAGMODIFIED _airdragmodifiedEnum;
        public AIRDRAGMODIFIED? AirdragModifiedEnum
		{
			get
			{
				return _airdragModifiedMultistep.toAirdragModifiedEnum();
			}
			set
			{
				var prevVal = AirdragModifiedMultistep;
				var newVal = value?.toNullableBool();
				if (prevVal != newVal) {
					AirdragModifiedMultistep = value?.toNullableBool();
				}
				if (_parameterViewModels.ContainsKey(nameof(AirdragModifiedEnum)))
				{
					_parameterViewModels[nameof(AirdragModifiedEnum)].CurrentContent = value;
				}
			}
		}

		public AIRDRAGMODIFIED? ConsolidatedAirdragModifiedEnum
		{
			get
			{
				if (_consolidatedVehicleData?.AirdragModifiedMultistep != null) {
					return _consolidatedVehicleData.AirdragModifiedMultistep.toAirdragModifiedEnum();
				} else {
					return null;
				}
			}
			set => throw new NotImplementedException();
		}


		public bool? AirdragModifiedMultistep
		{
			get
			{
				return _airdragModifiedMultistep;
			}
			set
			{
				if (SetProperty(ref _airdragModifiedMultistep, value)) {
					if(value == false){
						MultistageAirdragViewModel.AirDragViewModel = null;
					} else {
						MultistageAirdragViewModel.RestoreAirdragViewModel();
					}
					AirdragModifiedEnum = value.toAirdragModifiedEnum();
				};
			}
		}

		public bool AirdragModifiedMultistepMandatory
		{
			get => _airdragModifiedMultistepMandatory;
			set => SetProperty(ref _airdragModifiedMultistepMandatory, value);
		}

		public bool AirdragModifiedMultistepEditingEnabled
		{
			get
			{
				return _airdragModifiedEditingEnabled;
			}
			set
			{
				var val = value;
				if (AirdragModifiedMultistepMandatory) {
					val = true;
				} else {
					val = false;
				}
				if (SetProperty(ref _airdragModifiedEditingEnabled, val)) {
					
				}
				_parameterViewModels[nameof(AirdragModifiedEnum)].EditingEnabled = val;
			}
		}

		#endregion;

		public string ExemptedTechnology { get; }

		public RegistrationClass? RegisteredClass
		{
			get => _registeredClass;
			set => SetProperty(ref _registeredClass, value);
		}

		public VehicleCode? VehicleCode
		{
			get => _vehicleCode;
			set => SetProperty(ref _vehicleCode, value);
		}

			

        public bool? LowEntry
		{
			get => _lowEntry;
			set => SetProperty(ref _lowEntry, value);
		}

		



		public VehicleDeclarationType VehicleDeclarationType
		{
			get => _vehicleDeclarationType;
			set => SetProperty(ref _vehicleDeclarationType, value);
		}

		public Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits { get; }
		public TableData BoostingLimitations { get; }

		private string _vehicleTypeApprovalNumber;

		public string VehicleTypeApprovalNumber
		{
			get => _vehicleTypeApprovalNumber;
			set => SetProperty(ref _vehicleTypeApprovalNumber, value);
		}

		public ArchitectureID ArchitectureID { get; }
		public bool OvcHev { get; }
		public Watt MaxChargingPower { get; }

		#endregion


		private string _identifier;
		private IVehicleDeclarationInputData _consolidatedVehicleData;
		private VehicleDeclarationType _vehicleDeclarationType;


		#region implementation of IVehicleComponentsDeclaration

		public IAirdragDeclarationInputData AirdragInputData
		{
			get => MultistageAirdragViewModel?.AirDragViewModel;
		}

		public IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get => MultistageAuxiliariesViewModel != null &&  MultistageAuxiliariesViewModel.HasValues ? MultistageAuxiliariesViewModel : null;
		}

		#region not implemented

		public IGearboxDeclarationInputData GearboxInputData => throw new NotImplementedException();

		public ITorqueConverterDeclarationInputData TorqueConverterInputData => throw new NotImplementedException();

		public IAxleGearInputData AxleGearInputData => throw new NotImplementedException();

		public IAngledriveInputData AngledriveInputData => throw new NotImplementedException();

		public IEngineDeclarationInputData EngineInputData => throw new NotImplementedException();

		public IAuxiliariesDeclarationInputData AuxiliaryInputData => throw new NotImplementedException();

		public IRetarderInputData RetarderInputData => throw new NotImplementedException();

		public IPTOTransmissionInputData PTOTransmissionInputData => throw new NotImplementedException();

		public IAxlesDeclarationInputData AxleWheels => throw new NotImplementedException();


		public IElectricStorageSystemDeclarationInputData ElectricStorage => throw new NotImplementedException();

		public IElectricMachinesDeclarationInputData ElectricMachines => throw new NotImplementedException();
		public IIEPCDeclarationInputData IEPC { get; }

		#endregion

		#endregion



		#region implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		private PredictiveCruiseControlType _predictiveCruiseControl;
		private bool? _atEcoRollReleaseLockupClutch;
		private EcoRollType _ecoRoll;
		private bool _engineStopStart;
		private bool _adasEditingEnabled;
		private bool? _engineStopStartNullable;
		private EcoRollType? _ecoRollTypeNullable;
		private PredictiveCruiseControlType? _predictiveCruiseControlNullable;
		


		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get
			{
				if (EngineStopStartNullable.HasValue 
					|| EcoRollTypeNullable.HasValue
					|| PredictiveCruiseControlNullable.HasValue) {
					return this;
				} else {
					return null;
				}
			;
		}

				
			
		}

		public bool AdasEditingEnabled
		{
			get => _adasEditingEnabled;
			set
			{
				if(SetProperty(ref _adasEditingEnabled, value)) {
					_parameterViewModels[nameof(EcoRollTypeNullable)].EditingEnabled = value;
					_parameterViewModels[nameof(ATEcoRollReleaseLockupClutch)].EditingEnabled = value;
					_parameterViewModels[nameof(EngineStopStartNullable)].EditingEnabled = value;
					_parameterViewModels[nameof(PredictiveCruiseControlNullable)].EditingEnabled = value;
				}
			}
		}

		public bool? EngineStopStartNullable
		{
			get => _engineStopStartNullable;
			set
			{
				SetProperty(ref _engineStopStartNullable, value);
			}
		}

		public bool EngineStopStart
		{
			get
			{
				return _engineStopStartNullable.HasValue ? _engineStopStartNullable.Value : false;
			}
		}

		public EcoRollType? EcoRollTypeNullable
		{
			get => _ecoRollTypeNullable;
			set
			{
				SetProperty(ref _ecoRollTypeNullable, value);
			}
		}

		public EcoRollType EcoRoll
		{
			get
			{
				return _ecoRollTypeNullable.HasValue ? _ecoRollTypeNullable.Value : EcoRollType.None;
			}
		}

		public PredictiveCruiseControlType? PredictiveCruiseControlNullable
		{
			get => _predictiveCruiseControlNullable;
			set
			{
				SetProperty(ref _predictiveCruiseControlNullable, value);
			}
		}

		public PredictiveCruiseControlType PredictiveCruiseControl
		{
			get
			{
				return _predictiveCruiseControlNullable.HasValue
					? _predictiveCruiseControlNullable.Value
					: PredictiveCruiseControlType.None;
			}
		}

		public bool? ATEcoRollReleaseLockupClutch
		{
			get => _atEcoRollReleaseLockupClutch;
			set
			{
				SetProperty(ref _atEcoRollReleaseLockupClutch, value);
			}
		}

		#endregion


		#region implementation of IVehicleDeclarationInputData;

		public DateTime Date
		{
			get => DateTime.Today;
		}

		public string AppVersion
		{
			get { throw new NotImplementedException(); }
		}

		public CertificationMethod CertificationMethod
		{
			get { throw new NotImplementedException(); }
		}

		public string CertificationNumber
		{
			get { throw new NotImplementedException(); }
		}

		public DigestData DigestValue
		{
			get { throw new NotImplementedException(); }
		}

		public string Identifier
		{
			get
			{
				return _identifier;
			}
			private set
			{
				SetProperty(ref _identifier, value);
			}
		}

		public bool ExemptedVehicle
		{
			get => _exemptedVehicle;
			//set => SetProperty(ref _exemptedVehicle, value);
		}


		public VehicleCategory VehicleCategory
		{
			get { throw new NotImplementedException(); }
		}

		public AxleConfiguration AxleConfiguration
		{
			get { throw new NotImplementedException(); }
		}




		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get { throw new NotImplementedException(); }
		}


		public PerSecond EngineIdleSpeed
		{
			get { throw new NotImplementedException(); }
		}

		public bool VocationalVehicle
		{
			get { throw new NotImplementedException(); }
		}

		public bool? SleeperCab
		{
			get { throw new NotImplementedException(); }
		}







		public bool ZeroEmissionVehicle
		{
			get { throw new NotImplementedException(); }
		}

		public bool HybridElectricHDV
		{
			get { throw new NotImplementedException(); }
		}

		public bool DualFuelVehicle
		{
			get { throw new NotImplementedException(); }
		}

		public Watt MaxNetPower1
		{
			get { throw new NotImplementedException(); }
		}

		public Watt MaxNetPower2
		{
			get { throw new NotImplementedException(); }
		}



		public CubicMeter CargoVolume
		{
			get { throw new NotImplementedException(); }
		}


		public bool Articulated
		{
			get { throw new NotImplementedException(); }
		}





		public IVehicleComponentsDeclaration Components
		{
			get
			{
				if (AirdragInputData != null || BusAuxiliaries != null) {
					return this;
				} else {
					return null;
				}
			}
		}

		
		public XmlNode XMLSource
		{
			get { throw new NotImplementedException(); }
		}

		public IPTOViewModel PTOViewModel
		{
			get { throw new NotImplementedException(); }
		}

		public RetarderType RetarderType
		{
			get { throw new NotImplementedException(); }
		}

		public double RetarderRatio
		{
			get { throw new NotImplementedException(); }
		}

		public AngledriveType AngledriveType
		{
			get { throw new NotImplementedException(); }
		}

		#endregion;



		#region Implementation of IDataErrorInfo

		public Dictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();

		public string this[string propertyName]
		{
			get
			{
				string result = null;
				switch (propertyName) {
					case nameof(Manufacturer):
						if (string.IsNullOrWhiteSpace(Manufacturer)) {
							result = "Manufacturer must not be empty";
						}
						
						break;
					case nameof(ManufacturerAddress):
						if (string.IsNullOrWhiteSpace(ManufacturerAddress))
						{
							result = "Manufacturer address must not be empty";
						}
						break;
					case nameof(VIN):
						if (string.IsNullOrEmpty(VIN)) {
							result = "VIN must not be empty";
						}
						break;
					case nameof(AirdragModifiedEnum):
						if (AirdragModifiedMultistepEditingEnabled && (AirdragModifiedEnum == AIRDRAGMODIFIED.UNKNOWN)) {
							result = "Air drag modified has to be set";
						}
						break;
					case nameof(EcoRollTypeNullable):
					case nameof(EngineStopStartNullable):
					case nameof(PredictiveCruiseControlNullable):
                    case nameof(ATEcoRollReleaseLockupClutch): // only required for AT transmission
                        if (AdasEditingEnabled == true && (!_parameterViewModels[propertyName].AllowNullValue && this.GetType().GetProperty(propertyName).GetValue(this) == null)) {
							result = $"{NameResolver.ResolveName(propertyName, BusStrings.ResourceManager, Strings.ResourceManager)} has to be set if editing is enabled.";
						}
						break;
					default:
						if (_parameterViewModels[propertyName].EditingEnabled && !_parameterViewModels[propertyName].AllowNullValue) {
							var propertyValue = this.GetType().GetProperty(propertyName)?.GetValue(this);
							if (propertyValue == null) {
								result =
									$"{NameResolver.ResolveName(propertyName, BusStrings.ResourceManager, Strings.ResourceManager)} has to be set if editing is enabled.";
							} else { 
								if (propertyValue is string value && string.IsNullOrWhiteSpace(value))
								{
									result =
										$"{NameResolver.ResolveName(propertyName, BusStrings.ResourceManager, Strings.ResourceManager)} has to be set if editing is enabled.";
								}
							}
						}

						break;
				}
				//https://www.youtube.com/watch?v=5KF0GGObuAQ

				if (result == null) {
					if(Errors.ContainsKey(propertyName))
					Errors.Remove(propertyName);
				} else {
					Errors[propertyName] = result;
				}
				

				return result;
			}
		}

		public string Error
		{
			get => String.Join(",", Errors.Values);
		}
		public bool HasErrors =>
			!string.IsNullOrEmpty(Error) || 
			(MultistageAuxiliariesViewModel != null && MultistageAuxiliariesViewModel.HasErrors);

		#endregion

		private bool _airdragModifiedMultistepMandatory;
		private int? _numberPassengersStandingLowerDeck;
		private int? _numberPassengersStandingUpperDeck;
		private bool _exemptedVehicle;

	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle
{
	/// <summary>
	/// Abstract base class for Vehicle View Models. Implements Common Methods and the IVehicleViewModel Interface.
	/// All Properties of IVehicleDeclarationInputData throw an NotImplementedException and should be implemented in the derived classes.
	///  
	/// </summary>
    public abstract class VehicleViewModel : ViewModelBase, IVehicleViewModel
    {
        private static readonly string _name = "Vehicle";
        protected IXMLDeclarationVehicleData _vehicleInputData;
        public string Name => _name;
		protected bool _isPresent;
		public bool IsPresent => _isPresent;



		protected readonly IComponentViewModelFactory _componentViewModelFactory;

		protected ICommonComponentViewModel _commonComponentViewModel;

		//These ViewModels are displayed in the JobEditViewModel

		public ObservableCollection<IComponentViewModel> ComponentViewModels { get; set; } = new ObservableCollection<IComponentViewModel>();


		/// <summary>
		/// Gets the Data from the InputDataProvider and sets the properties of the viewModel. Should be overridden in the dervived Classes.
		/// </summary>
		protected virtual void CreateVehicleProperties() { throw new NotImplementedException(); }

		public IXMLDeclarationVehicleData InputData => _vehicleInputData;

		public virtual IPTOViewModel PTOViewModel
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}




		public VehicleViewModel(
			IXMLDeclarationVehicleData vehicleDeclarationInputData,
			IComponentViewModelFactory componentViewModelFactory)
		{
            _vehicleInputData = vehicleDeclarationInputData;
			_componentViewModelFactory = componentViewModelFactory;
			_isPresent = true;


			_commonComponentViewModel = _componentViewModelFactory.CreateCommonComponentViewModel(_vehicleInputData);

			CreateVehicleProperties();
        }

		public ICommonComponentViewModel CommonComponentViewModel
		{
			get => _commonComponentViewModel;
			set => SetProperty(ref _commonComponentViewModel, value);
		}


		#region Implementation of IVehicleViewModel, IVehicleDeclarationInputData
		protected string _identifier;
		protected string _vin;
		protected LegislativeClass _legislativeClass;
		protected VehicleCategory _vehicleCategory;
		protected AxleConfiguration _axleConfiguration;
		protected Kilogram _curbMassChassis;
		protected Kilogram _grossVehicleMassRating;
		protected string _manufacturerAddress;
		protected IComponentsViewModel _components;


		public string Manufacturer
		{
			get => _commonComponentViewModel.Manufacturer;
			set => _commonComponentViewModel.Manufacturer = value;
		}

		public string Model
		{
			get => _commonComponentViewModel.Model;
			set => _commonComponentViewModel.Model = value;
		}

		public DateTime Date
		{
			get => _commonComponentViewModel.Date;
			set => _commonComponentViewModel.Date = value;
		}

		public string CertificationNumber
		{
			get => _commonComponentViewModel.CertificationNumber;
			set => _commonComponentViewModel.CertificationNumber = value;
		}

		public CertificationMethod CertificationMethod
		{
			get => _commonComponentViewModel.CertificationMethod;
			set => _commonComponentViewModel.CertificationMethod = value;
		}

		public bool SavedInDeclarationMode
		{
			get => _commonComponentViewModel.SavedInDeclarationMode;
			set => _commonComponentViewModel.SavedInDeclarationMode = value;
		}

		public DigestData DigestValue
		{
			get => _commonComponentViewModel.DigestValue;
			set => _commonComponentViewModel.DigestValue = value;
		}

		public DataSource DataSource
		{
			get => _commonComponentViewModel.DataSource;
			set => _commonComponentViewModel.DataSource = value;
		}

		public string AppVersion => _commonComponentViewModel.AppVersion;


		public virtual IPTOTransmissionInputData PTOTransmissionInputData => throw new NotImplementedException();

		public virtual RetarderType RetarderType
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual double RetarderRatio
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual AngledriveType AngledriveType
		{
			get => throw new NotImplementedException(); 
			set => throw new NotImplementedException();
		}

		public virtual string Identifier
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual bool ExemptedVehicle
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual string VIN
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		LegislativeClass? IVehicleDeclarationInputData.LegislativeClass { get; }

		public virtual LegislativeClass LegislativeClass
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual VehicleCategory VehicleCategory
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual Kilogram CurbMassChassis
		{ 
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual Kilogram GrossVehicleMassRating
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IList<ITorqueLimitInputData> TorqueLimits
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual string ManufacturerAddress
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual PerSecond EngineIdleSpeed
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual bool VocationalVehicle
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual bool? SleeperCab
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public bool? AirdragModifiedMultistep => throw new NotImplementedException();

		public virtual TankSystem? TankSystem
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual bool ZeroEmissionVehicle
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual bool HybridElectricHDV
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual bool DualFuelVehicle
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual Watt MaxNetPower1
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual Watt MaxNetPower2
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public string ExemptedTechnology { get; }

		RegistrationClass? IVehicleDeclarationInputData.RegisteredClass { get; }
		int? IVehicleDeclarationInputData.NumberPassengerSeatsUpperDeck { get; }
		int? IVehicleDeclarationInputData.NumberPassengerSeatsLowerDeck { get; }

		public int? NumberPassengersStandingLowerDeck => throw new NotImplementedException();

		public int? NumberPassengersStandingUpperDeck => throw new NotImplementedException();

		public virtual RegistrationClass RegisteredClass
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual int NumberOfPassengersUpperDeck
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual int NumberOfPassengersLowerDeck
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual CubicMeter CargoVolume
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		VehicleCode? IVehicleDeclarationInputData.VehicleCode { get; }
		bool? IVehicleDeclarationInputData.LowEntry { get; }

		public virtual VehicleCode VehicleCode
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual bool LowEntry
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual bool Articulated
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual Meter Height
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual Meter Length
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual Meter Width
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual Meter EntranceHeight
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		ConsumerTechnology? IVehicleDeclarationInputData.DoorDriveTechnology { get; }

		public VehicleDeclarationType VehicleDeclarationType => throw new NotImplementedException();
		public Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits { get; }
		public TableData BoostingLimitations { get; }

		public string VehicleTypeApprovalNumber => throw new NotImplementedException();
		public ArchitectureID ArchitectureID { get; }
		public bool OvcHev { get; }
		public Watt MaxChargingPower { get; }

		public virtual ConsumerTechnology DoorDriveTechnology
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual IVehicleComponentsDeclaration Components
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual XmlNode XMLSource
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		#endregion
	}

    public class VehicleViewModel_v1_0 : VehicleViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationVehicleDataProviderV10).FullName;







        public VehicleViewModel_v1_0(
            IXMLDeclarationVehicleData inputData,
			IComponentViewModelFactory componentViewModelFactory): 
            base(
                inputData,
				componentViewModelFactory)
		{
			

		}

        protected override void CreateVehicleProperties()
        {
            throw new NotImplementedException();
		}

    }
	/// <summary>
	/// Class that Represents the ViewModel for the VehicleDeclarationType v2.0
	/// </summary>
    public class VehicleViewModel_v2_0 : VehicleViewModel_v1_0
    {
		public new static readonly string VERSION = typeof(XMLDeclarationVehicleDataProviderV20).FullName;
		private IAdasViewModel _aDASViewModel;
		private PerSecond _engineIdleSpeed;


		#region Implementation of present Properties

		/*
		public override IList<ITorqueLimitInputData> TorqueLimits
        {
            get => _torqueLimits;
            set => SetProperty(ref _torqueLimits, value);
        }

		public IAdasViewModel ADASViewModel
        {
            get => _aDASViewModel;
            set => SetProperty(ref _aDASViewModel, value);
        }
		*/


        public override string ManufacturerAddress
        {
            get => _manufacturerAddress;
			set => SetProperty(ref _manufacturerAddress, value);
		}


		public override VehicleCategory VehicleCategory
        {
            get => _vehicleCategory;
			set => SetProperty(ref _vehicleCategory, value);
		}

        public override AxleConfiguration AxleConfiguration
        {
            get => _axleConfiguration;
			set => SetProperty(ref _axleConfiguration, value);
		}

		public override Kilogram CurbMassChassis
        {
            get => _curbMassChassis;
			set => SetProperty(ref _curbMassChassis, value);
		}

		
        public override Kilogram GrossVehicleMassRating
        {
            get => _grossVehicleMassRating;
			set => SetProperty(ref _grossVehicleMassRating, value);
		}


		public override PerSecond EngineIdleSpeed
		{
			get => _engineIdleSpeed;
			set => SetProperty(ref _engineIdleSpeed, value);
		}


		public override AngledriveType AngledriveType
		{
			get => _components.AngleDriveViewModel.Type;
			set => _components.AngleDriveViewModel.Type = value;
		}

        public override RetarderType RetarderType
		{
			get => _components.RetarderViewModel.Type;
			set => _components.RetarderViewModel.Type = value;
		}

		public override double RetarderRatio
		{
			get => _components.RetarderViewModel.Ratio;
			set => _components.RetarderViewModel.Ratio = value;
		}
		/*
		public override IPTOViewModel PTOViewModel => _components.PTOViewModel;

		public override IPTOTransmissionInputData PTOTransmissionInputData => 
			(IPTOTransmissionInputData)PTOViewModel;
		*/
		public override string VIN
		{
			get => _vin;
			set => SetProperty(ref _vin, value);
		}

		public override LegislativeClass LegislativeClass
		{
			get => _legislativeClass;
			set => SetProperty(ref _legislativeClass, value);
		}



		public override IVehicleComponentsDeclaration Components
		{
			get => _components;
			set => throw new NotImplementedException();
		}



		public override string Identifier { get => _identifier; 
			set => SetProperty(ref _identifier, value); }

		#endregion


		public VehicleViewModel_v2_0(
			IXMLDeclarationVehicleData inputData,
			IComponentViewModelFactory componentViewModelFactory) :
            base(inputData,
				componentViewModelFactory)
		{

		}

		protected override void CreateVehicleProperties()
		{
			Debug.Assert(_vehicleInputData.LegislativeClass.HasValue);

			_manufacturerAddress = _vehicleInputData.ManufacturerAddress;
			_vin = _vehicleInputData.VIN;
			_legislativeClass = (LegislativeClass) _vehicleInputData.LegislativeClass;
			_vehicleCategory = _vehicleInputData.VehicleCategory;
			_axleConfiguration = _vehicleInputData.AxleConfiguration;
			_curbMassChassis = _vehicleInputData.CurbMassChassis;
			_grossVehicleMassRating = _vehicleInputData.GrossVehicleMassRating;
			_identifier = _vehicleInputData.Identifier;


			//_aDASViewModel = (IComponentViewModel)_componentViewModelFactory.CreateAdasViewModel(_vehicleInputData.ADAS as IAdvancedDriverAssistantSystemDeclarationInputData) as IAdasViewModel;

			//TorqueLimits = _vehicleInputData.TorqueLimits;

			_components = _componentViewModelFactory.CreateComponentsViewModel(
				_vehicleInputData.Components as IXMLVehicleComponentsDeclaration);

			//Add the viewmodel of every component to the ComponentViewModels collection
			foreach (var component in ((IComponentsViewModel)_components).Components) {
				ComponentViewModels.Add(component);
			}

			EngineIdleSpeed = _vehicleInputData.EngineIdleSpeed;
			RetarderType = _vehicleInputData.RetarderType;
			RetarderRatio = _vehicleInputData.RetarderRatio;
			AngledriveType = _vehicleInputData.AngledriveType;

		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter;
using VECTO3GUI.ViewModel.Interfaces;
using Component = VECTO3GUI.Util.Component;

namespace VECTO3GUI.ViewModel.Impl
{
	public class VehicleViewModel : AbstractViewModel, IVehicleViewModel
	{
		private string _manufacturer;
		private string _manufacturerAddress;
		private string _model;
		private string _vin;
		private DateTime _date;
		private LegislativeClass _legislativeClass;
		private VehicleCategory _vehicleCategory;
		private AxleConfiguration _axleConfiguration;
		private Kilogram _curbMassChassis;
		private Kilogram _grossVehicleMass;
		private PerSecond _idlingSpeed;
		private RetarderType _retarderType;
		private double _retarderRatio;
		private AngledriveType _angledriveType;
		private string _ptoTechnology;
		private bool _hasDedicatedRetarder;
		private VehicleClass? _vehicleClass;
		private readonly ObservableCollection<TorqueEntry> _torqueLimits = new ObservableCollection<TorqueEntry>();

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		public VehicleViewModel()
		{
			PropertyChanged += UpdateVehicleClass;
			PropertyChanged += UpdateComponents;
		}

		private void UpdateComponents(object sender, PropertyChangedEventArgs e)
		{
			var relevantProperties = new[] { "RetarderType", "AngledriveType", "TransmissionType" };
			if (!relevantProperties.Contains(e.PropertyName)) {
				return;
			}

			var gbxModel = GetComponentViewModel(Component.Gearbox);
			var gearboxType = gbxModel != null && gbxModel is IGearboxViewModel
				? ((IGearboxViewModel)gbxModel).TransmissionType
				: GearboxType.MT;
			var components = GetComponents(VehicleCategory,
				gearboxType, RetarderType, AngledriveType).ToList();
			DoUpdateComponents(components);
		}

		private void DoUpdateComponents(IList<Component> components)
		{
			var currentComponents = GetSubmodels().ToArray();
			foreach (var comp in components) {
				if (currentComponents.Contains(comp)) {
					continue;
				}

				var newModelType = ViewModelFactory.ComponentViewModelMapping[comp];
				var newModel = (IComponentViewModel)Kernel.Get(newModelType);
				if (newModel is IGearboxViewModel && newModel is ObservableObject) {
					(newModel as ObservableObject).PropertyChanged += UpdateComponents;
				}
				RegisterSubmodel(comp, newModel);
			}
			foreach (var comp in currentComponents) {
				if (components.Contains(comp))
					continue;

				UnregisterSubmodel(comp);
			}
		}

		private IEnumerable<Component> GetComponents(VehicleCategory vehicleCategory, GearboxType gbxType, RetarderType retarderType, AngledriveType angledrive)
		{
			var retVal = new List<Component>() {
				Component.Engine,
				Component.Gearbox,
				Component.Airdrag,
				Component.Axlegear,
				Component.Axles,
			};

			if (gbxType.AutomaticTransmission()) {
				retVal.Add(Component.TorqueConverter);
			}
			if (retarderType.IsDedicatedComponent()) {
				retVal.Add(Component.Retarder);
			}
			if (angledrive == AngledriveType.SeparateAngledrive) {
				retVal.Add(Component.Angledrive);
			}
			if (vehicleCategory != VehicleCategory.RigidTruck && vehicleCategory != VehicleCategory.Tractor) {
				retVal.Add(Component.BusAuxiliaries);
			} else {
				retVal.Add(Component.Auxiliaries);
			}
			retVal.Sort();
			return retVal;
		}

		private void UpdateVehicleClass(object sender, PropertyChangedEventArgs e)
		{
			var dependentProperties = new[] { "VehicleCategory", "AxleConfiguration", "GrossVehicleMass" };
			if (!dependentProperties.Contains(e.PropertyName)) {
				return;
			}

			try {
				//ToDo
				//var seg = DeclarationData.Segments.Lookup(
				//	VehicleCategory, AxleConfiguration, GrossVehicleMass, 0.SI<Kilogram>(), false);
				//VehicleClass = seg.VehicleClass;
			} catch (Exception ) {
				VehicleClass = null;
			}
		}

		public IVehicleDeclarationInputData ModelData
		{
			get { return AdapterFactory.VehicleDeclarationAdapter(this); }
		}


		private void SetValues(IVehicleDeclarationInputData vehicle)
		{
			Manufacturer = vehicle.Manufacturer;
			ManufacturerAddress = vehicle.ManufacturerAddress;
			Model = vehicle.Model;
			VIN = vehicle.VIN;
			Date = vehicle.Date;
			LegislativeClass = vehicle.LegislativeClass;
			VehicleCategory = vehicle.VehicleCategory;
			AxleConfiguration = vehicle.AxleConfiguration;
			CurbMassChassis = vehicle.CurbMassChassis;
			GrossVehicleMass = vehicle.GrossVehicleMassRating;
			IdlingSpeed = vehicle.EngineIdleSpeed;
			RetarderType = vehicle.Components.RetarderInputData.Type;
			if (RetarderType.IsDedicatedComponent()) {
				RetarderRatio = vehicle.Components.RetarderInputData.Ratio;
			}
			PTOTechnology = vehicle.Components.PTOTransmissionInputData.PTOTransmissionType;
			AngledriveType = vehicle.Components.AngledriveInputData.Type;
			var torqueLimits = vehicle.TorqueLimits.ToList();
			torqueLimits.Sort((entry1, entry2) => entry1.Gear.CompareTo(entry2.Gear));
			TorqueLimits.Clear();
			foreach (var entry in torqueLimits) {
				TorqueLimits.Add(new TorqueEntry(TorqueLimits, entry));
			}
		}

		public string Manufacturer
		{
			get { return _manufacturer; }
			set {
				//ValidateProperty(value);
				SetProperty(ref _manufacturer, value);
			}
		}

		public string ManufacturerAddress
		{
			get { return _manufacturerAddress; }
			set { SetProperty(ref _manufacturerAddress, value); }
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

		public DateTime Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}

		public LegislativeClass LegislativeClass
		{
			get { return _legislativeClass; }
			set { SetProperty(ref _legislativeClass, value); }
		}

		public AllowedEntry<LegislativeClass>[] AllowedLegislativeClasses
		{
			get
			{
				return null;
				//ToDo
				//var allowed = DeclarationMode ? 
				//	DeclarationData.Segments.GetLegislativeClasses()
				//	: Enum.GetValues(typeof(LegislativeClass)).Cast<LegislativeClass>();

				//return allowed.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();
			}
		}

		public VehicleCategory VehicleCategory
		{
			get { return _vehicleCategory; }
			set { SetProperty(ref _vehicleCategory, value); }
		}

		public AllowedEntry<VehicleCategory>[] AllowedVehicleCategories
		{
			get
			{
				return null;
				//ToDo
				//var categories = DeclarationMode ?  
				//	DeclarationData.Segments.GetVehicleCategories()
				//	: Enum.GetValues(typeof(VehicleCategory)).Cast<VehicleCategory>();
				//return categories.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();
			}
		}

		public AxleConfiguration AxleConfiguration
		{
			get { return _axleConfiguration; }
			set { SetProperty(ref _axleConfiguration, value); }
		}

		public AllowedEntry<AxleConfiguration>[] AllowedAxleConfigurations
		{
			get
			{
				return null;
				//ToDo
				//var axleConfigs = DeclarationMode
				//	? DeclarationData.Segments.GetAxleConfigurations()
				//	: Enum.GetValues(typeof(AxleConfiguration)).Cast<AxleConfiguration>();
				//return axleConfigs.Select(ac => AllowedEntry.Create(ac, ac.GetName())).ToArray();
			}
		}

		public VehicleClass? VehicleClass
		{
			get { return _vehicleClass; }
			private set { SetProperty(ref _vehicleClass, value); }
		}

		[VectoParameter(typeof(VehicleData), "CurbWeight")]
		public Kilogram CurbMassChassis
		{
			get { return _curbMassChassis; }
			set { SetProperty(ref _curbMassChassis, value); }
		}

		[VectoParameter(typeof(VehicleData), "GrossVehicleWeight")]
		public Kilogram GrossVehicleMass
		{
			get { return _grossVehicleMass; }
			set { SetProperty(ref _grossVehicleMass, value); }
		}

		[VectoParameter(typeof(CombustionEngineData), "IdleSpeed")]
		public PerSecond IdlingSpeed
		{
			get { return _idlingSpeed; }
			set { SetProperty(ref _idlingSpeed, value); }
		}

		public RetarderType RetarderType
		{
			get { return _retarderType; }
			set {
				SetProperty(ref _retarderType, value);
				HasDedicatedRetarder = _retarderType.IsDedicatedComponent();
			}
		}

		public AllowedEntry<RetarderType>[] AllowedRetarderTypes
		{
			get {
				return Enum.GetValues(typeof(RetarderType)).Cast<RetarderType>()
					.Select(rt => AllowedEntry.Create(rt, rt.GetLabel()) ).ToArray();
			}
		}

		public bool HasDedicatedRetarder
		{
			get { return _hasDedicatedRetarder; }
			private set { SetProperty(ref _hasDedicatedRetarder, value); }
		}

		[VectoParameter(typeof(RetarderData), "Ratio")]
		public double RetarderRatio
		{
			get { return _retarderRatio; }
			set { SetProperty(ref _retarderRatio, value); }
		}

		public AngledriveType AngledriveType
		{
			get { return _angledriveType; }
			set { SetProperty(ref _angledriveType, value); }
		}

		public AllowedEntry<AngledriveType>[] AllowedAngledriveTypes
		{
			get {
				return Enum.GetValues(typeof(AngledriveType)).Cast<AngledriveType>()
							.Select(at => AllowedEntry.Create(at, at.GetLabel())).ToArray();
			}
		}


		public string PTOTechnology
		{
			get { return _ptoTechnology; }
			set { SetProperty(ref _ptoTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedPTOTechnologies
		{
			get { return DeclarationData.PTOTransmission.GetTechnologies().Select(pt => AllowedEntry.Create(pt, pt)).ToArray(); }
			
		}

		public ObservableCollection<TorqueEntry> TorqueLimits
		{
			get { return _torqueLimits; }
		}

		public ICommand AddTorqueLimit { get {return new RelayCommand(DoAddTorqueLimit);} }

		private void DoAddTorqueLimit()
		{
			TorqueLimits.Add(new TorqueEntry(TorqueLimits, 0, null));
		}

		public ICommand RemoveTorqueLimit { get {return new RelayCommand<object>(DoRemoveTorqueLimit, CanRemoveTorqueLimit);} }

		private void DoRemoveTorqueLimit(object selected)
		{
			var list = (IList)selected;
			var selectedEntries = list.Cast<TorqueEntry>().ToArray();

			foreach (var entry in selectedEntries) {
				TorqueLimits.Remove(entry);
			}
		}

		private bool CanRemoveTorqueLimit(object selected)
		{
			var list = (IList)selected;
			return list.Count > 0;
		}


		#region Overrides of AbstractEditComponentViewModel

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle));
		}

		#endregion
	}
}

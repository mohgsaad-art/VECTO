using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Adapter.Declaration
{
	public class VehicleDeclarationAdapter : IVehicleDeclarationInputData, IPTOTransmissionInputData, IVehicleComponentsDeclaration, IAxlesDeclarationInputData
	{
		protected IVehicleViewModel ViewModel;
		private DateTime _date;
		private IAuxiliariesDeclarationInputData _auxiliaryInputData;

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		public VehicleDeclarationAdapter(IVehicleViewModel vehicleViewModel)
		{
			ViewModel = vehicleViewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }

		public string Manufacturer
		{
			get { return ViewModel.Manufacturer; }
		}

		public string Model
		{
			get { return ViewModel.Model; }
		}

		DateTime IComponentInputData.Date => _date;

		public string AppVersion { get; }

		public string Date
		{
			get { return ViewModel.Date.ToString(); }
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

		#endregion

		#region Implementation of IVehicleDeclarationInputData

		public string Identifier { get; }
		public bool ExemptedVehicle { get; }

		public string VIN
		{
			get { return ViewModel.VIN; }
		}

		public LegislativeClass LegislativeClass { get {return ViewModel.LegislativeClass; } }
		public VehicleCategory VehicleCategory { get { return ViewModel.VehicleCategory; } }
		public AxleConfiguration AxleConfiguration { get { return ViewModel.AxleConfiguration;} }
		public Kilogram CurbMassChassis { get { return ViewModel.CurbMassChassis; } }
		public Kilogram GrossVehicleMassRating { get { return ViewModel.GrossVehicleMass; } }
		public IList<ITorqueLimitInputData> TorqueLimits { get {
			return ViewModel.TorqueLimits.Select(x => new TorqueLimitInputData()
				{
					//Gear = x.Gear, MaxTorque = x.MaxTorque
				})
							.Cast<ITorqueLimitInputData>().ToList();
		} }

		public IList<IAxleDeclarationInputData> AxlesDeclaration
		{
			get { return GetComponentViewModel<IAxlesViewModel>(Component.Axles)?.ModelData; }
		}

		public string ManufacturerAddress { get { return ViewModel.ManufacturerAddress; } }
		public PerSecond EngineIdleSpeed { get { return ViewModel.IdlingSpeed; } }
		public bool VocationalVehicle { get; }
		public bool SleeperCab { get; }
		public TankSystem? TankSystem { get; }
		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; }
		public bool ZeroEmissionVehicle { get; }
		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public Watt MaxNetPower2 { get; }
		public RegistrationClass RegisteredClass { get; }
		public int NumberOfPassengersUpperDeck { get; }
		public int NumberOfPassengersLowerDeck { get; }
		public CubicMeter CargoVolume { get; }
		public VehicleCode VehicleCode { get; }
		public bool LowEntry { get; }
		public bool Articulated { get; }
		public Meter Height { get; }
		public Meter Length { get; }
		public Meter Width { get; }
		public Meter EntranceHeight { get; }
		public ConsumerTechnology DoorDriveTechnology { get; }

		public IAirdragDeclarationInputData AirdragInputData { get {
			return GetComponentViewModel<IAirdragViewModel>(Component.Airdrag)?.ModelData;
		} }

		public IVehicleComponentsDeclaration Components { get { return this; } }
		public XmlNode XMLSource { get; }

		public IGearboxDeclarationInputData GearboxInputData { get {
			return GetComponentViewModel<IGearboxViewModel>(Component.Gearbox)?.ModelData;
		} }
		public ITorqueConverterDeclarationInputData TorqueConverterInputData { get {
			return GetComponentViewModel<ITorqueConverterViewModel>(Component.TorqueConverter)?.ModelData;
		} }
		public IAxleGearInputData AxleGearInputData { get {
			return GetComponentViewModel<IAxlegearViewModel>(Component.Axlegear)?.ModelData;
		} }
		public IAngledriveInputData AngledriveInputData { get {
			var vm = GetComponentViewModel<IAngledriveViewModel>(Component.Angledrive);
			return vm != null? vm.ModelData : AdapterFactory.AngledriveDeclarationAdapter(new AngledriveViewModel {ParentViewModel = ViewModel});
		} }
		public IEngineDeclarationInputData EngineInputData { get {
			return GetComponentViewModel<IEngineViewModel>(Component.Engine)?.ModelData;
		} }

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => _auxiliaryInputData;

		public IAuxiliariesDeclarationInputData AuxiliaryInputData()
		{
			return GetComponentViewModel<IAuxiliariesViewModel>(Component.Auxiliaries)?.ModelData;
		}

		public IRetarderInputData RetarderInputData { get {
			var vm = GetComponentViewModel<IRetarderViewModel>(Component.Retarder);
				return vm != null ? vm.ModelData : new RetarderViewModel {ParentViewModel = ViewModel}.ModelData;
		} }
		public IPTOTransmissionInputData PTOTransmissionInputData { get { return this; } }

		public IAxlesDeclarationInputData AxleWheels { get { return this; } }
		public IBusAuxiliariesDeclarationData BusAuxiliaries { get; }

		#endregion

		private T GetComponentViewModel<T>(Component component) where T : class
		{
			var vm = ViewModel.GetComponentViewModel(component);
			var cvm = vm as T;
			return cvm;
		}

		#region Implementation of IPTOTransmissionInputData

		public string PTOTransmissionType { get { return ViewModel.PTOTechnology; } }
		public TableData PTOLossMap { get { throw new NotImplementedException(); } }
		public TableData PTOCycle { get { throw new NotImplementedException(); } }

		#endregion
	}

}

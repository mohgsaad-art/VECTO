using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
	public abstract class ComponentsViewModel : ViewModelBase, IComponentsViewModel
	{
		public ObservableCollection<IComponentViewModel> _components = new ObservableCollection<IComponentViewModel>();

		protected IComponentViewModelFactory _componentViewModelFactory;
		protected IVehicleComponentsDeclaration _inputData;

		public ObservableCollection<IComponentViewModel> Components
		{
			get { return _components; }
			set { SetProperty(ref _components, value); }
		}

		public IRetarderViewModel RetarderViewModel
		{
			get => _retarderViewModel;
		}

		public IEngineViewModel EngineViewModel
		{
			get => _engineViewModel;
		}

		public IAngleDriveViewModel AngleDriveViewModel
		{
			get => _angledriveViewModel;
		}

		public IAirDragViewModel AirDragViewModel
		{
			get => _airdragViewModel;
		}

		public IPTOViewModel PTOViewModel
		{
			get => _ptoViewModel;
		}

		protected abstract void CreateComponents();

		public ComponentsViewModel(IXMLVehicleComponentsDeclaration inputData, IComponentViewModelFactory vmFactory)
		{
			_componentViewModelFactory = vmFactory;
			_inputData = inputData;
			CreateComponents();
		}


		protected IAngleDriveViewModel _angledriveViewModel;
		protected IRetarderViewModel _retarderViewModel;
		protected IAirDragViewModel _airdragViewModel;
		protected IPTOViewModel _ptoViewModel;
		protected IEngineViewModel _engineViewModel;

		#region Implementation of IVehicleComponentsdeclaration

		protected IGearboxDeclarationInputData _gearboxInputData;
		protected ITorqueConverterDeclarationInputData _torqueConverterInputData;
		protected IAxleGearInputData _axleGearInputData;
		protected IEngineDeclarationInputData _engineInputData;
		protected IAuxiliariesDeclarationInputData _auxiliaryInputData;
		protected IAxlesDeclarationInputData _axleWheels;
		protected IBusAuxiliariesDeclarationData _busAuxiliaries;



		public virtual IAirdragDeclarationInputData AirdragInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IGearboxDeclarationInputData GearboxInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();

		}

		public virtual IAxleGearInputData AxleGearInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IAngledriveInputData AngledriveInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IEngineDeclarationInputData EngineInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IAuxiliariesDeclarationInputData AuxiliaryInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IRetarderInputData RetarderInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IPTOTransmissionInputData PTOTransmissionInputData
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IAxlesDeclarationInputData AxleWheels
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public IElectricStorageSystemDeclarationInputData ElectricStorage => throw new NotImplementedException();

		public IElectricMachinesDeclarationInputData ElectricMachines => throw new NotImplementedException();
		public IIEPCDeclarationInputData IEPC { get => throw new NotImplementedException(); }

		#endregion

	}
	public class ComponentsViewModel_v1_0 : ComponentsViewModel
	{

		public static new readonly string VERSION = typeof(XMLDeclarationComponentsDataProviderV10).FullName;

		public ComponentsViewModel_v1_0(IXMLVehicleComponentsDeclaration inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
		{

		}


		protected override void CreateComponents()
		{
			throw new NotImplementedException();
		}
	}


	public class ComponentsViewModel_v2_0 : ComponentsViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationComponentsDataProviderV20).FullName;
		private IAxleGearViewModel _axlegearViewModel;
		private IGearBoxViewModel _gearBoxViewModel;
		private IAxleWheelsViewModel _axleWheelsViewModel;
		private IAuxiliariesViewModel _auxiliaryViewModel;

		public ComponentsViewModel_v2_0(IXMLVehicleComponentsDeclaration inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
		{
			_inputData = inputData;
		}


		protected  override void CreateComponents()
		{
			_engineViewModel = _componentViewModelFactory.CreateComponentViewModel(_inputData.EngineInputData) as IEngineViewModel;
			Components.Add(_engineViewModel);

			_axlegearViewModel = _componentViewModelFactory.CreateComponentViewModel(_inputData.AxleGearInputData) as IAxleGearViewModel;
			Components.Add(_axlegearViewModel);

			_gearBoxViewModel = _componentViewModelFactory.CreateComponentViewModel(_inputData.GearboxInputData) as IGearBoxViewModel;
			Components.Add(_gearBoxViewModel);

			_angledriveViewModel = _componentViewModelFactory.CreateComponentViewModel(_inputData.AngledriveInputData) as IAngleDriveViewModel;
			Components.Add(_angledriveViewModel);
			
			_retarderViewModel = _componentViewModelFactory.CreateComponentViewModel(_inputData.RetarderInputData) as IRetarderViewModel;
			Components.Add(_retarderViewModel);

			_axleWheelsViewModel = _componentViewModelFactory.CreateComponentViewModel(_inputData.AxleWheels) as IAxleWheelsViewModel;
			Components.Add(_axleWheelsViewModel);
			
			_auxiliaryViewModel= _componentViewModelFactory.CreateComponentViewModel(_inputData.AuxiliaryInputData) as IAuxiliariesViewModel;
			Components.Add(_auxiliaryViewModel);
			
			_airdragViewModel = _componentViewModelFactory.CreateComponentViewModel(_inputData.AirdragInputData) as IAirDragViewModel;
			Components.Add(_airdragViewModel);

			
			_ptoViewModel =
				_componentViewModelFactory.CreateComponentViewModel(_inputData.PTOTransmissionInputData) as
					IPTOViewModel;
			Components.Add(_ptoViewModel);
			

		}

		#region Overrides

		public override IAirdragDeclarationInputData AirdragInputData => _airdragViewModel.IsPresent ? _airdragViewModel as IAirdragDeclarationInputData : null;

		public override IGearboxDeclarationInputData GearboxInputData => _gearBoxViewModel.IsPresent
			? _gearBoxViewModel as IGearboxDeclarationInputData
			: null;
		public override IAxleGearInputData AxleGearInputData => _axlegearViewModel.IsPresent ? _axlegearViewModel as IAxleGearInputData : null;
		public override IAngledriveInputData AngledriveInputData => _angledriveViewModel.IsPresent ? _angledriveViewModel as IAngledriveInputData : null;
		public override IEngineDeclarationInputData EngineInputData => _engineViewModel.IsPresent ? _engineViewModel as IEngineDeclarationInputData : null;
		public override IAuxiliariesDeclarationInputData AuxiliaryInputData => _auxiliaryViewModel.IsPresent ? _auxiliaryViewModel as IAuxiliariesDeclarationInputData : null;
		public override IRetarderInputData RetarderInputData => _retarderViewModel.IsPresent ? _retarderViewModel as IRetarderInputData : null;
		public override IAxlesDeclarationInputData AxleWheels => _axleWheelsViewModel.IsPresent ?  _axleWheelsViewModel as IAxlesDeclarationInputData : null;
		/*
		public override IPTOTransmissionInputData PTOTransmissionInputData =>
			_ptoViewModel.IsPresent ? _ptoViewModel as IPTOTransmissionInputData : null;

		public override ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get => _torqueConverterInputData;
			set => SetProperty(ref _torqueConverterInputData, value);
		}
		*/

		#endregion
	}






}

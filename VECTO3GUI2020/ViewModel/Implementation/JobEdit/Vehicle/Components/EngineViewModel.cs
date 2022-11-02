using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class EngineViewModel : ViewModelBase, IComponentViewModel, IEngineViewModel
    {
        

		protected readonly IEngineDeclarationInputData _inputData;


		private ICommonComponentViewModel _commonComponentViewModel;
        public ICommonComponentViewModel CommonComponentViewModel { 
            get => _commonComponentViewModel; 
            set => SetProperty(ref _commonComponentViewModel, value); }
		private static readonly string _name = "Engine";
		public string Name { get { return _name; } }
		private bool _isPresent;
		public bool IsPresent { get { return _isPresent; } }


		protected abstract void SetProperties();





        public EngineViewModel(IXMLEngineDeclarationInputData inputData,
			IComponentViewModelFactory componentFactory)
        {
            
            _inputData = inputData as IXMLEngineDeclarationInputData;
            Debug.Assert(_inputData != null);
            _isPresent = (_inputData != null);
			_componentFactory = componentFactory;
			Debug.Assert(_inputData != null);


			SetProperties();
		}

		#region Implementation of IEngineDeclarationInputData
		protected CubicMeter _displacement;
		protected Watt _ratedPowerDeclared;
		protected PerSecond _ratedSpeedDeclared;
		protected NewtonMeter _maxTorqueDeclared;
		//protected IList<IEngineModeDeclarationInputData> _engineModes;
		protected WHRType _whrType;
		protected IComponentViewModelFactory _componentFactory;
		protected PerSecond _idlingSpeed;


		public virtual DataSource DataSource
		{
			get => _commonComponentViewModel.DataSource;
			set => _commonComponentViewModel.DataSource = value;
		}

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

		public string AppVersion => _commonComponentViewModel.AppVersion;

		public virtual CubicMeter Displacement
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual Watt RatedPowerDeclared
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual PerSecond RatedSpeedDeclared
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual NewtonMeter MaxTorqueDeclared
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual IList<IEngineModeDeclarationInputData> EngineModes
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public virtual WHRType WHRType
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}


		public virtual PerSecond IdlingSpeed
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		#endregion
	}


	public class EngineViewModel_v1_0 : EngineViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationEngineDataProviderV10).FullName;
        public EngineViewModel_v1_0(IXMLEngineDeclarationInputData inputData,
			IComponentViewModelFactory componentFactory) : base(inputData, componentFactory)
        {

        }

		protected override void SetProperties()
		{
			throw new NotImplementedException();
		}
	}

	public class EngineViewModel_v2_0 : EngineViewModel_v1_0
	{
		public new static readonly string VERSION = typeof(XMLDeclarationEngineDataProviderV20).FullName;



		#region overrides of used Properties
		public override CubicMeter Displacement
		{
			get => _displacement;
			set => SetProperty(ref _displacement, value);
		}

		public override Watt RatedPowerDeclared
		{
			get => _ratedPowerDeclared;
			set => SetProperty(ref _ratedPowerDeclared, value);
		}

		public override PerSecond RatedSpeedDeclared
		{
			get => _ratedSpeedDeclared;
			set => SetProperty(ref _ratedSpeedDeclared, value);
		}

		public override NewtonMeter MaxTorqueDeclared
		{
			get => _maxTorqueDeclared;
			set => SetProperty(ref _maxTorqueDeclared, value);
		}

		private ObservableCollection<IEngineModeViewModel> _engineModeViewModels = new ObservableCollection<IEngineModeViewModel>();
		public ObservableCollection<IEngineModeViewModel> EngineModeViewModels
		{
			get => _engineModeViewModels;
			set => SetProperty(ref _engineModeViewModels, value);
		}


		public override IList<IEngineModeDeclarationInputData> EngineModes
		{
			get => _engineModeViewModels.Cast<IEngineModeDeclarationInputData>().ToList();
			set => throw new NotImplementedException();
		}


		public override WHRType WHRType
		{
			get => _whrType;
			set => SetProperty(ref _whrType, value);
		}

		public override PerSecond IdlingSpeed
		{
			get
			{
				return EngineModeViewModels[0].IdleSpeed;
			}
			set
			{
				EngineModeViewModels[0].IdleSpeed = value;
			}
		}

		#endregion


		public EngineViewModel_v2_0(IXMLEngineDeclarationInputData inputData,
			IComponentViewModelFactory componentFactory) :
			base(inputData, componentFactory)
		{
		}


		protected override void SetProperties()
		{
			CommonComponentViewModel = _componentFactory.CreateCommonComponentViewModel(_inputData);

			_displacement = _inputData.Displacement;
			_ratedSpeedDeclared = _inputData.RatedSpeedDeclared;
			_ratedPowerDeclared = _inputData.RatedPowerDeclared;
			_maxTorqueDeclared = _inputData.MaxTorqueDeclared;
			_whrType = _inputData.WHRType;


			foreach (var engineMode in _inputData.EngineModes) {
				EngineModeViewModels.Add(_componentFactory.CreateEngineModeViewModel(engineMode));
			}


		}
	}
}

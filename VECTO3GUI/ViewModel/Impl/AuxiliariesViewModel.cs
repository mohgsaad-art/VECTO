using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Castle.Core.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.Helper;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Model;
using VECTO3GUI.Model.TempDataObject;


namespace VECTO3GUI.ViewModel.Impl
{
	public class AuxiliariesViewModel : AbstractComponentViewModel, IAuxiliariesViewModel
	{
		#region Members

		private string _pneumaticSystemTechnology;
		private string _fanTechnology;
		private string _electricSystemTechnology;
		private string _hvacTechnology;
		private readonly ObservableCollection<SteeringPumpEntry> _steeringPumpTechnologies = new ObservableCollection<SteeringPumpEntry>();
		private IAxlesViewModel axlesViewModel;
		private IBusAuxiliariesDeclarationData _busAuxiliaries;


		private ObservableCollectionEx<AlternatorTechnologyModel> _alternatorTechnologies = new ObservableCollectionEx<AlternatorTechnologyModel>();
		private bool _dayRunningLightsLED;
		private bool _headlightyLED;
		private bool _positionlightsLED;
		private bool _breaklightsLED;
		private bool _interiorLightsLED;
		private BusHVACSystemConfiguration _systemConfiguration;
		private ACCompressorType _compressorTypeDriver;
		private ACCompressorType _compressorTypePassenger;
		private Watt _auxHeaterPower;
		private bool _doubleGlasing;
		private bool _heatPump;
		private bool _adjustableAuxiliaryHeater;
		private bool _separateAirDistributionDucts;

		private AuxiliariesBusComponentData _componentData;
		private ICommand _removeAlternatorCommand;
		private ICommand _removeAllAlternatorCommand;
		private ICommand _addAlternatorCommand;

		#endregion


		#region Implementation of IAuxiliariesViewModel

		public IAuxiliariesDeclarationInputData ModelData { get { return AdapterFactory.AuxiliariesDeclarationAdapter(this); } }

		public string PneumaticSystemTechnology
		{
			get { return _pneumaticSystemTechnology; }
			set { SetProperty(ref _pneumaticSystemTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedPneumaticSystemTechnologies
		{
			get { return DeclarationData.PneumaticSystem.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

		public string FanTechnology
		{
			get { return _fanTechnology; }
			set { SetProperty(ref _fanTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedFanTechnologies
		{
			get { return DeclarationData.Fan.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

		public ObservableCollection<SteeringPumpEntry> SteeringPumpTechnologies
		{
			get
			{
				if (axlesViewModel == null)
				{
					//ConnectAxleViewModel();
				}

				return _steeringPumpTechnologies;
			}
		}

		public AllowedEntry<string>[] AllowedSteeringPumpTechnologies
		{
			get { return DeclarationData.SteeringPump.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

		public string ElectricSystemTechnology
		{
			get { return _electricSystemTechnology; }
			set { SetProperty(ref _electricSystemTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedElectricSystemTechnologies
		{
			get { return DeclarationData.ElectricSystem.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

		public string HVACTechnology
		{
			get { return _hvacTechnology; }
			set { SetProperty(ref _hvacTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedHVACTechnologies
		{
			get
			{
				return DeclarationData.HeatingVentilationAirConditioning.GetTechnologies()
					.Select(t => AllowedEntry.Create(t, t)).ToArray();
			}
		}



		#endregion



		#region Implementation of IBusAuxiliaries

		public ObservableCollectionEx<AlternatorTechnologyModel> AlternatorTechnologies
		{
			get { return _alternatorTechnologies; }
			set { SetProperty(ref _alternatorTechnologies, value); }
		}

		public bool DayrunninglightsLED
		{
			get { return _dayRunningLightsLED; }
			set
			{
				if (!SetProperty(ref _dayRunningLightsLED, value))
					return;
				IsDataChanged(_dayRunningLightsLED, _componentData);
			}
		}

		public bool HeadlightsLED
		{
			get { return _headlightyLED; }
			set
			{
				if (!SetProperty(ref _headlightyLED, value))
					return;

				IsDataChanged(_headlightyLED, _componentData);
			}
		}

		public bool PositionlightsLED
		{
			get { return _positionlightsLED; }
			set
			{
				if (!SetProperty(ref _positionlightsLED, value))
					return;
				IsDataChanged(_positionlightsLED, _componentData);
			}
		}

		public bool BrakelightsLED
		{
			get { return _breaklightsLED; }
			set
			{
				if (!SetProperty(ref _breaklightsLED, value))
					return;
				IsDataChanged(_breaklightsLED, _componentData);
			}
		}

		public bool InteriorLightsLED
		{
			get { return _interiorLightsLED; }
			set
			{
				if (!SetProperty(ref _interiorLightsLED, value))
					return;
				IsDataChanged(_interiorLightsLED, _componentData);
			}
		}

		public BusHVACSystemConfiguration SystemConfiguration
		{
			get { return _systemConfiguration; }
			set
			{
				if (!SetProperty(ref _systemConfiguration, value))
					return;

				IsDataChanged(_systemConfiguration, _componentData);
			}
		}

		public ACCompressorType CompressorTypeDriver
		{
			get { return _compressorTypeDriver; }
			set
			{
				if (!SetProperty(ref _compressorTypeDriver, value))
					return;
				IsDataChanged(_compressorTypeDriver, _componentData);
			}
		}

		public ACCompressorType CompressorTypePassenger
		{
			get { return _compressorTypePassenger; }
			set
			{
				if (!SetProperty(ref _compressorTypePassenger, value))
					return;
				IsDataChanged(_compressorTypePassenger, _componentData);
			}
		}
		public Watt AuxHeaterPower
		{
			get { return _auxHeaterPower; }
			set
			{
				if (!SetProperty(ref _auxHeaterPower, value))
					return;
				IsDataChanged(_auxHeaterPower, _componentData);
			}
		}

		public bool DoubleGlasing
		{
			get { return _doubleGlasing; }
			set
			{
				if (!SetProperty(ref _doubleGlasing, value))
					return;
				IsDataChanged(_doubleGlasing, _componentData);
			}
		}

		public bool HeatPump
		{
			get { return _heatPump; }
			set
			{
				if (!SetProperty(ref _heatPump, value))
					return;
				IsDataChanged(_heatPump, _componentData);
			}
		}

		public bool AdjustableAuxiliaryHeater
		{
			get { return _adjustableAuxiliaryHeater; }
			set
			{
				if (!SetProperty(ref _adjustableAuxiliaryHeater, value))
					return;
				IsDataChanged(_adjustableAuxiliaryHeater, _componentData);
			}
		}

		public bool SeparateAirDistributionDucts
		{
			get { return _separateAirDistributionDucts; }
			set
			{
				if (!SetProperty(ref _separateAirDistributionDucts, value))
					return;
				IsDataChanged(_adjustableAuxiliaryHeater, _componentData);
			}
		}

		public Dictionary<string, string> XmlNamesToPropertyMapping { get; private set; }

		

		public AllowedEntry<BusHVACSystemConfiguration>[] AllowedSystemConfigurations { get; private set; }
		public AllowedEntry<ACCompressorType>[] AllowedDriverACCompressorTypes { get; private set; }
		public AllowedEntry<ACCompressorType>[] AllowedPassengerACCompressorTypes { get; private set; }
		public AllowedEntry<string>[] AllowedAlternatorTechnology { get; private set; }


		#endregion

		private List<AlternatorTechnologyModel> _cachedAlternators { get; set; }

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationInputDataProvider;

			//SetValues(inputData.JobInputData.Vehicle.Components.);

			////ToDo
			//JobViewModel.InputDataProvider.Switch()
			//			.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AuxiliaryInputData()))
			//			.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AuxiliaryInputData()));

			_busAuxiliaries = inputData?.JobInputData.Vehicle.Components.BusAuxiliaries;
			SetBusAuxiliaryValues(_busAuxiliaries);
			SetAllowedValues();

			XmlNamesToPropertyMapping = _componentData.XmlNamesToPropertyMapping;
			//ConnectAxleViewModel();
		}


		private void SetBusAuxiliaryValues(IBusAuxiliariesDeclarationData busAux)
		{
			if (busAux == null)
			{
				_componentData = new AuxiliariesBusComponentData(this, true);
				AlternatorTechnologies.CollectionChanged += AlternatorTechnologiesOnCollectionChanged;
				AlternatorTechnologies.CollectionItemChanged += AlternatorTechnologiesOnCollectionItemChanged;
			}
			else
			{
				if (!busAux.ElectricSupply.Alternators.IsNullOrEmpty())
				{
					AlternatorTechnologies.Clear();

					AlternatorTechnologies.CollectionChanged += AlternatorTechnologiesOnCollectionChanged;
					AlternatorTechnologies.CollectionItemChanged += AlternatorTechnologiesOnCollectionItemChanged;

					for (int i = 0; i < busAux.ElectricSupply.Alternators.Count; i++)
					{
						AlternatorTechnologies.Add(new AlternatorTechnologyModel() { AlternatorTechnology = busAux.ElectricSupply.Alternators[i].Technology });
					}
				}

				DayrunninglightsLED = busAux.ElectricConsumers.DayrunninglightsLED;
				HeadlightsLED = busAux.ElectricConsumers.HeadlightsLED;
				PositionlightsLED = busAux.ElectricConsumers.PositionlightsLED;
				BrakelightsLED = busAux.ElectricConsumers.BrakelightsLED;
				InteriorLightsLED = busAux.ElectricConsumers.InteriorLightsLED;
				SystemConfiguration = busAux.HVACAux.SystemConfiguration;
				CompressorTypeDriver = busAux.HVACAux.CompressorTypeDriver;
				CompressorTypePassenger = busAux.HVACAux.CompressorTypePassenger;
				AuxHeaterPower = busAux.HVACAux.AuxHeaterPower;
				DoubleGlasing = busAux.HVACAux.DoubleGlazing;
				HeatPump = busAux.HVACAux.HeatPump;
				AdjustableAuxiliaryHeater = busAux.HVACAux.AdjustableAuxiliaryHeater;
				SeparateAirDistributionDucts = busAux.HVACAux.SeparateAirDistributionDucts;
	
				_componentData = new AuxiliariesBusComponentData(this);
			}

			ClearChangedProperties();
		}

		private void AlternatorTechnologiesOnCollectionItemChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "AlternatorTechnology")
				return;

			var changed = IsAlternatorTechnologyChanged();
			SetChangedProperty(changed, nameof(AlternatorTechnologies));
		}

		private void AlternatorTechnologiesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var changed = IsAlternatorTechnologyChanged();
			SetChangedProperty(changed, nameof(AlternatorTechnologies));
		}

		private bool IsAlternatorTechnologyChanged()
		{
			if (AlternatorTechnologies == null || _componentData?.OriginAlternatorTechnologies == null)
				return true;

			if (AlternatorTechnologies.Count != _componentData.OriginAlternatorTechnologies.Count)
				return true;

			for (int i = 0; i < AlternatorTechnologies.Count; i++)
			{
				if (AlternatorTechnologies[i].AlternatorTechnology !=
					_componentData.OriginAlternatorTechnologies[i].AlternatorTechnology)
					return true;
			}

			return false;
		}

		private void SetAllowedValues()
		{
			AllowedSystemConfigurations = EnumHelper.GetValues<BusHVACSystemConfiguration>()
				.Where(x => x != BusHVACSystemConfiguration.Unknown)
				.Select(sc => AllowedEntry.Create(sc, sc.GetLabel())).ToArray();

			AllowedDriverACCompressorTypes = EnumHelper.GetValues<ACCompressorType>()
				.Where(x => x != ACCompressorType.Unknown)
				.Select(acc => AllowedEntry.Create(acc, acc.GetLabel())).ToArray();

			AllowedPassengerACCompressorTypes = AllowedDriverACCompressorTypes;

			AllowedAlternatorTechnology = DeclarationData.BusAuxiliaries.AlternatorTechnologies.Entries.Keys
														.Select(x => AllowedEntry.Create(x, x)).ToArray();
		}

		public override void ResetComponentData()
		{
			_componentData.ResetToComponentValues(this);
			AlternatorTechnologies.CollectionChanged += AlternatorTechnologiesOnCollectionChanged;
			AlternatorTechnologies.CollectionItemChanged += AlternatorTechnologiesOnCollectionItemChanged;

			var changed = IsAlternatorTechnologyChanged();
			SetChangedProperty(changed, nameof(AlternatorTechnologies));
		}

		public void CacheAlternatorTechnologies()
		{
			if (_componentData != null)
			{
				_cachedAlternators = new List<AlternatorTechnologyModel>();
				for (int i = 0; i < AlternatorTechnologies.Count; i++)
				{
					_cachedAlternators.Add(new AlternatorTechnologyModel { AlternatorTechnology = AlternatorTechnologies[i].AlternatorTechnology });
				}
			}
		}

		public void LoadCachedAlternatorTechnologies()
		{
			if (_componentData != null && _cachedAlternators != null)
			{
				AlternatorTechnologies.Clear();
				for (int i = 0; i < _cachedAlternators.Count; i++)
				{
					AlternatorTechnologies.Add(new AlternatorTechnologyModel { AlternatorTechnology = _cachedAlternators[i].AlternatorTechnology });
				}
			}
		}


		public override object CommitComponentData()
		{
			_componentData.UpdateCurrentValues(this);
			ClearChangedProperties();
			return _componentData;
		}
		public override void ShowValidationErrors(Dictionary<string, string> errors)
		{
			if (errors.IsNullOrEmpty())
				return;

			foreach (var error in errors)
			{
				string propertyName;
				if (XmlNamesToPropertyMapping.TryGetValue(error.Key, out propertyName))
					AddPropertyError(propertyName, error.Value);
			}
		}
		public override void RemoveValidationErrors(Dictionary<string, string> errors)
		{
			if (errors.IsNullOrEmpty())
				return;

			foreach (var error in errors)
			{
				string propertyName;
				if (XmlNamesToPropertyMapping.TryGetValue(error.Key, out propertyName))
					RemovePropertyError(propertyName);
			}
		}


		#region Commands

		public ICommand RemoveAlternatorCommand
		{
			get
			{
				return _removeAlternatorCommand ??
						(_removeAlternatorCommand = new RelayCommand<int>(DoRemoveAlternator));
			}
		}
		private void DoRemoveAlternator(int index)
		{
			AlternatorTechnologies.RemoveAt(index);
		}


		public ICommand RemoveAllAlternatorCommand
		{
			get
			{
				return _removeAllAlternatorCommand ??
						(_removeAllAlternatorCommand = new RelayCommand(DoRemoveAllAlternators));
			}
		}

		private void DoRemoveAllAlternators()
		{
			AlternatorTechnologies.Clear();
			OnPropertyChanged(nameof(AlternatorTechnologies));
		}

		public ICommand AddAlternatorCommand
		{
			get
			{
				return _addAlternatorCommand ??
						(_addAlternatorCommand = new RelayCommand(DoAddAlternator));
			}
		}

		private void DoAddAlternator()
		{
			AlternatorTechnologies.Add(new AlternatorTechnologyModel
			{
				AlternatorTechnology = null
			});
		}

		#endregion



		#region Legacy 

		//private void ConnectAxleViewModel()
		//{
		//	var axlesVm = ParentViewModel.GetComponentViewModel(Component.Axles);

		//	if (axlesVm == null)
		//	{
		//		return;
		//	}

		//	axlesViewModel = axlesVm as IAxlesViewModel;
		//	(axlesViewModel as AxlesViewModel).PropertyChanged += UpdateSteeringPump;
		//	DoUpdateSteeringPumpTechnologies();
		//}

		//private void UpdateSteeringPump(object sender, PropertyChangedEventArgs e)
		//{
		//	if (e.PropertyName == "NumSteeredAxles")
		//	{
		//		DoUpdateSteeringPumpTechnologies();
		//	}
		//}

		//private void DoUpdateSteeringPumpTechnologies()
		//{
		//	if (axlesViewModel == null)
		//	{
		//		return;
		//	}

		//	while (_steeringPumpTechnologies.Count > axlesViewModel.NumSteeredAxles)
		//	{
		//		_steeringPumpTechnologies.RemoveAt(_steeringPumpTechnologies.Count - 1);
		//	}
		//	while (_steeringPumpTechnologies.Count < axlesViewModel.NumSteeredAxles)
		//	{
		//		_steeringPumpTechnologies.Add(new SteeringPumpEntry(_steeringPumpTechnologies.Count + 1, AllowedSteeringPumpTechnologies.First().Label));
		//	}
		//}


		//private void SetValues(IAuxiliariesDeclarationInputData aux)
		//{
		//	FanTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.Fan).Technology.FirstOrDefault();
		//	ElectricSystemTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.ElectricSystem).Technology.FirstOrDefault();
		//	HVACTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.HVAC).Technology.FirstOrDefault();
		//	PneumaticSystemTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.PneumaticSystem).Technology.FirstOrDefault();
		//	SteeringPumpTechnologies.Clear();
		//	foreach (var tech in aux.Auxiliaries.First(x => x.Type == AuxiliaryType.SteeringPump).Technology)
		//	{
		//		SteeringPumpTechnologies.Add(new SteeringPumpEntry(SteeringPumpTechnologies.Count + 1, tech));
		//	}
		//}


		#endregion



	}
}
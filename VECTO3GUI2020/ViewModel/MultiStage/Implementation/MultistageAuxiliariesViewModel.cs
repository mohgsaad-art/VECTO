using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using EnumHelper = VECTO3GUI2020.Helper.EnumHelper;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{

	public interface IMultistageAuxiliariesViewModel : IBusAuxiliariesDeclarationData,
		IElectricSupplyDeclarationData,
		IPneumaticSupplyDeclarationData,
		IElectricConsumersDeclarationData,
		IPneumaticConsumersDeclarationData,
		IHVACBusAuxiliariesDeclarationData
	{
		void SetAuxiliariesInputData(IBusAuxiliariesDeclarationData componentsAuxiliaryInputData);
		bool HasValues { get; }
		object PrimaryVehicleHybridElectric { get; set; }
		bool HasErrors { get; }
		Dictionary<string, string> Errors { get; }
		bool ShowConsolidatedData { get; set; }
	}


	public class MultistageAuxiliariesViewModel : ViewModelBase, IMultistageAuxiliariesViewModel, IDataErrorInfo
	{

		public MultistageAuxiliariesViewModel()
		{
			CreateParameterViewModels();
		}

		public MultistageAuxiliariesViewModel(IBusAuxiliariesDeclarationData consolidatedAuxiliariesInputData)
		{
			ConsolidatedInputData = consolidatedAuxiliariesInputData;

			CreateParameterViewModels();
		}

		private void CreateParameterViewModels()
		{
			_parameterViewModels = new Dictionary<string, MultistageParameterViewModel>();
			var properties = this.GetType().GetProperties();
			var backedUpParameters = new HashSet<string>() {
				nameof(InteriorLightsLED),
				nameof(DayrunninglightsLED),
				nameof(PositionlightsLED),
				nameof(BrakelightsLED),
				nameof(HeadlightsLED),
			
				nameof(SystemConfiguration),
				nameof(HeatPumpTypeHeatingPassengerCompartment),
				nameof(HeatPumpTypeCoolingPassengerCompartment),
				nameof(HeatPumpTypeHeatingDriverCompartment),
				nameof(HeatPumpTypeCoolingDriverCompartment),

				nameof(AuxHeaterPower),
				nameof(DoubleGlazing),
				nameof(AirElectricHeater),
				nameof(AdjustableAuxiliaryHeater),
				nameof(SeparateAirDistributionDucts),
				nameof(OtherHeatingTechnology),
				nameof(WaterElectricHeater)
			};

			foreach (var property in properties)
			{
				if (!backedUpParameters.Contains(property.Name))
				{
					continue;
				}


				object previousInputData = null;
				try
				{
					previousInputData = ConsolidatedInputData?.GetType().GetProperty(nameof(property.Name))?
						.GetValue(ConsolidatedInputData);
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}

				_parameterViewModels.Add(property.Name, new MultistageParameterViewModel(property.Name, previousInputData, this,
					resourceManagers: new ResourceManager[] { BusStrings.ResourceManager, Strings.ResourceManager }
				));
			}

			//Set consolidated Data
			_parameterViewModels[nameof(InteriorLightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.InteriorLightsLED;
			_parameterViewModels[nameof(DayrunninglightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.DayrunninglightsLED;
			_parameterViewModels[nameof(PositionlightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.PositionlightsLED;
			_parameterViewModels[nameof(BrakelightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.BrakelightsLED;
			_parameterViewModels[nameof(HeadlightsLED)].PreviousContent =
				ConsolidatedInputData?.ElectricConsumers.HeadlightsLED;
			_parameterViewModels[nameof(SystemConfiguration)].PreviousContent =
				ConsolidatedInputData?.HVACAux.SystemConfiguration;
			_parameterViewModels[nameof(HeatPumpTypeCoolingDriverCompartment)].PreviousContent =
				ConsolidatedInputData?.HVACAux.HeatPumpTypeCoolingDriverCompartment;
			_parameterViewModels[nameof(HeatPumpTypeHeatingDriverCompartment)].PreviousContent =
				ConsolidatedInputData?.HVACAux.HeatPumpTypeHeatingDriverCompartment;
			_parameterViewModels[nameof(HeatPumpTypeCoolingPassengerCompartment)].PreviousContent =
				ConsolidatedInputData?.HVACAux.HeatPumpTypeCoolingPassengerCompartment;
			_parameterViewModels[nameof(HeatPumpTypeHeatingPassengerCompartment)].PreviousContent =
				ConsolidatedInputData?.HVACAux.HeatPumpTypeHeatingPassengerCompartment;

			_parameterViewModels[nameof(AuxHeaterPower)].PreviousContent =
				ConsolidatedInputData?.HVACAux.AuxHeaterPower;
			_parameterViewModels[nameof(DoubleGlazing)].PreviousContent =
				ConsolidatedInputData?.HVACAux.DoubleGlazing;
			_parameterViewModels[nameof(AdjustableAuxiliaryHeater)].PreviousContent =
				ConsolidatedInputData?.HVACAux.AdjustableAuxiliaryHeater;
			_parameterViewModels[nameof(SeparateAirDistributionDucts)].PreviousContent =
				ConsolidatedInputData?.HVACAux.SeparateAirDistributionDucts;
			_parameterViewModels[nameof(AirElectricHeater)].PreviousContent =
				ConsolidatedInputData?.HVACAux.AirElectricHeater;
			_parameterViewModels[nameof(OtherHeatingTechnology)].PreviousContent =
				ConsolidatedInputData?.HVACAux.OtherHeatingTechnology;


			//Set editinggroups
			Action<MultistageParameterViewModel> HeatPumpGroupEditingEnabledCallback = model => {
				HeatPumpGroupEditingEnabled = model.EditingEnabled;
			};
			_parameterViewModels[nameof(SystemConfiguration)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;
			_parameterViewModels[nameof(HeatPumpTypeCoolingDriverCompartment)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;
			_parameterViewModels[nameof(HeatPumpTypeHeatingDriverCompartment)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;
			_parameterViewModels[nameof(HeatPumpTypeCoolingPassengerCompartment)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;
			_parameterViewModels[nameof(HeatPumpTypeHeatingPassengerCompartment)].EditingChangedCallback =
				HeatPumpGroupEditingEnabledCallback;
			//Setup AllowedValues 
			HeatPumpTypeDriverAllowedValues =
				EnumHelper.GetValuesAsObservableCollection<Enum, HeatPumpType>();

			HeatPumpTypePassengerAllowedValues =
				EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpType>(HeatPumpType.not_applicable);

			SystemConfigurationAllowedValues =
				EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, BusHVACSystemConfiguration>(
					BusHVACSystemConfiguration.Unknown);
		}

		protected override bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			var propertyChanged = base.SetProperty(ref field, value, propertyName);

			if (propertyChanged && _parameterViewModels != null && _parameterViewModels.ContainsKey(propertyName))
			{
				_parameterViewModels[propertyName].CurrentContent = value;
			}
			return propertyChanged;
		}

		public void SetAuxiliariesInputData(IBusAuxiliariesDeclarationData componentsAuxiliaryInputData)
		{
			if (componentsAuxiliaryInputData == null)
			{
				return;
			}

			HeatPumpGroupEditingEnabled = componentsAuxiliaryInputData.HVACAux != null;
			SystemConfiguration = componentsAuxiliaryInputData.HVACAux?.SystemConfiguration;
			HeatPumpTypeCoolingDriverCompartment =
				componentsAuxiliaryInputData.HVACAux?.HeatPumpTypeCoolingDriverCompartment;
			HeatPumpTypeHeatingDriverCompartment =
				componentsAuxiliaryInputData.HVACAux?.HeatPumpTypeHeatingDriverCompartment;
			HeatPumpTypeCoolingPassengerCompartment =
				componentsAuxiliaryInputData.HVACAux?.HeatPumpTypeCoolingPassengerCompartment;
			HeatPumpTypeHeatingPassengerCompartment =
				componentsAuxiliaryInputData.HVACAux?.HeatPumpTypeHeatingPassengerCompartment;



			AuxHeaterPower = componentsAuxiliaryInputData.HVACAux?.AuxHeaterPower;
			DoubleGlazing = componentsAuxiliaryInputData.HVACAux?.DoubleGlazing;
			AdjustableAuxiliaryHeater = componentsAuxiliaryInputData.HVACAux?.AdjustableAuxiliaryHeater;
			SeparateAirDistributionDucts = componentsAuxiliaryInputData.HVACAux?.SeparateAirDistributionDucts;
			WaterElectricHeater = componentsAuxiliaryInputData.HVACAux?.WaterElectricHeater;
			AirElectricHeater = componentsAuxiliaryInputData.HVACAux?.AirElectricHeater;
			OtherHeatingTechnology = componentsAuxiliaryInputData.HVACAux?.OtherHeatingTechnology;

			InteriorLightsLED = componentsAuxiliaryInputData.ElectricConsumers?.InteriorLightsLED;
			DayrunninglightsLED = componentsAuxiliaryInputData.ElectricConsumers?.DayrunninglightsLED;
			PositionlightsLED = componentsAuxiliaryInputData.ElectricConsumers?.PositionlightsLED;
			HeadlightsLED = componentsAuxiliaryInputData.ElectricConsumers?.HeadlightsLED;
			BrakelightsLED = componentsAuxiliaryInputData.ElectricConsumers?.BrakelightsLED;

			foreach (var multistageParameterViewModel in _parameterViewModels.Values)
			{
				//TODO: Nullable values getting enabled on loading a file.
				multistageParameterViewModel.UpdateEditingEnabled();
			}
			OnPropertyChanged(String.Empty);
		}

		private bool _showConsolidatedData;

		public bool ShowConsolidatedData
		{
			get => _showConsolidatedData;
			set
			{
				SetProperty(ref _showConsolidatedData, value);
				foreach (var multistageParameterViewModel in ParameterViewModels) {
					multistageParameterViewModel.Value.ShowConsolidatedData = value;
				}
			}
		}



		private IBusAuxiliariesDeclarationData _consolidatedInputData;
		public IBusAuxiliariesDeclarationData ConsolidatedInputData
		{
			get => _consolidatedInputData;
			set => SetProperty(ref _consolidatedInputData, value);
		}

		public bool HasValues
		{
			get
			{
				var hasValues = false;


				//Check if one of the Implemented properties of is not null
				var HVACinterfaceProperties = typeof(IHVACBusAuxiliariesDeclarationData).GetProperties();

				var notImplemented = new List<string>(new string[] {
					nameof(IHVACBusAuxiliariesDeclarationData.AdjustableCoolantThermostat),
					nameof(IHVACBusAuxiliariesDeclarationData.EngineWasteGasHeatExchanger)
				});

				foreach (var propInfo in HVACinterfaceProperties) {
					if (notImplemented.Contains(propInfo.Name)) continue;
					hasValues = hasValues || propInfo.GetValue(this) != null;
				}


				var ElectricConsumersInterfaceProperties = typeof(IElectricConsumersDeclarationData).GetProperties();
				foreach (var propInfo in ElectricConsumersInterfaceProperties) {
					hasValues = hasValues || propInfo.GetValue(this) != null;
				}



				return hasValues;
			}
		}

		private object _primaryVehicleHybridElectric = false;
		public object PrimaryVehicleHybridElectric
		{
			get => _primaryVehicleHybridElectric;
			set => SetProperty(ref _primaryVehicleHybridElectric, value);
		}



		#region HVAC

		


		private Watt _auxHeaterPower;
		private bool? _doubleGlazing;
		private bool? _adjustableAuxiliaryHeater;
		private bool? _separateAirDistributionDucts;
		private bool? _waterElectricHeater;
		private bool? _airElectricHeater;
		private bool? _otherHeatingTechnology;



		public Watt AuxHeaterPower
		{
			get => _auxHeaterPower;
			set => SetProperty(ref _auxHeaterPower, value);
		}

		public bool? DoubleGlazing
		{
			get => _doubleGlazing;
			set => SetProperty(ref _doubleGlazing, value);
		}

		public bool? AdjustableAuxiliaryHeater
		{
			get => _adjustableAuxiliaryHeater;
			set => SetProperty(ref _adjustableAuxiliaryHeater, value);
		}

		public bool? SeparateAirDistributionDucts
		{
			get => _separateAirDistributionDucts;
			set => SetProperty(ref _separateAirDistributionDucts, value);
		}

		public bool? WaterElectricHeater
		{
			get => _waterElectricHeater;
			set => SetProperty(ref _waterElectricHeater, value);
		}

		public bool? AirElectricHeater
		{
			get => _airElectricHeater;
			set => SetProperty(ref _airElectricHeater, value);
		}

		public bool? OtherHeatingTechnology
		{
			get => _otherHeatingTechnology;
			set => SetProperty(ref _otherHeatingTechnology, value);
		}

		public bool? AdjustableCoolantThermostat => throw new NotImplementedException();

		public bool EngineWasteGasHeatExchanger => throw new NotImplementedException();


		private bool _heatPumpGroupEditingEnabled;
		private BusHVACSystemConfiguration? _systemConfiguration;
		private HeatPumpType? _heatPumpTypeCoolingDriverCompartment;
		private HeatPumpType? _heatPumpTypeHeatingDriverCompartment;
		private HeatPumpType? _heatPumpTypeCoolingPassengerCompartment;
		private HeatPumpType? _heatPumpTypeHeatingPassengerCompartment;
		private ObservableCollection<Enum> _heatPumpTypeDriverCompartmentAllowedValues;
		private ObservableCollection<Enum> _heatPumpTypePassengerCompartmentAllowedValues;

		public IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get => this;
			set => throw new NotImplementedException();
		}
		public bool HeatPumpGroupEditingEnabled
		{
			get => _heatPumpGroupEditingEnabled;
			set
			{
				if (SetProperty(ref _heatPumpGroupEditingEnabled, value)) {
					_parameterViewModels[nameof(SystemConfiguration)].EditingEnabled = value;
					_parameterViewModels[nameof(HeatPumpTypeCoolingDriverCompartment)].EditingEnabled = value;
					_parameterViewModels[nameof(HeatPumpTypeCoolingPassengerCompartment)].EditingEnabled = value;
					_parameterViewModels[nameof(HeatPumpTypeHeatingDriverCompartment)].EditingEnabled = value;
					_parameterViewModels[nameof(HeatPumpTypeHeatingPassengerCompartment)].EditingEnabled = value;
				}
			}
		}

		private ObservableCollection<Enum> _systemConfigurationAllowedValues;
		public ObservableCollection<Enum> SystemConfigurationAllowedValues
		{
			get => _systemConfigurationAllowedValues;
			set
			{
				if (SetProperty(ref _systemConfigurationAllowedValues, value)) {
					_parameterViewModels[nameof(SystemConfiguration)].AllowedItems = value;
				}
			}
		}


		public BusHVACSystemConfiguration? SystemConfiguration
		{
			get => _systemConfiguration;
			set {
				if (SetProperty(ref _systemConfiguration, value)) {
					switch (value) {
						case BusHVACSystemConfiguration.Configuration6:
						case BusHVACSystemConfiguration.Configuration10:
							HeatPumpTypeDriverAllowedValues =
								EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, HeatPumpType>(HeatPumpType
									.not_applicable);
							break;
						default:
							HeatPumpTypeDriverAllowedValues =
								EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, HeatPumpType>(HeatPumpType
									.not_applicable);
							break;
					}
				}}
		}

		#region Implementation of IHVACBusAuxiliariesDeclarationData

		public HeatPumpType? HeatPumpTypeCoolingDriverCompartment
		{
			get => _heatPumpTypeCoolingDriverCompartment;
			set
			{
				SetProperty(ref _heatPumpTypeCoolingDriverCompartment, value);
			}
		}

		public ObservableCollection<Enum> HeatPumpTypeDriverAllowedValues
		{
			get => _heatPumpTypeDriverCompartmentAllowedValues;
			set
			{
				if (SetProperty(ref _heatPumpTypeDriverCompartmentAllowedValues, value)) {
					_parameterViewModels[nameof(HeatPumpTypeCoolingDriverCompartment)].AllowedItems = value;
					_parameterViewModels[nameof(HeatPumpTypeHeatingDriverCompartment)].AllowedItems = value;
				}
			}
		}

		public ObservableCollection<Enum> HeatPumpTypePassengerAllowedValues
		{
			get => _heatPumpTypePassengerCompartmentAllowedValues;
			set {
				if (SetProperty(ref _heatPumpTypePassengerCompartmentAllowedValues, value)) {
					_parameterViewModels[nameof(HeatPumpTypeCoolingPassengerCompartment)].AllowedItems = value;
					_parameterViewModels[nameof(HeatPumpTypeHeatingPassengerCompartment)].AllowedItems = value;
				}
			}
		}

		public HeatPumpType? HeatPumpTypeHeatingDriverCompartment
		{
			get => _heatPumpTypeHeatingDriverCompartment;
			set
			{
				SetProperty(ref _heatPumpTypeHeatingDriverCompartment, value);
			}
		}

		public HeatPumpType? HeatPumpTypeCoolingPassengerCompartment
		{
			get => _heatPumpTypeCoolingPassengerCompartment;
			set
			{
				SetProperty(ref _heatPumpTypeCoolingPassengerCompartment, value);
			}
		}

		public HeatPumpType? HeatPumpTypeHeatingPassengerCompartment
		{
			get => _heatPumpTypeHeatingPassengerCompartment;
			set
			{
				SetProperty(ref _heatPumpTypeHeatingPassengerCompartment, value);
			}
		}

		#endregion
		#endregion

		#region IElectricConsumersDeclaration

		//LED lights
		private bool? _interiorLightsLed;
		private bool? _dayrunninglightsLed;
		private bool? _positionlightsLed;
		private bool? _headlightsLed;
		private bool? _brakelightsLed;
	

		public bool? InteriorLightsLED
		{
			get => _interiorLightsLed;
			set
			{
				SetProperty(ref _interiorLightsLed, value);
			}
		}

		public bool? DayrunninglightsLED
		{
			get => _dayrunninglightsLed;
			set => SetProperty(ref _dayrunninglightsLed, value);
		}

		public bool? PositionlightsLED
		{
			get => _positionlightsLed;
			set => SetProperty(ref _positionlightsLed, value);
		}

		public bool? HeadlightsLED
		{
			get => _headlightsLed;
			set => SetProperty(ref _headlightsLed, value);
		}

		public bool? BrakelightsLED
		{
			get => _brakelightsLed;
			set => SetProperty(ref _brakelightsLed, value);
		}



		#endregion
		#region IPneumaticSuppyDeclarationData
		public IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get => this;
			set => throw new NotImplementedException();
		}

		private CompressorDrive _compressorDrive;
		private Dictionary<string, MultistageParameterViewModel> _parameterViewModels;


		public CompressorDrive CompressorDrive
		{
			get => _compressorDrive;
			set => SetProperty(ref _compressorDrive, value);
		}


		#endregion


		#region Implementation of interfaces (unused Properties);

		public XmlNode XMLSource => throw new NotImplementedException();

		public string FanTechnology => throw new NotImplementedException();

		public IList<string> SteeringPumpTechnology => throw new NotImplementedException();

		public IElectricSupplyDeclarationData ElectricSupply
		{
			get => this;
			set => throw new NotImplementedException();
		}

		public IElectricConsumersDeclarationData ElectricConsumers
		{
			get => this;
			set => throw new NotImplementedException();
		}

		

		public IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get => this;
			set => throw new NotImplementedException();
		}

	



		#endregion


		#region Implementation of IElectricSupplyDeclarationData

		public AlternatorType AlternatorTechnology => throw new NotImplementedException();
		public IList<IAlternatorDeclarationInputData> Alternators => throw new NotImplementedException();
		public bool ESSupplyFromHEVREESS { get; }
		public IList<IBusAuxElectricStorageDeclarationInputData> ElectricStorage => throw new NotImplementedException();

		public bool SmartElectrics => throw new NotImplementedException();

		public Watt MaxAlternatorPower => throw new NotImplementedException();

		public WattSecond ElectricStorageCapacity => throw new NotImplementedException();

		#endregion

		#region Implementation of IPneumaticSupplyDeclarationData

		

		public string Clutch => throw new NotImplementedException();

		public double Ratio => throw new NotImplementedException();

		public string CompressorSize => throw new NotImplementedException();

		public bool SmartAirCompression => throw new NotImplementedException();

		public bool SmartRegeneration => throw new NotImplementedException();

		#endregion

		#region Implementation of IPneumaticConsumersDeclarationData

		public ConsumerTechnology AirsuspensionControl => throw new NotImplementedException();

		public ConsumerTechnology AdBlueDosing => throw new NotImplementedException();

		#endregion

		#region Implementation of IDataErrorInfo

		public Dictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();

		public string this[string propertyName]
		{
			get
			{
				string result = null;
				switch (propertyName)
				{
					case nameof(HeatPumpTypeCoolingPassengerCompartment):
					case nameof(HeatPumpTypeCoolingDriverCompartment):
					case nameof(HeatPumpTypeHeatingPassengerCompartment):
					case nameof(HeatPumpTypeHeatingDriverCompartment):
					case nameof(SystemConfiguration):
						if (HeatPumpGroupEditingEnabled == true &&
							this.GetType().GetProperty(propertyName).GetValue(this) == null) {
							result = $"{NameResolver.ResolveName(propertyName, BusStrings.ResourceManager, Strings.ResourceManager)} has to be set if editing is enabled.";
						}
						break;
					default:
						if (_parameterViewModels[propertyName].EditingEnabled == true && this.GetType().GetProperty(propertyName).GetValue(this) == null)
						{
							result = $"{NameResolver.ResolveName(propertyName, BusStrings.ResourceManager, Strings.ResourceManager)} has to be set if editing is enabled.";
						}
						break;
				}
				//https://www.youtube.com/watch?v=5KF0GGObuAQ

				if (result == null)
				{
					if (Errors.ContainsKey(propertyName))
						Errors.Remove(propertyName);
				}
				else
				{
					Errors[propertyName] = result;
				}


				return result;
			}
		}

		public string Error
		{
			get
			{
				string result = string.Empty;
				var auxVmError = String.Join(",", Errors.Values);
				result = auxVmError;

				return result;
			}
		}

		public bool HasErrors => !string.IsNullOrEmpty(Error);

		public Dictionary<string, MultistageParameterViewModel> ParameterViewModels
		{
			get
			{
				return _parameterViewModels;
			}
			set
			{
				SetProperty(ref _parameterViewModels, value);
			}
		}

		#endregion
	}
}
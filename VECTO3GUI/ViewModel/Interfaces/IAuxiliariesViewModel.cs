using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Model;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces {
	public interface IAuxiliariesViewModel : IAuxiliariesBus, IComponentViewModel
	{
		IAuxiliariesDeclarationInputData ModelData { get; }

		string PneumaticSystemTechnology { get; set; }
		AllowedEntry<string>[] AllowedPneumaticSystemTechnologies { get; }

		string FanTechnology { get; set; }
		AllowedEntry<string>[] AllowedFanTechnologies { get; }

		ObservableCollection<SteeringPumpEntry> SteeringPumpTechnologies { get; }
		AllowedEntry<string>[] AllowedSteeringPumpTechnologies { get; }

		string ElectricSystemTechnology { get; set; }
		AllowedEntry<string>[] AllowedElectricSystemTechnologies { get; }

		string HVACTechnology { get; set; }
		AllowedEntry<string>[] AllowedHVACTechnologies { get; }

		#region Commands

		ICommand RemoveAlternatorCommand { get; }
		ICommand RemoveAllAlternatorCommand { get; }
		ICommand AddAlternatorCommand { get; }


		#endregion


		#region Bus Auxiliaries

		AllowedEntry<BusHVACSystemConfiguration>[] AllowedSystemConfigurations { get; }
		AllowedEntry<ACCompressorType>[] AllowedDriverACCompressorTypes { get; }
		AllowedEntry<ACCompressorType>[] AllowedPassengerACCompressorTypes { get; }
		AllowedEntry<string>[] AllowedAlternatorTechnology { get;  }

		#endregion
	}
}
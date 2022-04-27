using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using VECTOTrailer.Properties;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.Helper
{
    /// <summary>
    /// Wrapper class for stored settings, i.e. Input/Output path, simulation settings ...
    /// </summary>
    public class TrailerToolSettings : ObservableObject, ITrailerToolSettings
    {
		#region Implementation of ISettings

		private SimulationSettings _simulationSettings;
		private Properties.Settings _settings;

		public SimulationSettings SimulationSettings => _simulationSettings ?? (_simulationSettings =
			SettingsBase.Synchronized(Properties.SimulationSettings.Default) as SimulationSettings);
		public Properties.Settings GeneralSettings => _settings ?? (_settings =
			SettingsBase.Synchronized(Properties.Settings.Default) as Properties.Settings);


		public void Save()
		{
			SimulationSettings.Save();
			GeneralSettings.Save();
		}

		public TrailerToolSettings()
		{
            SimulationSettings.PropertyChanged += SimulationSettings_PropertyChanged;
			GeneralSettings.PropertyChanged += SimulationSettings_PropertyChanged;
		}

        private void SimulationSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion
    }

	public interface ITrailerToolSettings: INotifyPropertyChanged
	{
		Properties.SimulationSettings SimulationSettings { get; }
		Properties.Settings GeneralSettings { get; }

		void Save();

	}
}

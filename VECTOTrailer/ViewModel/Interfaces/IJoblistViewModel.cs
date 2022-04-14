using System.Collections.ObjectModel;
using System.Windows.Input;
using VECTOTrailer.Helper;
using VECTOTrailer.Model.Simulation;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface IJoblistViewModel : IMainView
	{
		ObservableCollectionEx<JobEntry> Jobs { get; }
		ObservableCollection<MessageEntry> Messages { get; }
		ICommand AddTrailerJob { get; }
		ICommand AddJob { get; }
		void HandleFileOpen(string filename);
		ICommand RemoveJob { get; }
		ICommand RemoveAllJobs { get; }
		ICommand MoveJobUp { get; }
		ICommand MoveJobDown { get; }
		ICommand EditJob { get; }
		ICommand EditCompletedFile { get; }
		ICommand CreateNewJob { get; }
		ICommand OpenJob { get; }
		ICommand ExitMainCommand { get; }
		ICommand OpenInFolder { get; }
		ICommand DoubleClickCommand { get; }
		ICommand RunSimulation { get; }
		ICommand StopSimulation { get; }
		ICommand ShowUserGuide { get; }
		bool CanRunSimulation { get; }
		bool CanStopSimulation { get; }
		//bool WriteModData { get; set; }
		//bool WriteModData1Hz { get; set; }
		//bool ValidateData { get; set; }
		//bool WriteActualModData { get; set; }
		//string OutputDirectory { get; set; }
		ICommand BrowseOutputDirectory { get; }
		//bool WriteModelData { get; set; }
		//bool WritePDFReport { get; set; }
		ITrailerToolSettings TrailerToolSettings { get; }
		ICommand AboutViewCommand { get; }
		ICommand BrowseAutoSimOutputDirectory { get; }
		ICommand BrowseAutoSimInputDirectory { get; }
		IBackGroundSimulator BackGroundSimulator { get; }
		ICommand ToggleAutoSim { get; }

		bool? AllJobsChecked { get; set; }
		bool WritePdfCanBeEdited { get; }
	}
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IJoblistViewModel : IMainView
	{
		ObservableCollectionEx<JobEntry> Jobs { get; }
		ObservableCollection<MessageEntry> Messages { get; }
		ICommand AddBusJob { get; }
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
		ICommand OpenSettings { get; }
		ICommand ExitMainCommand { get; }
		ICommand OpenInFolder { get; }
		ICommand DoubleClickCommand { get; }
		ICommand RunSimulation { get; }
		ICommand StopSimulation { get; }
		bool CanRunSimulation { get; }
		bool CanStopSimulation { get; }
		bool WriteModData { get; set; }
		bool WriteModData1Hz { get; set; }
		bool ValidateData { get; set; }
		bool WriteActualModData { get; set; }
		string OutputDirectory { get; set; }
		ICommand BrowseOutputDirectory { get; }
		bool WriteModelData { get; set; }
		ICommand AboutViewCommand { get; }
	}
}

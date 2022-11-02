using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.Interfaces
{
    public interface IJobListViewModel : IMainViewModel
    {
		ObservableCollection<IDocumentViewModel> Jobs { get; }
		ICommand NewManufacturingStageFileCommand { get; }
		ICommand NewCompletedInputCommand { get; }
		ICommand NewExemptedCompletedInputCommand { get; }
		IRelayCommand<bool> NewVifCommand { get; }
		ICommand EditDocument { get; set; }
		ICommand ViewXMLFile { get; set; }
		IAsyncRelayCommand RemoveJob { get; set; }
		ICommand OpenSourceFileCommand { get; }
		ICommand ShowSourceFileCommand { get; }
		IAsyncRelayCommand AddJobAsyncCommand { get; }
		Task<IDocumentViewModel> AddJobAsync(string fileName, bool runSimulationAfterAdding = false);
		void AddJob(IDocumentViewModel jobToAdd);
	}
}

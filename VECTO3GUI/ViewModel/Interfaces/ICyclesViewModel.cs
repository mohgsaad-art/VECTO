using System.Collections.ObjectModel;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface ICyclesViewModel : IComponentViewModel
	{
		ObservableCollection<string> Cycles { get; }
	}
}

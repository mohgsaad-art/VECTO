using System.Collections.ObjectModel;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface ICyclesViewModel : IComponentViewModel
	{
		ObservableCollection<string> Cycles { get; }
	}
}

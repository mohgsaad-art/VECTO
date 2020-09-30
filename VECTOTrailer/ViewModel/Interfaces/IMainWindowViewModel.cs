namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface IMainWindowViewModel
	{
		IMainView CurrentViewModel { get; }
		string Version { get; }
	}
}

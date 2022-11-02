namespace VECTO3GUI2020.ViewModel.Interfaces
{
    public interface ISettingsViewModel : IMainViewModel
    {
		string DefaultFilePath { get; set; }
		string DefaultOutputPath { get; set; }
		bool SerializeVectoRunData { get; set; }
		bool ActualModalData { get; set; }
		bool Validate { get; set; }
		bool ModalResults1Hz { get; set; }
		bool WriteModalResults { get; set; }
	}
}

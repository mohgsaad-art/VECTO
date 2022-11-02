using System.Windows.Input;

namespace VECTO3GUI2020.ViewModel.Interfaces
{
    public interface IMainWindowViewModel
    {
        IMainViewModel CurrentViewModel { get; set; }

		#region Commands
        ICommand OpenSettings { get;}
        #endregion
    }
}

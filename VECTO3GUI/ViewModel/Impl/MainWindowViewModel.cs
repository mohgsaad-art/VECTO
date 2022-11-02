using System.Reflection;
using System.Text.RegularExpressions;
using Ninject;
using VECTO3GUI.ViewModel.Interfaces;


namespace VECTO3GUI.ViewModel.Impl
{
	public class MainWindowViewModel : ObservableObject, IMainWindowViewModel
	{ 
		private IMainView _currentViewModel;
		private string _version;

		public string Version
		{
			get { return _version; }
			set { SetProperty(ref _version, value); }
		}
		

		public MainWindowViewModel(IKernel kernel)
		{
			Kernel = kernel;
			CurrentViewModel = Kernel.Get<IJoblistViewModel>();
			SetCurrentVersionNumber();
		}
		

		public IMainView CurrentViewModel
		{
			get { return _currentViewModel; }
			set { SetProperty(ref _currentViewModel, value); }
		}

		private void SetCurrentVersionNumber()
		{
			var assembly = Assembly.ReflectionOnlyLoadFrom("VectoCommon.dll");
			var regex = new Regex(@"Version=([0-9*\.+]*),");
			var versionNumber = regex.Match(assembly.FullName).Groups[1].Value;

			Version = $"VECTO-DEV {versionNumber}-DEV";
		}
	}
}

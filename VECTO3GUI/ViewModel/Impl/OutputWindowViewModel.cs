using System.Windows;
using Ninject;

namespace VECTO3GUI.ViewModel.Impl
{
	public class OutputWindowViewModel : ValidatingViewModel
	{
		private object _viewModel;
		private string _windowTitle;
	
		
		public object ViewModel
		{
			get { return _viewModel; }
			set { SetProperty(ref _viewModel, value); }
		}

		public string WindowTitle
		{
			get { return _windowTitle; }
			set { SetProperty(ref _windowTitle, value); }

		}

		public OutputWindowViewModel(IKernel kernel, object viewModel)
		{
			Kernel = kernel;
			ViewModel = viewModel;
			WindowTitle = string.Empty;
		}

		public OutputWindowViewModel(IKernel kernel, object viewModel, string windowTitle)
		{
			Kernel = kernel;
			ViewModel = viewModel;
			WindowTitle = windowTitle;
		}
	}
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.Views;

namespace VECTOTrailer.Helper
{

	public static class OutputWindowHelper
	{
		public static OutputWindow CreateOutputWindow(IKernel kernel, object viewModel,
			double width = default(double), double height= default(double), ITrailerToolSettings settings = null)
		{
			return CreateWindow(kernel, viewModel, string.Empty, width, height, settings);
		}
		
		public static OutputWindow CreateOutputWindow(IKernel kernel, object viewModel,
			string windowName, double width = default(double), double height = default(double),
			ResizeMode resizeMode = ResizeMode.CanResize, ITrailerToolSettings settings = null)
		{
			var window = CreateWindow(kernel, viewModel, windowName, width, height, settings);
			
			window.ResizeMode = resizeMode;
			return window;
		}
		
		private static OutputWindow CreateWindow(IKernel kernel, object viewModel, string windowName,
			double width, double height, ITrailerToolSettings settings = null)
		{
			var window = new OutputWindow
			{
				SizeToContent = SizeToContent.WidthAndHeight,
				DataContext = new OutputWindowViewModel(kernel, viewModel, windowName)
			};

			
			

			if (Math.Abs(width - default(double)) > 0)
				window.Width = width;
			if (Math.Abs(height - default(double)) > 0)
				window.Height = height;

			if (settings != null) {

			}

			return window;
		}
	}
}

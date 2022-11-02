using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.Views;

namespace VECTO3GUI.Helper
{

	public static class OutputWindowHelper
	{
		public static OutputWindow CreateOutputWindow(IKernel kernel, object viewModel,
			double width = default(double), double height= default(double))
		{
			return CreateWindow(kernel, viewModel, string.Empty, width, height);
		}
		
		public static OutputWindow CreateOutputWindow(IKernel kernel, object viewModel,
			string windowName, double width = default(double), double height = default(double),
			ResizeMode resizeMode = ResizeMode.CanResize)
		{
			var window = CreateWindow(kernel, viewModel, windowName, width, height);
			window.ResizeMode = resizeMode;
			return window;
		}
		
		private static OutputWindow CreateWindow(IKernel kernel, object viewModel, string windowName,
			double width, double height)
		{
			var window = new OutputWindow
			{
				DataContext = new OutputWindowViewModel(kernel, viewModel, windowName)
			};

			if (Math.Abs(width - default(double)) > 0)
				window.Width = width;
			if (Math.Abs(height - default(double)) > 0)
				window.Height = height;

			return window;
		}
	}
}

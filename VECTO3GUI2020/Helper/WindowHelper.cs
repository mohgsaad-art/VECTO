using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using Binding = System.Windows.Data.Binding;

namespace VECTO3GUI2020.Helper
{
	public class WindowHelper : IWindowHelper
    {
		

		public void ShowWindow(object viewModel)
		{
			IViewModelBase viewModelBase = (IViewModelBase)viewModel;
			var height = viewModelBase?.Height ?? Double.NaN;
			var width = viewModelBase?.Height ?? Double.NaN;
			var sizeToContent = viewModelBase?.SizeToContent ?? SizeToContent.Manual;
			var title = viewModelBase?.Title ?? GUILabels.DefaultTitle;
			var minHeight = viewModelBase?.MinHeight ?? 0;
			var minWidth = viewModelBase?.MinWidth ?? 0;


			var window = new Window {
				Content = viewModel,
				Height = height,
				Width = width,
				MinHeight = minHeight,
				MinWidth = minWidth,
				SizeToContent = sizeToContent,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				Title = title
			};
			if (viewModelBase != null) {
				SetBinding(viewModelBase, window, new PropertyPath(nameof(viewModelBase.Height)), FrameworkElement.HeightProperty );
				SetBinding(viewModelBase, window, new PropertyPath(nameof(viewModelBase.Width)), FrameworkElement.WidthProperty);
				SetBinding(viewModelBase, window, new PropertyPath(nameof(viewModelBase.SizeToContent)), Window.SizeToContentProperty);
				SetBinding(viewModelBase, window, new PropertyPath(nameof(viewModelBase.Title)), Window.TitleProperty);
				SetBinding(viewModelBase, window, new PropertyPath(nameof(viewModelBase.SizeToContent)), Window.SizeToContentProperty);
				SetBinding(viewModelBase, window, new PropertyPath(nameof(viewModelBase.Title)), Window.TitleProperty);


			}
			

			window.Show();
		}

		private static void SetBinding(IViewModelBase viewModelBase,
				Window window,
				PropertyPath path,
				DependencyProperty dependencyProperty,
				UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
				BindingMode mode = BindingMode.TwoWay
			)
		{
			Binding binding = new Binding() {
				Source = viewModelBase,
				Path = path,
				UpdateSourceTrigger = updateSourceTrigger,
				Mode = mode
			};
			BindingOperations.SetBinding(window, dependencyProperty, binding);
		}
	}
}

using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Ninject;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Interfaces.Common;

namespace VECTO3GUI2020.ViewModel.Implementation.Common
{
	/// <summary>
    /// Base Implementation of INotifyPropertyChanged
    /// https://docs.microsoft.com/en-us/dotnet/framework/wpf/data/how-to-implement-property-change-notification
    /// </summary>
    /// 
    public class ViewModelBase : ObservableObject, IViewModelBase
	{
		private string _error;

		#region Size And Window position
		private double? _width = 800;
		private double? _height = 600;
		public double? Width
		{
			get => _width;
			set => SetProperty(ref _width, value);
		}
		public double? Height
		{
			get => _height;
			set => SetProperty(ref _height, value);
		}

		private double? _minHeight = null;

		public double? MinHeight
		{
			get => _minHeight;
			set => SetProperty(ref _minHeight, value);
		}

		private double? _minWidth = null;

		public double? MinWidth
		{
			get => _minWidth;
			set => SetProperty(ref _minWidth, value);
		}

		public SizeToContent _sizeToContent = SizeToContent.Manual;
		public SizeToContent SizeToContent
		{
			get => _sizeToContent;
			set => _sizeToContent = value;
		}
		#endregion


		//borrowed from Franz Kober

		private string _title = GUILabels.DefaultTitle;
		public virtual string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		[Inject]
        public IDialogHelper DialogHelper { get; set; }


#region Commands
		protected bool AskForConfirmationOnClose { get; set; } = false;
		private ICommand _closeWindowCommand;
		public ICommand CloseWindowCommand
		{
			get
			{
				return _closeWindowCommand ?? new RelayCommand<Window>(window => CloseWindow(window, DialogHelper, AskForConfirmationOnClose), window => true);
			}
		}



		protected void CloseWindow(Window window, IDialogHelper dialogHelper, bool showDialog = true)
		{
			MessageBoxResult result;
			if (showDialog) {
				result = dialogHelper.ShowMessageBox("Do you really want to close?", "Close", MessageBoxButton.YesNo,
					MessageBoxImage.Question);
            } else {
				result = MessageBoxResult.Yes;
			}
			

			if (result == MessageBoxResult.Yes) {
				window?.Close();
			}
		}
		#endregion
	}
}

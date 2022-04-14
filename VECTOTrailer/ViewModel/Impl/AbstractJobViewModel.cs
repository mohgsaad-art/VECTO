using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ninject;
using VECTOTrailer.Helper;
using VECTOTrailer.Model;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Interfaces;
using Component = VECTOTrailer.Util.Component;

namespace VECTOTrailer.ViewModel.Impl
{
	public abstract class AbstractJobViewModel : AbstractViewModel
	{
		protected bool IsDeclarationMode;
		protected bool WindowAlreadyClosed;
		protected readonly SettingsModel SettingsModel;

		private IComponentViewModel _currentComponent;
		private ICommand _saveJobCommand;
		private ICommand _closeJobCommand;
		private ICommand _saveAsJobCommand;
		private Component _selectedComponent;

		public Component SelectedComponent
		{
			get { return _selectedComponent; }
			set
			{
				if (SetProperty(ref _selectedComponent, value)) {
					DoEditComponent(_selectedComponent);
				}
			}
		}
		
		protected string XmlFilePath { get; private set; }

		protected bool IsNewJob { get; set; }

		public override bool DeclarationMode
		{
			get { return IsDeclarationMode; }
		}

		public IComponentViewModel CurrentComponent
		{
			get { return _currentComponent; }
			protected set {
				SetProperty(ref _currentComponent, value);
			}
		}

		public AbstractJobViewModel(ITrailerToolSettings trailerToolSettings)
		{
			SettingsModel = new SettingsModel(settings:trailerToolSettings);
		}

		public ICommand SaveJob
		{
			get { return _saveJobCommand ?? (_saveJobCommand =  new RelayCommand<Window>(DoSaveJob, CanSaveJob)); }
		}
		protected virtual bool CanSaveJob(Window window)
		{
			return true;
		}
		protected abstract void DoSaveJob(Window window);


		public ICommand CloseJob
		{
			get { return _closeJobCommand ?? (_closeJobCommand = new RelayCommand<Window>(DoCloseJob, CanCloseJob));}
		}
		protected virtual bool CanCloseJob(Window obj)
		{
			return true;
		}
		protected abstract void DoCloseJob(Window window);


		public ICommand SaveAsJob
		{
			get { return _saveAsJobCommand ?? (_saveAsJobCommand = new RelayCommand<Window>(DoSaveAsJob, CanSaveAsJob)); }
		}
		protected virtual bool CanSaveAsJob(Window window)
		{
			return true;
		}
		protected abstract void DoSaveAsJob(Window window);


		public ICommand EditComponent
		{
			get { return new RelayCommand<Component>(DoEditComponent); }
		}
		protected virtual void DoEditComponent(Component component)
		{
			var nextView = GetComponentViewModel(component);

			CurrentComponent = nextView ?? Kernel.Get<INoneViewModel>();
		}

		public ICommand CloseWindowCommand
		{
			get { return  new RelayCommand<CancelEventArgs>(DoCloseWindow);}
		}

		private void DoCloseWindow(CancelEventArgs cancelEvent)
		{
			if (WindowAlreadyClosed)
				return;

			if(!CloseWindowDialog())
				cancelEvent.Cancel = true;
		}


		protected void CreateComponentModel(Component component)
		{
			var viewModelType = ViewModelFactory.ComponentViewModelMapping[component];
			if (!typeof(IComponentViewModel).IsAssignableFrom(viewModelType)) {
				throw new Exception("Invalid entry in ViewModel Mapping");
			}

			var subModels = GetSubmodels().ToArray();
			if (!subModels.Contains(component)) {
				var viewModel = (IComponentViewModel)Kernel.Get(viewModelType);
				RegisterSubmodel(component, viewModel);
			}
		}

		protected void SetXmlFilePath(string baseUri)
		{
			XmlFilePath = XmlHelper.GetXmlAbsoluteFilePath(baseUri);
		}

		protected bool CloseWindowDialog()
		{

			var res = MessageBox.Show("Do you really want to close ?", "Close", MessageBoxButton.YesNo);

			return res == MessageBoxResult.Yes;
		}
	}
}

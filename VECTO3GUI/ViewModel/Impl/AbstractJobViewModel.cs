using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Ninject;
using VECTO3GUI.Helper;
using VECTO3GUI.Model;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;
using Component = VECTO3GUI.Util.Component;

namespace VECTO3GUI.ViewModel.Impl
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

		public AbstractJobViewModel()
		{
			SettingsModel = new SettingsModel();
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

			if (CurrentComponent is AuxiliariesViewModel) {
				var convert = CurrentComponent as AuxiliariesViewModel;
				convert?.CacheAlternatorTechnologies();
			}

			if (nextView is AuxiliariesViewModel) {
				var convert = nextView as AuxiliariesViewModel;
				convert.LoadCachedAlternatorTechnologies();
			}

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
			var dialogSettings = new MetroDialogSettings()
			{
				AffirmativeButtonText = "Yes",
				NegativeButtonText = "No",
				AnimateShow = true,
				AnimateHide = true
			};

			var res = MetroDialogHelper.GetModalDialogBox(this,
				"", "Do you really want to close ?", dialogSettings);

			return res == MessageDialogResult.Affirmative;
		}
	}
}

using System.Windows;
using Ninject.Modules;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Model.Implementation;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.ViewModel;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace VECTO3GUI2020.Ninject
{
	public class Vecto3GUI2020Module : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{

			Bind<IJobListViewModel>().To<JobListViewModel>().InSingletonScope();
			Bind<IMainWindowViewModel>().To<MainWindowViewModel>();
			Bind<IMainViewModel>().To<JobListViewModel>();
			Bind<ISettingsViewModel>().To<SettingsViewModel>();
			Bind<IOutputViewModel>().To<OutputViewModel>().InSingletonScope();
			Bind<ISettingsModel>().To<SettingsModel>().InSingletonScope();
			Bind<IDialogHelper>().To<DialogHelper>().InSingletonScope();
			Bind<IWindowHelper>().To<WindowHelper>();
			Bind<AboutViewModel>().ToSelf().InSingletonScope();
			
		}

		#endregion
	}
}
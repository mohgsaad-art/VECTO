using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using VECTOTrailer.Model.Simulation;
using VECTOTrailer.ViewModel.Adapter;
using VECTOTrailer.ViewModel.Adapter.Declaration;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel container;

        #region Overrides of Application

        protected override void OnStartup(StartupEventArgs e)
		{
			ConfigureContainer();
            base.OnStartup(e);
        }

        private void ConfigureContainer()
        {
            container = new StandardKernel(new VectoNinjectModule());

            container.Bind<IAdapterFactory>().ToFactory();

            container.Bind<IMainWindowViewModel>().To<MainWindowViewModel>();
			container.Bind<IJoblistViewModel>().To<JoblistViewModel>();
			container.Bind<IJobEditViewModel>().To<DeclarationJobViewModel>();
            container.Bind<IJobEditViewModel>().To<CompleteTrailerJobViewModel>();
            container.Bind<INoneViewModel>().To<NoneViewModel>();
			container.Bind<ICompleteTrailerViewModel>().To<CompleteTrailerViewModel>();
			container.Bind<IAutoSimulator>().To<AutoSimulator>();
            
			var mainWindow = container.Get<MainWindow>();
            mainWindow.Show();
        }

        #endregion
    }
}

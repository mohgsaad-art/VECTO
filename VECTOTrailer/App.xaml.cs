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
using NLog.Config;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using VECTOTrailer.Helper;
using VECTOTrailer.Model;
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
		private IPdfCreator _pdfCreator;
		private ITrailerToolSettings _trailerToolSettings;
        #region Overrides of Application

        protected override void OnStartup(StartupEventArgs e)
		{
			ConfigureNLog();
			ConfigureContainer();
			ConfigurePdfCreator();
			CreateSettings();
			ShowMainWindow();
			base.OnStartup(e);
        }

		private void ConfigurePdfCreator()
		{
			_pdfCreator = container.Get<IPdfCreator>();
			if (!_pdfCreator.Init()) {
				MessageBox.Show("PDF Creation not supported");
			};
		}

		private void CreateSettings()
		{
			_trailerToolSettings = container.Get<ITrailerToolSettings>();
		}

		private void ConfigureNLog()
		{
			NLog.LogManager.Configuration = new LoggingConfiguration();
		}

        private void ConfigureContainer()
        {
            container = new StandardKernel(new VectoNinjectModule());

            container.Bind<IAdapterFactory>().ToFactory();

            container.Bind<IMainWindowViewModel>().To<MainWindowViewModel>();
			container.Bind<IJoblistViewModel>().To<JoblistViewModel>().InSingletonScope();
			container.Bind<IJobEditViewModel>().To<DeclarationJobViewModel>();
            container.Bind<IJobEditViewModel>().To<CompleteTrailerJobViewModel>();
            container.Bind<INoneViewModel>().To<NoneViewModel>();
			container.Bind<ICompleteTrailerViewModel>().To<CompleteTrailerViewModel>();
			container.Bind<ISimulationBackgroundWorker>().To<SimulationBackgroundWorker>();
			container.Bind<IPdfCreator>().To<PdfCreator>().InSingletonScope();
			container.Bind<ITrailerToolSettings>().To<TrailerToolSettings>().InSingletonScope();
			container.Bind<SettingsModel>().ToSelf();

		}

		private void ShowMainWindow()
		{
			var mainWindow = container.Get<MainWindow>();
			mainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			_pdfCreator.ShutDown();
			_trailerToolSettings.Save();
			base.OnExit(e);
		}

		#endregion
    }
}

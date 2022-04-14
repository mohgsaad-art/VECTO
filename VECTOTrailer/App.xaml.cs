using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Castle.Core.Smtp;
using Ninject;
using Ninject.Extensions.Factory;
using NLog;
using NLog.Config;
using NLog.Targets;
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

		private bool _installModeActivated = false;
		private string _installIni = "install.ini";

		#region Overrides of Application

        protected override void OnStartup(StartupEventArgs e)
		{
			ReadInstallMode();
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
			NLog.LogManager.Configuration = new LoggingConfiguration() { };
			var folderName = "";
			if (_installModeActivated) {
				folderName = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create),
						Assembly.GetExecutingAssembly().GetName().Name
					);
			}
			LogFileHelper.SetBasePath(Path.GetFullPath(Path.Combine(folderName,"Log")));
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
			container.Bind<IBackGroundSimulator>().To<BackGroundSimulator>();
			container.Bind<IPdfCreator>().To<PdfCreator>().InSingletonScope();
			container.Bind<ITrailerToolSettings>().To<TrailerToolSettings>().InSingletonScope();
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


		private void ReadInstallMode()
		{
			if (File.Exists(Path.GetFullPath(_installIni))) {
				try {
					var lines = File.ReadLines(_installIni)
						.Where(s => s.SkipWhile(c => c.Equals(" ")).First() != '#');
					foreach (var line in lines) {
						var executionModeRegex = new Regex(@"ExecutionMode\s*=\s*install");
						if (executionModeRegex.IsMatch(line)) {
							_installModeActivated = true;
							break;
						}
					}
				} catch (Exception ex) {
					Debug.WriteLine($"Error parsing {_installIni}");
				}
			}
		}

		#endregion
	}
}

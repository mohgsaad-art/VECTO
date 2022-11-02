using System;
using System.Windows;
using Ninject;
using TUGraz.VectoCore;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Ninject.Vehicle;
using VECTO3GUI2020.Properties;
using Application = System.Windows.Application;

namespace VECTO3GUI2020
{
    public partial class App : Application
    {

        private IKernel container;

		protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

			if (Settings.Default.DefaultFilePath == null) {
				Settings.Default.DefaultFilePath = Environment.CurrentDirectory;
                Settings.Default.Save();
			}
            ConfigureContainer();
            ConfigureMainWindow();
            
        }

        private void ConfigureContainer()
        {
            container = new StandardKernel(
                new VectoNinjectModule(),
                new JobEditModule(),
                new ComponentModule(),
				new DocumentModule(),
				new XMLWriterFactoryModule(),
                new FactoryModule(),
                new MultistageModule(),
                new Vecto3GUI2020Module()
			);






		}

        private void ConfigureMainWindow()
        {
			var mainwindow = container.Get<MainWindow>();
            this.MainWindow = mainwindow;
			Application.Current.MainWindow = mainwindow;
            this.MainWindow.Show();
		}

    }

}

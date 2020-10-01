using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
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
            container.Bind<IVehicleViewModel>().To<VehicleViewModel>();
            container.Bind<ICompleteTrailerViewModel>().To<CompleteTrailerViewModel>();

            container.Bind<IAirdragViewModel>().To<AirdragViewModel>();
            container.Bind<IAngledriveViewModel>().To<AngledriveViewModel>();
            container.Bind<IAxlegearViewModel>().To<AxlegearViewModel>();
            container.Bind<IEngineViewModel>().To<EngineViewModel>();
            container.Bind<IGearboxViewModel>().To<GearboxViewModel>();
            container.Bind<IRetarderViewModel>().To<RetarderViewModel>();
            container.Bind<ITorqueConverterViewModel>().To<TorqueConverterViewModel>();
            container.Bind<ICyclesViewModel>().To<CyclesViewModel>();
            container.Bind<IAuxiliariesViewModel>().To<AuxiliariesViewModel>();

            container.Bind<IAxlesViewModel>().To<AxlesViewModel>();
            container.Bind<IAxleViewModel>().To<AxleViewModel>();
            container.Bind<ITyreViewModel>().To<TyreViewModel>();


            container.Bind<IAirdragDeclarationInputData>().To<AirdragDeclarationAdapter>();
            container.Bind<IAngledriveInputData>().To<AngledriveDeclarationAdapter>();
            container.Bind<IAuxiliariesDeclarationInputData>().To<AuxiliariesDeclarationAdapter>();
            container.Bind<IAxleDeclarationInputData>().To<AxleDeclarationAdapter>();
            container.Bind<IAxleGearInputData>().To<AxlegearDeclarationAdapter>();
            container.Bind<IEngineDeclarationInputData>().To<EngineDeclarationAdapter>();
            container.Bind<IGearboxDeclarationInputData>().To<GearboxDeclarationAdapter>();
            container.Bind<IRetarderInputData>().To<RetarderDeclarationAdapter>();
            container.Bind<ITorqueConverterDeclarationInputData>().To<TorqueConverterDeclarationAdapter>();
            container.Bind<ITyreDeclarationInputData>().To<TyreDeclarationAdapter>();
            container.Bind<IVehicleDeclarationInputData>().To<VehicleDeclarationAdapter>();

            var mainWindow = container.Get<MainWindow>();
            mainWindow.Show();
        }

        #endregion
    }
}

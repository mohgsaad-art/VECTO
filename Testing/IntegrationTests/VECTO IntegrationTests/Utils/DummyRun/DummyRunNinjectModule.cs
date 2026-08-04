using Ninject.Modules;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunNinjectModule : AbstractNinjectModule, INinjectModule
{
    #region Overrides of NinjectModule

    public override void Load()
    {
        Rebind<ISimulatorFactory>().To<DummyRunDeclarationSimulatorFactory>().Named(ExecutionMode.Declaration.ToString());

        Rebind<IModalDataFactory>().To<DummyRunModDataFactory>().InSingletonScope();

        Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
        Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
        Rebind<IPowertrainBuilder>().To<DummyRunPowertrainBuilder>().InSingletonScope();
		Rebind<IPowertrainComponentFactory>().To<DummyRunPowertrainComponentFactory>().InSingletonScope();
		//Rebind<IVehicleContainerFactory>().To<DummyRunVehicleContainerFactory>().InSingletonScope();
	}

    #endregion
}
    
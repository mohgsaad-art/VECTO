using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

internal class DummyRunPowertrainBuilder : IPowertrainBuilder
{
    protected readonly IPowertrainComponentFactory _powertrainComponentFactory;

    public DummyRunPowertrainBuilder(IPowertrainComponentFactory powertrainComponentFactory)
    {
        _powertrainComponentFactory = powertrainComponentFactory;
    }

    #region Implementation of IPowertrainBuilder

    public IVehicleContainer Build(VectoRunData data, IModalDataContainer modData, ISumData sumWriter = null)
    {
        return _powertrainComponentFactory.CreateVehicleContainer(data, modData, sumWriter);
    }

    public IExemptedVehicleContainer BuildExempted(VectoRunData data)
    {
        return _powertrainComponentFactory.CreateExemptedVehicleContainer(data, null, null);
    }

    public IShiftStrategy GetShiftStrategy(IVehicleContainer container)
    {
        throw new NotImplementedException();
    }

    public string GetShiftStrategyName(GearboxType gearboxType, VectoSimulationJobType jobType, bool isTestPowerTrain = false)
    {
        throw new NotImplementedException();
    }

    #endregion
}
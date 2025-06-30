using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

internal class DummyRunExemptedRun : ExemptedRun
{

    public DummyRunExemptedRun(IExemptedVehicleContainer data, Action<ModalDataContainer> writeSumData) : base(data,
        writeSumData)
    { }

    #region Overrides of ExemptedRun

    protected override void CheckValidInput()
    {
        return;
    }

    #endregion
}
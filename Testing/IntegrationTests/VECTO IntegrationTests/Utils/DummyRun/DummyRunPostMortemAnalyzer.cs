using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunPostMortemAnalyzer : IPostMortemAnalyzer
{
    public DummyRunNonExemptedRun Run { get; set; }

    #region Implementation of IPostMortemAnalyzer

    public bool AbortSimulation(IVehicleContainer container, Exception ex)
    {
        if (Run.IgnoreSimulationRun)
        {
            container.RunStatus = VectoRun.Status.PrimaryBusSimulationIgnore;
            return false;
        }
        return true;
    }

    #endregion
}
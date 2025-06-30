using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunNonExemptedRun : VectoRun
{
    private IVehicleContainer _vehicleContainer;

    public DummyRunNonExemptedRun(IVehicleContainer container, IPostMortemAnalyzer postMortem = null) : base(container, postMortem: postMortem)
    {
        _vehicleContainer = container;
        if (postMortem != null)
        {
            var dummyPm = postMortem as DummyRunPostMortemAnalyzer;
            dummyPm.Run = this;
        }
    }


    #region Overrides of VectoRun
    public override double Progress => 1;
    public bool FinishedWithError { get; set; }

    public bool IgnoreSimulationRun { get; set; }

    protected override IResponse DoSimulationStep()
    {
        _vehicleContainer.ModalData[ModalResultField.Gear] = 0u;
        _vehicleContainer.ModalData[ModalResultField.v_act] = 0.SI<MeterPerSecond>();
        _vehicleContainer.ModalData[ModalResultField.ICEOn] = false;
        _vehicleContainer.ModalData[ModalResultField.simulationInterval] = 1.SI<Second>();
        _vehicleContainer.ModalData.CommitSimulationStep();

        if (FinishedWithError)
        {
            throw new VectoException("Simulation run intentionally aborted!");
        }

        if (IgnoreSimulationRun)
        {
            throw new VectoException("Simulation run shall be ignored");
        }


        FinishedWithoutErrors = true;

        return new ResponseCycleFinished(this);
    }

    public override bool CalculateAggregateValues()
    {
        return false;
    }

    protected override bool CheckCyclePortProgress()
    {
        return false;
    }

    protected override IResponse Initialize()
    {
        return new ResponseSuccess(this);
    }

    #endregion
}
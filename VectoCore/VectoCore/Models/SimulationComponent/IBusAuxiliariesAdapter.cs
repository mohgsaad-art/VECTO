using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface IBusAuxiliariesAdapter : IAuxInProvider, IAuxPort
    {
        void DoWriteModalResultsICE(Second time, Second simulationInterval, IModalDataContainer container);

        ISimpleBattery ElectricStorage { get; set; }
        IDCDCConverter DCDCConverter { get; set; }
    }
}
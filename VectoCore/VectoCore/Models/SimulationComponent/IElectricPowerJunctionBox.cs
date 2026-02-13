using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface IElectricPowerJunctionBoxPort : IElectricSystem
    {
        
    }
    public interface IElectricPowerJunctionBox : IUpdateable
    {
        void ClearResults();

        void Connect(IElectricSystem powerSupply);

        IElectricSystem GetElectricPowerJunctionBoxPort(IPowertrainContainer powertrainContainer);

    }
}

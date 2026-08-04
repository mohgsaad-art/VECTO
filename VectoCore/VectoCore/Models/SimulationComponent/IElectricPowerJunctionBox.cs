using TUGraz.VectoCore.Models.Connector.Ports;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface IElectricPowerJunctionBox : IElectricSystem, IUpdateable
    {
        ITnOutPort CurrentTorqueSplitterNextComponent { get; set; }

        void AddTorqueSplitterNextComponent(ITnOutPort component);

        void ClearResults();

        void Connect(IElectricSystem powerSupply);
    }
}

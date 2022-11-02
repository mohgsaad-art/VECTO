using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IElectricMotor : IPowerTrainComponent, IElectricMotorInfo
	{
		void Connect(IElectricSystem powersupply);
		
		BusAuxiliariesAdapter BusAux { set; }
	}
}
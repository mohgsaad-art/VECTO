using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface ITestPowertrainTransmission : IGearbox, ITnOutPort, IUpdateable
    {
        GearshiftPosition SetGear { set; }
        GearshiftPosition SetNextGear { set; }
        bool SetDisengaged { set; }
        bool SetDisengageGearbox { set; }
        Second SetEngageTime { set; }
    }

	public interface ITestPowertrainVehicle : IVehicle, IDriverDemandOutPort
	{

	}
}
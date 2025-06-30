using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class TestPowertrainAPTNGearbox : APTNGearbox, ITestPowertrainTransmission
	{
		public TestPowertrainAPTNGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container,
			strategy, false)
		{
			if (!container.IsTestPowertrain) {
				throw new VectoException("This class shall not be used in a real powertrain!");
			}
		}

		#region Implementation of ITestPowertrainTransmission

		public GearshiftPosition SetGear {
			set => Gear = value;
		}
		public GearshiftPosition SetNextGear {
			set => _nextGear = value;
		}
		public bool SetDisengaged {
			set => Disengaged = value;
		}
		public bool SetDisengageGearbox {
			set => DisengageGearbox = value; // throw new NotImplementedException();
		}

		public Second SetEngageTime {
			set => EngageTime = value;
		}

		#endregion
	}
}
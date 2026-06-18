using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class TestPowertrainAPTNGearbox : APTNGearbox, ITestPowertrainTransmission
	{
		public TestPowertrainAPTNGearbox(IVehicleContainer container, IPowertrainContainer powertrainContainer, IShiftStrategy strategy) 
			: base(container, powertrainContainer, strategy, false)
		{
			if (!container.IsTestPowertrain) {
				throw new Exception("This class shall not be used in a real powertrain!");
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
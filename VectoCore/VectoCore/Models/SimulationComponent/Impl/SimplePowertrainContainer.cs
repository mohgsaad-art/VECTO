using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class SimplePowertrainContainer : VehicleContainer, IDriverInfo
	{
		public SimplePowertrainContainer(VectoRunData runData, IModalDataContainer modData = null) : base(runData.ExecutionMode, modData)
		{
			RunData = runData;
		}

		public IDriverDemandOutPort VehiclePort => (Vehicle as Vehicle)?.OutPort();

		public ITnOutPort GearboxOutPort => (Gearbox as IGearbox)?.OutPort();

		public IGearbox GearboxCtlTest => Gearbox as IGearbox;

		public override Second AbsTime => 0.SI<Second>();

		public override IDriverInfo DriverInfo => base.Driver ?? this;

		public override bool IsTestPowertrain => true;

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior => DrivingBehavior.Driving;

		public DrivingAction DrivingAction => DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;

		#endregion
	}
}
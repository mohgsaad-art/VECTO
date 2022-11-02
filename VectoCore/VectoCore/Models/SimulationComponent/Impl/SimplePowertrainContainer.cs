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

		public IDriverDemandOutPort VehiclePort => (VehicleInfo as Vehicle)?.OutPort();

		public ITnOutPort GearboxOutPort => (GearboxInfo as IGearbox)?.OutPort();

		public IGearbox GearboxCtlTest => GearboxInfo as IGearbox;

		public override Second AbsTime => 0.SI<Second>();

		public override IDriverInfo DriverInfo => base.DriverInfo ?? this;

		public override bool IsTestPowertrain => true;

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior => DrivingBehavior.Driving;

		public DrivingAction DrivingAction => DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();

		#endregion
	}
}
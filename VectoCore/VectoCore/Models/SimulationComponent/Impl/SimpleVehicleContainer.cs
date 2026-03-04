using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	
	public class SimpleVehicleContainer : VehicleContainer, IDriverInfo, ISimpleVehicleContainer
	{
		public SimpleVehicleContainer(VectoRunData runData, ISimplePowertrainBuilder ptBuilder) : base(runData, null, null, ptBuilder)
		{
			RunData = runData;
		}

		//public IDriverDemandOutPort VehiclePort => (VehicleInfo as Vehicle)?.OutPort();

		// public IList<ITnOutPort> GearboxOutPorts => Gearboxes.Select(gi => (gi as IGearbox)?.OutPort()).ToList();
		

		public override Second AbsTime => 0.SI<Second>();

		public override IDriverInfo DriverInfo => base.DriverInfo ?? this;

		public override bool IsTestPowertrain => true;

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior => DrivingBehavior.Driving;

		public DrivingAction DrivingAction => DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

		#endregion

		// public Dictionary<PowertrainPosition, IElectricMotorInfo> ElectricMotors => base.EMs.ToDictionary(x => x.Position, x => x);

		public IReadOnlyDictionary<AxleNumber, ISimplePowertrainContainer> SimplePowertrains => Powertrains.ToDictionary(kv => kv.Key, kv => (ISimplePowertrainContainer)kv.Value);

		public void UpdateComponents(IDataBus realContainer) => UpdateComponentsInternal(realContainer);
	}
}
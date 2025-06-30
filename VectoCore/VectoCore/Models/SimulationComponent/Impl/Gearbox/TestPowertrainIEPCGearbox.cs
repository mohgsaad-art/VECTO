using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class TestPowertrainIEPCGearbox : ITestPowertrainTransmission
	{
		protected ITestPowertrainTransmission _impl;

		//public TestPowertrainIEPCGearbox(IVehicleContainer container, IShiftStrategy strategy) : this(container, strategy, null, false) { }


		public TestPowertrainIEPCGearbox(IVehicleContainer container, IShiftStrategy strategy,
			IIEPCGearboxFactory gbxFactory) : this(container, strategy, gbxFactory, false)
		{
			if (!container.IsTestPowertrain) {
				throw new VectoException("This class shall not be used in a real powertrain!");
			}
		}

		protected TestPowertrainIEPCGearbox(IVehicleContainer container, IShiftStrategy strategy,
			IIEPCGearboxFactory gbxFactory, bool dummy)
		{
			_impl = gbxFactory.CreateIEPCGearbox(container.RunData.GearboxData.Gears.Count == 1, container, strategy) as ITestPowertrainTransmission;
			if (_impl == null) {
				throw new VectoException("Invalid implementation provided for Testpowertrain!");
			}
		}

		#region Implementation of ITnInProvider

		public ITnInPort InPort()
		{
			return _impl.InPort();
		}

		#endregion

		#region Implementation of ITnOutProvider

		public ITnOutPort OutPort()
		{
			return _impl.OutPort();
		}

		#endregion

		#region Implementation of IGearboxInfo

		public GearboxType GearboxType => _impl.GearboxType;

		public GearshiftPosition Gear => _impl.Gear;

		public bool TCLocked => _impl.TCLocked;

		public Watt GearboxLoss()
		{
			return _impl.GearboxLoss();
		}

		public Second LastShift => _impl.LastShift;

		public Second LastUpshift => _impl.LastUpshift;

		public Second LastDownshift => _impl.LastDownshift;

		public GearData GetGearData(uint gear)
		{
			return _impl.GetGearData(gear);
		}

		public GearshiftPosition NextGear => _impl.NextGear;

		public Second TractionInterruption => _impl.TractionInterruption;

		public uint NumGears => _impl.NumGears;

		public bool Disengaged => _impl.Disengaged;

		public bool DisengageGearbox
		{
			get => ((TUGraz.VectoCore.Models.Simulation.DataBus.IGearboxInfo)_impl).DisengageGearbox;
			set => ((TUGraz.VectoCore.Models.Simulation.DataBus.IGearboxControl)_impl).DisengageGearbox = value;
		}

		public void TriggerGearshift(Second absTime, Second dt)
		{
			_impl.TriggerGearshift(absTime, dt);
		}

		public event Action GearShiftTriggered
		{
			add => _impl.GearShiftTriggered += value;
			remove => _impl.GearShiftTriggered -= value;
		}

		public bool GearEngaged(Second absTime)
		{
			return _impl.GearEngaged(absTime);
		}

		public bool RequestAfterGearshift
		{
			get => _impl.RequestAfterGearshift;
			set => _impl.RequestAfterGearshift = value;
		}

		public IShiftStrategy Strategy => _impl.Strategy;

		#endregion

		#region Implementation of IUpdateable

		public bool UpdateFrom(object other)
		{
			return _impl.UpdateFrom(other);
		}

		#endregion

		#region Implementation of ITnOutPort

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			return _impl.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return _impl.Initialize(outTorque, outAngularVelocity);
		}

		#endregion

		#region Implementation of ITestPowertrainTransmission

		public GearshiftPosition SetGear
		{
			set => _impl.SetGear = value;
		}

		public GearshiftPosition SetNextGear
		{
			set => _impl.SetNextGear = value;
		}

		public bool SetDisengaged
		{
			set => _impl.SetDisengaged = value;
		}

		public bool SetDisengageGearbox
		{
			set => _impl.SetDisengageGearbox = value;
		}

		public Second SetEngageTime
		{
			set => _impl.SetEngageTime = value;
		}

		#endregion
	}
}
using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class IEPCGearbox : IIEPCGearbox, IGearbox
	{
		protected IGearbox _impl;

		public IEPCGearbox(IVehicleContainer container, IShiftStrategy strategy, IIEPCGearboxFactory gbxFactory) : this(container, strategy, gbxFactory, false)
		{
			if (container.IsTestPowertrain) {
				throw new VectoException(
					"This class shall not be used in a testpowertrain - use the dedicated class instead!");
			}
        }

		protected IEPCGearbox(IVehicleContainer container, IShiftStrategy strategy, IIEPCGearboxFactory gbxFactory, bool dummy)
		{
			_impl = gbxFactory.CreateIEPCGearbox(container.RunData.GearboxData.Gears.Count == 1, container, strategy);
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
			get => (_impl as IGearboxInfo).DisengageGearbox;
			set => (_impl as IGearboxControl).DisengageGearbox = value;
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
	}
}
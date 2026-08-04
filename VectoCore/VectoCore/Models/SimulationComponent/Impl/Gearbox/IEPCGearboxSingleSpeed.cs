using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class IEPCGearboxSingleSpeed : TransmissionComponent, IGearbox, IIEPCGearbox
    {
        protected GearData GearData;

        public event Action GearShiftTriggered;

        public IShiftStrategy Strategy => null;

		public IEPCGearboxSingleSpeed(IVehicleContainer container, IShiftStrategy strategy, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) 
            : this(container, strategy, false, axleNumber)
		{
			if (container.IsTestPowertrain) {
				throw new VectoException(
					"This class shall not be used in a testpowertrain - use the dedicated class instead!");
			}
        }

        protected IEPCGearboxSingleSpeed(IVehicleContainer container, IShiftStrategy strategy, bool dummy, int axleNumber) : base(container,
            container.RunData.GetGearboxData().First(x => x.Item1 == axleNumber).Item2.Gears.First().Value, axleNumber)
		{
			var modelData = container.RunData.GetGearboxData().FirstOrDefault(x => x.Item1 == axleNumber).Item2;
            GearboxType = modelData.Type;
            Gear = new GearshiftPosition(1);
            TCLocked = true;
            GearData = modelData.Gears.First().Value;
        }

        public override IResponse Request(
            Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
            bool dryRun)
        {
            var retVal = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
            retVal.Gearbox.PowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
            retVal.Gearbox.OutputTorque = outTorque;
            retVal.Gearbox.OutputSpeed = outAngularVelocity;



            return retVal;
        }

        #region Overrides of VectoSimulationComponent

        protected override void DoWriteModalResults(Second time, Second simulationInterval,
            IModalDataContainer container)
        {
            container[ModalResultField.Gear, AxleNumber.FormatAxleNumber()] = Gear.Gear;
            container[ModalResultField.n_IEPC_out_avg, AxleNumber.FormatAxleNumber()] = (PreviousState.OutAngularVelocity +
                                                        CurrentState.OutAngularVelocity) / 2.0;
            container[ModalResultField.T_IEPC_out, AxleNumber.FormatAxleNumber()] = CurrentState.OutTorque;
        }

        protected override bool DoUpdateFrom(object other) => false;

        #endregion

        #region Implementation of IGearboxInfo

        public GearboxType GearboxType { get; }
        public GearshiftPosition Gear { get; }
        public bool TCLocked { get; }

        public Watt GearboxLoss()
        {
            //MQ: 2022-05-18: single-speed gearbox is only used in IEPC as 'virtual component' without losses
            return 0.SI<Watt>();
        }

        public Second LastShift { get; }
        public Second LastUpshift { get; }
        public Second LastDownshift { get; }

        public GearData GetGearData(uint gear)
        {
            return GearData;
        }

        public GearshiftPosition NextGear { get; }
        public Second TractionInterruption { get; }
        public uint NumGears { get; }
        public bool Disengaged { get; }
        public bool DisengageGearbox { get; set; }

        public void TriggerGearshift(Second absTime, Second dt)
        {
            GearShiftTriggered?.Invoke();
            throw new NotImplementedException();
        }

        public bool GearEngaged(Second absTime)
        {
            return true;
        }

        public bool RequestAfterGearshift { get; set; }

        #endregion
    }
}
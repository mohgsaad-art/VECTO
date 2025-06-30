using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class DummyGearboxInfo : VectoSimulationComponent, IGearboxInfo
	{
		public DummyGearboxInfo(IVehicleContainer container, GearshiftPosition gear = null) : base(container)
		{
			Gear = gear ?? new GearshiftPosition(1);
		}

		public IShiftStrategy Strategy => null;
		
		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container) { }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval) { }

		#endregion

		#region Implementation of IGearboxInfo

		public GearboxType GearboxType => GearboxType.APTN;

		public GearshiftPosition Gear { get; }

		public bool TCLocked => true;

		public MeterPerSecond StartSpeed => DeclarationData.GearboxTCU.StartSpeed;

		public MeterPerSquareSecond StartAcceleration => DeclarationData.GearboxTCU.StartAcceleration;

		public Watt GearboxLoss()
		{
			return 0.SI<Watt>();
		}

		public Second LastShift => -double.MaxValue.SI<Second>();

		public Second LastUpshift => -double.MaxValue.SI<Second>();

		public Second LastDownshift => -double.MaxValue.SI<Second>();

		public GearData GetGearData(uint gear)
		{
			return new GearData() {
				MaxSpeed = null,
			};
		}

		public GearshiftPosition NextGear => throw new NotImplementedException();

		public Second TractionInterruption => 0.SI<Second>();

		public uint NumGears => 1;
		public bool Disengaged => false;

		public bool DisengageGearbox => false;

		public bool GearEngaged(Second absTime)
		{
			return true;
		}

		public bool RequestAfterGearshift { get; set; }

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}
}
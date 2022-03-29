using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class PTODriveAuxiliary
	{
		protected IDataBus DataBus;
		protected IDrivingCycleData Cycle;

		protected Second PTOActivityStart = null;

		protected Meter PTOCycleDistanceStart = null;

		public PTODriveAuxiliary(IVehicleContainer container, IDrivingCycleData cycle)
		{
			DataBus = container;
			Cycle = cycle;
		}

		public bool Active(Second absTime)
		{
			if (PTOActivityStart == null) {
				return false;
			}
			var timeInCycle = absTime - PTOActivityStart;
			return !timeInCycle.IsGreaterOrEqual(Cycle.Entries.Last().Time);
		}

		public Watt PowerDemand(PerSecond nEng, Second absTime, Second dt, bool dryRun)
		{
			if (DataBus.DrivingCycleInfo.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityWhileDrive) {
				if (PTOActivityStart != null && !DataBus.DrivingCycleInfo.CycleData.LeftSample.Distance.IsEqual(PTOCycleDistanceStart)) {
					throw new VectoSimulationException("Additional PTO activation during drive requested while PTO still active!");
				}

				if (PTOActivityStart == null && (PTOCycleDistanceStart == null || !DataBus.DrivingCycleInfo.CycleData.LeftSample.Distance.IsEqual(PTOCycleDistanceStart))) {
					PTOActivityStart = absTime;
					PTOCycleDistanceStart = DataBus.DrivingCycleInfo.CycleData.LeftSample.Distance;
				}
			}

			if (PTOActivityStart == null) {
				return 0.SI<Watt>();
			}

			var timeInCycle = absTime - PTOActivityStart;
			if (timeInCycle.IsGreaterOrEqual(Cycle.Entries.Last().Time)) {
				PTOActivityStart = null;
				return 0.SI<Watt>();
			}

			var sum = 0.SI<WattSecond>();
			foreach (var tuple in Cycle.Entries.Pairwise()) {
				if (timeInCycle > tuple.Item2.Time) {
					continue;
				}
				if (timeInCycle + dt < tuple.Item1.Time) {
					continue;
				}

				var tInt = VectoMath.Min(tuple.Item2.Time, timeInCycle + dt) - VectoMath.Max(tuple.Item1.Time, timeInCycle);
				sum += tuple.Item1.PTOPowerDemandDuringDrive * tInt;
			}

			if (sum.IsGreater(0) && DataBus.VehicleInfo.VehicleStopped) {
				LogManager.GetLogger(typeof(PTODriveAuxiliary).FullName).Warn("PTO still active while vehicle stopped. absTime: {0}", absTime);
			}
			return sum / dt;
		}
	}
}
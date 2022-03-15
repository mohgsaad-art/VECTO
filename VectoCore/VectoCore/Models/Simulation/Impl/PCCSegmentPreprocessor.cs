using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class PCCSegmentPreprocessor : ISimulationPreprocessor
	{
		protected SimplePowertrainContainer Container;
		protected PCCSegments PCCSegments;
		protected DriverData.PCCData PCCDriverData;

		public PCCSegmentPreprocessor(
			SimplePowertrainContainer simpleContainer, PCCSegments segments, DriverData.PCCData driverDataPCC)
		{
			Container = simpleContainer;
			PCCSegments = segments;
			PCCDriverData = driverDataPCC;
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			var slopes = new Dictionary<MeterPerSecond, Radian>();
			new PCCEcoRollEngineStopPreprocessor(Container, slopes, PCCDriverData.MinSpeed,
					VectoMath.Min(Container.VehicleInfo.MaxVehicleSpeed,
					Container.RunData.Cycle.Entries.Max(x => x.VehicleTargetSpeed)))
				.RunPreprocessing();

			var runData = Container.RunData;

#if DEBUG
			Console.WriteLine("Slopes:\n" + string.Join("\n", slopes.Select(p => $"{p.Key.Value()}\t{p.Value.Value()}")));
#endif
			var combustionEngineDrag = runData.EngineData?.FullLoadCurves[0].FullLoadEntries.Average(x =>
											x.EngineSpeed.Value() * x.TorqueDrag.Value()).SI<Watt>()
										?? 0.SI<Watt>();

			var engineDrag = combustionEngineDrag;
			var slopeEngineDrag = 0.0;
			if (runData.GearboxData != null && runData.GearboxData.Type.AutomaticTransmission()) {
				if ((runData.VehicleData.ADAS.EcoRoll != EcoRollType.None && runData.GearboxData.ATEcoRollReleaseLockupClutch) ||
					runData.VehicleData.ADAS.EcoRoll == EcoRollType.None) {
					slopeEngineDrag = (engineDrag / Physics.GravityAccelleration / runData.VehicleData.TotalVehicleMass).Value();
				}
			} else {
				if (runData.VehicleData.ADAS.EcoRoll == EcoRollType.None) {
					slopeEngineDrag = (engineDrag / Physics.GravityAccelleration / runData.VehicleData.TotalVehicleMass).Value();
				}
			}

			PCCSegment pccSegment = null;
			var targetSpeedChanged = 0.SI<Meter>();

			foreach (var pair in Container.RunData.Cycle.Entries.Pairwise()) {
				var start = pair.Item1;
				var end = pair.Item2;
				// pcc is only applicable on highway sections
				if (!start.Highway) {
					pccSegment = null;
					targetSpeedChanged = end.Distance;
					continue;
				}

				// only consider pcc segments where the target speed is at least the pcc-enable speed
				if (start.VehicleTargetSpeed.IsSmaller(PCCDriverData.PCCEnableSpeed)) {
					targetSpeedChanged = end.Distance;
					pccSegment = null;
					continue;
				}

				// can't calculate avg slope if difference between two entries is 0
				if (start.Distance.IsEqual(end.Distance)) {
					continue;
				}

				// target speed must not change within PCC segment
				if (!start.VehicleTargetSpeed.IsEqual(end.VehicleTargetSpeed)) {
					targetSpeedChanged = end.Distance;
					pccSegment = null;
					continue;
				}

				// target speed must not change within PCC segment
				if (pccSegment != null && !pccSegment.TargetSpeed.IsEqual(start.VehicleTargetSpeed)) {
					pccSegment = null;
					continue;
				}
				
				var slope = VectoMath.InclinationToAngle(
					(end.Altitude - start.Altitude) / (end.Distance - start.Distance));

				var minSlope = (slopes.Interpolate(x => x.Key.Value(), y => y.Value.Value(), start.VehicleTargetSpeed.Value())
								+ slopeEngineDrag / start.VehicleTargetSpeed.Value()).SI<Radian>();

				if (pccSegment == null && slope < minSlope) {
					pccSegment = new PCCSegment {
						DistanceMinSpeed = start.Distance,
						StartDistance = start.Distance - VectoMath.Min(
								start.Distance - targetSpeedChanged - 1.SI<Meter>(),
								PCCDriverData.PreviewDistanceUseCase1),
						TargetSpeed = start.VehicleTargetSpeed,
						Altitude = start.Altitude,
						EnergyMinSpeed = (runData.VehicleData.TotalVehicleMass * Physics.GravityAccelleration * start.Altitude)
										.Cast<Joule>() +
										runData.VehicleData.TotalVehicleMass * (start.VehicleTargetSpeed - PCCDriverData.UnderSpeed) *
										(start.VehicleTargetSpeed - PCCDriverData.UnderSpeed) / 2,
					};
				}

				if (pccSegment != null && slope > minSlope) {
					pccSegment.EndDistance = start.Distance;
					pccSegment.EnergyEnd =
						(runData.VehicleData.TotalVehicleMass * Physics.GravityAccelleration * start.Altitude).Cast<Joule>() +
						runData.VehicleData.TotalVehicleMass * start.VehicleTargetSpeed *
						start.VehicleTargetSpeed / 2;
					PCCSegments.Segments.Add(pccSegment);
					pccSegment = null;
				}
			}
		}

		#endregion
	}
}

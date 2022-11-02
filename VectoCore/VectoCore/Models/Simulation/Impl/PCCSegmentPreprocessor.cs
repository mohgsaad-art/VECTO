using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public void RunPreprocessing()
		{
			var slopes = new Dictionary<MeterPerSecond, Radian>();
			var maxOverspeed = VectoMath.Max(Container.RunData.DriverData.OverSpeed.OverSpeed, Container.RunData.DriverData.PCC.OverspeedUseCase3);
			var maxSpeed = VectoMath.Min(Container.VehicleInfo.MaxVehicleSpeed, Container.RunData.Cycle.Entries.Max(x => x.VehicleTargetSpeed)+maxOverspeed);
			
			var preProcessor = new PCCEcoRollEngineStopPreprocessor(Container, slopes, PCCDriverData.MinSpeed, maxSpeed);
			preProcessor.RunPreprocessing();

			DebugWriteLine($"Slopes:\n{slopes.Select(p => $"{p.Key.AsKmph:F}\t{p.Value.ToInclinationPercent():P}").Join("\n")}");

			var runData = Container.RunData;

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

			foreach (var (start, end) in Container.RunData.Cycle.Entries.Pairwise()) {
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

				// target speed must not change within cycle pairs
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
				//DebugWriteLine($"MinSlope:{minSlope.ToInclinationPercent():P}");
				var potentialEnergy = runData.VehicleData.TotalVehicleMass * Physics.GravityAccelleration * start.Altitude;

				if (pccSegment is null && slope < minSlope) {
					var lowestSpeed = start.VehicleTargetSpeed - PCCDriverData.UnderSpeed;
					var lowestKineticEnergy = runData.VehicleData.TotalVehicleMass * lowestSpeed * lowestSpeed / 2;
					pccSegment = new PCCSegment {
						DistanceAtLowestSpeed = start.Distance,
						StartDistance = start.Distance - VectoMath.Min(
								start.Distance - targetSpeedChanged - 1.SI<Meter>(),
								PCCDriverData.PreviewDistanceUseCase1),
						TargetSpeed = start.VehicleTargetSpeed,
						Altitude = start.Altitude,
						EnergyAtLowestSpeed = potentialEnergy + lowestKineticEnergy,
					};
				}

				if (pccSegment != null && slope > minSlope) {
					pccSegment.EndDistance = start.Distance;
					var currentKineticEnergy = runData.VehicleData.TotalVehicleMass
						* start.VehicleTargetSpeed * start.VehicleTargetSpeed / 2;
					pccSegment.EnergyAtEnd = potentialEnergy + currentKineticEnergy;
					PCCSegments.Segments.Add(pccSegment);
					pccSegment = null;
				}
			}
		}
		[Conditional("DEBUG")]
		private static void DebugWriteLine(object value) => Console.WriteLine(value);
	}
}

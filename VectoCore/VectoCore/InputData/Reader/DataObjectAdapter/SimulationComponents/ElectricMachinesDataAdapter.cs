using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public class ElectricMachinesDataAdapter : IElectricMachinesDataAdapter
	{
		public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
			IElectricMachinesDeclarationInputData electricMachines,
			IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage, GearList gearlist = null)
		{
			if (electricMachines == null) {
				return null;
			}

			if (electricMachines.Entries.Select(x => x.Position).Where(x => x != PowertrainPosition.GEN).Distinct().Count() > 1) {
				throw new VectoException("multiple electric propulsion motors are not supported at the moment");
			}

			CheckTorqueLimitVoltageLevels(electricMachines, torqueLimits);

			return electricMachines.Entries
				.Select(m => Tuple.Create(m.Position,
					CreateElectricMachine(
						powertrainPosition: m.Position,
						motorData: m.ElectricMachine,
						count: m.Count, 
						adcRatio: m.RatioADC, 
						ratioPerGear: m.RatioPerGear,
						adcLossMap: m.MechanicalTransmissionLossMap,
						torqueLimits: torqueLimits?.FirstOrDefault(t => t.Key.Position == m.Position).Value, averageVoltage, gearlist))).ToList();

		}

        public virtual Tuple<PowertrainPosition, ElectricMotorData> CreateElectricMachine(
            ElectricMachineEntry<IElectricMotorDeclarationInputData> em,
            IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
            Volt averageVoltage,
            int axleNumber)
		{
            CheckTorqueLimitVoltageLevels(em, torqueLimits, axleNumber);
			
			return Tuple.Create(
				em.Position,
				CreateElectricMachine(
					powertrainPosition: em.Position,
					motorData: em.ElectricMachine,
					count: em.Count,
					adcRatio: em.RatioADC,
					ratioPerGear: em.RatioPerGear,
					adcLossMap: em.MechanicalTransmissionLossMap,
					torqueLimits: torqueLimits?.FirstOrDefault(t => (t.Key.Position == em.Position) && (t.Key.AxleNumber == axleNumber)).Value, averageVoltage, null));
        }

        protected void CheckTorqueLimitVoltageLevels(
			ElectricMachineEntry<IElectricMotorDeclarationInputData> em,
            IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
			int axleNumber)
		{
            if (torqueLimits == null)
            {
                return;
            }

            var torqueLimit = torqueLimits.FirstOrDefault(x => (x.Key.Position == em.Position) && (x.Key.AxleNumber == axleNumber));

            if (torqueLimit.Equals(default(KeyValuePair<EMPlacement, IList<Tuple<Volt, TableData>>>)))
            {
				return;
            }
			
            foreach (var torqueLimitVoltageLevel in torqueLimit.Value.Select(tl => tl.Item1))
            {
                if (em.ElectricMachine.VoltageLevels.All(vl => vl.VoltageLevel != torqueLimitVoltageLevel))
                {
                    throw new VectoException(
                        $"EM Torque Limit: Voltage level {torqueLimitVoltageLevel} not found for EM at position {torqueLimit.Key.Position}" +
						((axleNumber != Constants.NOT_IN_AXLE_POWERTRAIN) ? $", axle {torqueLimit.Key.AxleNumber}" : ""));
                }
            }
        }

        private void CheckTorqueLimitVoltageLevels(IElectricMachinesDeclarationInputData electricMachines,
			IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits)
		{
			if (torqueLimits == null) {
				return;
			}

			foreach (var torqueLimit in torqueLimits.OrderBy(x => x.Key.Position)) {

				//E-machines at position
				foreach (var eMachine in electricMachines.Entries.Where(e => e.Position == torqueLimit.Key.Position).Select(x => x.ElectricMachine)) {
					foreach (var torqueLimitVoltageLevel in torqueLimit.Value.Select(tl => tl.Item1)) {
						if (eMachine.VoltageLevels.All(vl => vl.VoltageLevel != torqueLimitVoltageLevel)) {
							throw new VectoException(
								$"EM Torque Limit: Voltage level {torqueLimitVoltageLevel} not found for EM at position {torqueLimit.Key.Position}");
						}
					}
				}
			}
		}

		private ElectricMotorData CreateElectricMachine(PowertrainPosition powertrainPosition,
			IElectricMotorDeclarationInputData motorData,
			int count,
			double adcRatio,
			double[] ratioPerGear,
			TableData adcLossMap,
			IList<Tuple<Volt, TableData>> torqueLimits,
			Volt averageVoltage,
			GearList gearList)
		{
			if (motorData.CertificationMethod == CertificationMethod.StandardValues) {
				//Fake one very low voltage level and one very high for standard values
				motorData = new StandardValuesInputData.StandardValuesEmInputData(motorData, 1.SI<Volt>(), 10E9.SI<Volt>());
			}
			var voltageLevels = new List<ElectricMotorVoltageLevelData>();

			foreach (var entry in motorData.VoltageLevels.OrderBy(x => x.VoltageLevel)) {
				var fullLoadCurve = ElectricFullLoadCurveReader.Create(entry.FullLoadCurve.First().LoadCurve, count);
				var maxTorqueCurve = torqueLimits == null
					? null
					: ElectricFullLoadCurveReader.Create(
						torqueLimits.FirstOrDefault(x => x.Item1 == null || x.Item1.IsEqual(entry.VoltageLevel))?.Item2, count);

				var fullLoadCurveCombined = IntersectEMFullLoadCurves(fullLoadCurve, maxTorqueCurve);

				try {
					var vLevelData = motorData.IsIHPC()
						? CreateIHPCVoltageLevelData(count, entry, fullLoadCurveCombined, gearList)
						: CreateEmVoltageLevelData(count, entry, fullLoadCurveCombined);
					voltageLevels.Add(vLevelData);
				} catch (Exception ex) {
					throw new VectoException(
						$"Could not create Voltage Level data for {entry.VoltageLevel} at position {powertrainPosition}!\n" +
						$"{ex.Message} {ex.InnerException?.Message?.Substring(0, Math.Min(256, ex.InnerException.Message.Length))}",
						ex);
				}
			}

			if (averageVoltage == null) {
				// if no average voltage is provided (e.g. for supercap) use mean value of measured voltage maps
				averageVoltage = (voltageLevels.Min(x => x.Voltage) + voltageLevels.Max(x => x.Voltage)) / 2.0;
			}

			var lossMap = powertrainPosition == PowertrainPosition.IHPC
				? TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC IHPC LossMap Eff")
				: adcLossMap != null
					? TransmissionLossMapReader.CreateEmADCLossMap(adcLossMap, adcRatio, "EM ADC LossMap", true)
					: TransmissionLossMapReader.CreateEmADCLossMap(DeclarationData.ElectricMachineDefaultMechanicalTransmissionEfficiency, adcRatio, "EM ADC LossMap Eff");


			var retVal = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() { VoltageLevels = voltageLevels },
				EMDragCurve = ElectricMotorDragCurveReader.Create(motorData.DragCurve, count),
				Inertia = motorData.Inertia * count,
				OverloadRecoveryFactor = DeclarationData.OverloadRecoveryFactor,
				RatioADC = adcRatio,
				RatioPerGear = ratioPerGear,
				TransmissionLossMap = lossMap,
			};
			retVal.Overload = CalculateOverloadData(motorData, count, retVal.EfficiencyData, averageVoltage);
			return retVal;
		}

		protected internal static ElectricMotorFullLoadCurve IntersectEMFullLoadCurves(ElectricMotorFullLoadCurve fullLoadCurve,
			ElectricMotorFullLoadCurve maxTorqueCurve)
		{
			if (maxTorqueCurve == null) {
				return fullLoadCurve;
			}

			if (maxTorqueCurve.MaxSpeed.IsSmaller(fullLoadCurve.MaxSpeed)) {
				throw new VectoException("EM Torque limitation has to cover the whole speed range");
			}

			var motorSpeeds = fullLoadCurve.FullLoadEntries.Select(x => x.MotorSpeed)
				.Concat(maxTorqueCurve.FullLoadEntries.Select(x => x.MotorSpeed)).ToList();
			// iterate over all segments in the full-load curve
			foreach (var fldTuple in fullLoadCurve.FullLoadEntries.Pairwise()) {
				// find all grid points of max torque curve within the current segment of fld
				var maxPtsWithinSegment = maxTorqueCurve.FullLoadEntries.Where(x =>
					x.MotorSpeed.IsGreaterOrEqual(fldTuple.Item1.MotorSpeed) &&
					x.MotorSpeed.IsSmallerOrEqual(fldTuple.Item2.MotorSpeed)).OrderBy(x => x.MotorSpeed).ToList();
				if (maxPtsWithinSegment.Count == 0) {
					// if grid pint is within, take the 'surrounding' segment
					var segment =
						maxTorqueCurve.FullLoadEntries.GetSection(x => x.MotorSpeed < fldTuple.Item1.MotorSpeed);
					maxPtsWithinSegment = new[] { segment.Item1, segment.Item2 }.ToList();
				} else {
					// add the point just before and just after the current list of points 
					if (maxPtsWithinSegment.Min(x => x.MotorSpeed).IsGreater(fldTuple.Item1.MotorSpeed)) {
						maxPtsWithinSegment.Add(maxTorqueCurve.FullLoadEntries.Last(x =>
							x.MotorSpeed.IsSmaller(fldTuple.Item1.MotorSpeed)));
					}

					if (maxPtsWithinSegment.Max(x => x.MotorSpeed).IsSmaller(fldTuple.Item2.MotorSpeed)) {
						maxPtsWithinSegment.Add(maxTorqueCurve.FullLoadEntries.First(x => x.MotorSpeed.IsGreater(fldTuple.Item2.MotorSpeed)));
					}

					maxPtsWithinSegment = maxPtsWithinSegment.OrderBy(x => x.MotorSpeed).ToList();
				}

				var fldEdgeDrive =
					Edge.Create(new Point(fldTuple.Item1.MotorSpeed.Value(), fldTuple.Item1.FullDriveTorque.Value()),
						new Point(fldTuple.Item2.MotorSpeed.Value(), fldTuple.Item2.FullDriveTorque.Value()));
				var fldEdgeGenerate =
					Edge.Create(new Point(fldTuple.Item1.MotorSpeed.Value(), fldTuple.Item1.FullGenerationTorque.Value()),
						new Point(fldTuple.Item2.MotorSpeed.Value(), fldTuple.Item2.FullGenerationTorque.Value()));
				foreach (var maxTuple in maxPtsWithinSegment.Pairwise()) {
					var maxEdgeDrive =
						Edge.Create(new Point(maxTuple.Item1.MotorSpeed.Value(), maxTuple.Item1.FullDriveTorque.Value()),
							new Point(maxTuple.Item2.MotorSpeed.Value(), maxTuple.Item2.FullDriveTorque.Value()));
					if (!(maxEdgeDrive.SlopeXY - fldEdgeDrive.SlopeXY).IsEqual(0, 1e-12)) {
						// lines are not parallel
						var nIntersectDrive =
							((fldEdgeDrive.OffsetXY - maxEdgeDrive.OffsetXY) /
							(maxEdgeDrive.SlopeXY - fldEdgeDrive.SlopeXY)).SI<PerSecond>();
						if (nIntersectDrive.IsBetween(fldTuple.Item1.MotorSpeed, fldTuple.Item2.MotorSpeed)) {
							motorSpeeds.Add(nIntersectDrive);
						}
					}

					var maxEdgeGenerate =
						Edge.Create(new Point(maxTuple.Item1.MotorSpeed.Value(), maxTuple.Item1.FullGenerationTorque.Value()),
							new Point(maxTuple.Item2.MotorSpeed.Value(), maxTuple.Item2.FullGenerationTorque.Value()));
					if (!((maxEdgeGenerate.SlopeXY - fldEdgeGenerate.SlopeXY).IsEqual(0, 1e-12))) {
						// lines are not parallel
						var nIntersectGenerate =
							((fldEdgeGenerate.OffsetXY - maxEdgeGenerate.OffsetXY) /
							(maxEdgeGenerate.SlopeXY - fldEdgeGenerate.SlopeXY)).SI<PerSecond>();


						if (nIntersectGenerate.IsBetween(fldTuple.Item1.MotorSpeed, fldTuple.Item2.MotorSpeed)) {
							motorSpeeds.Add(nIntersectGenerate);
						}
					}
				}
			}

			// create new full-load curve with values closest to zero.
			return new ElectricMotorFullLoadCurve(motorSpeeds.OrderBy(x => x.Value()).Distinct().Select(x =>
				new ElectricMotorFullLoadCurve.FullLoadEntry() {
					MotorSpeed = x,
					FullDriveTorque = VectoMath.Max(fullLoadCurve.FullLoadDriveTorque(x),
						maxTorqueCurve.FullLoadDriveTorque(x)),
					FullGenerationTorque = VectoMath.Min(fullLoadCurve.FullGenerationTorque(x),
						maxTorqueCurve.FullGenerationTorque(x)),
				}).ToList());
		}

		private ElectricMotorVoltageLevelData CreateIHPCVoltageLevelData(int count, IElectricMotorVoltageLevel entry, ElectricMotorFullLoadCurve fullLoadCurveCombined, GearList gearList)
		{
			if (gearList == null) {
				throw new VectoException("no gears provided for IHPC EM");
			}

			if (gearList.Count() != entry.PowerMap.Count) {
				throw new VectoException(
					$"number of gears in transmission does not match gears in electric motor (IHPC) - {gearList.Count()}/{entry.PowerMap.Count}");
			}
			var effMap = new Dictionary<uint, EfficiencyMap>();
			foreach (var gear in gearList) {
				effMap.Add(gear.Gear, ElectricMotorMapReader.Create(entry.PowerMap[(int)gear.Gear - 1].PowerMap, count, ExecutionMode.Declaration));
			}
			return new IHPCVoltageLevelData() {
				Voltage = entry.VoltageLevel,
				FullLoadCurve = fullLoadCurveCombined,
				EfficiencyMaps = effMap,
			};
		}

		private static ElectricMotorVoltageLevelData CreateEmVoltageLevelData(int count,
			IElectricMotorVoltageLevel entry, ElectricMotorFullLoadCurve fullLoadCurveCombined)
		{
			try {
				return new ElectricMotorVoltageLevelData() {
					Voltage = entry.VoltageLevel,

					FullLoadCurve = fullLoadCurveCombined,
					// DragCurve = ElectricMotorDragCurveReader.Create(entry.DragCurve, count),
					EfficiencyMap = ElectricMotorMapReader.Create(entry.PowerMap.First().PowerMap, count, ExecutionMode.Declaration), //PowerMap
				};
			} catch (Exception ex) {
				throw new VectoException($"Invalid efficiency map at voltage level {entry.VoltageLevel}", ex);
			}
		}


		public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
		{
			if (iepc == null) {
				return null;
			}
			if (iepc.CertificationMethod == CertificationMethod.StandardValues) {
				//Fake one very low voltage level and one very high for standard values
				iepc = new StandardValuesInputData.StandardValueIEPCInputData(iepc, 1.SI<Volt>(), 10E9.SI<Volt>());
			}

			var pos = PowertrainPosition.IEPC;
			var count = iepc.DesignTypeWheelMotor && iepc.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;

			// the full-load curve is measured in the gear with the ratio closest to 1,
			// in case two gears have the same difference, the higher one is used
			var gearRatioUsedForMeasurement = iepc.Gears
				.Select(x => new { x.GearNumber, x.Ratio, Diff = Math.Round(Math.Abs(x.Ratio - 1), 6) }).GroupBy(x => x.Diff)
				.OrderBy(x => x.Key).First().OrderBy(x => x.Ratio).Reverse().First();

			var voltageLevels = new List<ElectricMotorVoltageLevelData>();

			foreach (var entry in iepc.VoltageLevels.OrderBy(x => x.VoltageLevel).AsEnumerable()) {
				var effMap = new Dictionary<uint, EfficiencyMap>();

                var fldCurve = (entry.FullLoadCurve.Count() == 1) && (entry.FullLoadCurve.First().Gear == 0)
                    ? IEPCFullLoadCurveReader.Create(entry.FullLoadCurve.First().LoadCurve, count, gearRatioUsedForMeasurement.Ratio)
					: null;

				var fldCurves = new Dictionary<uint, ElectricMotorFullLoadCurve>();
				foreach (var curve in entry.FullLoadCurve.Where(x => x.Gear > 0))
				{
                    var ratio = iepc.Gears.First(x => x.GearNumber == curve.Gear).Ratio;
					fldCurves.Add((uint)curve.Gear, IEPCFullLoadCurveReader.Create(curve.LoadCurve, count, ratio));
				}
					
                for (var i = 0u; i < entry.PowerMap.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					effMap.Add(i + 1, IEPCMapReader.Create(entry.PowerMap[(int)i].PowerMap, count, ratio, fldCurve ?? fldCurves[i + 1], ExecutionMode.Declaration));
				}

				voltageLevels.Add(new IEPCVoltageLevelData() {

					Voltage = iepc.CertificationMethod != CertificationMethod.StandardValues ? entry.VoltageLevel : null, //No voltagelevel is provided for standard values
					FullLoadCurve = fldCurve,
					FullLoadCurves = fldCurves,
					EfficiencyMaps = effMap,
				});
			}

			voltageLevels.First().Voltage = voltageLevels.First().Voltage ?? 1.SI<Volt>();
			voltageLevels.Last().Voltage = voltageLevels.Last().Voltage ?? 10E9.SI<Volt>();


			var dragCurves = new Dictionary<uint, DragCurve>();
			if (iepc.DragCurves.Count > 1) {
				for (var i = 0u; i < iepc.DragCurves.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(iepc.DragCurves[(int)i].DragCurve, count, ratio));
				}
			} else {
				var dragCurve = iepc.DragCurves.First().DragCurve;
				for (var i = 0u; i < iepc.Gears.Count; i++) {
					var ratio = iepc.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(dragCurve, count, ratio));
				}
			}

			var retVal = new IEPCElectricMotorData() {
				EfficiencyData = new VoltageLevelData() { VoltageLevels = voltageLevels },
				IEPCDragCurves = dragCurves,
				Inertia = iepc.Inertia * count,
				OverloadRecoveryFactor = DeclarationData.OverloadRecoveryFactor,
				RatioADC = 1,
				RatioPerGear = null,
				TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC LossMap Eff"),

			};
			retVal.Overload = CalculateOverloadData(iepc, count, retVal.EfficiencyData, averageVoltage,
				Tuple.Create((uint)gearRatioUsedForMeasurement.GearNumber, gearRatioUsedForMeasurement.Ratio));
			;
			return new List<Tuple<PowertrainPosition, ElectricMotorData>>() { Tuple.Create<PowertrainPosition, ElectricMotorData>(pos, retVal) };
		}

		protected OverloadData CalculateOverloadData(IIEPCDeclarationInputData iepc, int count,
			VoltageLevelData voltageLevel, Volt averageVoltage, Tuple<uint, double> gearRatioUsedForMeasurement)
		{

			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < iepc.VoltageLevels.Min(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(iepc.VoltageLevels.First(), count, voltageLevel, gearRatioUsedForMeasurement);
			}
			if (averageVoltage > iepc.VoltageLevels.Max(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(iepc.VoltageLevels.Last(), count, voltageLevel, gearRatioUsedForMeasurement);
			}

			var (vLow, vHigh) = iepc.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevel, gearRatioUsedForMeasurement);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevel, gearRatioUsedForMeasurement);

			var retVal = new OverloadData() {
				OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
				ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
                ContinuousTorqueGen = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorqueGen, ovlHi.ContinuousTorqueGen, averageVoltage),
                ContinuousPower = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPower, ovlHi.ContinuousPower, averageVoltage),
                ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
			};
			return retVal;
		}

		protected OverloadData CalculateOverloadData(IElectricMotorDeclarationInputData motorData, int count,
			VoltageLevelData voltageLevel, Volt averageVoltage)
		{
			var vMin = motorData.VoltageLevels.MinBy(x => x.VoltageLevel);
			var vMax = motorData.VoltageLevels.MaxBy(x => x.VoltageLevel);

			var ratioContTqMin = vMin.ContinuousTorque / vMin.OverloadTorque;
			var ratioContTqMax = vMax.ContinuousTorque / vMax.OverloadTorque;
			bool lowContinuousTorque = ratioContTqMin < 0.1 && ratioContTqMax < 0.1;

			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < motorData.VoltageLevels.Min(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(motorData.VoltageLevels.First(), count, voltageLevel);
			}
			if (averageVoltage > motorData.VoltageLevels.Max(x => x.VoltageLevel)) {
				return CalculateOverloadBuffer(motorData.VoltageLevels.Last(), count, voltageLevel);
			}

			var (vLow, vHigh) = motorData.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevel);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevel);

			var continuousPowerLoss = lowContinuousTorque
				? VectoMath.Interpolate(vLow.VoltageLevel,
					vHigh.VoltageLevel, CalculatePowerLossLowContinuousTorque(vLow, count, voltageLevel),
					CalculatePowerLossLowContinuousTorque(vHigh, count, voltageLevel), averageVoltage)
				: VectoMath.Interpolate(vLow.VoltageLevel,
					vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage);

            var retVal = new OverloadData() {
				OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
				ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
                ContinuousTorqueGen = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorqueGen, ovlHi.ContinuousTorqueGen, averageVoltage),
                ContinuousPower = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPower, ovlHi.ContinuousPower, averageVoltage),
                ContinuousPowerLoss = continuousPowerLoss
			};
			return retVal;
		}

		private Watt CalculatePowerLossLowContinuousTorque(IElectricMotorVoltageLevel voltageEntry, int count, VoltageLevelData voltageLevel)
		{
			var estimatedContTq = voltageEntry.OverloadTorque * count * 0.5;
			var extimatedContTqSpeed = voltageEntry.OverloadTestSpeed;
			var gear = new GearshiftPosition(0);
			var contElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, extimatedContTqSpeed,
								-estimatedContTq, gear).ElectricalPower ??
							voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, extimatedContTqSpeed,
								voltageLevel.FullLoadDriveTorque(voltageEntry.VoltageLevel, extimatedContTqSpeed, gear.Gear),
								gear, true).ElectricalPower;

            var continuousPowerLoss = -contElPwr - estimatedContTq * extimatedContTqSpeed; // loss needs to be positive
			return continuousPowerLoss;
        }

		protected OverloadData CalculateOverloadBuffer(IElectricMotorVoltageLevel voltageEntry,
			int count, VoltageLevelData voltageLevel, Tuple<uint, double> gearUsedForMeasurement = null)
		{
			var ovl1 = CalculateOverloadBufferDirect(voltageEntry, count, voltageLevel, gearUsedForMeasurement);
			var ovl2 = CalculateOverloadBufferTransf(voltageEntry, count,  voltageLevel, gearUsedForMeasurement);

			if (ovl2.OverloadBuffer.IsGreater(ovl1.OverloadBuffer)) {
				return ovl2;
			}

			return ovl1;
		}

		protected OverloadData CalculateOverloadBufferDirect(IElectricMotorVoltageLevel voltageEntry,
			int count, VoltageLevelData voltageLevel, Tuple<uint, double> gearUsedForMeasurement = null)
		{
			var gearRatioUsedForMeasurement = gearUsedForMeasurement?.Item2 ?? 1.0;
			var gear = new GearshiftPosition(gearUsedForMeasurement?.Item1 ?? 1);
			var continuousTorque = voltageEntry.ContinuousTorque * count / gearRatioUsedForMeasurement;
			var continuousTorqueSpeed = voltageEntry.ContinuousTorqueSpeed * gearRatioUsedForMeasurement;
			var overloadTorque = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count / gearRatioUsedForMeasurement;
			var overloadTestSpeed = (voltageEntry.OverloadTestSpeed ?? 0.RPMtoRad()) * gearRatioUsedForMeasurement;

			//ElectricMotorRatedSpeedHelper.GetRatedSpeed(voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque)
			var voltageLevelData = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First();
            var FullLoadCurve = (voltageLevelData is IEPCVoltageLevelData iepc) && (iepc.FullLoadCurves.Count() > 0)
				? iepc.FullLoadCurves[gear.Gear]
				: voltageLevelData.FullLoadCurve;

            // FullLoadCurve express torque and speed at the rotor level, not the output shaft
            
            //var FullLoadEntries = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries; 
            var continuousPower = continuousTorque * continuousTorqueSpeed;
			
            var continuousTorqueSpeedRef = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque)); 
            var continuousTorqueSpeedRefGen = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullGenerationTorque));

            var continuousTorqueRef = VectoMath.Min(continuousPower / continuousTorqueSpeedRef, -FullLoadCurve.MaxDriveTorque);
			var continuousTorqueRefGen = VectoMath.Min(continuousPower / continuousTorqueSpeedRefGen, FullLoadCurve.MaxGenerationTorque);

            var peakElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
					overloadTestSpeed,
					-overloadTorque,
					gear,
					true)
					.ElectricalPower;
			var peakPwrLoss = -peakElPwr - overloadTorque * overloadTestSpeed; // losses need to be positive

			var contElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
								-continuousTorque, gear).ElectricalPower ??
							voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
								voltageLevel.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed, gear.Gear),
								gear, true).ElectricalPower;
			var continuousPowerLoss = -contElPwr - continuousTorque * continuousTorqueSpeed; // loss needs to be positive
			var overloadBuffer = VectoMath.Max(peakPwrLoss - continuousPowerLoss, continuousPowerLoss * 0.0001) * voltageEntry.OverloadTime;
			return new OverloadData() {
				OverloadBuffer = overloadBuffer,
				ContinuousTorque = continuousTorqueRef,
                ContinuousTorqueGen = continuousTorqueRefGen,
                ContinuousPower = continuousPower,
                ContinuousPowerLoss = continuousPowerLoss
			};
		}

		protected OverloadData CalculateOverloadBufferTransf(IElectricMotorVoltageLevel voltageEntry,
			int count, VoltageLevelData voltageLevel, Tuple<uint, double> gearUsedForMeasurement = null)
		{

            /* Previous version computation:
             * To compute derated max torque and buffer by comparing 2 transfered operating points (OP) based on:
			 *   - the overload OP
			 *   - the continuous OP 
			 * Both points were "transfered" to the lowest speed of both, assuming an iso-power condition (Ttrans = Wop*Top/Wtrans, note that it means one of the two OP is unchanged)
			 * The max torque in derated mode was then the torque of the transfered continuous OP (ensuring choosing the max possible torque at Pow=Pow,continuous)
			 * The buffer was expressed as (Ploss,overload,trans - Ploss,continuous,trans) * OverloadTime assuming a constant efficiency eta (computed on OVL OP)
			 * Expression can be simplified to (Pmech,overload,trans - Pmech,continuous,trans) * (1/eta - 1) * OverloadTime
			 * OP is in most common scenario defined as declared speed and torque values for continuous OP, and measured speed and torque values for overload OP
			 * If Manufacturer the same OP as overload and continuous OP, this means we can have Pmech,overload,trans<0.98*Pmech,continuous,trans, and buffer size < 0,
			 * which is not expected.
			 * 
			 * New computation: Thermal derating is now made by limiting the mechanical power to the continuous OP power above rated speed, and Torque=(Pmech,continuous)/Wrated below
			 * Most of the function was not relevant anymore for the definition of torque, except for the definition of the alternative thermal buffer, for which an alternative definition
			 * is proposed, based on a condition comparing two OP:
			 *   - the continuous OP 
			 *   - the max torque OP (interpolated from maximum torque curve), which, according to regulation, shall be reached for at least 3 seconds
			 * in that case, the buffer becomes buffer = (Ploss,max - Ploss,continuous) * 3seconds
			 * By definition of those points, which are both based on declared Pmax - Pcontinous is always positive, avoiding the case of negative buffer as long as the losses respect the same constraint
			 * A safety alternative formula is computed based on a constant efficiency assumption :  buffer_eta = (Pmech,max - Pmech,continuous) * (1/eta - 1) * 3seconds
			 * If the buffer first computation gives negative value, the alternative formula is used instead, saturated to a minimum of 0.01% * Ploss,continuous * 3seconds (unsignificant non-zero buffer, but functional).
			 */

            var gearRatioUsedForMeasurement = gearUsedForMeasurement?.Item2 ?? 1.0;
			var gear = new GearshiftPosition(gearUsedForMeasurement?.Item1 ?? 1);
			var continuousTorqueSpeed = voltageEntry.ContinuousTorqueSpeed * gearRatioUsedForMeasurement;
			//var overloadTorque = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count / gearRatioUsedForMeasurement;
			var overloadTestSpeed = (voltageEntry.OverloadTestSpeed ?? 0.RPMtoRad()) * gearRatioUsedForMeasurement;

            var voltageLevelData = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First();
            var FullLoadCurve = (voltageLevelData is IEPCVoltageLevelData iepc) && (iepc.FullLoadCurves.Count() > 0)
                ? iepc.FullLoadCurves[gear.Gear]
                : voltageLevelData.FullLoadCurve;

            var continuousTorqueSpeedRef = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque));
            var continuousTorqueSpeedRefGen = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullGenerationTorque));


            if (overloadTestSpeed.IsEqual(0)) {
				throw new VectoException("Invalid model parameters for EM overload");
			}

            var overloadPwr = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count *
                                (voltageEntry.OverloadTestSpeed ?? 0.RPMtoRad());
            var continuousPwr = (voltageEntry.ContinuousTorque ?? 0.SI<NewtonMeter>()) * count *
                                    (voltageEntry.ContinuousTorqueSpeed ?? 0.RPMtoRad());

            var continuousTorque = voltageEntry.ContinuousTorque * count / gearRatioUsedForMeasurement;
            var overloadTorque = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count / gearRatioUsedForMeasurement;

            var maxTorqueDrvFldContSpeed =
                    -voltageLevel.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed, gear.Gear);

            var maxTorqueGenFldContSpeed =
                    voltageLevel.FullGenerationTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed, gear.Gear);

            var maxTorqueFldContSpeed = VectoMath.Max(maxTorqueGenFldContSpeed, maxTorqueDrvFldContSpeed);

            // define the OVL point for reference efficiency: at continuous speed, in driving conditions

            var overloadTorqueContSpeed = VectoMath.Min(overloadPwr / continuousTorqueSpeed, maxTorqueDrvFldContSpeed);

            var ovlElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
				-overloadTorqueContSpeed, gear, true).ElectricalPower;

            if (ovlElPwr == null)
            {
                throw new VectoException(
                    $"Overloadbuffer calculation: failed to lookup electric power for overload point {continuousTorqueSpeed.AsRPM} [rpm] {overloadTorqueContSpeed / count}");
            }

            var etaOvl = continuousTorqueSpeed * overloadTorqueContSpeed / -ovlElPwr;

            // continuous powerloss (standard and at etaOVL)

            var contElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                -continuousTorque, gear).ElectricalPower ??
                            voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                voltageLevel.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed, gear.Gear),
                                gear, true).ElectricalPower;
            var continuousPowerLoss = -contElPwr - continuousTorque * continuousTorqueSpeed; // loss needs to be positive

            var continuousPowerLossEtaOVL = (1 / etaOvl.Value() - 1) * continuousTorqueSpeed * continuousTorque; // losses need to be positive

            // peak losses considering maxTorque instead of OVL torque, but with a time of 3 seconds constraint instead of Tovl used in previous version

            var peakElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                    continuousTorqueSpeed,
                    -maxTorqueDrvFldContSpeed,
                    gear,
                    true)
                    .ElectricalPower;
            var peakPwrLoss = -peakElPwr - continuousTorqueSpeed * maxTorqueDrvFldContSpeed; // losses need to be positive
            var peakPwrLossEtaOVL = (1 / etaOvl.Value() - 1) * continuousTorqueSpeed * maxTorqueDrvFldContSpeed; // losses need to be positive

            // For case when Gen max Torque is higher than Drive Torque, The Buffer might need to be increased
			// Use Gen torque, to ensure reaching it during 3 seconds
            var peakElPwrGen = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                continuousTorqueSpeed,
                maxTorqueGenFldContSpeed,
                gear,
                true)
                .ElectricalPower;
            var peakPwrLossGen = continuousTorqueSpeed * maxTorqueGenFldContSpeed - peakElPwrGen; // losses need to be positive
            var peakPwrLossGenEtaOVL = (1 - etaOvl.Value()) * continuousTorqueSpeed * maxTorqueGenFldContSpeed; // losses need to be positive

            // additional saturation taking into account all the operating points of the FullLoadCurve

			var peakElPwr_i = peakElPwr * 0.0;
			var peakPwrLossGen_i = peakElPwr * 0.0;
			var peakPwrLoss_i = peakElPwr * 0.0;

            foreach (var OP in FullLoadCurve.FullLoadEntries)
            {
				if (OP.MotorSpeed.IsGreater(0.0)) {
                    peakElPwr_i = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                    OP.MotorSpeed,
                    OP.FullDriveTorque, // OP.FullGenerationTorque
                    gear,
                    true)
                    .ElectricalPower;

                    peakPwrLoss_i = OP.MotorSpeed * OP.FullDriveTorque - peakElPwr_i; // operation reversed to fit sign rule

                    peakElPwr_i = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                        OP.MotorSpeed,
                        OP.FullGenerationTorque, // OP.FullGenerationTorque
                        gear,
                        true)
                        .ElectricalPower;

                    peakPwrLossGen_i = OP.MotorSpeed * OP.FullGenerationTorque - peakElPwr_i; // operation reversed to fit sign rule

                    peakPwrLoss = VectoMath.Max(peakPwrLoss, peakPwrLoss_i);
                    peakPwrLossGen = VectoMath.Max(peakPwrLossGen, peakPwrLossGen_i);
                }
                
            }

			// BUFFER
			// buffer is the difference of power losses between the OVL point (a Max Torque point for 3s) and the CONT point,
			// two computation methods:
			// - using losses computation on maps
			// - iso efficiency assumption for both leads to a difference of mechanical power in expression, which has to be positive

			var dPloss = VectoMath.Max(peakPwrLoss, peakPwrLossGen) - continuousPowerLoss;
			var dPlossEtaOVL = VectoMath.Max(peakPwrLossEtaOVL, peakPwrLossGenEtaOVL) - continuousPowerLossEtaOVL;

            var OverloadTime = 3.SI<Second>();

            var overloadBuffer = dPloss * OverloadTime;

            if (overloadBuffer.IsSmaller(0.0)) {
                overloadBuffer = VectoMath.Max(dPlossEtaOVL, continuousPowerLoss * 0.0001) * OverloadTime;
            }

            // definition of continuousPower and continuousTorqueRef@ratedSpeed limits in derated mode

            var continuousPower = continuousTorque * continuousTorqueSpeed;
            var continuousTorqueRef = VectoMath.Min(continuousPower / continuousTorqueSpeedRef, -FullLoadCurve.MaxDriveTorque);
            var continuousTorqueRefGen = VectoMath.Min(continuousPower / continuousTorqueSpeedRefGen, FullLoadCurve.MaxGenerationTorque);

			
            return new OverloadData() {
				OverloadBuffer = overloadBuffer,
				ContinuousTorque = continuousTorqueRef,
                ContinuousTorqueGen = continuousTorqueRefGen,
                ContinuousPower = continuousPower,
                ContinuousPowerLoss = continuousPowerLoss
			};
		}
	}

	

	public class GenericElectricMachinesDataAdapter : ElectricMachinesDataAdapter
	{
		private readonly GenericBusElectricMotorData _genericEMotorData = new GenericBusElectricMotorData();
		private GenericBusIEPCData _genericIepcData = new GenericBusIEPCData();

		#region Implementation of IElectricMachinesDataAdapter

		public override IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
			Volt averageVoltage, GearList gearlist = null)
		{
			if (electricMachines == null) {
				return null;
			}

			if (electricMachines.Entries.Select(x => x.Position)
					.Where(x => x != PowertrainPosition.GEN).Distinct().Count() > 1) {
				throw new VectoException("multiple electric propulsion motors are not supported at the moment");
			}

			CheckTorqueLimitVoltageLevels(electricMachines, torqueLimits);

			return electricMachines.Entries.Select(m =>
				Tuple.Create(m.Position, _genericEMotorData.CreateGenericElectricMotorData(m, torqueLimits?.FirstOrDefault(t => t.Key.Position == m.Position).Value,
					averageVoltage))).ToList();

		}

		public override Tuple<PowertrainPosition, ElectricMotorData> CreateElectricMachine(
			ElectricMachineEntry<IElectricMotorDeclarationInputData> em, 
			IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
            Volt averageVoltage,
			int axleNumber)
		{
			if (em == null) 
			{ 
				return null; 
			}

			CheckTorqueLimitVoltageLevels(em, torqueLimits, axleNumber);

            return Tuple.Create(
				em.Position, 
				_genericEMotorData.CreateGenericElectricMotorData(
					em, 
					torqueLimits?.FirstOrDefault(t => (t.Key.Position == em.Position) && (t.Key.AxleNumber == axleNumber)).Value,
                averageVoltage));
		}

        public override List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
		{
			if (iepc == null) {
				return null;
			}
			if (iepc.CertificationMethod == CertificationMethod.StandardValues) {
				//Fake one very low voltage level and one very high for standard values
				iepc = new StandardValuesInputData.StandardValueIEPCInputData(iepc, 1.SI<Volt>(), 10E9.SI<Volt>());
			}

            var pos = PowertrainPosition.IEPC;
			var count = iepc.DesignTypeWheelMotor && iepc.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;
            var gearRatioUsedForMeasurement = iepc.Gears
				.Select(x => new { x.GearNumber, x.Ratio, Diff = Math.Round(Math.Abs(x.Ratio - 1), 6) }).GroupBy(x => x.Diff)
				.OrderBy(x => x.Key).First().OrderBy(x => x.Ratio).Reverse().First();
			//var voltageLevels = new List<ElectricMotorVoltageLevelData>();
			var genericIEPCData = _genericIepcData.CreateIEPCElectricMotorData(iepc);
			genericIEPCData.OverloadRecoveryFactor = DeclarationData.OverloadRecoveryFactor;
			genericIEPCData.TransmissionLossMap =
				TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC LossMap Eff");
			genericIEPCData.RatioADC = 1;
			genericIEPCData.Overload = CalculateOverloadData(iepc, count, genericIEPCData.EfficiencyData, averageVoltage,
				Tuple.Create((uint)gearRatioUsedForMeasurement.GearNumber, gearRatioUsedForMeasurement.Ratio));


            return new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
				Tuple.Create<PowertrainPosition, ElectricMotorData>(pos, genericIEPCData)
			};



        }

        #endregion

        private void CheckTorqueLimitVoltageLevels(IElectricMachinesDeclarationInputData electricMachines,
			IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits)
		{
			if (torqueLimits == null) {
				return;
			}

			foreach (var torqueLimit in torqueLimits.OrderBy(x => x.Key.Position)) {

				//E-machines at position
				foreach (var eMachine in electricMachines.Entries.Where(e => e.Position == torqueLimit.Key.Position).Select(x => x.ElectricMachine)) {
					foreach (var torqueLimitVoltageLevel in torqueLimit.Value.Select(tl => tl.Item1)) {
						if (eMachine.VoltageLevels.All(vl => vl.VoltageLevel != torqueLimitVoltageLevel)) {
							throw new VectoException(
								$"EM Torque Limit: Voltage level {torqueLimitVoltageLevel} not found for EM at position {torqueLimit.Key.Position}");
						}
					}
				}
			}
		}
    }

    internal abstract class StandardValuesInputData
    {
        /// <summary>
        /// Wraps the standard value input data and overrides the voltage level
        /// </summary>
        internal class StandardValueVoltageLevelInputData : IElectricMotorVoltageLevel
        {

            public StandardValueVoltageLevelInputData(IElectricMotorVoltageLevel inputData, Volt voltageLevel)
            {
                _electricMotorVoltageLevelImplementation = inputData;
                _voltageLevel = voltageLevel;
            }
            private IElectricMotorVoltageLevel _electricMotorVoltageLevelImplementation;
            private Volt _voltageLevel;

            public Volt VoltageLevel => _voltageLevel;


            #region Implementation of IElectricMotorVoltageLevel



            public NewtonMeter ContinuousTorque => _electricMotorVoltageLevelImplementation.ContinuousTorque;

            public PerSecond ContinuousTorqueSpeed => _electricMotorVoltageLevelImplementation.ContinuousTorqueSpeed;

            public NewtonMeter OverloadTorque => _electricMotorVoltageLevelImplementation.OverloadTorque;

            public PerSecond OverloadTestSpeed => _electricMotorVoltageLevelImplementation.OverloadTestSpeed;

            public Second OverloadTime => _electricMotorVoltageLevelImplementation.OverloadTime;

            public IList<IElectricMotorLoadCurve> FullLoadCurve => _electricMotorVoltageLevelImplementation.FullLoadCurve;

            public IList<IElectricMotorPowerMap> PowerMap => _electricMotorVoltageLevelImplementation.PowerMap;

            #endregion
        }

        internal class StandardValueIEPCInputData : IIEPCDeclarationInputData
        {
            private IIEPCDeclarationInputData _iiepcDeclarationInputDataImplementation;
            private IList<IElectricMotorVoltageLevel> _voltageLevels = new List<IElectricMotorVoltageLevel>();

            /// <summary>
            /// Wraps the inputData and creates new voltage levels based on the voltage level provided in the input data
            /// </summary>
            /// <param name="inputData"></param>
            /// <param name="voltageLevels"></param>
            /// <exception cref="ArgumentException"></exception>
            public StandardValueIEPCInputData(IIEPCDeclarationInputData inputData, params Volt[] voltageLevels)
            {
                _iiepcDeclarationInputDataImplementation = inputData;
                if (inputData.CertificationMethod != CertificationMethod.StandardValues)
                {
                    throw new ArgumentException("Only for standard value certification");
                }

                foreach (var voltageLevel in voltageLevels)
                {
                    _voltageLevels.Add(new StandardValueVoltageLevelInputData(inputData.VoltageLevels.First(), voltageLevel));
                }

            }

            public IList<IElectricMotorVoltageLevel> VoltageLevels => _voltageLevels;

            #region Implementation of IComponentInputData

            public DataSource DataSource => _iiepcDeclarationInputDataImplementation.DataSource;

            public bool SavedInDeclarationMode => _iiepcDeclarationInputDataImplementation.SavedInDeclarationMode;

            public string Manufacturer => _iiepcDeclarationInputDataImplementation.Manufacturer;

            public string Model => _iiepcDeclarationInputDataImplementation.Model;

            public DateTime Date => _iiepcDeclarationInputDataImplementation.Date;

            public string AppVersion => _iiepcDeclarationInputDataImplementation.AppVersion;

            public CertificationMethod CertificationMethod => _iiepcDeclarationInputDataImplementation.CertificationMethod;

            public string CertificationNumber => _iiepcDeclarationInputDataImplementation.CertificationNumber;

            public DigestData DigestValue => _iiepcDeclarationInputDataImplementation.DigestValue;

            #endregion

            #region Implementation of IIEPCDeclarationInputData

            public ElectricMachineType ElectricMachineType => _iiepcDeclarationInputDataImplementation.ElectricMachineType;

            public Watt R85RatedPower => _iiepcDeclarationInputDataImplementation.R85RatedPower;

			public Watt TotalRatedPowerCalculated => _iiepcDeclarationInputDataImplementation.TotalRatedPowerCalculated;

			public KilogramSquareMeter Inertia => _iiepcDeclarationInputDataImplementation.Inertia;

            public bool DifferentialIncluded => _iiepcDeclarationInputDataImplementation.DifferentialIncluded;

            public bool DesignTypeWheelMotor => _iiepcDeclarationInputDataImplementation.DesignTypeWheelMotor;

            public int? NrOfDesignTypeWheelMotorMeasured => _iiepcDeclarationInputDataImplementation.NrOfDesignTypeWheelMotorMeasured;

            public IList<IGearEntry> Gears => _iiepcDeclarationInputDataImplementation.Gears;



            public IList<IDragCurve> DragCurves => _iiepcDeclarationInputDataImplementation.DragCurves;

            public TableData Conditioning => _iiepcDeclarationInputDataImplementation.Conditioning;

			public bool DisengagementClutch => _iiepcDeclarationInputDataImplementation.DisengagementClutch;

            #endregion
        }

        internal class StandardValuesEmInputData : IElectricMotorDeclarationInputData
        {
            private IElectricMotorDeclarationInputData _electricMotorDeclarationInputDataImplementation;

            private IList<IElectricMotorVoltageLevel> _voltageLevels = new List<IElectricMotorVoltageLevel>();

            /// <summary>
            /// Wraps the inputData and creates new voltage levels based on the voltage level provided in the input data
            /// </summary>
            /// <param name="inputData"></param>
            /// <param name="voltageLevels"></param>
            /// <exception cref="ArgumentException"></exception>
            public StandardValuesEmInputData(IElectricMotorDeclarationInputData inputData, params Volt[] voltageLevels)
            {
                _electricMotorDeclarationInputDataImplementation = inputData;
                if (inputData.CertificationMethod != CertificationMethod.StandardValues)
                {
                    throw new ArgumentException("Only for standard value certification");
                }

                foreach (var voltageLevel in voltageLevels)
                {
                    _voltageLevels.Add(new StandardValueVoltageLevelInputData(inputData.VoltageLevels.First(), voltageLevel));
                }

            }

            public IList<IElectricMotorVoltageLevel> VoltageLevels => _voltageLevels;


            #region Implementation of IComponentInputData

            public DataSource DataSource => _electricMotorDeclarationInputDataImplementation.DataSource;

            public bool SavedInDeclarationMode => _electricMotorDeclarationInputDataImplementation.SavedInDeclarationMode;

            public string Manufacturer => _electricMotorDeclarationInputDataImplementation.Manufacturer;

            public string Model => _electricMotorDeclarationInputDataImplementation.Model;

            public DateTime Date => _electricMotorDeclarationInputDataImplementation.Date;

            public string AppVersion => _electricMotorDeclarationInputDataImplementation.AppVersion;

            public CertificationMethod CertificationMethod => _electricMotorDeclarationInputDataImplementation.CertificationMethod;

            public string CertificationNumber => _electricMotorDeclarationInputDataImplementation.CertificationNumber;

            public DigestData DigestValue => _electricMotorDeclarationInputDataImplementation.DigestValue;

            #endregion

            #region Implementation of IElectricMotorDeclarationInputData

            public ElectricMachineType ElectricMachineType => _electricMotorDeclarationInputDataImplementation.ElectricMachineType;

            public Watt R85RatedPower => _electricMotorDeclarationInputDataImplementation.R85RatedPower;

            public KilogramSquareMeter Inertia => _electricMotorDeclarationInputDataImplementation.Inertia;

            public bool DcDcConverterIncluded => _electricMotorDeclarationInputDataImplementation.DcDcConverterIncluded;

            public string IHPCType => _electricMotorDeclarationInputDataImplementation.IHPCType;



            public TableData DragCurve => _electricMotorDeclarationInputDataImplementation.DragCurve;

            public TableData Conditioning => _electricMotorDeclarationInputDataImplementation.Conditioning;

            #endregion
        }
    }
}
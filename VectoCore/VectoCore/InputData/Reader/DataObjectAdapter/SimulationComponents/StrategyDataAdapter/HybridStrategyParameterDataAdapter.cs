using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter
{
	public abstract class HybridStrategyDataAdapter : IHybridStrategyDataAdapter
	{
        protected internal static Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque> CreateMaxPropulsionTorque(
			ArchitectureID archId, CombustionEngineData engineData,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> emData, GearboxData gearboxData,
			TableData boostingLimitations)
        {

            // engine data contains full-load curves already cropped with max gearbox torque and max ICE torque (vehicle level)

            var maxBoostingTorque = boostingLimitations;
            var offset = maxBoostingTorque == null ? null : MaxBoostingTorqueReader.Create(maxBoostingTorque);
            var belowIdle = offset?.FullLoadEntries.Where(x => x.MotorSpeed < engineData.IdleSpeed).ToList();

            var retVal = new Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque>();

			var isP3OrP4Hybrid = archId.IsOneOf(ArchitectureID.P3, ArchitectureID.P4);
			var isAtGearbox = gearboxData.Type.IsOneOf(GearboxType.ATSerial, GearboxType.ATPowerSplit);
			var em = isP3OrP4Hybrid ? null : emData.First(x => x.Item1 != PowertrainPosition.GEN).Item2;
			var ratioAdc = em?.RatioADC ?? 1.0;

            foreach (var key in engineData.FullLoadCurves.Keys)
            {
                if (key == 0)
                {
                    continue;
                }
                if (maxBoostingTorque == null)
                {
                    if (gearboxData.Gears[key].MaxTorque == null)
                    {
                        continue;
                    }
                    // don't know what to do...
                    // idea 1: apply gearbox limit for whole speed range
                    // idea 2: use em max torque as boosting limitation
                    //var gbxLimit = new[] {
                    //    new VehicleMaxPropulsionTorque.FullLoadEntry()
                    //        { MotorSpeed = 0.RPMtoRad(), FullDriveTorque = gearboxData.Gears[key].MaxTorque },
                    //    new VehicleMaxPropulsionTorque.FullLoadEntry() {
                    //        MotorSpeed = engineData.FullLoadCurves[0].N95hSpeed * 1.1,
                    //        FullDriveTorque = gearboxData.Gears[key].MaxTorque
                    //    }
                    //}.ToList();
					var gbxLimit = new List<VehicleMaxPropulsionTorque.FullLoadEntry>();
					gbxLimit.Add(new VehicleMaxPropulsionTorque.FullLoadEntry() {
						MotorSpeed = 0.RPMtoRad(),
						FullDriveTorque = VectoMath.Min(gearboxData.Gears[key].MaxTorque,
							(em?.EfficiencyData.VoltageLevels.Last().FullLoadDriveTorque(0.RPMtoRad(), key)?.Abs() ?? 0.SI<NewtonMeter>() * ratioAdc))
					});
                    foreach (var iceEntry in engineData.FullLoadCurves[0].FullLoadEntries) {
						gbxLimit.Add(new VehicleMaxPropulsionTorque.FullLoadEntry() {
							MotorSpeed = iceEntry.EngineSpeed,
							FullDriveTorque = VectoMath.Min(gearboxData.Gears[key].MaxTorque, 
								iceEntry.TorqueFullLoad + (em?.EfficiencyData.VoltageLevels.Last().FullLoadDriveTorque(
									iceEntry.EngineSpeed * ratioAdc, key).Abs() * ratioAdc ?? 0.SI<NewtonMeter>()))
						});
					}
					var bKey = isAtGearbox
						? new GearshiftPosition(key, true)
						: new GearshiftPosition(key);
					if (isAtGearbox && gearboxData.Gears[key].HasTorqueConverter) {
						var limit1C = new List<VehicleMaxPropulsionTorque.FullLoadEntry>() {
                            new VehicleMaxPropulsionTorque.FullLoadEntry() {
                                MotorSpeed = 0.RPMtoRad(),
                                FullDriveTorque = gearboxData.Gears[key].MaxTorque
                            },
                            new VehicleMaxPropulsionTorque.FullLoadEntry() {
                                MotorSpeed = engineData.FullLoadCurves[0].N95hSpeed,
                                FullDriveTorque = gearboxData.Gears[key].MaxTorque
                            }
                        };

                        retVal[new GearshiftPosition(key, false)] = new VehicleMaxPropulsionTorque(limit1C);
					}
                    retVal[bKey] = new VehicleMaxPropulsionTorque(gbxLimit);
                    continue;
                }

                // case boosting limit is defined, gearbox limit can be defined or not (handled in Intersect method)

                // entries contains ICE full-load curve with the boosting torque added. handles ICE speeds below idle
                var entries = belowIdle.Select(fullLoadEntry => new VehicleMaxPropulsionTorque.FullLoadEntry()
                { MotorSpeed = fullLoadEntry.MotorSpeed, FullDriveTorque = fullLoadEntry.FullDriveTorque })
                    .Concat(
                        engineData.FullLoadCurves[key].FullLoadEntries.Where(x => x.EngineSpeed > engineData.IdleSpeed)
                            .Select(fullLoadCurveEntry =>
                                new VehicleMaxPropulsionTorque.FullLoadEntry()
                                {
                                    MotorSpeed = fullLoadCurveEntry.EngineSpeed,
                                    FullDriveTorque = fullLoadCurveEntry.TorqueFullLoad +
                                                    VectoMath.Max(
                                                        offset?.FullLoadDriveTorque(fullLoadCurveEntry.EngineSpeed),
                                                        0.SI<NewtonMeter>())
                                }))
                    .Concat(
                        new[] { engineData.IdleSpeed, engineData.IdleSpeed - 0.1.RPMtoRad() }.Select(x =>
                            new VehicleMaxPropulsionTorque.FullLoadEntry()
                            {
                                MotorSpeed = x,
                                FullDriveTorque = engineData.FullLoadCurves[0].FullLoadStationaryTorque(x) +
                                                VectoMath.Max(offset?.FullLoadDriveTorque(x),
                                                    0.SI<NewtonMeter>())
                            }))
                    .OrderBy(x => x.MotorSpeed).ToList();

                // if no gearbox limit is defined, MaxTorque is null;
                // in case of P3 or P4, do not apply gearbox limit to propulsion limit as ICE is already cropped with max torque
                var gearboxTorqueLimit = isP3OrP4Hybrid ? null : gearboxData.Gears[key].MaxTorque;
				var dKey = isAtGearbox
					? new GearshiftPosition(key, true)
					: new GearshiftPosition(key);
				if (isAtGearbox && gearboxData.Gears[key].HasTorqueConverter) {
					retVal[new GearshiftPosition(key, false)] =
						new VehicleMaxPropulsionTorque(IntersectMaxPropulsionTorqueCurve(entries, gearboxTorqueLimit));
				}
                retVal[dKey] = new VehicleMaxPropulsionTorque(IntersectMaxPropulsionTorqueCurve(entries, gearboxTorqueLimit));

            }

            return retVal;
        }
        /// <summary>
        /// Intersects max torque curve.
        /// </summary>
        /// <param name="maxTorqueEntries"></param>
        /// <param name="maxTorque"></param>
        /// <returns>A combined EngineFullLoadCurve with the minimum full load torque over all inputs curves.</returns>
        internal static IList<VehicleMaxPropulsionTorque.FullLoadEntry> IntersectMaxPropulsionTorqueCurve(IList<VehicleMaxPropulsionTorque.FullLoadEntry> maxTorqueEntries, NewtonMeter maxTorque)
        {
            if (maxTorque == null)
            {
                return maxTorqueEntries;
            }

            var entries = new List<VehicleMaxPropulsionTorque.FullLoadEntry>();
            var firstEntry = maxTorqueEntries.First();
            if (firstEntry.FullDriveTorque < maxTorque)
            {
                entries.Add(maxTorqueEntries.First());
            }
            else
            {
                entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
                {
                    MotorSpeed = firstEntry.MotorSpeed,
                    FullDriveTorque = maxTorque,
                });
            }
            foreach (var entry in maxTorqueEntries.Pairwise(Tuple.Create))
            {
                if (entry.Item1.FullDriveTorque <= maxTorque && entry.Item2.FullDriveTorque <= maxTorque)
                {
                    // segment is below maxTorque line -> use directly
                    entries.Add(entry.Item2);
                }
                else if (entry.Item1.FullDriveTorque > maxTorque && entry.Item2.FullDriveTorque > maxTorque)
                {
                    // segment is above maxTorque line -> add limited entry
                    entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
                    {
                        MotorSpeed = entry.Item2.MotorSpeed,
                        FullDriveTorque = maxTorque,
                    });
                }
                else
                {
                    // segment intersects maxTorque line -> add new entry at intersection
                    var edgeFull = Edge.Create(
                        new Point(entry.Item1.MotorSpeed.Value(), entry.Item1.FullDriveTorque.Value()),
                        new Point(entry.Item2.MotorSpeed.Value(), entry.Item2.FullDriveTorque.Value()));

                    var intersectionX = (maxTorque.Value() - edgeFull.OffsetXY) / edgeFull.SlopeXY;
                    if (!entries.Any(x => x.MotorSpeed.IsEqual(intersectionX)) && !intersectionX.IsEqual(entry.Item2.MotorSpeed.Value()))
                    {
                        entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
                        {
                            MotorSpeed = intersectionX.SI<PerSecond>(),
                            FullDriveTorque = maxTorque,
                        });
                    }

                    entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry
                    {
                        MotorSpeed = entry.Item2.MotorSpeed,
                        FullDriveTorque = entry.Item2.FullDriveTorque > maxTorque ? maxTorque : entry.Item2.FullDriveTorque,

                    });
                }
            }


            return entries;
        }

		#region Implementation of IHybridStrategyDataAdapter

		public abstract HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData,
			SuperCapData superCap,
			OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType,
			ArchitectureID archID,
			CombustionEngineData engineData,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> runDataElectricMachinesData, GearboxData gearboxData,
			TableData boostingLimitations);

		public abstract HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData, SuperCapData superCapData,
			Kilogram vehicleMass, OvcHevMode ovcMode);

		#endregion
	}

    public class ParallelHybridStrategyParameterDataAdapter : HybridStrategyDataAdapter
	{
		public override HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData,
			SuperCapData superCap,
			OvcHevMode ovcMode,
			LoadingType loading,
			VehicleClass vehicleClass,
			MissionType missionType, ArchitectureID archID, CombustionEngineData engineData,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> emData, GearboxData gearboxData,
			TableData boostingLimitations)
		{
			if (batterySystemData == null && superCap == null) {
				return null;
			}

			if (batterySystemData != null && superCap != null) {
				throw new VectoException("REESS input: Supercap AND Batteries in the same vehicle are not supported", 
					ErrorCodes.REESS_MULTIPLE_TYPES,
					$"Make sure to provide either Supercap or Batteries, but not both.");
			}

			var result = new HybridStrategyParameters();
			
			if (batterySystemData != null) {
				var tmpBatterySystem = new BatterySystem(null, batterySystemData);
				result.MinSoC = tmpBatterySystem.MinSoC;
				result.MaxSoC = tmpBatterySystem.MaxSoC;
				result.TargetSoC = (result.MaxSoC + result.MinSoC) / 2    ;
				result.InitialSoc = batterySystemData.InitialSoC;
			} else {
				result.MinSoC = superCap.MinVoltage / superCap.MaxVoltage;
				result.MaxSoC = 1;// superCap.MaxVoltage / superCap.MaxVoltage;
				
				result.TargetSoC = Math.Sqrt((Math.Pow(superCap.MaxVoltage.Value(), 2) + Math.Pow(superCap.MinVoltage.Value(), 2)) / 2) /
									superCap.MaxVoltage.Value();
				result.InitialSoc = superCap.InitialSoC;
			}



			//var equivalenceFactor = DeclarationData.HevStrategyParameters.

			result.AuxReserveTime = 0.SI<Second>();
			result.AuxReserveChargeTime = 0.SI<Second>();
			result.MinICEOnTime = 10.SI<Second>();
			result.ICEStartPenaltyFactor = 0.1;
			result.CostFactorSOCExponent = 1;
			result.MaxPropulsionTorque =
				CreateMaxPropulsionTorque(archID, engineData, emData, gearboxData, boostingLimitations);

			if (ovcMode == OvcHevMode.ChargeSustaining) {
				result.EquivalenceFactor =
					DeclarationData.HEVStrategyParameters.LookupEquivalenceFactor(missionType,
						vehicleClass, loading, result.MaxSoC - result.MinSoC);
			} else {
				result.EquivalenceFactor = DeclarationData.HEVStrategyParameters.PHEVChargeDepletingEquivalenceFactor;


			}


			var genericEMEfficiency = 0.85;
			result.EquivalenceFactorCharge = result.EquivalenceFactor * genericEMEfficiency;
			result.EquivalenceFactorDischarge = result.EquivalenceFactor / genericEMEfficiency;
			

			return result;
		}

		public override HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData, SuperCapData superCapData,
			Kilogram vehicleMass, OvcHevMode ovcMode)
		{
			throw new NotImplementedException("Not supported for parallel hybrid strategy");
		}
	}

	public class SerialHybridStrategyParameterDataAdapter : HybridStrategyDataAdapter
	{
		public override HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData,
			SuperCapData superCap,
			OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType,
			ArchitectureID archID,
			CombustionEngineData engineData,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> runDataElectricMachinesData, GearboxData gearboxData,
			TableData boostingLimitations)
		{
			throw new NotImplementedException("Not supported for serial hybrid strategy");
		}

		public override HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData,
			SuperCapData superCapData, Kilogram vehicleMass, OvcHevMode ovcMode)
		{
			if (batterySystemData == null && superCapData == null) {
				throw new VectoException("REESS input: Either Battery or SuperCap must be set", 
					ErrorCodes.REESS_MISSING,
					$"Make sure to provide either Battery or SuperCap data.");
			}

			
			var result = new HybridStrategyParameters();
			
			result.AuxReserveTime = null;
			result.AuxReserveChargeTime = null;
			result.MinICEOnTime = null;
			result.GensetMinOptPowerFactor = 0.2;

			result.ICEStartPenaltyFactor = double.NaN;
			result.MaxPropulsionTorque = new Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque>();
			result.EquivalenceFactorCharge = double.NaN;
			result.EquivalenceFactorDischarge = double.NaN;
			result.CostFactorSOCExponent = double.NaN;



			if (batterySystemData != null) {
				SetGenericParameters(ref result, batterySystemData, vehicleMass, ovcMode);
			}
			if (superCapData != null) {
				SetGenericParameters(ref result, superCapData, vehicleMass, ovcMode);
			}


			return result;
		}

		private void SetGenericParameters(ref HybridStrategyParameters result, SuperCapData superCapData, Kilogram vehicleMass, OvcHevMode ovcMode)
		{
			SetSuperCapParameters(result, superCapData, vehicleMass, 70.KMPHtoMeterPerSecond());
			if (result.MinSoC >= result.TargetSoC) {
				SetSuperCapParameters(result, superCapData, vehicleMass, 30.KMPHtoMeterPerSecond());
			}

			if (result.MinSoC >= result.TargetSoC || result.TargetSoC >= 1) {
				throw new VectoException("Hybrid strategy parameters input: Min SOC higher than Target SOC", 
					ErrorCodes.HYBRIDSTRATEGY_INVALID_MINSOC,
					$"Make sure to provide valid SOC values.");
			}
		}

		private void SetSuperCapParameters(HybridStrategyParameters result, SuperCapData superCapData, Kilogram vehicleMass, MeterPerSecond velocity)
		{
			var min_sc_energy = SC_Energy(superCapData.MinVoltage, superCapData.Capacity);
			var max_sc_energy = SC_Energy(superCapData.MaxVoltage, superCapData.Capacity);

			var kin_energy = KineticEnergy(vehicleMass, velocity);

			var min_hs_energy = min_sc_energy + kin_energy;
			var target_hs_energy = max_sc_energy - kin_energy;


			var u_min = SC_Voltage(min_hs_energy, superCapData.Capacity); 
			var u_target = double.MinValue.SI<Volt>();
			if (target_hs_energy.IsGreaterOrEqual(0)) {
				u_target = SC_Voltage(target_hs_energy, superCapData.Capacity);
			}


			result.MinSoC = u_min / superCapData.MaxVoltage;
			result.TargetSoC = u_target / superCapData.MaxVoltage;
			result.InitialSoc =
				Math.Sqrt(((Math.Pow(superCapData.MaxVoltage.Value(),2) + Math.Pow(superCapData.MinVoltage.Value(),2)) / 2.0)) /
				superCapData.MaxVoltage.Value();
			result.MaxSoC = 1;
		}

		private void SetGenericParameters(ref HybridStrategyParameters result,
			BatterySystemData batterySystemData, 
			Kilogram vehicleMass,
			OvcHevMode ovcMode)
		{
			var tmpSystem = new BatterySystem(null, batterySystemData);
			var deltaSoc = CalculateDeltaSocSHev(vehicleMass, tmpSystem, 100.KMPHtoMeterPerSecond());

			var reessMinSoc = tmpSystem.MinSoC;
			var reessMaxSoc = tmpSystem.MaxSoC;

			result.MinSoC = reessMinSoc + 2 * deltaSoc;
			result.TargetSoC = tmpSystem.MaxSoC - 5 * deltaSoc;
			result.MaxSoC = double.NaN;

			if (result.MinSoC >= result.TargetSoC) {
				deltaSoc = CalculateDeltaSocSHev(vehicleMass, tmpSystem, 50.KMPHtoMeterPerSecond());
				//Small battery
				result.TargetSoC = reessMaxSoc - 1 * deltaSoc;
				result.MinSoC = reessMinSoc + 2 * deltaSoc;
				if (reessMinSoc >= result.TargetSoC) {
					throw new VectoException("Hybrid strategy parameters input: Min SOC higher than Target SOC - probably the battery capacity is too small", 
						ErrorCodes.HYBRIDSTRATEGY_INVALID_MINSOC,
						$"Make sure to provide valid SOC values.");
				}
			}

			switch (ovcMode)
			{
				case OvcHevMode.ChargeSustaining:
					result.InitialSoc = result.MinSoC + deltaSoc;
					break;
				case OvcHevMode.ChargeDepleting:
					result.InitialSoc = (tmpSystem.MaxSoC + tmpSystem.MinSoC) / 2;
					result.TargetSoC = result.InitialSoc - 0.01;  // target SoC is 1% below initial SoC
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ovcMode), ovcMode, null);
			}
		}

		private double CalculateDeltaSocSHev(Kilogram vehicleMass, 
			BatterySystem tmpSystem, MeterPerSecond v)
		{

			var v_nom = tmpSystem.NominalVoltage;
			var c_nom = tmpSystem.Capacity;
			//var result = KineticEnergy(vehicleMass, v).ConvertToWattHour()  / v_nom / c_nom;
			var result = KineticEnergy(vehicleMass, v) / v_nom / c_nom;
			return result.Value();
		}

		private static WattSecond KineticEnergy(Kilogram vehicleMass, MeterPerSecond v)
		{
			return ((vehicleMass / 2.0) * v * v).Cast<WattSecond>();
		}

		private static Volt SC_Voltage(WattSecond energy, Farad capacity)
		{
			if (energy.IsSmaller(0)) {
				throw new ArgumentException($"Supercap: nameof(energy) < 0");
			}
			return Math.Sqrt((2.0 * energy / capacity).Value()).SI<Volt>();
		}

		private static Joule SC_Energy(Volt voltage, Farad capacity)
		{
			return 0.5 * capacity * voltage * voltage;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.OutputData
{
    public static class ModalDataContainerExtensions
	{
		public static T Max<T>(this IModalDataContainer data, ModalResultField field)
		{
			return data.GetValues<T>(field).Max();
		}

		public static T Min<T>(this IModalDataContainer data, ModalResultField field)
		{
			return data.GetValues<T>(field).Min();
		}
		
		public static MeterPerSquareSecond AccelerationsPositive(this IModalDataContainer data)
		{
			return data.GetValues<MeterPerSquareSecond>(ModalResultField.acc)
				.Where(x => x > 0.125)
				.DefaultIfEmpty(0.SI<MeterPerSquareSecond>())
				.Average();
		}


		public static MeterPerSquareSecond AverageAccelerationBelowTargetSpeed(this IModalDataContainer data)
		{
			var accPos = data.GetValues(
				x => new {
					a = x.Field<MeterPerSquareSecond>(ModalResultField.acc.GetName()).DefaultIfNull(0),
					dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()).DefaultIfNull(0),
					dv = x.Field<MeterPerSecond>(ModalResultField.v_targ.GetName()).DefaultIfNull(0) -
						x.Field<MeterPerSecond>(ModalResultField.v_act.GetName()).DefaultIfNull(0),
					driverStatus = x.Field<int>(ModalResultField.drivingBehavior.GetName())
				}).Where(x => x.driverStatus == 2 && x.dv > 0).ToArray();
			var duration = accPos.Sum(x => x.dt).DefaultIfNull(0);
			var accSum = accPos.Sum(x => x.a * x.dt).DefaultIfNull(0);
			if (duration.IsEqual(0, 1e-12) && accSum.IsEqual(0, 1e-12)) {
				return 0.SI<MeterPerSquareSecond>();
			}
			return accSum / duration;
		}

		public static MeterPerSquareSecond AccelerationsNegative(this IModalDataContainer data)
		{
			return data.GetValues<MeterPerSquareSecond>(ModalResultField.acc)
				.Where(x => x < -0.125)
				.DefaultIfEmpty(0.SI<MeterPerSquareSecond>())
				.Average();
		}

		public static Scalar AccelerationTimeShare(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return null;
			}
            var accelerationTimeShare = data.GetValues(x => new {
					a = x.Field<MeterPerSquareSecond>(ModalResultField.acc.GetName()).DefaultIfNull(0),
					dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
				})
				.Sum(x => x.a > 0.125 ? x.dt : 0.SI<Second>()).DefaultIfNull(0);
			return 100 * (accelerationTimeShare / data.Duration).Cast<Scalar>();
		}

		public static Scalar DecelerationTimeShare(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return null;
			}
            var decelerationTimeShare = data.GetValues(x => new {
					a = x.Field<MeterPerSquareSecond>(ModalResultField.acc.GetName()).DefaultIfNull(0),
					dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
				})
				.Sum(x => x.a < -0.125 ? x.dt : 0.SI<Second>()).DefaultIfNull(0);
			return 100 * (decelerationTimeShare / data.Duration).Cast<Scalar>();
		}

		public static Scalar CruiseTimeShare(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return null;
			}
            var cruiseTime = data.GetValues(x => new {
					v = x.Field<MeterPerSecond>(ModalResultField.v_act.GetName()).DefaultIfNull(0),
					a = x.Field<MeterPerSquareSecond>(ModalResultField.acc.GetName()).DefaultIfNull(0),
					dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
				})
				.Sum(x => x.v >= 0.1.KMPHtoMeterPerSecond() && x.a.IsBetween(-0.125, 0.125) ? x.dt : 0.SI<Second>())
				.DefaultIfNull(0);
			return 100 * (cruiseTime / data.Duration).Cast<Scalar>();
		}

		public static Scalar StopTimeShare(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return null;
			}
            var stopTime = data.GetValues(x => new {
					v = x.Field<MeterPerSecond>(ModalResultField.v_act.GetName()).DefaultIfNull(0),
					dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
				})
				.Sum(x => x.v < 0.1.KMPHtoMeterPerSecond() ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * (stopTime / data.Duration).Cast<Scalar>();
		}

		public static MeterPerSquareSecond AccelerationAverage(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>()) {
				return null;
			}
			return data.TimeIntegral<MeterPerSecond>(ModalResultField.acc) / data.Duration;
		}

		public static Meter AltitudeDelta(this IModalDataContainer data)
		{
			var altitudes = data.GetValues<Meter>(ModalResultField.altitude).ToList();
			var first = altitudes.FirstOrDefault();
			var last = altitudes.LastOrDefault();
			return first == null || last == null ? null : last - first;
		}

		public static WattSecond PowerAccelerations(this IModalDataContainer data)
		{
			var paEngine = data.TimeIntegral<WattSecond>(ModalResultField.P_ice_inertia) ?? 0.SI<WattSecond>();
			var paGearbox = data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_inertia) ?? 0.SI<WattSecond>();
			return paEngine + paGearbox;
		}


		public static WattSecond WorkClutch(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_clutch_loss);
		}

		public static WattSecond WorkGearshift(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_shift_loss);
		}

		public static WattSecond WorkGearbox(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_loss);
		}

		public static WattSecond WorkWheels(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_wheel_in);
		}

		public static WattSecond WorkWheelsPos(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_wheel_in, x => x > 0);
		}

		public static WattSecond WorkAxlegear(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_axle_loss);
		}

		public static WattSecond WorkRetarder(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_ret_loss);
		}

		public static WattSecond WorkAngledrive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_angle_loss);
		}

		public static WattSecond WorkTorqueConverter(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_TC_loss);
		}

		public static WattSecond WorkWheelEnd(this IModalDataContainer data)
		{ 
			return data.TimeIntegral<WattSecond>(ModalResultField.P_wheelEnd_saving);
		}

		public static WattSecond WorkTotalMechanicalBrake(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_brake_loss);
		}

		public static WattSecond WorkVehicleInertia(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_veh_inertia) +
					data.TimeIntegral<WattSecond>(ModalResultField.P_wheel_inertia);
		}

		public static WattSecond WorkAuxiliaries(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_aux_mech);
		}

		public static WattSecond WorkElectricAuxiliaries(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_aux_el);
		}

		public static WattSecond WorkRoadGradientResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_slope);
		}

		public static WattSecond WorkRollingResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_roll);
		}

		public static WattSecond WorkAirResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_air);
		}


		public static WattSecond TotalEngineWorkPositive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_ice_fcmap, x => x > 0);
		}

		public static WattSecond TotalEngineWorkNegative(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_ice_fcmap, x => x < 0);
		}

		public static WattSecond WorkEngineStart(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_ice_start);
		}

		public static Watt PowerWheelPositive(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return 0.SI<Watt>();
			}
            return (data.WorkWheelsPos() ?? 0.SI<WattSecond>()) / data.Duration;
		}

		public static Watt PowerWheel(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return 0.SI<Watt>();
			}
            return (data.TimeIntegral<WattSecond>(ModalResultField.P_wheel_in) ?? 0.SI<WattSecond>()) / data.Duration;
		}

		public static WattSecond WorkREESSChargeTerminal(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_reess_terminal, x => x.IsGreater(0));
		}

		public static WattSecond WorkREESSDischargeTerminal(this IModalDataContainer data)
		{
			return -data.TimeIntegral<WattSecond>(ModalResultField.P_reess_terminal, x => x.IsSmaller(0));
		}

		public static WattSecond WorkREESSChargeTerminal_ES(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES, x => x.IsGreater(0));
		}

		public static WattSecond WorkREESSDischargeTerminal_ES(this IModalDataContainer data)
		{
			return -data.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES, x => x.IsSmaller(0));
		}

        public static WattSecond WorkREESSChargeInternal(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, x => x.IsGreater(0));

		}

		public static WattSecond WorkREESSDischargeInternal(this IModalDataContainer data)
		{
			return -data.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, x => x.IsSmaller(0));
		}

		public static double BatteryEfficiencyDischarge(this IModalDataContainer data)
		{
			return data.WorkREESSDischargeTerminal() / data.WorkREESSDischargeInternal();
		}


        public static KilogramPerSecond FuelConsumptionPerSecond(this IModalDataContainer data, ModalResultField mrf, IFuelProperties fuelData)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return 0.SI<KilogramPerSecond>();
			}
            return data.TimeIntegral<Kilogram>(data.GetColumnName(fuelData, mrf)) / data.Duration;
		}

		public static KilogramPerMeter FuelConsumptionPerMeter(this IModalDataContainer data, ModalResultField mrf, IFuelProperties fuelData)
		{
			var distance = data.Distance;
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}

			return data.TimeIntegral<Kilogram>(data.GetColumnName(fuelData, mrf)) / distance;
		}

		public static NormLiter AirGenerated(this IModalDataContainer data)
		{
			return data.GetValues<NormLiter>(ModalResultField.Nl_busAux_PS_generated).Sum(x => x);
		}

		public static NormLiter AirConsumed(this IModalDataContainer data)
		{
			return data.GetValues<NormLiter>(ModalResultField.Nl_busAux_PS_consumer).Sum(x => x);
		}

		public static NormLiter AirGeneratedAlwaysOn(this IModalDataContainer data)
		{
			return data.GetValues<NormLiter>(ModalResultField.Nl_busAux_PS_generated_alwaysOn).Sum(x => x);
		}

		public static WattSecond EnergyPneumaticCompressorPowerOff(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_busAux_PS_generated_dragOnly);
		}

		public static WattSecond EnergyPneumaticCompressorOn(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_busAux_PS_generated);
		}

		public static WattSecond EnergyPneumaticCompressorAlwaysOn(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_busAux_PS_generated_alwaysOn);
		}

		public static WattSecond EnergyBusAuxESGeneratedMech(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_sum_mech);
		}

		public static WattSecond EnergyBusAuxESGenerated(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_generated);
		}

		public static WattSecond EnergyBusAuxESConsumed(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_consumer_sum);
		}

		public static double DeltaSOCBusAuxBattery(this IModalDataContainer data)
		{
			return (data.GetValues<double>(ModalResultField.BatterySOC).First() -
					data.GetValues<double>(ModalResultField.BatterySOC).Last()) / 100;
		}


		public static Kilogram TotalFuelConsumption(this IModalDataContainer data, ModalResultField mrf, IFuelProperties fuelData)
		{
			return data.TimeIntegral<Kilogram>(data.GetColumnName(fuelData, mrf));
		}

		
		public static Watt TotalPowerEnginePositiveAverage(this IModalDataContainer data)
		{
			//var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval);
			var values = data.GetValues(x => new {
					Value = x.Field<Watt>(ModalResultField.P_ice_fcmap.GetName()).DefaultIfNull(0) * x.Field<Second>(ModalResultField.simulationInterval.GetName())
				})
				//.GetValues<Watt>(ModalResultField.P_ice_fcmap)
				//.Zip(simulationIntervals, (value, dt) => new { Dt = dt, Value = value * dt })
				.Where(v => v.Value > 0).ToList();
			if (values.Any()) {
				return values.Sum(v => v.Value) / data.Duration;
			}
			return 0.SI<Watt>();
		}

		public static Watt TotalPowerEngineAverage(this IModalDataContainer data)
		{
			var values = data.GetValues(
				x => new {
					Value = x.Field<Watt>(ModalResultField.P_ice_fcmap.GetName()).DefaultIfNull(0) *
							x.Field<Second>(ModalResultField.simulationInterval.GetName())
				}).ToList();
			if (values.Any()) {
				return values.Sum(v => v.Value) / data.Duration;
			}

			return 0.SI<Watt>();
		}

		public static MeterPerSecond Speed(this IModalDataContainer data)
		{
			var distance = data.Distance;
			var duration = data.Duration;
			if (distance == null || duration == null || duration.IsEqual(0)) {
				return null;
			}
			return distance / duration;
		}

		public static WattSecond AuxiliaryWork(this IModalDataContainer data, DataColumn auxCol)
		{
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval).ToArray();
			var auxValues = data.GetValues<Watt>(auxCol).ToArray();
			var sum = 0.SI<WattSecond>();
			for (var i = 0; i < simulationIntervals.Length; i++) {
				if (auxValues[i] != null && simulationIntervals[i] != null) {
					sum += auxValues[i] * simulationIntervals[i];
				}
			}
			return sum;
		}


		public static MeterPerSecond MaxSpeed(this IModalDataContainer data)
		{
			return data.Max<MeterPerSecond>(ModalResultField.v_act).DefaultIfNull(0);
		}

		public static MeterPerSecond MinSpeed(this IModalDataContainer data)
		{
			return data.Min<MeterPerSecond>(ModalResultField.v_act).DefaultIfNull(0);
		}

		public static MeterPerSquareSecond MaxAcceleration(this IModalDataContainer data)
		{
			return data.Max<MeterPerSquareSecond>(ModalResultField.acc).DefaultIfNull(0);
		}

		public static MeterPerSquareSecond MaxDeceleration(this IModalDataContainer data)
		{
			return -data.Min<MeterPerSquareSecond>(ModalResultField.acc).DefaultIfNull(0);
		}

		public static PerSecond AvgEngineSpeed(this IModalDataContainer data)
		{
			var integral = data.GetValues(x => x.Field<PerSecond>(ModalResultField.n_ice_avg.GetName()).Value() *
												x.Field<Second>(ModalResultField.simulationInterval.GetName()).Value()).Sum();
			return (integral / data.Duration.Value()).SI<PerSecond>();
		}

		public static PerSecond MaxEngineSpeed(this IModalDataContainer data)
		{
			return data.Max<PerSecond>(ModalResultField.n_ice_avg);
		}

		public static Scalar ICEMaxLoadTimeShare(this IModalDataContainer data)
		{
			if (!data.HasCombustionEngine) {
				return 0.SI<Scalar>();
			}
			var tmp = data.GetValues(x => new {
				tMax = x.Field<NewtonMeter>(ModalResultField.T_ice_full.GetName()).DefaultIfNull(-1),
				tEng = x.Field<NewtonMeter>(ModalResultField.T_ice_fcmap.GetName()).DefaultIfNull(0),
				dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
				iceOn = !(x[ModalResultField.ICEOn.GetName()] is DBNull) &&
						x.Field<bool>(ModalResultField.ICEOn.GetName())
			});
			var sum = tmp.Where(x => x.iceOn).Sum(x => x.tMax.IsEqual(x.tEng, 5.SI<NewtonMeter>()) ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * sum / data.Duration;
		}

		public static Scalar ICEOffTimeShare(this IModalDataContainer data)
		{
			var iceOn = data.GetValues(x => new {
				dt = x[ModalResultField.ICEOn.GetName()] is DBNull || !x.Field<Boolean>(ModalResultField.ICEOn.GetName())
					? 0.SI<Second>()
					: x.Field<Second>(ModalResultField.simulationInterval.GetName())
			}).Sum(x => x.dt) ?? 0.SI<Second>();
			return 100 * (1 - iceOn / data.Duration);
		}

		public static Scalar ElectricMotorOffTimeShare(this IModalDataContainer data, PowertrainPosition pos)
		{
			if (data.Duration == 0.SI<Second>()) {
				return null;
			}
			var offField = pos == PowertrainPosition.IEPC ? ModalResultField.IEPC_Off_ : ModalResultField.EM_Off_;
			var emOff = data.GetValues(x => new {
				dt = x[string.Format(offField.GetCaption(), pos.GetName())] is DBNull || !x.Field<Scalar>(string.Format(offField.GetCaption(), pos.GetName())).IsEqual(1)
					? 0.SI<Second>()
					: x.Field<Second>(ModalResultField.simulationInterval.GetName())
			}).Sum(x => x.dt) ?? 0.SI<Second>();
			return 100 * emOff / data.Duration;
		}

		/// <summary>
		/// The following logic applies:
		/// - shifting from gear A to gear B counts as gearshift (with or without traction interruption)
		/// - shifting from gear A to neutral couts as gearshift if the vehicle stopped
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Scalar GearshiftCount(this IModalDataContainer data)
		{
			if (!data.HasGearbox) {
				return 0.SI<Scalar>();
			}
			var prevGear = data.GetValues<uint>(ModalResultField.Gear).First();
			var lastGear = prevGear;
			var gearCount = 0;

			var shifts = data.GetValues(x => new {
				Gear = x.Field<uint>(ModalResultField.Gear.GetName()),
				Speed = x.Field<MeterPerSecond>(ModalResultField.v_act.GetName())
			});
			foreach (var entry in shifts) {
				if (entry.Speed != null && entry.Speed.IsSmallerOrEqual(0.1)) {
					if (prevGear != entry.Gear) {
						gearCount++;
					}
				}
				if (entry.Gear != 0 && entry.Gear != prevGear) {
					if (lastGear != entry.Gear) {
						gearCount++;
					}
					lastGear = entry.Gear;
				}

				prevGear = entry.Gear;
			}
			return gearCount.SI<Scalar>();
		}

		public static Scalar CoastingTimeShare(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>()) {
				return null;
			}
			var sum = data.GetValues(x => new {
					DrivingBehavior = x.Field<DrivingBehavior>(ModalResultField.drivingBehavior.GetName()),
					dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
				})
				.Sum(x => x.DrivingBehavior == DrivingBehavior.Coasting ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * sum / data.Duration;
		}

		public static Scalar BrakingTimeShare(this IModalDataContainer data)
		{
			if (data.Duration == 0.SI<Second>())
			{
				return null;
			}
            var sum = data.GetValues(x => new {
					DrivingBehavior = x.Field<DrivingBehavior>(ModalResultField.drivingBehavior.GetName()),
					dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
				})
				.Sum(x => x.DrivingBehavior == DrivingBehavior.Braking ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * sum / data.Duration;
		}

		public static Dictionary<uint, Scalar> TimeSharePerGear(this IModalDataContainer data, uint gearCount)
		{
			var retVal = new Dictionary<uint, Scalar>();
			for (uint i = 0; i <= gearCount; i++) {
				retVal[i] = 0.SI<Scalar>();
			}

			if (!data.ContainsColumn(ModalResultField.Gear.GetName())) {
				return retVal;
			}
			var gearData = data.GetValues(x => new {
				Gear = x.Field<uint>(ModalResultField.Gear.GetName()),
				dt = x.Field<Second>(ModalResultField.simulationInterval.GetName())
			});

			foreach (var entry in gearData) {
				retVal[entry.Gear] += entry.dt.Value();
			}

			var duration = data.Duration.Value();
			for (uint i = 0; i <= gearCount; i++) {
				if (data.Duration == 0.SI<Second>())
				{
					retVal[i] = null;
				}
                retVal[i] = 100 * retVal[i] / duration;
			}
			return retVal;
		}

		public static int NumICEStarts(this IModalDataContainer data)
		{
			return data.GetValues(x => x.Field<bool>(ModalResultField.ICEOn.GetName())).Pairwise((x, y) => !x && y ? 1 : 0).Sum();
		}

		public static WattSecond REESSLoss(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_reess_loss);
		}

		public static WattSecond ESConnectorLoss(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_ES_Conn_loss);
		}

		/// <summary>
		/// SOC(end) - SOC(start)
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
        public static double REESSDeltaSoc(this IModalDataContainer data)
		{
			return data.REESSEndSoC() - data.REESSStartSoC();
		}


		public static double REESSStartSoC(this IModalDataContainer data)
		{
			return (data.GetValues<SI>(ModalResultField.REESSStateOfCharge)?.First()?.Value() ?? 0) * 100;
		}

		public static double REESSEndSoC(this IModalDataContainer data)
		{
			return (data.GetValues<SI>(ModalResultField.REESSStateOfCharge)?.LastOrDefault()?.Value() ?? 0) * 100;
		}

		public static double REESSMinSoc(this IModalDataContainer data)
		{
			return (data.GetValues<Scalar>(ModalResultField.REESSStateOfCharge)?.Min()?.Value() ?? 0) * 100;
		}

		public static double REESSMaxSoc(this IModalDataContainer data)
		{
			return (data.GetValues<Scalar>(ModalResultField.REESSStateOfCharge)?.Max()?.Value() ?? 0) * 100;
		}
	}
}
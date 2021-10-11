/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.OutputData
{
	public interface IModalDataFilter
	{
		ModalResults Filter(ModalResults data);
		string ID { get; }
	}

	public interface IModalDataContainer
	{
		/// <summary>
		/// Indexer for fields of the DataWriter. Accesses the data of the current step.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object this[ModalResultField key] { get; set; }

		/// <summary>
		/// Indexer for auxiliary fields of the DataWriter.
		/// </summary>
		/// <param name="auxId"></param>
		/// <returns></returns>
		object this[string auxId] { get; set; }

		bool HasTorqueConverter { set; }

		/// <summary>
		/// Commits the data of the current simulation step.
		/// </summary>
		void CommitSimulationStep();

		FuelData.Entry FuelData { get; }

		VectoRun.Status RunStatus { get; }

		string Error { get; }

		string StackTrace { get; }

		IEnumerable<T> GetValues<T>(ModalResultField key);

		IEnumerable<T> GetValues<T>(DataColumn col);

		IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc);

		Dictionary<string, DataColumn> Auxiliaries { get; }

		T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T>;

		void SetDataValue(string fieldName, object value);

		void AddAuxiliary(string id, string columnName = null);

		/// <summary>
		/// Finishes the writing of the DataWriter.
		/// </summary>
		void Finish(VectoRun.Status runStatus, Exception exception = null);

		/// <summary>
		/// clear the modal data after the simulation
		/// called after the simulation is finished and the sum-entries have been written
		/// </summary>
		void FinishSimulation();

		void Reset();
	}

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

		/// <summary>
		/// Returns a default value if the SI object is null.
		/// </summary>
		/// <typeparam name="T">The SI Type.</typeparam>
		/// <param name="self">The SI Instance.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>If self is null, the default value as SI-Type is returned. Otherwise self is returned.</returns>
		/// <code>
		/// NewtonMeter t = null;
		/// var x = t.DefaultIfNull(0);
		/// </code>
		public static T DefaultIfNull<T>(this T self, double defaultValue) where T : SIBase<T>
		{
			return self ?? defaultValue.SI<T>();
		}

		public static MeterPerSquareSecond AccelerationsPositive(this IModalDataContainer data)
		{
			return data.GetValues<MeterPerSquareSecond>(ModalResultField.acc)
				.Where(x => x > 0.125)
				.DefaultIfEmpty(0.SI<MeterPerSquareSecond>())
				.Average();
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
			var accelerationTimeShare = data.GetValues(x => new {
				a = x.Field<MeterPerSquareSecond>((int)ModalResultField.acc).DefaultIfNull(0),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			})
				.Sum(x => x.a > 0.125 ? x.dt : 0.SI<Second>()).DefaultIfNull(0);
			return 100 * (accelerationTimeShare / data.Duration()).Cast<Scalar>();
		}

		public static Scalar DecelerationTimeShare(this IModalDataContainer data)
		{
			var decelerationTimeShare = data.GetValues(x => new {
				a = x.Field<MeterPerSquareSecond>((int)ModalResultField.acc).DefaultIfNull(0),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			})
				.Sum(x => x.a < -0.125 ? x.dt : 0.SI<Second>()).DefaultIfNull(0);
			return 100 * (decelerationTimeShare / data.Duration()).Cast<Scalar>();
		}

		public static Scalar CruiseTimeShare(this IModalDataContainer data)
		{
			var cruiseTime = data.GetValues(x => new {
				v = x.Field<MeterPerSecond>((int)ModalResultField.v_act).DefaultIfNull(0),
				a = x.Field<MeterPerSquareSecond>((int)ModalResultField.acc).DefaultIfNull(0),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			})
				.Sum(x => x.v >= 0.1.KMPHtoMeterPerSecond() && x.a.IsBetween(-0.125, 0.125) ? x.dt : 0.SI<Second>())
				.DefaultIfNull(0);
			return 100 * (cruiseTime / data.Duration()).Cast<Scalar>();
		}

		public static Scalar StopTimeShare(this IModalDataContainer data)
		{
			var stopTime = data.GetValues(x => new {
				v = x.Field<MeterPerSecond>((int)ModalResultField.v_act).DefaultIfNull(0),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			})
				.Sum(x => x.v < 0.1.KMPHtoMeterPerSecond() ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * (stopTime / data.Duration()).Cast<Scalar>();
		}

		public static MeterPerSquareSecond AccelerationAverage(this IModalDataContainer data)
		{
			return data.TimeIntegral<MeterPerSecond>(ModalResultField.acc) / data.Duration();
		}

		public static Meter AltitudeDelta(this IModalDataContainer data)
		{
			var altitudes = data.GetValues<Meter>(ModalResultField.altitude).ToList();
			var first = altitudes.First();
			var last = altitudes.Last();
			return first == null || last == null ? null : last - first;
		}

		public static WattSecond PowerAccelerations(this IModalDataContainer data)
		{
			var paEngine = data.TimeIntegral<WattSecond>(ModalResultField.P_eng_inertia);
			var paGearbox = data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_inertia);
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

		public static Second Duration(this IModalDataContainer data)
		{
			var time = data.GetValues<Second>(ModalResultField.time).ToList();
			var dt = data.GetValues<Second>(ModalResultField.simulationInterval).ToList();
			if (time.Count == 1) {
				return time.First();
			}
			return time.Max() - time.Min() + dt.First() / 2 + dt.Last() / 2;
		}

		public static Meter Distance(this IModalDataContainer data)
		{
			var max = data.GetValues<Meter>(ModalResultField.dist).LastOrDefault() ?? 0.SI<Meter>();
			var first = data.GetValues(
				r => new {
					dist = r.Field<Meter>((int)ModalResultField.dist),
					vact = r.Field<MeterPerSecond>((int)ModalResultField.v_act),
					acc = r.Field<MeterPerSquareSecond>((int)ModalResultField.acc),
					dt = r.Field<Second>((int)ModalResultField.simulationInterval)
				}).First();
			var min = 0.SI<Meter>();
			if (first != null && first.vact != null && first.acc != null && first.dt != null) {
				min = first.dist - first.vact * first.dt - first.acc * first.dt * first.dt / 2.0;
			}
			
			return max == null || min == null ? null : max - min;
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
			return data.TimeIntegral<WattSecond>(ModalResultField.P_aux);
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
			return data.TimeIntegral<WattSecond>(ModalResultField.P_eng_fcmap, x => x > 0);
		}

		public static WattSecond TotalEngineWorkNegative(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_eng_fcmap, x => x < 0);
		}

		public static Watt PowerWheelPositive(this IModalDataContainer data)
		{
			return data.WorkWheelsPos() / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionWHTC(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCWHTCc) / distance;
		}

		public static KilogramPerSecond FuelConsumptionWHTCPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCWHTCc) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionNCVCorrected(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCNCVc) / distance;
		}

		public static KilogramPerSecond FuelConsumptionNCVCorrectedPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCNCVc) / data.Duration();
		}

		public static KilogramPerSecond FuelConsumptionAAUXPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAAUX) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionAAUX(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAAUX) / distance;
		}

		public static KilogramPerSecond FuelConsumptionADASPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCADAS) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionADAS(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCADAS) / distance;
		}

		

		public static KilogramPerSecond FuelConsumptionFinalPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCFinal) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionFinal(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCFinal) / distance;
		}

		public static VolumePerMeter FuelConsumptionFinalVolumePerMeter(this IModalDataContainer data)
		{
			var fuelConsumptionFinal = data.FuelConsumptionFinal();
			if (fuelConsumptionFinal == null || data.FuelData.FuelDensity == null) {
				return null;
			}

			var fcVolumePerMeter = fuelConsumptionFinal / data.FuelData.FuelDensity;
			return fcVolumePerMeter.Cast<VolumePerMeter>();
		}

		public static KilogramPerMeter CO2PerMeter(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCFinal) * data.FuelData.CO2PerFuelWeight / distance;
		}

		public static JoulePerMeter EnergyPerMeter(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCFinal) * data.FuelData.LowerHeatingValueVecto / distance;
		}

		public static Kilogram TotalFuelConsumption(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCMap);
		}

		public static KilogramPerSecond FCMapPerSecond(this IModalDataContainer data)
		{
			return data.TotalFuelConsumption() / data.Duration();
		}

		public static KilogramPerMeter FCMapPerMeter(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TotalFuelConsumption() / distance;
		}


		public static Watt TotalPowerEnginePositiveAverage(this IModalDataContainer data)
		{
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval);
			var values = data.GetValues<Watt>(ModalResultField.P_eng_fcmap)
				.Zip(simulationIntervals, (value, dt) => new { Dt = dt, Value = value * dt })
				.Where(v => v.Value > 0).ToList();
			if (values.Any()) {
				return values.Sum(v => v.Value) / Duration(data);
			}
			return 0.SI<Watt>();
		}

		public static MeterPerSecond Speed(this IModalDataContainer data)
		{
			var distance = Distance(data);
			var duration = Duration(data);
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
			var integral = data.GetValues(x => x.Field<PerSecond>((int)ModalResultField.n_eng_avg).Value() *
												x.Field<Second>((int)ModalResultField.simulationInterval).Value()).Sum();
			return (integral / Duration(data).Value()).SI<PerSecond>();
		}

		public static PerSecond MaxEngineSpeed(this IModalDataContainer data)
		{
			return data.Max<PerSecond>(ModalResultField.n_eng_avg);
		}

		public static Scalar EngineMaxLoadTimeShare(this IModalDataContainer data)
		{
			var sum = data.GetValues(x => new {
				tMax = x.Field<NewtonMeter>((int)ModalResultField.Tq_full).DefaultIfNull(-1),
				tEng = x.Field<NewtonMeter>((int)ModalResultField.T_eng_fcmap).DefaultIfNull(0),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			}).Sum(x => x.tMax.IsEqual(x.tEng, 5.SI<NewtonMeter>()) ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * sum / Duration(data);
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
			var prevGear = data.GetValues<uint>(ModalResultField.Gear).First();
			var lastGear = prevGear;
			var gearCount = 0;

			var shifts = data.GetValues(x => new {
				Gear = x.Field<uint>((int)ModalResultField.Gear),
				Speed = x.Field<MeterPerSecond>((int)ModalResultField.v_act)
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
			var sum = data.GetValues(x => new {
				DrivingBehavior = x.Field<DrivingBehavior>((int)ModalResultField.drivingBehavior),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			})
				.Sum(x => x.DrivingBehavior == DrivingBehavior.Coasting ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * sum / Duration(data);
		}

		public static Scalar BrakingTimeShare(this IModalDataContainer data)
		{
			var sum = data.GetValues(x => new {
				DrivingBehavior = x.Field<DrivingBehavior>((int)ModalResultField.drivingBehavior),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			})
				.Sum(x => x.DrivingBehavior == DrivingBehavior.Braking ? x.dt : 0.SI<Second>()) ?? 0.SI<Second>();
			return 100 * sum / Duration(data);
		}

		public static Dictionary<uint, Scalar> TimeSharePerGear(this IModalDataContainer data, uint gearCount)
		{
			var retVal = new Dictionary<uint, Scalar>();
			for (uint i = 0; i <= gearCount; i++) {
				retVal[i] = 0.SI<Scalar>();
			}

			var gearData = data.GetValues(x => new {
				Gear = x.Field<uint>((int)ModalResultField.Gear),
				dt = x.Field<Second>((int)ModalResultField.simulationInterval)
			});

			foreach (var entry in gearData) {
				retVal[entry.Gear] += entry.dt.Value();
			}

			var duration = Duration(data).Value();
			for (uint i = 0; i <= gearCount; i++) {
				retVal[i] = 100 * retVal[i] / duration;
			}
			return retVal;
		}
	}
}

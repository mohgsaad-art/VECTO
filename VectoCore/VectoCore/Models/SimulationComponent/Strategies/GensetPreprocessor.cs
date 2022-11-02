using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	public class GensetPreprocessor : ISimulationPreprocessor
	{
		protected TestGenset Genset;
		private CombustionEngineData IceData;
		private ElectricMotorData EmData;
		protected GenSetCharacteristics OptimalPoints;

		public GensetPreprocessor(GenSetCharacteristics optimalPoints, TestGenset testGenSet, CombustionEngineData engineData,
			ElectricMotorData electricMotorData)
		{
			Genset = testGenSet;
			IceData = engineData;
			EmData = electricMotorData;
			OptimalPoints = optimalPoints;
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			var voltage = EmData.EfficiencyData.VoltageLevels.First().Voltage;

			IterateElectricPower(voltage);

			MaxElectricPower(voltage, false);

			MaxElectricPower(voltage, true);
			
        }


		private void MaxElectricPower(Volt voltage, bool emDerated)
		{
			var continuousTq = emDerated ? EmData.Overload.ContinuousTorque : double.MaxValue.SI<NewtonMeter>();
			var maxSpeed = VectoMath.Min(EmData.EfficiencyData.MaxSpeed,
				IceData.FullLoadCurves[0].FullLoadEntries.Select(x => x.EngineSpeed).Max());
			var emFldDrivetrain = new ElectricMotorFullLoadCurve(EmData.EfficiencyData.VoltageLevels[0].FullLoadCurve
				.FullLoadEntries.Select(x =>
					new ElectricMotorFullLoadCurve.FullLoadEntry() {
						FullGenerationTorque =
							Genset.ElectricMotor.ConvertEmTorqueToDrivetrain(x.MotorSpeed, VectoMath.Min(continuousTq ,x.FullGenerationTorque), false),
						FullDriveTorque =
							Genset.ElectricMotor.ConvertEmTorqueToDrivetrain(x.MotorSpeed, VectoMath.Max(-continuousTq, x.FullDriveTorque), false),
						MotorSpeed = Genset.ElectricMotor.ConvertEmSpeedToDrivetrain(x.MotorSpeed)
					}).Where(x => x.MotorSpeed.IsSmallerOrEqual(maxSpeed)).ToList());
			if (!emFldDrivetrain.FullLoadEntries.Any(x => x.MotorSpeed.IsEqual(maxSpeed))) {
				emFldDrivetrain.FullLoadEntries.Add(new ElectricMotorFullLoadCurve.FullLoadEntry() {
					FullGenerationTorque =
						Genset.ElectricMotor.ConvertEmTorqueToDrivetrain(maxSpeed, VectoMath.Min(continuousTq, emFldDrivetrain.FullGenerationTorque(maxSpeed)), false),
					FullDriveTorque =
						Genset.ElectricMotor.ConvertEmTorqueToDrivetrain(maxSpeed, VectoMath.Max(-continuousTq, emFldDrivetrain.FullLoadDriveTorque(maxSpeed)), false),
					MotorSpeed = maxSpeed
				});
			}
			var iceFld = new ElectricMotorFullLoadCurve(IceData.FullLoadCurves[0].FullLoadEntries.Select(x =>
				new ElectricMotorFullLoadCurve.FullLoadEntry() {
					FullDriveTorque = -x.TorqueFullLoad,
					FullGenerationTorque = 0.SI<NewtonMeter>(),
					MotorSpeed = x.EngineSpeed
				}).Where(x => x.MotorSpeed.IsSmallerOrEqual(maxSpeed)).ToList());
			if (!iceFld.FullLoadEntries.Any(x => x.MotorSpeed.IsEqual(maxSpeed))) {
				iceFld.FullLoadEntries.Add(new ElectricMotorFullLoadCurve.FullLoadEntry() {
					FullDriveTorque = -IceData.FullLoadCurves[0].FullLoadStationaryTorque(maxSpeed),
					FullGenerationTorque = 0.SI<NewtonMeter>(),
					MotorSpeed = maxSpeed
				});
			}

			var combinedFldEntries = AbstractSimulationDataAdapter.IntersectEMFullLoadCurves(iceFld, emFldDrivetrain).FullLoadEntries.Select(x =>
				new EngineFullLoadCurve.FullLoadCurveEntry() {
					EngineSpeed = x.MotorSpeed,
					TorqueFullLoad = -x.FullDriveTorque,
					TorqueDrag = 0.SI<NewtonMeter>()
				}).Where(x => x.EngineSpeed > IceData.IdleSpeed && x.EngineSpeed < IceData.FullLoadCurves[0].N95hSpeed).ToList();

			var combinedFld = new EngineFullLoadCurve(combinedFldEntries, null);
			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var ratedSpeed = combinedFld.RatedSpeed;
			Genset.ElectricMotorCtl.EMTorque = combinedFld.FullLoadStationaryTorque(ratedSpeed);
			Genset.ElectricMotor.Initialize(combinedFld.FullLoadStationaryTorque(ratedSpeed), ratedSpeed);
			var response = Genset.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), ratedSpeed);

			if (response is ResponseSuccess) {
				var fc = IceData.Fuels.Sum(x =>
					x.ConsumptionMap.GetFuelConsumptionValue(response.Engine.TotalTorqueDemand,
						response.Engine.EngineSpeed));
				var tmp = new GenSetOperatingPoint() {
					ICEOn = true,
					ElectricPower = response.ElectricSystem.ConsumerPower,
					ICESpeed = ratedSpeed,
					ICETorque = Genset.ElectricMotorCtl.EMTorque,
					FuelConsumption = fc,
					AvgEmDrivetrainSpeed = response.ElectricMotor.AngularVelocity,
					EMTorque = response.ElectricMotor.TorqueRequestEmMap
				};
				if (emDerated) {
					OptimalPoints.MaxPowerDeRated = tmp;
				} else {
					OptimalPoints.MaxPower = tmp;
				}
			}
		}

		private void IterateElectricPower(Volt voltage)
		{
			var maxPower = IceData.FullLoadCurves[0].MaxPower;

			var speedRange = IceData.FullLoadCurves[0].RatedSpeed - IceData.IdleSpeed;
			var maxSpeedNorm = (IceData.FullLoadCurves[0].N95hSpeed - IceData.IdleSpeed) / speedRange;

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var tolerance = 0.5 / 100;

			Genset.Battery?.Initialize(Genset.Battery.MinSoC);
			if (Genset.BatterySystem != null) {
				foreach (var bsKey in Genset.BatterySystem.Batteries.Keys) {
					for (var i = 0; i < Genset.BatterySystem.Batteries[bsKey].Batteries.Count; i++) {
						Genset.BatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(Genset.BatterySystem.MinSoC);
					}
				}
			}

			Genset.SuperCap?.Initialize(Genset.SuperCap.MinSoC);

			var stepsPwr = 0.05;
			for (var i = stepsPwr; i <= 1; i += stepsPwr) {
				var pwr = i * maxPower;
				var stepsSpeed = 0.05;
				var genPts = new List<GenSetOperatingPoint>();
				var responses = new List<Tuple<PerSecond, IResponse>>();
				for (var n = 0.0; n <= maxSpeedNorm; n += stepsSpeed) {
					var speed = n * speedRange + IceData.IdleSpeed;

					try {
						Genset.ElectricMotor.Initialize(0.SI<NewtonMeter>(), speed);
						var tq = Genset.ElectricMotor.GetTorqueForElectricPower(voltage, pwr, speed * EmData.RatioADC, dt, new GearshiftPosition(0), false);

						if (tq == null || tq.IsSmallerOrEqual(0)) {
							continue;
						}

						Genset.ElectricMotorCtl.EMTorque = tq;

						var response = Genset.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), speed);
						responses.Add(Tuple.Create(speed, response));
						if (response is ResponseSuccess) {
							var fc = IceData.Fuels.Sum(x =>
								x.ConsumptionMap.GetFuelConsumptionValue(response.Engine.TotalTorqueDemand,
									response.Engine.EngineSpeed));
							genPts.Add(new GenSetOperatingPoint() {
								ICEOn = true,
								ElectricPower = pwr,
								ICESpeed = speed,
								ICETorque = tq,
								FuelConsumption = fc,
								AvgEmDrivetrainSpeed = response.ElectricMotor.AngularVelocity,
								EMTorque = response.ElectricMotor.TorqueRequestEmMap
							});
						}
					} catch (Exception) { }
				}

				if (genPts.Any()) {
					var min = genPts.MinBy(x => x.FuelConsumption.Value()).FuelConsumption;
					OptimalPoints.OptimalPoints[pwr] = genPts.Where(x => x.FuelConsumption / min < (1 + tolerance)).ToList();
				}
			}
		}

		#endregion
	}
}
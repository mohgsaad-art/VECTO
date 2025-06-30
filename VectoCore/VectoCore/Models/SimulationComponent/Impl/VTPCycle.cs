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
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public interface IVTPCycle : IPWheelCycle {}

	internal class VTPCycle : PWheelCycle, IVTPCycle
	{
		protected uint StartGear;

		protected Second SimulationIntervalEndTime;

		public VTPCycle(IVehicleContainer container, IDrivingCycleData cycle) : base(container, cycle) { }

		public override IResponse Initialize()
		{
			PrepareCycleData();
			if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
				VerifyInputData();
			}
			SelectStartGear();
			return base.Initialize();
		}

		protected internal void PrepareCycleData()
		{
			Second timeDriftZero = Data.Entries.First().Time;
			Second timeDriftCheck = Data.Entries.Last().Time;
			
			foreach (var entry in Data.Entries) {
				var timeFactor = (entry.Time - timeDriftZero) / (timeDriftCheck - timeDriftZero);

				entry.TorqueWheelLeft = entry.TorqueWheelLeft - (RunData.TorqueDriftLeftWheel * timeFactor);
                entry.TorqueWheelRight = entry.TorqueWheelRight - (RunData.TorqueDriftRightWheel * timeFactor);

				var wheelSpeed = (entry.WheelSpeedLeft + entry.WheelSpeedRight) / 2;
				var wheelPower = entry.TorqueWheelLeft * entry.WheelSpeedLeft + entry.TorqueWheelRight * entry.WheelSpeedRight;
				entry.PWheel = wheelPower;
				entry.WheelAngularVelocity = wheelSpeed;
				entry.Torque = wheelSpeed.IsEqual(0, 1e-3) ? 0.SI<NewtonMeter>() : wheelPower / wheelSpeed;
			}
		}

		protected internal void VerifyInputData()
		{
			var electricFanTechs = DeclarationData.Fan.FullyElectricTechnologies();
			var hasElectricFan = RunData.Aux.Any(
				x => x.ID == Constants.Auxiliaries.IDs.Fan && x.Technology.Any(t => electricFanTechs.Contains(t)));

			foreach (var tuple in Data.Entries.Pairwise()) {
				if (!(tuple.Item2.Time - tuple.Item1.Time).IsEqual(
						DeclarationData.VTPMode.SamplingInterval, 0.1 * DeclarationData.VTPMode.SamplingInterval)) {
					Log.Error("Cycle Data exceeds expected sampling frequence of {0}: t: {1}, dt: {2}",
						DeclarationData.VTPMode.SamplingInterval, tuple.Item1.Time, tuple.Item2.Time - tuple.Item1.Time);
				}
			}

			foreach (var entry in Data.Entries) {
				VerifyWheelSpeeds(entry);

				VerifyFanSpeed(hasElectricFan, entry);
			}

			VerifyFCInput();
		}

		private void VerifyFCInput()
		{
			var idx = 0L;
			var count = Convert.ToInt32(DeclarationData.VTPMode.FCAccumulationWindow / DeclarationData.VTPMode.SamplingInterval);
			var sumFC= 0.SI<Kilogram>();
			var sumEWheel = 0.SI<WattSecond>();

			var window = new {FC = 0.SI<Kilogram>(), EWheel = 0.SI<WattSecond>()}.Repeat(count).ToArray();
			
			foreach (var entry in Data.Entries.Pairwise()) {
				var dt = entry.Item2.Time - entry.Item1.Time;
				var fc = entry.Item1.Fuelconsumption.Sum(x => x.Value) * dt;
				var eWheel = entry.Item1.PWheel > 0 ? entry.Item1.PWheel * dt : 0.SI<WattSecond>();
				//window[idx % count] = new {FC= fc, EWheel = eWheel};
				sumFC += window[idx % count].FC;
				sumFC -= window[(idx + 1) % count].FC;
				sumEWheel += window[idx % count].EWheel;
				sumEWheel -= window[(idx + 1) % count].EWheel;
				idx++;

				if (sumEWheel.IsSmaller(DeclarationData.VTPMode.MinPosWorkAtWheelsForFC)) {
					continue;
				}
				if (sumFC / sumEWheel < DeclarationData.VTPMode.LowerFCThreshold ) {
					Log.Error("Fuel consumption for the previous {0} [min] below threshold of {1} [g/kWh]. t: {2} [s], FC: {3} [g/kWh]", DeclarationData.VTPMode.FCAccumulationWindow.ConvertToMinutes(),
						DeclarationData.VTPMode.LowerFCThreshold.ConvertToGramPerKiloWattHour(), entry.Item1.Time, (sumFC / sumEWheel).ConvertToGramPerKiloWattHour());
				}
				if (sumFC / sumEWheel > DeclarationData.VTPMode.UpperFCThreshold) {
					Log.Error("Fuel consumption for the previous {0} [min] above threshold of {1} [g/kWh]. t: {2} [s], FC: {3} [g/kWh]", DeclarationData.VTPMode.FCAccumulationWindow.ConvertToMinutes(),
							DeclarationData.VTPMode.UpperFCThreshold.ConvertToGramPerKiloWattHour(), entry.Item1.Time, (sumFC / sumEWheel).ConvertToGramPerKiloWattHour());
				}
			}
		}

		private void VerifyFanSpeed(bool hasElectricFan, DrivingCycleData.DrivingCycleEntry entry)
		{
			if (entry.FanSpeed == null) {
				return;
			}

			if (hasElectricFan) {
				if (entry.FanSpeed.IsSmaller(0)) {
					Log.Error("Fan speed (electric) below zero! t: {0}, n_fan: {1}", entry.Time, entry.FanSpeed);
				}
			} else {
				if (entry.FanSpeed.IsSmaller(DeclarationData.VTPMode.MinFanSpeed) ||
					entry.FanSpeed.IsGreater(DeclarationData.VTPMode.MaxFanSpeed)) {
					Log.Error(
						"Fan speed (non-electric) exceeds range {0} < n_fan < {1}. t: {2}, n_fan: {3}", DeclarationData.VTPMode.MinFanSpeed,
						DeclarationData.VTPMode.MaxFanSpeed, entry.Time, entry.FanSpeed);
				}
			}
		}

		
		private void VerifyWheelSpeeds(DrivingCycleData.DrivingCycleEntry entry)
		{
			if (!entry.WheelSpeedLeft.IsEqual(0.RPMtoRad(), DeclarationData.VTPMode.WheelSpeedZeroTolerance) &&
				!entry.WheelSpeedRight.IsEqual(0.RPMtoRad(), DeclarationData.VTPMode.WheelSpeedZeroTolerance)) {
				var wheelSpeedRatio = VectoMath.Max(
					entry.WheelSpeedLeft / entry.WheelSpeedRight, entry.WheelSpeedRight / entry.WheelSpeedLeft);
				if (wheelSpeedRatio > DeclarationData.VTPMode.WheelSpeedDifferenceFactor) {
					Log.Error(
						"Wheel-speed difference rel. (L/R) too high! t: {0} n_left: {1} rpm, n_right: {2} rpm", entry.Time,
						entry.WheelSpeedLeft.AsRPM, entry.WheelSpeedRight.AsRPM);
				}
			} else {
				if (VectoMath.Abs(entry.WheelSpeedLeft - entry.WheelSpeedRight) >
					DeclarationData.VTPMode.MaxWheelSpeedDifferenceStandstill) {
					Log.Error(
						"Wheel-speed difference abs. (L/R) too high! t: {0} n_left: {1} rpm, n_right: {2} rpm", entry.Time,
						entry.WheelSpeedLeft.AsRPM, entry.WheelSpeedRight.AsRPM);
				}
			}
		}

		private void SelectStartGear()
		{
            if (RunData.GearboxData.Type.AutomaticTransmission()) {
                StartGear = 1;
                return;
            }
            var transmissionRatio = RunData.AxleGearData.AxleGear.Ratio *
									(RunData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
									RunData.VehicleData.DynamicTyreRadius;
			var cardanStartSpeed = (RunData.GearshiftParameters.StartSpeed * transmissionRatio).Cast<PerSecond>();
			var minEngineSpeed = (RunData.EngineData.FullLoadCurves[0].RatedSpeed - RunData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + RunData.EngineData.IdleSpeed;
			var wheelStartTorque =
				(RunData.VehicleData.VehicleCategory == VehicleCategory.Tractor
					? 40000.SI<Kilogram>()
					: RunData.VehicleData.GrossVehicleMass) * RunData.GearshiftParameters.StartAcceleration *
				RunData.VehicleData.DynamicTyreRadius;
			var wheelStartSpeed = RunData.GearshiftParameters.StartSpeed / RunData.VehicleData.DynamicTyreRadius;
			CycleIterator.LeftSample.WheelAngularVelocity = wheelStartSpeed;
			var maxStartGear = 1u;
			foreach (var gearData in RunData.GearboxData.Gears.Reverse())
				if (cardanStartSpeed * gearData.Value.Ratio > minEngineSpeed) {
					maxStartGear = gearData.Key;
					break;
				}
			for (var gear = maxStartGear; gear > 1; gear--) {
				var inAngularSpeed = cardanStartSpeed * RunData.GearboxData.Gears[gear].Ratio;

				var ratedSpeed = DataBus.EngineInfo.EngineRatedSpeed;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0))
					continue;

				var response = Initialize(gear, wheelStartTorque, wheelStartSpeed);

				var fullLoadPower = response.Engine.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = fullLoadPower.IsEqual(0) ? 0.SI<Scalar>() : 1 - response.Engine.PowerRequest / fullLoadPower;

				if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineIdleSpeed && reserve >= RunData.GearshiftParameters.StartTorqueReserve) {
					StartGear = gear;
					return;
				}
			}
			StartGear = 1;
		}

		internal ResponseDryRun Initialize(uint gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			CycleIterator.LeftSample.Gear = gear;
			//var inAngularVelocity = outAngularVelocity * RunData.GearboxData.Gears[gear].Ratio;
			//var torqueLossResult = RunData.GearboxData.Gears[gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			//var inTorque = outTorque / RunData.GearboxData.Gears[gear].Ratio + torqueLossResult.Value;

			var response =
				(ResponseDryRun)
				NextComponent.Request(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, outTorque,
					outAngularVelocity, true);

			//var fullLoad = DataBus.EngineStationaryFullPower(inAngularVelocity);

			return new ResponseDryRun(this) {
				Engine = {
					PowerRequest = response.Engine.PowerRequest,
					EngineSpeed = response.Engine.EngineSpeed,
					DynamicFullLoadPower = response.Engine.DynamicFullLoadPower,
				},
				Clutch = {
					PowerRequest = response.Clutch.PowerRequest,
				},
				Gearbox = {
					PowerRequest = outTorque * outAngularVelocity,
				}
				//DeltaFullLoad = response.EnginePowerRequest - fullLoad
			};
		}

		protected override void InitializeCycleData()
		{
			FirstRun = false;
			var minEngineSpeed = (RunData.EngineData.FullLoadCurves[0].RatedSpeed - RunData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + RunData.EngineData.IdleSpeed;

			var gearRatios = RunData.GearboxData.Gears.ToDictionary(g => g.Key, g => g.Value.Ratio);

			var stopped = false;
			var hasATGbx = RunData.GearboxData.TorqueConverterData != null && RunData.GearboxData.Type.AutomaticTransmission();

			foreach (var entry in Data.Entries) {
				stopped = stopped || entry.VehicleTargetSpeed.IsEqual(0.KMPHtoMeterPerSecond(),
							0.3.KMPHtoMeterPerSecond());
				entry.AngularVelocity =
					entry.VehicleTargetSpeed.IsEqual(0.KMPHtoMeterPerSecond(), 0.3.KMPHtoMeterPerSecond())
						? 0.RPMtoRad()
						: entry.WheelAngularVelocity;

				var cardanSpeed = entry.WheelAngularVelocity *
								RunData.AxleGearData.AxleGear.Ratio * (RunData.AngledriveData?.Angledrive.Ratio ?? 1);
				if (cardanSpeed.IsEqual(0.RPMtoRad(), 1.RPMtoRad()) || entry.AngularVelocity.IsEqual(0.RPMtoRad(), 1.RPMtoRad())) {
					entry.Gear = 0;
					continue;
				}

				if (hasATGbx && entry.TorqueConverterActive != null && entry.TorqueConverterActive.Value) {
					continue;
				}

				var ratio = (entry.EngineSpeed / cardanSpeed).Value();
				var gear = gearRatios.Aggregate((x, y) =>
					Math.Abs(ratio/x.Value   - 1) < Math.Abs(ratio/y.Value - 1) ? x : y).Key;
				while (gear > 0 && cardanSpeed * gearRatios[gear] < RunData.EngineData.IdleSpeed)
					gear--;

				//entry.Gear = entry.EngineSpeed < (RunData.EngineData.IdleSpeed + 50.RPMtoRad()) && entry.VehicleTargetSpeed < 5.KMPHtoMeterPerSecond() ? 0 :  gear;
				if (stopped && gear <= StartGear) {
					entry.Gear = entry.VehicleTargetSpeed.IsEqual(0.KMPHtoMeterPerSecond(),
						0.3.KMPHtoMeterPerSecond())
						? 0
						: StartGear;
				} else {
					entry.Gear = gear == 1 && cardanSpeed * gearRatios[1] <= minEngineSpeed ? 0 : gear;
				}
				if (gear > StartGear)
					stopped = false;
			}
		}

		public override IResponse Request(Second absTime, Second dt)
		{
			if (CycleIterator.LastEntry && CycleIterator.RightSample.Time == absTime) {
				return new ResponseCycleFinished(this);
			}

			// interval exceeded
			if (CycleIterator.RightSample != null && (absTime + dt).IsGreater(CycleIterator.RightSample.Time)) {
				return new ResponseFailTimeInterval(this) {
					AbsTime = absTime,
					DeltaT = CycleIterator.RightSample.Time - absTime
				};
			}

			SimulationIntervalEndTime = absTime + dt;
			if (CycleIterator.LeftSample.Time > absTime) {
				Log.Warn("absTime: {0} cycle: {1}", absTime, CycleIterator.LeftSample.Time);
			}
			var tmp = NextComponent.Initialize(CycleIterator.LeftSample.Torque, CycleIterator.LeftSample.WheelAngularVelocity);

			return DoHandleRequest(absTime, dt, CycleIterator.LeftSample.WheelAngularVelocity);
		}

		public override bool VehicleStopped =>
			CycleIterator.Previous().LeftSample.VehicleTargetSpeed
				.IsEqual(0.KMPHtoMeterPerSecond(), 0.3.KMPHtoMeterPerSecond());

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			if (SimulationIntervalEndTime.IsGreaterOrEqual(CycleIterator.RightSample.Time)) {
				CycleIterator.MoveNext();
			}
			AdvanceState();
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			base.DoWriteModalResults(time, simulationInterval, container);
			container[ModalResultField.P_wheel_in] = CurrentState.InTorque * CurrentState.InAngularVelocity;
			container[ModalResultField.v_act] = CycleIterator.LeftSample.VehicleTargetSpeed;
		}
	}
}
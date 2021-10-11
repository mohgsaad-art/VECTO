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
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	public enum PCCStates
	{
		OutsideSegment,
		WithinSegment,
		UseCase1,
		UseCase2,
		PCCinterrupt
	}

	public class DefaultDriverStrategy : LoggingObject, IDriverStrategy
	{
		public static readonly SIBase<Meter> BrakingSafetyMargin = 0.1.SI<Meter>();

		protected internal DrivingBehaviorEntry NextDrivingAction;

		public enum DrivingMode
		{
			DrivingModeDrive,
			DrivingModeBrake,
		}

		protected internal DrivingMode CurrentDrivingMode;

		protected readonly Dictionary<DrivingMode, IDriverMode> DrivingModes = new Dictionary<DrivingMode, IDriverMode>();
		protected Second VehicleHaltTimestamp;
		protected Second EngineOffTimestamp;
		private VehicleData.ADASData ADAS;

		public readonly MeterPerSecond PTODriveMinSpeed;

		protected EcoRoll EcoRollState;
		protected PCCSegments PCCSegments;

		public PCCStates PCCState => _PCCState;
		protected internal PCCStates _PCCState = PCCStates.OutsideSegment;
		protected bool ATEcoRollReleaseLockupClutch;


		public DefaultDriverStrategy(IVehicleContainer container)
		{
			PTODriveMinSpeed = container.RunData.DriverData.PTODriveMinSpeed;
			DrivingModes.Add(DrivingMode.DrivingModeDrive, new DriverModeDrive() { DriverStrategy = this });
			DrivingModes.Add(DrivingMode.DrivingModeBrake, new DriverModeBrake() { DriverStrategy = this });
			CurrentDrivingMode = DrivingMode.DrivingModeDrive;

			VehicleCategory = container.RunData.VehicleData.VehicleCategory;

			var data = container.RunData;
			ADAS = data?.VehicleData?.ADAS ?? new VehicleData.ADASData() {
				EcoRoll = EcoRollType.None,
				EngineStopStart = false,
				PredictiveCruiseControl = PredictiveCruiseControlType.None,
			};
			ATEcoRollReleaseLockupClutch = data?.GearboxData?.ATEcoRollReleaseLockupClutch ?? false;

			EcoRollState = new EcoRoll {
				State = EcoRollStates.EcoRollOff,
				Gear = new GearshiftPosition(0),
				StateChangeTstmp = -double.MaxValue.SI<Second>(),
				PreviousBrakePower = 0.SI<Watt>(),
				AcceleratorPedalIdle = false,
			};

			PCCSegments = new PCCSegments();

			if (ADAS.PredictiveCruiseControl != PredictiveCruiseControlType.None) {
				// create a dummy powertrain for pre-processing and estimations
				var modData = new ModalDataContainer(data, null, null);
				var builder = new PowertrainBuilder(modData);
				var testContainer = new SimplePowertrainContainer(data);
				//if (data.JobType != VectoSimulationJobType.BatteryElectricVehicle)
					builder.BuildSimplePowertrain(data, testContainer);
				//else {
				//	builder.BuildSimplePowertrainElectric(data, testContainer);
				//}

				container.AddPreprocessor(new PCCSegmentPreprocessor(testContainer, PCCSegments, data?.DriverData.PCC));
			}
		}

		public VehicleCategory VehicleCategory { get; set; }

		public IDriverActions Driver { get; set; }

		protected internal DrivingBehaviorEntry BrakeTrigger { get; set; }

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			var retVal = DoHandleRequest(absTime, ds, targetVelocity, gradient);
			if (retVal is ResponseSuccess) {
				EcoRollState.PreviousBrakePower = Driver.DataBus.BrakePower;
				if (retVal.Source is ICombustionEngine) {
					var success = retVal as ResponseSuccess;
					var avgEngineSpeed = (success.EngineSpeed + Driver.DataBus.EngineSpeed) / 2.0;
					EcoRollState.AcceleratorPedalIdle = success.DragPower.IsEqual(success.Engine.TotalTorqueDemand * avgEngineSpeed, 10.SI<Watt>());
				} else {
					EcoRollState.AcceleratorPedalIdle = false;
				}
			}
			return retVal;
		}


		public IResponse Request(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient)
		{
			Driver.DriverBehavior = DrivingBehavior.Halted;
			CurrentDrivingMode = DrivingMode.DrivingModeDrive;
			if (ADAS.EngineStopStart) {
				HandleEngineStopStartDuringVehicleStop(absTime);
			}

			var retVal = Driver.DrivingActionHalt(
				absTime, dt, VectoMath.Min(Driver.DataBus.VehicleInfo.MaxVehicleSpeed, targetVelocity), gradient);
			EcoRollState.PreviousBrakePower = Driver.DataBus.Brakes.BrakePower;
			return retVal;
		}

		public void WriteModalResults(IModalDataContainer container)
		{
			container.SetDataValue("EcoRollConditionsMet", EcoRollState.AllConditionsMet ? 1 : 0);
			if (PCCSegments.Count > 0) {
				var val = 0;
				if (Driver.DataBus.MileageCounter.Distance > PCCSegments.Current.EndDistance) {
					val = 0;
				} else if (Driver.DataBus.MileageCounter.Distance > PCCSegments.Current.DistanceMinSpeed) {
					val = 5;
				} else if (Driver.DataBus.MileageCounter.Distance > PCCSegments.Current.StartDistance) {
					val = -5;
				}
				container.SetDataValue("PCCSegment", val);
				container.SetDataValue("PCCState", (int)_PCCState);
			} else {
				container.SetDataValue("PCCSegment", 0);
				container.SetDataValue("PCCState", (int)_PCCState);
			}
		}

		public void CommitSimulationStep()
		{
			if (PCCSegments.Count > 0) {
				if (Driver.DataBus.MileageCounter.Distance > PCCSegments.Current.EndDistance) {
					PCCSegments.MoveNext();
					_PCCState = PCCStates.OutsideSegment;
				}
			}
		}

		protected virtual IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			VehicleHaltTimestamp = null;

			if (ADAS.PredictiveCruiseControl != PredictiveCruiseControlType.None) {
				HandlePCC(absTime, targetVelocity);
			}
			if (ADAS.EcoRoll != EcoRollType.None &&
				(_PCCState == PCCStates.OutsideSegment || _PCCState == PCCStates.WithinSegment)
			) {
				HandleEcoRoll(absTime, targetVelocity);
			}

			//if (ADAS.EcoRoll != EcoRollType.None) {
			// todo MQ: keep something like this to prevent driver to turn on engine in every timestep (in combination with hybrids leads to errors!)
			if (EcoRollState.State != EcoRollStates.EcoRollOn && _PCCState != PCCStates.UseCase1 &&
				_PCCState != PCCStates.UseCase2) {
				EngineOffTimestamp = null;
				//if (Driver.DataBus.PowertrainInfo.HasCombustionEngine && !Driver.DataBus.PowertrainInfo.HasElectricMotor) {
				Driver.DataBus.EngineCtl.CombustionEngineOn = true;
				//}
			}
			//}

			if (CurrentDrivingMode == DrivingMode.DrivingModeBrake) {
				if (Driver.DataBus.Distance.IsGreaterOrEqual(BrakeTrigger.TriggerDistance, 1e-3.SI<Meter>())) {
					CurrentDrivingMode = DrivingMode.DrivingModeDrive;
					NextDrivingAction = null;
					DrivingModes[CurrentDrivingMode].ResetMode();
					Log.Debug("Switching to DrivingMode DRIVE");
				}
			}
			if (CurrentDrivingMode == DrivingMode.DrivingModeDrive) {
				var currentDistance = Driver.DataBus.Distance;

				//var coasting = LookAheadCoasting(ds);

				UpdateDrivingAction(currentDistance, ds);
				if (NextDrivingAction != null) {
					var remainingDistance = NextDrivingAction.ActionDistance - currentDistance;
					var estimatedTimestep = remainingDistance / Driver.DataBus.VehicleSpeed;

					var atTriggerTistance = remainingDistance.IsEqual(
						0.SI<Meter>(), Constants.SimulationSettings.DriverActionDistanceTolerance);
					var closeBeforeBraking = estimatedTimestep.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval);
					var brakingIntervalTooShort = NextDrivingAction.Action == DrivingBehavior.Braking &&
												NextDrivingAction.ActionDistance.IsSmaller(currentDistance + ds) &&
												((NextDrivingAction.TriggerDistance - NextDrivingAction.ActionDistance) / Driver.DataBus.VehicleSpeed)
												.IsSmaller(
													Constants.SimulationSettings.LowerBoundTimeInterval / 2) && (Driver.DataBus.GearboxType.AutomaticTransmission() || !Driver.DataBus.ClutchClosed(absTime));
					var brakingIntervalShort = NextDrivingAction.Action == DrivingBehavior.Braking &&
												NextDrivingAction.ActionDistance.IsSmaller(currentDistance + ds) &&
												((NextDrivingAction.TriggerDistance - NextDrivingAction.ActionDistance) / Driver.DataBus.VehicleSpeed)
												.IsSmaller(
													Constants.SimulationSettings.LowerBoundTimeInterval / 2) && (Driver.DataBus.GearboxType.AutomaticTransmission() || !Driver.DataBus.ClutchClosed(absTime));
					if (brakingIntervalShort && remainingDistance.IsEqual(ds)) {
						return new ResponseDrivingCycleDistanceExceeded(this) {
							MaxDistance = ds / 2
						};
					}
					if (atTriggerTistance || closeBeforeBraking || brakingIntervalTooShort) {
						CurrentDrivingMode = DrivingMode.DrivingModeBrake;
						DrivingModes[CurrentDrivingMode].ResetMode();
						Log.Debug("Switching to DrivingMode BRAKE");

						BrakeTrigger = NextDrivingAction;

						//break;
					} else if ((currentDistance + ds).IsGreater(NextDrivingAction.ActionDistance)) {
						Log.Debug(
							"Current simulation interval exceeds next action distance at {0}. reducing maxDistance to {1}",
							NextDrivingAction.ActionDistance, NextDrivingAction.ActionDistance - currentDistance);
						return new ResponseDrivingCycleDistanceExceeded(this) {
							MaxDistance = NextDrivingAction.ActionDistance - currentDistance
						};
					}
				}
			}

			var retVal = DrivingModes[CurrentDrivingMode].Request(
				absTime, ds, VectoMath.Min(Driver.DataBus.MaxVehicleSpeed, targetVelocity), gradient);

			return retVal;
		}

		private void HandlePCC(Second absTime, MeterPerSecond targetVelocity)
		{
			var dataBus = Driver.DataBus;
			var vehicleSpeed = dataBus.VehicleInfo.VehicleSpeed;

			UpdatePCCState(targetVelocity);

			switch (_PCCState) {
				case PCCStates.UseCase1:
					if (vehicleSpeed <= targetVelocity - Driver.DriverData.PCC.UnderSpeed * 1.05) {
						_PCCState = PCCStates.PCCinterrupt;
					}
					if (vehicleSpeed >= targetVelocity + 1.KMPHtoMeterPerSecond()) {
						_PCCState = PCCStates.WithinSegment;
					}
					break;
				case PCCStates.UseCase2:
					if (vehicleSpeed < Driver.DriverData.PCC.MinSpeed || vehicleSpeed > targetVelocity + 1.KMPHtoMeterPerSecond()) {
						_PCCState = PCCStates.WithinSegment;
					}
					break;
				case PCCStates.PCCinterrupt:
					if (vehicleSpeed >= targetVelocity - Driver.DriverData.PCC.UnderSpeed * 0.95) {
						_PCCState = PCCStates.UseCase1;
					}
					break;
			}

			switch (_PCCState) {
				case PCCStates.UseCase1:
				case PCCStates.UseCase2:
					switch (ADAS.EcoRoll) {
						case EcoRollType.None:
							break;
						case EcoRollType.WithoutEngineStop:
							dataBus.GearboxCtl.DisengageGearbox = true;
							break;
						case EcoRollType.WithEngineStop:
							dataBus.GearboxCtl.DisengageGearbox = true;
							if (dataBus.EngineCtl != null) {
								dataBus.EngineCtl.CombustionEngineOn = false;
							}
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					break;
				case PCCStates.OutsideSegment:
				case PCCStates.WithinSegment:
				case PCCStates.PCCinterrupt:
					if (dataBus.GearboxCtl != null) {
						dataBus.GearboxCtl.DisengageGearbox = false;
					}

					if (dataBus.EngineCtl != null) {
						dataBus.EngineCtl.CombustionEngineOn = true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdatePCCState(MeterPerSecond targetVelocity)
		{
			// check if within PCC segment and update state ----------------------------
			var distance = Driver.DataBus.MileageCounter.Distance;
			var withinPCCSegment = PCCSegments.Current != null
									&& distance.IsBetween(PCCSegments.Current.StartDistance, PCCSegments.Current.EndDistance);

			if (!withinPCCSegment) {
				_PCCState = PCCStates.OutsideSegment;
				return;
			}
			if (_PCCState == PCCStates.OutsideSegment) {
				_PCCState = PCCStates.WithinSegment;
			}
			if (_PCCState != PCCStates.WithinSegment)
				return;

			// check for PCC usecase 1 -------------------------------------------------
			var endUseCase1 = PCCSegments.Current.EndDistance;
			var endEnergyUseCase1 = PCCSegments.Current.EnergyEnd;
			if ((distance + Driver.DriverData.PCC.PreviewDistanceUseCase1).IsSmallerOrEqual(endUseCase1)) {
				endUseCase1 = distance + Driver.DriverData.PCC.PreviewDistanceUseCase1;
				var endCycleEntry = Driver.DataBus.DrivingCycleInfo.CycleLookAhead(Driver.DriverData.PCC.PreviewDistanceUseCase1);
				endEnergyUseCase1 = CalculateEnergy(endCycleEntry.Altitude, endCycleEntry.VehicleTargetSpeed, Driver.DataBus.VehicleInfo.TotalMass);
			}
			var vehicleSpeed = Driver.DataBus.VehicleInfo.VehicleSpeed;
			var coastingForce = CoastingForce(targetVelocity, vehicleSpeed);
			var energyCoastingEndUseCase1 = (coastingForce * (endUseCase1 - distance)).Cast<Joule>();
			var currentEnergy = CalculateEnergy(Driver.DataBus.DrivingCycleInfo.Altitude, vehicleSpeed, Driver.DataBus.VehicleInfo.TotalMass);
			var energyCoastingLow = (coastingForce * (PCCSegments.Current.DistanceMinSpeed - distance)).Cast<Joule>();

			var beforeVLow = distance.IsSmaller(PCCSegments.Current.DistanceMinSpeed);
			var speedSufficient = vehicleSpeed.IsGreaterOrEqual(targetVelocity - Driver.DriverData.PCC.UnderSpeed);
			var currentEnergyHigherThanEndUseCase1 = currentEnergy.IsGreaterOrEqual(endEnergyUseCase1 + energyCoastingEndUseCase1);
			var currentEnergyHigherThanMin = currentEnergy.IsGreaterOrEqual(PCCSegments.Current.EnergyMinSpeed + energyCoastingLow);

			if (beforeVLow
				&& speedSufficient
				&& currentEnergyHigherThanEndUseCase1
				&& currentEnergyHigherThanMin) {
				_PCCState = PCCStates.UseCase1;
			}

			// check for PCC use case 2 ------------------------------------------------
			var endUseCase2 = PCCSegments.Current.EndDistance;
			var endEnergyUseCase2 = PCCSegments.Current.EnergyEnd;
			if ((distance + Driver.DriverData.PCC.PreviewDistanceUseCase2).IsSmallerOrEqual(endUseCase1)) {
				endUseCase2 = distance + Driver.DriverData.PCC.PreviewDistanceUseCase2;
				var endCycleEntry = Driver.DataBus.DrivingCycleInfo.CycleLookAhead(Driver.DriverData.PCC.PreviewDistanceUseCase2);
				endEnergyUseCase2 = CalculateEnergy(endCycleEntry.Altitude, endCycleEntry.VehicleTargetSpeed, Driver.DataBus.VehicleInfo.TotalMass);
			}
			var energyCoastingEndUseCase2 = (coastingForce * (endUseCase2 - distance)).Cast<Joule>();

			var beyondVLow = distance.IsGreaterOrEqual(PCCSegments.Current.DistanceMinSpeed);
			var currentEnergyHigherThanEndUseCase2 = currentEnergy.IsGreaterOrEqual(endEnergyUseCase2 + energyCoastingEndUseCase2);
			var speedSufficientUseCase2 = vehicleSpeed.IsGreaterOrEqual(VectoMath.Max(targetVelocity - Driver.DriverData.PCC.UnderSpeed, Driver.DriverData.PCC.MinSpeed));
			var speedBelowTargetspeed = vehicleSpeed.IsSmallerOrEqual(targetVelocity - 1.KMPHtoMeterPerSecond());

			if (beyondVLow
				&& currentEnergyHigherThanEndUseCase2
				&& speedSufficientUseCase2
				&& speedBelowTargetspeed) {
				_PCCState = PCCStates.UseCase2;
			}
		}

		private Newton CoastingForce(MeterPerSecond targetVelocity, MeterPerSecond vehicleSpeed)
		{
			var dataBus = Driver.DataBus;
			var airDragForce = Driver.DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, targetVelocity);
			var rollResistanceForce = Driver.DataBus.VehicleInfo.RollingResistance(dataBus.DrivingCycleInfo.RoadGradient);
			var engineDragLoss = 0.SI<Watt>();
			if (dataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
				if (ADAS.EcoRoll == EcoRollType.None && ATEcoRollReleaseLockupClutch) {
					engineDragLoss = Driver.DataBus.EngineInfo.EngineDragPower(Driver.DataBus.EngineInfo.EngineSpeed);
				}
			} else {
				if (ADAS.EcoRoll == EcoRollType.None) {
					engineDragLoss = Driver.DataBus.EngineInfo.EngineDragPower(Driver.DataBus.EngineInfo.EngineSpeed);
				}
			}

			var gearboxLoss = Driver.DataBus.GearboxInfo.GearboxLoss();
			var axleLoss = Driver.DataBus.AxlegearInfo.AxlegearLoss();

			var coastingResistanceForce = airDragForce + rollResistanceForce + (gearboxLoss + axleLoss - engineDragLoss) / vehicleSpeed;
			return coastingResistanceForce;
		}

		private Joule CalculateEnergy(Meter altitude, MeterPerSecond velocity, Kilogram mass) =>
			(mass * Physics.GravityAccelleration * altitude).Cast<Joule>() + mass * velocity * velocity / 2;

		private void HandleEcoRoll(Second absTime, MeterPerSecond targetVelocity)
		{
			var dBus = Driver.DataBus;
			var vehicleSpeedAboveLowerThreshold = dBus.VehicleInfo.VehicleSpeed >= Driver.DriverData.EcoRoll.MinSpeed;
			var slopeNegative = dBus.DrivingCycleInfo.RoadGradient.IsSmaller(0);
			// potential optimization...
			//if (EcoRollState.State != EcoRollStates.EcoRollOn && !slopeNegative) {
			//	EcoRollState.State = EcoRollStates.EcoRollOff;
			//	return;
			//}
			var forces = dBus.VehicleInfo.SlopeResistance(dBus.DrivingCycleInfo.RoadGradient) + dBus.VehicleInfo.RollingResistance(dBus.DrivingCycleInfo.RoadGradient) +
						dBus.VehicleInfo.AirDragResistance(dBus.VehicleInfo.VehicleSpeed, dBus.VehicleInfo.VehicleSpeed);

			if (dBus.GearboxInfo.GearboxType.AutomaticTransmission() && ATEcoRollReleaseLockupClutch && dBus.VehicleInfo.VehicleSpeed.IsGreater(0)) {
				// for AT transmissions consider engine drag losses during eco-roll events
				forces -= dBus.EngineInfo.EngineDragPower(dBus.EngineInfo.EngineSpeed) / dBus.VehicleInfo.VehicleSpeed;
				forces += (dBus.GearboxInfo.GearboxLoss() + dBus.AxlegearInfo.AxlegearLoss()) / dBus.VehicleInfo.VehicleSpeed;
			}
			var accelerationWithinLimits = (-forces / dBus.VehicleInfo.TotalMass).IsBetween(
				Driver.DriverData.EcoRoll.AccelerationLowerLimit, Driver.DriverData.EcoRoll.AccelerationUpperLimit);
			var accelerationPedalIdle = EcoRollState.AcceleratorPedalIdle;
			var brakeActive = !EcoRollState.PreviousBrakePower.IsEqual(0);
			var vehcleSpeedBelowMax = dBus.VehicleInfo.VehicleSpeed <=
									(ApplyOverspeed(dBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed) - 2.KMPHtoMeterPerSecond());

			EcoRollState.AllConditionsMet = vehicleSpeedAboveLowerThreshold && vehcleSpeedBelowMax && slopeNegative && accelerationWithinLimits &&
											accelerationPedalIdle && !brakeActive;

			EcoRollState.Gear = dBus.GearboxInfo.Gear;
			switch (EcoRollState.State) {
				case EcoRollStates.EcoRollOff:
					if (EcoRollState.AllConditionsMet) {
						EcoRollState.State = EcoRollStates.PreActivation;
						EcoRollState.StateChangeTstmp = absTime;
					}
					break;
				case EcoRollStates.PreActivation:
					if (!EcoRollState.AllConditionsMet) {
						EcoRollState.State = EcoRollStates.EcoRollOff;
						EcoRollState.StateChangeTstmp = absTime;
						break;
					}

					if (absTime - EcoRollState.StateChangeTstmp > Driver.DriverData.EcoRoll.ActivationPhaseDuration) {
						EcoRollState.State = EcoRollStates.EcoRollOn;
						EcoRollState.StateChangeTstmp = absTime;
					}
					break;
				case EcoRollStates.EcoRollOn:
					var belowTargetSpeed = dBus.VehicleInfo.VehicleSpeed.IsSmaller(targetVelocity - Driver.DriverData.EcoRoll.UnderspeedThreshold);
					if (belowTargetSpeed || brakeActive) {
						EcoRollState.State = EcoRollStates.EcoRollOff;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (EcoRollState.State) {
				case EcoRollStates.EcoRollOn:
					dBus.GearboxCtl.DisengageGearbox = true;
					if (ADAS.EcoRoll == EcoRollType.WithEngineStop) {
						dBus.EngineCtl.CombustionEngineOn = false;
					}
					return;
				case EcoRollStates.EcoRollOff:
					dBus.GearboxCtl.DisengageGearbox = false;
					if (ADAS.EcoRoll == EcoRollType.WithEngineStop) {
						dBus.EngineCtl.CombustionEngineOn = true;
					}
					return;
			}

			EngineOffTimestamp = null;
			dBus.EngineCtl.CombustionEngineOn = true;
		}


		private void HandleEngineStopStartDuringVehicleStop(Second absTime)
		{
			if (Driver.DataBus.DrivingCycleInfo.CycleData.LeftSample.PTOActive != PTOActivity.Inactive) {
				// engine stop start is disabled for stops where the PTO is activated
				return;
			}

			if (VehicleHaltTimestamp == null) {
				VehicleHaltTimestamp = absTime;
			}

			if ((absTime - VehicleHaltTimestamp).IsGreaterOrEqual(
				Driver.DriverData.EngineStopStart.EngineOffStandStillActivationDelay)) {
				if (EngineOffTimestamp == null) {
					EngineOffTimestamp = absTime;
					Driver.DataBus.EngineCtl.CombustionEngineOn = false;
				}
			}
			if (EngineOffTimestamp != null &&
				(absTime - EngineOffTimestamp).IsGreaterOrEqual(Driver.DriverData.EngineStopStart.MaxEngineOffTimespan)) {
				Driver.DataBus.EngineCtl.CombustionEngineOn = true;
			}
		}


		private void UpdateDrivingAction(Meter currentDistance, Meter ds)
		{
			var nextAction = GetNextDrivingAction(ds);
			if (NextDrivingAction == null) {
				if (nextAction != null) {
					// take the new action
					NextDrivingAction = nextAction;
				}
			} else {
				// update action distance for current 'next action'
				UpdateDistancesForCurrentNextAction();

				SetNextDrivingAction(currentDistance, nextAction);
			}
			Log.Debug("Next Driving Action: {0}", NextDrivingAction);
		}

		private void SetNextDrivingAction(Meter currentDistance, DrivingBehaviorEntry nextAction)
		{
			if (nextAction != null) {
				if (nextAction.HasEqualTrigger(NextDrivingAction)) {
					// if the action changes and the vehicle has not yet exceeded the action distance => update the action
					// otherwise do nothing, NextDrivingAction's action distance has already been updated
					if (nextAction.Action != NextDrivingAction.Action && nextAction.ActionDistance > currentDistance) {
						NextDrivingAction = nextAction;
					}
				} else {
					// hmm, we've got a new action that is closer to what we got before?
					if (nextAction.ActionDistance < NextDrivingAction.ActionDistance) {
						NextDrivingAction = nextAction;
					}
				}
			} else {
				NextDrivingAction = null;
			}
		}

		private void UpdateDistancesForCurrentNextAction()
		{
			if (Driver.DataBus.VehicleSpeed > NextDrivingAction.NextTargetSpeed) {
				var brakingDistance = Driver.ComputeDecelerationDistance(NextDrivingAction.NextTargetSpeed) + BrakingSafetyMargin;
				switch (NextDrivingAction.Action) {
					case DrivingBehavior.Coasting:

						//var coastingDistance = ComputeCoastingDistance(Driver.DataBus.VehicleSpeed, NextDrivingAction.NextTargetSpeed);
						var coastingDistance = ComputeCoastingDistance(Driver.DataBus.VehicleSpeed, NextDrivingAction.CycleEntry);
						NextDrivingAction.CoastingStartDistance = NextDrivingAction.TriggerDistance - coastingDistance;
						NextDrivingAction.BrakingStartDistance = NextDrivingAction.TriggerDistance - brakingDistance;
						break;
					case DrivingBehavior.Braking:
						NextDrivingAction.BrakingStartDistance = NextDrivingAction.TriggerDistance - brakingDistance;
						NextDrivingAction.CoastingStartDistance = double.MaxValue.SI<Meter>();
						break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed)
		{
			return (targetSpeed + GetOverspeed()).LimitTo(
				0.KMPHtoMeterPerSecond(), /*VehicleCategory.IsBus() ? Constants.BusParameters.MaxBusSpeed :*/ 500.KMPHtoMeterPerSecond());

		}

		protected internal MeterPerSecond GetOverspeed()
		{
			return ADAS.PredictiveCruiseControl == PredictiveCruiseControlType.Option_1_2_3 && Driver.DataBus.DrivingCycleInfo.CycleData.LeftSample.Highway
				? Driver.DriverData.PCC.OverspeedUseCase3
				: Driver.DriverData.OverSpeed.OverSpeed;
		}

		protected internal DrivingBehaviorEntry GetNextDrivingAction(Meter ds)
		{
			var currentSpeed = Driver.DataBus.VehicleSpeed;

			var lookaheadDistance =
				(currentSpeed.Value() * 3.6 * Driver.DriverData.LookAheadCoasting.LookAheadDistanceFactor).SI<Meter>();
			var stopDistance = Driver.ComputeDecelerationDistance(0.SI<MeterPerSecond>());
			lookaheadDistance = VectoMath.Max(2 * ds, lookaheadDistance, 1.2 * stopDistance + ds);
			var lookaheadData = Driver.DataBus.LookAhead(lookaheadDistance);

			Log.Debug("Lookahead distance: {0} @ current speed {1}", lookaheadDistance, currentSpeed);
			var nextActions = new List<DrivingBehaviorEntry>();
			foreach (var entry in lookaheadData) {
				var nextTargetSpeed = OverspeedAllowed(entry.VehicleTargetSpeed)
					? ApplyOverspeed(entry.VehicleTargetSpeed)
					: entry.VehicleTargetSpeed;
				if (nextTargetSpeed >= currentSpeed) {
					// acceleration is not relevant
					continue;
				}

				nextActions.Add(GetDrivingBehaviorEntry(nextTargetSpeed, currentSpeed, entry));
			}

			if (!nextActions.Any()) {
				return null;
			}

			var nextBrakingAction = nextActions.OrderBy(x => x.BrakingStartDistance).First();
			var nextCoastingAction = nextActions.OrderBy(x => x.CoastingStartDistance).First();

			return nextBrakingAction.TriggerDistance.IsEqual(nextCoastingAction.TriggerDistance)
				? nextCoastingAction
				: nextBrakingAction;

			// MQ: 27.5.2016 remark: one could set the coasting distance to the closest coasting distance as found above to start coasting a little bit earlier.
		}

		private DrivingBehaviorEntry GetDrivingBehaviorEntry(
			MeterPerSecond nextTargetSpeed, MeterPerSecond currentSpeed,
			DrivingCycleData.DrivingCycleEntry entry)
		{
			var action = DrivingBehavior.Braking;

			var brakingDistance = Driver.ComputeDecelerationDistance(nextTargetSpeed) + BrakingSafetyMargin;
			var coastingDistance = ComputeCoastingDistance(currentSpeed, entry);
			if (!Driver.DriverData.LookAheadCoasting.Enabled || coastingDistance < 0) {
				Log.Debug(
					"adding 'Braking' starting at distance {0}. brakingDistance: {1}, triggerDistance: {2}, nextTargetSpeed: {3}",
					entry.Distance - brakingDistance, brakingDistance, entry.Distance, nextTargetSpeed);
				coastingDistance = brakingDistance;
			} else {
				//var coastingDistance = ComputeCoastingDistance(currentSpeed, nextTargetSpeed);
				if (currentSpeed > Driver.DriverData.LookAheadCoasting.MinSpeed) {
					action = DrivingBehavior.Coasting;

					Log.Debug(
						"adding 'Coasting' starting at distance {0}. coastingDistance: {1}, triggerDistance: {2}, nextTargetSpeed: {3}",
						entry.Distance - coastingDistance, coastingDistance, entry.Distance, nextTargetSpeed);
				} else {
					coastingDistance = -1.SI<Meter>();
				}
			}
			var nextEntry = new DrivingBehaviorEntry {
				Action = action,
				CoastingStartDistance = entry.Distance - coastingDistance,
				BrakingStartDistance = entry.Distance - brakingDistance,
				TriggerDistance = entry.Distance,
				NextTargetSpeed = nextTargetSpeed,
				CycleEntry = entry,
			};
			return nextEntry;
		}

		protected internal virtual Meter ComputeCoastingDistance(
			MeterPerSecond vehicleSpeed,
			DrivingCycleData.DrivingCycleEntry actionEntry)
		{
			var targetSpeed = OverspeedAllowed(actionEntry.VehicleTargetSpeed)
				? actionEntry.VehicleTargetSpeed + Driver.DriverData.OverSpeed.OverSpeed
				: actionEntry.VehicleTargetSpeed;

			var vehicleMass = Driver.DataBus.TotalMass + Driver.DataBus.ReducedMassWheels;
			var targetAltitude = actionEntry.Altitude; //dec.Altitude;

			var vehicleAltitude = Driver.DataBus.Altitude;

			var targetEnergy = vehicleMass * Physics.GravityAccelleration * targetAltitude +
								vehicleMass * targetSpeed * targetSpeed / 2;
			var vehicleEnergy = vehicleMass * Physics.GravityAccelleration * vehicleAltitude +
								vehicleMass * vehicleSpeed * vehicleSpeed / 2;

			var energyDifference = vehicleEnergy - targetEnergy;

			var airDragForce = Driver.DataBus.AirDragResistance(vehicleSpeed, targetSpeed);
			var rollResistanceForce = Driver.DataBus.RollingResistance(
				((targetAltitude - vehicleAltitude) / (actionEntry.Distance - Driver.DataBus.Distance))
				.Value().SI<Radian>());
			var engineDragLoss = Driver.DataBus.EngineDragPower(Driver.DataBus.EngineSpeed);
			var gearboxLoss = Driver.DataBus.GearboxLoss();
			var axleLoss = Driver.DataBus.AxlegearLoss();

			var coastingResistanceForce = airDragForce + rollResistanceForce +
										(gearboxLoss + axleLoss - engineDragLoss) / vehicleSpeed;

			var coastingDecisionFactor = Driver.DriverData.LookAheadCoasting.LookAheadDecisionFactor.Lookup(
				targetSpeed,
				vehicleSpeed - targetSpeed);
			var coastingDistance = (energyDifference / (coastingDecisionFactor * coastingResistanceForce)).Cast<Meter>();
			return coastingDistance;
		}

		public bool OverspeedAllowed(MeterPerSecond velocity, bool prohibitOverspeed = false)
		{
			if (prohibitOverspeed) {
				return false;
			}

			return Driver.DriverData.OverSpeed.Enabled
					&& velocity > Driver.DriverData.OverSpeed.MinSpeed
					&& (velocity + Driver.DriverData.OverSpeed.OverSpeed) <
					(Driver.DataBus.MaxVehicleSpeed ?? 500.KMPHtoMeterPerSecond());
		}
	}


	public struct EcoRoll
	{
		public EcoRollStates State;
		public Second StateChangeTstmp;
		public GearshiftPosition Gear;
		public Watt PreviousBrakePower;
		public bool AcceleratorPedalIdle;
		public bool AllConditionsMet;
	}

	public enum EcoRollStates
	{
		EcoRollOff,
		PreActivation,
		EcoRollOn,
	}

	//=====================================

	public interface IDriverMode
	{
		DefaultDriverStrategy DriverStrategy { get; set; }

		IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient);

		void ResetMode();
	}

	public abstract class AbstractDriverMode : LoggingObject, IDriverMode
	{
		private IDriverActions _driver;
		private DriverData _driverData;
		private IDataBus _dataBus;

		public DefaultDriverStrategy DriverStrategy { get; set; }

		protected IDriverActions Driver
		{
			get { return _driver ?? (_driver = DriverStrategy.Driver); }
		}

		protected DriverData DriverData
		{
			get { return _driverData ?? (_driverData = Driver.DriverData); }
		}

		protected IDataBus DataBus
		{
			get { return _dataBus ?? (_dataBus = Driver.DataBus); }
		}

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			var response = DoHandleRequest(absTime, ds, targetVelocity, gradient);

			if (DriverStrategy.NextDrivingAction == null || !(response is ResponseSuccess)) {
				return response;
			}

			// if we accelerate in the current simulation interval the ActionDistance of the next action
			// changes and we might pass the ActionDistance - check again...
			if (response.Acceleration <= 0) {
				return response;
			}

			// if the speed at the end of the simulation interval is below the next target speed 
			// we are fine (no need to brake right now)
			var v2 = Driver.DataBus.VehicleSpeed + response.Acceleration * response.SimulationInterval;
			if (v2 <= DriverStrategy.NextDrivingAction.NextTargetSpeed) {
				return response;
			}

			Meter newds;
			response = CheckRequestDoesNotExceedNextAction(absTime, ds, targetVelocity, gradient, response, out newds);

			if (ds.IsEqual(newds, 1e-3.SI<Meter>())) {
				return response;
			}

			if (newds.IsSmallerOrEqual(0, 1e-3)) {
				newds = ds / 2.0;

				//DriverStrategy.CurrentDrivingMode = DefaultDriverStrategy.DrivingMode.DrivingModeBrake;
				//DriverStrategy.BrakeTrigger = DriverStrategy.NextDrivingAction;
			}

			var newOperatingPoint = VectoMath.ComputeTimeInterval(
				DataBus.VehicleSpeed, response.Acceleration, DataBus.Distance,
				newds);
			if (newOperatingPoint.SimulationInterval.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval)) {
				// the next time interval will be too short, this may lead to issues with inertia etc. 
				// instead of accelerating, drive at constant speed.
				response = DoHandleRequest(absTime, ds, Driver.DataBus.VehicleSpeed, gradient, true);
				return response;
			}

			Log.Debug(
				"Exceeding next ActionDistance at {0}. Reducing max Distance from {2} to {1}",
				DriverStrategy.NextDrivingAction.ActionDistance, newds, ds);
			return new ResponseDrivingCycleDistanceExceeded(this) {
				MaxDistance = newds,
			};
		}

		protected abstract IResponse DoHandleRequest(
			Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed = false);

		protected abstract IResponse CheckRequestDoesNotExceedNextAction(
			Second absTime, Meter ds,
			MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newSimulationDistance);

		public abstract void ResetMode();
	}

	//=====================================

	public class DriverModeDrive : AbstractDriverMode
	{
		protected override IResponse DoHandleRequest(
			Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed = false)
		{
			var debug = new DebugData();

			Driver.DriverBehavior = DrivingBehavior.Driving;
			var velocity = targetVelocity;
			if (DriverStrategy.OverspeedAllowed(targetVelocity, prohibitOverspeed)) {
				velocity += DriverData.OverSpeed.OverSpeed;
			}
			if (DataBus.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityRoadSweeping && targetVelocity < DriverStrategy.PTODriveMinSpeed) {
				velocity = DriverStrategy.PTODriveMinSpeed;
				targetVelocity = velocity;
			}

			if (DataBus.GearboxType.AutomaticTransmission() || DataBus.ClutchClosed(absTime)) {
				for (var i = 0; i < 3; i++) {
					var retVal = HandleRequestEngaged(absTime, ds, targetVelocity, gradient, prohibitOverspeed, velocity, debug);
					if (retVal != null) {
						return retVal;
					}
				}

				throw new VectoException("HandleRequestEngaged found no operating point.");
			} 

			var response = HandleRequestDisengaged(absTime, ds, gradient, velocity, debug);
			if (!(response is ResponseSuccess) && DataBus.ClutchClosed(absTime)) {
				response = HandleRequestEngaged(absTime, ds, targetVelocity, gradient, prohibitOverspeed, velocity, debug);
			}
			
			return response;
		}

		private IResponse HandleRequestDisengaged(
			Second absTime, Meter ds, Radian gradient, MeterPerSecond velocity,
			DebugData debug)
		{
			if (DataBus.VehicleSpeed.IsSmallerOrEqual(0.SI<MeterPerSecond>())) {
				// the clutch is disengaged, and the vehicle stopped - we can't perform a roll action. wait for the clutch to be engaged
				// todo mk 2016-08-23: is this still needed?
				var remainingShiftTime = Constants.SimulationSettings.TargetTimeInterval;
				while (!DataBus.ClutchClosed(absTime + remainingShiftTime)) {
					remainingShiftTime += Constants.SimulationSettings.TargetTimeInterval;
				}

				return new ResponseFailTimeInterval(this) {
					DeltaT = remainingShiftTime,
				};
			}

			var response = Driver.DrivingActionRoll(absTime, ds, velocity, gradient);
			debug.Add(new { action = "ClutchOpen -> Roll", response });
			response.Switch().Case<ResponseUnderload>(
						r => {
							if (DataBus.ClutchClosed(absTime)) {
								response = HandleRequestEngaged(absTime, ds, velocity, gradient, false, velocity, debug);
							} else {
								response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient, r);
								debug.Add(new { action = "Roll:Underload -> Brake", response });
							}
						})
					.Case<ResponseSpeedLimitExceeded>(
						() => {
							response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
							debug.Add(new { action = "Roll:SpeedLimitExceeded -> Brake", response });
						});
			return response;
		}

		private IResponse HandleRequestEngaged(
			Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed, MeterPerSecond velocity, DebugData debug)
		{
			// drive along
			var first = FirstAccelerateOrCoast(absTime, ds, targetVelocity, gradient, prohibitOverspeed, velocity, debug);

			var second = first;
			first.Switch().Case<ResponseUnderload>(
				r => {
					if (DataBus.GearboxType.AutomaticTransmission() && !DataBus.ClutchClosed(absTime)) {
						second = Driver.DrivingActionRoll(absTime, ds, velocity, gradient);
					}
					if (DataBus.VehicleSpeed.IsGreater(0) && DriverStrategy.OverspeedAllowed(targetVelocity, prohibitOverspeed)) {
						second = Driver.DrivingActionCoast(absTime, ds, velocity, gradient);
						debug.Add(new { action = "first:(Underload & Overspeed)-> Coast", second });
						second = HandleCoastAfterUnderloadWithOverspeed(absTime, ds, gradient, velocity, debug, second);
					} else {
						second = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
						debug.Add(new { action = "first:(Underload & !Overspeed) -> Brake", second });
					}
				}).Case<ResponseEngineSpeedTooHigh>(
				r => { second = Driver.DrivingActionBrake(absTime, ds, targetVelocity, gradient, r); });

			if (second == null) {
				return null;
			}

			var third = second;

			second.Switch().Case<ResponseGearShift>(
				r => {
					third = Driver.DrivingActionRoll(absTime, ds, velocity, gradient);
					debug.Add(new { action = "second: GearShift -> Roll", third });
					third.Switch().Case<ResponseUnderload>(
						() => {
							// overload may happen if driver limits acceleration when rolling downhill
							third = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
							debug.Add(new { action = "third:Underload -> Brake", third });
						}).Case<ResponseSpeedLimitExceeded>(
						() => {
							third = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
							debug.Add(new { action = "third:SpeedLimitExceeded -> Brake", third });
						});
				}).Case<ResponseOverload>(
				r => {
					third = Driver.DrivingActionCoast(absTime, ds, velocity, gradient);
					debug.Add(new { action = "second:Overload -> Coast", third });
				});

			return third;
		}

		private IResponse HandleCoastAfterUnderloadWithOverspeed(
			Second absTime, Meter ds, Radian gradient,
			MeterPerSecond velocity, DebugData debug, IResponse second)
		{
			if (second is ResponseUnderload || second is ResponseSpeedLimitExceeded) {
				second = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
				debug.Add(
					new {
						action = "second:(Underload|SpeedLimitExceeded) -> Brake",
						second
					});
			}
			if (second is ResponseEngineSpeedTooHigh) {
				second = Driver.DrivingActionBrake(absTime, ds, velocity, gradient, second);
				debug.Add(
					new {
						action = "second:(EngineSpeedTooHigh|SpeedLimitExceeded) -> Brake with reduced acceleration",
						second
					});
			}
			return second;
		}

		private IResponse FirstAccelerateOrCoast(
			Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed, MeterPerSecond velocity, DebugData debug)
		{
			IResponse first;
			if (DriverStrategy.OverspeedAllowed(targetVelocity, prohibitOverspeed) &&
				DataBus.VehicleSpeed.IsEqual(targetVelocity)) {
				first = Driver.DrivingActionCoast(absTime, ds, velocity, gradient);
				debug.Add(new { action = "Coast", first });
				if (first is ResponseSuccess && first.Acceleration < 0) {
					first = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
					debug.Add(new { action = "Coast:(Success & Acc<0) -> Accelerate", first });
				}
			} else {
				first = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
				debug.Add(new { action = "Accelerate", first });
			}
			return first;
		}

		protected override IResponse CheckRequestDoesNotExceedNextAction(
			Second absTime, Meter ds,
			MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newds)
		{
			var nextAction = DriverStrategy.NextDrivingAction;
			newds = ds;
			if (nextAction == null) {
				return response;
			}

			var v2 = Driver.DataBus.VehicleSpeed + response.Acceleration * response.SimulationInterval;
			var newBrakingDistance = Driver.DriverData.AccelerationCurve.ComputeAccelerationDistance(
										v2,
										nextAction.NextTargetSpeed) + DefaultDriverStrategy.BrakingSafetyMargin;
			switch (DriverStrategy.NextDrivingAction.Action) {
				case DrivingBehavior.Coasting:
					var coastingDistance = DriverStrategy.ComputeCoastingDistance(v2, nextAction.CycleEntry);
					var newActionDistance = coastingDistance;
					var safetyFactor = 4.0;
					if (newBrakingDistance > coastingDistance) {
						newActionDistance = newBrakingDistance;
						safetyFactor = 0.5;
					}

					// if the distance at the end of the simulation interval is smaller than the new ActionDistance
					// we are safe - go ahead...
					if ((Driver.DataBus.Distance + ds).IsSmallerOrEqual(
							nextAction.TriggerDistance - newActionDistance,
							Constants.SimulationSettings.DriverActionDistanceTolerance * safetyFactor) &&
						(Driver.DataBus.Distance + ds).IsSmallerOrEqual(nextAction.TriggerDistance - newBrakingDistance)) {
						return response;
					}

					newds = ds / 2; //EstimateAccelerationDistanceBeforeBrake(response, nextAction) ?? ds;
					break;
				case DrivingBehavior.Braking:
					if ((Driver.DataBus.Distance + ds).IsSmaller(nextAction.TriggerDistance - newBrakingDistance)) {
						return response;
					}

					newds = nextAction.TriggerDistance - newBrakingDistance - Driver.DataBus.Distance -
							Constants.SimulationSettings.DriverActionDistanceTolerance / 2;
					break;
				default: return response;
			}

			return response;
		}

		public override void ResetMode() { }
	}

	//=====================================

	public class DriverModeBrake : AbstractDriverMode
	{
		protected enum BrakingPhase
		{
			Coast,
			Brake
		}

		protected BrakingPhase Phase;
		protected bool RetryDistanceExceeded;

		protected override IResponse DoHandleRequest(
			Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed = false)
		{
			if (DataBus.VehicleSpeed <= DriverStrategy.BrakeTrigger.NextTargetSpeed && !DataBus.VehicleStopped) {
				var retVal = HandleTargetspeedReached(absTime, ds, targetVelocity, gradient);
				for (var i = 0; i < 3 && retVal == null; i++) {
					retVal = HandleTargetspeedReached(absTime, ds, targetVelocity, gradient);
				}

				if (retVal == null) {
					throw new VectoException("Failed to find operating point!");
				}

				return retVal;
			}

			var currentDistance = DataBus.Distance;

			var brakingDistance = Driver.ComputeDecelerationDistance(DriverStrategy.BrakeTrigger.NextTargetSpeed) +
								DefaultDriverStrategy.BrakingSafetyMargin;
			DriverStrategy.BrakeTrigger.BrakingStartDistance = DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance;
			if (DriverStrategy.BrakeTrigger.Action == DrivingBehavior.Braking) {
				Phase = BrakingPhase.Brake;
			}
			if (Phase == BrakingPhase.Coast) {
				var resp = CheckSwitchingToBraking(ds, currentDistance);
				if (resp != null) {
					return resp;
				}
			}

			switch (Phase) {
				case BrakingPhase.Coast:
					for (var i = 1; i < 3; i++) {
						var retVal = DoCoast(absTime, ds, targetVelocity, gradient, currentDistance);
						if (retVal != null) {
							return retVal;
						}
					}
					throw new VectoException("No valid operating point found");
				case BrakingPhase.Brake:
					return DoBrake(absTime, ds, targetVelocity, gradient, brakingDistance, currentDistance);
				default: throw new VectoException("Invalid Phase in DriverModeBrake");
			}
		}

		private IResponse DoBrake(
			Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			Meter brakingDistance, Meter currentDistance)
		{
			IResponse response;
			Log.Debug(
				"Phase: BRAKE. breaking distance: {0} start braking @ {1}", brakingDistance,
				DriverStrategy.BrakeTrigger.BrakingStartDistance);
			if (DriverStrategy.BrakeTrigger.BrakingStartDistance.IsSmaller(
				currentDistance,
				Constants.SimulationSettings.DriverActionDistanceTolerance / 2)) {
				Log.Info(
					"Expected Braking Deceleration could not be reached! {0}",
					DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance);
			}
			var targetDistance = DataBus.VehicleSpeed < Constants.SimulationSettings.MinVelocityForCoast
				? DriverStrategy.BrakeTrigger.TriggerDistance
				: null;
			if (targetDistance == null && DriverStrategy.BrakeTrigger.NextTargetSpeed.IsEqual(0.SI<MeterPerSecond>())) {
				targetDistance = DriverStrategy.BrakeTrigger.TriggerDistance - DefaultDriverStrategy.BrakingSafetyMargin;
			}
			Driver.DriverBehavior = DrivingBehavior.Braking;

			if (DataBus.VehicleSpeed.IsEqual(0) && DriverStrategy.BrakeTrigger.NextTargetSpeed.IsEqual(0)) {
				if (ds.IsEqual(targetDistance - currentDistance)) {
					return new ResponseDrivingCycleDistanceExceeded(this) {
						MaxDistance = ds / 2
					};
				}

				response = Driver.DrivingActionAccelerate(absTime, ds, 1.KMPHtoMeterPerSecond(), gradient);
				if (response is ResponseUnderload) {
					response = Driver.DrivingActionBrake(
						absTime, ds, 1.KMPHtoMeterPerSecond(), gradient, response, overrideAction: DrivingAction.Accelerate);
				}
			} else {
				response = Driver.DrivingActionBrake(
					absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
					gradient, targetDistance: targetDistance);
			}

			if (DataBus.GearboxType.AutomaticTransmission() && response == null) {
				for (var i = 0; i < 3 && response == null; i++) {
					DataBus.BrakePower = 0.SI<Watt>();
					response = Driver.DrivingActionBrake(
						absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
						gradient, targetDistance: targetDistance);
				}

				if (response == null) {
					throw new VectoException("No valid operating point found");
				}
			}

			response.Switch().Case<ResponseOverload>(
				r => {
					Log.Info(
						"Brake -> Got OverloadResponse during brake action - desired deceleration could not be reached! response: {0}",
						r);
					if (!DataBus.ClutchClosed(absTime)) {
						Log.Info("Brake -> Overload -> Clutch is open - Trying roll action");
						response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
						response.Switch().Case<ResponseSpeedLimitExceeded>(
							() => { response = Driver.DrivingActionBrake(absTime, ds, targetVelocity, gradient); }
						);
					} else {
						Log.Info("Brake -> Overload -> Clutch is closed - Trying brake action again");
						DataBus.BrakePower = 0.SI<Watt>();
						response = Driver.DrivingActionBrake(
							absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient,
							targetDistance: targetDistance);
						response.Switch().Case<ResponseOverload>(
							r1 => {
								Log.Info("Brake -> Overload -> 2nd Brake -> Overload -> Trying accelerate action");
								response = Driver.DrivingActionAccelerate(
									absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
								response.Switch().Case<ResponseGearShift>(
									rs => {
										Log.Info(
											"Brake -> Overload -> 2nd Brake -> Accelerate -> Got GearShift response, performing roll action");
										response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
									});
							});
					}
				}).Case<ResponseGearShift>(
				r => {
					Log.Info("Brake -> Got GearShift response, performing roll action + brakes");

					//response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
					DataBus.BrakePower = 0.SI<Watt>();
					response = Driver.DrivingActionBrake(
						absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
						gradient, targetDistance: targetDistance);
					response.Switch().Case<ResponseOverload>(
						() => {
							Log.Info("Brake -> Geearshift -> Overload -> trying roll action (no gear engaged)");
							response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
						});
				});
			return response;
		}

		private IResponse DoCoast(
			Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			Meter currentDistance)
		{
			IResponse response;
			Driver.DriverBehavior = DrivingBehavior.Coasting;
			response = DataBus.ClutchClosed(absTime)
				? Driver.DrivingActionCoast(absTime, ds, VectoMath.Max(targetVelocity, DataBus.VehicleSpeed), gradient)
				: Driver.DrivingActionRoll(absTime, ds, VectoMath.Max(targetVelocity, DataBus.VehicleSpeed), gradient);
			response.Switch().Case<ResponseUnderload>(
				r => {
					// coast would decelerate more than driver's max deceleration => issue brakes to decelerate with driver's max deceleration
					response = Driver.DrivingActionBrake(
						absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
						gradient, r);
					if ((DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance).IsSmallerOrEqual(
						Constants.SimulationSettings.DriverActionDistanceTolerance)) {
						Phase = BrakingPhase.Brake;
					}
				}).Case<ResponseOverload>(
				r => {
					// limiting deceleration while coast may result in an overload => issue brakes to decelerate with driver's max deceleration
					response = DataBus.ClutchClosed(absTime)
						? Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient)
						: Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);

					//Phase = BrakingPhase.Brake;
				}).Case<ResponseDrivingCycleDistanceExceeded>(
				r => {
					if (!ds.IsEqual(r.MaxDistance)) {
						// distance has been reduced due to vehicle stop in coast/roll action => use brake action to get exactly to the stop-distance
						// TODO what if no gear is enaged (and we need driveline power to get to the stop-distance?
						response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
					}
				}).Case<ResponseEngineSpeedTooHigh>(
				r => { response = Driver.DrivingActionBrake(absTime, ds, targetVelocity, gradient, r); });
			if (response == null) {
				return response;
			}

			// handle the SpeedLimitExceeded Response and Gearshift Response separately in case it occurs in one of the requests in the second try
			for (var i = 0; i < 3 && (response is ResponseGearShift || response is ResponseSpeedLimitExceeded); i++) {
				response.Switch()
				.Case<ResponseGearShift>(
						r => { response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient); })
				.Case<ResponseSpeedLimitExceeded>(
					() => {
						response = Driver.DrivingActionBrake(
							absTime, ds, DataBus.VehicleSpeed,
							gradient);
						if (response is ResponseOverload && !DataBus.ClutchClosed(absTime)) {
							response = Driver.DrivingActionRoll(absTime, ds, DataBus.VehicleSpeed, gradient);
						}
					});
			}

			return response;
		}

		private IResponse CheckSwitchingToBraking(Meter ds, Meter currentDistance)
		{
			var nextBrakeAction = DriverStrategy.GetNextDrivingAction(ds);
			if (nextBrakeAction != null &&
				!DriverStrategy.BrakeTrigger.TriggerDistance.IsEqual(nextBrakeAction.TriggerDistance) &&
				nextBrakeAction.BrakingStartDistance.IsSmaller(DriverStrategy.BrakeTrigger.BrakingStartDistance)) {
				DriverStrategy.BrakeTrigger = nextBrakeAction;
				Log.Debug(
					"setting brake trigger to new trigger: trigger distance: {0}, start braking @ {1}",
					nextBrakeAction.TriggerDistance, nextBrakeAction.BrakingStartDistance);
			}

			Log.Debug("start braking @ {0}", DriverStrategy.BrakeTrigger.BrakingStartDistance);
			var remainingDistanceToBrake = DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance;
			var estimatedTimeInterval = remainingDistanceToBrake / DataBus.VehicleSpeed;
			if (estimatedTimeInterval.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval) ||
				currentDistance + Constants.SimulationSettings.DriverActionDistanceTolerance >
				DriverStrategy.BrakeTrigger.BrakingStartDistance) {
				Phase = BrakingPhase.Brake;
				Log.Debug("Switching to BRAKE Phase. currentDistance: {0}", currentDistance);
			} else {
				if ((currentDistance + ds).IsGreater(DriverStrategy.BrakeTrigger.BrakingStartDistance)) {
					return new ResponseDrivingCycleDistanceExceeded(this) {
						MaxDistance = DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance
					};
				}
			}

			if (DataBus.VehicleSpeed < Constants.SimulationSettings.MinVelocityForCoast) {
				Phase = BrakingPhase.Brake;
				Log.Debug(
					"Switching to BRAKE Phase. currentDistance: {0}  v: {1}", currentDistance,
					DataBus.VehicleSpeed);
			}
			return null;
		}

		private IResponse HandleTargetspeedReached(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			var response = TargetSpeedReachedDriveAlong(absTime, ds, targetVelocity, gradient);

			//var i = 0;
			//do {
			response.Switch().Case<ResponseGearShift>(
				() => {
					response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
					response.Switch().Case<ResponseUnderload>(
						r => {
							// under-load may happen if driver limits acceleration when rolling downhill
							response = Driver.DrivingActionBrake(
								absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
								gradient, r);
						}).Case<ResponseSpeedLimitExceeded>(
						() => {
							response = Driver.DrivingActionBrake(
								absTime, ds, DataBus.VehicleSpeed,
								gradient);
						});
				}).Case<ResponseSpeedLimitExceeded>(
				() => {
					response = Driver.DrivingActionBrake(
						absTime, ds, DataBus.VehicleSpeed,
						gradient);
				}).Case<ResponseUnderload>(
				r => {
					//response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
					//	gradient, r);
					response = Driver.DrivingActionBrake(
						absTime, ds, DataBus.VehicleSpeed + r.Acceleration * r.SimulationInterval,
						gradient, r);
					if (response != null) {
						response.Switch().Case<ResponseGearShift>(
							() => {
								DataBus.BrakePower = 0.SI<Watt>();
								response = Driver.DrivingActionBrake(
									absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
									gradient, r);
							}).Case<ResponseOverload>(
							() => {
								DataBus.BrakePower = 0.SI<Watt>();
								if (DataBus.GearboxType.AutomaticTransmission() || DataBus.ClutchClosed(absTime)) {
									if (DataBus.VehicleSpeed.IsGreater(0)) {
										response = Driver.DrivingActionAccelerate(
											absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
									} else {
										if (RetryDistanceExceeded) {
											response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
										} else {
											RetryDistanceExceeded = true;
											response = new ResponseDrivingCycleDistanceExceeded(this) { MaxDistance = ds / 2 };
										}
									}
								} else {
									response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
								}
							});
					}
				});

			//} while (!(response is ResponseSuccess) && i++ < 3);
			return response;
		}

		private IResponse TargetSpeedReachedDriveAlong(
			Second absTime, Meter ds, MeterPerSecond targetVelocity,
			Radian gradient)
		{
			IResponse response;
			if (DataBus.GearboxType.AutomaticTransmission() || DataBus.ClutchClosed(absTime)) {
				if (DataBus.VehicleSpeed.IsGreater(0)) {
					response = Driver.DrivingActionAccelerate(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
				} else {
					if (RetryDistanceExceeded) {
						response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
					} else {
						RetryDistanceExceeded = true;
						response = new ResponseDrivingCycleDistanceExceeded(this) { MaxDistance = ds / 2 };
					}
				}
			} else {
				response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
			}
			return response;
		}

		protected override IResponse CheckRequestDoesNotExceedNextAction(
			Second absTime, Meter ds,
			MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newds)
		{
			var nextAction = DriverStrategy.BrakeTrigger;
			newds = ds;
			if (nextAction == null) {
				return response;
			}

			switch (nextAction.Action) {
				case DrivingBehavior.Coasting:
					var v2 = Driver.DataBus.VehicleSpeed + response.Acceleration * response.SimulationInterval;
					var newBrakingDistance = Driver.DriverData.AccelerationCurve.ComputeAccelerationDistance(
						v2,
						nextAction.NextTargetSpeed);
					if ((Driver.DataBus.Distance + ds).IsSmaller(nextAction.TriggerDistance - newBrakingDistance)) {
						return response;
					}

					newds = nextAction.TriggerDistance - newBrakingDistance - Driver.DataBus.Distance -
							Constants.SimulationSettings.DriverActionDistanceTolerance / 2;
					break;
				default: return response;
			}

			return response;
		}

		public override void ResetMode()
		{
			RetryDistanceExceeded = false;
			Phase = BrakingPhase.Coast;
		}
	}

	//=====================================

	[DebuggerDisplay("ActionDistance: {ActionDistance}, TriggerDistance: {TriggerDistance}, Action: {Action}")]
	public class DrivingBehaviorEntry
	{
		public DrivingBehavior Action;
		public MeterPerSecond NextTargetSpeed;
		public Meter TriggerDistance;

		public Meter ActionDistance
		{
			get {
				return VectoMath.Min(
					CoastingStartDistance ?? double.MaxValue.SI<Meter>(),
					BrakingStartDistance ?? double.MaxValue.SI<Meter>());
			}
		}

		public Meter SelectActionDistance(Meter minDistance)
		{
			return
				new[] { BrakingStartDistance, CoastingStartDistance }.OrderBy(x => x.Value()).First(x => x >= minDistance);
		}

		public Meter CoastingStartDistance { get; set; }

		public Meter BrakingStartDistance { get; set; }

		public DrivingCycleData.DrivingCycleEntry CycleEntry;

		public bool HasEqualTrigger(DrivingBehaviorEntry other)
		{
			return TriggerDistance.IsEqual(other.TriggerDistance) && NextTargetSpeed.IsEqual(other.NextTargetSpeed);
		}

		public override string ToString()
		{
			return string.Format(
				"action: {0} @ {1} / {2}. trigger: {3} targetSpeed: {4}", Action, CoastingStartDistance,
				BrakingStartDistance, TriggerDistance, NextTargetSpeed);
		}
	}
}

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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
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

		protected internal EcoRoll EcoRollState;
		protected PCCSegments PCCSegments;

		public PCCStates PCCState => pccState;
		protected internal PCCStates pccState = PCCStates.OutsideSegment;
		protected internal bool ATEcoRollReleaseLockupClutch;

		private readonly VectoSimulationJobType PowertrainArchitecture;

		protected IPowertrainContainer _powertrainContainer = null;
		public virtual IPowertrainContainer PowertrainContainer => _powertrainContainer ?? DataBus.Powertrains.Values.Single();
		protected IGearboxInfo GearboxInfo => PowertrainContainer.GearboxInfo;
		protected IAxlegearInfo AxlegearInfo => PowertrainContainer.AxlegearInfo;
		protected IDataBus DataBus { get; }
		
		public DefaultDriverStrategy(IVehicleContainer container)
		{
			DataBus = container;

			PTODriveMinSpeed = container.RunData.DriverData.PTODriveMinSpeed;
			DrivingModes.Add(DrivingMode.DrivingModeDrive, new DriverModeDrive() { DriverStrategy = this });
			DrivingModes.Add(DrivingMode.DrivingModeBrake, new DriverModeBrake() { DriverStrategy = this });
			CurrentDrivingMode = DrivingMode.DrivingModeDrive;

			VehicleCategory = container.RunData.VehicleData.VehicleCategory;

			
			var data = container.RunData;
			PowertrainArchitecture = data.JobType;

			ADAS = data.VehicleData?.ADAS ?? new VehicleData.ADASData()
			{
				EcoRoll = EcoRollType.None,
				EngineStopStart = false,
				PredictiveCruiseControl = PredictiveCruiseControlType.None,
			};

			
			ATEcoRollReleaseLockupClutch = data.AxlePowertrainsDict[PowertrainContainer.AxleNumber].GearboxData?.ATEcoRollReleaseLockupClutch ?? false;
			

			EcoRollState = new EcoRoll
			{
				State = EcoRollStates.EcoRollOff,
				Gear = new GearshiftPosition(0),
				StateChangeTstmp = -double.MaxValue.SI<Second>(),
				PreviousBrakePower = 0.SI<Watt>(),
				AcceleratorPedalIdle = false,
			};

			PCCSegments = new PCCSegments();



			if (ADAS.PredictiveCruiseControl != PredictiveCruiseControlType.None)
			{
				// create a dummy powertrain for pre-processing and estimations
				var testPowertrain =
					container.SimplePowertrainBuilder.CreateTestPowertrain(container, false, data.JobType);
				
				container.AddPreprocessor(new PCCSegmentPreprocessor(testPowertrain, PCCSegments,
					data?.DriverData.PCC));
			}


		}

		public VehicleCategory VehicleCategory { get; set; }

		public IDriverActions Driver { get; set; }

		public DrivingBehaviorEntry BrakeTrigger { get; protected internal set; }

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			var retVal = DoHandleRequest(absTime, ds, targetVelocity, gradient);
			if (retVal is ResponseSuccess) {
				EcoRollState.PreviousBrakePower = DataBus.Brakes.BrakePower;
				if (retVal.Source is ICombustionEngine) {
					var success = retVal as ResponseSuccess;
					var avgEngineSpeed = (success.Engine.EngineSpeed + DataBus.EngineInfo.EngineSpeed) / 2.0;
					EcoRollState.AcceleratorPedalIdle = success.Engine.DragPower.IsEqual(success.Engine.TotalTorqueDemand * avgEngineSpeed, 10.SI<Watt>());
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
				absTime, dt, VectoMath.Min(DataBus.VehicleInfo.MaxVehicleSpeed, targetVelocity), gradient);
			EcoRollState.PreviousBrakePower = DataBus.Brakes.BrakePower;
			return retVal;
		}

		public void WriteModalResults(IModalDataContainer container)
		{
			container.SetDataValue("EcoRollConditionsMet", EcoRollState.AllConditionsMet ? 1 : 0);
			if (PCCSegments.Count > 0) {
				var val = 0;
				if (DataBus.MileageCounter.Distance > PCCSegments.Current.EndDistance) {
					val = 0;
				} else if (DataBus.MileageCounter.Distance > PCCSegments.Current.DistanceAtLowestSpeed) {
					val = 5;
				} else if (DataBus.MileageCounter.Distance > PCCSegments.Current.StartDistance) {
					val = -5;
				}
				container.SetDataValue("PCCSegment", val);
				container.SetDataValue("PCCState", (int)pccState);
			} else {
				container.SetDataValue("PCCSegment", 0);
				container.SetDataValue("PCCState", (int)pccState);
			}
		}

		public void CommitSimulationStep()
		{
			if (PCCSegments.Count > 0) {
				if (DataBus.MileageCounter.Distance > PCCSegments.Current.EndDistance) {
					PCCSegments.MoveNext();
					pccState = PCCStates.OutsideSegment;
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
				(pccState == PCCStates.OutsideSegment || pccState == PCCStates.WithinSegment)) {
				HandleEcoRoll(absTime, targetVelocity);
			}

			//if (ADAS.EcoRoll != EcoRollType.None) {
			// todo MQ: keep something like this to prevent driver to turn on engine in every timestep (in combination with hybrids leads to errors!)
			if (EcoRollState.State != EcoRollStates.EcoRollOn && pccState != PCCStates.UseCase1 &&
				pccState != PCCStates.UseCase2) {
				EngineOffTimestamp = null;
				if (DataBus.PowertrainInfo.HasCombustionEngine && !DataBus.PowertrainInfo.HasElectricMotor) {
					DataBus.EngineCtl.CombustionEngineOn = true;
				}
			}

			if (CurrentDrivingMode == DrivingMode.DrivingModeBrake) {
				var nextAction = GetNextDrivingAction(ds);
				var currentDistance = DataBus.MileageCounter.Distance;

                if (nextAction != null && !BrakeTrigger.HasEqualTrigger(nextAction) && 
					(nextAction.ActionDistance.IsSmallerOrEqual(BrakeTrigger.ActionDistance) || nextAction.BrakingStartDistance.IsBetween(currentDistance, currentDistance + ds))) {
					BrakeTrigger = nextAction;
				}
				if (DataBus.MileageCounter.Distance.IsGreaterOrEqual(BrakeTrigger.TriggerDistance, 1e-3.SI<Meter>())) {
					CurrentDrivingMode = DrivingMode.DrivingModeDrive;
					NextDrivingAction = null;
					DrivingModes[CurrentDrivingMode].ResetMode();
					Log.Debug("Switching to DrivingMode DRIVE");
				}


			}
			if (CurrentDrivingMode == DrivingMode.DrivingModeDrive) {
				var currentDistance = DataBus.MileageCounter.Distance;

				//var coasting = LookAheadCoasting(ds);

				UpdateDrivingAction(currentDistance, ds);
				if (NextDrivingAction != null) {
					var remainingDistance = NextDrivingAction.ActionDistance - currentDistance;
					var estimatedTimestep = remainingDistance / DataBus.VehicleInfo.VehicleSpeed;

					var atTriggerTistance = remainingDistance.IsEqual(
						0.SI<Meter>(), Constants.SimulationSettings.DriverActionDistanceTolerance);
					var closeBeforeBraking = estimatedTimestep.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval);
					var brakingIntervalTooShort = NextDrivingAction.Action == DrivingBehavior.Braking &&
												((NextDrivingAction.TriggerDistance - NextDrivingAction.ActionDistance) / DataBus.VehicleInfo.VehicleSpeed)
												.IsSmaller(
													Constants.SimulationSettings.LowerBoundTimeInterval / 20) && !DataBus.ClutchesInfo.First().ClutchClosed(absTime);
					var brakingIntervalShort = NextDrivingAction.Action == DrivingBehavior.Braking &&
												NextDrivingAction.ActionDistance.IsSmaller(currentDistance + ds) &&
												((NextDrivingAction.TriggerDistance - NextDrivingAction.ActionDistance) / DataBus.VehicleInfo.VehicleSpeed)
												.IsSmaller(
													Constants.SimulationSettings.LowerBoundTimeInterval / 2) 
													&& !DataBus.PowertrainInfo.VehicleArchitecture.IsMultiplePowertrains()
													&& (GearboxInfo.GearboxType.AutomaticTransmission() || !DataBus.ClutchesInfo.First().ClutchClosed(absTime));
					
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
				absTime, ds, VectoMath.Min(DataBus.VehicleInfo.MaxVehicleSpeed, targetVelocity), gradient);

			return retVal;
		}

		private void HandlePCC(Second absTime, MeterPerSecond targetVelocity)
		{
			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;

			UpdatePCCState(targetVelocity);

			switch (pccState) {
				case PCCStates.UseCase1:
					if (vehicleSpeed <= targetVelocity - Driver.DriverData.PCC.UnderSpeed * 1.05) {
						pccState = PCCStates.PCCinterrupt;
					}
					if (vehicleSpeed >= targetVelocity + 1.KMPHtoMeterPerSecond()) {
						pccState = PCCStates.WithinSegment;
					}
					break;
				case PCCStates.UseCase2:
					if (vehicleSpeed < Driver.DriverData.PCC.MinSpeed || vehicleSpeed > targetVelocity + 1.KMPHtoMeterPerSecond()) {
						pccState = PCCStates.WithinSegment;
					}
					break;
				case PCCStates.PCCinterrupt:
					if (vehicleSpeed >= targetVelocity - Driver.DriverData.PCC.UnderSpeed * 0.95) {
						pccState = PCCStates.UseCase1;
					}
					break;
			}

			switch (pccState) {
				case PCCStates.UseCase1:
				case PCCStates.UseCase2:
					switch (ADAS.EcoRoll) {
						case EcoRollType.None: break;
						case EcoRollType.WithoutEngineStop:
							if (DataBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN) != null) {
								DataBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN).DisengageGearbox = true;
							}
						
							break;
						case EcoRollType.WithEngineStop:
							if (DataBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN) != null) {
								DataBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN).DisengageGearbox = true;
							}
							if (DataBus.EngineCtl != null) {
								DataBus.EngineCtl.CombustionEngineOn = false;
							}
							break;
						default: throw new ArgumentOutOfRangeException();
					}

					break;
				case PCCStates.OutsideSegment:
				case PCCStates.WithinSegment:
				case PCCStates.PCCinterrupt:
					if (DataBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN) != null) {
						DataBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN).DisengageGearbox = false;
					}

					if (DataBus.EngineCtl != null) {
						DataBus.EngineCtl.CombustionEngineOn = true;
					}
					break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdatePCCState(MeterPerSecond targetVelocity)
		{
			// check if within PCC segment and update state ----------------------------
			var distance = DataBus.MileageCounter.Distance;
			var withinPCCSegment = PCCSegments.Current != null
									&& PCCSegments.Current.StartDistance <= distance
									&& distance <= PCCSegments.Current.EndDistance;

			if (!withinPCCSegment) {
				pccState = PCCStates.OutsideSegment;
				return;
			}
			if (pccState == PCCStates.OutsideSegment) {
				pccState = PCCStates.WithinSegment;
			}
			if (pccState != PCCStates.WithinSegment)
				return;

			// check for PCC usecase 1 -------------------------------------------------
			var endUseCase1 = PCCSegments.Current.EndDistance;
			var endEnergyUseCase1 = PCCSegments.Current.EnergyAtEnd;
			if ((distance + Driver.DriverData.PCC.PreviewDistanceUseCase1).IsSmallerOrEqual(endUseCase1)) {
				endUseCase1 = distance + Driver.DriverData.PCC.PreviewDistanceUseCase1;
				var endCycleEntry = DataBus.DrivingCycleInfo.CycleLookAhead(Driver.DriverData.PCC.PreviewDistanceUseCase1);
				endEnergyUseCase1 = CalculateEnergy(endCycleEntry.Altitude, endCycleEntry.VehicleTargetSpeed, DataBus.VehicleInfo.TotalMass);
			}
			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;
			var coastingForce = CalculateCoastingForce(targetVelocity, vehicleSpeed, PCCSegments.Current.Altitude, endUseCase1);
			var energyCoastingEndUseCase1 = (coastingForce * (endUseCase1 - distance)).Cast<Joule>();
			var currentEnergy = CalculateEnergy(DataBus.DrivingCycleInfo.Altitude, vehicleSpeed, DataBus.VehicleInfo.TotalMass);
			var energyCoastingLow = (coastingForce * (PCCSegments.Current.DistanceAtLowestSpeed - distance)).Cast<Joule>();

			var beforeVLow = distance.IsSmaller(PCCSegments.Current.DistanceAtLowestSpeed);
			var speedSufficient = vehicleSpeed.IsGreaterOrEqual(targetVelocity - Driver.DriverData.PCC.UnderSpeed);
			var currentEnergyHigherThanEndUseCase1 = currentEnergy.IsGreaterOrEqual(endEnergyUseCase1 + energyCoastingEndUseCase1);
			var currentEnergyHigherThanMin = currentEnergy.IsGreaterOrEqual(PCCSegments.Current.EnergyAtLowestSpeed + energyCoastingLow);

			if (beforeVLow
				&& speedSufficient
				&& currentEnergyHigherThanEndUseCase1
				&& currentEnergyHigherThanMin) {
				pccState = PCCStates.UseCase1;
				return;
			}

			// check for PCC use case 2 ------------------------------------------------
			var endUseCase2 = PCCSegments.Current.EndDistance;
			var endEnergyUseCase2 = PCCSegments.Current.EnergyAtEnd;
			if ((distance + Driver.DriverData.PCC.PreviewDistanceUseCase2).IsSmallerOrEqual(endUseCase2)) {
				endUseCase2 = distance + Driver.DriverData.PCC.PreviewDistanceUseCase2;
				var endCycleEntry = DataBus.DrivingCycleInfo.CycleLookAhead(Driver.DriverData.PCC.PreviewDistanceUseCase2);
				endEnergyUseCase2 = CalculateEnergy(endCycleEntry.Altitude, endCycleEntry.VehicleTargetSpeed, DataBus.VehicleInfo.TotalMass);
			}
			var energyCoastingEndUseCase2 = coastingForce * (endUseCase2 - distance);

			var beyondVLow = distance.IsGreaterOrEqual(PCCSegments.Current.DistanceAtLowestSpeed);
			var currentEnergyHigherThanEndUseCase2 = currentEnergy.IsGreaterOrEqual(endEnergyUseCase2 + energyCoastingEndUseCase2);
			var speedSufficientUseCase2 = vehicleSpeed.IsGreaterOrEqual(VectoMath.Max(targetVelocity - Driver.DriverData.PCC.UnderSpeed, Driver.DriverData.PCC.MinSpeed));
			var speedBelowTargetspeed = vehicleSpeed.IsSmallerOrEqual(targetVelocity - 1.KMPHtoMeterPerSecond());

			if (beyondVLow
				&& currentEnergyHigherThanEndUseCase2
				&& speedSufficientUseCase2
				&& speedBelowTargetspeed) {
				pccState = PCCStates.UseCase2;
			}
		}

		private Newton CalculateCoastingForce(MeterPerSecond targetVelocity, MeterPerSecond vehicleSpeed, Meter targetAltitude, Meter targetDistance)
		{
			var dataBus = DataBus;
			var airDrag = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, targetVelocity);
			var rollResistanceForce = DataBus.VehicleInfo.RollingResistance(dataBus.DrivingCycleInfo.RoadGradient);

			//mk20211008 shouldn't we calculate it the same as in ComputeCoastingDistance?
			//var rollResistanceForce = DataBus.VehicleInfo.RollingResistance(
			//	((targetAltitude - DataBus.DrivingCycleInfo.Altitude) / (targetDistance - DataBus.MileageCounter.Distance))
			//	.Value().SI<Radian>());

			var gearboxLoss = GearboxInfo.GearboxLoss();
			var axleLoss = AxlegearInfo.AxlegearLoss();
			var emDragLoss = CalculateElectricMotorDragLoss();
			var iceDragLoss = GetICEDragLossPCC(dataBus);

			var totalComponentLossPowers = gearboxLoss + axleLoss + emDragLoss - iceDragLoss;
			var coastingResistanceForce = airDrag.AirdragForce + rollResistanceForce + totalComponentLossPowers / vehicleSpeed;
			return coastingResistanceForce;
		}

		private Watt GetICEDragLossLookAheadCoasting(IDataBus dataBus)
		{
			if (PowertrainArchitecture.IsOneOf(VectoSimulationJobType.SerialHybridVehicle,
					VectoSimulationJobType.IEPC_S, VectoSimulationJobType.FCHV, VectoSimulationJobType.FCHV_IEPC)) {
				return 0.SI<Watt>();
			}
			return DataBus.EngineInfo.EngineDragPower(DataBus.EngineInfo.EngineSpeed);
        }

		private Watt GetICEDragLossPCC(IDataBus dataBus)
		{
			if (PowertrainArchitecture.IsOneOf(VectoSimulationJobType.SerialHybridVehicle,
					VectoSimulationJobType.IEPC_S, VectoSimulationJobType.FCHV, VectoSimulationJobType.FCHV_IEPC)) {
				return 0.SI<Watt>();
			}
			var iceDragLoss = 0.SI<Watt>();
			if (GearboxInfo.GearboxType.AutomaticTransmission() &&
			    GearboxInfo.GearboxType != GearboxType.IHPC) {
				switch (ADAS.EcoRoll) {
					case EcoRollType.None:
						iceDragLoss = DataBus.EngineInfo.EngineDragPower(DataBus.EngineInfo.EngineSpeed);
                        break;
					case EcoRollType.WithoutEngineStop:
						iceDragLoss = ATEcoRollReleaseLockupClutch
							? 0.SI<Watt>()
							: DataBus.EngineInfo.EngineDragPower(DataBus.EngineInfo.EngineSpeed);
						break;
					case EcoRollType.WithEngineStop:
						iceDragLoss = 0.SI<Watt>();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else {
				if (ADAS.EcoRoll == EcoRollType.None) {
					iceDragLoss = DataBus.EngineInfo.EngineDragPower(DataBus.EngineInfo.EngineSpeed);
				}
			}

			return iceDragLoss;
		}

		private Joule CalculateEnergy(Meter altitude, MeterPerSecond velocity, Kilogram mass) =>
			mass * Physics.GravityAccelleration * altitude + mass * velocity * velocity / 2;

		private void HandleEcoRoll(Second absTime, MeterPerSecond targetVelocity)
		{
			var dBus = DataBus;
			var vehicleSpeedAboveLowerThreshold = dBus.VehicleInfo.VehicleSpeed >= Driver.DriverData.EcoRoll.MinSpeed;
			var slopeNegative = dBus.DrivingCycleInfo.RoadGradient.IsSmaller(0);
			// potential optimization...
			//if (EcoRollState.State != EcoRollStates.EcoRollOn && !slopeNegative) {
			//	EcoRollState.State = EcoRollStates.EcoRollOff;
			//	return;
			//}
			var forces = dBus.VehicleInfo.SlopeResistance(dBus.DrivingCycleInfo.RoadGradient) + dBus.VehicleInfo.RollingResistance(dBus.DrivingCycleInfo.RoadGradient) +
						dBus.VehicleInfo.AirDragResistance(dBus.VehicleInfo.VehicleSpeed, dBus.VehicleInfo.VehicleSpeed).AirdragForce;

			if (GearboxInfo.GearboxType.AutomaticTransmission() && ATEcoRollReleaseLockupClutch && dBus.VehicleInfo.VehicleSpeed.IsGreater(0)) {
				// for AT transmissions consider engine drag losses during eco-roll events
				forces -= dBus.EngineInfo.EngineDragPower(dBus.EngineInfo.EngineSpeed) / dBus.VehicleInfo.VehicleSpeed;
				forces += (GearboxInfo.GearboxLoss() + dBus.AxlegearInfo(AxleNumber.NOT_IN_AXLE_POWERTRAIN).AxlegearLoss()) / dBus.VehicleInfo.VehicleSpeed;
			}
			var accelerationWithinLimits = (-forces / dBus.VehicleInfo.TotalMass).IsBetween(
				Driver.DriverData.EcoRoll.AccelerationLowerLimit, Driver.DriverData.EcoRoll.AccelerationUpperLimit);
			var accelerationPedalIdle = EcoRollState.AcceleratorPedalIdle;
			var brakeActive = !EcoRollState.PreviousBrakePower.IsEqual(0);
			var vehcleSpeedBelowMax = dBus.VehicleInfo.VehicleSpeed <=
									(ApplyOverspeed(dBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed) - 2.KMPHtoMeterPerSecond());

			EcoRollState.AllConditionsMet = vehicleSpeedAboveLowerThreshold && vehcleSpeedBelowMax && slopeNegative && accelerationWithinLimits &&
											accelerationPedalIdle && !brakeActive;

			EcoRollState.Gear = GearboxInfo.Gear;
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
				default: throw new ArgumentOutOfRangeException();
			}

			switch (EcoRollState.State) {
				case EcoRollStates.EcoRollOn:
					if (dBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN) != null) {
						dBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN).DisengageGearbox = true;
					}
			
					if (ADAS.EcoRoll == EcoRollType.WithEngineStop) {
						dBus.EngineCtl.CombustionEngineOn = false;
					}
					return;
				case EcoRollStates.EcoRollOff:
					if (dBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN) != null) {
						dBus.GearboxCtl(AxleNumber.NOT_IN_AXLE_POWERTRAIN).DisengageGearbox = false;
					} 
					
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
			if (DataBus.DrivingCycleInfo.CycleData.LeftSample.PTOActive != PTOActivity.Inactive) {
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
					DataBus.EngineCtl.CombustionEngineOn = false;
				}
			}
			if (EngineOffTimestamp != null &&
				(absTime - EngineOffTimestamp).IsGreaterOrEqual(Driver.DriverData.EngineStopStart.MaxEngineOffTimespan)) {
				DataBus.EngineCtl.CombustionEngineOn = true;
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
			if (DataBus.VehicleInfo.VehicleSpeed > NextDrivingAction.NextTargetSpeed) {
				var brakingDistance = Driver.ComputeDecelerationDistance(NextDrivingAction.NextTargetSpeed) + BrakingSafetyMargin;
				switch (NextDrivingAction.Action) {
					case DrivingBehavior.Coasting:

						//var coastingDistance = ComputeCoastingDistance(DataBus.VehicleSpeed, NextDrivingAction.NextTargetSpeed);
						var coastingDistance = ComputeCoastingDistance(DataBus.VehicleInfo.VehicleSpeed, NextDrivingAction.CycleEntry);
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

		public virtual MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed)
		{
			return (targetSpeed + GetOverspeed()).LimitTo(
					0.KMPHtoMeterPerSecond(), VehicleCategory.IsBus() ? Constants.BusParameters.MaxBusSpeed : 500.KMPHtoMeterPerSecond());

		}

		protected internal MeterPerSecond GetOverspeed()
		{
			return ADAS.PredictiveCruiseControl == PredictiveCruiseControlType.Option_1_2_3 && DataBus.DrivingCycleInfo.CycleData.LeftSample.Highway
				? Driver.DriverData.PCC.OverspeedUseCase3
				: Driver.DriverData.OverSpeed.OverSpeed;
		}

		protected internal DrivingBehaviorEntry GetNextDrivingAction(Meter ds)
		{
			var currentSpeed = DataBus.VehicleInfo.VehicleSpeed;

			var lookaheadDistance =
				(currentSpeed.Value() * 3.6 * Driver.DriverData.LookAheadCoasting.LookAheadDistanceFactor).SI<Meter>();
			var stopDistance = Driver.ComputeDecelerationDistance(0.SI<MeterPerSecond>());
			lookaheadDistance = VectoMath.Max(2 * ds, lookaheadDistance, 1.2 * stopDistance + ds);
			var lookaheadData = DataBus.DrivingCycleInfo.LookAhead(lookaheadDistance);

			Log.Debug("Lookahead distance: {0} @ current speed {1}", lookaheadDistance, currentSpeed);
			var nextActions = new List<DrivingBehaviorEntry>();
			foreach (var entry in lookaheadData) {
				var nextTargetSpeed = IsOverspeedAllowed(entry.VehicleTargetSpeed)
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

		protected internal virtual Meter ComputeCoastingDistance(MeterPerSecond vehicleSpeed,
			DrivingCycleData.DrivingCycleEntry actionEntry)
		{
			var targetSpeed = actionEntry.VehicleTargetSpeed;
			if (IsOverspeedAllowed(actionEntry.VehicleTargetSpeed)) {
				targetSpeed += Driver.DriverData.OverSpeed.OverSpeed;
			}

			var vehicleMass = DataBus.VehicleInfo.TotalMass + DataBus.WheelsInfo.ReducedMassWheels;
			var targetAltitude = actionEntry.Altitude;
			var vehicleAltitude = DataBus.DrivingCycleInfo.Altitude;

			var kineticEnergyAtTarget = vehicleMass * Physics.GravityAccelleration * targetAltitude
											+ vehicleMass * targetSpeed * targetSpeed / 2;
			var currentKineticEnergy = vehicleMass * Physics.GravityAccelleration * vehicleAltitude
											+ vehicleMass * vehicleSpeed * vehicleSpeed / 2;
			var energyDifference = currentKineticEnergy - kineticEnergyAtTarget;

			var airDrag = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, targetSpeed);
			var rollingResistanceForce = DataBus.VehicleInfo.RollingResistance(
				((targetAltitude - vehicleAltitude) / (actionEntry.Distance - DataBus.MileageCounter.Distance))
				.Value().SI<Radian>());

			var gearboxLossPower = GearboxInfo?.GearboxLoss() ?? 0.SI<Watt>();
			var axleLossPower = AxlegearInfo?.AxlegearLoss() ?? 0.SI<Watt>();
			var emDragLossPower = CalculateElectricMotorDragLoss();
			var iceDragLossPower = GetICEDragLossLookAheadCoasting(DataBus); //DataBus.EngineInfo?.EngineDragPower(DataBus.EngineInfo.EngineSpeed) ?? 0.SI<Watt>();

			var totalComponentLossPowers = gearboxLossPower + axleLossPower + emDragLossPower - iceDragLossPower;
			var coastingResistanceForce = airDrag.AirdragForce + rollingResistanceForce + totalComponentLossPowers / vehicleSpeed;

			var coastingDecisionFactor = Driver.DriverData.LookAheadCoasting.LookAheadDecisionFactor.Lookup(
				targetSpeed, vehicleSpeed - targetSpeed);
			var coastingDistance = (energyDifference / (coastingDecisionFactor * coastingResistanceForce)).Cast<Meter>();
			return coastingDistance;
		}

		private Watt CalculateElectricMotorDragLoss()
		{
			var sum = 0.SI<Watt>();
			var db = DataBus;
			foreach (var emInfo in db.ElectricMotorsInfo.Where(x => x.Position != PowertrainPosition.GEN))
				sum += emInfo.DragPower(
					db.BatteryInfo.InternalVoltage,
					emInfo.ElectricMotorSpeed, db.GearboxesInfo.First(x => x.AxleNumber == emInfo.AxleNumber).Gear);
			return sum;
		}

		public bool IsOverspeedAllowed(MeterPerSecond velocity, bool prohibitOverspeed = false) =>
			!prohibitOverspeed
			// allow overspeed either if enabled in the driver model, or ADAS PCC option 3 is enabled in the vehicle and we are on a highway
			&& (Driver.DriverData.OverSpeed.Enabled || ADAS.PredictiveCruiseControl == PredictiveCruiseControlType.Option_1_2_3 && DataBus.DrivingCycleInfo.CycleData.LeftSample.Highway)
			&& velocity > Driver.DriverData.OverSpeed.MinSpeed
			&& ApplyOverspeed(velocity) < (DataBus.VehicleInfo.MaxVehicleSpeed ?? 500.KMPHtoMeterPerSecond());
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

	[DebuggerDisplay("ActionDistance: {ActionDistance}, TriggerDistance: {TriggerDistance}, Action: {Action}")]
	public class DrivingBehaviorEntry
	{
		public DrivingBehavior Action;
		public MeterPerSecond NextTargetSpeed;
		public Meter TriggerDistance;

		public Meter ActionDistance => VectoMath.Min(
			CoastingStartDistance ?? double.MaxValue.SI<Meter>(),
			BrakingStartDistance ?? double.MaxValue.SI<Meter>());

		public Meter SelectActionDistance(Meter minDistance) =>
			new[] { BrakingStartDistance, CoastingStartDistance }.OrderBy(x => x.Value()).First(x => x >= minDistance);

		public Meter CoastingStartDistance { get; set; }

		public Meter BrakingStartDistance { get; set; }

		public DrivingCycleData.DrivingCycleEntry CycleEntry;

		public bool HasEqualTrigger(DrivingBehaviorEntry other) =>
			TriggerDistance.IsEqual(other.TriggerDistance) && NextTargetSpeed.IsEqual(other.NextTargetSpeed);

		public override string ToString() =>
			$"action: {Action} @ {CoastingStartDistance} / {BrakingStartDistance}. trigger: {TriggerDistance} targetSpeed: {NextTargetSpeed}";
	}
}

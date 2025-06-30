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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class TorqueConverter : StatefulVectoSimulationComponent<TorqueConverter.TorqueConverterComponentState>,
		ITnInPort, ITnOutPort, ITorqueConverter, IUpdateable
	{
		protected readonly IGearboxInfo Gearbox;
		protected readonly IShiftStrategy ShiftStrategy;
		protected readonly TorqueConverterData ModelData;
		private readonly KilogramSquareMeter _engineInertia;

		public ITnOutPort NextComponent { protected internal get; set; }

		public TorqueConverter(
			IGearboxInfo gearbox, IShiftStrategy shiftStrategy, IVehicleContainer container,
			TorqueConverterData tcData, VectoRunData runData) : base(container)
		{
			Gearbox = gearbox;
			ShiftStrategy = shiftStrategy;
			ModelData = tcData;
			_engineInertia = runData != null && runData.EngineData != null
				? runData.EngineData.Inertia
				: 0.SI<KilogramSquareMeter>();
		}

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var operatingPointList = ModelData.FindOperatingPoint(outTorque, outAngularVelocity, DataBus.EngineInfo.EngineIdleSpeed);
			TorqueConverterOperatingPoint operatingPoint;
			if (operatingPointList.Count > 0) {
				operatingPoint = SelectOperatingPoint(operatingPointList);
			} else {
				if (!DataBus.IsTestPowertrain) {
					if (outTorque.IsEqual(0) && outAngularVelocity.IsEqual(0)) {
						Log.Info(
							"TorqueConverter Initialize: No operating point found. Using output as input values as fallback for initialize.");
					} else {
						Log.Warn(
							"TorqueConverter Initialize: No operating point found. Using output as input values as fallback for initialize.");
					}
				}

				var inAngularVelocity = outAngularVelocity.LimitTo(DataBus.EngineInfo.EngineIdleSpeed, DataBus.EngineInfo.EngineN95hSpeed);
				operatingPoint = new TorqueConverterOperatingPoint {
					OutAngularVelocity = outAngularVelocity,
					OutTorque = outTorque,
					InAngularVelocity = inAngularVelocity,
					InTorque = outTorque * (outAngularVelocity / inAngularVelocity)
				};
			}
			var retVal = NextComponent.Initialize(operatingPoint.InTorque, operatingPoint.InAngularVelocity);
			PreviousState.SetState(
				operatingPoint.InTorque, operatingPoint.InAngularVelocity, operatingPoint.OutTorque,
				operatingPoint.OutAngularVelocity);
			return retVal;
		}

		public IResponse Request(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			var operatingPoint = FindOperatingPoint(absTime, dt, outTorque, outAngularVelocity);
			var inTorque = CalculateAverageInTorque(operatingPoint);

			if (dryRun) {
				var retValD = HandleDryRun(absTime, dt, outTorque, outAngularVelocity, inTorque, operatingPoint);
				retValD.TorqueConverter.TorqueConverterTorqueDemand = outTorque;
				return retValD;
			}

			// normal request

			// check if out-side of the operating point is equal to requested values
			if (!outAngularVelocity.IsEqual(operatingPoint.OutAngularVelocity) || !outTorque.IsEqual(operatingPoint.OutTorque)) {
				var delta = (outTorque - operatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + operatingPoint.OutAngularVelocity) / 2.0;
				var engineResponse = NextComponent.Request(absTime, dt, operatingPoint.InTorque,
					operatingPoint.InAngularVelocity, true);
				if (!delta.IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
					var retVal1 = delta > 0
						? new ResponseOverload(this) {
							Delta = delta, TorqueConverter = {TorqueConverterOperatingPoint = operatingPoint },
							
						}
						: (IResponse)
						new ResponseUnderload(this) { Delta = delta, TorqueConverter = { TorqueConverterOperatingPoint = operatingPoint },
						};

					retVal1.Engine.EngineSpeed = engineResponse.Engine.EngineSpeed;
					retVal1.Engine.EngineOn = DataBus.EngineCtl.CombustionEngineOn;
					retVal1.Engine.TorqueOutDemand = engineResponse.Engine.TorqueOutDemand;
					retVal1.Engine.TotalTorqueDemand = engineResponse.Engine.TotalTorqueDemand;
					retVal1.Engine.DynamicFullLoadPower = engineResponse.Engine.DynamicFullLoadPower;
					retVal1.Engine.DynamicFullLoadTorque = engineResponse.Engine.DynamicFullLoadTorque;
					retVal1.Engine.DragPower = engineResponse.Engine.DragPower;
					retVal1.Engine.DragTorque = engineResponse.Engine.DragTorque;

					retVal1.ElectricSystem = engineResponse.ElectricSystem;
					retVal1.ElectricMotor.TorqueRequest = engineResponse.ElectricMotor.TorqueRequest;
					retVal1.ElectricMotor.InertiaTorque = engineResponse.ElectricMotor.InertiaTorque;
					retVal1.ElectricMotor.TotalTorqueDemand = engineResponse.ElectricMotor.TotalTorqueDemand;

                    retVal1.ElectricMotor.MaxDriveTorque = engineResponse.ElectricMotor.MaxDriveTorque;
                    retVal1.ElectricMotor.MaxDriveTorqueEM = engineResponse.ElectricMotor.MaxDriveTorqueEM;
                    retVal1.ElectricMotor.MaxRecuperationTorque = engineResponse.ElectricMotor.MaxRecuperationTorque;
                    retVal1.ElectricMotor.MaxRecuperationTorqueEM = engineResponse.ElectricMotor.MaxRecuperationTorqueEM;
                    retVal1.ElectricMotor.AngularVelocity = engineResponse.ElectricMotor.AngularVelocity;
                    retVal1.ElectricMotor.ElectricMotorPowerMech = engineResponse.ElectricMotor.ElectricMotorPowerMech;
                    return retVal1;
				}
			}

			CurrentState.SetState(inTorque, operatingPoint.InAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.OperatingPoint = operatingPoint;
			CurrentState.IgnitionOn = DataBus.EngineCtl.CombustionEngineOn;

			var retVal = NextComponent.Request(absTime, dt, inTorque, operatingPoint.InAngularVelocity, false);
			//retVal.TorqueConverterOperatingPoint = operatingPoint;
			// check if shift is required
			var ratio = Gearbox.GetGearData(Gearbox.Gear.Gear).TorqueConverterRatio;
			if (!Gearbox.DisengageGearbox && absTime > DataBus.GearboxInfo.LastShift && retVal is ResponseSuccess) {
				var shiftRequired = ShiftStrategy?.ShiftRequired(
					absTime, dt, outTorque * ratio, outAngularVelocity / ratio, inTorque,
					operatingPoint.InAngularVelocity, Gearbox.Gear, Gearbox.LastShift, retVal) ?? false;
				return shiftRequired ? new ResponseGearShift(this) : retVal;
			}

			return retVal;
		}

		private NewtonMeter CalculateAverageInTorque(TorqueConverterOperatingPoint operatingPoint)
		{
			var prevInSpeed = PreviousState.IgnitionOn ? PreviousState.InAngularVelocity : DataBus.EngineInfo.EngineIdleSpeed;
			var avgEngineSpeed = (prevInSpeed + operatingPoint.InAngularVelocity) / 2;
			//var avgEngineSpeed = (PreviousState.InAngularVelocity + operatingPoint.InAngularVelocity) / 2;
			//var prevInSpeed = PreviousState.InAngularVelocity;

			////var prevInSpeed = PreviousState.OperatingPoint?.InAngularVelocity ?? PreviousState.InAngularVelocity;
			////var prevInTorque = PreviousState.OperatingPoint?.InTorque ?? PreviousState.InTorque;
			var prevInTorque = PreviousState.InTorque;
			var avgPower = (prevInSpeed * prevInTorque +
							operatingPoint.InAngularVelocity * operatingPoint.InTorque) / 2;
			var inTorque = avgPower / avgEngineSpeed;
			return inTorque;
		}

		private ResponseDryRun HandleDryRun(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, TorqueConverterOperatingPoint operatingPoint)
		{
			// dry run request
			var engineResponse = (ResponseDryRun)
				NextComponent.Request(absTime, dt, inTorque, operatingPoint.InAngularVelocity, true);
			var maxEngineSpeed = DataBus.EngineInfo.EngineN95hSpeed;

			var engineOK = engineResponse.DeltaDragLoad.IsGreaterOrEqual(0) && engineResponse.DeltaFullLoad.IsSmallerOrEqual(0);
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Braking && engineOK && operatingPoint.Creeping) {
				var delta = (outTorque - operatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + operatingPoint.OutAngularVelocity) / 2.0;
				return new ResponseDryRun(this) {
					DeltaFullLoad = delta,
					DeltaDragLoad = delta,
					DeltaDragLoadTorque = outTorque - operatingPoint.OutTorque,
					DeltaFullLoadTorque = outTorque - operatingPoint.OutTorque,

					DeltaEngineSpeed = operatingPoint.InAngularVelocity - maxEngineSpeed,
					TorqueConverter = { TorqueConverterOperatingPoint = operatingPoint},
					Engine = {
						TorqueOutDemand = inTorque,
						EngineSpeed = engineResponse.Engine.EngineSpeed,
						PowerRequest = engineResponse.Engine.PowerRequest,
						//
						TotalTorqueDemand = engineResponse.Engine.TotalTorqueDemand,
						DynamicFullLoadTorque = engineResponse.Engine.DynamicFullLoadTorque,
						DragTorque = engineResponse.Engine.DragTorque,
                    }
                };
			}

			var dryOperatingPointMax = GetMaxPowerOperatingPoint(
				absTime, dt, outAngularVelocity, engineResponse,
				PreviousState.InTorque * PreviousState.InAngularVelocity);
			if (dryOperatingPointMax == null) {
				var engineInternalTorque = (Formulas.InertiaPower(engineResponse.Engine.EngineSpeed,
												DataBus.EngineInfo.EngineSpeed, _engineInertia, dt) +
											engineResponse.Engine.AuxiliariesPowerDemand) /
											((engineResponse.Engine.EngineSpeed + DataBus.EngineInfo.EngineIdleSpeed) /
											2);

				dryOperatingPointMax = ModelData.FindOperatingPointForward(engineResponse.Engine.DynamicFullLoadTorque - engineInternalTorque,
					engineResponse.Engine.EngineSpeed, outAngularVelocity);
					//dryOperatingPointMax = ModelData.LookupOperatingPoint(outAngularVelocity,
					//	DataBus.EngineInfo.EngineIdleSpeed, outTorque);
			}
			var deltaMax = double.MaxValue.SI<Watt>() / 20;
			if (dryOperatingPointMax != null) {
				var avgOutSpeedMax = (PreviousState.OutAngularVelocity + dryOperatingPointMax.OutAngularVelocity) / 2.0;
				deltaMax = (outTorque - dryOperatingPointMax.OutTorque) * avgOutSpeedMax;
			}

			var dryOperatingPointMin = GetDragPowerOperatingPoint(
				absTime, dt, outAngularVelocity, engineResponse,
				PreviousState.InTorque * PreviousState.InAngularVelocity);
			var deltaMin = -double.MaxValue.SI<Watt>() / 20;
			if (dryOperatingPointMin != null) {
				var avgOutSpeedMin = (PreviousState.OutAngularVelocity + dryOperatingPointMin.OutAngularVelocity) / 2.0;
				deltaMin = (outTorque - dryOperatingPointMin.OutTorque) * avgOutSpeedMin;
			}

			return new ResponseDryRun(this) {
				DeltaFullLoad = 10 * deltaMax,
				DeltaDragLoad = 10 * deltaMin,
				// TODO! delta full/drag torque
				//TorqueConverterOperatingPoint = DataBus.DrivingAction == DrivingAction.Brake ? dryOperatingPointMin : dryOperatingPointMax
				DeltaEngineSpeed = dryOperatingPointMax.InAngularVelocity - maxEngineSpeed,
				TorqueConverter = { TorqueConverterOperatingPoint = dryOperatingPointMax},
				Engine = {
					TorqueOutDemand = inTorque,
					EngineSpeed = dryOperatingPointMax?.InAngularVelocity ??
								dryOperatingPointMin?.InAngularVelocity ?? 0.RPMtoRad(),
					PowerRequest = engineResponse.Engine.PowerRequest,
					//
					TotalTorqueDemand = engineResponse.Engine.TotalTorqueDemand,
					DynamicFullLoadTorque = engineResponse.Engine.DynamicFullLoadTorque,
					DragTorque = engineResponse.Engine.DragTorque,
				}
            };
		}

		private TorqueConverterOperatingPoint GetDragPowerOperatingPoint(
			Second absTime, Second dt, PerSecond outAngularVelocity,
			ResponseDryRun engineResponse, Watt previousPower)
		{
			try {
				var emPower = DataBus.ElectricMotorInfo(PowertrainPosition.HybridP1) == null
					? 0.SI<Watt>()
					: engineResponse.ElectricMotor.ElectricMotorPowerMech;
				var operatingPoint = ModelData.FindOperatingPointForPowerDemand(
					engineResponse.Engine.DragPower - engineResponse.Engine.AuxiliariesPowerDemand - emPower,
					DataBus.EngineInfo.EngineSpeed, outAngularVelocity, _engineInertia, dt, previousPower);
				var maxInputSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineInfo.EngineN95hSpeed);
				var lowerInputSpeed = DataBus.DriverInfo.DrivingAction == DrivingAction.Brake
					? DataBus.EngineInfo.EngineIdleSpeed * 1.001
					: VectoMath.Max(DataBus.EngineInfo.EngineIdleSpeed * 1.001, 0.8 * DataBus.EngineInfo.EngineSpeed);
				var corrected = false;
				if (operatingPoint.InAngularVelocity.IsGreater(maxInputSpeed, 1e-2)) {
					operatingPoint = ModelData.FindOperatingPoint(maxInputSpeed, outAngularVelocity);
					corrected = true;
				}
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed * 1.001, 1e-2)) {
					operatingPoint = ModelData.FindOperatingPoint(lowerInputSpeed, outAngularVelocity);
					corrected = true;
				}
				if (!corrected) {
					return operatingPoint;
				}

				operatingPoint = FindValidTorqueConverterOperatingPoint(
					absTime, dt, outAngularVelocity, operatingPoint.InAngularVelocity,
					x => x.DeltaDragLoad.IsGreater(0),
					x => VectoMath.Abs(DataBus.EngineInfo.EngineSpeed - x.Engine.EngineSpeed).Value());
				return operatingPoint;
			} catch (VectoException ve) {
				if (!DataBus.IsTestPowertrain) {
					Log.Warn(ve, "TorqueConverter: Failed to find operating point for DragPower {0}",
						engineResponse.Engine.DragPower);
					//var engineSpeed = VectoMath.Max(DataBus.EngineIdleSpeed * 1.001, 0.8 * DataBus.EngineSpeed);
				}

				var engineSpeed = DataBus.DriverInfo.DrivingAction == DrivingAction.Brake
					? DataBus.EngineInfo.EngineIdleSpeed * 1.001
					: VectoMath.Max(DataBus.EngineInfo.EngineIdleSpeed * 1.001, 0.8 * DataBus.EngineInfo.EngineSpeed);

				var retVal = FindValidTorqueConverterOperatingPoint(
					absTime, dt, outAngularVelocity, engineSpeed,
					x => x.DeltaDragLoad.IsGreater(0),
					x => VectoMath.Abs(DataBus.EngineInfo.EngineSpeed - x.Engine.EngineSpeed).Value());
				if (retVal != null) {
					retVal.Creeping = true;
				}

				return retVal;
			} 
		}


		private TorqueConverterOperatingPoint GetMaxPowerOperatingPoint(
			Second absTime,
			Second dt, PerSecond outAngularVelocity,
			ResponseDryRun engineResponse, Watt previousPower)
		{
			try {
				var emPower = DataBus.ElectricMotorInfo(PowertrainPosition.HybridP1) == null
					? 0.SI<Watt>()
					: engineResponse.ElectricMotor.ElectricMotorPowerMech;
				var operatingPoint = ModelData.FindOperatingPointForPowerDemand(
					(engineResponse.Engine.DynamicFullLoadPower - engineResponse.Engine.AuxiliariesPowerDemand - emPower),
					DataBus.EngineInfo.EngineSpeed, outAngularVelocity, _engineInertia, dt, previousPower);
				var maxInputSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineInfo.EngineN95hSpeed);
				if (operatingPoint.InAngularVelocity.IsGreater(maxInputSpeed)) {
					//operatingPoint = ModelData.FindOperatingPoint(maxInputSpeed, outAngularVelocity);
					operatingPoint = FindValidTorqueConverterOperatingPoint(
						absTime, dt, outAngularVelocity, maxInputSpeed,
						x => x.DeltaFullLoad.IsSmaller(0),
						x => VectoMath.Abs(DataBus.EngineInfo.EngineSpeed - x.Engine.EngineSpeed).Value());
				}

				if (operatingPoint == null) {
					return null;
				}
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed)) {
					operatingPoint = FindValidTorqueConverterOperatingPoint(absTime, dt, outAngularVelocity,
						DataBus.EngineInfo.EngineIdleSpeed,
						x => x.DeltaFullLoad.IsSmaller(0),
						x => VectoMath.Abs(DataBus.EngineInfo.EngineSpeed - x.Engine.EngineSpeed).Value());
					
				}
				return operatingPoint;
			} catch (VectoException ve) {
				if (!DataBus.IsTestPowertrain) {
					Log.Warn(
						ve, "TorqueConverter: Failed to find operating point for MaxPower {0}",
						engineResponse.Engine.DynamicFullLoadPower);
				}

				var engineSpeed = VectoMath.Max(DataBus.EngineInfo.EngineSpeed, VectoMath.Min(DataBus.EngineInfo.EngineRatedSpeed, DataBus.EngineInfo.EngineSpeed));

				var tqOperatingPoint = FindValidTorqueConverterOperatingPoint(
					absTime, dt, outAngularVelocity, engineSpeed,
					x => x.DeltaFullLoad.IsSmaller(0),
					x => VectoMath.Abs(DataBus.EngineInfo.EngineSpeed - x.Engine.EngineSpeed).Value());
				if (tqOperatingPoint == null) {
					return null;
				}
				tqOperatingPoint.Creeping = true;
				return tqOperatingPoint;
			}
		}


		private TorqueConverterOperatingPoint FindValidTorqueConverterOperatingPoint(
			Second absTime, Second dt, PerSecond outAngularVelocity, PerSecond engineSpeed, Func<ResponseDryRun, bool> selector,
			Func<ResponseDryRun, double> orderFunc)
		{
			var retVal = ModelData.FindOperatingPoint(engineSpeed, outAngularVelocity);
			var inTorqueMin = CalculateAverageInTorque(retVal);
			var engRespMin = (ResponseDryRun)
				NextComponent.Request(absTime, dt, inTorqueMin, retVal.InAngularVelocity, true);
			if (engRespMin.DeltaFullLoad.IsSmallerOrEqual(0) && engRespMin.DeltaDragLoad.IsGreaterOrEqual(0)) {
				return retVal;
			}

			var search = new List<ResponseDryRun>();
			var maxSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineInfo.EngineN95hSpeed);
			for (var n = DataBus.EngineInfo.EngineIdleSpeed;
				n <= maxSpeed;
				n += maxSpeed / 100) {
				var tcOp = ModelData.FindOperatingPoint(n, outAngularVelocity);
				var inTorque = CalculateAverageInTorque(tcOp);
				var res = (ResponseDryRun)NextComponent.Request(absTime, dt, inTorque, tcOp.InAngularVelocity, true);
				if (res.DeltaFullLoad.IsGreater(0) || res.DeltaDragLoad.IsSmaller(0)) {
					continue;
				}

				search.Add(res);
			}

			if (search.Count == 0) {
				return null;
			}
			var selected = search.Where(selector).OrderBy(orderFunc).First();
			retVal = ModelData.FindOperatingPoint(selected.Engine.EngineSpeed, outAngularVelocity);

			return retVal;
		}

		public TorqueConverterOperatingPoint FindOperatingPoint(Second absTime, Second dt,
			NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			if (SetOperatingPoint != null) {
				return SetOperatingPoint;
			}
			var operatingPointList = ModelData.FindOperatingPoint(outTorque, outAngularVelocity, DataBus.EngineInfo.EngineIdleSpeed);
			if (operatingPointList.Count == 0) {
				if (!DataBus.IsTestPowertrain) {
					Log.Debug("TorqueConverter: Failed to find torque converter operating point, fallback: creeping");
				}

				//var tqOperatingPoint = ModelData.FindOperatingPoint(DataBus.EngineInfo.EngineIdleSpeed, outAngularVelocity);
				var tqOperatingPoint = ModelData.FindOperatingPoint(DataBus.EngineInfo.EngineIdleSpeed * 1.00, outAngularVelocity);

				var engineResponse = (ResponseDryRun)
					NextComponent.Request(absTime, dt, tqOperatingPoint.InTorque, tqOperatingPoint.InAngularVelocity, true);

				var engineOK = engineResponse.DeltaDragLoad.IsGreaterOrEqual(0) && engineResponse.DeltaFullLoad.IsSmallerOrEqual(0);
				if (!engineOK) {
					tqOperatingPoint = ModelData.FindOperatingPoint(VectoMath.Max(DataBus.EngineInfo.EngineIdleSpeed, DataBus.EngineInfo.EngineSpeed * 0.9), outAngularVelocity);
				}

				tqOperatingPoint.Creeping = true;
				return tqOperatingPoint;
			}

			var operatingPoint = SelectOperatingPoint(operatingPointList);
			if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed)) {
				throw new VectoException(
					"TorqueConverter: Invalid operating point, inAngularVelocity would be below engine's idle speed: {0}",
					operatingPoint.InAngularVelocity);
			}

			if (operatingPoint.InAngularVelocity.IsEqual(DataBus.EngineInfo.EngineIdleSpeed, 1.RPMtoRad())) {
				operatingPoint.Creeping = true;
			}
			var maxInputSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineInfo.EngineN95hSpeed);
			if (operatingPoint.InAngularVelocity.IsGreater(maxInputSpeed)) {
				operatingPoint = ModelData.FindOperatingPoint(maxInputSpeed, outAngularVelocity);
			}
			return operatingPoint;
		}

		private TorqueConverterOperatingPoint SelectOperatingPoint(IList<TorqueConverterOperatingPoint> operatingPointList)
		{
			if (operatingPointList.Count == 1) {
				return operatingPointList[0];
			}

			foreach (var x in operatingPointList) {
				if ((x.InTorque * x.InAngularVelocity).IsSmallerOrEqual(
						DataBus.EngineInfo.EngineStationaryFullPower(x.InAngularVelocity),
						Constants.SimulationSettings.LineSearchTolerance.SI<Watt>()) &&
					(x.InTorque * x.InAngularVelocity).IsGreaterOrEqual(
						DataBus.EngineInfo.EngineDragPower(x.InAngularVelocity),
						Constants.SimulationSettings.LineSearchTolerance.SI<Watt>())) {
					return x;
				}
			}

			return operatingPointList[0];
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			if (CurrentState.OperatingPoint == null) {
				container[ModalResultField.TorqueConverterTorqueRatio] = 1.0;
				container[ModalResultField.TorqueConverterSpeedRatio] = 1.0;
			} else {
				container[ModalResultField.TorqueConverterTorqueRatio] = CurrentState.OperatingPoint.TorqueRatio;
				container[ModalResultField.TorqueConverterSpeedRatio] = CurrentState.OperatingPoint.SpeedRatio;
			}
			container[ModalResultField.TC_TorqueIn] = CurrentState.InTorque;
			container[ModalResultField.TC_TorqueOut] = CurrentState.OutTorque;
			container[ModalResultField.TC_angularSpeedIn] = CurrentState.InAngularVelocity;
			container[ModalResultField.TC_angularSpeedOut] = CurrentState.OutAngularVelocity;

			var avgOutVelocity = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			var avgInVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.P_TC_out] = CurrentState.OutTorque * avgOutVelocity;
			container[ModalResultField.P_TC_loss] = CurrentState.InTorque * avgInVelocity -
													CurrentState.OutTorque * avgOutVelocity;
			container[ModalResultField.P_TC_in] = CurrentState.InTorque * avgInVelocity;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			SetOperatingPoint = null;
			AdvanceState();
		}

		public void Locked(
			NewtonMeter inTorque, PerSecond inAngularVelocity, NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.IgnitionOn = DataBus.EngineCtl.CombustionEngineOn;
		}

		public class TorqueConverterComponentState : SimpleComponentState
		{
			public TorqueConverterOperatingPoint OperatingPoint;
			public bool IgnitionOn;

			public new TorqueConverterComponentState Clone() => (TorqueConverterComponentState)base.Clone();
		}

		#region Implementation of ITorqueConverterControl

		public Tuple<TorqueConverterOperatingPoint, NewtonMeter> CalculateOperatingPoint(PerSecond inSpeed, PerSecond outSpeed)
		{
			var tcOps = ModelData.FindOperatingPoint(inSpeed, outSpeed);
			
			return Tuple.Create(tcOps, CalculateAverageInTorque(tcOps));
		}

		public TorqueConverterOperatingPoint SetOperatingPoint { get; set; }


		#endregion

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is TorqueConverter tc) {
				PreviousState = tc.PreviousState.Clone();
				return true;
			}
			return false;
		}

		#endregion
	}
}

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
		ITnInPort, ITnOutPort
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
			var operatingPointList = ModelData.FindOperatingPoint(outTorque, outAngularVelocity, DataBus.EngineIdleSpeed);
			TorqueConverterOperatingPoint operatingPoint;
			if (operatingPointList.Count > 0) {
				operatingPoint = SelectOperatingPoint(operatingPointList);
			} else {
				Log.Warn(
					"TorqueConverter Initialize: No operating point found. Using output as input values as fallback for initialize.");
				var inAngularVelocity = outAngularVelocity.LimitTo(DataBus.EngineIdleSpeed, DataBus.EngineN95hSpeed);
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
			bool dryRun = false)
		{
			var operatingPoint = FindOperatingPoint(absTime, dt, outTorque, outAngularVelocity);
			var inTorque = CalculateAverageInTorque(operatingPoint);

			if (dryRun) {
				return HandleDryRun(absTime, dt, outTorque, outAngularVelocity, inTorque, operatingPoint);
			}

			// normal request

			// check if out-side of the operating point is equal to requested values
			if (!outAngularVelocity.IsEqual(operatingPoint.OutAngularVelocity) || !outTorque.IsEqual(operatingPoint.OutTorque)) {
				var delta = (outTorque - operatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + operatingPoint.OutAngularVelocity) / 2.0;
				if (!delta.IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
					return delta > 0
						? new ResponseOverload(this) { Delta = delta, TorqueConverterOperatingPoint = operatingPoint }
						: (IResponse)
						new ResponseUnderload(this) {Delta = delta, TorqueConverterOperatingPoint = operatingPoint };
				}
			}

			CurrentState.SetState(inTorque, operatingPoint.InAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.OperatingPoint = operatingPoint;

			var retVal = NextComponent.Request(absTime, dt, inTorque, operatingPoint.InAngularVelocity);

			// check if shift is required
			var ratio = Gearbox.GetGearData(Gearbox.Gear.Gear).TorqueConverterRatio;
			if (absTime > DataBus.LastShift && retVal is ResponseSuccess) {
				var shiftRequired = ShiftStrategy.ShiftRequired(
					absTime, dt, outTorque * ratio, outAngularVelocity / ratio, inTorque,
					operatingPoint.InAngularVelocity, Gearbox.Gear, Gearbox.LastShift, retVal);
				return shiftRequired ? new ResponseGearShift(this) : retVal;
			}

			return retVal;
		}

		private NewtonMeter CalculateAverageInTorque(TorqueConverterOperatingPoint operatingPoint)
		{
			var avgEngineSpeed = (PreviousState.InAngularVelocity + operatingPoint.InAngularVelocity) / 2;

			//var prevInSpeed = PreviousState.OperatingPoint?.InAngularVelocity ?? PreviousState.InAngularVelocity;
			//var prevInTorque = PreviousState.OperatingPoint?.InTorque ?? PreviousState.InTorque;
			var prevInSpeed = PreviousState.InAngularVelocity;
			var prevInTorque = PreviousState.InTorque;
			var avgPower = (prevInSpeed * prevInTorque +
							operatingPoint.InAngularVelocity * operatingPoint.InTorque) / 2;
			var inTorque = avgPower / avgEngineSpeed;
			return inTorque;
		}

		private IResponse HandleDryRun(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, TorqueConverterOperatingPoint operatingPoint)
		{
			// dry run request
			var engineResponse = (ResponseDryRun)
				NextComponent.Request(absTime, dt, inTorque, operatingPoint.InAngularVelocity, true);

			var engineOK = engineResponse.DeltaDragLoad.IsGreaterOrEqual(0) && engineResponse.DeltaFullLoad.IsSmallerOrEqual(0);
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Braking && engineOK && operatingPoint.Creeping) {
				var delta = (outTorque - operatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + operatingPoint.OutAngularVelocity) / 2.0;
				return new ResponseDryRun(this) {
					DeltaFullLoad = delta,
					DeltaDragLoad = delta,
					TorqueConverterOperatingPoint = operatingPoint
				};
			}

			var dryOperatingPointMax = GetMaxPowerOperatingPoint(
				absTime, dt, outAngularVelocity, engineResponse,
				PreviousState.InTorque * PreviousState.InAngularVelocity);
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
				TorqueConverterOperatingPoint = DataBus.DriverInfo.DrivingAction == DrivingAction.Brake ? dryOperatingPointMin : dryOperatingPointMax
			};
		}

		private TorqueConverterOperatingPoint GetDragPowerOperatingPoint(
			Second absTime, Second dt, PerSecond outAngularVelocity,
			ResponseDryRun engineResponse, Watt previousPower)
		{
			try {
				var operatingPoint = ModelData.FindOperatingPointForPowerDemand(
					engineResponse.DragPower - engineResponse.AuxiliariesPowerDemand,
					DataBus.EngineSpeed, outAngularVelocity, _engineInertia, dt, previousPower);
				var maxInputSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineN95hSpeed);
				var lowerInputSpeed = DataBus.DriverInfo.DrivingAction == DrivingAction.Brake
					? DataBus.EngineIdleSpeed * 1.001
					: VectoMath.Max(DataBus.EngineIdleSpeed * 1.001, 0.8 * DataBus.EngineSpeed);
				var corrected = false;
				if (operatingPoint.InAngularVelocity.IsGreater(maxInputSpeed)) {
					operatingPoint = ModelData.FindOperatingPoint(maxInputSpeed, outAngularVelocity);
					corrected = true;
				}
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					operatingPoint = ModelData.FindOperatingPoint(lowerInputSpeed, outAngularVelocity);
					corrected = true;
				}
				if (!corrected) {
					return operatingPoint;
				}

				operatingPoint = FindValidTorqueConverterOperatingPoint(
					absTime, dt, outAngularVelocity, operatingPoint.InAngularVelocity,
					x => x.DeltaDragLoad.IsGreater(0),
					x => VectoMath.Abs(DataBus.EngineSpeed - x.EngineSpeed).Value());
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Error(ve, "TorqueConverter: Failed to find operating point for DragPower {0}", engineResponse.DragPower);
				var engineSpeed = DataBus.DriverInfo.DrivingAction == DrivingAction.Brake
					? DataBus.EngineIdleSpeed * 1.001
					: VectoMath.Max(DataBus.EngineIdleSpeed * 1.001, 0.8 * DataBus.EngineSpeed);

				var retVal = FindValidTorqueConverterOperatingPoint(
					absTime, dt, outAngularVelocity, engineSpeed,
					x => x.DeltaDragLoad.IsGreater(0),
					x => VectoMath.Abs(DataBus.EngineSpeed - x.EngineSpeed).Value());
				if (retVal != null)
				retVal.Creeping = true;
				return retVal;
			}
		}


		private TorqueConverterOperatingPoint GetMaxPowerOperatingPoint(
			Second absTime,
			Second dt, PerSecond outAngularVelocity,
			ResponseDryRun engineResponse, Watt previousPower)
		{
			try {
				var operatingPoint = ModelData.FindOperatingPointForPowerDemand(
					engineResponse.DynamicFullLoadPower - engineResponse.AuxiliariesPowerDemand,
					DataBus.EngineSpeed, outAngularVelocity, _engineInertia, dt, previousPower);
				var maxInputSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineN95hSpeed);
				if (operatingPoint.InAngularVelocity.IsGreater(maxInputSpeed)) {
					//operatingPoint = ModelData.FindOperatingPoint(maxInputSpeed, outAngularVelocity);
					operatingPoint = FindValidTorqueConverterOperatingPoint(
						absTime, dt, outAngularVelocity, maxInputSpeed,
						x => x.DeltaFullLoad.IsSmaller(0),
						x => VectoMath.Abs(DataBus.EngineSpeed - x.EngineSpeed).Value());
				}
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Error(
					ve, "TorqueConverter: Failed to find operating point for MaxPower {0}",
					engineResponse.DynamicFullLoadPower);
				var engineSpeed = VectoMath.Max(DataBus.EngineSpeed, VectoMath.Min(DataBus.EngineRatedSpeed, DataBus.EngineSpeed));

				var tqOperatingPoint = FindValidTorqueConverterOperatingPoint(
					absTime, dt, outAngularVelocity, engineSpeed,
					x => x.DeltaFullLoad.IsSmaller(0),
					x => VectoMath.Abs(DataBus.EngineSpeed - x.EngineSpeed).Value());
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
			var maxSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineN95hSpeed);
			for (var n = DataBus.EngineIdleSpeed;
				n <= maxSpeed;
				n += maxSpeed / 20) {
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
			retVal = ModelData.FindOperatingPoint(selected.EngineSpeed, outAngularVelocity);

			return retVal;
		}

		protected internal TorqueConverterOperatingPoint FindOperatingPoint(Second absTime, Second dt,
			NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			var operatingPointList = ModelData.FindOperatingPoint(outTorque, outAngularVelocity, DataBus.EngineIdleSpeed);
			if (operatingPointList.Count == 0) {
				Log.Debug("TorqueConverter: Failed to find torque converter operating point, fallback: creeping");
				var tqOperatingPoint = ModelData.FindOperatingPoint(DataBus.EngineIdleSpeed * 1.00, outAngularVelocity);

				var engineResponse = (ResponseDryRun)
					NextComponent.Request(absTime, dt, tqOperatingPoint.InTorque, tqOperatingPoint.InAngularVelocity, true);

				var engineOK = engineResponse.DeltaDragLoad.IsGreaterOrEqual(0) && engineResponse.DeltaFullLoad.IsSmallerOrEqual(0);
				if (!engineOK) {
					tqOperatingPoint = ModelData.FindOperatingPoint(VectoMath.Max(DataBus.EngineIdleSpeed, DataBus.EngineSpeed * 0.9), outAngularVelocity);
				}

				tqOperatingPoint.Creeping = true;
				return tqOperatingPoint;
			}

			var operatingPoint = SelectOperatingPoint(operatingPointList);
			if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				throw new VectoException(
					"TorqueConverter: Invalid operating point, inAngularVelocity would be below engine's idle speed: {0}",
					operatingPoint.InAngularVelocity);
			}

			var maxInputSpeed = VectoMath.Min(ModelData.TorqueConverterSpeedLimit, DataBus.EngineN95hSpeed);
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
						DataBus.EngineStationaryFullPower(x.InAngularVelocity),
						Constants.SimulationSettings.LineSearchTolerance.SI<Watt>()) &&
					(x.InTorque * x.InAngularVelocity).IsGreaterOrEqual(
						DataBus.EngineDragPower(x.InAngularVelocity),
						Constants.SimulationSettings.LineSearchTolerance.SI<Watt>())) {
					return x;
				}
			}

			return operatingPointList[0];
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
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

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}

		public void Locked(
			NewtonMeter inTorque, PerSecond inAngularVelocity, NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
		}

		public class TorqueConverterComponentState : SimpleComponentState
		{
			public TorqueConverterOperatingPoint OperatingPoint;
		}
	}
}

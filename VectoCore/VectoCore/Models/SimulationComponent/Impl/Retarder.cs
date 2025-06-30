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

using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public interface IRetarder : IPowerTrainComponent
	{

	}

    /// <summary>
    /// Retarder component.
    /// </summary>
    public class Retarder : StatefulProviderComponent<SimpleComponentState, ITnOutPort, ITnInPort, ITnOutPort>,
		IRetarder, ITnInPort, ITnOutPort
	{
		private readonly RetarderLossMap _lossMap;
		private readonly double _ratio;
		private bool _primaryRetarder;

		/// <summary>
		/// Creates a new Retarder.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="lossMap"></param>
		/// <param name="ratio"></param>
		public Retarder(IVehicleContainer container, RetarderLossMap lossMap, double ratio) : base(container)
		{
			_lossMap = lossMap;
			_ratio = ratio;
			_primaryRetarder = container.RunData != null && container.RunData.Retarder.Type == RetarderType.TransmissionInputRetarder;
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var retarderTorqueLoss = _lossMap.GetTorqueLoss(angularVelocity * _ratio) * _ratio;
			PreviousState.SetState(torque + retarderTorqueLoss, angularVelocity, torque, angularVelocity);
			return NextComponent.Initialize(PreviousState.InTorque, PreviousState.InAngularVelocity);
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun)
		{
			if (angularVelocity == null || (_primaryRetarder && (!DataBus.ClutchInfo.ClutchClosed(absTime) || !DataBus.GearboxInfo.GearEngaged(absTime)))) {
				return NextComponent.Request(absTime, dt, torque, angularVelocity, dryRun);
			}
			var avgAngularSpeed = (PreviousState.InAngularVelocity + angularVelocity) / 2.0;
			var retarderTorqueLoss = avgAngularSpeed.IsEqual(0, 1e-9) ? 0.SI<NewtonMeter>() : _lossMap.GetTorqueLoss(avgAngularSpeed * _ratio) * _ratio;
			var inTorque = torque + retarderTorqueLoss;
			if (!dryRun) {
				CurrentState.SetState(inTorque, angularVelocity, torque, angularVelocity);
			}
			return NextComponent.Request(absTime, dt, inTorque, angularVelocity, dryRun);
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var avgAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.P_ret_loss] = (CurrentState.InTorque - CurrentState.OutTorque) * avgAngularSpeed;
			container[ModalResultField.P_retarder_in] = CurrentState.InTorque * avgAngularSpeed;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			var avgAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0 * _ratio;
			if (!avgAngularSpeed.IsBetween(_lossMap.MinSpeed, _lossMap.MaxSpeed)) {
				Log.Warn(
					"Retarder LossMap data was extrapolated: range for loss map is not sufficient: n:{0} (min:{1}, max:{2}), ratio:{3}",
					avgAngularSpeed.AsRPM, _lossMap.MinSpeed.AsRPM, _lossMap.MaxSpeed.AsRPM, _ratio);
				if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
					throw new VectoException(
						"Retarder LossMap data was extrapolated in Declaration mode: range for loss map is not sufficient: n:{0} (min:{1}, max:{2}), ratio:{3}",
						avgAngularSpeed.AsRPM, _lossMap.MinSpeed.AsRPM, _lossMap.MaxSpeed.AsRPM, _ratio);
				}
			}
			base.DoCommitSimulationStep(time, simulationInterval);
		}

		protected override bool DoUpdateFrom(object other) => false;
	}
}
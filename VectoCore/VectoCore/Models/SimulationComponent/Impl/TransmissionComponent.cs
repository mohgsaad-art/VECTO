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
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public abstract class TransmissionComponent :
		StatefulVectoSimulationComponent<TransmissionComponent.TransmissionState>, IPowerTrainComponent, ITnInPort,
		ITnOutPort
	{
		protected ITnOutPort NextComponent;
		[ValidateObject] internal readonly TransmissionData ModelData;

		public class TransmissionState : SimpleComponentState
		{
			public TransmissionLossMap.LossMapResult TorqueLossResult;
			//public NewtonMeter TorqueLoss = 0.SI<NewtonMeter>();
			public new TransmissionState Clone() => (TransmissionState)MemberwiseClone();
		}

		protected TransmissionComponent(IVehicleContainer container, TransmissionData modelData) : base(container)
		{
			ModelData = modelData;
		}

		public virtual ITnInPort InPort()
		{
			return this;
		}

		public virtual ITnOutPort OutPort()
		{
			return this;
		}

		public virtual void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public virtual IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			Log.Debug("request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);

			var inAngularVelocity = outAngularVelocity * ModelData.Ratio;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;

			var torqueLossResult = ModelData.LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			if (avgOutAngularVelocity.IsEqual(0, 1e-9)) {
				torqueLossResult.Value = 0.SI<NewtonMeter>();
			}
			var inTorque = outTorque / ModelData.Ratio + torqueLossResult.Value;

			if (!dryRun) {
				CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
				CurrentState.TorqueLossResult = torqueLossResult;
			}
			InTorque = inTorque;
			var retVal = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);
			return retVal;
		}

		protected NewtonMeter InTorque { get; private set; }

		public virtual IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var inAngularVelocity = outAngularVelocity * ModelData.Ratio;
			var torqueLossResult = ModelData.LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorque = outTorque / ModelData.Ratio + torqueLossResult.Value;

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.TorqueLossResult = torqueLossResult;

			return NextComponent.Initialize(inTorque, inAngularVelocity);
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			if (CurrentState.TorqueLossResult.Extrapolated) {
				Log.Warn("{2} LossMap data was extrapolated: range for loss map is not sufficient: n_out:{0}, torque_out:{1}",
					CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque, GetType().Name);

				if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
					throw new VectoException(
						"{2} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n_out:{0}, torque_out:{1}",
						CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque, GetType().Name);
				}
			}
			AdvanceState();
		}
	}
}
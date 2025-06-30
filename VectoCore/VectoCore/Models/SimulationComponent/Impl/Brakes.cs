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
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class Brakes : StatefulProviderComponent<SimpleComponentState, ITnOutPort, ITnInPort, ITnOutPort>,
		IPowerTrainComponent, ITnOutPort, ITnInPort, IBrakes, IUpdateable
	{
		public Watt BrakePower { get; set; }

		public Brakes(IVehicleContainer container) : base(container) {}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			BrakePower = 0.SI<Watt>();
			PreviousState.SetState(torque, angularVelocity, torque, angularVelocity);
			return DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted && DataBus.VehicleInfo.VehicleStopped
				? NextComponent.Initialize(0.SI<NewtonMeter>(), 0.SI<PerSecond>())
				: NextComponent.Initialize(torque, angularVelocity);
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			var brakeTorque = 0.SI<NewtonMeter>();
			var avgAngularSpeed = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;

			if (!BrakePower.IsEqual(0)) {
				if (avgAngularSpeed.IsEqual(0)) {
					brakeTorque = -outTorque;
				} else {
					brakeTorque = BrakePower / avgAngularSpeed;
				}
			}

			if (!dryRun && !DataBus.IsTestPowertrain && BrakePower < 0) {
				throw new VectoSimulationException("Negative Braking Power is not allowed! P_br: {0}", BrakePower);
			}
			CurrentState.SetState(outTorque + brakeTorque, outAngularVelocity, outTorque, outAngularVelocity);

			var retVal = NextComponent.Request(absTime, dt, outTorque + brakeTorque, outAngularVelocity, dryRun);
			retVal.Brakes.BrakePower = brakeTorque * avgAngularSpeed;
			return retVal;
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_brake_loss] = BrakePower;
			container[ModalResultField.P_brake_in] = CurrentState.InTorque *
													(PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			BrakePower = 0.SI<Watt>();
			base.DoCommitSimulationStep(time, simulationInterval);
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is Brakes b) {
				PreviousState = b.PreviousState.Clone();
				BrakePower = b.BrakePower;
				return true;
			}
			return false;
		}

		#endregion
	}
}
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

using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class DummyRetarder : StatefulProviderComponent<SimpleComponentState, ITnOutPort, ITnInPort, ITnOutPort>,
		IPowerTrainComponent, ITnInPort, ITnOutPort
	{
		public DummyRetarder(IVehicleContainer dataBus) : base(dataBus) {}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			PreviousState.SetState(torque, angularVelocity, torque, angularVelocity);
			return NextComponent.Initialize(torque, angularVelocity);
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
		{
			CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
			return NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_ret_loss] = 0.SI<Watt>();

			if (CurrentState.InAngularVelocity == null || PreviousState.InAngularVelocity == null) {
				container[ModalResultField.P_retarder_in] = 0.SI<Watt>();
			} else {
				container[ModalResultField.P_retarder_in] = CurrentState.InTorque *
															(CurrentState.InAngularVelocity + PreviousState.InAngularVelocity) / 2.0;
			}
		}
	}
}
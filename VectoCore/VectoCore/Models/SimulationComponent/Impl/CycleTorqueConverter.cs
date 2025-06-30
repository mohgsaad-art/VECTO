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
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class CycleTorqueConverter : StatefulVectoSimulationComponent<TorqueConverter.TorqueConverterComponentState>
	{
		protected internal ITnOutPort NextComponent;
		private TorqueConverterData ModelData;

		public CycleTorqueConverter(IVehicleContainer container, TorqueConverterData modelData) : base(container)
		{
			ModelData = modelData;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity)
		{
			
			var operatingPoint = ModelData.LookupOperatingPoint(outAngularVelocity, inAngularVelocity, outTorque);

			PreviousState.OperatingPoint = operatingPoint;
			return NextComponent.Initialize(operatingPoint.InTorque, inAngularVelocity);
		}

		public IResponse Request(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity,
			bool dryRun = false)
		{
			var operatingPoint = ModelData.LookupOperatingPoint(outAngularVelocity, inAngularVelocity, outTorque);
			if (!dryRun) {
				CurrentState.OperatingPoint = operatingPoint;
			}
			return NextComponent.Request(absTime, dt, operatingPoint.InTorque, inAngularVelocity, dryRun);
		}

		public void Locked(
			NewtonMeter inTorque, PerSecond inAngularVelocity, NewtonMeter outTorque,
			PerSecond outAngularVelocity) { }

		#region Overrides of VectoSimulationComponent

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
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}
}
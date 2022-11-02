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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AxleGear : TransmissionComponent, IAxlegear
	{
		public AxleGear(IVehicleContainer container, AxleGearData modelData) : base(container, modelData.AxleGear) { }

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var retVal = base.Initialize(outTorque, outAngularVelocity);
			retVal.Axlegear.PowerRequest = outTorque * outAngularVelocity;
			retVal.Axlegear.CardanTorque = PreviousState.InTorque;
			return retVal;
		}

		public override IResponse Request(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			var retVal = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
			retVal.Axlegear.PowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			retVal.Axlegear.CardanTorque = InTorque;
			retVal.Axlegear.OutputTorque = outTorque;
			retVal.Axlegear.OutputSpeed = outAngularVelocity;
			return retVal;
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var avgAngularVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.P_axle_loss] = (CurrentState.InTorque - CurrentState.OutTorque / ModelData.Ratio) *
													avgAngularVelocity;
			container[ModalResultField.P_axle_in] = CurrentState.InTorque * avgAngularVelocity;
		}

		public Watt AxlegearLoss()
		{
			return PreviousState.TorqueLossResult.Value * PreviousState.InAngularVelocity;
		}

		public Tuple<PerSecond, NewtonMeter> CurrentAxleDemand =>
			Tuple.Create(
				(PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0, CurrentState.InTorque);

		public double Ratio => ModelData.Ratio;
	}
}

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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl {
	internal class ExemptedRun : VectoRun
	{
		private Action<ModalDataContainer> _writeSumData;

		public ExemptedRun(VehicleContainer data, Action<ModalDataContainer> writeSumData) : base(data)
		{
			_writeSumData = writeSumData;
		}

		#region Overrides of VectoRun

		public override double Progress => 1;

		public override string CycleName => "ExemptedVehicle";

		public override string RunSuffix => "";

		protected override IResponse DoSimulationStep()
		{
			CheckValidInput();
			FinishedWithoutErrors = true;
			_writeSumData(null);
			return new ResponseCycleFinished(this);
		}

		private void CheckValidInput()
		{
			if (Container.RunData.MultistageRun) {
				return;
			}
			var vehicleData = Container.RunData.VehicleData;
			if (vehicleData.ZeroEmissionVehicle && vehicleData.DualFuelVehicle) {
				throw new VectoException("Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!");
			}

			if (!vehicleData.ZeroEmissionVehicle && !vehicleData.HybridElectricHDV && !vehicleData.DualFuelVehicle) {
				throw new VectoException("Invalid input: at least one option of ZE-HDV, He-HDV, and DualFuelVehicle has to be set for an exempted vehicle!");
			}

			if (vehicleData.HybridElectricHDV && (vehicleData.MaxNetPower1 == null || vehicleData.MaxNetPower2 == null)) {
				throw new VectoException("For He-HDV both MaxNetPower1 and MaxNetPower2 have to be provided!");
			}
		}

		protected override IResponse Initialize()
		{
			return new ResponseSuccess(this);
		}

		#endregion
	}
}
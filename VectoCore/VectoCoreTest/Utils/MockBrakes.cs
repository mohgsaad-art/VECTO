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

using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
    public class MockBrakes : VectoSimulationComponent, IBrakes, IUpdateable
	{
		public MockBrakes(IVehicleContainer vehicle) : base(vehicle)
		{
			BrakePower = 0.SI<Watt>();
		}

		public Watt BrakePower { get; set; }

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container) {}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval) {}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other)
		{
			if (other is IBrakes b) {
				BrakePower = b.BrakePower;
				return true;
			}

			return false;
		}

		#endregion

		#region Implementation of ITnInProvider

		public ITnInPort InPort()
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region Implementation of ITnOutProvider

		public ITnOutPort OutPort()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
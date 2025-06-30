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

using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class WheelsTest
	{
		private const string VehicleDataFile = @"TestData/Components/24t Coach.vveh";

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void WheelsRequestTest()
		{
			var container = VehicleContainer.CreateVehicleContainer(null, null, null);
			//var reader = new EngineeringModeSimulationDataReader();
			var vehicleData = MockSimulationDataFactory.CreateVehicleDataFromFile(VehicleDataFile);

			IWheels wheels = new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia);
			var mockPort = new MockTnOutPort();

			wheels.InPort().Connect(mockPort);

			var requestPort = wheels.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var force = 5000.SI<Newton>();
			var velocity = 20.SI<MeterPerSecond>();

			requestPort.Initialize(force, velocity);

			var retVal = requestPort.Request(absTime, dt, force, velocity, false);

			Assert.AreEqual(2600.0, mockPort.Torque.Value(), 0.0001);
			Assert.AreEqual(38.4615384615, mockPort.AngularVelocity.Value(), 0.0001);
		}
	}
}
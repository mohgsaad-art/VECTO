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
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.DrivingCycleData;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class EngineFanAuxTest
	{

		[
		TestCase(200 ,  53.998),
		TestCase(500,    843.713),
		TestCase(1000,   6749.700),
		TestCase(1300,   14829.091),
		TestCase(1500,   22780.238),
		Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestEngineFanPowerDemand(double fanSpeedRPM, double expectedPowerDemand)
		{
			var engineFan = new EngineFanAuxiliary(new[] { 5.5e-7, 14.62, 108.5 }, 0.225.SI<Meter>());

			Assert.AreEqual(expectedPowerDemand, engineFan.PowerDemand(
				new DrivingCycleEntry() { FanSpeed = fanSpeedRPM.RPMtoRad() }).Value(), 1e-3);
        }

        [
        TestCase(14, 20),
        TestCase(28, 40),
        TestCase(7, 10),
		Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void TestEngineFanPowerDemandWithElectricalPower(double fanElectricalPower, double expectedPowerDemand)
        {
			var engineFan = new EngineFanAuxiliary(new[] { 5.5e-7, 14.62, 108.5 }, 0.225.SI<Meter>());

			Assert.AreEqual(expectedPowerDemand, engineFan.PowerDemand(
				new DrivingCycleEntry() { FanElectricalPower = fanElectricalPower.SI<Watt>() }).Value(), 1e-3);
		}

    }
}
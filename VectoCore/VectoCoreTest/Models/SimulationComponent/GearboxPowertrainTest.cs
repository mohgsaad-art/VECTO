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

using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class GearboxPowertrainTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void Gearbox_Initialize_Empty()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(new[] {
				// <s>, <v>, <grad>,           <stop>
				"  0,    0,  2.95016969027809, 1",
				"1000,  60,  2.95016969027809, 0",
			});
			var container = Truck40tPowerTrain.CreatePowerTrain(cycle, "Gearbox_Initialize", 7500.0.SI<Kilogram>(),
				0.SI<Kilogram>());
			var retVal = container.Cycle.Initialize();
			Assert.AreEqual(4u, container.GearboxInfo.Gear.Gear);
			Assert.IsInstanceOf<ResponseSuccess>(retVal);

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineInfo.EngineSpeed);

			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();
			container.AbsTime = absTime;

			retVal = container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);
			absTime += retVal.SimulationInterval;

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineInfo.EngineSpeed);

			container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);
			Assert.AreEqual(4u, container.GearboxInfo.Gear.Gear);
			AssertHelper.AreRelativeEqual(65.6890, container.EngineInfo.EngineSpeed);
		}

		[TestCase]
		public void Gearbox_Initialize_RefLoad()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(new[] {
				// <s>, <v>,           <grad>, <stop>
				"    0,   0, 2.95016969027809,      1",
				" 1000,  60, 2.95016969027809,      0",
			});
			var container = Truck40tPowerTrain.CreatePowerTrain(cycle, "Gearbox_Initialize", 7500.0.SI<Kilogram>(),
				19300.SI<Kilogram>());
			var retVal = container.Cycle.Initialize();
			Assert.AreEqual(4u, container.GearboxInfo.Gear.Gear);
			Assert.IsInstanceOf<ResponseSuccess>(retVal);

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineInfo.EngineSpeed);

			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();
			container.AbsTime = absTime;

			retVal = container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);
			absTime += retVal.SimulationInterval;

			AssertHelper.AreRelativeEqual(560.RPMtoRad(), container.EngineInfo.EngineSpeed);

			container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);

			AssertHelper.AreRelativeEqual(87.3192, container.EngineInfo.EngineSpeed);
		}

		[TestCase]
		public void Gearbox_Initialize_85_RefLoad()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(new[] {
				// <s>,<v>,<grad>,<stop>
				"  0,  85, 2.95016969027809,     0",
				" 100, 85, 2.95016969027809,     0",
			});
			var container = Truck40tPowerTrain.CreatePowerTrain(cycle, "Gearbox_Initialize", 7500.0.SI<Kilogram>(),
				19300.SI<Kilogram>());
			var retVal = container.Cycle.Initialize();
			Assert.AreEqual(12u, container.GearboxInfo.Gear.Gear);
			Assert.IsInstanceOf<ResponseSuccess>(retVal);

			AssertHelper.AreRelativeEqual(1195.996.RPMtoRad(), container.EngineInfo.EngineSpeed, toleranceFactor: 1e-3);

			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();
			container.AbsTime = absTime;

			retVal = container.Cycle.Request(absTime, ds);
			container.CommitSimulationStep(absTime, retVal.SimulationInterval);
			absTime += retVal.SimulationInterval;
		}
	}
}
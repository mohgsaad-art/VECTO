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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using DummyDriverInfo = TUGraz.VectoCore.Tests.Utils.DummyDriverInfo;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DrivingCycleTests
	{
		[OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase()]
		//[Parallelizable(ParallelScope.All)]
		public void TestEngineOnly()
		{
			var dataWriter = new MockModalDataContainer();
			var container = VehicleContainer.CreateVehicleContainer(new VectoRunData(), dataWriter, null);

			var cycleData = DrivingCycleDataReader.ReadFromFile(@"TestData/Cycles/Coach Engine Only.vdri", CycleType.EngineOnly,
				false);
			var cycle = new PowertrainDrivingCycle(container, cycleData);
			new EngineOnlyGearboxInfo(container);
			new ZeroMileageCounter(container);
			new DummyDriverInfo(container);

			var outPort = new MockTnOutPort();
			var inPort = cycle.InPort();
			var cycleOut = cycle.OutPort();

			inPort.Connect(outPort);

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var response = cycleOut.Request(absTime, dt);
			Assert.IsInstanceOf<ResponseSuccess>(response);

			var time = absTime + dt / 2;
			var simulationInterval = dt;
			container.CommitSimulationStep(time, simulationInterval);

			Assert.AreEqual(absTime, outPort.AbsTime);
			Assert.AreEqual(dt, outPort.Dt);
			Assert.AreEqual(600.RPMtoRad(), outPort.AngularVelocity);
			Assert.AreEqual(0.SI<NewtonMeter>(), outPort.Torque);
		}

		[TestCase()]
		public void TestEngineOnlyWithTimestamps()
		{
			var container = VehicleContainer.CreateVehicleContainer(new VectoRunData(), null, null);

			var cycleData = DrivingCycleDataReader.ReadFromFile(@"TestData/Cycles/Coach Engine Only Paux_var-dt.vdri",
				CycleType.EngineOnly, false);
			var cycle = new PowertrainDrivingCycle(container, cycleData);
			new EngineOnlyGearboxInfo(container);
			new ZeroMileageCounter(container);
			new DummyDriverInfo(container);
			var outPort = new MockTnOutPort();
			var inPort = cycle.InPort();

			inPort.Connect(outPort);

			var absTime = 0.SI<Second>();
			var dt = 5.SI<Second>();

			var response = cycle.OutPort().Request(absTime, dt);
			var timeFail = (response as ResponseFailTimeInterval);
			Assert.IsInstanceOf<ResponseFailTimeInterval>(response);
			// ReSharper disable once PossibleNullReferenceException
			Assert.AreEqual(0.25.SI<Second>(), timeFail.DeltaT);

			dt = timeFail.DeltaT;

			response = cycle.OutPort().Request(absTime, dt);
			Assert.IsInstanceOf<ResponseSuccess>(response);

			container.CommitSimulationStep(absTime, dt);

			Assert.AreEqual(absTime, outPort.AbsTime);
			Assert.AreEqual(dt, outPort.Dt);
			Assert.AreEqual(600.RPMtoRad(), outPort.AngularVelocity);
			Assert.AreEqual(0.SI<NewtonMeter>(), outPort.Torque);

			// ========================
			absTime += dt;
			dt = 1.SI<Second>();

			response = cycle.OutPort().Request(absTime, dt);
			Assert.IsInstanceOf<ResponseFailTimeInterval>(response);

			dt = ((ResponseFailTimeInterval)response).DeltaT;
			Assert.AreEqual(0.5.SI<Second>(), dt);

			for (var i = 0; i < 100; i++) {
				response = cycle.OutPort().Request(absTime, dt);
				switch (response) {
					case ResponseFailTimeInterval r: 
						dt = r.DeltaT; 
						break;
					case ResponseSuccess _:
						container.CommitSimulationStep(absTime, dt);
						Assert.AreEqual(absTime, outPort.AbsTime);
						Assert.AreEqual(dt, outPort.Dt);

						if (absTime < 5) {
							Assert.AreEqual(600.RPMtoRad(), outPort.AngularVelocity);
							AssertHelper.AreRelativeEqual(0.SI<NewtonMeter>(), outPort.Torque, toleranceFactor: 1e-3);
						} else if (absTime.IsBetween(12.75, 13.25) || absTime.IsBetween(14, 15)) {
							Assert.IsTrue(outPort.AngularVelocity > 600.RPMtoRad());
							Assert.IsTrue(outPort.Torque < 0);
						} else {
							Assert.IsTrue(outPort.AngularVelocity > 600.RPMtoRad());
							Assert.IsTrue(outPort.Torque > 0);
						}

						absTime += dt;
						dt = 1.SI<Second>();
						break;
					default:
						throw new UnexpectedResponseException("Got an unexpected response", response);
				}
			}
		}

		[
			// declaration mode - distance based
			TestCase("<s>,<v>,<grad>,<stop>", CycleType.DistanceBased),
			TestCase("<s>,<<v>,>grad>,<stop>", CycleType.DistanceBased),

			// engineering mode - distance based
			TestCase("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>", CycleType.DistanceBased),
			TestCase("<s>,<v>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>", CycleType.DistanceBased),
			TestCase("<s>,<v>,<grad>,<stop>,<Padd>,<Aux_ELE>,<Aux_SP>", CycleType.DistanceBased),
			TestCase("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>", CycleType.DistanceBased),
			TestCase("<s>,<v>,<stop>,<Padd>", CycleType.DistanceBased),
			TestCase("s,v,stop,Padd", CycleType.DistanceBased),
			TestCase("s,v,stop", CycleType.DistanceBased),

			// engineering mode - time based
			// mk 2016-03-01: plain time based cycle does not exist anymore. replaced by measuredspeed, measuredspeed gear, engineonly and pwheel

			// engine only
			TestCase("<t>,<n>,<Me>,<Padd>", CycleType.EngineOnly),
			TestCase("<t>,<n>,<Me>", CycleType.EngineOnly),
			TestCase("<t>,<n>,<Me>,<Pe>,<Padd>", CycleType.EngineOnly),
			TestCase("<t>,<n>,<Pe>,<Padd>", CycleType.EngineOnly),
			TestCase("<t>,<n>,<Pe>", CycleType.EngineOnly),
			TestCase("<Me>,<n>,<Padd>,<t>", CycleType.EngineOnly),
			TestCase("t,n,Me,Padd", CycleType.EngineOnly),

			// p_wheel
			TestCase("<t>,<Pwheel>,<gear>,<n>,<Padd>", CycleType.PWheel),
			TestCase("<gear>,<t>,<n>,<Padd>,<Pwheel>", CycleType.PWheel),
			TestCase("<t>,<Pwheel>,<gear>,<n>", CycleType.PWheel),
			TestCase("t,Pwheel,gear,n,Padd", CycleType.PWheel),
			TestCase("Pwheel,t,gear,n,Padd", CycleType.PWheel),

			// measured speed
			TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>", CycleType.MeasuredSpeed),
			TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>", CycleType.MeasuredSpeed),
			TestCase("<t>,<v>,<grad>,<Padd>", CycleType.MeasuredSpeed),
			TestCase("<t>,<v>,<grad>,<Padd>,<Aux_ALT>,<Aux_ES>", CycleType.MeasuredSpeed),
			TestCase("<t>,<v>,<grad>", CycleType.MeasuredSpeed),
			TestCase("<t>,<Padd>,<grad>,<v>", CycleType.MeasuredSpeed),
			TestCase("t,v,grad,Padd", CycleType.MeasuredSpeed),
			TestCase("t,v,grad", CycleType.MeasuredSpeed),

			// measured speed with gear
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>",
				CycleType.MeasuredSpeedGear),
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>", CycleType.MeasuredSpeedGear),
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<Aux_HVAC>,<Aux_HP>", CycleType.MeasuredSpeedGear),
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>", CycleType.MeasuredSpeedGear),
			TestCase("<t>,<v>,<grad>,<n>,<gear>", CycleType.MeasuredSpeedGear),
			TestCase("<n>,<Padd>,<gear>,<v>,<grad>,<t>", CycleType.MeasuredSpeedGear),
			TestCase("t,v,grad,Padd,n,gear", CycleType.MeasuredSpeedGear),

			// Verification test simulation
			TestCase("<t>,<v>,<n_eng>,<n_fan>,<tq_wh_left>,<tq_wh_right>,<n_wh_left>,<n_wh_right>,<fc_DIESEL CI>,<tq_eng>,<CO>,<NOx>,<THC>,<PN>,<CO2>", CycleType.VTP),
			TestCase("<t>,<v>,<n_eng>,<n_fan>,<tq_wh_left>,<tq_wh_right>,<n_wh_left>,<n_wh_right>,<fc_DIESEL CI>,<gear>,<tq_eng>,<CO>,<NOx>,<THC>,<PN>,<CO2>", CycleType.VTP),
			TestCase("<t>,<v>,<n_eng>,<Pel_fan>,<tq_wh_left>,<tq_wh_right>,<n_wh_left>,<n_wh_right>,<fc_DIESEL CI>,<gear>,<tq_eng>,<CH4>,<CO>,<NMHC>,<NOx>,<PN>,<CO2>", CycleType.VTP),
			TestCase("<t>,<v>,<n_eng>,<n_fan>,<tq_wh_left>,<tq_wh_right>,<n_wh_left>,<n_wh_right>,<fc_NG CI>,<tq_eng>,<CH4>,<CO>,<NMHC>,<NOx>,<PN>,<CO2>", CycleType.VTP),
			TestCase("<t>,<v>,<n_eng>,<n_fan>,<tq_wh_left>,<tq_wh_right>,<n_wh_left>,<n_wh_right>,<fc_NG CI>,<fc_DIESEL CI>,<tq_eng>,<CH4>,<CO>,<NMHC>,<NOx>,<THC>,<PN>,<CO2>", CycleType.VTP),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void DrivingCycle_AutoDetect(string cycle, CycleType type)
		{
			TestCycleDetect(cycle, type);
		}

		[
			// wrong cycles
			TestCase("v,grad,Padd,n,gear", CycleType.MeasuredSpeedGear),
			TestCase("<t>,<grad>", CycleType.MeasuredSpeed),
			//TestCase("<t>,<Pwheel>,<n>,<Padd>", CycleType.PWheel),
			//TestCase("<t>,<Pwheel>,<Pwheel>,<n>,<Padd>", CycleType.PWheel),
			TestCase("<t>,<n>,<torque>,<>,<Padd>", CycleType.EngineOnly),
			TestCase("x,y,z", CycleType.EngineOnly),
			TestCase("x", CycleType.EngineOnly),
			TestCase("", CycleType.MeasuredSpeed),
			TestCase("<t>,<v>,<gear>,<Pwheel>,<s>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>",
				CycleType.MeasuredSpeedGear),
			TestCase("<t>,<v>,<n_eng>,<n_fan>,<tq_left>,<tq_right>,<n_wh_left>,<n_wh_right>,<fc_DIESEL CI>,<CO>,<NOx>,<THC>,<PN>", 
				CycleType.VTP),
			TestCase("<t>,<v>,<n_eng>,<n_fan>,<tq_left>,<tq_right>,<n_wh_left>,<n_wh_right>,<fc_NG CI>,<tq_eng>,<CH4>,<NMHC>,<CO>,<NOx>,<THC>", 
				CycleType.VTP),
			TestCase("<t>,<v>,<n_eng>,<n_fan>,<tq_left>,<tq_right>,<n_wh_left>,<n_wh_right>,<fc_NG CI>,<tq_eng>,<CO>,<NOx>,<PN>", 
				CycleType.VTP),
			Category(Definitions.TESTCASE_MIGRATED)
        ]
		public void DrivingCycle_AutoDetect_Exception(string cycle, CycleType type)
		{
			AssertHelper.Exception<VectoException>(() => TestCycleDetect(cycle, type));
		}

		[
			// declaration mode - distance based
			TestCase("<s>,<v>,<grad>,<stop>\n1,1,1,0", CycleType.DistanceBased, 2),
			TestCase("<s>,<v>,<grad>,<stop>\n1,0,1,1", CycleType.DistanceBased, 3),
			TestCase("<s>,<<v>,>grad>,<stop>\n1,1,1,0", CycleType.DistanceBased, 2),

			// engineering mode - distance based
			TestCase("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>\n1,1,1,0,1,1,1,1,1",
				CycleType.DistanceBased, 2),
			TestCase("<s>,<v>,<stop>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>\n1,1,0,1,1,1,1,1",
				CycleType.DistanceBased, 2),
			TestCase("<s>,<v>,<grad>,<stop>,<Padd>,<Aux_ELE>,<Aux_SP>\n1,1,1,0,1,1,1", CycleType.DistanceBased, 2),
			TestCase("<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>\n1,1,1,0,1,1,1", CycleType.DistanceBased, 2),
			TestCase("<s>,<v>,<stop>,<Padd>\n1,1,0,1", CycleType.DistanceBased, 2),
			TestCase("s,v,stop,Padd\n1,1,0,1", CycleType.DistanceBased, 2),
			TestCase("s,v,stop\n1,1,0", CycleType.DistanceBased, 2),

			// engineering mode - time based
			// mk 2016-03-01: plain time based cycle does not exist anymore. replaced by measuredspeed, measuredspeed gear, engineonly and pwheel

			// engine only
			TestCase("<t>,<n>,<Me>,<Padd>\n1,1,1,1", CycleType.EngineOnly, 1),
			TestCase("<t>,<n>,<Me>\n1,1,1", CycleType.EngineOnly, 1),
			TestCase("<t>,<n>,<Me>,<Pe>,<Padd>\n1,1,1,1,1", CycleType.EngineOnly, 1),
			TestCase("<t>,<n>,<Pe>,<Padd>\n1,1,1,1", CycleType.EngineOnly, 1),
			TestCase("<t>,<n>,<Pe>\n1,1,1", CycleType.EngineOnly, 1),
			TestCase("<Me>,<n>,<Padd>,<t>\n1,1,1,1", CycleType.EngineOnly, 1),
			TestCase("t,n,Me,Padd\n1,1,1,1", CycleType.EngineOnly, 1),

			// p_wheel
			TestCase("<t>,<Pwheel>,<gear>,<n>,<Padd>\n1,1,1,1,1", CycleType.PWheel, 1),
			TestCase("<gear>,<t>,<n>,<Padd>,<Pwheel>\n1,1,1,1,1", CycleType.PWheel, 1),
			TestCase("<t>,<Pwheel>,<gear>,<n>\n1,1,1,1", CycleType.PWheel, 1),
			TestCase("t,Pwheel,gear,n,Padd\n1,1,1,1,1", CycleType.PWheel, 1),
			TestCase("Pwheel,t,gear,n,Padd\n1,1,1,1,1", CycleType.PWheel, 1),

			// measured speed
			TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_ELE>,<Aux_SP>\n1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeed, 1),
			TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_ALT>,<Aux_ES>\n1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeed, 1),
			TestCase("<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>\n1,1,1,1,1,1", CycleType.MeasuredSpeed, 1),
			TestCase("<t>,<v>,<grad>,<Padd>\n1,1,1,1", CycleType.MeasuredSpeed, 1),
			TestCase("<t>,<v>,<grad>,<Padd>,<Aux_ALT>,<Aux_ES>\n1,1,1,1,1,1", CycleType.MeasuredSpeed, 1),
			TestCase("<t>,<v>,<grad>\n1,1,1", CycleType.MeasuredSpeed, 1),
			TestCase("<t>,<Padd>,<grad>,<v>\n1,1,1,1", CycleType.MeasuredSpeed, 1),
			TestCase("t,v,grad,Padd\n1,1,1,1", CycleType.MeasuredSpeed, 1),
			TestCase("t,v,grad\n1,1,1", CycleType.MeasuredSpeed, 1),

			// measured speed with gear
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>\n1,1,1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeedGear, 1),
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>\n1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeedGear, 1),
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>,<Aux_HVAC>,<Aux_HP>\n1,1,1,1,1,1,1,1", CycleType.MeasuredSpeedGear, 1),
			TestCase("<t>,<v>,<grad>,<Padd>,<n>,<gear>\n1,1,1,1,1,1", CycleType.MeasuredSpeedGear, 1),
			TestCase("<t>,<v>,<grad>,<n>,<gear>\n1,1,1,1,1", CycleType.MeasuredSpeedGear, 1),
			TestCase("<n>,<Padd>,<gear>,<v>,<grad>,<t>\n1,1,1,1,1,1", CycleType.MeasuredSpeedGear, 1),
			TestCase("t,v,grad,Padd,n,gear\n1,1,1,1,1,1", CycleType.MeasuredSpeedGear, 1),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void DrivingCycle_Read(string cycle, CycleType type, int entryCount)
		{
			TestCycleRead(cycle, type, entryCount);
		}


		[
			// wrong cycles
			TestCase("<s>,<v>,<grad>,<stop>\n1,1,1,1", CycleType.DistanceBased),
			TestCase("v,grad,Padd,n,gear\n1,1,1,1,1", CycleType.MeasuredSpeedGear),
			TestCase("<t>,<grad>\n1,1,1,1,1,1,1,1,1", CycleType.MeasuredSpeed),
			TestCase("<t>,<Pwheel>,<n>,<Padd>\n1,1,1,1,1,1,1,1,1", CycleType.PWheel),
			TestCase("<t>,<Pwheel>,<Pwheel>,<n>,<Padd>\n1,1,1,1,1,1,1,1,1", CycleType.PWheel),
			TestCase("<t>,<n>,<torque>,<>,<Padd>\n1,1,1,1,1,1,1,1,1", CycleType.EngineOnly),
			TestCase("x,y,z\n1,1,1,1,1,1,1,1,1", CycleType.EngineOnly),
			TestCase("x\n1,1,1,1,1,1,1,1,1", CycleType.EngineOnly),
			TestCase("\n1,1,1,1,1,1,1,1,1", CycleType.MeasuredSpeed),
			TestCase(
				"<t>,<v>,<gear>,<Pwheel>,<s>,<grad>,<Padd>,<n>,<gear>,<vair_res>,<vair_beta>,<Aux_HVAC>,<Aux_HP>\n1,1,1,1,1,1,1,1,1",
				CycleType.MeasuredSpeedGear),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void DrivingCycle_Read_Exception(string cycle, CycleType type)
		{
			AssertHelper.Exception<VectoException>(() => TestCycleRead(cycle, type));
		}

		[TestCase(@"TestData/Cycles/Coach.vdri", CycleType.DistanceBased),
		TestCase(@"TestData/Cycles/Engine Only1.vdri", CycleType.EngineOnly),
		TestCase(@"TestData/Pwheel/RD_#1_Pwheel_AuxStd.vdri", CycleType.PWheel),
		TestCase(@"TestData/MeasuredSpeed/MeasuredSpeedVairAux.vdri", CycleType.MeasuredSpeed),
		TestCase(@"TestData/MeasuredSpeed/MeasuredSpeed_Gear_Rural_VairAux.vdri",
			CycleType.MeasuredSpeedGear),
		]
		public void DrivingCycle_Detect_File(string filename, CycleType type)
		{
			TestCycleDetect(File.ReadAllText(filename), type);
		}

		[TestCase(@"TestData/Cycles/Coach.vdri", CycleType.DistanceBased, 6036),
		TestCase(@"TestData/Cycles/Engine Only1.vdri", CycleType.EngineOnly, 696),
		TestCase(@"TestData/Pwheel/RD_#1_Pwheel_AuxStd.vdri", CycleType.PWheel, 3917),
		TestCase(@"TestData/MeasuredSpeed/MeasuredSpeedVairAux.vdri", CycleType.MeasuredSpeed, 1300),
		TestCase(@"TestData/MeasuredSpeed/MeasuredSpeed_Gear_Rural_VairAux.vdri",
			CycleType.MeasuredSpeedGear, 1300),
		]
		public void DrivingCycle_Read_File(string filename, CycleType type, int entryCount)
		{
			TestCycleRead(File.ReadAllText(filename), type, entryCount);
		}

		[
			TestCase("t, Engine Speed, PTO Torque\n1,2,3", CycleType.PTO),
			TestCase("t, engine speed, PTO Torque\n1,2,3", CycleType.PTO),
			TestCase("t, Engine Speed, pto torque\n1,2,3", CycleType.PTO),
			TestCase("t, engine speed, pto torque\n1,2,3", CycleType.PTO),
			TestCase("T, ENGINE SPEED, PTO TORQUE\n1,2,3", CycleType.PTO),
			TestCase("<Me>,<n>,<Padd>,<t>\n1,1,1,1", CycleType.EngineOnly),
			TestCase("t,n,Me,Padd\n1,1,1,1", CycleType.EngineOnly),
			TestCase("<s>,<v>,<Grad>,<STOP>\n1,0,1,1", CycleType.DistanceBased),
			TestCase("<s>,<V>,<grad>,<stop>,<PADD>,<vAir_res>,<vAir_Beta>,<Aux_ELE>,<Aux_SP>\n1,1,1,0,1,1,1,1,1",
				CycleType.DistanceBased),
			TestCase("<S>,<v>,<stop>,<pAdd>,<Vair_res>,<vair_BETA>,<Aux_ELE>,<Aux_SP>\n1,1,0,1,1,1,1,1",
				CycleType.DistanceBased),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void DrivingCycleDetect_CaseInsensitive(string cycle, CycleType type)
		{
			TestCycleDetect(cycle, type);
		}

		[
			TestCase("t, Engine Speed, PTO Torque\n1,2,3", CycleType.PTO, 1),
			TestCase("t, engine speed, PTO Torque\n1,2,3", CycleType.PTO, 1),
			TestCase("t, Engine Speed, pto torque\n1,2,3", CycleType.PTO, 1),
			TestCase("t, engine speed, pto torque\n1,2,3", CycleType.PTO, 1),
			TestCase("T, ENGINE SPEED, PTO TORQUE\n1,2,3", CycleType.PTO, 1),
			TestCase("<Me>,<n>,<Padd>,<t>\n1,1,1,1", CycleType.EngineOnly, 1),
			TestCase("t,n,Me,Padd\n1,1,1,1", CycleType.EngineOnly, 1),
			TestCase("<s>,<v>,<Grad>,<STOP>\n1,0,1,1", CycleType.DistanceBased, 3),
			TestCase("<s>,<V>,<grad>,<stop>,<PADD>,<vAir_res>,<vAir_Beta>,<Aux_ELE>,<Aux_SP>\n1,1,1,0,1,1,1,1,1",
				CycleType.DistanceBased, 2),
			TestCase("<S>,<v>,<stop>,<pAdd>,<Vair_res>,<vair_BETA>,<Aux_ELE>,<Aux_SP>\n1,1,0,1,1,1,1,1",
				CycleType.DistanceBased, 2),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void DrivingCycleRead_CaseInsensitive(string cycle, CycleType type, int entryCount)
		{
			TestCycleRead(cycle, type, entryCount);
		}


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void DrivingCycleRead_CompressEntries_TargetSpeedOnly()
		{
			var cycle = "<s>,<v>,<Grad>,<STOP>\n" +
						" 1, 0,0,1\n" +
						" 2,50,0,0\n" +
						" 5,50,0,0\n" +
						"50,50,0,0\n" +
						"99,50,0,0";
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycle.ToStream(), CycleType.DistanceBased, "", false);
			Assert.AreEqual(3, drivingCycle.Entries.Count);
			Assert.AreEqual(1, drivingCycle.Entries[0].Distance.Value());
			Assert.AreEqual(1, drivingCycle.Entries[1].Distance.Value());
			Assert.AreEqual(99, drivingCycle.Entries[2].Distance.Value());
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void DrivingCycleRead_CompressEntries_TargetSpeedVAirBeta1()
		{
			var cycle = "<s>,<v>,<Grad>,<STOP>,vair_res,vair_beta\n" +
						" 1, 0,0,1,30,10\n" +
						" 2,50,0,0,30,10\n" +
						" 5,50,0,0,30,15\n" +
						"50,50,0,0,30,10\n" +
						"99,50,0,0,30,15";
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycle.ToStream(), CycleType.DistanceBased, "", true);
			Assert.AreEqual(5, drivingCycle.Entries.Count);

			Assert.AreEqual(1, drivingCycle.Entries[0].Distance.Value());
			Assert.AreEqual(1, drivingCycle.Entries[1].Distance.Value());
			Assert.AreEqual(5, drivingCycle.Entries[2].Distance.Value());
			Assert.AreEqual(50, drivingCycle.Entries[3].Distance.Value());
			Assert.AreEqual(99, drivingCycle.Entries[4].Distance.Value());
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void DrivingCycleRead_CompressEntries_TargetSpeedVAirBeta2()
		{
			var cycle = "<s>,<v>,<Grad>,<STOP>,vair_res,vair_beta\n" +
						" 1, 0,0,1,30,10\n" +
						" 2,50,0,0,30,10\n" +
						" 5,50,0,0,35,10\n" +
						"50,50,0,0,30,10\n" +
						"99,50,0,0,33,10";
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycle.ToStream(), CycleType.DistanceBased, "", true);
			Assert.AreEqual(5, drivingCycle.Entries.Count);

			Assert.AreEqual(1, drivingCycle.Entries[0].Distance.Value());
			Assert.AreEqual(1, drivingCycle.Entries[1].Distance.Value());
			Assert.AreEqual(5, drivingCycle.Entries[2].Distance.Value());
			Assert.AreEqual(50, drivingCycle.Entries[3].Distance.Value());
			Assert.AreEqual(99, drivingCycle.Entries[4].Distance.Value());
		}


		private static void TestCycleDetect(string inputData, CycleType cycleType)
		{
			var cycleTypeCalc = DrivingCycleDataReader.DetectCycleType(VectoCSVFile.ReadStream(inputData.ToStream()));
			Assert.AreEqual(cycleType, cycleTypeCalc);
		}

		private static void TestCycleRead(string inputData, CycleType cycleType, int entryCount = 1)
		{
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.ToStream(), cycleType, "", false);
			Assert.AreEqual(cycleType, drivingCycle.CycleType);
			Assert.AreEqual(entryCount, drivingCycle.Entries.Count, "Driving Cycle Entry count.");
		}
	}
}

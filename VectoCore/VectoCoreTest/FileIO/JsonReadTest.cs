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
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using System.Linq;


namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class JsonReadTest
	{
		private const string TestJobFile = @"TestData/Jobs/40t_Long_Haul_Truck.vecto";
		private const string TestVehicleFile = @"TestData/Components/24t Coach.vveh";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void ReadJobTest()
		{
			var job = JSONInputDataFactory.ReadJsonJob(TestJobFile);

			Assert.IsNotNull(job);
			//			AssertHelper.Exception<InvalidFileFormatException>(() => );
		}

		[TestCase]
		public void NoEngineFileTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("EngineFile").Remove();

			AssertHelper.Exception<VectoException>(() => new JSONInputDataV2(json, TestJobFile),
				"JobFile: Failed to read Engine file '': Key EngineFile not found");
		}

		[TestCase]
		public void NoGearboxFileTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("GearboxFile").Remove();

			AssertHelper.Exception<VectoException>(() => new JSONInputDataV2(json, TestJobFile),
				"JobFile: Failed to read Gearbox file '': Key GearboxFile not found");
		}

		[TestCase]
		public void NoVehicleFileTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("VehicleFile").Remove();

			AssertHelper.Exception<VectoException>(() => new JSONInputDataV2(json, TestJobFile),
				"JobFile: Failed to read Vehicle file '': Key VehicleFile not found");
		}

		[TestCase]
		public void NoCyclesTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("Cycles").Remove();

			var tmp = new JSONInputDataV2(json, TestJobFile).Cycles;
			Assert.AreEqual(0, tmp.Count);
		}

		[TestCase]
		public void NoAuxTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("Aux").Remove();

			// MK,2016-01-20: Changed for PWheel: aux entry may be missing, and that is ok.
			// MQ,2021-03-02: refactoring aux: remove mapping and only have 3 fixed values
			var tmp = new JSONInputDataV2(json, TestJobFile).JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries;
			Assert.AreEqual(0, tmp.ConstantPowerDemand.Value());
			Assert.AreEqual(0, tmp.PowerDemandICEOffDriving.Value());
			Assert.AreEqual(0, tmp.PowerDemandICEOffStandstill.Value());
		}

		[TestCase]
		public void NoDriverAccCurveTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("VACC").Remove();

			IEngineeringInputDataProvider input = new JSONInputDataV2(json, TestJobFile);
			var tmp = input.DriverInputData.AccelerationCurve;
			Assert.IsNull(tmp);
		}

		[TestCase]
		public void UseDeclarationDriverAccCurveTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			json["Body"]["VACC"] = "Truck";

			IEngineeringInputDataProvider input = new JSONInputDataV2(json, TestJobFile);
			var tmp = input.DriverInputData.AccelerationCurve;
			Assert.IsNotNull(tmp);
		}

		[TestCase]
		public void NoLookaheadCoastingTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("LAC").Remove();

			IEngineeringInputDataProvider input = new JSONInputDataV2(json, TestJobFile);
			var tmp = input.DriverInputData.Lookahead;
			Assert.IsNull(tmp);
		}

		[TestCase]
		public void NoOverspeedEcoRollTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("OverSpeedEcoRoll").Remove();

			//AssertHelper.Exception<VectoException>(
			//	() => {
			//		var tmp = ((IEngineeringInputDataProvider)new JSONInputDataV2(json, TestJobFile)).DriverInputData.OverSpeedData;
			//	},
			//	"Key OverSpeedEcoRoll not found");

			var tmp = ((IEngineeringInputDataProvider)new JSONInputDataV2(json, TestJobFile)).DriverInputData.OverSpeedData;
			Assert.AreEqual(true, tmp.Enabled);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.MinSpeed.AsKmph, tmp.MinSpeed.AsKmph);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.AllowedOverSpeed.AsKmph, tmp.OverSpeed.AsKmph);
		}

		[TestCase]
		public void ReadGearboxV5()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/Gearbox_v5.vgbx");

			var ratios = new[] { 3.0, 1.0, 0.8 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}

			var gbxData = new EngineeringDataAdapter().CreateGearboxData(
				new MockEngineeringInputProvider() {
					DriverInputData = new MockDriverTestInputData() {
						GearshiftInputData = (IGearshiftEngineeringInputData)inputProvider
					},
					JobInputData = new MockJobTestInputData() {
						Vehicle = new MockEngineeringVehicleInputData() {
							GearboxInputData = inputProvider,
							TorqueConverterInputData = (ITorqueConverterEngineeringInputData)inputProvider,
						}
					}
				}, new VectoRunData() {
					EngineData = MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
					VehicleData = new VehicleData() {
						VehicleCategory = VehicleCategory.RigidTruck,
						DynamicTyreRadius = 0.5.SI<Meter>()
					},
					AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1} }
				});
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			// interpreted as gearbox with first and second gear using TC (due to gear ratios)
			Assert.IsFalse(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ReadGearboxSerialTC()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerial.vgbx");

			var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}

			var gbxData = new EngineeringDataAdapter().CreateGearboxData(
				new MockEngineeringInputProvider() {
					DriverInputData = new MockDriverTestInputData() {
						GearshiftInputData = (IGearshiftEngineeringInputData)inputProvider
					},
					JobInputData = new MockJobTestInputData() {
						Vehicle = new MockEngineeringVehicleInputData() {
							GearboxInputData = inputProvider,
							TorqueConverterInputData = (ITorqueConverterEngineeringInputData)inputProvider,
						}
					}
				}, new VectoRunData() {
					EngineData = MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
					VehicleData = new VehicleData() {
						VehicleCategory = VehicleCategory.RigidTruck,
						DynamicTyreRadius = 0.5.SI<Meter>()
					},
					AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1 } }
				});

				//inputProvider,
				//MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
				//(IGearshiftEngineeringInputData)inputProvider, 2.1,
				//0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			Assert.IsTrue(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);

			var gear = gbxData.Gears[1];
			Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ReadGearboxPowersplitTC()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxPowerSplit.vgbx");

			var ratios = new[] { 1.35, 1.0, 0.73 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}

			var gbxData = new EngineeringDataAdapter().CreateGearboxData(
				new MockEngineeringInputProvider() {
					DriverInputData = new MockDriverTestInputData() {
						GearshiftInputData = (IGearshiftEngineeringInputData)inputProvider
					},
					JobInputData = new MockJobTestInputData() {
						Vehicle = new MockEngineeringVehicleInputData() {
							GearboxInputData = inputProvider,
							TorqueConverterInputData = (ITorqueConverterEngineeringInputData)inputProvider,
						}
					}
				}, new VectoRunData() {
					EngineData = MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
					VehicleData = new VehicleData() {
						VehicleCategory = VehicleCategory.RigidTruck,
						DynamicTyreRadius = 0.5.SI<Meter>()
					},
					AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1 } }
				});

				//inputProvider,
				//MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
				//(IGearshiftEngineeringInputData)inputProvider, 2.1,
				//0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			Assert.IsTrue(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);

			Assert.AreEqual(1, gbxData.Gears[1].TorqueConverterRatio);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ReadGearboxDualTCTruck()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerialDualTC.vgbx");

			var ratios = new[] { 4.35, 2.4, 1.8, 1.3, 1.0 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}

			var gbxData = new EngineeringDataAdapter().CreateGearboxData(
				new MockEngineeringInputProvider() {
					DriverInputData = new MockDriverTestInputData() {
						GearshiftInputData = (IGearshiftEngineeringInputData)inputProvider
					},
					JobInputData = new MockJobTestInputData() {
						Vehicle = new MockEngineeringVehicleInputData() {
							GearboxInputData = inputProvider,
							TorqueConverterInputData = (ITorqueConverterEngineeringInputData)inputProvider,
						}
					}
				}, new VectoRunData() {
					EngineData = MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
					VehicleData = new VehicleData() {
						VehicleCategory = VehicleCategory.RigidTruck,
						DynamicTyreRadius = 0.5.SI<Meter>()
					},
					AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1 } }
				});
				//inputProvider,
				//MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
				//(IGearshiftEngineeringInputData)inputProvider, 2.1,
				//0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			Assert.IsFalse(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);


			var gear = gbxData.Gears[2];
			Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ReadGearboxSingleTCBus()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerialDualTC.vgbx");

			var ratios = new[] { 4.35, 2.4, 1.8, 1.3, 1.0 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}

			var gbxData = new EngineeringDataAdapter().CreateGearboxData(
				new MockEngineeringInputProvider() {
					DriverInputData = new MockDriverTestInputData() {
						GearshiftInputData = (IGearshiftEngineeringInputData)inputProvider
					},
					JobInputData = new MockJobTestInputData() {
						Vehicle = new MockEngineeringVehicleInputData() {
							GearboxInputData = inputProvider,
							TorqueConverterInputData = (ITorqueConverterEngineeringInputData)inputProvider,
						}
					}
				}, new VectoRunData() {
					EngineData = MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
					VehicleData = new VehicleData() {
						VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
						DynamicTyreRadius = 0.5.SI<Meter>()
					},
					AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1 } }
				});
				//inputProvider,
				//MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
				//(IGearshiftEngineeringInputData)inputProvider, 2.1,
				//0.5.SI<Meter>(), VehicleCategory.InterurbanBus, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			Assert.IsTrue(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);


			var gear = gbxData.Gears[1];
			Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ReadGearboxDualTCBus()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData/Components/AT_GBX/GearboxSerialDualTCBus.vgbx");

			var ratios = new[] { 4.58, 2.4, 1.8, 1.3, 1.0 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}

			var gbxData = new EngineeringDataAdapter().CreateGearboxData(
				new MockEngineeringInputProvider() {
					DriverInputData = new MockDriverTestInputData() {
						GearshiftInputData = (IGearshiftEngineeringInputData)inputProvider
					},
					JobInputData = new MockJobTestInputData() {
						Vehicle = new MockEngineeringVehicleInputData() {
							GearboxInputData = inputProvider,
							TorqueConverterInputData = (ITorqueConverterEngineeringInputData)inputProvider,
						}
					}
				}, new VectoRunData() {
					EngineData = MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
					VehicleData = new VehicleData() {
						VehicleCategory = VehicleCategory.RigidTruck,
						DynamicTyreRadius = 0.5.SI<Meter>()
					},
					AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1 } }
				});
				
				//inputProvider,
				//MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData/Components/AT_GBX/Engine.veng", 0),
				//(IGearshiftEngineeringInputData)inputProvider, 2.1,
				//0.5.SI<Meter>(), VehicleCategory.InterurbanBus, (ITorqueConverterEngineeringInputData)inputProvider, null, null);
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			Assert.IsFalse(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);


			var gear = gbxData.Gears[2];
			Assert.AreEqual(gear.Ratio, gear.TorqueConverterRatio);
		}



		//[TestCase]
		//public void TestReadingElectricTechlist()
		//{
		//	var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
		//	((JArray)json["Body"]["Aux"][3]["TechList"]).Add("LED lights");

		//	var job = new JSONInputDataV2(json, TestJobFile);
		//	foreach (var aux in job.Auxiliaries) {
		//		if (aux.ID == "ES") {
		//			Assert.AreEqual(1, aux.TechList.Count);
		//			Assert.AreEqual("LED lights", aux.TechList.First());
		//		}
		//	}
		//}

		[TestCase]
		public void JSON_Read_AngleGear()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestVehicleFile)));
			var angleGear = json["Body"]["Angledrive"];

			Assert.AreEqual(AngledriveType.SeparateAngledrive,
				angleGear["Type"].Value<string>().ParseEnum<AngledriveType>());
			Assert.AreEqual(3.5, angleGear["Ratio"].Value<double>());
			Assert.AreEqual("AngleGear.vtlm", angleGear["LossMap"].Value<string>());
		}

		[
		TestCase(@"Group5_Tractor_4x2/Class5_Tractor_DECL.vecto", 3.2, double.NaN, -3.2, TestName="JSON_job_WheelBearings_Conventional"),
		TestCase(@"GenericVehicleE2/BEV_E2.vecto", 3.2, double.NaN, -3.2, TestName="JSON_job_WheelBearings_HEV_BEV"),
		TestCase(@"GenericIEPC/IEPC_Gbx1Speed/IEPC__Gbx1.vecto", 3.2, double.NaN, -3.2, TestName="JSON_job_WheelBearings_IEPC"),
		]
		public void ReadJobWithWheelBearings(string jobfile, double friction0, double friction1, double delta)
		{
			var testDir = @"TestData/Generic Vehicles/Declaration Mode";
			var filename = Path.Combine(testDir, jobfile);

			var inputProvider = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(filename);

			Assert.NotNull(inputProvider);
		
			var axlesDec = inputProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration;

			Assert.AreEqual(axlesDec[0].WheelEndFriction?.Value() ?? double.NaN, friction0);
			Assert.AreEqual(axlesDec[1].WheelEndFriction?.Value() ?? double.NaN, friction1);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputProvider, null);
			var deltaFriction = runsFactory.RunDataFactory.NextRun().First().WheelEndData.DeltaFrictionTorque;

			Assert.AreEqual(delta, deltaFriction.Value(), 1E-03);
		}

		[
		TestCase(@"Group5_Tractor_4x2/Class5_Tractor_ENG.vecto", 3, double.NaN, 1.5, -10.2, TestName = "JSON_ENG_job_WheelBearings_Conventional"),
		]
		public void ReadEngineeringJobWithWheelBearings(string jobfile, double friction0, double friction1, double friction2, double delta)
		{
			var testDir = @"TestData/Generic Vehicles/Engineering Mode";
			var filename = Path.Combine(testDir, jobfile);

			var inputProvider = (IEngineeringInputDataProvider)JSONInputDataFactory.ReadJsonJob(filename);

			Assert.NotNull(inputProvider);

			var axlesDec = inputProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesEngineering;

			Assert.AreEqual(friction0, axlesDec[0].WheelEndFriction?.Value() ?? double.NaN);
			Assert.AreEqual(friction1, axlesDec[1].WheelEndFriction?.Value() ?? double.NaN);
			Assert.AreEqual(friction2, axlesDec[2].WheelEndFriction?.Value() ?? double.NaN);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, null);
			var deltaFriction = runsFactory.RunDataFactory.NextRun().First().WheelEndData.DeltaFrictionTorque;

			Assert.AreEqual(delta, deltaFriction.Value(), 1E-03);
		}

		[
		TestCase(@"Group5_Tractor_4x2/Class5_Tractor_DECL_BAD.vecto", "VehicleDriven", TestName="JSON_job_WheelBearings_Bad"),
		TestCase(@"Group5_Tractor_4x2/Class5_Tractor_DECL_Negative.vecto", "negative", TestName="JSON_job_WheelBearings_Negative"),
		TestCase(@"Group5_Tractor_4x2/Class5_Tractor_DECL_TooBig.vecto", "greater than", TestName="JSON_job_WheelBearings_TooBig"),
		]
		public void TestBadWheelBearings(string jobfile, string keyword)
		{
			var testDir = @"TestData/Generic Vehicles/Declaration Mode";
			var filename = Path.Combine(testDir, jobfile);

			var dataProvider = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(filename);
					
			var exception = Assert.Throws<VectoException>(
				() => { 
					var axlesDec = dataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration;
					var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, null);
					runsFactory.RunDataFactory.NextRun().First();
				});
				
			TestContext.WriteLine(exception.Message);
		 	Assert.IsTrue(exception.Message.Contains(keyword));
		}

		[
			TestCase(@"MultiplePowertrains/MultipleBEV_E2_E3/MultipleBEV.vecto", TestName="JSON_job_Multiple_powertrains_E2_E3")
		]
		public void JSON_Read_MultiplePowertrains(string jobfile)
		{
			var testDir = @"TestData/Generic Vehicles/Engineering Mode";
			var filename = Path.Combine(testDir, jobfile);

			var inputProvider = (IEngineeringInputDataProvider)JSONInputDataFactory.ReadJsonJob(filename);

			var axlePts = inputProvider.JobInputData.Vehicle.Components.AxlePowertrainEngineeringInputData;

			Assert.NotNull(axlePts);
			Assert.IsTrue(axlePts.Count() > 0);

			Assert.IsTrue(axlePts[0].AxleNumber == 1);
			Assert.IsTrue(axlePts[0].Type == VectoSimulationJobType.BatteryElectricVehicle);
			Assert.NotNull(axlePts[0].GearboxInputData);
			Assert.NotNull(axlePts[0].AxleGearInputData);
			Assert.NotNull(axlePts[0].TorqueConverterInputData);
			Assert.NotNull(axlePts[0].GearshiftInputData);
			Assert.NotNull(axlePts[0].AngledriveInputData);
			Assert.NotNull(axlePts[0].RetarderInputData);
			Assert.NotNull(axlePts[0].PTOTransmissionInputData);
			Assert.NotNull(axlePts[0].ElectricMotor);
		}

		[TestCase]
		public void JSON_Read_HeavyBus()
		{
			var inputProvider = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(@"TestData/Generic Vehicles/Engineering Mode/HeavyBusPrimary/HeavyBusPrimary_DECL.vecto");
			var busAux = inputProvider.JobInputData.Vehicle.Components.BusAuxiliaries;

			Assert.AreEqual("Electrically driven - Electronically controlled", busAux.FanTechnology);

			Assert.AreEqual(1, busAux.SteeringPumpTechnology.Count);
			Assert.AreEqual("Dual displacement with mech. control", busAux.SteeringPumpTechnology[0]);


			Assert.AreEqual(1, busAux.ElectricSupply.Alternators.Count);
			//Assert.AreEqual("standard alternator", busAux.ElectricSupply.Alternators[0].Technology);
			Assert.AreEqual(AlternatorType.Conventional, busAux.ElectricSupply.AlternatorTechnology);

			Assert.AreEqual("", busAux.PneumaticSupply.CompressorSize);
			Assert.AreEqual(1.0, busAux.PneumaticSupply.Ratio);

			Assert.AreEqual(ConsumerTechnology.Mechanically, busAux.PneumaticConsumers.AirsuspensionControl);
			Assert.AreEqual(ConsumerTechnology.Pneumatically, busAux.PneumaticConsumers.AdBlueDosing);

			Assert.AreEqual(true, busAux.HVACAux.AdjustableCoolantThermostat);
			Assert.AreEqual(true, busAux.HVACAux.EngineWasteGasHeatExchanger);
		}

		[TestCase()]
		public void JSON_Read_IEPC()
		{
			var inputProvider = new JSONComponentInputData(@"TestData/BatteryElectric/IEPC/GenericIEPC.viepc", null);
			var iepcData = (IIEPCEngineeringInputData)inputProvider.IEPC;

			Assert.AreEqual("3", iepcData.AppVersion);
			Assert.AreEqual(DateTime.MinValue, iepcData.Date);

			Assert.AreEqual(false, iepcData.SavedInDeclarationMode);
			Assert.AreEqual("Generic IEPC", iepcData.Model);
			Assert.AreEqual(0.15.SI<KilogramSquareMeter>(), iepcData.Inertia);
			Assert.AreEqual(false, iepcData.DifferentialIncluded);
			Assert.AreEqual(false, iepcData.DesignTypeWheelMotor);
			Assert.AreEqual(1, iepcData.NrOfDesignTypeWheelMotorMeasured);
			Assert.AreEqual(0.9, iepcData.OverloadRecoveryFactor);

			TestIEPCGears(iepcData.Gears);
			TestIEPCVoltageLevel(iepcData.VoltageLevels);
			TestDragCurve(iepcData.DragCurves);
		}

		private void TestDragCurve(IList<IDragCurve> dragCurvesData)
		{
			Assert.AreEqual(3, dragCurvesData.Count);

			var dragCurve = dragCurvesData[0];
			Assert.AreEqual(1, dragCurve.Gear);
			Assert.IsNotNull(dragCurve.DragCurve);

			dragCurve = dragCurvesData[1];
			Assert.AreEqual(2, dragCurve.Gear);
			Assert.IsNotNull(dragCurve.DragCurve);

			dragCurve = dragCurvesData[2];
			Assert.AreEqual(3, dragCurve.Gear);
			Assert.IsNotNull(dragCurve.DragCurve);
		}

		private void TestIEPCGears(IList<IGearEntry> gears)
		{
			Assert.AreEqual(3, gears.Count);

			var gear = gears[0];
			Assert.AreEqual(1, gear.GearNumber);
			Assert.AreEqual(3.2, gear.Ratio);
			Assert.AreEqual(10000.SI<NewtonMeter>(), gear.MaxOutputShaftTorque);
			Assert.AreEqual(5000.RPMtoRad(), gear.MaxOutputShaftSpeed);

			gear = gears[1];
			Assert.AreEqual(2, gear.GearNumber);
			Assert.AreEqual(1.6, gear.Ratio);
			Assert.AreEqual(10000.SI<NewtonMeter>(), gear.MaxOutputShaftTorque);
			Assert.AreEqual(5000.RPMtoRad(), gear.MaxOutputShaftSpeed);

			gear = gears[2];
			Assert.AreEqual(3, gear.GearNumber);
			Assert.AreEqual(1.6, gear.Ratio);
			Assert.AreEqual(10000.SI<NewtonMeter>(), gear.MaxOutputShaftTorque);
			Assert.AreEqual(5000.RPMtoRad(), gear.MaxOutputShaftSpeed);
		}
		
		private void TestIEPCVoltageLevel(IList<IElectricMotorVoltageLevel> voltageLevels)
		{
			Assert.AreEqual(2, voltageLevels.Count);
			
			var voltageLevel = voltageLevels[0];
			Assert.AreEqual(400.SI<Volt>(), voltageLevel.VoltageLevel);
			Assert.AreEqual(200.SI<NewtonMeter>(), voltageLevel.ContinuousTorque);
			Assert.AreEqual(2000.RPMtoRad(), voltageLevel.ContinuousTorqueSpeed);
			Assert.AreEqual(500.SI<NewtonMeter>(), voltageLevel.OverloadTorque);
			Assert.AreEqual(2000.RPMtoRad(), voltageLevel.OverloadTestSpeed);
			Assert.AreEqual(60.SI<Second>(), voltageLevel.OverloadTime);
			Assert.IsNotNull(voltageLevel.FullLoadCurve);
			TestPowerMap(voltageLevel.PowerMap);

			voltageLevel = voltageLevels[1];
			Assert.AreEqual(800.SI<Volt>(), voltageLevel.VoltageLevel);
			Assert.AreEqual(200.SI<NewtonMeter>(), voltageLevel.ContinuousTorque);
			Assert.AreEqual(2000.RPMtoRad(), voltageLevel.ContinuousTorqueSpeed);
			Assert.AreEqual(500.SI<NewtonMeter>(), voltageLevel.OverloadTorque);
			Assert.AreEqual(2000.RPMtoRad(), voltageLevel.OverloadTestSpeed);
			Assert.AreEqual(60.SI<Second>(), voltageLevel.OverloadTime);
			Assert.IsNotNull(voltageLevel.FullLoadCurve);
			TestPowerMap(voltageLevel.PowerMap);
		}

		private void TestPowerMap(IList<IElectricMotorPowerMap> powerMapData)
		{
			Assert.AreEqual(3, powerMapData.Count);

			var powerMap = powerMapData[0];
			Assert.AreEqual(1, powerMap.Gear);
			Assert.IsNotNull(powerMap.PowerMap);

			powerMap = powerMapData[1];
			Assert.AreEqual(2, powerMap.Gear);
			Assert.IsNotNull(powerMap.PowerMap);

			powerMap = powerMapData[2];
			Assert.AreEqual(3, powerMap.Gear);
			Assert.IsNotNull(powerMap.PowerMap);
		}
	}

	public class MockDriverTestInputData : IDriverEngineeringInputData {
		#region Implementation of IDriverDeclarationInputData

		public bool SavedInDeclarationMode { get; }

		#endregion

		#region Implementation of IDriverEngineeringInputData

		public IOverSpeedEngineeringInputData OverSpeedData { get; set; }
		public IDriverAccelerationData AccelerationCurve { get; set; }
		public ILookaheadCoastingInputData Lookahead { get; set; }
		public IGearshiftEngineeringInputData GearshiftInputData { get; set; }
		public IEngineStopStartEngineeringInputData EngineStopStartData { get; set; }
		public IEcoRollEngineeringInputData EcoRollData { get; set; }
		public IPCCEngineeringInputData PCCData { get; set; }

		#endregion
	}

	public class MockJobTestInputData : IEngineeringJobInputData {
		#region Implementation of IDeclarationJobInputData

		public bool SavedInDeclarationMode { get; set; }
		public IVehicleEngineeringInputData Vehicle { get; set; }
		public IHybridStrategyParameters HybridStrategyParameters { get; }
		public IList<ICycleData> Cycles { get; set; }
		public VectoSimulationJobType JobType { get; set; }
		public IEngineEngineeringInputData EngineOnly { get; set; }

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle => Vehicle;

		public string JobName { get; set; }
		
		#endregion
	}

	public class MockEngineeringInputProvider : IEngineeringInputDataProvider {
		#region Implementation of IInputDataProvider

		public DataSource DataSource { get; }

		#endregion

		#region Implementation of IEngineeringInputDataProvider

		public IEngineeringJobInputData JobInputData { get; set; }
		public IDriverEngineeringInputData DriverInputData { get; set; }

		#endregion
	}

	//	[TestFixture]
	//	public class JsonTest
	//	{
	//		private const string jsonExpected = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""2015-11-17T11:49:03Z"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";

	//		private const string jsonExpected2 = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""2015-01-07T11:49:03Z"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";

	//		[TestCase]
	//		public void TestJsonHeaderEquality()
	//		{
	//			var h1 = new JsonDataHeader {
	//				AppVersion = "MyVecto3",
	//				CreatedBy = "UnitTest",
	//				Date = new DateTime(1970, 1, 1),
	//				FileVersion = 3
	//			};
	//			var h2 = new JsonDataHeader {
	//				AppVersion = "MyVecto3",
	//				CreatedBy = "UnitTest",
	//				Date = new DateTime(1970, 1, 1),
	//				FileVersion = 3
	//			};
	//			Assert.AreEqual(h1, h1);
	//			Assert.AreEqual(h1, h2);
	//			Assert.AreNotEqual(h1, null);
	//			Assert.AreNotEqual(h1, "hello world");
	//		}

	//		[TestCase]
	//		public void Test_Json_DateFormat_German()
	//		{
	//			var json = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""17.11.2015 11:49:03"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";
	//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

	//			Assert.AreEqual("3.0.1.320", header.AppVersion);
	//			Assert.AreEqual(7u, header.FileVersion);
	//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
	//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

	//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
	//			Assert.AreEqual(jsonExpected, jsonCompare);
	//		}

	//		[TestCase]
	//		public void Test_Json_DateFormat_German2()
	//		{
	//			var json = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""7.1.2015 11:49:03"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";
	//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

	//			Assert.AreEqual("3.0.1.320", header.AppVersion);
	//			Assert.AreEqual(7u, header.FileVersion);
	//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
	//			Assert.AreEqual(new DateTime(2015, 1, 7, 11, 49, 3, DateTimeKind.Utc), header.Date);

	//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
	//			Assert.AreEqual(jsonExpected2, jsonCompare);
	//		}

	//		[TestCase]
	//		public void Test_Json_DateFormat_English()
	//		{
	//			var json = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""11/17/2015 11:49:03 AM"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";
	//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

	//			Assert.AreEqual("3.0.1.320", header.AppVersion);
	//			Assert.AreEqual(7u, header.FileVersion);
	//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
	//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

	//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
	//			Assert.AreEqual(jsonExpected, jsonCompare);
	//		}

	//		[TestCase]
	//		public void Test_Json_DateFormat_English2()
	//		{
	//			var json = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""1/7/2015 11:49:03 AM"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";
	//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

	//			Assert.AreEqual("3.0.1.320", header.AppVersion);
	//			Assert.AreEqual(7u, header.FileVersion);
	//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
	//			Assert.AreEqual(new DateTime(2015, 1, 7, 11, 49, 3, DateTimeKind.Utc), header.Date);

	//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
	//			Assert.AreEqual(jsonExpected2, jsonCompare);
	//		}

	//		[TestCase]
	//		public void Test_Json_DateFormat_ISO8601()
	//		{
	//			var json = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""2015-11-17T11:49:03Z"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";
	//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

	//			Assert.AreEqual("3.0.1.320", header.AppVersion);
	//			Assert.AreEqual(7u, header.FileVersion);
	//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
	//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

	//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
	//			Assert.AreEqual(json, jsonCompare);
	//		}

	//		[TestCase]
	//		public void Test_Json_DateFormat_ISO8601_CET()
	//		{
	//			var json = @"{
	//  ""CreatedBy"": ""Michael Krisper"",
	//  ""Date"": ""2015-11-17T11:49:03+01:00"",
	//  ""AppVersion"": ""3.0.1.320"",
	//  ""FileVersion"": 7
	//}";
	//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

	//			Assert.AreEqual("3.0.1.320", header.AppVersion);
	//			Assert.AreEqual(7u, header.FileVersion);
	//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
	//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

	//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
	//			Assert.AreEqual(json, jsonCompare);
	//		}
	//	}
}

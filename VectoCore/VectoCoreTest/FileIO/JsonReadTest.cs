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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class JsonReadTest
	{
		private const string TestJobFile = @"Testdata\Jobs\40t_Long_Haul_Truck.vecto";
		private const string TestVehicleFile = @"Testdata\Components\24t Coach.vveh";

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
			var tmp = new JSONInputDataV2(json, TestJobFile).JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries;
			Assert.IsTrue(tmp.Count == 0);
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

			AssertHelper.Exception<VectoException>(
				() => {
					var tmp = ((IEngineeringInputDataProvider)new JSONInputDataV2(json, TestJobFile)).DriverInputData.OverSpeedData;
				},
				"Key OverSpeedEcoRoll not found");
		}

		[TestCase]
		public void ReadGearboxV5()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData\Components\AT_GBX\Gearbox_v5.vgbx");

			var ratios = new[] { 3.0, 1.0, 0.8 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}
			var gbxData = new EngineeringDataAdapter().CreateGearboxData(inputProvider,
				MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData\Components\AT_GBX\Engine.veng", 0),
				(IGearshiftEngineeringInputData)inputProvider, 2.1,
				0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider);
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			// interpreted as gearbox with first and second gear using TC (due to gear ratios)
			Assert.IsFalse(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);
		}

		[TestCase]
		public void ReadGearboxSerialTC()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData\Components\AT_GBX\GearboxSerial.vgbx");

			var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}
			var gbxData = new EngineeringDataAdapter().CreateGearboxData(inputProvider,
				MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData\Components\AT_GBX\Engine.veng", 0),
				(IGearshiftEngineeringInputData)inputProvider, 2.1,
				0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider);
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

		[TestCase]
		public void ReadGearboxPowersplitTC()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData\Components\AT_GBX\GearboxPowerSplit.vgbx");

			var ratios = new[] { 1.35, 1.0, 0.73 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}
			var gbxData = new EngineeringDataAdapter().CreateGearboxData(inputProvider,
				MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData\Components\AT_GBX\Engine.veng", 0),
				(IGearshiftEngineeringInputData)inputProvider, 2.1,
				0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider);
			Assert.AreEqual(ratios.Length, gbxData.Gears.Count);

			Assert.IsTrue(gbxData.Gears[1].HasLockedGear);
			Assert.IsTrue(gbxData.Gears[1].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[2].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[2].HasTorqueConverter);
			Assert.IsTrue(gbxData.Gears[3].HasLockedGear);
			Assert.IsFalse(gbxData.Gears[3].HasTorqueConverter);

			Assert.AreEqual(1, gbxData.Gears[1].TorqueConverterRatio);
		}

		[TestCase]
		public void ReadGearboxDualTCTruck()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData\Components\AT_GBX\GearboxSerialDualTC.vgbx");

			var ratios = new[] { 4.35, 2.4, 1.8, 1.3, 1.0 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}
			var gbxData = new EngineeringDataAdapter().CreateGearboxData(inputProvider,
				MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData\Components\AT_GBX\Engine.veng", 0),
				(IGearshiftEngineeringInputData)inputProvider, 2.1,
				0.5.SI<Meter>(), VehicleCategory.RigidTruck, (ITorqueConverterEngineeringInputData)inputProvider);
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

		[TestCase]
		public void ReadGearboxSingleTCBus()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData\Components\AT_GBX\GearboxSerialDualTC.vgbx");

			var ratios = new[] { 4.35, 2.4, 1.8, 1.3, 1.0 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}
			var gbxData = new EngineeringDataAdapter().CreateGearboxData(inputProvider,
				MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData\Components\AT_GBX\Engine.veng", 0),
				(IGearshiftEngineeringInputData)inputProvider, 2.1,
				0.5.SI<Meter>(), VehicleCategory.InterurbanBus, (ITorqueConverterEngineeringInputData)inputProvider);
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

		[TestCase]
		public void ReadGearboxDualTCBus()
		{
			var inputProvider = JSONInputDataFactory.ReadGearbox(@"TestData\Components\AT_GBX\GearboxSerialDualTCBus.vgbx");

			var ratios = new[] { 4.58, 2.4, 1.8, 1.3, 1.0 };
			Assert.AreEqual(ratios.Length, inputProvider.Gears.Count);
			for (int i = 0; i < ratios.Length; i++) {
				Assert.AreEqual(ratios[i], inputProvider.Gears[i].Ratio);
			}
			var gbxData = new EngineeringDataAdapter().CreateGearboxData(inputProvider,
				MockSimulationDataFactory.CreateEngineDataFromFile(@"TestData\Components\AT_GBX\Engine.veng", 0),
				(IGearshiftEngineeringInputData)inputProvider, 2.1,
				0.5.SI<Meter>(), VehicleCategory.InterurbanBus, (ITorqueConverterEngineeringInputData)inputProvider);
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

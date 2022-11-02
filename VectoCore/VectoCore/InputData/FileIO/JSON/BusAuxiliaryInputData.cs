using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public static class BusAuxiliaryInputData
	{
		public static IAuxiliaryConfig ReadBusAuxiliaries(string filename, IVehicleData vehicleData)
		{
			var json = JSONInputDataFactory.ReadFile(filename);
			var body = (JObject)json["Body"];

			return LoadValues(body, Path.GetDirectoryName(filename), vehicleData);
		}

		private static AuxiliaryConfig LoadValues(JObject data, string baseDir, IVehicleData vehicleData)
		{
			var ec = LoadElectricalConfig((JObject)data["ElectricalUserInputsConfig"], baseDir);
			var pac = LoadPneumaticsAuxConfig((JObject)data["PneumaticAuxillariesConfig"], baseDir);
			var puc = LoadPneumaticUserConfig((JObject)data["PneumaticUserInputsConfig"], baseDir);
			var env = string.IsNullOrWhiteSpace(data["EnvironmentalConditions"]?.ToString())
				? DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions
				: EnvironmentalContidionsMapReader.ReadFile(data["EnvironmentalConditions"].ToString());
			//var techList = string.IsNullOrWhiteSpace(data["SSMTechologies"]?.ToString())
			//	? DeclarationData.BusAuxiliaries.SSMTechnologyList
			//	: SSMTechnologiesReader.ReadFromFile(data["SSMTechologies"].ToString());
			var actuations = new Actuations() {
				Braking = data["Actuations"]?.GetEx<int>("Brakes") ?? 0,
				ParkBrakeAndDoors = data["Actuations"]?.GetEx<int>("Park brake + 2 doors") ?? 0,
				Kneeling = data["Actuations"]?.GetEx<int>("Kneeling") ?? 0,
				CycleTime = (data["Actuations"]?.GetEx<int>("CycleTime") ?? 3600).SI<Second>()
			};
				//ActuationsMapReader.Read(Path.Combine(baseDir, data.GetEx<string>("ActuationsMap")));
			var ssm = string.IsNullOrWhiteSpace(data["SSMFilePath"]?.ToString()) ?
				new SSMInputs("", FuelData.Diesel) {
					EnvironmentalConditionsMap =  env,
					Technologies = new TechnologyBenefits()
				}
				: SSMInputData.ReadFile(
				Path.Combine(baseDir, data["SSMFilePath"].ToString()), vehicleData, env);
			return new AuxiliaryConfig( ) {
				ElectricalUserInputsConfig = ec,
				PneumaticAuxillariesConfig  = pac,
				PneumaticUserInputsConfig  = puc,
				SSMInputs = ssm,
				Actuations = actuations,
				VehicleData = vehicleData
			};
		}

		private static IElectricsUserInputsConfig LoadElectricalConfig(JObject elData, string baseDir)
		{
			var electricalUserInputsConfig = new ElectricsUserInputsConfig();

			// AlternatorGearEfficiency
			electricalUserInputsConfig.AlternatorGearEfficiency = elData.GetEx<double>("AlternatorGearEfficiency");

			// AlternatorMap
			electricalUserInputsConfig.AlternatorMap = AlternatorReader.ReadMap(Path.Combine(baseDir , elData.GetEx("AlternatorMap").Value<string>()));

			// DoorActuationTimeSecond
			electricalUserInputsConfig.DoorActuationTimeSecond = elData.GetEx<double>("DoorActuationTimeSecond").SI<Second>();

			var averageCurrentDemandInclBaseLoad = elData["ElectricalConsumers"]
				.GetEx<double>("AverageCurrentDemandInclBaseLoad").SI<Ampere>();
			var averageCurrentDemandWithoutBaseLoad = elData["ElectricalConsumers"]
				.GetEx<double>("AverageCurrentDemandWithoutBaseLoad").SI<Ampere>();

			electricalUserInputsConfig.ElectricalConsumers = new Dictionary<string, ElectricConsumerEntry>();
			electricalUserInputsConfig.ElectricalConsumers["BaseLoad"] = new ElectricConsumerEntry() { 
				BaseVehicle = true,
				Current = averageCurrentDemandInclBaseLoad - averageCurrentDemandWithoutBaseLoad };

			electricalUserInputsConfig.ElectricalConsumers["Consumers"] = new ElectricConsumerEntry() {
				BaseVehicle = false,
				Current = averageCurrentDemandWithoutBaseLoad
			};

			// PowerNetVoltage
			electricalUserInputsConfig.PowerNetVoltage = elData.GetEx<double>("PowerNetVoltage").SI<Volt>();


			// SmartElectrical
			electricalUserInputsConfig.AlternatorType = elData["SmartElectrical"] != null 
				? (elData.GetEx<bool>("SmartElectrical") ? AlternatorType.Smart : AlternatorType.Conventional)
				: elData.GetEx<string>("AlternatorType").ParseEnum<AlternatorType>();

			// ResultCardIdle

			electricalUserInputsConfig.ResultCardIdle = electricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new ResultCard(
					elData["ResultCardIdle"].Select(
						result => new SmartResult(
							result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>())).ToList())
				: (IResultCard)new DummyResultCard();

			// ResultCardOverrun
			electricalUserInputsConfig.ResultCardOverrun = electricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new ResultCard(
					elData["ResultCardOverrun"].Select(
						result => new SmartResult(
							result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>())).ToList())
				: (IResultCard)new DummyResultCard();

			// ResultCardTraction
			electricalUserInputsConfig.ResultCardTraction = electricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new ResultCard(
					elData["ResultCardTraction"].Select(
						result => new SmartResult(
							result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>())).ToList())
				: (IResultCard)new DummyResultCard();

			return electricalUserInputsConfig;
		}

		private static IPneumaticsConsumersDemand LoadPneumaticsAuxConfig(JObject paData, string baseDir)
		{
			var pneumaticAuxillariesConfig = new PneumaticsConsumersDemand();
			pneumaticAuxillariesConfig.AdBlueInjection =
				paData.GetEx<double>("AdBlueNIperMinute").SI(Unit.SI.Liter.Per.Minute).Cast<NormLiterPerSecond>();
			pneumaticAuxillariesConfig.AirControlledSuspension =
				paData.GetEx<double>("AirControlledSuspensionNIperMinute").SI(Unit.SI.Liter.Per.Minute).Cast<NormLiterPerSecond>();
			pneumaticAuxillariesConfig.Braking = paData
				.GetEx<double>("BrakingNIperKG").SI(Unit.SI.Liter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>();
			//pneumaticAuxillariesConfig.BrakingWithRetarderNIperKG = paData
				//.GetEx<double>("BrakingWithRetarderNIperKG").SI(Unit.SI.Liter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>();
			pneumaticAuxillariesConfig.BreakingWithKneeling = paData
				.GetEx<double>("BreakingPerKneelingNIperKGinMM").SI(Unit.SI.Liter.Per.Kilo.Gramm.Milli.Meter)
				.Cast<NormLiterPerKilogramMeter>();
			pneumaticAuxillariesConfig.DeadVolBlowOuts =
				paData.GetEx<double>("DeadVolBlowOutsPerLitresperHour").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			pneumaticAuxillariesConfig.DeadVolume = paData.GetEx<double>("DeadVolumeLitres").SI<NormLiter>();
			pneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand =
				paData.GetEx<double>("NonSmartRegenFractionTotalAirDemand");
			pneumaticAuxillariesConfig.DoorOpening = paData.GetEx<double>("PerDoorOpeningNI").SI<NormLiter>();
			pneumaticAuxillariesConfig.StopBrakeActuation = paData
				.GetEx<double>("PerStopBrakeActuationNIperKG").SI(Unit.SI.Liter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>();
			pneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand =
				paData.GetEx<double>("SmartRegenFractionTotalAirDemand");
			pneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction =
				paData.GetEx<double>("OverrunUtilisationForCompressionFraction");
			return pneumaticAuxillariesConfig;
		}

		private static IPneumaticUserInputsConfig LoadPneumaticUserConfig(JObject puData, string baseDir)
		{
			var pneumaticUserInputsConfig = new PneumaticUserInputsConfig();
			//pneumaticUserInputsConfig.ActuationsMap = PneumaticActuationsMapReader.Read(Path.Combine(baseDir, puData.GetEx<string>("ActuationsMap")));
			pneumaticUserInputsConfig.AdBlueDosing = ConsumerTechnologyHelper.Parse(puData.GetEx<string>("AdBlueDosing"));
			pneumaticUserInputsConfig.AirSuspensionControl =
				ConsumerTechnologyHelper.Parse(puData.GetEx<string>("AirSuspensionControl"));
			pneumaticUserInputsConfig.CompressorGearEfficiency = puData.GetEx<double>("CompressorGearEfficiency");
			pneumaticUserInputsConfig.CompressorGearRatio = puData.GetEx<double>("CompressorGearRatio");
			var file = puData.GetEx<string>("CompressorMap");
			if (!string.IsNullOrWhiteSpace(file)) {
				pneumaticUserInputsConfig.CompressorMap = CompressorMapReader.ReadFile(Path.Combine(baseDir, file), 1.0, "");
			}
			pneumaticUserInputsConfig.Doors = ConsumerTechnologyHelper.Parse(puData.GetEx<string>("Doors"));
			pneumaticUserInputsConfig.KneelingHeight =
				puData.GetEx<double>("KneelingHeightMillimeters").SI(Unit.SI.Milli.Meter).Cast<Meter>();
			//pneumaticUserInputsConfig.RetarderBrake = puData.GetEx<bool>("RetarderBrake");
			pneumaticUserInputsConfig.SmartAirCompression = puData.GetEx<bool>("SmartAirCompression");
			pneumaticUserInputsConfig.SmartRegeneration = puData.GetEx<bool>("SmartRegeneration");
			return pneumaticUserInputsConfig;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class BusAuxWriter
	{
		public static bool SaveAuxConfig(IAuxiliaryConfig auxCfg, string auxFile)
		{
			var returnValue = true;

			// JSON METHOD
			try {
				var body = new Dictionary<string, object>();
				var basePath = Path.GetDirectoryName(Path.GetFullPath(auxFile));
				body["ElectricalUserInputsConfig"] = SaveElectricalConfig(auxCfg.ElectricalUserInputsConfig, basePath);
				body["PneumaticAuxillariesConfig"] = SavePneumaticAuxconfig(auxCfg.PneumaticAuxillariesConfig);
				body["PneumaticUserInputsConfig"] = SavePneumaticUserConfig(auxCfg.PneumaticUserInputsConfig, basePath);
				//body["HvacUserInputsConfig"] = SaveHVACUserConfig();

				if (auxCfg.SSMInputs is ISSMDeclarationInputs ssmInputs) {
					body["SSMFilePath"] = string.IsNullOrWhiteSpace(ssmInputs.Source)
						? ""
						: JSONFileWriter.GetRelativePath(ssmInputs.Source, basePath);
					body["EnvironmentalConditions"] =
						string.IsNullOrWhiteSpace(ssmInputs.EnvironmentalConditions.Source)
							? ""
							: JSONFileWriter.GetRelativePath(ssmInputs.EnvironmentalConditions.Source, basePath);
				}

				//string.IsNullOrWhiteSpace(auxCfg.SSMInputs.Technologies.Source)
					//? ""
					//: JSONFileWriter.GetRelativePath(auxCfg.SSMInputs.Technologies.Source, basePath);
				body["Actuations"] = new Dictionary<string, object>() {
					{"Brakes", auxCfg.Actuations.Braking },
					{"Park brake + 2 doors", auxCfg.Actuations.ParkBrakeAndDoors },
					{"Kneeling", auxCfg.Actuations.Kneeling },
					{"CycleTime", auxCfg.Actuations.CycleTime.Value() }
				};
				
				JSONInputDataFactory.WriteFile(JToken.FromObject(new Dictionary<string, object>() { { "Header", "AAUX" }, { "Body", body } }), auxFile);
			} catch (Exception) {
				returnValue = false;
			}

			return returnValue;
		}

		protected static Dictionary<string, object> SaveElectricalConfig(IElectricsUserInputsConfig electricalUserCfg, string auxPath)
		{
			var elData = new Dictionary<string, object>();
			// AlternatorGearEfficiency
			elData["AlternatorGearEfficiency"] = electricalUserCfg.AlternatorGearEfficiency;

			// AlternatorMap
			elData["AlternatorMap"] = JSONFileWriter.GetRelativePath(electricalUserCfg.AlternatorMap.Source, auxPath);

			// DoorActuationTimeSecond
			elData["DoorActuationTimeSecond"] = electricalUserCfg.DoorActuationTimeSecond.Value();

			// Electrical Consumer list

			elData["ElectricalConsumers"] = new Dictionary<string, object>() {
				{"AverageCurrentDemandInclBaseLoad", electricalUserCfg.AverageCurrentDemandInclBaseLoad(false, false).Value() },
				{"AverageCurrentDemandWithoutBaseLoad", electricalUserCfg.AverageCurrentDemandWithoutBaseLoad(false, false).Value() }
			};

			// PowerNetVoltage
			elData["PowerNetVoltage"] = electricalUserCfg.PowerNetVoltage.Value();

			// ResultCardIdle
			var resultCard = new List<object>();
			foreach (var result in electricalUserCfg.ResultCardIdle.Entries) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Key.Value()},
					{"SmartAmps",result.Value.Value() }
				});
			}

			elData["ResultCardIdle"] = resultCard;


			// ResultCardOverrun
			resultCard.Clear();
			foreach (var result in electricalUserCfg.ResultCardOverrun.Entries) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Key.Value()},
					{"SmartAmps",result.Value.Value() }
				});
			}

			elData["ResultCardOverrun"] = resultCard;

			// ResultCardTraction
			resultCard.Clear();
			foreach (var result in electricalUserCfg.ResultCardTraction.Entries) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Key.Value()},
					{"SmartAmps",result.Value.Value() }
				});
			}

			elData["ResultCardTraction"] = resultCard;

			// SmartElectrical
			elData["SmartElectrical"] = electricalUserCfg.AlternatorType;

			return elData;
		}


		private static Dictionary<string, object> SavePneumaticUserConfig(IPneumaticUserInputsConfig pneumaticUserCfg, string auxPath)
		{
			var puData = new Dictionary<string, object>();

			puData["AdBlueDosing"] = pneumaticUserCfg.AdBlueDosing.ToString();
			puData["AirSuspensionControl"] = pneumaticUserCfg.AirSuspensionControl.ToString();
			puData["CompressorGearEfficiency"] = pneumaticUserCfg.CompressorGearEfficiency;
			puData["CompressorGearRatio"] = pneumaticUserCfg.CompressorGearRatio;
			puData["CompressorMap"] = pneumaticUserCfg.CompressorMap == null ? "" : JSONFileWriter.GetRelativePath(pneumaticUserCfg.CompressorMap.Source, auxPath);
			puData["Doors"] = pneumaticUserCfg.Doors.ToString();
			puData["KneelingHeightMillimeters"] = pneumaticUserCfg.KneelingHeight.ConvertToMilliMeter().Value;
			//puData["RetarderBrake"] = pneumaticUserCfg.RetarderBrake;
			puData["SmartAirCompression"] = pneumaticUserCfg.SmartAirCompression;
			puData["SmartRegeneration"] = pneumaticUserCfg.SmartRegeneration;

			return puData;
		}

		protected static Dictionary<string, object> SavePneumaticAuxconfig(IPneumaticsConsumersDemand pneumaticAuxCfg)
		{
			var paData = new Dictionary<string, object>();

			paData["AdBlueNIperMinute"] = pneumaticAuxCfg.AdBlueInjection.ConvertToNlPerMin().Value;
			paData["AirControlledSuspensionNIperMinute"] = pneumaticAuxCfg.AirControlledSuspension.ConvertToNlPerMin().Value;
			paData["BrakingNIperKG"] = pneumaticAuxCfg.Braking.Value();
			paData["BreakingPerKneelingNIperKGinMM"] = pneumaticAuxCfg.BreakingWithKneeling.Value() / 1000;
			paData["DeadVolBlowOutsPerLitresperHour"] = pneumaticAuxCfg.DeadVolBlowOuts.ConvertToPerHour().Value;
			paData["DeadVolumeLitres"] = pneumaticAuxCfg.DeadVolume.Cast<CubicMeter>().ConvertToCubicDeziMeter().Value;
			paData["NonSmartRegenFractionTotalAirDemand"] = pneumaticAuxCfg.NonSmartRegenFractionTotalAirDemand;
			paData["PerDoorOpeningNI"] = pneumaticAuxCfg.DoorOpening.Cast<CubicMeter>().ConvertToCubicDeziMeter().Value;
			paData["PerStopBrakeActuationNIperKG"] = pneumaticAuxCfg.StopBrakeActuation.Value();
			paData["SmartRegenFractionTotalAirDemand"] = pneumaticAuxCfg.SmartRegenFractionTotalAirDemand;
			paData["OverrunUtilisationForCompressionFraction"] =
				pneumaticAuxCfg.OverrunUtilisationForCompressionFraction;

			return paData;
		}



		public static bool SaveSSMConfig(ISSMDeclarationInputs ssmInput, string filePath)
		{
			var returnValue = true;
			try {
				var body = new Dictionary<string, object>();
				body["SSMInputs"] = SaveGenInputs(ssmInput);
				body["SSMTechologyBenefits"] = new Dictionary<string, object>() {
					{ "Heating", ssmInput.Technologies.HValueVariation},
					{ "Cooling", ssmInput.Technologies.CValueVariation },
					{ "Ventilation", ssmInput.Technologies.VVValueVariation },
					{ "VentilationHeating", ssmInput.Technologies.VHValueVariation},
					{ "VentilationCooling", ssmInput.Technologies.VCValueVariation}
				};
				//body["TechList"] = SaveTechlist(ssmInput);

				JSONInputDataFactory.WriteFile(JToken.FromObject(new Dictionary<string, object>() { { "Header", "AHSM" }, { "Body", body } }), filePath);

			} catch (Exception) {
				// Nothing to do except return false.
				returnValue = false;
			}

			return returnValue;
		}

		private static Dictionary<string, object> SaveGenInputs(ISSMDeclarationInputs ssmInputs)
		{
			var retVal = new Dictionary<string, object>();

			retVal["BC_GFactor"] = ssmInputs.BoundaryConditions.GFactor;
			retVal["BC_HeatingBoundaryTemperature"] = ssmInputs.BoundaryConditions.HeatingBoundaryTemperature.AsDegCelsius;
			retVal["BC_CoolingBoundaryTemperature"] = ssmInputs.BoundaryConditions.CoolingBoundaryTemperature.AsDegCelsius;
			retVal["BC_VentilationRate"] = ssmInputs.BoundaryConditions.VentilationRate.ConvertToPerHour().Value;
			retVal["BC_VentilationRateHeating"] = ssmInputs.BoundaryConditions.VentilationRateHeating.ConvertToPerHour().Value;
			//retVal["BC_lowVentilation"] = ssmInputs.BoundaryConditions.LowVentilation.ConvertToPerHour().Value;
			retVal["BC_SpecificVentilationPower"] = ssmInputs.BoundaryConditions.SpecificVentilationPower.ConvertToWattHourPerCubicMeter().Value;
			retVal["BC_AuxHeaterEfficiency"] = ssmInputs.BoundaryConditions.AuxHeaterEfficiency;
			retVal["BC_GCVDieselOrHeatingOil"] = ssmInputs.BoundaryConditions.GCVDieselOrHeatingOil.ConvertToKiloWattHourPerKilogram().Value;
			retVal["BC_MaxTemperatureDeltaForLowFloorBusses"] = ssmInputs.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses.AsDegCelsius;
			retVal["BC_MaxPossibleBenefitFromTechnologyList"] = ssmInputs.BoundaryConditions.MaxPossibleBenefitFromTechnologyList;
			retVal["BC_UValue"] = ssmInputs.BoundaryConditions.UValue.Value();

			retVal["BP_FloorType"] = ssmInputs.BusParameters.BusFloorType.ToString();
			retVal["BP_BusSurfaceArea"] = ssmInputs.BusParameters.BusSurfaceArea.Value();
			retVal["BP_BusWindowSurfaceArea"] = ssmInputs.BusParameters.BusWindowSurface.Value();
			retVal["BP_BusVolume"] = ssmInputs.BusParameters.BusVolume.Value();
			retVal["BP_PassengerCount"] = ssmInputs.BusParameters.NumberOfPassengers;

			retVal["EC_EnviromentalTemperature"] = ssmInputs.EnvironmentalConditions.DefaultConditions.Temperature.AsDegCelsius;
			retVal["EC_Solar"] = ssmInputs.EnvironmentalConditions.DefaultConditions.Solar.Value();
			retVal["AC_CompressorType"] = ssmInputs.ACSystem.HVACCompressorType.ToString();
			retVal["AC_CompressorCapacitykW"] = ssmInputs.ACSystem.HVACMaxCoolingPower.ConvertToKiloWatt().Value;
			retVal["AC_COP"] = ssmInputs.ACSystem.COP;
			retVal["VEN_VentilationOnDuringHeating"] = ssmInputs.Ventilation.VentilationOnDuringHeating;
			retVal["VEN_VentilationWhenBothHeatingAndACInactive"] = ssmInputs.Ventilation.VentilationWhenBothHeatingAndACInactive;
			retVal["VEN_VentilationDuringAC"] = ssmInputs.Ventilation.VentilationDuringAC;
			//retVal["VEN_VentilationFlowSettingWhenHeatingAndACInactive"] = ssmInputs.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive;
			//retVal["VEN_VentilationDuringHeating"] = ssmInputs.Ventilation.VentilationDuringHeating;
			//retVal["VEN_VentilationDuringCooling"] = ssmInputs.Ventilation.VentilationDuringCooling;
			retVal["AH_FuelFiredHeaterkW"] = ssmInputs.AuxHeater.FuelFiredHeaterPower.ConvertToKiloWatt().Value;
			retVal["AH_FuelEnergyToHeatToCoolant"] = ssmInputs.AuxHeater.FuelEnergyToHeatToCoolant;
			retVal["AH_CoolantHeatTransferredToAirCabinHeater"] = ssmInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater;

			return retVal;
		}
	}
}

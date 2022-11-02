using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public static class SSMInputData
	{
		public static ISSMDeclarationInputs ReadStream(Stream str, IVehicleData vehicleData, IEnvironmentalConditionsMap env)
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(new StreamReader(str)));
			var body = (JObject)json["Body"];

			var retVal = Create(body, vehicleData, null);
			return retVal;
		}

		public static ISSMDeclarationInputs ReadFile(string fileName, IVehicleData vehicleData, IEnvironmentalConditionsMap env)
		{
			var json = JSONInputDataFactory.ReadFile(fileName);
			var body = (JObject)json["Body"];

			var retVal = Create(body, vehicleData, fileName);

			retVal.EnvironmentalConditionsMap = env;
			//retVal.Technologies = technologies;

			return retVal;
		}

		private static SSMInputs Create(JObject body, IVehicleData vehicleData, string fileName)
		{
			var genInput = ((JObject)body["SSMInputs"]);

			var retVal = new SSMInputs(fileName);
			
			retVal.GFactor = genInput.GetEx<double>("BC_GFactor");
			//retVal.PassengerBoundaryTemperature = genInput.GetEx<double>("BC_PassengerBoundaryTemperature").DegCelsiusToKelvin();
			retVal.HeatingBoundaryTemperature = genInput.GetEx<double>("BC_HeatingBoundaryTemperature").DegCelsiusToKelvin();
			retVal.CoolingBoundaryTemperature = genInput.GetEx<double>("BC_CoolingBoundaryTemperature").DegCelsiusToKelvin();
			retVal.VentilationRate = genInput.GetEx<double>("BC_VentilationRate").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			retVal.VentilationRateHeating = genInput.GetEx<double>("BC_VentilationRateHeating").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			//retVal.LowVentilation = genInput.GetEx<double>("BC_lowVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			retVal.SpecificVentilationPower = genInput.GetEx<double>("BC_SpecificVentilationPower").SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>();
			retVal.AuxHeaterEfficiency = genInput.GetEx<double>("BC_AuxHeaterEfficiency");
			retVal.UValue = genInput.GetEx<double>("BC_UValue").SI<WattPerKelvinSquareMeter>();
			retVal.COP = genInput.GetEx<double>("AC_COP");
			//retVal.GCVDieselOrHeatingOil = genInput.GetEx<double>("BC_GCVDieselOrHeatingOil").SI(Unit.SI.Kilo.Watt.Hour.Per.Kilo.Gramm).Cast<JoulePerKilogramm>();
																										//retVal.MaxTemperatureDeltaForLowFloorBusses = genInput.GetEx<double>("BC_MaxTemperatureDeltaForLowFloorBusses").SI<Kelvin>();
			retVal.MaxPossibleBenefitFromTechnologyList = genInput.GetEx<double>("BC_MaxPossibleBenefitFromTechnologyList");

			retVal.BusFloorType = genInput.GetEx<string>("BP_FloorType").ParseEnum<FloorType>();
			retVal.BusSurfaceArea = genInput.GetEx<double>("BP_BusSurfaceArea").SI<SquareMeter>();
			retVal.BusWindowSurface = genInput.GetEx<double>("BP_BusWindowSurfaceArea").SI<SquareMeter>();
			retVal.BusVolume = genInput.GetEx<double>("BP_BusVolume").SI<CubicMeter>();
			retVal.NumberOfPassengers = genInput.GetEx<double>("BP_PassengerCount");

			//retVal.EnviromentalTemperature = genInput.GetEx<double>("EC_EnviromentalTemperature").DegCelsiusToKelvin();
			//retVal.Solar = genInput.GetEx<double>("EC_Solar").SI<WattPerSquareMeter>();
			retVal.DefaultConditions = new EnvironmentalConditionMapEntry(
				genInput.GetEx<double>("EC_EnviromentalTemperature").DegCelsiusToKelvin(),
				genInput.GetEx<double>("EC_Solar").SI<WattPerSquareMeter>(), 1.0);
			//retVal.EnviromentalConditions_BatchFile = genInput.GetEx<string>("EC_EnviromentalConditions_BatchFile");
			//retVal.BatchMode = genInput.GetEx<bool>("EC_EnviromentalConditions_BatchEnabled");
			retVal.HVACCompressorType = HeatPumpTypeHelper.Parse(genInput.GetEx<string>("AC_CompressorType"));
			retVal.HVACMaxCoolingPower = genInput.GetEx<double>("AC_CompressorCapacitykW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			retVal.VentilationOnDuringHeating = genInput.GetEx<bool>("VEN_VentilationOnDuringHeating");
			retVal.VentilationWhenBothHeatingAndACInactive = genInput.GetEx<bool>("VEN_VentilationWhenBothHeatingAndACInactive");
			retVal.VentilationDuringAC = genInput.GetEx<bool>("VEN_VentilationDuringAC");
			//retVal.VentilationFlowSettingWhenHeatingAndACInactive = genInput.GetEx<string>("VEN_VentilationFlowSettingWhenHeatingAndACInactive").ParseEnum<VentilationLevel>();
			//retVal.VentilationDuringHeating = genInput.GetEx<string>("VEN_VentilationDuringHeating").ParseEnum<VentilationLevel>();
			//retVal.VentilationDuringCooling = genInput.GetEx<string>("VEN_VentilationDuringCooling").ParseEnum<VentilationLevel>();
			//retVal.EngineWasteHeatkW = genInput.GetEx<double>("AH_EngineWasteHeatkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			retVal.FuelFiredHeaterPower = genInput.GetEx<double>("AH_FuelFiredHeaterkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			retVal.FuelEnergyToHeatToCoolant = genInput.GetEx<double>("AH_FuelEnergyToHeatToCoolant");
			retVal.CoolantHeatTransferredToAirCabinHeater = genInput.GetEx<double>("AH_CoolantHeatTransferredToAirCabinHeater");

			var benefits = body["SSMTechologyBenefits"];
			retVal.Technologies = benefits != null
				? new TechnologyBenefits() {
					CValueVariation = benefits.GetEx<double>("Cooling"),
					HValueVariation = benefits.GetEx<double>("Heating"),
					VVValueVariation = benefits.GetEx<double>("Ventilation"),
					VHValueVariation = benefits.GetEx<double>("VentilationHeating"),
					VCValueVariation = benefits.GetEx<double>("VentilationCooling")
				}
				: new TechnologyBenefits();

			return retVal;
		}
	}
}

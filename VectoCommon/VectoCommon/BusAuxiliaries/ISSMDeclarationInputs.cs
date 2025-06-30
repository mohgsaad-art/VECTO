using System;
using System.ComponentModel;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface ISSMInputs { }

	public interface ISSMDeclarationInputs : ISSMInputs
	{

		ISSMBusParameters BusParameters { get; }

		ISSMTechnologyBenefits Technologies { get; }

		ISSMBoundaryConditions BoundaryConditions { get; }

		IEnvironmentalConditions EnvironmentalConditions { get; }

		double NumberOfPassengers { get; }

		IACSystem ACSystem { get; }

		IVentilation Ventilation { get; }

		IAuxHeater AuxHeater { get; }

		string HVACTechnology { get; }

		HeatingDistributionCase HeatingDistributionCaseDriver { get; }

		HeatingDistributionCase HeatingDistributionCasePassenger { get; }

		//HeatPumpType HeatPumpTypeHeatingDriverCompartment { get; }

		HeatPumpType HeatPumpTypeDriverCompartment { get; }
		
		//HeatPumpType HeatPumpTypeHeatingPassengerCompartment { get; }
		
		HeatPumpType HeatPumpTypePassengerCompartment { get; }

		BusHVACSystemConfiguration HVACSystemConfiguration { get; }

		string Source { get; }
		double ElectricWasteHeatToCoolant { get; }
	}

	public interface ISSMBusParameters
	{
		double NumberOfPassengers { get; }
		FloorType BusFloorType { get; }
		SquareMeter BusWindowSurface { get; }
		SquareMeter BusSurfaceArea { get; }
		CubicMeter BusVolumeVentilation { get; }
	}

	public interface ISSMBoundaryConditions
	{
		// Boundary Conditions:			
		double GFactor { get; }

		double SolarClouding(Kelvin envTemp);

		Watt HeatPerPassengerIntoCabin(Kelvin envTemp);

		WattPerKelvinSquareMeter UValue { get; }

		Kelvin HeatingBoundaryTemperature { get; }

		Kelvin CoolingBoundaryTemperature { get; }

		Kelvin TemperatureCoolingTurnsOff { get; }

		PerSecond VentilationRate { get; }
		
		PerSecond VentilationRateHeating { get; }

		Watt VentPower(bool heating);

		JoulePerCubicMeter SpecificVentilationPower { get; }

		double AuxHeaterEfficiency { get; }

		JoulePerKilogramm GCVDieselOrHeatingOil { get; }

		Kelvin MaxTemperatureDeltaForLowFloorBusses { get; }

		double MaxPossibleBenefitFromTechnologyList { get; }
	}

	public interface IEnvironmentalConditions
	{
		// EnviromentalConditions				
		IEnvironmentalConditionsMapEntry DefaultConditions { get; }

		IEnvironmentalConditionsMap EnvironmentalConditionsMap { get; }
		bool BatchMode { get; }

		string Source { get; }
	}

	public interface IACSystem
	{
		// AC-system				            
		//HeatPumpType HVACCompressorType { get; }

		Watt HVACMaxCoolingPower { get; }

		Watt HVACMaxCoolingPowerDriver { get; }

		Watt HVACMaxCoolingPowerPassenger { get; }

		HeaterType ElectricHeater { get; }

		Watt MaxHeatingPower { get; }

		Watt MaxHeatingPowerDriver { get; }

		Watt MaxHeatingPowerPassenger { get; }

		double DriverHVACContribution { get; }

		double PassengerHVACContribution { get; }

		//double COP { get; }
	}

	public interface IVentilation
	{
		// Ventilation				
		bool VentilationOnDuringHeating { get; }

		bool VentilationWhenBothHeatingAndACInactive { get; }

		bool VentilationDuringAC { get; }
	}


	//public enum ACCompressorType
	//{
	//	Unknown,
	//	None,
	//	TwoStage,
	//	ThreeStage,
	//	FourStage,
	//	Continuous
	//}

	//public static class ACCompressorTypeExtensions
	//{
	//	public static ACCompressorType ParseEnum(string txt)
	//	{
	//		switch (txt) {
	//			case "2-stage": return ACCompressorType.TwoStage;
	//			case "3-stage": return ACCompressorType.ThreeStage;
	//			case "4-stage": return ACCompressorType.FourStage;
	//			default: return txt.ParseEnum<ACCompressorType>();
	//		}
	//	}

	//	public static string ToString(this ACCompressorType type)
	//	{
	//		switch (type) {
	//			case ACCompressorType.TwoStage: return "2-stage";
	//			case ACCompressorType.ThreeStage: return "3-stage";
	//			case ACCompressorType.FourStage: return "4-stage";
	//			default: return type.ToString().ToLowerInvariant();
	//		}
	//	}

	//	public static string GetName(this ACCompressorType type)
	//	{
	//		return type.ToString();
	//	}

	//	public static string GetLabel(this ACCompressorType type)
	//	{
	//		switch (type)
	//		{
	//			case ACCompressorType.TwoStage: return "2-stage";
	//			case ACCompressorType.ThreeStage: return "3-stage";
	//			case ACCompressorType.FourStage: return "4-stage";
	//			default: return type.ToString();
	//		}
	//	}


	//	public static bool IsElectrical(this ACCompressorType type)
	//	{
	//		return type == ACCompressorType.Continuous;
	//	}

	//	public static bool IsMechanical(this ACCompressorType type)
	//	{
	//		return type != ACCompressorType.Continuous;
	//	}

	//	public static double COP(this ACCompressorType type, FloorType floortype)
	//	{
	//		var cop = 3.5;

	//		switch (type) {
	//			case ACCompressorType.None:
	//			case ACCompressorType.Unknown: return 0;
	//			case ACCompressorType.TwoStage: return cop;
	//			case ACCompressorType.ThreeStage:
	//			case ACCompressorType.FourStage: return cop * 1.02;
	//			case ACCompressorType.Continuous:
	//				return floortype == FloorType.LowFloor
	//					? cop * 1.04
	//					: cop * 1.06;
	//			default: throw new ArgumentOutOfRangeException();
	//		}
	//	}
	//}

	public interface IAuxHeater
	{
		Watt FuelFiredHeaterPower { get; }
		double FuelEnergyToHeatToCoolant { get; }
		double CoolantHeatTransferredToAirCabinHeater { get; }
	}


	public enum HeatPumpType
	{
		[GuiLabel("not applicable")]
		not_applicable,

		[GuiLabel("None")]
		none,

		[GuiLabel("R 744")]
		R_744,

		[GuiLabel("non R 744: 2-stage")]
		non_R_744_2_stage,

		[GuiLabel("non R 744: 3-stage")]
		non_R_744_3_stage,

		[GuiLabel("non R 744: 4-stage")]
		non_R_744_4_stage,

		[GuiLabel("non R 744: continuous")]
		non_R_744_continuous
	}

	public static class HeatPumpTypeHelper
	{
		private const string NONE = "none";
		private const string NOT_APPLICABLE = "not applicable";
		private const string R_744 = "R-744";
		private const string NON_R_744_2_STAGE = "non R-744 2-stage";
		private const string NON_R_744_3_STAGE = "non R-744 3-stage";
		private const string NON_R_744_4_STAGE = "non R-744 4-stage";
		private const string NON_R_744_CONTINUOUS = "non R-744 continuous";
		
		public static HeatPumpType? TryParse(string parse)
		{
			switch (parse)
			{
				case NONE: return HeatPumpType.none;
				case NOT_APPLICABLE: return HeatPumpType.not_applicable;
				case R_744: return HeatPumpType.R_744;
				case NON_R_744_2_STAGE: return HeatPumpType.non_R_744_2_stage;
				case NON_R_744_3_STAGE: return HeatPumpType.non_R_744_3_stage;
				case NON_R_744_4_STAGE: return HeatPumpType.non_R_744_4_stage;
				case NON_R_744_CONTINUOUS: return HeatPumpType.non_R_744_continuous;
				// to support old input parameters
				case "2-stage": return HeatPumpType.non_R_744_2_stage;
				case "3-stage": return HeatPumpType.non_R_744_3_stage;
				case "4-stage": return HeatPumpType.non_R_744_4_stage;
				default: return null; //throw new InvalidEnumArgumentException("HeatPumpType");
			}
		}

		public static HeatPumpType Parse(string parse)
		{
			var retVal = TryParse(parse);
			if (retVal == null) {
				throw new InvalidEnumArgumentException("HeatPumpType");
			}

			return retVal.Value;
		}

		public static string ToXML(this HeatPumpType type)
		{
			return type.GetLabel();
		}


		public static string GetLabel(this HeatPumpType? type)
		{
			if (type == null) {
				return "~null~";
			}
			switch (type) {
				case HeatPumpType.none: return NONE;
				case HeatPumpType.not_applicable: return NOT_APPLICABLE;
				case HeatPumpType.R_744: return R_744;
				case HeatPumpType.non_R_744_2_stage: return NON_R_744_2_STAGE;
				case HeatPumpType.non_R_744_3_stage: return NON_R_744_3_STAGE;
				case HeatPumpType.non_R_744_4_stage: return NON_R_744_4_STAGE;
				case HeatPumpType.non_R_744_continuous: return NON_R_744_CONTINUOUS;
				default: return null;
			}
		}

		public static string GetLabel(this HeatPumpType type)
		{
			return GetLabel(type as HeatPumpType?);
		}

		public static string GetName(this HeatPumpType type)
		{
			return type.ToString();
		}

		public static bool IsElectrical(this HeatPumpType type)
		{
			return type == HeatPumpType.R_744 || type == HeatPumpType.non_R_744_continuous;
		}

		public static bool IsMechanical(this HeatPumpType type)
		{
			return !type.IsElectrical();
		}

		//public static double COP(this HeatPumpType type, FloorType floortype)
		//{
		//	var cop = 3.5;

		//	switch (type) {
		//		case HeatPumpType.none:
		//		//case HeatPumpType.Unknown:
		//			return 0;
		//		case HeatPumpType.non_R_744_2_stage:
		//			return cop;
		//		case HeatPumpType.non_R_744_3_stage:
		//		case HeatPumpType.non_R_744_4_stage:
		//			return cop * 1.02;
		//		case HeatPumpType.non_R_744_continuous:
		//		case HeatPumpType.R_744:
		//			return floortype == FloorType.LowFloor
		//				? cop * 1.04
		//				: cop * 1.06;
		//		default:
		//			throw new ArgumentOutOfRangeException();
		//	}
		//}
	}


    [Flags]
	public enum HeaterType
	{
		None = 0,
		WaterElectricHeater = 1<<1,
		AirElectricHeater = 1<<2,
		OtherElectricHeating = 1<<3,
		FuelHeater = 1<<4,
	}

	public static class HeaterTypeHelper
	{
		private const string WATER_ELECTRIC_HEATER = "water electric heater";
		private const string AIR_ELECTRIC_HEATER = "air electric heater";
		private const string OTHER_ELECTRIC_HEATING = "other electric heating";
		private const string FUEL_HEATER = "fuel heater";

		public static HeaterType? TryParse(string parse)
		{
			switch (parse) {
				case WATER_ELECTRIC_HEATER: return HeaterType.WaterElectricHeater;
				case AIR_ELECTRIC_HEATER: return HeaterType.AirElectricHeater;
				case OTHER_ELECTRIC_HEATING: return HeaterType.OtherElectricHeating;
				case FUEL_HEATER: return HeaterType.FuelHeater;
				default: return null;
			}
		}
	}

	public interface ISSMEngineeringInputs : ISSMInputs
	{
		Watt ElectricPower { get; }

		Watt MechanicalPower { get; }

		Watt AuxHeaterPower { get; }

		Joule HeatingDemand { get; }

		double AuxHeaterEfficiency { get; set; }

		double FuelEnergyToHeatToCoolant { get; set; }

		double CoolantHeatTransferredToAirCabinHeater { get; set; }
		double ElectricWasteHeatToCoolant { get; }
	}

}

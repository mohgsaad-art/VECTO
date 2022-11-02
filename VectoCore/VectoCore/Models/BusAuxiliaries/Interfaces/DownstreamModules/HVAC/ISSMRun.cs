using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMRun
	{
		
		Watt TotalW(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
		//Watt PowerFuelHeater(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
		Watt TechListAmendedFuelHeater(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
	}
}

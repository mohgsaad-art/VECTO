using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public interface IFuelProperties
	{
		FuelType FuelType { get; }

		TankSystem? TankSystem { get; }

		KilogramPerCubicMeter FuelDensity { get; }

		double CO2PerFuelWeight { get; }

		JoulePerKilogramm LowerHeatingValueVecto { get; }

		JoulePerKilogramm LowerHeatingValueVectoEngine { get; }

		double HeatingValueCorrection { get; }

		string GetLabel();
	}
}
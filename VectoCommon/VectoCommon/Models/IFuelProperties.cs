using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
    public interface IFuelProperties
    {
        FuelType FuelType { get; }

        TankSystem? TankSystem { get; }

        KilogramPerCubicMeter FuelDensity { get; }

        double CO2PerFuelWeight { get; }

        JoulePerKilogramm LowerHeatingValueVecto { get; }

        JoulePerKilogramm LowerHeatingValueVectoEngine { get; }

        double HeatingValueCorrection { get; }
        double CO2PerFuelWeightVTP { get; }

        string GetLabel();
    }
}
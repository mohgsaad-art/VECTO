using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing
{
    public interface IFuelConsumptionCorrection
    {
        IFuelProperties Fuel { get; }
        KilogramPerWattSecond EngineLineCorrectionFactor { get; }
        KilogramPerWattSecond VehicleLine { get; }
        KilogramPerWattSecond FuelCellLine { get; }

        KilogramPerSecond FC_ESS_H { get; }
        KilogramPerSecond FC_ESS_CORR_H { get; }
        KilogramPerSecond FC_BusAux_PS_CORR_H { get; }
        KilogramPerSecond FC_BusAux_ES_CORR_H { get; }
        KilogramPerSecond FC_WHR_CORR_H { get; }
        KilogramPerSecond FC_AUXHTR_H { get; }
        KilogramPerSecond FC_AUXHTR_H_CORR { get; }
        KilogramPerSecond FC_REESS_SOC_H { get; }
        KilogramPerSecond FC_REESS_SOC_CORR_H { get; }
        KilogramPerSecond FC_FINAL_H { get; }
        KilogramPerMeter FC_WHR_CORR_KM { get; }
        KilogramPerMeter FC_BusAux_PS_CORR_KM { get; }
        KilogramPerMeter FC_BusAux_ES_CORR_KM { get; }
        KilogramPerMeter FC_AUXHTR_KM { get; }
        KilogramPerMeter FC_AUXHTR_KM_CORR { get; }
        KilogramPerMeter FC_REESS_SOC_KM { get; }
        KilogramPerMeter FC_REESS_SOC_CORR_KM { get; }
        KilogramPerMeter FC_ESS_KM { get; }

        KilogramPerMeter FC_ESS_CORR_KM { get; }
        KilogramPerMeter FC_FINAL_KM { get; }
        VolumePerMeter FuelVolumePerMeter { get; }

        Kilogram TotalFuelConsumptionCorrected { get; }
        Joule EnergyDemand { get; }
    }
}
using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing
{
    public interface ICorrectedModalData
    {
        WattSecond WorkESSMissing { get; }
        WattSecond WorkWHREl { get; }
        WattSecond WorkWHRElMech { get; }
        WattSecond WorkWHRMech { get; }
        WattSecond WorkWHR { get; }

        // mechanical compressor work applied when correcting fuel consumption
        WattSecond WorkBusAuxPSCorr { get; }

		// electric compressor work applied to REESS when calculating electric range (HEV CD, PEV) 
        WattSecond WorkBusAux_elPS_SoC_ElRange { get; }

		// electric compressor work applied to REESS correcting deltaSoC (and consequently deltaFC)
        WattSecond WorkBusAux_elPS_SoC_Corr { get; }

		// electric compressor work applied as fuel consumption
        WattSecond WorkBusAux_elPS_Corr_mech { get; }

        WattSecond WorkBusAuxESMech { get; }
        WattSecond WorkBusAuxHeatPumpHeatingElMech { get; }
        WattSecond WorkBusAuxHeatPumpHeatingMech { get; }

        WattSecond WorkBusAuxElectricHeater { get; }

        WattSecond WorkBusAuxCorr { get; }
        WattSecond EnergyDCDCMissing { get; }
        Joule AuxHeaterDemand { get; }

		NormLiter CorrectedAirDemand { get; }
		NormLiter DeltaAir { get; }
		IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel);

        KilogramPerMeter KilogramCO2PerMeter { get; }
        Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection { get; }
        Kilogram CO2Total { get; }
        Joule FuelEnergyConsumptionTotal { get; }
        WattSecond ElectricEnergyConsumption_SoC { get; set; }
        WattSecond ElectricEnergyConsumption_SoC_Corr { get; }
        WattSecondPerMeter ElectricEnergyConsumption_SoC_PerMeter { get; }

        WattSecond ElectricEnergyConsumption_Final { get; set; }
        WattSecondPerMeter ElectricEnergyConsumption_Final_PerMeter { get; }
		WattSecond DeltaEReessFuelCell { get; set; }
	}
}
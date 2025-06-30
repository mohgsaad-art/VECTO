using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public abstract class AbstractCorrectedModalData : ICorrectedModalData
	{
        protected readonly IModalDataContainer _modData;

        public AbstractCorrectedModalData(IModalDataContainer modData)
        {
            _modData = modData;
        }


        #region Implementation of ICorrectedModalData

        public virtual WattSecond WorkESSMissing { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHREl { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHRElMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHRMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHR => WorkWHRElMech + WorkWHRMech;
        public virtual WattSecond WorkBusAuxPSCorr { get; set; } = 0.SI<WattSecond>();
		public virtual WattSecond WorkBusAuxESMech { get; set; } = 0.SI<WattSecond>();
		public virtual WattSecond WorkBusAuxHeatPumpHeatingElMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkBusAuxHeatPumpHeatingMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkBusAuxElectricHeater { get; set; } = 0.SI<WattSecond>();
		
		// electric compressor work applied to REESS when calculating electric range (HEV CD, PEV) 
		public virtual WattSecond WorkBusAux_elPS_SoC_ElRange { get; set; } = 0.SI<WattSecond>();

        // electric compressor work applied to REESS correcting deltaSoC (and consequently deltaFC)
		public virtual WattSecond WorkBusAux_elPS_SoC_Corr { get; set; } = 0.SI<WattSecond>();

        // electric compressor work applied as fuel consumption
        public virtual WattSecond WorkBusAux_elPS_Corr_mech { get; set; } = 0.SI<WattSecond>();


        public virtual WattSecond WorkBusAuxCorr => WorkBusAuxPSCorr + WorkBusAuxESMech;
        public virtual WattSecond EnergyDCDCMissing { get; set; } = 0.SI<WattSecond>();
        public virtual Joule AuxHeaterDemand { get; set; } = 0.SI<WattSecond>();
		public virtual KilogramPerMeter KilogramCO2PerMeter { get; set; } = 0.SI<KilogramPerMeter>();
        public virtual Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection { get; protected set; } = new Dictionary<FuelType, IFuelConsumptionCorrection>();
        public virtual Kilogram CO2Total { get; set; } = 0.SI<Kilogram>();
        public virtual Joule FuelEnergyConsumptionTotal => 0.SI<Joule>();
        public virtual WattSecond ElectricEnergyConsumption_SoC { get; set; } = 0.SI<WattSecond>();
		public virtual WattSecond ElectricEnergyConsumption_SoC_Corr
		{
			get => ElectricEnergyConsumption_SoC + WorkBusAux_elPS_SoC_ElRange;
			set => throw new NotImplementedException();
		}

		public virtual WattSecond ElectricEnergyConsumption_Final { get; set; } = 0.SI<WattSecond>();


		public abstract IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel);

        #endregion

        public virtual NormLiter CorrectedAirDemand { get; set; }
        public virtual NormLiter DeltaAir { get; set; }

        public WattSecondPerMeter ElectricEnergyConsumption_SoC_PerMeter => ElectricEnergyConsumption_SoC == null || _modData.Distance.IsEqual(0)
            ? null
            : ElectricEnergyConsumption_SoC / _modData.Distance;

        public WattSecondPerMeter ElectricEnergyConsumption_Final_PerMeter =>
            ElectricEnergyConsumption_Final == null || _modData.Distance.IsEqual(0)
                ? null
                : ElectricEnergyConsumption_Final / _modData.Distance;


		public WattSecond DeltaEReessFuelCell { get; set; }
    }


    public class CorrectedModalData : AbstractCorrectedModalData
    {
        public SI kAir { get; set; }

		#region Implementation of ICorrectedModalData

        public CorrectedModalData(IModalDataContainer modData) : base(modData)
        {

            //FuelCorrection = new Dictionary<FuelType, IFuelConsumptionCorrection>();
        }

        public override IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
        {
            if (!FuelCorrection.ContainsKey(fuel.FuelType)) {
                throw new VectoException("Invalid fuel {0}", fuel);
            }

            return FuelCorrection[fuel.FuelType];
        }

        public virtual Second ICEOffTimeStandstill { get; set; }
        public virtual WattSecond EnergyAuxICEOffStandstill { get; set; }
        public virtual WattSecond EnergyAuxICEOffStandstill_UF {
            get {
                return EnergyAuxICEOffStandstill * UtilityFactorStandstill;
            }
        }

        public virtual double UtilityFactorStandstill { get; set; }

        public virtual WattSecond EnergyAuxICEOnStandstill { get; set; }
        public virtual WattSecond EnergyAuxICEOnStandstill_UF {
            get { return EnergyAuxICEOnStandstill * (1 - UtilityFactorStandstill); }
        }
        public virtual Watt AvgAuxPowerICEOnStandstill => ICEOffTimeStandstill.IsEqual(0) ? 0.SI<Watt>() : EnergyAuxICEOnStandstill / ICEOffTimeStandstill;


        public virtual Second ICEOffTimeDriving { get; set; }
        public virtual WattSecond EnergyAuxICEOffDriving { get; set; }
        public virtual WattSecond EnergyAuxICEOffDriving_UF { get { return EnergyAuxICEOffDriving * UtilityFactorDriving; } }
        public virtual double UtilityFactorDriving { get; set; }

        public virtual WattSecond EnergyAuxICEOnDriving { get; set; }
        public virtual WattSecond EnergyAuxICEOnDriving_UF {
            get {
                return EnergyAuxICEOnDriving * (1 - UtilityFactorDriving);
            }
        }
        public virtual Watt AvgAuxPowerICEOnDriving => ICEOffTimeDriving.IsEqual(0) ? 0.SI<Watt>() : EnergyAuxICEOnDriving / ICEOffTimeDriving;

        //public virtual WattSecond EnergyDCDCMissing { get; set; }
        public virtual WattSecond DeltaEReessMech { get; set; }



		#endregion
    }

	public class PEVCorrectedModalData : AbstractCorrectedModalData
	{

		public PEVCorrectedModalData(IModalDataContainer modData) : base(modData)
		{

		}



		public override IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			return new NoFuelConsumptionCorrection();
		}
	}

	public class FCHVCorrectedModalData : AbstractCorrectedModalData
	{
		public FCHVCorrectedModalData(IModalDataContainer modData) : base(modData)
		{

		}

		

		public override IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			if (fuel.FuelType != FuelType.H2FC || !FuelCorrection.ContainsKey(fuel.FuelType))
			{
				throw new VectoException("Invalid fuel {0}", fuel);
			}

			return FuelCorrection[fuel.FuelType];
		}

		public override WattSecond ElectricEnergyConsumption_SoC_Corr { get; set; }
    }

	public class EngineOnlyCorrectedModalData : AbstractCorrectedModalData
	{
		public EngineOnlyCorrectedModalData(IModalDataContainer modData) : base(modData)
		{

		}

		public override IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			return new NoFuelConsumptionCorrection();
		}
    }
}
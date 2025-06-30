using System;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
    // Used by SSMHVAC Class
    public class SSMInputs : ISSMDeclarationInputs, ISSMBoundaryConditions, IEnvironmentalConditions, IACSystem, IVentilation,
		IAuxHeater, ISSMBusParameters
	{
		private readonly IFuelProperties HeatingFuel;
		private HeatingDistributionCase? _heatingDistributionDriverCase;
		private HeatingDistributionCase? _heatingDistributionPassengerCase;

		public SSMInputs(string source, IFuelProperties heatingFuel = null)
		{
			Source = source;
			HeatingFuel = heatingFuel ?? FuelData.Diesel;
			DriverCompartmentLength = 0.SI<Meter>();
			PassengerCompartmentLength = 0.SI<Meter>();
		}

		public string Source { get; }

		// C5/D5
		public double NumberOfPassengers { get; internal set; }

		// C6/D6
		public FloorType BusFloorType { get; internal set; }

		// D8/C8 - ( M/2 )
		public SquareMeter BusSurfaceArea { get; internal set; }

		// D9/C9 - ( M/2 )
		public SquareMeter BusWindowSurface { get; internal set; }

		// D11/C11 - ( M/3 )
		public CubicMeter BusVolumeVentilation { get; internal set; }

		// C17
		public double GFactor { get; set; }

		// C18            
		public double SolarClouding(Kelvin enviromentalTemperature)
		{
			// =IF(C46<17,0.65,0.8)
			return enviromentalTemperature < Constants.BusAuxiliaries.SteadyStateModel.PassengerBoundaryTemperature
				? Constants.BusAuxiliaries.SteadyStateModel.SolarCloudingLow
				: Constants.BusAuxiliaries.SteadyStateModel.SolarCloudingHigh;
		}

		// C19 - ( W )
		public Watt HeatPerPassengerIntoCabin(Kelvin enviromentalTemperature)
		{
			// =IF(C46<17,50,80)
			return enviromentalTemperature < Constants.BusAuxiliaries.SteadyStateModel.PassengerBoundaryTemperature
				? Constants.BusAuxiliaries.SteadyStateModel.HeatPerPassengerIntoCabinLow
				: Constants.BusAuxiliaries.SteadyStateModel.HeatPerPassengerIntoCabinHigh;
		}


		// C25 - ( W/K/M3 )
		public WattPerKelvinSquareMeter UValue { get; internal set; }

		// C26 - ( oC )
		public Kelvin HeatingBoundaryTemperature { get; set; }

		// C27 - ( oC )
		public Kelvin CoolingBoundaryTemperature { get; set; }

		// C28 - ( oC )
		public Kelvin TemperatureCoolingTurnsOff => 17.0.DegCelsiusToKelvin();

		// C29 - ( L/H )  --- !! 1/h
		public PerSecond VentilationRate { get; set; }

		public PerSecond VentilationRateHeating { get; internal set; }

		// C33 - ( W )
		public Watt VentPower(bool heating)
		{
			// =C31*C35
			return BusVolumeVentilation * (heating ? VentilationRateHeating : VentilationRate) * SpecificVentilationPower;
		}

		// C35 - ( Wh/M3 )
		public JoulePerCubicMeter SpecificVentilationPower { get; set; }

		// C37               
		public double AuxHeaterEfficiency { get; set; }

		// C38 - ( KW/HKG )
		public JoulePerKilogramm GCVDieselOrHeatingOil => HeatingFuel.LowerHeatingValueVecto;

		// C42 - ( K )
		public Kelvin MaxTemperatureDeltaForLowFloorBusses => Constants.BusAuxiliaries.SteadyStateModel.MaxTemperatureDeltaForLowFloorBusses;

		// C43 - ( Fraction )
		public double MaxPossibleBenefitFromTechnologyList { get; set; }


		public IEnvironmentalConditionsMapEntry DefaultConditions { get; set; }

		// ( EC_EnviromentalTemperature and  EC_Solar) (Batch Mode)
		public IEnvironmentalConditionsMap EnvironmentalConditionsMap { get; set; }

		public bool BatchMode => EnvironmentalConditionsMap != null && EnvironmentalConditionsMap.GetEnvironmentalConditions().Any();


		// C53 - "Continous/2-stage/3-stage/4-stage
		//public HeatPumpType HVACCompressorType { get; set; }


		// C54 -  ( KW )
		public Watt HVACMaxCoolingPower => (HVACMaxCoolingPowerDriver ?? 0.SI<Watt>()) + (HVACMaxCoolingPowerPassenger ?? 0.SI<Watt>());

		public Watt HVACMaxCoolingPowerDriver { get; set; }

		public Watt HVACMaxCoolingPowerPassenger { get; set; }

        // C59
        //public double COP { get; set; }


        // C62 - Boolean Yes/No
        public bool VentilationOnDuringHeating { get; set; }

		// C63 - Boolean Yes/No
		public bool VentilationWhenBothHeatingAndACInactive { get; set; }

		// C64 - Boolean Yes/No
		public bool VentilationDuringAC { get; set; }

		// C71 - ( KW )
		public Watt FuelFiredHeaterPower { get; set; }

		public double FuelEnergyToHeatToCoolant { get; set; }

		public double CoolantHeatTransferredToAirCabinHeater { get; set; }

		public double ElectricWasteHeatToCoolant { get; set; }

		#region Implementation of ISSMInputs

		[JsonIgnore]
		public ISSMBusParameters BusParameters => this;

		[JsonIgnore]
		public ISSMBoundaryConditions BoundaryConditions => this;

		[JsonIgnore]
		public IEnvironmentalConditions EnvironmentalConditions => this;

		[JsonIgnore]
		public IACSystem ACSystem => this;

		[JsonIgnore]
		public IVentilation Ventilation => this;

		[JsonIgnore]
		public IAuxHeater AuxHeater => this;

		public ISSMTechnologyBenefits Technologies { get; set; }
		public string HVACTechnology => $"{HVACSystemConfiguration.GetName()} " +
										$"({string.Join(", ", HeatPumpTypePassengerCompartment.GetName(), HeatPumpTypeDriverCompartment.GetName())})";

		public HeatingDistributionCase HeatingDistributionCaseDriver
		{
			get
			{
				if (!_heatingDistributionDriverCase.HasValue) {
					_heatingDistributionDriverCase = GetHeatingDistributionCase(HeatPumpTypeDriverCompartment);
				}

				return _heatingDistributionDriverCase.Value;
			}
		}

		public HeatingDistributionCase HeatingDistributionCasePassenger {
			get {
				if (!_heatingDistributionPassengerCase.HasValue) {
					_heatingDistributionPassengerCase = GetHeatingDistributionCase(HeatPumpTypePassengerCompartment);
				}

				return _heatingDistributionPassengerCase.Value;
			}
		}

		protected virtual HeatingDistributionCase GetHeatingDistributionCase(HeatPumpType heatPump)
		{
			return HeatingDistributions?.GetHeatingDistributionCase(heatPump, ElectricHeater,
				AuxHeater.FuelFiredHeaterPower.IsGreater(0)) ?? HeatingDistributionCase.HeatingDistribution_NotAvailable_;
		}

		public HeatingDistributionCasesMap HeatingDistributions { get; set; }

		//public HeatPumpType HeatPumpTypeHeatingDriverCompartment { get; set; }
		public HeatPumpType HeatPumpTypeDriverCompartment { get; set; }
		//public HeatPumpType HeatPumpTypeHeatingPassengerCompartment { get; set; }
		public HeatPumpType HeatPumpTypePassengerCompartment { get; set; }
		public BusHVACSystemConfiguration HVACSystemConfiguration { get; set; }
		public HeaterType ElectricHeater { get; set; }
		public Watt MaxHeatingPower => (MaxHeatingPowerDriver ?? 0.SI<Watt>()) + (MaxHeatingPowerPassenger ?? 0.SI<Watt>());

		public Watt MaxHeatingPowerDriver { get; set; }
		public Watt MaxHeatingPowerPassenger { get; set; }

		public Meter DriverCompartmentLength { get; set; }

		public Meter PassengerCompartmentLength { get; set; }

		public double DriverHVACContribution => (DriverCompartmentLength + PassengerCompartmentLength).IsEqual(0)
			? 0
			: (DriverCompartmentLength / (DriverCompartmentLength + PassengerCompartmentLength)).Value();

		public double PassengerHVACContribution => (DriverCompartmentLength + PassengerCompartmentLength).IsEqual(0)
			? 0
			: (PassengerCompartmentLength / (DriverCompartmentLength + PassengerCompartmentLength)).Value();

		#endregion
	}

	public class SSMEngineeringInputs :ISSMEngineeringInputs
	{
		#region Implementation of ISSMEngineeringInputs

		public Watt ElectricPower { get; set; }
		public Watt MechanicalPower { get; set; }
		public Watt AuxHeaterPower { get; set; }
		public Joule HeatingDemand { get; set; }
		public double AuxHeaterEfficiency { get; set; }
		public double FuelEnergyToHeatToCoolant { get; set; }
		public double CoolantHeatTransferredToAirCabinHeater { get; set; }

		public double ElectricWasteHeatToCoolant { get; set; }
		#endregion
	}

}

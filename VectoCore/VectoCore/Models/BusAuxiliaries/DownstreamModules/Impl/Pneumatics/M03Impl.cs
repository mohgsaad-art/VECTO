using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class M03Impl : AbstractModule, IM3_AveragePneumaticLoadDemand
	{
		protected ICompressorMap _pneumaticsCompressorFlowRateMap;
		protected ISignals _signals;
		protected double _compressorGearEfficiency;
		protected double _compressorGearRatio;


		//public M03Impl(IPneumaticUserInputsConfig pneumaticsUserInputConfig, IPneumaticsAuxilliariesConfig pneumaticsAuxillariesConfig, IPneumaticActuationsMap pneumaticsActuationsMap, ICompressorMap pneumaticsCompressorFlowRateMap, Kilogram vehicleMassKG, string cycleName, ISignals signals)
		public M03Impl(IAuxiliaryConfig auxConfig, ICompressorMap compressorMap, IActuations actuations, ISignals signals)
		{
			_pneumaticsCompressorFlowRateMap = compressorMap;
			_compressorGearEfficiency = auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency;
			_compressorGearRatio = auxConfig.PneumaticUserInputsConfig.CompressorGearRatio;

			_signals = signals;

			//'Calculate the Total Required Air Delivery Rate L / S
			var totalAirDemand = TotalAirDemandCalculation(auxConfig, actuations);
			AverageAirConsumed = totalAirDemand / actuations.CycleTime;
		}


		public static NormLiter TotalAirDemandCalculation(IAuxiliaryConfig auxConfig, IActuations actuations)
		{
			var psUserConfig = auxConfig.PneumaticUserInputsConfig;
			var psAuxconfig = auxConfig.PneumaticAuxillariesConfig;
			var vehicleMass = auxConfig.VehicleData.TotalVehicleMass;

			//'* * Breaks * *
			var airConsumptionPerActuation = psAuxconfig.Braking * vehicleMass;
			var breaks = actuations.Braking * airConsumptionPerActuation;

			//'* * ParkBrakesBreakplus2Doors * *Park break +2 doors
			airConsumptionPerActuation = psUserConfig.Doors == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: (psUserConfig.Doors == ConsumerTechnology.Mixed ? 0.5 : 1) * psAuxconfig.DoorOpening;
			airConsumptionPerActuation += psAuxconfig.StopBrakeActuation * vehicleMass;
			var parkBrakesplus2Doors = actuations.ParkBrakeAndDoors * airConsumptionPerActuation;

			//'* * Kneeling * *
			airConsumptionPerActuation = psAuxconfig.BreakingWithKneeling *
										psUserConfig.KneelingHeight * vehicleMass;
			var kneeling = actuations.Kneeling * airConsumptionPerActuation;

			//'* * AdBlue * *
			var adBlue = psUserConfig.AdBlueDosing == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: psAuxconfig.AdBlueInjection * actuations.CycleTime;

			//'* * Regeneration * *
			var regeneration = breaks + parkBrakesplus2Doors + kneeling + adBlue;
			var regenFraction = psUserConfig.SmartRegeneration
				? psAuxconfig.SmartRegenFractionTotalAirDemand
				: psAuxconfig.NonSmartRegenFractionTotalAirDemand;
			regeneration = regeneration * regenFraction;

			//'* * DeadVolBlowOuts * *
			airConsumptionPerActuation = psAuxconfig.DeadVolume;
			var deadVolBlowOuts = (airConsumptionPerActuation * psAuxconfig.DeadVolBlowOuts *
									actuations.CycleTime).Cast<NormLiter>();

			//'* * AirSuspension * *
			var airSuspension = psUserConfig.AirSuspensionControl == ConsumerTechnology.Electrically
				? 0.SI<NormLiter>()
				: psAuxconfig.AirControlledSuspension * actuations.CycleTime;

			//'* * Total Air Demand**
			var totalAirDemand = breaks + parkBrakesplus2Doors + kneeling + adBlue + regeneration + deadVolBlowOuts +
								airSuspension;

			return totalAirDemand;
		}


		#region Implementation of IM3_AveragePneumaticLoadDemand

		public Watt GetAveragePowerDemandAtCrankFromPneumatics()
		{
			var cmp = _pneumaticsCompressorFlowRateMap.Interpolate(
				_signals.EngineSpeed * _compressorGearRatio);

			var sum6 = cmp.FlowRate;
			var sum7 = cmp.PowerOn - cmp.PowerOff;
			var sum2 = sum7 / sum6 * AverageAirConsumed; // ' Watt / Nl/s * Nl/s = Watt
			var sum3 = sum2 + cmp.PowerOff;
			var sum4 = sum3 * (1 / _compressorGearEfficiency);
			return sum4;
		}

		public NormLiterPerSecond AverageAirConsumed { get; }

		#endregion
	}
}

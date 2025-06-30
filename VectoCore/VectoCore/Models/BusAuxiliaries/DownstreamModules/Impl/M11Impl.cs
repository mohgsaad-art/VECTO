using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
    public class M11Impl : AbstractModule, IM11
	{
		
#region "Private Aggregates"
		protected Joule AG1;
		protected Joule AG2;
		protected Joule AG3;
		protected Kilogram AG4;
		protected Kilogram AG5;
		protected Joule AG6;
		protected Kilogram AG7;
#endregion

		protected readonly IM1_AverageHVACLoadDemand M1;
		protected readonly IM3_AveragePneumaticLoadDemand M3;
		protected readonly IM6 M6;
		protected readonly IM8 M8;
		protected readonly IFuelConsumptionMap FMAP;
		protected readonly ISignals Signals;

		public M11Impl(IM1_AverageHVACLoadDemand m1, IM3_AveragePneumaticLoadDemand m3, IM6 m6, IM8 m8, IFuelConsumptionMap fmap, ISignals signals)
		{
			M1 = m1;
			M3 = m3;
			M6 = m6;
			M8 = m8;
			FMAP = fmap;
			Signals = signals;
			ClearAggregates();
		}

		#region Implementation of IM11

		public Joule SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly => AG1;

		public Joule SmartElectricalTotalCycleEletricalEnergyGenerated => AG2;

		public Joule TotalCycleElectricalDemand => AG3;

		public Kilogram TotalCycleFuelConsumptionSmartElectricalLoad => AG4;

		public Kilogram TotalCycleFuelConsumptionZeroElectricalLoad => AG5;

		public Joule StopStartSensitiveTotalCycleElectricalDemand => AG6;

		public Kilogram TotalCycleFuelConsuptionAverageLoads => AG7;

		public void ClearAggregates()
		{
			AG1 = 0.SI<Joule>();
			AG2 = 0.SI<Joule>();
			AG3 = 0.SI<Joule>();
			AG4 = 0.SI<Kilogram>();
			AG5 = 0.SI<Kilogram>();
			AG6 = 0.SI<Joule>();
			AG7 = 0.SI<Kilogram>();
		}

		protected PerSecond Sum0(PerSecond rpm)
		{
			if (rpm < 1) {
				rpm = 1.RPMtoRad();
			}

			return rpm; // ' / RPM_to_RadiansPerSecond
		}

		public void CycleStep(Second stepTimeInSeconds)
		{
			//'S/S Insensitive
			AG3 += M6.AvgPowerDemandAtCrankFromElectricsIncHVAC * stepTimeInSeconds;


			if (Signals.EngineStopped) {
				return;
			}

			// 'S/S Sensitive
			
			var sum1 = M6.OverrunFlag ? M8.SmartElectricalAlternatorPowerGenAtCrank : 0.SI<Watt>();
			AG1 += (sum1 * stepTimeInSeconds);
			AG2 += (M8.SmartElectricalAlternatorPowerGenAtCrank * stepTimeInSeconds);

			AG6 += (M6.AvgPowerDemandAtCrankFromElectricsIncHVAC * stepTimeInSeconds);

			var sum2 = M3.GetAveragePowerDemandAtCrankFromPneumatics() + M1.AveragePowerDemandAtCrankFromHVACMechanicals;
			var sum3 = M8.SmartElectricalAlternatorPowerGenAtCrank / Sum0(Signals.EngineSpeed);
			var sum4 = sum2 / Sum0(Signals.EngineSpeed);
			var sum9 = Signals.EngineDrivelineTorque +
						(Signals.PreExistingAuxPower / Sum0(Signals.EngineSpeed));
			var sum5 = sum4 + sum9;
			var sum6 = sum3 + sum5;
			var intrp1 = FMAP.GetFuelConsumptionValue(sum6, Signals.EngineSpeed);
			intrp1 = !double.IsNaN(intrp1.Value()) && intrp1 > 0 ? intrp1 : 0.SI<KilogramPerSecond>();
			var sum7 = intrp1;

			var intrp2 = FMAP.GetFuelConsumptionValue(sum5, Signals.EngineSpeed);
			intrp2 = !double.IsNaN(intrp2.Value()) && intrp2 > 0 ? intrp2 : 0.SI<KilogramPerSecond>();
			var sum8 = intrp2;

			var sum10 = M6.AvgPowerDemandAtCrankFromElectricsIncHVAC / Sum0(Signals.EngineSpeed);
			var sum11 = sum5 + sum10;
			var intrp3 = FMAP.GetFuelConsumptionValue(sum11, Signals.EngineSpeed);
			intrp3 = !double.IsNaN(intrp3.Value()) && intrp3 > 0 ? intrp3 : 0.SI<KilogramPerSecond>();
			var sum12 = intrp3;

			//'MQ: No longer needed - already per Second 'These need to be divided by 3600 as the Fuel
			//	Map output is in Grams / Second.
			AG4 += sum7 * stepTimeInSeconds; // '/ 3600
			AG5 += sum8 * stepTimeInSeconds; // '  / 3600
			AG7 += sum12 * stepTimeInSeconds; // '/ 3600
		}

		#endregion
	}
}

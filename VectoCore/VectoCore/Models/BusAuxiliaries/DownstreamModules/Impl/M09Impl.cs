using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
    public class M09Impl : AbstractModule, IM9
	{
		
		#region "Aggregates"

		//'AG1
		protected NormLiter _LitresOfAirCompressorOnContinuallyAggregate;

		//'AG2
		protected NormLiter _LitresOfAirCompressorOnOnlyInOverrunAggregate;

		//'AG3
		protected Kilogram _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate;

		//'AG4
		protected Kilogram _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate;

		#endregion

		protected readonly IM1_AverageHVACLoadDemand M1;
		protected readonly IM4_AirCompressor M4;
		protected readonly IM6 M6;
		protected readonly IM8 M8;
		protected readonly IFuelConsumptionMap FMAP;
		protected readonly IPneumaticsConsumersDemand PSAC;
		protected readonly ISignals Signals;

		public M09Impl(IM1_AverageHVACLoadDemand m1, IM4_AirCompressor m4, IM6 m6, IM8 m8, IFuelConsumptionMap fmap, IPneumaticsConsumersDemand psac, ISignals signals)
		{
			M1 = m1;
			M4 = m4;
			M6 = m6;
			M8 = m8;
			FMAP = fmap;
			PSAC = psac;
			Signals = signals;
			ClearAggregates();
		}


		#region Implementation of IAuxiliaryEvent

		//public event AuxiliaryEventEventHandler AuxiliaryEvent;

		#endregion

		#region Implementation of IM9

		public void ClearAggregates()
		{
			_LitresOfAirCompressorOnContinuallyAggregate = 0.SI<NormLiter>();
			_LitresOfAirCompressorOnOnlyInOverrunAggregate = 0.SI<NormLiter>();
			_TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate = 0.SI<Kilogram>();
			_TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate = 0.SI<Kilogram>();
		}

		protected PerSecond S0(PerSecond rpm)
		{
			if (rpm < 1) {
				rpm = 1.RPMtoRad();
			}

			return rpm;
		}

		public void CycleStep(Second stepTimeInSeconds)
		{
			if (Signals.EngineStopped) {
				return;
			}

			var s9 = M6.OverrunFlag && M8.CompressorFlag ? M4.GetFlowRate() : 0.SI<NormLiterPerSecond>();
			var s13 = Signals.ClutchEngaged && !(Signals.InNeutral) ? s9 : 0.SI<NormLiterPerSecond>();
			var s10 = s13 * PSAC.OverrunUtilisationForCompressionFraction;

			if (S0(Signals.EngineSpeed).IsEqual(0)) {
				throw new DivideByZeroException("Engine speed is zero and cannot be used as a divisor.");
			}

			var s1 = M6.AvgPowerDemandAtCrankFromElectricsIncHVAC + M1.AveragePowerDemandAtCrankFromHVACMechanicals;
			var s2 = M4.GetPowerCompressorOn() / S0(Signals.EngineSpeed);
			var s3 = M4.GetPowerCompressorOff() / S0(Signals.EngineSpeed);
			var s4 = s1 / S0(Signals.EngineSpeed);
			var s14 = Signals.EngineDrivelineTorque +
					Signals.PreExistingAuxPower / S0(Signals.EngineSpeed);
			var s5 = s2 + s14;
			var s6 = s14 + s3;
			var s7 = s4 + s5;
			var s8 = s4 + s6;

			var int1 = FMAP.GetFuelConsumptionValue(s7, Signals.EngineSpeed);
			int1 = int1 > 0 && !double.IsNaN(int1.Value()) ? int1 : 0.SI<KilogramPerSecond>();
			var s11 = int1;

			var int2 = FMAP.GetFuelConsumptionValue(s8, Signals.EngineSpeed);
			int2 = int2 > 0 && !double.IsNaN(int2.Value()) ? int2 : 0.SI<KilogramPerSecond>();
			var s12 = int2;

			_LitresOfAirCompressorOnContinuallyAggregate += M4.GetFlowRate() * stepTimeInSeconds;
			_LitresOfAirCompressorOnOnlyInOverrunAggregate += s10 * stepTimeInSeconds;
			_TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate += s11 * stepTimeInSeconds;
			_TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate += s12 * stepTimeInSeconds;
		}

		public NormLiter LitresOfAirCompressorOnContinually => _LitresOfAirCompressorOnContinuallyAggregate;

		public NormLiter LitresOfAirCompressorOnOnlyInOverrun => _LitresOfAirCompressorOnOnlyInOverrunAggregate;

		public Kilogram TotalCycleFuelConsumptionCompressorOnContinuously => _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate;

		public Kilogram TotalCycleFuelConsumptionCompressorOffContinuously => _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate;

		#endregion
	}
}

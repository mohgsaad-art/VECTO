using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{


	public class M13Impl : AbstractModule, IM13
	{
		protected readonly IM10 M10;
		protected readonly IM11 M11;
		protected readonly IM12 M12;
		protected readonly ISignals Signals;

		private Kilogram _whtcTotalCycleFuelConsumptionGrams;
		private bool _smartPneumatics;
		private bool _smartElectrics;

		public M13Impl(IAuxiliaryConfig auxCfg, IM10 m10, IM11 m11, IM12 m12, ISignals signals)
		{
			M10 = m10;
			M11 = m11;
			M12 = m12;
			Signals = signals;
			_smartElectrics = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart;
			_smartPneumatics = auxCfg.PneumaticUserInputsConfig.SmartAirCompression;
		}

		protected override void DoCalculate()
		{
			var sum1 = M11.TotalCycleFuelConsuptionAverageLoads * M12.StopStartCorrection;
			var sum2 = M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics * M12.StopStartCorrection;
			var sum3 = M10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand * M12.StopStartCorrection;
			var sum4 = -M12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand + sum1;
			var sum5 = sum2 - sum3;
			var sum6 = M12.BaseFuelConsumptionWithTrueAuxiliaryLoads - sum4;
			var sum7 = M12.BaseFuelConsumptionWithTrueAuxiliaryLoads - sum5;
			var sum8 = -sum4 + sum7;
			var sw1 = _smartPneumatics? sum8: sum6;
			var sw2 = _smartPneumatics ? sum3 : M12.BaseFuelConsumptionWithTrueAuxiliaryLoads;
			var sw3 = _smartElectrics? sw1: sw2;
			var sw4 =  Signals.WHTC;
			var sum9 = sw4 * sw3;

			_whtcTotalCycleFuelConsumptionGrams = sum9;
		}

		#region Implementation of IM13

		public Kilogram WHTCTotalCycleFuelConsumption
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _whtcTotalCycleFuelConsumptionGrams; }
		}

		#endregion
	}
}

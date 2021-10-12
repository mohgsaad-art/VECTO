using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M10Impl : AbstractModule, IM10
	{
		protected Kilogram _averageLoadsFuelConsumptionInterpolatedForPneumatics;
		protected Kilogram _fuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand;

		protected readonly IM3_AveragePneumaticLoadDemand M3;

		protected readonly IM9 M9;
			//'Not Currently used but there for ease of refactoring in future.
		
		//'Aggregators
		protected NormLiter _AverageAirConsumedLitre;

		
		public M10Impl(IM3_AveragePneumaticLoadDemand m3, IM9 m9)
		{
			M3 = m3;
			M9 = m9;
			
			_AverageAirConsumedLitre = 0.SI<NormLiter>();
		}

		protected enum InterpolationType
		{
			NonSmartPneumtaics,
			SmartPneumtaics
		}

		protected override void DoCalculate()
		{
			var intrp1 = Interpolate(InterpolationType.NonSmartPneumtaics);
			var intrp2 = Interpolate(InterpolationType.SmartPneumtaics);

			_averageLoadsFuelConsumptionInterpolatedForPneumatics = intrp1;
			_fuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand = intrp2;
		}

		protected virtual Kilogram Interpolate(InterpolationType interpType)
		{
			var returnValue = 0.SI<Kilogram>();
			//' Dim x1,y1,x2,y2,x3,y3, xTA As Single

			var x1 = M9.LitresOfAirCompressorOnContinually;
			var y1 = M9.TotalCycleFuelConsumptionCompressorOnContinuously;
			var x2 = 0.SI<NormLiter>();
			var y2 = M9.TotalCycleFuelConsumptionCompressorOffContinuously;
			var x3 = M9.LitresOfAirCompressorOnOnlyInOverrun;
			var y3 = M9.TotalCycleFuelConsumptionCompressorOffContinuously;
			
			var xTA = _AverageAirConsumedLitre;  //'m3.AverageAirConsumedPerSecondLitre

			switch (interpType) {
				// 'Non-Smart Pneumatics ( OUT 1 )
				case InterpolationType.NonSmartPneumtaics:
					//'returnValue = (y2 + (((y1 - y2) * xTA) / x1))
					returnValue = VectoMath.Interpolate(x1, x2, y1, y2, xTA);
					break;
				// 'Smart Pneumatics ( OUT 2 )
				case InterpolationType.SmartPneumtaics:
					//'returnValue = (y3 + (((y1 - y3) / (x1 - x3)) * (xTA - x3)))
					returnValue = VectoMath.Interpolate(x1, x3, y1, y3, xTA);
					break;
				default: throw new ArgumentOutOfRangeException(nameof(interpType), interpType, null);
			}

			return returnValue;
		}

		#region Implementation of IM10

		public Kilogram AverageLoadsFuelConsumptionInterpolatedForPneumatics
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _averageLoadsFuelConsumptionInterpolatedForPneumatics; }
		}

		public Kilogram FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _fuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand; }
		}

		public void CycleStep(Second stepTimeInSeconds)
		{
			_AverageAirConsumedLitre += double.IsNaN(M3.AverageAirConsumed.Value())
				? 0.SI<NormLiter>()
				: M3.AverageAirConsumed * stepTimeInSeconds;
		}

		#endregion
	}
}

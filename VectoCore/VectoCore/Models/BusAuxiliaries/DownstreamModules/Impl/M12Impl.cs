using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M12Impl : AbstractModule, IM12
	{
		protected readonly IM11 M11;
		protected readonly IM10 M10;

		protected Kilogram _fuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand;
		protected Kilogram _baseFuelConsumptionWithTrueAuxiliaryLoads;
		protected double _stopStartCorrection;

		public M12Impl(IM10 m10, IM11 m11)
		{
			M10 = m10;
			M11 = m11;
		}

		private class Point
		{
			public Joule X;
			public Kilogram Y;
		}

		protected override void DoCalculate()
		{
			var p1 = new Point { X = 0.SI<Joule>(), Y = M11.TotalCycleFuelConsumptionZeroElectricalLoad };
			var p2 = new Point {
				X = M11.SmartElectricalTotalCycleEletricalEnergyGenerated * Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
				Y = M11.TotalCycleFuelConsumptionSmartElectricalLoad
			};

			var ip5X = M11.TotalCycleElectricalDemand;
			var tanTetaP2 = ((p2.Y - p1.Y).Value() / (p2.X - p1.X).Value());
			var ip5yP2 = (p1.Y.Value() + (tanTetaP2 * ip5X.Value())).SI<Kilogram>();

			var interp1 = double.IsNaN(ip5yP2.Value()) ? 0.SI<Kilogram>() : ip5yP2;

			interp1 = !double.IsNaN(interp1.Value()) && M11.StopStartSensitiveTotalCycleElectricalDemand > 0
				? interp1
				: M11.TotalCycleFuelConsumptionZeroElectricalLoad;

			_fuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand = interp1;

			var p3 = new Point {
				X = M11.StopStartSensitiveTotalCycleElectricalDemand,
				Y = M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics
			};

			var tanTetaP3 = (p3.Y - p1.Y).Value() / (p3.X - p1.X).Value();

			var ip5yP3 = p1.Y + (tanTetaP3 * ip5X.Value()).SI<Kilogram>();

			var interp2 = double.IsNaN(ip5yP3.Value()) ? 0.SI<Kilogram>() : ip5yP3;

			interp2 = !double.IsNaN(interp2.Value()) && M11.StopStartSensitiveTotalCycleElectricalDemand > 0
				? interp2
				: M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics;

			_baseFuelConsumptionWithTrueAuxiliaryLoads = interp2;

			var stopStartCorrectionV = _baseFuelConsumptionWithTrueAuxiliaryLoads /
										(M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics > 0
											? M10.AverageLoadsFuelConsumptionInterpolatedForPneumatics
											: 1.SI<Kilogram>());

			_stopStartCorrection = stopStartCorrectionV > 0 ? stopStartCorrectionV.Value() : 1;
		}


		#region Implementation of IM12

		public Kilogram FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _fuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand;
			}
		}

		public Kilogram BaseFuelConsumptionWithTrueAuxiliaryLoads
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _baseFuelConsumptionWithTrueAuxiliaryLoads;
			}
		}

		public double StopStartCorrection
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _stopStartCorrection;
			}
		}

		#endregion
	}
}

using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M02Impl : AbstractModule, IM2_AverageElectricalLoadDemand
	{
		private Volt _powerNetVoltage;

		private IM0_NonSmart_AlternatorsSetEfficiency _module0;
		private double _alternatorPulleyEffiency;
		private Ampere _totalAverageDemandAmpsIncludingBaseLoad;
		private Ampere _totalAverageDemandAmpsEngineOffStandstill;
		private Ampere _totalAverageDemandAmpsEngineOffDriving;
		private ISignals _signals;

		public M02Impl(IM0_NonSmart_AlternatorsSetEfficiency m0, IElectricsUserInputsConfig electricConfig, ISignals signals)
		{
			var altPulleyEfficiency = electricConfig.AlternatorGearEfficiency;
			var powerNetVoltage = electricConfig.PowerNetVoltage;
			if (m0 == null) {
				throw new ArgumentException("Must supply module 0");
			}
			if (altPulleyEfficiency.IsEqual(0) || altPulleyEfficiency > 1) {
				throw new ArgumentException("Alternator Gear efficiency out of range.");
			}
			if (powerNetVoltage < Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMin ||
				powerNetVoltage > Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException("Powernet Voltage out of known range.");
			}

			_signals = signals;

			_powerNetVoltage = powerNetVoltage;
			_totalAverageDemandAmpsIncludingBaseLoad = electricConfig.AverageCurrentDemandInclBaseLoad(false, false);
			_totalAverageDemandAmpsEngineOffStandstill = electricConfig.AverageCurrentDemandInclBaseLoad(true, true);
			_totalAverageDemandAmpsEngineOffDriving = electricConfig.AverageCurrentDemandInclBaseLoad(true, false);
			_module0 = m0;
			_alternatorPulleyEffiency = altPulleyEfficiency;
		}

		#region Implementation of IM2_AverageElectricalLoadDemand

		public Watt GetAveragePowerAtCrankFromElectrics()
		{
			var alternatorsEfficiency = _module0.AlternatorsEfficiency;
			var electricalPowerDemandsWattsDividedByAlternatorEfficiency =
				AveragePowerDemandAtAlternatorFromElectrics * (1 / alternatorsEfficiency);

			var averagePowerDemandAtCrankFromElectricsWatts =
				electricalPowerDemandsWattsDividedByAlternatorEfficiency * (1 / _alternatorPulleyEffiency);

			return averagePowerDemandAtCrankFromElectricsWatts;
		}

		public Watt AveragePowerDemandAtAlternatorFromElectrics
		{
			get {
				var current = _totalAverageDemandAmpsIncludingBaseLoad;
				if (_signals.EngineStopped) {
					current = _signals.VehicleStopped
						? _totalAverageDemandAmpsEngineOffStandstill
						: _totalAverageDemandAmpsEngineOffDriving;
				}
				return _powerNetVoltage * current;
			}
		}

		#endregion
	}
}

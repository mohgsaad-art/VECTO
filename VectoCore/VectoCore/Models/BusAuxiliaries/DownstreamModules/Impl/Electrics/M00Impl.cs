using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M00Impl : AbstractModule, IM0_NonSmart_AlternatorsSetEfficiency
	{
		protected IAlternatorMap _alternatorEfficiencyMap;
		protected Volt _powernetVoltage;
		protected ISignals _signals;
		protected Watt _ElectricalPowerW;

		//private Ampere _totalAverageDemandAmpsIncludingBaseLoad;
		private Ampere _totalDemandAmps;

		private Ampere _totalDemandAmpsEngineOffStandstill;
		private Ampere _totalDemandAmpsEngineOffDriving;

		//private IM0_1_AverageElectricLoadDemand _m0_1;

		public M00Impl(IElectricsUserInputsConfig electricConfig, ISignals signals, Watt electricalPowerHVAC)
		{
			var alternatorEfficiencyMap = electricConfig.AlternatorMap;
			var powernetVoltage = electricConfig.PowerNetVoltage;

			if (alternatorEfficiencyMap == null) {
				throw new ArgumentException("No Alternator Efficiency Map Supplied");
			}

			if (powernetVoltage < Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMin || powernetVoltage > Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException("Powernet Voltage out of range");
			}

			if (signals == null) {
				throw new ArgumentException("No Signals reference was supplied.");
			}

			_alternatorEfficiencyMap = alternatorEfficiencyMap;

			_powernetVoltage = powernetVoltage;

			_signals = signals;

			_ElectricalPowerW = electricalPowerHVAC;
			_totalDemandAmps = electricConfig.AverageCurrentDemandInclBaseLoad(false, false) + GetHVACElectricalCurrentDemand;
			_totalDemandAmpsEngineOffStandstill =
				electricConfig.AverageCurrentDemandInclBaseLoad(true, true) + GetHVACElectricalCurrentDemand;
			_totalDemandAmpsEngineOffDriving =
				electricConfig.AverageCurrentDemandInclBaseLoad(true, false) + GetHVACElectricalCurrentDemand;
		}

		#region Implementation of IM0_NonSmart_AlternatorsSetEfficiency

		public Ampere GetHVACElectricalCurrentDemand => _ElectricalPowerW / _powernetVoltage;

		public double AlternatorsEfficiency
		{
			get {
				var current = _totalDemandAmps;
				if (_signals.EngineStopped) {
					current = _signals.VehicleStopped ? _totalDemandAmpsEngineOffStandstill : _totalDemandAmpsEngineOffDriving;
				}
				return _alternatorEfficiencyMap.GetEfficiency(_signals.EngineSpeed, current);
			}
		}

		#endregion
	}
}

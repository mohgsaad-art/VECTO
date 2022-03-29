using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M05Impl : AbstractModule, IM5_SmartAlternatorSetGeneration
	{
		private IM0_5_SmartAlternatorSetEfficiency _m05;
		private Volt _powerNetVoltage;
		private double _alternatorGearEfficiency;

		public M05Impl(IM0_5_SmartAlternatorSetEfficiency m05, Volt powernetVoltage, double alternatorGearEfficiency)
		{
			//'sanity check
			if (m05 == null) {
				throw new ArgumentException("Please supply a valid module M05");
			}

			if (powernetVoltage < Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMin || powernetVoltage > Constants.BusAuxiliaries.ElectricConstants.PowenetVoltageMax) {
				throw new ArgumentException("Powernet Voltage out of range");
			}
			if (alternatorGearEfficiency < 0 || alternatorGearEfficiency > 1) {
				throw new ArgumentException("AlternatorGearEfficiency Out of bounds, should be 0 to 1");
			}

			_m05 = m05;
			_powerNetVoltage = powernetVoltage;
			_alternatorGearEfficiency = alternatorGearEfficiency;
		}

		#region Implementation of IM5_SmartAlternatorSetGeneration

		public Watt AlternatorsGenerationPowerAtCrankIdle()
		{
			return _m05.SmartIdleCurrent * _powerNetVoltage *
					(1 / (_m05.AlternatorsEfficiencyIdleResultCard * _alternatorGearEfficiency));
		}

		public Watt AlternatorsGenerationPowerAtCrankTractionOn()
		{
			return _m05.SmartTractionCurrent * _powerNetVoltage *
					(1 / (_m05.AlternatorsEfficiencyTractionOnResultCard * _alternatorGearEfficiency));
		}

		public Watt AlternatorsGenerationPowerAtCrankOverrun()
		{
			return _m05.SmartOverrunCurrent * _powerNetVoltage *
					(1 / (_m05.AlternatorsEfficiencyOverrunResultCard * _alternatorGearEfficiency));
		}

		#endregion
	}
}

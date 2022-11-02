using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class M01Impl : AbstractModule, IM1_AverageHVACLoadDemand
	{
		protected IM0_NonSmart_AlternatorsSetEfficiency _m0;
		protected Double _alternatorGearEfficiency;
		protected Double _compressorGearEfficiency;
		
		protected Watt _ElectricalPower;
		protected Watt _MechanicalPower;
		//protected KilogramPerSecond _FuelingLPerH;

		public M01Impl(IM0_NonSmart_AlternatorsSetEfficiency m0, double altGearEfficiency, double compressorGearEfficiency, Watt electricalPowerHVAC , Watt mechanicalPowerHVAC)
		{
			//'Sanity Check - Illegal operations without all params.
			if (m0 == null) {
				throw new ArgumentException("Module0 as supplied is null");
			}

			if (altGearEfficiency < Constants.BusAuxiliaries.ElectricConstants.AlternatorPulleyEfficiencyMin ||
				altGearEfficiency > Constants.BusAuxiliaries.ElectricConstants.AlternatorPulleyEfficiencyMax) {
				throw new ArgumentException(string.Format("Gear efficiency must be between {0} and {1}",
						Constants.BusAuxiliaries.ElectricConstants.AlternatorPulleyEfficiencyMin, Constants.BusAuxiliaries.ElectricConstants.AlternatorPulleyEfficiencyMax));
			}
			
			if (compressorGearEfficiency <= 0 || compressorGearEfficiency > 1) {
				throw new ArgumentException("Compressor Gear efficiency must be between 0 and 1");
			}

			//'Assign
			_m0 = m0;
			_alternatorGearEfficiency = altGearEfficiency;
			
			_compressorGearEfficiency = compressorGearEfficiency;
			
			_ElectricalPower = electricalPowerHVAC;
			_MechanicalPower = mechanicalPowerHVAC;
			//_FuelingLPerH = ssm.FuelPerHBaseAdjusted; // ' SI(Of LiterPerHour)()

		}

		#region Implementation of IM1_AverageHVACLoadDemand

		public Watt AveragePowerDemandAtCrankFromHVACMechanicals => _MechanicalPower * (1 / _compressorGearEfficiency);

		public Watt AveragePowerDemandAtAlternatorFromHVACElectrics => _ElectricalPower;

		public Watt AveragePowerDemandAtCrankFromHVACElectrics => _ElectricalPower * (1 / _m0.AlternatorsEfficiency / _alternatorGearEfficiency);

		//public KilogramPerSecond HVACFueling()
		//{
		//	return _FuelingLPerH;
		//}

		#endregion
	}
}

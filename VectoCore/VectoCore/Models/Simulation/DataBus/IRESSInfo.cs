using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IRESSInfo
	{
		Volt InternalVoltage { get; }

		double StateOfCharge { get; }

		WattSecond StoredEnergy { get; }

		//Ampere MaxCurrent { get; }

		Watt MaxChargePower(Second dt);

		Watt MaxDischargePower(Second dt);

		double MinSoC { get; }

		double MaxSoC { get; }
		AmpereSecond Capacity { get; }
		Volt NominalVoltage { get; }
	}
}
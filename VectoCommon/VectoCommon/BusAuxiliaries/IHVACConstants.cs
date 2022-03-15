using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IHVACConstants
	{
		/// <summary>
		/// 	Diesel: 44800   [J/g]
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		JoulePerKilogramm DieselGCVJperGram { get; }

		/// <summary>
		/// 	835  [g/l]
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		KilogramPerCubicMeter FuelDensity { get; }

		//double FuelDensityAsGramPerLiter { get; }
	}
}

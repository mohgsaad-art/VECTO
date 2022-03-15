using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public interface IFuelConsumptionMap
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="torque"></param>
		/// <param name="angularVelocity"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		KilogramPerSecond GetFuelConsumptionValue(NewtonMeter torque, PerSecond angularVelocity);

	}
}

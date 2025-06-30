using System.Diagnostics;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
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

        FuelConsumptionResult GetFuelConsumption(NewtonMeter currentStateEngineTorque, PerSecond avgEngineSpeed, bool allowExtrapolation = false);
    }

    [DebuggerDisplay("{Value} (extrapolated: {Extrapolated})")]
    public class FuelConsumptionResult
    {
        public KilogramPerSecond Value;
        public bool Extrapolated;
    }
}

using TUGraz.VectoCommon.Utils;

namespace TUGraz.Vecto.UnitTests.Utils;

public class CalculationHelper
{
	public static PerSecond SpeedToAngularSpeed(double v, double r)
	{
		return ((60 * v) / (2 * r * Math.PI / 1000)).RPMtoRad();
	}
}
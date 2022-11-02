using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models {
	public class TorqueConverterOperatingPoint
	{
		public PerSecond OutAngularVelocity;
		public NewtonMeter OutTorque;

		public PerSecond InAngularVelocity;
		public NewtonMeter InTorque;

		public double SpeedRatio;
		public double TorqueRatio;
		public bool Creeping;

		public override string ToString()
		{
			return $"n_out: {OutAngularVelocity}, n_in: {InAngularVelocity}, tq_out: {OutTorque}, tq_in {InTorque}, nu: {SpeedRatio}, my: {TorqueRatio}";
		}
	}
}
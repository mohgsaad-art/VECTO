using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class TorqueConverterCharacteristics
	{
		public TorqueConverterCharacteristics(TorqueConverterEntry entry)
		{
			SpeedRatio = entry.SpeedRatio;
			TorqueRatio = entry.TorqueRatio;
			InputTorqueRef = entry.Torque;
		}

		public double SpeedRatio { get; set; }

		public double TorqueRatio { get; set; }

		public NewtonMeter InputTorqueRef { get; set; }
	}
}
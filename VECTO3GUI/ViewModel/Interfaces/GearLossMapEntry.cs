using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class GearLossMapEntry : ObservableObject
	{
		private NewtonMeter _torqueLoss;
		private PerSecond _inputSpeed;
		private NewtonMeter _inputTorque;

		public GearLossMapEntry(TransmissionLossMap.GearLossMapEntry entry)
		{
			InputSpeed = entry.InputSpeed;
			InputTorque = entry.InputTorque;
			TorqueLoss = entry.TorqueLoss;
		}

		public PerSecond InputSpeed
		{
			get { return _inputSpeed; }
			set { SetProperty(ref _inputSpeed, value); }
		}

		public NewtonMeter InputTorque
		{
			get { return _inputTorque; }
			set { SetProperty( ref _inputTorque, value); }
		}

		public NewtonMeter TorqueLoss
		{
			get { return _torqueLoss; }
			set { SetProperty(ref _torqueLoss, value); }
		}
	}
}
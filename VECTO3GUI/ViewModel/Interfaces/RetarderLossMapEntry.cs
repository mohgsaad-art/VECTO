using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class RetarderLossMapEntry : ObservableObject
	{
		private NewtonMeter _torqueLoss;
		private PerSecond _retarderSpeed;

		public RetarderLossMapEntry(RetarderLossMap.RetarderLossEntry entry)
		{
			RetarderSpeed = entry.RetarderSpeed;
			TorqueLoss = entry.TorqueLoss;
		}

		public PerSecond RetarderSpeed
		{
			get { return _retarderSpeed; }
			set { SetProperty(ref _retarderSpeed, value); }
		}

		public NewtonMeter TorqueLoss
		{
			get { return _torqueLoss; }
			set {SetProperty(ref _torqueLoss, value); }
		}
	}
}
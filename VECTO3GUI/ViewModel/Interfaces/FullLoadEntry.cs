using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class FullLoadEntry : ObservableObject
	{
		private PerSecond _engineSpeed;
		private NewtonMeter _maxTorque;
		private NewtonMeter _dragTorque;

		//public FullLoadEntry(EngineFullLoadCurve.FullLoadCurveEntry entry)
		//{
		//	EngineSpeed = entry.EngineSpeed;
		//	MaxTorque = entry.TorqueFullLoad;
		//	DragTorque = entry.TorqueDrag;
		//}
		public FullLoadEntry(EngineFullLoadCurve entry)
		{
			//EngineSpeed = entry.EngineSpeed;
			//MaxTorque = entry.TorqueFullLoad;
			//DragTorque = entry.TorqueDrag;
		}

		public PerSecond EngineSpeed
		{
			get { return _engineSpeed; }
			set { SetProperty(ref _engineSpeed, value); }
		}

		public NewtonMeter MaxTorque
		{
			get { return _maxTorque; }
			set { SetProperty(ref _maxTorque, value); }
		}

		public NewtonMeter DragTorque
		{
			get { return _dragTorque; }
			set { SetProperty(ref _dragTorque, value); }
		}
	}
}
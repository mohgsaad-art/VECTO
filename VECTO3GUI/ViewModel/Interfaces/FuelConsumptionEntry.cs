using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class FuelConsumptionEntry : ObservableObject
	{
		private PerSecond _engineSpeed;
		private NewtonMeter _torque;
		private KilogramPerSecond _fuelConsumption;

		public FuelConsumptionEntry(FuelConsumptionMap.Entry entry)
		{
			EngineSpeed = entry.EngineSpeed;
			Torque = entry.Torque;
			FuelConsumption = entry.FuelConsumption;
		}

		public PerSecond EngineSpeed
		{
			get { return _engineSpeed; }
			set { SetProperty(ref _engineSpeed, value); }
		}

		public NewtonMeter Torque
		{
			get { return _torque; }
			set { SetProperty(ref _torque, value); }
		}

		public KilogramPerSecond FuelConsumption
		{
			get { return _fuelConsumption; }
			set { SetProperty(ref _fuelConsumption, value); }
		}
	}
}
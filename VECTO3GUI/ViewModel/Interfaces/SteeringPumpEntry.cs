using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class SteeringPumpEntry : ObservableObject
	{
		private string _steeringPumpTechnology;
		private int _steeredAxle;
		
		public SteeringPumpEntry(int idx, string tech)
		{
			_steeredAxle = idx;
			_steeringPumpTechnology = tech;
		}

		public int SteeredAxle
		{
			get { return _steeredAxle; }
			set { SetProperty(ref _steeredAxle, value); }
		}

		public string SteeringPumpTechnology
		{
			get { return _steeringPumpTechnology; }
			set { SetProperty(ref _steeringPumpTechnology, value); }
		}
	}
}
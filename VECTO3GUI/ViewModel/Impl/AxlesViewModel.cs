using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.ViewModel.Interfaces;
using Component = VECTO3GUI.Util.Component;

namespace VECTO3GUI.ViewModel.Impl
{
	public class AxlesViewModel : AbstractViewModel, IAxlesViewModel
	{
		private readonly ObservableCollection<IAxleViewModel> _axles = new ObservableCollection<IAxleViewModel>();

		protected VehicleViewModel vehicleViewModel;
		private IAxleViewModel _currentAxle;
		private int _numSteeredAxles;

		#region Implementation of IAxlesViewModel

		public IList<IAxleDeclarationInputData> ModelData { get {
			return Axles.OrderBy(a => a.AxleIndex).Select(a => a.ModelData).ToArray();
		} }

		public AxleConfiguration AxleConfiguration
		{
			get { return vehicleViewModel != null ? vehicleViewModel.AxleConfiguration : AxleConfiguration.AxleConfig_4x2; }
			set { }
		}

		public IAxleViewModel CurrentAxle
		{
			get { return _currentAxle; }
			set { SetProperty(ref _currentAxle, value); }
		}

		public ObservableCollection<IAxleViewModel> Axles
		{
			get { return _axles; }
		}

		public int NumSteeredAxles
		{
			get { return _numSteeredAxles; }
			protected set { SetProperty(ref _numSteeredAxles, value); }
		}

		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AxleWheels.AxlesEngineering));

			
			vehicleViewModel = ParentViewModel as VehicleViewModel;
			if (vehicleViewModel == null) {
				return;
			}

			vehicleViewModel.PropertyChanged += UpdateAxles;
			DoUpdateSteeredAxles();
			DoUpdateAxles();
		}

		private void UpdateAxles(object sender, PropertyChangedEventArgs e)
		{
			var dependentProperties = new[] { "AxleConfiguration" };
			if (!dependentProperties.Contains(e.PropertyName)) {
				return;
			}
			DoUpdateAxles();
		}

		private void SetValues(IList<IAxleEngineeringInputData> axles)
		{
			throw new NotImplementedException();
		}

		private void SetValues(IList<IAxleDeclarationInputData> axles)
		{
			Axles.Clear();
			foreach (var axle in axles) {
				var entry = Kernel.Get<IAxleViewModel>();
				entry.JobViewModel = JobViewModel;

				var axleEntry = (entry as AxleViewModel);
				if (axleEntry == null) {
					throw new Exception("Unknown Axle ViewModel!");
				}

				axleEntry.SetValues(Axles.Count + 1, axle);
				axleEntry.PropertyChanged += (sender, args) => DoUpdateSteeredAxles();
				Axles.Add(entry);
			}

			if (Axles.Count > 0) {
				CurrentAxle = Axles.First();
			}
			DoUpdateSteeredAxles();
		}

		private void DoUpdateAxles()
		{
			if (!DeclarationMode) {
				return;
			}
			if (vehicleViewModel == null) {
				return;
			}

			var numAxles = vehicleViewModel.AxleConfiguration.NumAxles();
			while (Axles.Count > numAxles) {
				Axles.RemoveAt(Axles.Count - 1);
			}
			while (Axles.Count < numAxles) {
				var entry = Kernel.Get<IAxleViewModel>();
				entry.JobViewModel = JobViewModel;
				var axle = (entry as AxleViewModel);
				if (axle == null) {
					throw new Exception("Unknown Axle ViewModel!");
				}
				axle.SetValues(Axles.Count + 1);
				axle.PropertyChanged += (sender, args) => DoUpdateSteeredAxles();
				Axles.Add(entry);
			}
			DoUpdateSteeredAxles();
			OnPropertyChanged("Axles");
		}

		private void DoUpdateSteeredAxles()
		{
			NumSteeredAxles = _axles.Count(a => a.Steered);
		}
	}

}

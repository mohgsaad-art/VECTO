using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.ViewModel.Interfaces;
using Component = VECTO3GUI.Util.Component;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CyclesViewModel : AbstractViewModel, ICyclesViewModel
	{
		private readonly ObservableCollection<string> _cycles = new ObservableCollection<string>();

		private IVehicleViewModel _vehicleViewModel;

		#region Implementation of ICyclesViewModel

		public ObservableCollection<string> Cycles
		{
			get { return _cycles; }
		}

		#endregion

		protected override void InputDataChanged()
		{
			var model = JobViewModel as DeclarationJobViewModel;
			if (model == null) {
				return;
			}

			_vehicleViewModel = model.GetComponentViewModel(Component.Vehicle) as IVehicleViewModel;
			if (_vehicleViewModel == null) {
				return;
			}

			if (_vehicleViewModel is ObservableObject) {
				(_vehicleViewModel as ObservableObject).PropertyChanged += UpdateDeclarationCycles;
			}
			DoUpdateDeclarationCycles();
		}

		private void UpdateDeclarationCycles(object sender, PropertyChangedEventArgs e)
		{
			var dependentProperties = new[] { "VehicleCategory", "AxleConfiguration", "GrossVehicleMass" };
			if (!dependentProperties.Contains(e.PropertyName)) {
				return;
			}
			DoUpdateDeclarationCycles();
		}

		private void DoUpdateDeclarationCycles()
		{
			if (!DeclarationMode) {
				return;
			}
			if (_vehicleViewModel == null) {
				return;
			}
			

			try {

				//ToDo
				//var seg = DeclarationData.Segments.Lookup(
				//	_vehicleViewModel.VehicleCategory, _vehicleViewModel.AxleConfiguration, _vehicleViewModel.GrossVehicleMass, 0.SI<Kilogram>(), false);
				//_cycles.Clear();
				//foreach (var mission in seg.Missions.Where(m => !m.MissionType.IsEMS())) {
				//	_cycles.Add(mission.MissionType.GetLabel());
				//}
			} catch (Exception) {
				_cycles.Clear();
			}

			OnPropertyChanged("Cycles");
		}
	}
}

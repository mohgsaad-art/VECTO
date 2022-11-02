using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI.Model.TempDataObject;
using VECTO3GUI.ViewModel.Interfaces;
using Component = VECTO3GUI.Util.Component;

namespace VECTO3GUI.ViewModel.Impl
{
	public abstract class AbstractViewModel : ValidatingViewModel, IComponentViewModel
	{
		private IJobEditViewModel _jobViewModel;

		private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();

		protected readonly Dictionary<Component, IComponentViewModel> _subModels = new Dictionary<Component, IComponentViewModel>();

		
		public IJobEditViewModel JobViewModel
		{
			protected get { return _jobViewModel; }
			set {
				_jobViewModel = value;
				if (_jobViewModel is ObservableObject) {
					((ObservableObject)_jobViewModel).PropertyChanged += (sender, args) => {
						if (args.PropertyName == "InputDataProvider") {
							InputDataChanged();
						}
					};
				}
				JobViewModelChanged();
				InputDataChanged();
			}
		}

		protected virtual void InputDataChanged() {}

		protected virtual void JobViewModelChanged() { }

		public virtual bool DeclarationMode { get { return JobViewModel.DeclarationMode; } }

		public ObservableCollection<Component> Components
		{
			get { return _components; }
		}

		public IComponentViewModel ParentViewModel { get; set; }

		public virtual IComponentViewModel GetComponentViewModel(Component component)
		{
			if (_subModels.ContainsKey(component)) {
				return _subModels[component];
			}

			foreach (var entry in _subModels) {
				if (entry.Value.Components.Contains(component)) {
					return entry.Value.GetComponentViewModel(component);	
				}
			}

			return null;
		}

		public virtual bool IsComponentDataChanged()
		{
			return false;
		}

		public virtual void ResetComponentData() {}

		public virtual object CommitComponentData()
		{
			return null;
		}

		public virtual void ShowValidationErrors(Dictionary<string, string> errors)
		{
		}

		public virtual void RemoveValidationErrors(Dictionary<string, string> errors)
		{
		}


		#region Submodule Handling
		protected IEnumerable<Component> GetSubmodels()
		{
			return _subModels.Keys.ToArray();
		}

		protected bool RegisterSubmodel(Component component, IComponentViewModel model)
		{
			if (_subModels.ContainsKey(component)) {
				return false;
			}

			_subModels[component] = model;
			model.ParentViewModel = this;
			model.JobViewModel = JobViewModel;
			model.Components.CollectionChanged += (sender, args) => UpdateSubModules();
			UpdateSubModules();
			return true;
		}

		private void UpdateSubModules()
		{
			_components.Clear();
			foreach (var entry in _subModels) {
				_components.Add(entry.Key);
				foreach (var component in entry.Value.Components) {
					_components.Add(component);
				}
			}
		}

		protected IComponentViewModel UnregisterSubmodel(Component component)
		{
			if (!_subModels.ContainsKey(component)) {
				return null;
			}

			var retVal = _subModels[component];
			_subModels.Remove(component);
			UpdateSubModules();
			return retVal;
		}

		#endregion
	}
}

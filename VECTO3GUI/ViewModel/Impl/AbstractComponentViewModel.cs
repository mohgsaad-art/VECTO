using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ninject;
using VECTO3GUI.ViewModel.Adapter;

namespace VECTO3GUI.ViewModel.Impl
{
	public abstract class AbstractComponentViewModel : AbstractViewModel
	{
		private HashSet<string> _changedInput = new HashSet<string>();
		protected bool UnsavedChanges ;

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		protected void SetChangedProperty(bool changed, [CallerMemberName] string propertyName = "")
		{
			if (!changed) {
				if (_changedInput.Contains(propertyName))
					_changedInput.Remove(propertyName);
			} else {
				if (!_changedInput.Contains(propertyName))
					_changedInput.Add(propertyName);
			}

			UnsavedChanges = _changedInput.Count > 0;
		}

		protected void IsDataChanged<T>(T inputValue, object componentData, [CallerMemberName] string propertyName = null)
		{
			if (propertyName == null)
				return;

			if (componentData != null)
			{
				var currentValue = (T)componentData.GetType().GetProperty(propertyName)?.GetValue(componentData);
				UnsavedChanges = !EqualityComparer<T>.Default.Equals(currentValue, inputValue);
			}

			else
				UnsavedChanges = !EqualityComparer<T>.Default.Equals(inputValue, default(T));

			SetChangedProperty(UnsavedChanges, propertyName);
		}

		protected void ClearChangedProperties()
		{
			UnsavedChanges = false;
			_changedInput.Clear();
		}
		
		public override bool IsComponentDataChanged()
		{
			return UnsavedChanges || _changedInput.Count > 0;
		}
	}
}
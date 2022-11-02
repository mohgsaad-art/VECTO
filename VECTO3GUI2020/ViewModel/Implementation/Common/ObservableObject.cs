using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VECTO3GUI2020.ViewModel.Implementation.Common
{
	public class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Needs to be called when a Property is changed
		/// </summary>
		///
		/// <param name="name">Is automatically set to CallerMemberName</param>
		protected void OnPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			bool propertyChanged = false;

			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				OnPropertyChanged(propertyName);
				propertyChanged = true;
			}
			return propertyChanged;
		}
	}
}
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ninject;

namespace VECTO3GUI.ViewModel.Impl {
	public abstract class ObservableObject : INotifyPropertyChanged
	{

		[Inject] public IKernel Kernel { set; protected get; }

		public event PropertyChangedEventHandler PropertyChanged;


		protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName]string proptertyName = null)
		{
			bool propertyChanged = false;

			if (!EqualityComparer<T>.Default.Equals(field, value)) {
				field = value;
				OnPropertyChanged(proptertyName);
				propertyChanged = true;
			}
			return propertyChanged;
		}

		protected void OnPropertyChanged([CallerMemberName]string propterty = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propterty));
		}
	}
}
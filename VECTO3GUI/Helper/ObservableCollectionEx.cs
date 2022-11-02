using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Castle.Core.Internal;

namespace VECTO3GUI.Helper
{
	public class ObservableCollectionEx<T> : ObservableCollection<T>
	{
		public event PropertyChangedEventHandler CollectionItemChanged;

		public ObservableCollectionEx()
		{
			CollectionChanged += FullObservableCollectionCollectionChanged;
		}

		public ObservableCollectionEx(IEnumerable<T> items) : this()
		{
			if (items == null)
				return;

			foreach (var item in items) {
				Add(item);
			}
		}

		public ObservableCollectionEx(IList<T> items) : this()
		{
			if(items.IsNullOrEmpty())
				return;
			for (int i = 0; i < items.Count; i++) {
				Add(items[i]);
			}
		}


		private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (object item in e.NewItems)
				{
					((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
				}
			}
			if (e.OldItems != null)
			{
				foreach (object item in e.OldItems)
				{
					((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
				}
			}
		}

		private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			CollectionItemChanged?.Invoke(sender, e);

			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
			OnCollectionChanged(args);
		}
	}
}

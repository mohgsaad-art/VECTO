using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace VECTO3GUI2020.Behaviours
{
    public class AutoScrollDataGridBehaviour : Behavior<DataGrid>
    {
		#region Overrides of Behavior

		private INotifyCollectionChanged sourceCollection;
		private DependencyPropertyDescriptor _dpd;

		protected override void OnAttached()
		{
			base.OnAttached();

			//subsrice to Itemssource
			_dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
			if (_dpd != null)
			{
				_dpd.AddValueChanged(this.AssociatedObject, OnItemsSourceChanged);
			}


		}

		private void OnItemsSourceChanged(object sender, EventArgs e)
		{
			UnSubscribeFromSourceCollectionChanged();

			sourceCollection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
			if (sourceCollection != null) {
				sourceCollection.CollectionChanged += SourceCollectionChanged;
			}
		}

		private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var newIndex = e.NewStartingIndex;
			if (newIndex < 0) {
				return;
			}
			Dispatcher.CurrentDispatcher.BeginInvoke(
				DispatcherPriority.ApplicationIdle,
				new Action(() => this.AssociatedObject.ScrollIntoView(this.AssociatedObject.Items[newIndex]))
				);
			
		}


		protected override void OnDetaching()
		{
			base.OnDetaching();
			UnSubscribeFromSourceCollectionChanged();
			if (_dpd != null)
			{
				_dpd.RemoveValueChanged(this.AssociatedObject, OnItemsSourceChanged);
			}
		}

		private void UnSubscribeFromSourceCollectionChanged()
		{
			if (sourceCollection != null) {
				sourceCollection.CollectionChanged -= SourceCollectionChanged;
			}

		}

		#endregion
	}
}

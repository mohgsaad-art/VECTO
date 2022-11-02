using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace VECTO3GUI.Helper
{
	public abstract class DataContextBaseExtension : MarkupExtension
	{
		/// <summary>
		/// Gets or sets the target object from the service provider
		/// </summary>
		protected object TargetObject { get; set; }

		/// <summary>
		/// Gets or sets the target property from the service provider
		/// </summary>
		protected object TargetProperty { get; set; }

		/// <summary>
		/// Gets or sets the target Dependency Object from the service provider
		/// </summary>
		protected DependencyObject TargetObjectDependencyObject { get; set; }

		/// <summary>
		/// Gets or sets the target Dependency Property from the service provider;
		/// </summary>
		protected DependencyProperty TargetObjectDependencyProperty { get; set; }

		/// <summary>
		/// Gets or sets the DataContext that this extension is hooking into.
		/// </summary>
		protected object DataContext { get; private set; }


		/// <summary>
		/// By sealing this method, we guarantee that the developer will always
		/// have to use this implementation.
		/// </summary>
		public sealed override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

			if (target != null) {
				TargetObject = target.TargetObject;
				TargetProperty = target.TargetProperty;

				// Convert to DP and DO if possible...
				TargetObjectDependencyProperty = TargetProperty as DependencyProperty;
				TargetObjectDependencyObject = TargetObject as DependencyObject;

				if (!FindDataContext()) {
					SubscribeToDataContextChangedEvent();
				}
			}

			return OnProvideValue(serviceProvider);
		}


		protected abstract object OnProvideValue(IServiceProvider serviceProvider);

		private void SubscribeToDataContextChangedEvent()
		{
			if (TargetObjectDependencyObject == null) {
				return;
			}
			DependencyPropertyDescriptor.FromProperty(FrameworkElement.DataContextProperty,
													TargetObjectDependencyObject.GetType())
										.AddValueChanged(TargetObjectDependencyObject, DataContextChanged);
		}

		private void DataContextChanged(object sender, EventArgs e)
		{
			if (FindDataContext()) {
				UnsubscribeFromDataContextChangedEvent();
			}
		}

		private bool FindDataContext()
		{
			if (TargetObjectDependencyObject == null) {
				return false;
			}
			var dc = TargetObjectDependencyObject.GetValue(FrameworkElement.DataContextProperty) ??
					TargetObjectDependencyObject.GetValue(FrameworkContentElement.DataContextProperty);
			if (dc != null) {
				DataContext = dc;

				OnDataContextFound();
			}

			return dc != null;
		}

		private void UnsubscribeFromDataContextChangedEvent()
		{
			DependencyPropertyDescriptor.FromProperty(FrameworkElement.DataContextProperty,
													TargetObjectDependencyObject.GetType())
										.RemoveValueChanged(TargetObjectDependencyObject, DataContextChanged);
		}

		protected abstract void OnDataContextFound();
	}
}

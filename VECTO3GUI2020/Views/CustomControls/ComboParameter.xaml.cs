using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using VECTO3GUI2020.Properties;

namespace VECTO3GUI2020.Views.CustomControls
{
	public partial class ComboParameter : UserControl
	{
		ResourceManager _resourceManager;
		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LabelProperty =
			DependencyProperty.Register("Label", typeof(string), typeof(ComboParameter), new PropertyMetadata(""));



		public List<object> ListItems
		{
			get { return (List<object>)GetValue(ListItemsProperty); }
			set { SetValue(ListItemsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ListItems.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ListItemsProperty =
			DependencyProperty.Register("ListItems", typeof(List<object>), typeof(ComboParameter),
				new FrameworkPropertyMetadata(AvailableItemsChanged) { BindsTwoWayByDefault = true });

		public static void AvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var newVal = e.NewValue;
			//Console.WriteLine("Items Changed...");
		}



		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(ComboParameter), new FrameworkPropertyMetadata(AvailableItemsChanged) { BindsTwoWayByDefault = true });



		public new object Content
		{
			get { return (object)GetValue(ContentProperty); }
			set
			{
				SetCurrentValue(ContentProperty, value);
			}
		}

		public static new readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
										typeof(object),
										typeof(ComboParameter),
										new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ContentChanged)));


		private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ComboParameter comboParameter = (ComboParameter)d;
			comboParameter.UpdateLabel(e);
			comboParameter.UpdateComboBox(e);
			

		}

		private void UpdateLabel(DependencyPropertyChangedEventArgs e)
		{

			if ((e.NewValue == e.OldValue))
			{
				return; //Check if this can happen
			}
			Content = e.NewValue;

			var Binding = this.GetBindingExpression(ContentProperty);
			var PropertyName = Binding?.ResolvedSourcePropertyName;
			if (PropertyName == null || Binding == null)
			{
				//Debug.WriteLine("Binding or Property name == null");
				return;
			}
			//Debug.WriteLine("PropertyName: " + PropertyName);

			Label = _resourceManager?.GetString(PropertyName) ?? PropertyName;

		}

		private void UpdateComboBox(DependencyPropertyChangedEventArgs e)
		{

			if ((e.NewValue == e.OldValue))
			{
				return; //Check if this can happen
			}

			var Binding = this.GetBindingExpression(ContentProperty);
			var PropertyName = Binding?.ResolvedSourcePropertyName;
			if (PropertyName == null || Binding == null)
			{
				//Debug.WriteLine("Binding or Property name == null");
				return;
			}

			var data = Binding.ResolvedSource.GetType().GetProperty(PropertyName).GetValue(Binding.DataItem);
			//Console.WriteLine("Selected Item:" + data);


			var items = Enum.GetValues(data.GetType()).Cast<object>().ToList<object>();
			ListItems = items;
			SelectedItem = data;

		}


		public ComboParameter()
		{
			InitializeComponent();
			_resourceManager = Strings.ResourceManager;

		}

	}
}

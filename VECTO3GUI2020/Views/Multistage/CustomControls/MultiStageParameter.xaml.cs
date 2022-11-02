using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Annotations;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Views.CustomControls;

namespace VECTO3GUI2020.Views.Multistage.CustomControls
{

	public enum MultistageParameterViewMode
	{
		TEXTBOX,
		CHECKBOX,
		COMBOBOX
	}


	/// <summary>
	/// Interaction logic for MultiStageParameter.xaml
	/// </summary>
	public partial class MultiStageParameter : UserControl, INotifyPropertyChanged
    {

		#region Dependency Properties


		public static readonly DependencyProperty PreviousContentProperty = DependencyProperty.Register(
			"PreviousContent", typeof(object), typeof(MultiStageParameter), new PropertyMetadata(default(object)));

		public object PreviousContent
		{
			get { return (object)GetValue(PreviousContentProperty); }
			set { SetValue(PreviousContentProperty, value); }
		}


		public static readonly DependencyProperty OptionalProperty = DependencyProperty.Register(
			"Optional", typeof(bool), typeof(MultiStageParameter), new PropertyMetadata(true, new PropertyChangedCallback(OptionalChanged)));

		private static void OptionalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiStageParameter multiStageParameter = (MultiStageParameter)d;
			if ((bool)e.NewValue == false) {
				multiStageParameter.EditingEnabled = true;
				multiStageParameter.ShowCheckBox = false;
			} else {
				multiStageParameter.ShowCheckBox = true;
			}
		}

		public bool Optional
		{
			get { return (bool)GetValue(OptionalProperty); }
			set
			{
				SetValue(OptionalProperty, value);
			}
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(MultistageParameterViewMode), typeof(MultiStageParameter), 
			new PropertyMetadata(MultistageParameterViewMode.TEXTBOX, propertyChangedCallback:ModeChanged));

		private static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var param = (MultiStageParameter)d;
			if(param.GetBindingExpression(e.Property)?.Status != BindingStatus.Active)
			{
				return;
			}
			param.OnModeChanged((MultistageParameterViewMode)e.NewValue);
		}

		public void OnModeChanged(MultistageParameterViewMode mode)
		{
			if (mode != MultistageParameterViewMode.CHECKBOX) {
				checkBox1.SetValue(CheckBox.IsCheckedProperty, DependencyProperty.UnsetValue);
			}



		}

		public MultistageParameterViewMode Mode
		{
			get { return (MultistageParameterViewMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
			"Label", typeof(string), typeof(MultiStageParameter), new PropertyMetadata(default(string)));

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}


		//public static readonly DependencyProperty GeneratedLabelTextProperty = DependencyProperty.Register(
		//	"GeneratedLabelText", typeof(string), typeof(MultiStageParameter), new PropertyMetadata(null));

		//public string GeneratedLabelText
		//{
		//	get { return (string)GetValue(GeneratedLabelTextProperty); }
		//	set { SetValue(GeneratedLabelTextProperty, value); }
		//}

		private string _generatedLabelText;
		public string GeneratedLabelText
		{
			get { return _generatedLabelText; }
			set
			{
				_generatedLabelText = value;
				OnPropertyChanged(nameof(GeneratedLabelText));
			}
		}


		public static readonly DependencyProperty NameLookUpResourceManagerProperty = DependencyProperty.Register(
			"NameLookUpResourceManager", typeof(ResourceManager), typeof(MultiStageParameter), new PropertyMetadata(default(ResourceManager), NameLookUpResourceManagerChanged));

		private static void NameLookUpResourceManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var multistageParameter = (MultiStageParameter)d;
			multistageParameter.SetLabelText();
		}

		public ResourceManager NameLookUpResourceManager
		{
			get { return (ResourceManager)GetValue(NameLookUpResourceManagerProperty); }
			set { SetValue(NameLookUpResourceManagerProperty, value); }
		}


		public static readonly DependencyProperty EditingEnabledProperty = DependencyProperty.Register(
			"EditingEnabled", typeof(bool), typeof(MultiStageParameter), new FrameworkPropertyMetadata(
				false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EditingEnabledChanged)));

		public bool EditingEnabled
		{
			get
			{
				return (bool)GetValue(EditingEnabledProperty);
			}
			set
			{
				SetCurrentValue(EditingEnabledProperty, value);
				//OnPropertyChanged(nameof(EditingEnabled));
			}
		}

		public static readonly DependencyProperty DummyContentProperty = DependencyProperty.Register(
			"DummyContent", typeof(object), typeof(MultiStageParameter),
			new FrameworkPropertyMetadata(null));

		public object DummyContent
		{
			get { return (object)GetValue(DummyContentProperty); }
			set { SetCurrentValue(DummyContentProperty, value); }
		}

		private object _dummyContent = null;
		public object DummyContentGenerated
		{
			get { return _dummyContent;}
			set
			{
				_dummyContent = value;
				OnPropertyChanged(nameof(DummyContentGenerated));
			}
		}


		public static readonly DependencyProperty ShowCheckBoxProperty = DependencyProperty.Register(
			"ShowCheckBox", typeof(bool), typeof(MultiStageParameter), new PropertyMetadata(true));

		public bool ShowCheckBox
		{
			get { return (bool)GetValue(ShowCheckBoxProperty); }
			set { SetValue(ShowCheckBoxProperty, value); }
		}


		// Using a DependencyProperty as the backing store for MyObject.  This enables animation, styling, binding, etc...
		public new static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
				typeof(object),
				typeof(MultiStageParameter),
				new FrameworkPropertyMetadata(defaultValue: null, 
					flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
					propertyChangedCallback: OnContentChanged,
					coerceValueCallback: ContentCoerced)); //TODO Default value null breaks it for SIs default value "" for strings

		private static object ContentCoerced(DependencyObject d, object basevalue)
		{
			var multiStageParameter = ((MultiStageParameter)d);
			var binding = multiStageParameter.GetBindingExpression(ContentProperty);



			if (binding?.Status != BindingStatus.Active)
			{
				return basevalue;
			}


			if (multiStageParameter.resolvedContentProperty == true)
			{
				return basevalue;
			}

			if (multiStageParameter.DummyContent == null)
			{
				multiStageParameter.DummyContent = multiStageParameter.CreateDummyContent();
			}

			if (multiStageParameter.Content != null && multiStageParameter.EditingEnabled == false)
			{
				multiStageParameter.EditingEnabled = true;
			}

			if (multiStageParameter.Mode == MultistageParameterViewMode.COMBOBOX)
			{
				multiStageParameter.GenerateListItemsAndSetComboboxValue();
			}


			if (multiStageParameter.GeneratedLabelText == null)
			{
				multiStageParameter.SetLabelText();
			}

			multiStageParameter.resolvedContentProperty = true;








			return basevalue;
		}

		public new object Content
		{
			get { return (object)GetValue(ContentProperty); }
			set
			{
				SetCurrentValue(ContentProperty, value);
				OnPropertyChanged(nameof(Content));
			}
		}

		public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register(
			"ListItems", typeof(ObservableCollection<Enum>), typeof(MultiStageParameter),
			new FrameworkPropertyMetadata(defaultValue:default(ObservableCollection<Enum>), propertyChangedCallback:ListItemsChanged));

		private static void ListItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			return;
			
			var listItems = e.NewValue as ObservableCollection<Enum>;
			var multistageParameter = (MultiStageParameter)d;
			if (multistageParameter.Mode == MultistageParameterViewMode.COMBOBOX && listItems != null) {
				if (!listItems.Contains(multistageParameter.Content as Enum)) {
					multistageParameter.Content = listItems[0];
				}
			}

			
		}

		public ObservableCollection<Enum> ListItems
		{
			get { return (ObservableCollection<Enum>)GetValue(ListItemsProperty); }
			set { SetValue(ListItemsProperty, value); }
		}
		

		public static readonly DependencyProperty GeneratedListItemsProperty = DependencyProperty.Register(
			"GeneratedListItems", typeof(List<object>), typeof(MultiStageParameter), new PropertyMetadata(defaultValue:null));



		public List<object> GeneratedListItems
		{
			get { return (List<object>)GetValue(GeneratedListItemsProperty); }
			set { SetCurrentValue(GeneratedListItemsProperty, value); }
		}

		private object StoredContent { get; set; } = null;
		

		public bool resolvedContentProperty { get; set; } = false;

		#endregion

		private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MultiStageParameter)d).ContentChanged();
		}

		public void ContentChanged()
		{
			var multiStageParameter = (CustomControls.MultiStageParameter)this;
			var binding = multiStageParameter.GetBindingExpression(ContentProperty);

			if (binding?.Status != BindingStatus.Active) {
				return;
			}

			if (multiStageParameter.Content != null)
			{
				multiStageParameter.EditingEnabled = true;
			}

		}


		public void SetLabelText()
		{
			var binding = this.GetBindingExpression(ContentProperty);
			if (Label == null && binding != null) {
				this.GeneratedLabelText = this.GetLabelByPropertyName(
					MultiStageParameter.ContentProperty,
					this.NameLookUpResourceManager,
					Strings.ResourceManager);
			}
		}

		private void GenerateListItemsAndSetComboboxValue()
		{
			if (Mode != MultistageParameterViewMode.COMBOBOX) 
				return;

			if (GeneratedListItems == null) {
				if (DummyContent is Enum dummyEnum) {
					var enType = dummyEnum.GetType();
					GeneratedListItems = Enum.GetValues(enType).Cast<object>().ToList();


				}else if (Content is Enum contentEnum) {
					var enType = contentEnum.GetType();
					GeneratedListItems = Enum.GetValues(enType).Cast<object>().ToList();

				}
			}
		}

		private object CreateDummyContent()
		{
			var type = this.GetPropertyType(ContentProperty);
			if (type == null) {
				return null;
			}

			try {
				dynamic dynType = type;
				var baseType = dynType.BaseType;
				//Create SI Dummy

				


				if (baseType?.BaseType != null && baseType.BaseType == typeof(SI)) {
					var createMethod = baseType.GetMethod("Create");
					var dummyContent = createMethod?.Invoke(null, new object[] { (new double()) });
					return dummyContent;
				}
				
				
				
				
				if(Mode == MultistageParameterViewMode.COMBOBOX) {
					var bindingProperty = this.GetBindingExpression(ContentProperty);
					var dataItemType = bindingProperty?.DataItem?.GetType();
					var sourcePropertyType =
						dataItemType?.GetProperty(bindingProperty?.ResolvedSourcePropertyName)?.PropertyType;

					var underlyingType = Nullable.GetUnderlyingType(dynType);
					Enum dummyEnum;
					try {
						if (underlyingType != null) {
							dummyEnum = Enum.Parse(underlyingType, underlyingType.GetEnumNames()[0]);
						} else {
							dummyEnum = Enum.Parse(dynType, dynType.GetEnumNames()[0]);
						}
					} catch (Exception ex) {
						Debug.WriteLine(ex.Message);
						return null;
					}
					return dummyEnum;
				}
			
			} catch (Exception ex) {
				Debug.WriteLine(ex.Message);
				return null;
			}

			return null;
		}

		private static void EditingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((MultiStageParameter) d).EditingEnabledChanged((bool)e.NewValue);
		}

		public void EditingEnabledChanged(bool editingEnabled)
		{
			if (GetBindingExpression(EditingEnabledProperty)?.Status != BindingStatus.Active) {
				return;
			}

			this.CoerceValue(ContentProperty);
			this.CoerceValue(ListItemsProperty);
			MultiStageParameter multistageParameter = this;
			if (editingEnabled == false)
			{
				//Temporary store current value
				if (!Validation.GetHasError(multistageParameter.TextBoxContent))
				{
					multistageParameter.StoredContent = multistageParameter.Content;
				}

				if (multistageParameter.Content != null) {
					multistageParameter.Content = null;
				}
				
			}
			else
			{
				if (Content == null) {
					if (multistageParameter.StoredContent != null)
					{
						//Restore previous value
						multistageParameter.Content = multistageParameter.StoredContent;
					}
					else
					{
						if (multistageParameter.Mode == MultistageParameterViewMode.COMBOBOX)
						{
							//Set default value;
							//multistageParameter.GetBindingExpression(ListItemsProperty)?.UpdateTarget();
							//multistageParameter.GetBindingExpression(ContentProperty)?.UpdateTarget();


							/*
							multistageParameter.Content = multistageParameter.ListItems?[0] ??
														multistageParameter.GeneratedListItems?[0];
							//Debug.Assert(false);
							*/
						}
						else
						{
							if (multistageParameter.Content == null && multistageParameter.DummyContent != null)
							{
								multistageParameter.Content = multistageParameter.DummyContent;
							}
						}
					}
				}
			}
		}






		public MultiStageParameter()
        {
			InitializeComponent();

		}

		private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			EditingEnabled = true;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

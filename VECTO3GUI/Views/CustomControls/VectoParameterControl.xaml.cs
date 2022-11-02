using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;


namespace VECTO3GUI.Views.CustomControls
{
	/// <summary>
	/// Interaction logic for VectoParameterControl.xaml
	/// </summary>
	public partial class VectoParameterControl : UserControl, INotifyDataErrorInfo
	{
		
		public VectoParameterControl()
		{
			InitializeComponent();
			Validation.SetErrorTemplate(this, null);
			(Content as FrameworkElement).DataContext = this;
		}


		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value);  }
		}

		//Using a DependencyProperty as the backing store for Caption.This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register("Caption", typeof(string), typeof(VectoParameterControl), new PropertyMetadata(string.Empty));


		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value);  }
		}

		//Using a DependencyProperty as the backing store for Value.This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(string), typeof(VectoParameterControl), new FrameworkPropertyMetadata() {BindsTwoWayByDefault = true});




		public string Unit
		{
			get { return (string)GetValue(UnitProperty); }
			set { SetValue(UnitProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UnitProperty =
			DependencyProperty.Register("Unit", typeof(string), typeof(VectoParameterControl), new PropertyMetadata(""));




		public string ValueAlign
		{
			get { return (string)GetValue(ValueAlignProperty); }
			set { SetValue(ValueAlignProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ValueAlign.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueAlignProperty =
			DependencyProperty.Register("ValueAlign", typeof(string), typeof(VectoParameterControl), new PropertyMetadata("left"));



		public string CaptionWidthGroup
		{
			get { return (string)GetValue(CaptionWidthGroupProperty); }
			set { SetValue(CaptionWidthGroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CaptionWidthGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionWidthGroupProperty =
			DependencyProperty.Register("CaptionWidthGroup", typeof(string), typeof(VectoParameterControl), new PropertyMetadata(""));



		public string UnitWidthGroup
		{
			get { return (string)GetValue(UnitWidthGroupProperty); }
			set { SetValue(UnitWidthGroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for UnitWidthGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UnitWidthGroupProperty =
			DependencyProperty.Register("UnitWidthGroup", typeof(string), typeof(VectoParameterControl), new PropertyMetadata(""));




		public delegate void ErrorsChangedEventHandler(object sender, DataErrorsChangedEventArgs e);

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public bool HasErrors
		{
			get {
				var validationErrors = Validation.GetErrors(this);
				return validationErrors.Any();
			}
		}

		public IEnumerable GetErrors(string propertyName)
		{
			var validationErrors = Validation.GetErrors(this);
			var specificValidationErrors =
				validationErrors.Where(
					error => ((BindingExpression)error.BindingInError).TargetProperty.Name == propertyName).ToList();
			var specificValidationErrorMessages = specificValidationErrors.Select(valError => valError.ErrorContent);
			return specificValidationErrorMessages;
		}

		public void NotifyErrorsChanged(string propertyName)
		{
			if (ErrorsChanged != null) {
				ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}

		protected static void ValidatePropertyWhenChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((VectoParameterControl)dependencyObject).NotifyErrorsChanged(dependencyPropertyChangedEventArgs.Property.Name);
		}

	}
}

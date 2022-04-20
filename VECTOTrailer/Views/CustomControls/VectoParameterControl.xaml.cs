using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.Helper;


namespace VECTOTrailer.Views.CustomControls
{
	/// <summary>
	/// Interaction logic for VectoParameterControl.xaml
	/// </summary>
	public partial class VectoParameterControl : UserControl//, INotifyDataErrorInfo
    {
		private RoutedEventHandler _routedEventHandler;
		public VectoParameterControl()
		{
			InitializeComponent();
			//Validation.SetErrorTemplate(this, null);
			if (DataContext is ViewModelBase vmBase) {
				vmBase.ErrorsChanged += (sender, args) => NotifyErrorsChanged("Value");
			}

			Loaded += (sender, args) => {
				_routedEventHandler = new RoutedEventHandler(RoutedEventHandler);
				AddHandler(Validation.ErrorEvent, _routedEventHandler);
			};
			Unloaded += (sender, args) => {
				if (_routedEventHandler != null) {
					RemoveHandler(Validation.ErrorEvent, _routedEventHandler);
				}
			};

			
		}



        private void RoutedEventHandler(object sender, RoutedEventArgs e)
		{
			switch (e) {
				case ValidationErrorEventArgs valError:
					ValidationErrorEventHandler(sender, valError);
					break;
				default:
					break;
			}
		}

		private void ValidationErrorEventHandler(object sender, ValidationErrorEventArgs valError)
		{

			
			if (DataContext is ViewModelBase viewModelBase)
			{

				var propertyName = "unknown";
				if (valError.Error.BindingInError is BindingExpression bindingEx) {
					propertyName = bindingEx.ResolvedSourcePropertyName;
					
				}
				else
				{
					throw new Exception("Multibinding not implemented");
				}

				if (valError.Action == ValidationErrorEventAction.Added)
				{
					viewModelBase.AddPropertyError(propertyName, valError.Error.ErrorContent.ToString());

				}
				else
				{
					viewModelBase.RemovePropertyError(propertyName, valError.Error.ErrorContent.ToString());
				}
			}

		}


		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value);  }
		}

		//Using a DependencyProperty as the backing store for Caption.This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register("Caption", typeof(string), typeof(VectoParameterControl), new PropertyMetadata(string.Empty));


		public object Value
		{
			get
			{
				return (object)GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		//Using a DependencyProperty as the backing store for Value.This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), 
				typeof(VectoParameterControl),
				new FrameworkPropertyMetadata() {
					BindsTwoWayByDefault = true,
					PropertyChangedCallback = PropertyChangedCallback
				});

		private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			
		}


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
			DependencyProperty.Register("CaptionWidthGroup", typeof(string), typeof(VectoParameterControl), new PropertyMetadata("CustomControlcaptionWidth"));



		public string UnitWidthGroup
		{
			get { return (string)GetValue(UnitWidthGroupProperty); }
			set { SetValue(UnitWidthGroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for UnitWidthGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UnitWidthGroupProperty =
			DependencyProperty.Register("UnitWidthGroup", typeof(string), typeof(VectoParameterControl), new PropertyMetadata("CustomControlUnitWidth"));




		public delegate void ErrorsChangedEventHandler(object sender, DataErrorsChangedEventArgs e);

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public bool HasErrors
		{
			get {
				if (DataContext is ViewModelBase vmBase) {
					return vmBase.HasErrors;
				}

				var validationErrors = Validation.GetErrors(this);
				var hasErrors = validationErrors.Any();
				return hasErrors;
			}
		}

		public IEnumerable GetErrors(string propertyName)
		{
			if (DataContext is ViewModelBase vmBase) {
				return vmBase.GetErrors(propertyName);
			}
			var validationErrors = Validation.GetErrors(this);
			var specificValidationErrors =
				validationErrors.Where(
					error => ((BindingExpression)error.BindingInError).TargetProperty.Name == propertyName).ToList();
			var specificValidationErrorMessages = specificValidationErrors.Select(valError => valError.ErrorContent);
			return specificValidationErrorMessages;
		}
		

		public void NotifyErrorsChanged(string propertyName)
		{
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		protected static void ValidatePropertyWhenChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var errors = Validation.GetErrors(dependencyObject).Where(error => !(error.RuleInError.GetType() == typeof(NotifyDataErrorValidationRule))).ToList();
			((VectoParameterControl)dependencyObject).UpdateErrorsInViewModel(errors, dependencyPropertyChangedEventArgs);
			
			((VectoParameterControl)dependencyObject).NotifyErrorsChanged(dependencyPropertyChangedEventArgs.Property.Name);

		}

		private void UpdateErrorsInViewModel(IList<ValidationError> errors, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var bindingEx = BindingOperations.GetBindingExpression(this, dependencyPropertyChangedEventArgs.Property);
			if (DataContext is ViewModelBase viewModelBase)
            {
                if (errors.Any())
                {
                    viewModelBase.AddPropertyError(bindingEx.ResolvedSourcePropertyName, string.Join("\n", errors.Where(error => ((BindingExpression)error.BindingInError).ResolvedSourcePropertyName == bindingEx.ResolvedSourcePropertyName).Select(valError => valError.ErrorContent)));
                }
                else
                {
                    viewModelBase.RemovePropertyError(bindingEx.ResolvedSourcePropertyName);
                }
            }
        }
    }
}

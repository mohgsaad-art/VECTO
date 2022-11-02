using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VECTO3GUI.Views.CustomControls
{
	/// <summary>
	/// Interaction logic for OtherVectoParameterControl.xaml
	/// </summary>
	public partial class IntegerVectoParameterControl : UserControl, IDataErrorInfo
		
		
		//, INotifyDataErrorInfo
	{
		public IntegerVectoParameterControl()
		{
			InitializeComponent();
			(Content as FrameworkElement).DataContext = this;

			

		}


		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		public static explicit operator IntegerVectoParameterControl(BindingExpression v)
		{
			throw new NotImplementedException();
		}

		//Using a DependencyProperty as the backing store for Caption.This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register(nameof(Caption), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata(string.Empty));


		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		//Using a DependencyProperty as the backing store for Value.This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register(nameof(Value), typeof(string), typeof(IntegerVectoParameterControl), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });


		public string Unit
		{
			get { return (string)GetValue(UnitProperty); }
			set { SetValue(UnitProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UnitProperty =
			DependencyProperty.Register(nameof(Unit), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata(""));




		public string ValueAlign
		{
			get { return (string)GetValue(ValueAlignProperty); }
			set { SetValue(ValueAlignProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ValueAlign.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueAlignProperty =
			DependencyProperty.Register(nameof(ValueAlign), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata("left"));



		public string CaptionWidthGroup
		{
			get { return (string)GetValue(CaptionWidthGroupProperty); }
			set { SetValue(CaptionWidthGroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CaptionWidthGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionWidthGroupProperty =
			DependencyProperty.Register(nameof(CaptionWidthGroup), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata(""));


		public string UnitWidthGroup
		{
			get { return (string)GetValue(UnitWidthGroupProperty); }
			set { SetValue(UnitWidthGroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for UnitWidthGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UnitWidthGroupProperty =
			DependencyProperty.Register(nameof(UnitWidthGroup), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata(""));


		public string MinValue
		{
			get { return (string)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value);}
		}

		public static readonly DependencyProperty MinValueProperty =
			DependencyProperty.Register(nameof(MinValue), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata($"{int.MinValue}"));


		public string MaxValue
		{
			get { return (string)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}

		public static readonly DependencyProperty MaxValueProperty =
			DependencyProperty.Register(nameof(MaxValue), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata($"{int.MaxValue}"));

		
		public string ValidateInput
		{
			get { return (string)GetValue(ValidateInputProperty); }
			set { SetValue(ValidateInputProperty, value); }
		}

		public static readonly DependencyProperty ValidateInputProperty =
			DependencyProperty.Register(nameof(ValidateInput), typeof(string), typeof(IntegerVectoParameterControl), new PropertyMetadata("False"));





		//public IEnumerable GetErrors(string propertyName)
		//{
		//	var validationErrors = Validation.GetErrors(this);
		//	var specificValidationErrors =
		//		validationErrors.Where(
		//			error => ((BindingExpression)error.BindingInError).TargetProperty.Name == propertyName).ToList();
		//	var specificValidationErrorMessages = specificValidationErrors.Select(valError => valError.ErrorContent);
		//	return specificValidationErrorMessages;
		//}

		//public bool HasErrors
		//{
		//	get
		//	{
		//		var validationErrors = Validation.GetErrors(this);
		//		return validationErrors.Any();
		//	}
		//}

		//public void NotifyErrorsChanged(string propertyName)
		//{
		//	if (ErrorsChanged != null)
		//	{
		//		ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
		//	}
		//}


		//public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
		public string this[string columnName]
		{
			get { return $"blabla{columnName}"; }
		}

		public string Error { get; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VECTO3GUI.Helper.Validation
{
	public class DoubleValidatorConfig : Freezable
	{
		private DoubleValidator Validator { get; set; }

		public int MinValue
		{
			get { return (int)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
			nameof(MinValue), typeof(int), typeof(DoubleValidatorConfig), new FrameworkPropertyMetadata(MinPropertyChangedCallback));
		
		private static void MinPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var doubleValidator = (DoubleValidatorConfig)d;
			if (doubleValidator.Validator != null)
				doubleValidator.Validator.MinValue = (int)e.NewValue;
		}


		public int MaxValue
		{
			get { return (int)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
			nameof(MaxValue), typeof(int), typeof(DoubleValidatorConfig), new FrameworkPropertyMetadata(MaxPropertyChangedCallback));

		private static void MaxPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var doubleValidator = (DoubleValidatorConfig)d;
			if (doubleValidator.Validator != null)
				doubleValidator.Validator.MaxValue = (int)e.NewValue;
		}

		public int Decimals
		{
			get { return (int)GetValue(DecimalsProperty); }
			set { SetValue(DecimalsProperty, value); }
		}
		public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
			nameof(Decimals), typeof(int), typeof(DoubleValidatorConfig), new FrameworkPropertyMetadata(DecimalsPropertyChangedCallback));

		private static void DecimalsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var doubleValidator = (DoubleValidatorConfig)d;
			if (doubleValidator.Validator != null)
				doubleValidator.Validator.Decimals = (int)e.NewValue;
		}
		
		public void SetValidator(DoubleValidator validator)
		{
			Validator = validator;
			if (validator != null)
			{
				validator.MaxValue = MaxValue;
				validator.MinValue = MinValue;
				validator.Decimals = Decimals;
			}
		}

		protected override Freezable CreateInstanceCore()
		{
			return new DoubleValidatorConfig();
		}
	}
}

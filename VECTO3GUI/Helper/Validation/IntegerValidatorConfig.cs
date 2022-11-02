using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI.Helper.Validation
{
	public class IntegerValidatorConfig : Freezable
	{
		public int MinValue
		{
			get { return (int)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}

		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
			nameof(MinValue), typeof(int), typeof(IntegerValidatorConfig), new FrameworkPropertyMetadata(MinPropertyChangedCallback));


		private static void MinPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var integerValidator = (IntegerValidatorConfig)d;
			if (integerValidator.Validator != null)
				integerValidator.Validator.MinValue = (int)e.NewValue;
		}

		public int MaxValue
		{
			get { return (int)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}

		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
			nameof(MaxValue), typeof(int), typeof(IntegerValidatorConfig), new FrameworkPropertyMetadata(MaxPropertyChangedCallback));

		private static void MaxPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var integerValidator = (IntegerValidatorConfig)d;
			if (integerValidator.Validator != null)
				integerValidator.Validator.MaxValue = (int)e.NewValue;
		}

		private IntegerValidator Validator { get; set; }

		public void SetValidator(IntegerValidator validator)
		{
			Validator = validator;
			if (validator != null) {
				validator.MaxValue = MaxValue;
				validator.MinValue = MinValue;
			}
		}
		
		protected override Freezable CreateInstanceCore()
		{
			return new IntegerValidatorConfig();
		}
	}
}

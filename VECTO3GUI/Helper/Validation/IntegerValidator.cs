using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace VECTO3GUI.Helper.Validation
{
	public class IntegerValidator : ValidationRule
	{
		private IntegerValidatorConfig _integerValidator;

		public int MinValue { get; set; }
		public int MaxValue { get; set; }
		
		public IntegerValidator() {}

		public IntegerValidatorConfig ValidatorConfig
		{
			get { return _integerValidator; }
			set
			{
				_integerValidator = value;
				value?.SetValidator(this);
			}
		}

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var strValue = value as string;
			if (strValue != null) {
				int number;
				if (!int.TryParse(strValue, out number))
					return new ValidationResult(false, "Not a valid integer value!");


				if (number < MinValue)
					return new ValidationResult(false, $"Only integer values greater than or equals to {MinValue} are allowed!");

				if (MaxValue > MinValue) {
					if (number > MaxValue)
						return new ValidationResult(false, $"Only integer values less than or equals to {MaxValue} are allowed!");
				}
			}
			return ValidationResult.ValidResult;
		}
	}
}

using System;
using System.Globalization;
using System.Windows.Controls;

namespace VECTO3GUI2020.Helper
{
	public class DoubleValidation : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			try {
				Double.Parse(value as string);
			} catch (Exception e) {
				return new ValidationResult(false, "Not a number");
			}

			return ValidationResult.ValidResult;
		}
	}
}
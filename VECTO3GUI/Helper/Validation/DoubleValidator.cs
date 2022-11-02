using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VECTO3GUI.Helper.Validation
{
	public class DoubleValidator :  ValidationRule
	{
		private DoubleValidatorConfig _doubleValidator;

		public int MinValue { get; set; }
		public int MaxValue { get; set; }
		public int Decimals { get; set; }

		public DoubleValidator() { }

		public DoubleValidatorConfig ValidatorConfig
		{
			get { return _doubleValidator; }
			set
			{
				_doubleValidator = value;
				value?.SetValidator(this);
			}
		}
		
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var strValue = value as string;
			if (strValue != null) {
				double number;
				if(!double.TryParse(strValue, NumberStyles.Float, cultureInfo, out number))
					return new ValidationResult(false, "Not a valid number!");

				if (number < MinValue) {
					return new ValidationResult(false, $"Only values greater than or equals to {MinValue} are allowed!");
				}

				if (MaxValue > MinValue)
				{
					if (number > MaxValue)
						return new ValidationResult(false, $"Only values less than or equals to {MaxValue} are allowed!");
				}
			}

			return ValidationResult.ValidResult;

		}
	}
}

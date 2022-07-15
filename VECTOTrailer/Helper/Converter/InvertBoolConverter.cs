using System;
using System.Globalization;
using System.Windows.Data;

namespace VECTOTrailer.Helper.Converter
{
	public class InvertBoolConverter :  BaseConverter, IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool booleanVal) {
				return !booleanVal;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
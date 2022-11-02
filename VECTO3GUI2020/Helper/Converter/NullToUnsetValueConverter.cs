using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
	public class NullToUnsetValueConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return DependencyProperty.UnsetValue;
			} else {
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
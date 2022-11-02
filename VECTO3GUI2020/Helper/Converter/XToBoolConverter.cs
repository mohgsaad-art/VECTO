using System;
using System.Globalization;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
	/// <summary>
	/// Converts bool to bool and object to null; used to avoid errors in collapsed checkboxes;
	/// </summary>
	public class XToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool? b = value as bool?;


			return b;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
			
		}
	}
}
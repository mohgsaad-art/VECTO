using System;
using System.Globalization;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{

	public class BoolToIntConverter : IValueConverter
	{
		#region Implementation of IValueConverter


		/// <summary>
		/// Returns -1 if value is false, 0 if value is true
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value == false ? -1 : 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}

		#endregion
	}
}
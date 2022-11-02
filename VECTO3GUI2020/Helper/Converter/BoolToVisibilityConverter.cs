using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
	public class InvertedBoolToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts boolean to Visibility
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter">if set to true the result is inverted</param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			
			

			if (value is bool b) {
				if (parameter != null) {
					if (string.Equals((string)parameter, "invert", StringComparison.InvariantCultureIgnoreCase)) {
						b = !b;
					}
				}

				var visibility = b ? Visibility.Collapsed : Visibility.Visible;
				return visibility;
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException();
		}
	}
}
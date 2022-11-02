using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
    class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">set to "invert" to invert the result</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var invert = false;
			if (parameter != null) {
				invert = string.Equals("invert", (string) parameter, StringComparison.InvariantCultureIgnoreCase);
            }
			
            if (value == null)
            {
				if (invert) {
					return Visibility.Visible;
				}
                return Visibility.Collapsed;
            }
            else {
				if (invert) {
					return Visibility.Collapsed;
				}
				return Visibility.Visible;
			}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

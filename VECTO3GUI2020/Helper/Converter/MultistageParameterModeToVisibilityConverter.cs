using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VECTO3GUI2020.Views.Multistage.CustomControls;

namespace VECTO3GUI2020.Helper.Converter
{
	public class MultistageParameterModeToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var selectedMode = (MultistageParameterViewMode)value;
			if (parameter is MultistageParameterViewMode mode) {
				if (selectedMode == mode) {
					return Visibility.Visible;
                } else {
					return Visibility.Collapsed;
				}
            } else {
				throw new ArgumentException("Provide a  MultiStageParameter.MODE as parameter!");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
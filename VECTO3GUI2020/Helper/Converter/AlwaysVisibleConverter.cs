using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
    //https://stackoverflow.com/questions/9893825/mvvm-hiding-a-control-when-bound-property-is-not-present
    public class AlwaysVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

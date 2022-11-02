using System;
using System.Globalization;
using System.Windows.Data;


namespace VECTO3GUI2020.Helper.Converter
{
    class LabledTextBoxLabelConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

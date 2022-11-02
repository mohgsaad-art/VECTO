using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI2020.Helper.Converter
{
    class EnumConverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return Binding.DoNothing;
			}

			Type valueType = value.GetType();
			if (!valueType.IsEnum) {
				return value;
			}

			var attributes =
				valueType.GetField(value.ToString())?.GetCustomAttributes( typeof(GuiLabelAttribute),false);

			if (!(attributes?.First() is GuiLabelAttribute attribute)) {
				return value;
			} else {
				return attribute.Label;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}

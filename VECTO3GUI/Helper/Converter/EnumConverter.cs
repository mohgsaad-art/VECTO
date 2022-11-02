using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.Helper.Converter
{
	public class EnumConverter : BaseConverter , IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is AirdragConfig) {
				return ((AirdragConfig)value).GetLabel();
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

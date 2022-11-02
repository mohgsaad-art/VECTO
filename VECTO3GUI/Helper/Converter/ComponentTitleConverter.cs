using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using VECTO3GUI.Util;

namespace VECTO3GUI.Helper.Converter
{
	public class ComponentTitleConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Component) {
				var component = (Component)value;
				return Regex.Replace(component.GetLabel(), "(\\B[A-Z])", " $1"); ;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

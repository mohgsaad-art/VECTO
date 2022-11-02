using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VECTO3GUI.Util;

namespace VECTO3GUI.Helper.Converter
{
	public class SaveButtonLabelConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var param = parameter as string;
			if (param == null)
				return value;


			if (value is Component)
			{
				var component = (Component)value;

				if (param == "Save") 
					return $"{param} {component.GetLabel()}";
				if (param == "Commit")
					return $"{param} {component.GetLabel()} changes";
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

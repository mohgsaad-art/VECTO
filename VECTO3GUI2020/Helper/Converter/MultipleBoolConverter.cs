using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
	public class MultipleBoolConverter : IMultiValueConverter
	{
		#region Implementation of IMultiValueConverter

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var boolVals = values.Cast<bool>();
			bool returnValue = (bool)values[0];
			foreach (var val in boolVals) {
				returnValue = returnValue && val;
				if (!returnValue) {
					break;
				}
			}

			return returnValue;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
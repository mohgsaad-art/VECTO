using System;
using System.Globalization;
using System.Windows.Data;

namespace VECTOTrailer.Helper.Converter
{
	public class BoolANDMultiValConverter : BaseConverter, IMultiValueConverter
	{
		#region Implementation of IMultiValueConverter

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			foreach (var value in values) {
				if (value is bool booleanValue) {
					if (!booleanValue) {
						return false;
					}
				} else {
					return false;
				}
			}
			return true;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
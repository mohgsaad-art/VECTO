using System;
using System.Globalization;
using System.Windows.Data;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace VECTO3GUI.Helper.Converter
{
	public class VehicleClassConverter : BaseConverter, IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var vehicleClass = value as VehicleClass?;
			return vehicleClass == null ? string.Empty : vehicleClass.Value.GetClassNumber();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return null;
			}

			try {
				return VehicleClassHelper.Parse(value.ToString());
			} catch (Exception) {
				return null;
			}
		}

		#endregion
	}
}

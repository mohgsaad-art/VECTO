using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;

using Expression = System.Linq.Expressions.Expression;
using SIUtils = VECTO3GUI.Util.SIUtils;

namespace VECTO3GUI.Helper.Converter
{
	public class SIValueConverter : BaseConverter, IValueConverter
	{
		protected const double ToRpm = 60 / (2 * Math.PI);
		protected const double ToCubicCentimeter = 1e6;

		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return DependencyProperty.UnsetValue;
			}
			if (!(value is SI)) {
				return DependencyProperty.UnsetValue;
				//throw new Exception("Can only convert SI types!");
			}

			var siValue = value as SI;
			var doubleVal = siValue.Value();

			var conversionFactor = 1.0;
			var stringParam = parameter as string;
			int? decimals = null;
			if (!string.IsNullOrEmpty(stringParam)) {
				var args = stringParam.Split('|');
				foreach (var arg in args) {
					GetDecimals(arg, ref decimals);
					GetConversionFactor(ref conversionFactor, arg);
				}
			}
			doubleVal *= conversionFactor;
			return decimals == null ? doubleVal.ToString(culture) : doubleVal.ToString("F" + decimals.Value, culture);
		}

		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!typeof(SI).IsAssignableFrom(targetType)) {
				return DependencyProperty.UnsetValue;

				//throw new Exception("Can only convert SI types!");
			}

			if(value == null) {
				return DependencyProperty.UnsetValue;
			}

			var stringParam = parameter as string;
			int? decimals = null;
			var conversionFactor = 1.0;
			if (!string.IsNullOrEmpty(stringParam)) {
				var args = stringParam.Split('|');
				foreach (var arg in args) {
					GetDecimals(arg, ref decimals);
					GetConversionFactor(ref conversionFactor, arg);
				}
			}

			double doubleVal;
			var success = double.TryParse(value.ToString(), NumberStyles.Float, culture, out doubleVal);
			if (!success) {
				return DependencyProperty.UnsetValue;
			}

			if (decimals != null) {
				doubleVal = Math.Round(doubleVal, decimals.Value, MidpointRounding.AwayFromZero);
			}
			doubleVal /= conversionFactor;

			return SIUtils.CreateSIValue(targetType, doubleVal);
		}

		private void GetConversionFactor(ref double factor, string convertId)
		{
			switch (convertId.ToLower()) {
				case "asrpm": factor = ToRpm;
					break;
				case "ascubiccentimeter": factor = ToCubicCentimeter;
					break;
			}
		}

		private void GetDecimals(string arg, ref int? decimals)
		{
			switch (arg.ToLower()) {
				case "int":
					decimals = 0;
					break;
				case "double2":
					decimals = 2;
					break;
				case "double3":
					decimals = 3;
					break;
				case "double4":
					decimals = 4;
					break;
			}
		}
		#endregion
	}
}

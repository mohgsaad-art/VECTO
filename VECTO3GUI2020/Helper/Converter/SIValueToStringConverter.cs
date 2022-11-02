using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Util;
using SIUtils = VECTO3GUI2020.Util.SIUtils;

namespace VECTO3GUI2020.Helper.Converter
{
    class SIValueToStringConverter : IValueConverter
	{
		private SI _si;
		private ConvertedSI _convertedSI;
		private Type _sourceType;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return value;
			}


            if(value is SI SIvalue) {
				_si = SIvalue;
				return SIvalue.ToGUIFormat();
			}

			if (value is ConvertedSI convertedSI) {
				_convertedSI = convertedSI;
				return convertedSI.ToOutputFormat(showUnit: false, decimals:0);
			}

			return value?.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
			try {
				if (_si != null || _convertedSI != null) {
					var hackedString = value as string;
					hackedString = hackedString.Replace(",", ".");
					var doubleVal = Double.Parse(hackedString, CultureInfo.InvariantCulture);
					if (_convertedSI != null)
					{
						return new ConvertedSI(doubleVal, _convertedSI.Units);
					}
					if (_si != null)
					{
						var newSi = SIUtils.CreateSIValue(_si.GetType(), doubleVal);
						return newSi;
					}

				}
			}
			catch (Exception e) {
				return value;
			}

			return value;
		}
    }
}

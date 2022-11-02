using System;
using System.Globalization;
using System.Windows.Data;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI2020.Helper.Converter
{
    class SIToUnitString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var unitString = getUnitString(value);
			if (unitString != null) {
				unitString = unitString.Replace("^2", ((char)0xB2).ToString());
				unitString = unitString.Replace("^3", ((char)0xB3).ToString());
				return unitString;
			} else {
				return Binding.DoNothing;
			}

			//if (value == null) {
			//	return Binding.DoNothing;
			//}


			//         if(value is SI SIValue)
			//         {
			//             if (SIValue.UnitString == "1/s")
			//             {
			//                 return "rpm";
			//             }
			//             return SIValue.UnitString;
			//         }

			//if (value is ConvertedSI convertedSI) {
			//	return convertedSI.Units;
			//}

			//return Binding.DoNothing;

		}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

		private string getUnitString(object value)
		{
			if (value == null) {
				return null;
			}


			if (value is SI SIValue)
			{
				if (SIValue.UnitString == "1/s")
				{
					return "rpm";
				}
				return SIValue.UnitString;
			}

			if (value is ConvertedSI convertedSI)
			{
				return convertedSI.Units;
			}

			return null;
		}
    }
}

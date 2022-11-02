using System;
using System.Globalization;
using System.Windows.Data;

namespace VECTO3GUI2020.Helper.Converter
{
    class JobTypeStringConverter : IValueConverter
    {
        /*
        public string JobTypeToString(JobType jobtype)
        {
            switch (jobtype)
            {
                case JobType.CompletedBus:
                    return "Completed Bus";
                case JobType.HeavyLorry:
                    return "Heavy Lorry";
                case JobType.PrimaryBus:
                    return "Primary Bus";
                case JobType.Unknown:
                    return "Unknown";
                default:
                    return jobtype.ToString();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is JobType))
            {
                return Binding.DoNothing;
            }
            JobType jobtype = (JobType)value;
            return JobTypeToString(jobtype);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

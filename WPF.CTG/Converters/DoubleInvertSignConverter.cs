using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF.CTG.Converters
{
    public class DoubleInvertSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new NullReferenceException();

            return (Double) value * -1;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new NullReferenceException();

            return (Double) value * -1;
        }
    }
}

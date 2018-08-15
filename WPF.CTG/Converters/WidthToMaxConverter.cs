using System;
using System.Windows;
using System.Windows.Data;

namespace WPF.CTG.Converters
{
    public class WidthToMaxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            var widthWithScaling = (values[0] != DependencyProperty.UnsetValue && 
                                    values[1] != DependencyProperty.UnsetValue) ? (double)values[0] : 0.0;
            var coordinateViewPortActualWidth = (values[0] != DependencyProperty.UnsetValue && 
                                                 values[1] != DependencyProperty.UnsetValue) ? (double)values[1] : 0.0;
            return widthWithScaling - (coordinateViewPortActualWidth - 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

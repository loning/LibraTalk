using System;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.UI.Xaml.Converters
{
    public class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            /*if (typeof (bool) != targetType)
            {
                throw new InvalidCastException();
            }*/

            return false == (bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (typeof(bool) != targetType)
            {
                throw new InvalidCastException();
            }

            return false == (bool)value;
        }
    }
}
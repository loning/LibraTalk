using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.UI.Xaml.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (typeof (Visibility) != targetType)
            {
                throw new InvalidCastException();
            }

            var flag = (bool) value;

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Visibility.Visible == (Visibility) value;
        }
    }
}
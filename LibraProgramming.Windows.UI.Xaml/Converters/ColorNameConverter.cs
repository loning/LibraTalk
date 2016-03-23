using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.UI.Xaml.Converters
{
    public class ColorNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (typeof(string)!= targetType)
            {
                throw new InvalidCastException();
            }

            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.GetType() != typeof (Color))
            {
                throw new InvalidCastException();
            }

            var color = (Color) value;

            return color.ToString(new CultureInfo(language));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
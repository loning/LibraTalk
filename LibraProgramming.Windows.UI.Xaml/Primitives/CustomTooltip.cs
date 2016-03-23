using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    public class CustomTooltip : ControlPrimitive
    {
        public static readonly DependencyProperty TextProperty;

        public string Text
        {
            get
            {
                return (string) GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public CustomTooltip()
        {
            DefaultStyleKey = typeof (CustomTooltip);
        }

        static CustomTooltip()
        {
            TextProperty = DependencyProperty
                .Register(
                    "Text",
                    typeof (string),
                    typeof (CustomTooltip),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnTextPropertyChanged)
                );
        }

        private static void OnTextPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
//            throw new System.NotImplementedException();
        }
    }
}
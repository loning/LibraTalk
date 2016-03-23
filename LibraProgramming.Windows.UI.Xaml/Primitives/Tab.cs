using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    public class Tab : Control
    {
        public static readonly DependencyProperty HeaderProperty;
        public static readonly DependencyProperty HeaderTemplateProperty;

        /// <summary>
        /// 
        /// </summary>
        public object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate) GetValue(HeaderTemplateProperty);
            }
            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        static Tab()
        {
            HeaderProperty = DependencyProperty
                .Register(
                    "Header",
                    typeof (object),
                    typeof (Tab),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            HeaderTemplateProperty = DependencyProperty
                .Register(
                    "HeaderTemplate",
                    typeof (DataTemplate),
                    typeof (Tab),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }
    }
}
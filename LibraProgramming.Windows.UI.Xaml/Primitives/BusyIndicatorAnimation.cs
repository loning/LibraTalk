using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Type = typeof(FrameworkElement), Name = LayoutRootPartName)]
    [TemplatePart(Type = typeof(Storyboard), Name = AnimationPartName)]
    public class BusyIndicatorAnimation : Control
    {
        private const string LayoutRootPartName = "PART_LayoutRoot";
        private const string AnimationPartName = "PART_Animation";

        public static readonly DependencyProperty IsActiveProperty;

        private bool isActive;
        private Storyboard storyboard;

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        public BusyIndicatorAnimation()
        {
            DefaultStyleKey = typeof(BusyIndicatorAnimation);
        }

        static BusyIndicatorAnimation()
        {
            IsActiveProperty = DependencyProperty
                .Register(
                    "IsActive",
                    typeof (bool),
                    typeof (BusyIndicatorAnimation),
                    new PropertyMetadata(false, OnIsActivePropertyChanged)
                );
        }
        
        public void Start()
        {
            isActive = true;
            storyboard?.Begin();
        }

        public void Stop()
        {
            isActive = false;
            storyboard?.Stop();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var element = GetTemplateChild(LayoutRootPartName) as FrameworkElement;

            if (null != element)
            {
                if (element.Resources.ContainsKey(AnimationPartName))
                {
                    storyboard = element.Resources[AnimationPartName] as Storyboard;

                    if (isActive)
                    {
                        Start();
                    }
                }
                else
                {
                    storyboard = null;
                }
            }
        }

        private void OnIsActiveChanged(bool value)
        {
            if (value)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private static void OnIsActivePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((BusyIndicatorAnimation) source).OnIsActiveChanged((bool) e.NewValue);
        }
    }
}

using System;
using System.IO;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public enum IndicatorAnimation
    {
        /// <summary>
        /// 
        /// </summary>
        Ring,

        /// <summary>
        /// 
        /// </summary>
        Bar
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ContentPosition
    {
        /// <summary>
        /// 
        /// </summary>
        Left,

        /// <summary>
        /// 
        /// </summary>
        Top,

        /// <summary>
        /// 
        /// </summary>
        Right,

        /// <summary>
        /// 
        /// </summary>
        Bottom
    }

    /// <summary>
    /// 
    /// </summary>
    [TemplateVisualState(Name = RunningStateName, GroupName = IndicatorStatesGroupName)]
    [TemplateVisualState(Name = StoppedStateName, GroupName = IndicatorStatesGroupName)]
    [TemplatePart(Type = typeof(ContentPresenter), Name = ContentPresenterPartName)]
    [TemplatePart(Type = typeof(BusyIndicatorAnimation), Name = AnimationPartName)]
    [StyleTypedProperty(Property = AnimationStylePropertyName, StyleTargetType = typeof(BusyIndicatorAnimation))]
    [ContentProperty(Name = AnimationStylePropertyName)]
    public class BusyIndicator : ContentControlPrimitive
    {
        public static readonly DependencyProperty AnimationStyleProperty;
        public static readonly DependencyProperty ContentPositionProperty;
        public static readonly DependencyProperty DelayProperty;
        public static readonly DependencyProperty IsActiveProperty;
        public static readonly DependencyProperty IndicatorAnimationProperty;

        private const string ContentPresenterPartName = "PART_Content";
        private const string AnimationPartName = "PART_Animation";

        private const string IndicatorStatesGroupName = "IndicatorStates";
        private const string RunningStateName = "Running";
        private const string StoppedStateName = "Stopped";

        private const string AnimationStylePropertyName = "AnimationStyle";

        private ContentPresenter content;
        private readonly DispatcherTimer timer;
        private BusyIndicatorAnimation animation;
        private readonly Style[] stylesCache;

        /// <summary>
        /// 
        /// </summary>
        public Style AnimationStyle
        {
            get
            {
                return (Style) GetValue(AnimationStyleProperty);
            }
            set
            {
                SetValue(AnimationStyleProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ContentPosition ContentPosition
        {
            get
            {
                return (ContentPosition)GetValue(ContentPositionProperty);
            }
            set
            {
                SetValue(ContentPositionProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Delay
        {
            get
            {
                return (TimeSpan) GetValue(DelayProperty);
            }
            set
            {
                SetValue(DelayProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (bool) GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IndicatorAnimation IndicatorAnimation
        {
            get
            {
                return (IndicatorAnimation)GetValue(IndicatorAnimationProperty);
            }
            set
            {
                SetValue(IndicatorAnimationProperty, value);
            }
        }

        public BusyIndicator()
        {
            DefaultStyleKey = typeof (BusyIndicator);

            stylesCache = new Style[2];
            timer = new DispatcherTimer();
            timer.Tick += OnTimerTick;

            SizeChanged += OnSizeChanged;
        }

        static BusyIndicator()
        {
            AnimationStyleProperty = DependencyProperty
                .Register(
                    AnimationStylePropertyName,
                    typeof (Style),
                    typeof (BusyIndicator),
                    new PropertyMetadata(null)
                );
            ContentPositionProperty = DependencyProperty
                .Register(
                    "ContentPosition",
                    typeof (ContentPosition),
                    typeof (BusyIndicator),
                    new PropertyMetadata(ContentPosition.Bottom, OnContentPositionPropertyChanged)
                );
            DelayProperty = DependencyProperty
                .Register(
                    "Delay",
                    typeof (TimeSpan),
                    typeof (BusyIndicator),
                    new PropertyMetadata(TimeSpan.Zero)
                );
            IsActiveProperty = DependencyProperty
                .Register(
                    "IsActive",
                    typeof (bool),
                    typeof (BusyIndicator),
                    new PropertyMetadata(false, OnIsActivePropertyChanged)
                );
            IndicatorAnimationProperty = DependencyProperty
                .Register(
                    "IndicatorAnimation",
                    typeof (IndicatorAnimation),
                    typeof (BusyIndicator),
                    new PropertyMetadata(IndicatorAnimation.Ring, OnIndicatorAnimationPropertyChanged)
                );
        }

        protected override void OnApplyTemplate()
        {
            content = GetTemplatePart<ContentPresenter>(ContentPresenterPartName);

            if (DependencyProperty.UnsetValue == ReadLocalValue(ContentProperty) && null == GetValue(ContentProperty))
            {
                Content = PrimitivesLocalizationManager.Current.BusyIndicatorContent;
            }

            SynchronizeContentPosition();

            animation = GetTemplatePart<BusyIndicatorAnimation>(AnimationPartName);

            if (DependencyProperty.UnsetValue == ReadLocalValue(AnimationStyleProperty))
            {
                ApplyAnimationStyle(IndicatorAnimation);
            }

            base.OnApplyTemplate();

            UpdateVisualState(false);

            if (String.Equals(RunningStateName, CurrentVisualState))
            {
                animation.Start();
            }
            else
            {
                animation.Stop();
            }
        }

        protected override string GetCurrentVisualStateName()
        {
            return IsActive ? RunningStateName : StoppedStateName;
        }

        private void ApplyAnimationStyle(IndicatorAnimation value)
        {
            var index = (int)value;

            if (null == stylesCache[index])
            {
                var assembly = typeof(BusyIndicatorAnimation).GetTypeInfo().Assembly;
                var resourceName = "LibraProgramming.Windows.UI.Xaml.BusyIndicatorAnimation." + index + ".xaml";

                using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                {
                    var xaml = reader.ReadToEnd();
                    stylesCache[index] = XamlReader.Load(xaml) as Style;
                }
            }

            animation?.Stop();

            AnimationStyle = stylesCache[index];

            if (null != animation && String.Equals(RunningStateName, CurrentVisualState))
            {
                animation.Start();
            }
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);

            if (String.Equals(RunningStateName, CurrentVisualState))
            {
                animation.Start();
            }
            else
            {
                if (timer.IsEnabled)
                {
                    return;
                }

                animation.Stop();
            }
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);
            timer.Stop();
            animation.Stop();
        }

        private void OnIsActiveChanged(bool value)
        {
            var delay = Delay;

            if (value)
            {
                if (TimeSpan.Zero == delay)
                {
                    UpdateVisualState(true);
                }
                else
                {
                    timer.Interval = delay;
                    timer.Start();
                }
            }
            else
            {
                timer.Stop();
                UpdateVisualState(true);
            }
        }
        
        private void OnTimerTick(object sender, object e)
        {
            timer.Stop();
            UpdateVisualState(true);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry
            {
                Rect = new Rect(new Point(), e.NewSize)
            };
        }

        private void SynchronizeContentPosition()
        {
            switch (ContentPosition)
            {
                case ContentPosition.Left:
                    Grid.SetColumn(content, 0);
                    Grid.SetRow(content, 1);
                    break;

                case ContentPosition.Top:
                    Grid.SetColumn(content, 1);
                    Grid.SetRow(content, 0);
                    break;

                case ContentPosition.Right:
                    Grid.SetColumn(content, 2);
                    Grid.SetRow(content, 1);
                    break;

                case ContentPosition.Bottom:
                    Grid.SetColumn(content, 1);
                    Grid.SetRow(content, 2);
                    break;
            }
        }

        /*
                private void OnContentPositionChanged(ContentPosition value)
                {
                    SynchronizeContentPosition();
                }
        */

        private void OnIndicatorAnimationChanged(IndicatorAnimation value)
        {
            ApplyAnimationStyle(value);
        }

        private static void OnIndicatorAnimationPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((BusyIndicator) source).OnIndicatorAnimationChanged((IndicatorAnimation) e.NewValue);
        }

        private static void OnIsActivePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((BusyIndicator) source).OnIsActiveChanged((bool) e.NewValue);
        }

        private static void OnContentPositionPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((BusyIndicator) source).SynchronizeContentPosition();
        }
    }
}

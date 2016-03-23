using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    /// <summary>
    /// Sliding animation type for <see cref="LiveTile" /> control.
    /// </summary>
    public enum TileSliding
    {
        /// <summary>
        /// 
        /// </summary>
        Up,

        /// <summary>
        /// 
        /// </summary>
        Down,

        /// <summary>
        /// 
        /// </summary>
        Left,

        /// <summary>
        /// 
        /// </summary>
        Right
    }

    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = ScrollerPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = CurrentElementPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = NextElementPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = StackPanelPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TransformPartName, Type = typeof(FrameworkElement))]
    [ContentProperty(Name = ItemTemplatePropertyName)]
    public sealed class LiveTile : Control
    {
        private const string ScrollerPartName = "PART_Scroller";
        private const string CurrentElementPartName = "PART_Current";
        private const string NextElementPartName = "PART_Next";
        private const string StackPanelPartName = "PART_StackPanel";
        private const string TransformPartName = "PART_Transform";

        private const string ItemTemplatePropertyName = "ItemTemplate";

        public static readonly DependencyProperty EasingFunctionProperty;
        public static readonly DependencyProperty DelayProperty;
        public static readonly DependencyProperty ItemsSourceProperty;
        public static readonly DependencyProperty ItemTemplateProperty;
        public static readonly DependencyProperty TileSlidingProperty;
        public static readonly DependencyProperty TimeoutProperty;
        public static readonly DependencyProperty IsActiveProperty;

        private int currentIndex;
        private bool isFirstIteration;
        private DispatcherTimer timer;
        private FrameworkElement currentElement;
        private FrameworkElement nextElement;
        private FrameworkElement scroller;
        private TranslateTransform transform;
        private StackPanel stackPanel;

        /// <summary>
        /// The easing function to be applied to the sliding animation.
        /// </summary>
        public EasingFunctionBase EasingFunction
        {
            get
            {
                return (EasingFunctionBase) GetValue(EasingFunctionProperty);
            }
            set
            {
                SetValue(EasingFunctionProperty, value);
            }
        }
        
        /// <summary>
        /// The delay before the first animation.
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
        /// The source of the items.
        /// </summary>
        public object ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate) GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

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

        public TileSliding TileSliding
        {
            get
            {
                return (TileSliding) GetValue(TileSlidingProperty);
            }
            set
            {
                SetValue(TileSlidingProperty, value);
            }
        }

        public TimeSpan Timeout
        {
            get
            {
                return (TimeSpan) GetValue(TimeoutProperty);
            }
            set
            {
                SetValue(TimeoutProperty, value);
            }
        }

        private bool IsTemplateApplied
        {
            get;
            set;
        }

        public LiveTile()
        {
            DefaultStyleKey = typeof(LiveTile);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;
        }

        static LiveTile()
        {
            EasingFunctionProperty = DependencyProperty
                .Register(
                    "EasingFunction",
                    typeof (EasingFunctionBase),
                    typeof (LiveTile),
                    new PropertyMetadata(DependencyProperty.UnsetValue,
                        OnEasingFunctionPropertyChanged)
                );
            DelayProperty = DependencyProperty
                .Register(
                    "Delay",
                    typeof (TimeSpan),
                    typeof (LiveTile),
                    new PropertyMetadata(TimeSpan.Zero)
                );
            TileSlidingProperty = DependencyProperty
                .Register(
                    "TileSliding",
                    typeof (TileSliding),
                    typeof (LiveTile),
                    new PropertyMetadata(TileSliding.Up)
                );
            ItemsSourceProperty = DependencyProperty
                .Register(
                    "ItemsSource",
                    typeof (object),
                    typeof (LiveTile),
                    new PropertyMetadata(DependencyProperty.UnsetValue,
                        OnItemsSourcePropertyChanged)
                );
            ItemTemplateProperty = DependencyProperty
                .Register(
                    ItemTemplatePropertyName,
                    typeof (DataTemplate),
                    typeof (LiveTile),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            IsActiveProperty = DependencyProperty
                .Register(
                    "IsActive",
                    typeof (bool),
                    typeof (LiveTile),
                    new PropertyMetadata(true, OnIsActivePropertyChanged)
                );
            TimeoutProperty = DependencyProperty
                .Register(
                    "Timeout",
                    typeof (TimeSpan),
                    typeof (LiveTile),
                    new PropertyMetadata(TimeSpan.FromSeconds(5d),
                        OnTimeoutPropertyChanged)
                );
        }

        protected override void OnApplyTemplate()
        {
            try
            {
                IsTemplateApplied = false;

                scroller = GetTemplateChild(ScrollerPartName) as FrameworkElement;
                currentElement = GetTemplateChild(CurrentElementPartName) as FrameworkElement;
                nextElement = GetTemplateChild(NextElementPartName) as FrameworkElement;
                stackPanel = GetTemplateChild(StackPanelPartName) as StackPanel;
                transform = GetTemplateChild(TransformPartName) as TranslateTransform;

                if (null == scroller)
                {
                    throw new MissingTemplatePartException(typeof (FrameworkElement), ScrollerPartName);
                }

                if (null == stackPanel)
                {
                    throw new MissingTemplatePartException(typeof (StackPanel), StackPanelPartName);
                }

                if (null == currentElement)
                {
                    throw new MissingTemplatePartException(typeof (FrameworkElement), CurrentElementPartName);
                }

                if (null == nextElement)
                {
                    throw new MissingTemplatePartException(typeof (FrameworkElement), NextElementPartName);
                }

                stackPanel.Orientation = IsHorizontalSliding(TileSliding)
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

                base.OnApplyTemplate();
            }
            finally
            {
                IsTemplateApplied = true;
            }

            if (null != ItemsSource && IsActive)
            {
                Start();
            }
        }

        public void Start()
        {
            currentIndex = 0;
            isFirstIteration = true;

            if (Timeout <= TimeSpan.Zero || System.Threading.Timeout.InfiniteTimeSpan == Timeout)
            {
                return;
            }

            currentElement.DataContext = GetCurrent();
            nextElement.DataContext = GetNext();

            if (null == timer)
            {
                var interval = Delay;

                if (Delay <= TimeSpan.Zero || System.Threading.Timeout.InfiniteTimeSpan == Delay)
                {
                    interval = Timeout;
                }
                
                timer = new DispatcherTimer
                {
                    Interval = interval
                };
                timer.Tick += OnTimerTick;
            }

            timer.Start();
            IsActive = true;
        }

        public void Stop()
        {
            currentIndex = 0;
            currentElement.DataContext = GetCurrent();
            nextElement.DataContext = null;

            timer?.Stop();

            IsActive = false;
        }

        private object GetItemAt(int index)
        {
            if (null != ItemsSource)
            {
                var list = ItemsSource as IList;

                if (null != list)
                {
                    return list.Count > index ? list[index] : null;
                }

                var enumerable = ItemsSource as IEnumerable<object>;

                if (null != enumerable)
                {
                    return enumerable.ElementAt(index);
                }
            }

            return null;
        }

        private object GetCurrent()
        {
            return GetItemAt(currentIndex);
        }

        private object GetNext()
        {
            return GetItemAt(GetNextIndex());
        }

        private int GetNextIndex()
        {
            var count = GetItemsCount();

            if (count < 1)
            {
                return count;
            }

            if (TileSliding.Down == TileSliding || TileSliding.Left == TileSliding)
            {
                var index = currentIndex - 1;
                return index >= 0 ? index : count - 1;
            }
            
            return (currentIndex + 1) % count;
        }

        private int GetItemsCount()
        {
            if (null != ItemsSource)
            {
                if (ItemsSource is IList)
                {
                    return (ItemsSource as IList).Count;
                }
                
                if (ItemsSource is IEnumerable<object>)
                {
                    var items = ItemsSource as IEnumerable<object>;
                    return items.Count();
                }
            }

            return 0;
        }

        private void UpdateNextItem()
        {
            var hasSequence = false;

            if (ItemsSource is IEnumerable)
            {
                var count = 0;
                var enumerator = (ItemsSource as IEnumerable).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (++count > 1)
                    {
                        hasSequence = true;
                        break;
                    }
                }
            }

            if (!hasSequence)
            {
                return;
            }

            var storyboard = new Storyboard();

            if (null != transform)
            {
                var animation = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(700)),
                    FillBehavior = FillBehavior.HoldEnd,
                    EasingFunction = EasingFunction
                };

                switch (TileSliding)
                {
                    case TileSliding.Up:
                        animation.From = 0;
                        animation.To = -ActualHeight;
                        break;
                        
                    case TileSliding.Down:
                        animation.From = -ActualHeight;
                        animation.To = 0;
                        break;

                    case TileSliding.Left:
                        animation.From = 0;
                        animation.To = -ActualWidth;
                        break;

                    case TileSliding.Right:
                        animation.From = -ActualWidth;
                        animation.To = 0;
                        break;
                }

                Storyboard.SetTarget(animation, transform);
                Storyboard.SetTargetProperty(animation, IsHorizontalSliding(TileSliding) ? "X" : "Y");

                storyboard.Children.Add(animation);

            }

            storyboard.Completed += OnStoryboardComplete;

            if (IsNegativeSliding(TileSliding))
            {
                var next = nextElement.DataContext;

                nextElement.DataContext = currentElement.DataContext;
                currentElement.DataContext = next;
            }

            storyboard.Begin();
        }

        private static bool IsHorizontalSliding(TileSliding value)
        {
            return TileSliding.Left == value || TileSliding.Right == value;
        }

        private static bool IsNegativeSliding(TileSliding value)
        {
            return TileSliding.Down == value || TileSliding.Right == value;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsActive)
            {
                Start();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            timer?.Stop();

            if (null != transform)
            {
                transform.X = 0;
                transform.Y = 0;
            }
        }

        private void OnStoryboardComplete(object sender, object e)
        {
            var storyboard = (Storyboard) sender;

            storyboard.Stop();

            if (!IsNegativeSliding(TileSliding))
            {
                currentElement.DataContext = GetCurrent();
            }

            nextElement.DataContext = GetNext();
        }

        private void OnTimerTick(object sender, object e)
        {
            if (isFirstIteration)
            {
                timer.Interval = Timeout;
                isFirstIteration = false;
            }

            currentIndex = GetNextIndex();
            UpdateNextItem();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            currentElement.Width = e.NewSize.Width;
            currentElement.Height = e.NewSize.Height;

            nextElement.Width = e.NewSize.Width;
            nextElement.Height = e.NewSize.Height;

            if (TileSliding.Up == TileSliding || TileSliding.Down == TileSliding)
            {
                scroller.Height = e.NewSize.Height * 2;
            }
            else
            {
                scroller.Width = e.NewSize.Width * 2;
            }

            Clip = new RectangleGeometry
            {
                Rect = new Rect(new Point(), e.NewSize)
            };
        }

        private void OnTimeoutChanged(TimeSpan value)
        {
            if (null != timer)
            {
                timer.Interval = value;
            }
        }

        private void OnItemsSourceChanged(object current)
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            if (current is IEnumerable)
            {
                Start();
            }
            else
            {
                timer?.Stop();
            }
        }

        private void OnEasingFunctionChanged()
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            timer?.Stop();

            Start();
        }

        private void OnIsActiveChanged(bool current, bool previous)
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            if (current)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private static void OnEasingFunctionPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((LiveTile) source).OnEasingFunctionChanged();
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((LiveTile) source).OnItemsSourceChanged(e.NewValue);
        }

        private static void OnIsActivePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((LiveTile) source).OnIsActiveChanged((bool) e.NewValue, (bool) e.OldValue);
        }

        private static void OnTimeoutPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((LiveTile) source).OnTimeoutChanged((TimeSpan) e.NewValue);
        }
    }
}

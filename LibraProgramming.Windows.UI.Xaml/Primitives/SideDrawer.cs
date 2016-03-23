using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using LibraProgramming.Windows.UI.Xaml.Primitives.Animations;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    public enum DrawerState
    {
        Closed,
        Moving,
        Opened
    }

    public enum DrawerLocation
    {
        Left,
        Right
    }

    public enum DrawerTransition
    {
        Push
    }

    public enum DrawerManipulation
    {
        Button,
        Gesture,
        Both
    }

    [TemplatePart(Name = DrawerContainerPartName, Type = typeof(Canvas))]
    [TemplatePart(Name = DrawerPartName, Type = typeof(Grid))]
    [TemplatePart(Name = MainContentPartName, Type = typeof(SidePanel))]
    [TemplatePart(Name = DrawerButtonPartName, Type = typeof(Button))]
    [StyleTypedProperty(Property = DrawerButtonStylePropertyName, StyleTargetType = typeof(Button))]
    public sealed class SideDrawer : ControlPrimitive
    {
        private const string DrawerContainerPartName = "PART_SideDrawer";
        private const string DrawerPartName = "PART_Drawer";
        private const string MainContentPartName = "PART_ContentRoot";
        private const string DrawerButtonPartName = "PART_DrawerButton";
        private const string OverlayPopupPartName = "PART_Overlay";

        private const string DrawerButtonStylePropertyName = "DrawerButtonStyle";

        public static readonly DependencyProperty AnimationDurationProperty;
        public static readonly DependencyProperty MainContentProperty;
        public static readonly DependencyProperty MainContentTemplateProperty;
        public static readonly DependencyProperty DrawerContentProperty;
        public static readonly DependencyProperty DrawerContentTemplateProperty;
        public static readonly DependencyProperty DrawerStateProperty;
        public static readonly DependencyProperty DrawerTransitionProperty;
        public static readonly DependencyProperty DrawerLocationProperty;
        public static readonly DependencyProperty DrawerButtonStyleProperty;
        public static readonly DependencyProperty IsOpenProperty;
        public static readonly DependencyProperty TouchTargetThresholdProperty;

        private Canvas drawerContainer;
        private Grid drawer;
        private SidePanel mainContent;
        private Button drawerButton;
        private Popup overlayPopup;
        private bool hasPendingAnimation;
        private bool closeDrawer;
        private SideDrawerAnimationContext animationContext;
        private DoubleAnimation drawerHorizontalAnimation;
        private DoubleAnimation drawerReversedHorizontalAnimation;
        private DoubleAnimation mainContentHorizontalAnimation;
        private DoubleAnimation mainContentReversedHorizontalAnimation;
        private DoubleAnimation mainContentOpacityAnimation;
        private DoubleAnimation mainContentReversedOpacityAnimation;
        private Storyboard drawerStoryboard;
        private Storyboard drawerReversedStoryboard;
        private Storyboard mainContentStoryboard;
        private Storyboard mainContentReversedStoryboard;

        public Duration AnimationDuration
        {
            get
            {
                return (Duration) GetValue(AnimationDurationProperty);
            }
            set
            {
                SetValue(AnimationDurationProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object DrawerContent
        {
            get
            {
                return GetValue(DrawerContentProperty);
            }
            set
            {
                SetValue(DrawerContentProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate DrawerContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DrawerContentTemplateProperty);
            }
            set
            {
                SetValue(DrawerContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DrawerState DrawerState
        {
            get
            {
                return (DrawerState) GetValue(DrawerStateProperty);
            }
            set
            {
                SetValue(DrawerStateProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DrawerTransition DrawerTransition
        {
            get
            {
                return (DrawerTransition) GetValue(DrawerTransitionProperty);
            }
            set
            {
                SetValue(DrawerTransitionProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DrawerLocation DrawerLocation
        {
            get
            {
                return (DrawerLocation) GetValue(DrawerLocationProperty);
            }
            set
            {
                SetValue(DrawerLocationProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Style DrawerButtonStyle
        {
            get
            {
                return (Style) GetValue(DrawerButtonStyleProperty);
            }
            set
            {
                SetValue(DrawerButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object MainContent
        {
            get
            {
                return GetValue(MainContentProperty);
            }
            set
            {
                SetValue(MainContentProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate MainContentTemplate
        {
            get
            {
                return (DataTemplate) GetValue(MainContentTemplateProperty);
            }
            set
            {
                SetValue(MainContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return (bool) GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double TouchTargetThreshold
        {
            get
            {
                return (double) GetValue(TouchTargetThresholdProperty);
            }
            set
            {
                SetValue(TouchTargetThresholdProperty, value);
            }
        }

        internal DrawerManipulation DrawerManipulation
        {
            get;
            set;
        }

        public SideDrawer()
        {
            DefaultStyleKey = typeof(SideDrawer);
            SizeChanged += OnSizeChanged;
        }

        static SideDrawer()
        {
            AnimationDurationProperty = DependencyProperty
                .Register(
                    "AnimationDuration",
                    typeof (Duration),
                    typeof (SideDrawer),
                    new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300.0d)),
                        OnAnimationDurationPropertyChanged)
                );
            DrawerContentProperty = DependencyProperty
                .Register(
                    "DrawerContent",
                    typeof (object),
                    typeof (SideDrawer),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            DrawerContentTemplateProperty = DependencyProperty
                .Register(
                    "DrawerContentTemplate",
                    typeof (DataTemplate),
                    typeof (SideDrawer),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            DrawerLocationProperty = DependencyProperty
                .Register(
                    "DrawerLocation",
                    typeof (DrawerLocation),
                    typeof (SideDrawer),
                    new PropertyMetadata(DrawerLocation.Left, OnDrawerLocationPropertyChanged)
                );
            DrawerStateProperty = DependencyProperty
                .Register(
                    "DrawerState",
                    typeof (DrawerState),
                    typeof (SideDrawer),
                    new PropertyMetadata(DrawerState.Closed, OnDrawerStatePropertyChanged)
                );
            DrawerTransitionProperty = DependencyProperty
                .Register(
                    "DrawerTransition",
                    typeof (DrawerTransition),
                    typeof (SideDrawer),
                    new PropertyMetadata(DrawerTransition.Push, OnDrawerTransitionPropertyChanged)
                );
            DrawerButtonStyleProperty = DependencyProperty
                .Register(
                    DrawerButtonStylePropertyName,
                    typeof (Style),
                    typeof (SideDrawer),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            MainContentProperty = DependencyProperty
                .Register(
                    "MainContent",
                    typeof (object),
                    typeof (SideDrawer),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            MainContentTemplateProperty = DependencyProperty
                .Register(
                    "MainContentTemplate",
                    typeof (DataTemplate),
                    typeof (SideDrawer),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            IsOpenProperty = DependencyProperty
                .Register(
                    "IsOpen",
                    typeof (bool),
                    typeof (SideDrawer),
                    new PropertyMetadata(false, OnIsOpenPropertyChanged)
                );
            TouchTargetThresholdProperty = DependencyProperty
                .Register(
                    "TouchTargetThreshold",
                    typeof (double),
                    typeof (SideDrawer),
                    new PropertyMetadata(20.0d)
                );
        }

        protected override void OnApplyTemplate()
        {
            if (null != drawerButton)
            {
                drawerButton.Click -= OnDrawerButtonClick;
            }

            drawerButton = GetTemplatePart<Button>(DrawerButtonPartName, false);

            if (null != drawerButton)
            {
                drawerButton.Click += OnDrawerButtonClick;

                if (DrawerManipulation.Gesture == DrawerManipulation)
                {
                    drawerButton.Visibility = Visibility.Collapsed;
                }
            }

            drawerContainer = GetTemplatePart<Canvas>(DrawerContainerPartName);

            if (null != drawer)
            {
                drawer.ManipulationStarted -= OnDrawerManipulationStarted;
                drawer.ManipulationDelta -= OnDrawerManipulationDelta;
                drawer.ManipulationCompleted -= OnDrawerManipulationCompleted;
            }

            drawer = GetTemplatePart<Grid>(DrawerPartName);

            if (DrawerManipulation.Button != DrawerManipulation)
            {
                SubscribeToManipulationEvents();
            }

            overlayPopup = GetTemplatePart<Popup>(OverlayPopupPartName);

            if (null != mainContent)
            {
                mainContent.ManipulationStarted -= OnMainContentManipulationStarted;
                mainContent.ManipulationDelta -= OnMainContentManipulationDelta;
                mainContent.ManipulationCompleted -= OnMainContentManipulationCompleted;
                mainContent.Tapped -= OnMainContentTapped;
                mainContent.Drawer = null;
            }

            mainContent = GetTemplatePart<SidePanel>(MainContentPartName);

            mainContent.Tapped += OnMainContentTapped;
            mainContent.Drawer = this;
            
            base.OnApplyTemplate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            drawer.Measure(new Size(availableSize.Width, availableSize.Height));
            mainContent.Measure(new Size(availableSize.Width, availableSize.Height));

            var contentSize = mainContent.DesiredSize;

            drawer.Width = drawer.DesiredSize.Width;
            drawer.Height = contentSize.Height;

            var size = base.MeasureOverride(availableSize);

            size.Width = Math.Max(size.Width, contentSize.Width);
            size.Height = Math.Max(size.Height, contentSize.Height);

            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var geometry = new RectangleGeometry
            {
                Rect = new Rect(new Point(), finalSize)
            };

            Clip = geometry;

            if (hasPendingAnimation)
            {
                var context = CreateAnimationContext();

                SetAnimationContext(context);

                hasPendingAnimation = false;
            }

            return base.ArrangeOverride(finalSize);
        }

        private void ResetDrawer()
        {
            if (DrawerState.Closed != DrawerState)
            {
                if (IsOpen)
                {
                    animationContext.MainContentReversedStoryboard.Begin();
                    animationContext.MainContentReversedStoryboard.Pause();

                    animationContext.DrawerReversedStoryboard.Begin();
                    animationContext.DrawerReversedStoryboard.Pause();

                    animationContext.MainContentReversedStoryboard.Seek(AnimationDuration.TimeSpan);
                    animationContext.DrawerReversedStoryboard.Seek(AnimationDuration.TimeSpan);

                    animationContext.MainContentReversedStoryboard.Resume();
                    animationContext.DrawerReversedStoryboard.Resume();
                }
                else
                {
                    animationContext.MainContentStoryboard.Begin();
                    animationContext.MainContentStoryboard.Pause();

                    animationContext.DrawerStoryboard.Begin();
                    animationContext.DrawerStoryboard.Pause();

                    animationContext.MainContentStoryboard.Seek(AnimationDuration.TimeSpan);
                    animationContext.DrawerStoryboard.Seek(AnimationDuration.TimeSpan);

                    animationContext.MainContentStoryboard.Resume();
                    animationContext.DrawerStoryboard.Resume();
                }

                IsOpen = false;
                DrawerState = DrawerState.Closed;
            }

            hasPendingAnimation = true;

            drawer.ClearValue(HeightProperty);
            drawer.ClearValue(WidthProperty);

            InvalidateMeasure();
        }

        private void PrepareSideBar()
        {
            drawer.Clip = null;
            drawer.RenderTransform = null;

            switch (DrawerLocation)
            {
                case DrawerLocation.Left:
                    Canvas.SetTop(drawer, 0);
                    switch (DrawerTransition)
                    {
                        default:
                            Canvas.SetLeft(drawer, -drawer.Width);
                            break;
                    }

                    break;

                case DrawerLocation.Right:
                    Canvas.SetTop(drawer, 0);
                    switch (DrawerTransition)
                    {
                        default:
                            Canvas.SetLeft(drawer, mainContent.ActualWidth - drawer.Width);
                            break;
                    }

                    break;
            }
        }

        private SideDrawerAnimationContext CreateAnimationContext()
        {
            CreateDoubleAnimations();
            PrepareSideBar();

            switch (DrawerTransition)
            {
                case DrawerTransition.Push:
                    return CreatePushAnimations();

                default:
                    throw new Exception();
            }
        }

        private void CreateDoubleAnimations()
        {
            const string canvasLeftProperty = "(Canvas.Left)";
            const string opacityProperty = "Opacity";

            mainContentStoryboard = new Storyboard();
            mainContentReversedStoryboard = new Storyboard();
            drawerStoryboard = new Storyboard();
            drawerReversedStoryboard = new Storyboard();

            drawerHorizontalAnimation = new DoubleAnimation
            {
                Duration = AnimationDuration
            };

            Storyboard.SetTarget(drawerHorizontalAnimation, drawer);
            Storyboard.SetTargetProperty(drawerHorizontalAnimation, canvasLeftProperty);

            drawerReversedHorizontalAnimation = new DoubleAnimation
            {
                Duration = AnimationDuration
            };

            Storyboard.SetTarget(drawerReversedHorizontalAnimation, drawer);
            Storyboard.SetTargetProperty(drawerReversedHorizontalAnimation, canvasLeftProperty);

            mainContentHorizontalAnimation = new DoubleAnimation
            {
                Duration = AnimationDuration
            };

            Storyboard.SetTarget(mainContentHorizontalAnimation, mainContent);
            Storyboard.SetTargetProperty(mainContentHorizontalAnimation, canvasLeftProperty);

            mainContentReversedHorizontalAnimation = new DoubleAnimation
            {
                Duration = AnimationDuration
            };

            Storyboard.SetTarget(mainContentReversedHorizontalAnimation, mainContent);
            Storyboard.SetTargetProperty(mainContentReversedHorizontalAnimation, canvasLeftProperty);

            mainContentOpacityAnimation = new DoubleAnimation
            {
                Duration = AnimationDuration
            };

            //Storyboard.SetTarget(mainContentOpacityAnimation, mainContent);
            Storyboard.SetTarget(mainContentOpacityAnimation, overlayPopup);
            Storyboard.SetTargetProperty(mainContentOpacityAnimation, opacityProperty);

            mainContentReversedOpacityAnimation = new DoubleAnimation
            {
                Duration = AnimationDuration
            };

            //Storyboard.SetTarget(mainContentReversedOpacityAnimation, mainContent);
            Storyboard.SetTarget(mainContentReversedOpacityAnimation, overlayPopup);
            Storyboard.SetTargetProperty(mainContentReversedOpacityAnimation, opacityProperty);
        }

        private SideDrawerAnimationContext CreatePushAnimations()
        {
            switch (DrawerLocation)
            {
                case DrawerLocation.Left:
                    drawerHorizontalAnimation.From = -drawer.Width;
                    drawerHorizontalAnimation.To = 0.0d;
                    drawerStoryboard.Children.Add(drawerHorizontalAnimation);
                    drawerReversedHorizontalAnimation.From = 0.0d;
                    drawerReversedHorizontalAnimation.To = -drawer.Width;
                    drawerReversedStoryboard.Children.Add(drawerReversedHorizontalAnimation);
                    mainContentHorizontalAnimation.From = 0.0d;
                    mainContentHorizontalAnimation.To = drawer.Width;
                    mainContentStoryboard.Children.Add(mainContentHorizontalAnimation);
                    mainContentReversedHorizontalAnimation.From = drawer.Width;
                    mainContentReversedHorizontalAnimation.To = 0.0d;
                    mainContentReversedStoryboard.Children.Add(mainContentReversedHorizontalAnimation);

                    break;

                case DrawerLocation.Right:
                    var availableWidth = mainContent.ActualWidth - drawer.Width;

                    drawerHorizontalAnimation.From = mainContent.ActualWidth;
                    drawerHorizontalAnimation.To = availableWidth;
                    drawerStoryboard.Children.Add(drawerHorizontalAnimation);
                    drawerReversedHorizontalAnimation.From = availableWidth;
                    drawerReversedHorizontalAnimation.To = -drawer.Width;
                    drawerReversedStoryboard.Children.Add(drawerReversedHorizontalAnimation);
                    mainContentHorizontalAnimation.From = 0.0d;
                    mainContentHorizontalAnimation.To = -drawer.Width;
                    mainContentStoryboard.Children.Add(mainContentHorizontalAnimation);
                    mainContentReversedHorizontalAnimation.From = -drawer.Width;
                    mainContentReversedHorizontalAnimation.To = 0.0d;
                    mainContentReversedStoryboard.Children.Add(mainContentReversedHorizontalAnimation);

                    break;
            }

            mainContentOpacityAnimation.From = 1.0d;
            mainContentOpacityAnimation.To = 0.5d;
            mainContentStoryboard.Children.Add(mainContentOpacityAnimation);
            mainContentReversedOpacityAnimation.From = 0.5d;
            mainContentReversedOpacityAnimation.To = 1.0d;
            mainContentReversedStoryboard.Children.Add(mainContentReversedOpacityAnimation);

            return new SideDrawerAnimationContext(
                mainContentStoryboard,
                mainContentReversedStoryboard,
                drawerStoryboard,
                drawerReversedStoryboard
                );
        }

        private void SubscribeToManipulationEvents()
        {
            if (IsOpen)
            {
                drawer.ManipulationStarted += OnDrawerManipulationStarted;
                drawer.ManipulationDelta += OnDrawerManipulationDelta;
                drawer.ManipulationCompleted += OnDrawerManipulationCompleted;
            }
            else
            {
                mainContent.ManipulationStarted += OnMainContentManipulationStarted;
                mainContent.ManipulationDelta += OnMainContentManipulationDelta;
                mainContent.ManipulationCompleted += OnMainContentManipulationCompleted;
            }
        }

        private void UnsubscribeFromManipulationEvents()
        {
            if (IsOpen)
            {
                mainContent.ManipulationStarted -= OnMainContentManipulationStarted;
                mainContent.ManipulationDelta -= OnMainContentManipulationDelta;
                mainContent.ManipulationCompleted -= OnMainContentManipulationCompleted;
            }
            else
            {
                drawer.ManipulationStarted -= OnDrawerManipulationStarted;
                drawer.ManipulationDelta -= OnDrawerManipulationDelta;
                drawer.ManipulationCompleted -= OnDrawerManipulationCompleted;
            }
        }

        private void SetAnimationContext(SideDrawerAnimationContext value)
        {
            if (null != animationContext)
            {
                animationContext.DrawerStoryboard.Completed -= OnDrawerStoryboardCompleted;
                animationContext.DrawerReversedStoryboard.Completed -= OnDrawerReversedStoryboardCompleted;
            }

            animationContext = value;

            if (null != animationContext)
            {
                animationContext.DrawerStoryboard.Completed += OnDrawerStoryboardCompleted;
                animationContext.DrawerReversedStoryboard.Completed += OnDrawerReversedStoryboardCompleted;
            }
        }

        private void OnDrawerButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsOpen)
            {
                animationContext.MainContentReversedStoryboard.Begin();
                animationContext.DrawerReversedStoryboard.Begin();
                IsOpen = false;
            }
            else
            {
                animationContext.MainContentStoryboard.Begin();
                animationContext.DrawerStoryboard.Begin();
                IsOpen = true;
                closeDrawer = false;
            }
        }

        private void OnDrawerStoryboardCompleted(object sender, object e)
        {
            DrawerState = DrawerState.Opened;
        }

        private void OnDrawerReversedStoryboardCompleted(object sender, object e)
        {
            DrawerState = DrawerState.Closed;
        }

        private void OnDrawerManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnDrawerManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnDrawerManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnMainContentManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            switch (DrawerLocation)
            {
                case DrawerLocation.Left:
                    if (e.Position.X >= TouchTargetThreshold)
                    {
                        return;
                    }

                    break;

                case DrawerLocation.Right:
                    if (e.Position.X <= (mainContent.ActualWidth - TouchTargetThreshold))
                    {
                        return;
                    }

                    break;
            }

            DrawerState = DrawerState.Moving;
            animationContext.MainContentStoryboard.Begin();
            animationContext.MainContentStoryboard.Pause();
            animationContext.DrawerStoryboard.Begin();
            animationContext.DrawerStoryboard.Pause();

            hasPendingAnimation = true;
        }

        private void OnMainContentManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!hasPendingAnimation)
            {
                return;
            }

            var delta = 0.0d;
            var distance = 0.0d;

            switch (DrawerLocation)
            {
                case DrawerLocation.Left:
                    delta = e.Cumulative.Translation.X;
                    distance = drawer.Width;
                    break;

                case DrawerLocation.Right:
                    delta = -e.Cumulative.Translation.X;
                    distance = drawer.Width;
                    break;
            }

            var milliseconds = delta / distance * AnimationDuration.TimeSpan.Milliseconds;

            if (milliseconds > AnimationDuration.TimeSpan.Milliseconds)
            {
                milliseconds = AnimationDuration.TimeSpan.Milliseconds;
            }
            else if (milliseconds < 0.0)
            {
                milliseconds = 0.0;
                if (DrawerState.Closed != DrawerState)
                {
                    DrawerState = DrawerState.Closed;
                }
            }

            if (milliseconds < AnimationDuration.TimeSpan.Milliseconds && milliseconds > 0.0 && DrawerState.Moving != DrawerState)
            {
                animationContext.MainContentStoryboard.Begin();
                animationContext.MainContentStoryboard.Pause();
                animationContext.DrawerStoryboard.Begin();
                animationContext.DrawerStoryboard.Pause();
                DrawerState = DrawerState.Moving;
            }

            animationContext.MainContentStoryboard.Seek(TimeSpan.FromMilliseconds(milliseconds));
            animationContext.DrawerStoryboard.Seek(TimeSpan.FromMilliseconds(milliseconds));

            if (milliseconds == AnimationDuration.TimeSpan.Milliseconds && e.IsInertial)
            {
                IsOpen = true;
                hasPendingAnimation = true;
            }
            else
            {
                if (milliseconds != 0.0 || e.IsInertial)
                {
                    return;
                }

                IsOpen = false;
                hasPendingAnimation = false;
            }
        }

        private void OnMainContentManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!hasPendingAnimation)
            {
                return;
            }

            var delta = 0.0;
            var distance = 0.0d;

            switch (DrawerLocation)
            {
                case DrawerLocation.Left:
                    delta = e.Cumulative.Translation.X;
                    distance = drawer.Width;
                    break;

                case DrawerLocation.Right:
                    delta = -e.Cumulative.Translation.X;
                    distance = drawer.Width;
                    break;
            }

            var milliseconds = delta / distance * AnimationDuration.TimeSpan.Milliseconds;

            if (milliseconds > (distance/3.0d))
            {
                animationContext.MainContentStoryboard.Resume();
                animationContext.DrawerStoryboard.Resume();
                IsOpen = true;
            }
            else
            {
                animationContext.MainContentReversedStoryboard.Begin();
                animationContext.MainContentReversedStoryboard.Pause();
                animationContext.DrawerReversedStoryboard.Begin();
                animationContext.DrawerReversedStoryboard.Pause();
                animationContext.MainContentReversedStoryboard.Seek(TimeSpan.FromMilliseconds(AnimationDuration.TimeSpan.Milliseconds - milliseconds));
                animationContext.DrawerReversedStoryboard.Seek(TimeSpan.FromMilliseconds(AnimationDuration.TimeSpan.Milliseconds - milliseconds));
                animationContext.MainContentReversedStoryboard.Resume();
                animationContext.DrawerReversedStoryboard.Resume();
                IsOpen = false;
            }

            hasPendingAnimation = false;
        }

        private void OnMainContentTapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsOpen && closeDrawer)
            {
                animationContext.MainContentReversedStoryboard.Begin();
                animationContext.DrawerReversedStoryboard.Begin();
                IsOpen = false;
            }

            closeDrawer = true;
        }

        private void OnDrawerLocationChanged()
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            ResetDrawer();
        }

        private void OnDrawerStateChanged()
        {
            //throw new NotImplementedException();
        }

        private void OnIsOpenChanged()
        {
            if (DrawerManipulation.Button == DrawerManipulation)
            {
                return;
            }

            SubscribeToManipulationEvents();
            UnsubscribeFromManipulationEvents();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize == e.PreviousSize)
            {
                return;
            }

            var context = CreateAnimationContext();

            SetAnimationContext(context);
        }

        private void OnDrawerTransitionChanged()
        {
            if (!IsTemplateApplied)
            {
                return;
            }

            ResetDrawer();
        }

        private static void OnAnimationDurationPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnDrawerLocationPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SideDrawer) source).OnDrawerLocationChanged();
        }

        private static void OnDrawerTransitionPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SideDrawer) source).OnDrawerTransitionChanged();
        }

        private static void OnDrawerStatePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SideDrawer) source).OnDrawerStateChanged();
        }

        private static void OnIsOpenPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SideDrawer) source).OnIsOpenChanged();
        }
    }
}

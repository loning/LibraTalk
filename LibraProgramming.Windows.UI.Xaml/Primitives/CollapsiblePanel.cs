using System;
using System.Linq;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
/*
    [TemplateVisualState(GroupName = FloatingStatesGroupName, Name = FloatingVisibleStateName)]
    [TemplateVisualState(GroupName = FloatingStatesGroupName, Name = FloatingHiddenStateName)]
    [TemplatePart(Name = LayoutRootPartName)]
    [TemplatePart(Name = TransitionTransformPartName, Type = typeof(CompositeTransform))]
    [TemplatePart(Name = FloatingVisibleHorizontalTransitionPartName, Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = FloatingHiddenHorizontalTransitionPartName, Type = typeof(DoubleAnimation))]
    public class CollapsiblePanel : ContentControlPrimitive
    {
        private const string LayoutRootPartName = "PART_LayoutRoot";
        private const string TransitionTransformPartName = "PART_TransitionTransform";

        private const string FloatingStatesGroupName = "FloatingStates";
        private const string FloatingVisibleStateName = "FloatingVisible";
        private const string FloatingHiddenStateName = "FloatingHidden";

        private const string FloatingVisibleHorizontalTransitionPartName = "PART_FloatingVisibleHorizontalTransition";
        private const string FloatingVisibleVerticalTransitionPartName = "PART_FloatingVisibleVerticalTransition";
        private const string FloatingHiddenHorizontalTransitionPartName = "PART_FloatingHiddenHorizontalTransition";
        private const string FloatingHiddenVerticalTransitionPartName = "PART_FloatingHiddenVerticalTransition";

        public static readonly DependencyProperty IsOpenProperty;

        private CompositeTransform transitionTransform;
        private DoubleAnimation floatingVisibleHorizontalTransition;
        private DoubleAnimation floatingHiddenHorizontalTransition;
        private bool mouseRightButtonPressed;

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
        public event EventHandler<object> Opened;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<object> Closed;

        public CollapsiblePanel()
        {
            DefaultStyleKey = typeof (CollapsiblePanel);
        }

        static CollapsiblePanel()
        {
            IsOpenProperty = DependencyProperty
                .Register(
                    "IsOpen",
                    typeof (bool),
                    typeof (CollapsiblePanel),
                    new PropertyMetadata(false, OnIsOpenPropertyChanged)
                );
        }

        protected override void OnApplyTemplate()
        {
            var layoutRoot = GetTemplatePart<FrameworkElement>(LayoutRootPartName, false);

            if (null != layoutRoot)
            {
                var visualStateGroup = VisualStateManager
                    .GetVisualStateGroups(layoutRoot)
                    .FirstOrDefault(group => String.Equals(group.Name, FloatingStatesGroupName));

                var state = visualStateGroup?.States
                    .FirstOrDefault(
                        floatingState => String.Equals(floatingState.Name, FloatingVisibleStateName)
                    );

                if (null != state?.Storyboard)
                {
                    floatingVisibleHorizontalTransition = state.Storyboard.Children
                        .FirstOrDefault(
                            timeline =>
                                String.Equals(Storyboard.GetTargetName(timeline), TransitionTransformPartName)
                                && String.Equals(Storyboard.GetTargetProperty(timeline), "TranslateX")
                        ) as DoubleAnimation;
                }

                state = visualStateGroup?.States
                    .FirstOrDefault(
                        floatingState => String.Equals(floatingState.Name, FloatingHiddenStateName)
                    );

                if (null != state?.Storyboard)
                {
                    floatingHiddenHorizontalTransition = state.Storyboard.Children
                        .FirstOrDefault(
                            timeline =>
                                String.Equals(Storyboard.GetTargetName(timeline), TransitionTransformPartName)
                                && String.Equals(Storyboard.GetTargetProperty(timeline), "TranslateY")
                        ) as DoubleAnimation;
                }
            }

            transitionTransform = GetTemplatePart<CompositeTransform>(TransitionTransformPartName);

            base.OnApplyTemplate();

            if (IsOpen)
            {
                DoOpen(false);
            }
            else
            {
                DoClosed(false);
            }
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);

            EdgeGesture.GetForCurrentView().Completed += OnEdgeGestureCompleted;
            Window.Current.SizeChanged += OnWindowSizeChanged;
            Window.Current.CoreWindow.PointerPressed += OnCoreWindowPointerPressed;
            Window.Current.CoreWindow.PointerReleased += OnCoreWindowPoinerReleased;

            if (!IsOpen)
            {
                SetPositionOutsideClipBounds();
            }
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);

            EdgeGesture.GetForCurrentView().Completed -= OnEdgeGestureCompleted;
            Window.Current.SizeChanged -= OnWindowSizeChanged;
            Window.Current.CoreWindow.PointerPressed -= OnCoreWindowPointerPressed;
            Window.Current.CoreWindow.PointerReleased -= OnCoreWindowPoinerReleased;
        }

        protected virtual void OnOpened(object obj)
        {
            var handler = Opened;
            handler?.Invoke(this, obj);
        }

        protected virtual void OnClosed(object obj)
        {
            var handler = Closed;
            handler?.Invoke(this, obj);
        }

        private void SetPositionOutsideClipBounds()
        {
            if (null == transitionTransform)
            {
                return;
            }

            if (VerticalAlignment.Bottom == VerticalAlignment)
            {
                transitionTransform.TranslateY = ActualHeight;
            }
            else if (VerticalAlignment.Top == VerticalAlignment)
            {
                transitionTransform.TranslateY = -ActualHeight;
            }

            if (HorizontalAlignment.Left == HorizontalAlignment)
            {
                transitionTransform.TranslateX = -ActualWidth;
            }
            else if (HorizontalAlignment.Right == HorizontalAlignment)
            {
                transitionTransform.TranslateX = ActualWidth;
            }
        }

        private void OnIsOpenChanged(bool previous, bool current)
        {
            if (!IsLoaded)
            {
                return;
            }

            if (current)
            {
                DoOpen(true);
            }
            else
            {
                DoClosed(true);
            }
        }

        private void DoOpen(bool useTransitions)
        {
            GoToFloatingVisibleVisualState(useTransitions);
            OnOpened(null);
        }

        private void DoClosed(bool useTransitions)
        {
            GoToFloatingHiddenVisualState(useTransitions);
            OnClosed(null);
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnCoreWindowPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (IsOpen && null != Window.Current && null != Window.Current.Content)
            {
                var t = Window.Current.Content.TransformToVisual(this);
                var p = t.TransformPoint(args.CurrentPoint.Position);
                //var b = this.GetBoundingRect(this);

                IsOpen = false;
            }

            if (PointerDeviceType.Mouse == args.CurrentPoint.PointerDevice.PointerDeviceType)
            {
                var props = args.CurrentPoint.Properties;
                
                mouseRightButtonPressed = props.IsRightButtonPressed && !(props.IsLeftButtonPressed || props.IsMiddleButtonPressed);

                if (mouseRightButtonPressed)
                {
                    args.Handled = true;
                }
            }
        }

        private void OnCoreWindowPoinerReleased(CoreWindow sender, PointerEventArgs args)
        {
            if (PointerDeviceType.Mouse == args.CurrentPoint.PointerDevice.PointerDeviceType)
            {
                var props = args.CurrentPoint.Properties;

                if (mouseRightButtonPressed && !(props.IsLeftButtonPressed || props.IsMiddleButtonPressed))
                {
                    OnSwitchGesture();
                    args.Handled = true;
                }

                mouseRightButtonPressed = false;
            }
        }

        private void OnEdgeGestureCompleted(EdgeGesture sender, EdgeGestureEventArgs args)
        {
            OnSwitchGesture();
        }

        private void OnSwitchGesture()
        {
            if (IsOpen)
            {
                IsOpen = false;
            }
            else
            {
                IsOpen = true;
            }
        }

        private void GoToFloatingHiddenVisualState(bool useTransition)
        {
            if (HorizontalAlignment.Left == HorizontalAlignment && null != floatingHiddenHorizontalTransition)
            {
                floatingHiddenHorizontalTransition.To = -ActualWidth;
            }
            else if (HorizontalAlignment.Right == HorizontalAlignment && null != floatingHiddenHorizontalTransition)
            {
                floatingHiddenHorizontalTransition.To = ActualWidth;
            }

            VisualStateManager.GoToState(this, FloatingHiddenStateName, useTransition);
        }
        private void GoToFloatingVisibleVisualState(bool useTransition)
        {
            SetPositionOutsideClipBounds();

            if (null != floatingVisibleHorizontalTransition)
            {
                floatingVisibleHorizontalTransition.To = 0;
            }
            else if (HorizontalAlignment.Right == HorizontalAlignment && null != floatingHiddenHorizontalTransition)
            {
                floatingHiddenHorizontalTransition.To = ActualWidth;
            }

            VisualStateManager.GoToState(this, FloatingVisibleStateName, useTransition);
        }

        private static void OnIsOpenPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((CollapsiblePanel) source).OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
        }
    }
*/
}
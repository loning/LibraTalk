using System;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using LibraProgramming.Windows.UI.Xaml.Primitives;

namespace LibraProgramming.Windows.UI.Xaml.Interactivity
{
    public sealed class TooltipBehavior : ElementAdorner
    {
        public static readonly DependencyProperty PlacementProperty;
        public static readonly DependencyProperty PlacementTargetProperty;
        public static readonly DependencyProperty TooltipProperty;

        private Popup popup;

        public PlacementMode Placement
        {
            get
            {
                return (PlacementMode) GetValue(PlacementProperty);
            }
            set
            {
                SetValue(PlacementProperty, value);
            }
        }

        public UIElement PlacementTarget
        {
            get
            {
                return (UIElement) GetValue(PlacementTargetProperty);
            }
            set
            {
                SetValue(PlacementTargetProperty, value);
            }
        }

        public object Tooltip
        {
            get
            {
                return GetValue(TooltipProperty);
            }
            set
            {
                SetValue(TooltipProperty, value);
            }
        }

        static TooltipBehavior()
        {
            PlacementProperty = DependencyProperty
                .Register(
                    "Placement",
                    typeof(PlacementMode),
                    typeof(TooltipBehavior),
                    new PropertyMetadata(PlacementMode.Mouse, OnPlacementPropertyChanged)
                );
            PlacementTargetProperty = DependencyProperty
                .Register(
                    "PlacementTarget",
                    typeof(UIElement),
                    typeof(TooltipBehavior),
                    new PropertyMetadata(null, OnPlacementTargetPropertyChanged)
                );
            TooltipProperty = DependencyProperty
                .Register(
                    "Tooltip",
                    typeof (object),
                    typeof (TooltipBehavior),
                    new PropertyMetadata(null, OnTooltipPropertyChanged)
                );
        }

        protected override void DoAttach()
        {
            Element.Loaded += OnElementLoaded;
            Element.Unloaded += OnElementUnloaded;
        }

        protected override void DoRelease()
        {
            UnhookPointerEvents();
            Element.Unloaded -= OnElementUnloaded;
            Element.Loaded -= OnElementLoaded;
        }

        private void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            HookPointerEvents();
        }

        private void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            UnhookPointerEvents();
        }

        private void HookPointerEvents()
        {
            Element.PointerEntered += OnElementPointerEntered;
            Element.PointerExited += OnElementPointerExited;
            Element.PointerMoved += OnElementPointerMoved;
        }

        private void UnhookPointerEvents()
        {
            Element.PointerMoved -= OnElementPointerMoved;
            Element.PointerExited -= OnElementPointerExited;
            Element.PointerEntered -= OnElementPointerEntered;
        }

        private void OnElementPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (null != popup)
            {
                return;
            }

            var tooltip = CustomTooltipService.GetTooltip(Element);
            var placement = PlacementMode.Bottom;// CustomTooltipService.GetPlacement(Element);
            var host = tooltip as CustomTooltip;

            if (null == host)
            {
                host = new CustomTooltip
                {
                    Content = tooltip
                };
            }

            popup = new Popup
            {
                Child = host,
                IsOpen = true,
                IsLightDismissEnabled = true
            };

            host.SetBinding(
                FrameworkElement.DataContextProperty,
                new Binding
                {
                    Path = new PropertyPath("DataContext"),
                    Source = Element
                });

            UIElement placementTarget = null;

            if (placement != PlacementMode.Mouse)
            {
                placementTarget = CustomTooltipService.GetPlacementTarget(Element) ?? (sender as UIElement);
            }

            PerformPlacement(popup, placement, placementTarget, e.GetCurrentPoint(Window.Current.Content));
        }

        private void OnElementPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var placement = CustomTooltipService.GetPlacement(Element);

            if (PlacementMode.Mouse != placement)
            {
                return;
            }


        }

        private void OnElementPointerExited(object sender, PointerRoutedEventArgs e)
        {
            popup.IsOpen = false;
            popup.Child = null;
            popup = null;
        }

        private void UpdateTooltip(object value)
        {
            /*if (null == host || !popup.IsOpen)
            {
                return;
            }

            host.Content = value;*/
        }

        private void PerformPlacement(Popup popup, PlacementMode placement, UIElement placementTarget, PointerPoint point)
        {
            var bounds = Window.Current.Bounds;
            var availableSize = new Size(bounds.Width / 2.0d, Double.MaxValue);
            Point origin;
            Size size;

            if (PlacementMode.Mouse == placement)
            {
                origin = point.Position;
                size = availableSize;
            }
            else
            {
                var transform = placementTarget.TransformToVisual(Window.Current.Content);

                size = placementTarget.DesiredSize;
                origin = transform.TransformPoint(new Point(0.0, size.Height));
            }

            placementTarget.Measure(availableSize);

            popup.HorizontalOffset = origin.X;
            popup.VerticalOffset = origin.Y;
            popup.Width = size.Width;
            popup.Height = size.Height;

            var element = popup.Child as FrameworkElement;

            if (null == element)
            {
                return;
            }

            element.Width = size.Width;
            element.Height = size.Height;

            var temp = new ToolTip();
        }

        private static void OnPlacementTargetPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
//            throw new System.NotImplementedException();
        }

        private static void OnPlacementPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
//            throw new System.NotImplementedException();
        }

        private static void OnTooltipPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((TooltipBehavior) source).UpdateTooltip(e.NewValue);
        }
    }
}
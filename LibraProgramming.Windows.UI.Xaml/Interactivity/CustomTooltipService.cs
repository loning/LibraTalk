using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace LibraProgramming.Windows.UI.Xaml.Interactivity
{
    public class CustomTooltipService
    {
        public static readonly DependencyProperty TooltipProperty;
        public static readonly DependencyProperty PlacementProperty;
        public static readonly DependencyProperty PlacementTargetProperty;
        internal static readonly DependencyProperty TooltipBehaviorProperty;

        public static PlacementMode GetPlacement(UIElement element)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (PlacementMode) element.GetValue(PlacementProperty);
        }

        public static void SetPlacement(UIElement element, PlacementMode value)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(PlacementProperty, value);
        }

        public static UIElement GetPlacementTarget(UIElement element)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (UIElement) element.GetValue(PlacementTargetProperty);
        }

        public static void SetPlacementTarget(UIElement element, UIElement value)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(PlacementTargetProperty, value);
        }

        public static object GetTooltip(UIElement element)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetValue(TooltipProperty);
        }

        public static void SetTooltip(UIElement element, object value)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(TooltipProperty, value);
        }

        static CustomTooltipService()
        {
            PlacementProperty = DependencyProperty
                .RegisterAttached(
                    "Placement",
                    typeof (PlacementMode),
                    typeof (CustomTooltipService),
                    new PropertyMetadata(PlacementMode.Mouse, OnPlacementPropertyChanged)
                );
            PlacementTargetProperty = DependencyProperty
                .RegisterAttached(
                    "PlacementTarget",
                    typeof (UIElement),
                    typeof (CustomTooltipService),
                    new PropertyMetadata(null, OnPlacementTargetPropertyChanged)
                );
            TooltipProperty = DependencyProperty
                .RegisterAttached(
                    "Tooltip",
                    typeof (object),
                    typeof (CustomTooltipService),
                    new PropertyMetadata(null, OnTooltipPropertyChanged)
                );
            TooltipBehaviorProperty = DependencyProperty
                .RegisterAttached(
                    "TooltipBehavior",
                    typeof (TooltipBehavior),
                    typeof (CustomTooltipService),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        private static TooltipBehavior GetTooltipBehavior(FrameworkElement element)
        {
            var behavior = (TooltipBehavior) element.GetValue(TooltipBehaviorProperty);

            if (null == behavior)
            {
                behavior = new TooltipBehavior();
                behavior.Attach(element);
            }

            return behavior;
        }

        private static bool HasTooltipBehavior(FrameworkElement element)
        {
            return null != element.GetValue(TooltipBehaviorProperty);
        }

        private static void OnPlacementPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var previous = (PlacementMode) e.OldValue;
            var current = (PlacementMode) e.NewValue;

            if (previous == current)
            {
                return;
            }

            var element = source as FrameworkElement;

            if (null != element && HasTooltipBehavior(element))
            {
                var behavior = GetTooltipBehavior(element);

                behavior.Placement = current;
            }
        }

        private static void OnPlacementTargetPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var previous = (UIElement)e.OldValue;
            var current = (UIElement)e.NewValue;

            if (previous == current)
            {
                return;
            }

            var element = source as FrameworkElement;

            if (null != element && HasTooltipBehavior(element))
            {
                var behavior = GetTooltipBehavior(element);

                behavior.PlacementTarget = current;
            }
        }

        private static void OnTooltipPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var previous = e.OldValue;
            var current = e.NewValue;

            if (previous == current)
            {
                return;
            }

            var element = source as FrameworkElement;

            if (null != element)
            {
                var behavior = GetTooltipBehavior(element);

                behavior.Tooltip = current;
            }
        }
    }
}
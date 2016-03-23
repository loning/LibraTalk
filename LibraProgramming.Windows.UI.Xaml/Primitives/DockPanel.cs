using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public enum Dock
    {
        /// <summary>
        /// 
        /// </summary>
        Left = 0,

        /// <summary>
        /// 
        /// </summary>
        Top = 1,

        /// <summary>
        /// 
        /// </summary>
        Right = 2,

        /// <summary>
        /// 
        /// </summary>
        Bottom = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public class DockPanel : Panel
    {
        public static readonly DependencyProperty LastChildFillProperty;
        public static readonly DependencyProperty DockProperty;

        /// <summary>
        /// 
        /// </summary>
        public bool LastChildFill
        {
            get
            {
                return (bool)GetValue(LastChildFillProperty);
            }
            set
            {
                SetValue(LastChildFillProperty, value);
            }
        }

        public static void SetDock(DependencyObject element, Dock value)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(DockProperty, value);
        }

        public static Dock GetDock(DependencyObject element)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }

            return (Dock)element.GetValue(DockProperty);
        }

        static DockPanel()
        {
            DockProperty = DependencyProperty
                .RegisterAttached(
                    "Dock",
                    typeof (Dock),
                    typeof (DockPanel),
                    new PropertyMetadata(Dock.Left, OnDockPropertyChanged)
                );
            LastChildFillProperty = DependencyProperty
                .Register(
                    "LastChildFill",
                    typeof (bool),
                    typeof (DockPanel),
                    new PropertyMetadata(true, OnLastChildFillPropertyChanged)
                );
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var usedWidth = 0.0D;
            var usedHeight = 0.0D;
            var maximumWidth = 0.0D;
            var maximumHeight = 0.0D;

            foreach (var element in Children)
            {
                var remaining = new Size(
                    Math.Max(0.0D, constraint.Width - usedWidth),
                    Math.Max(0.0D, constraint.Height - usedHeight)
                    );

                element.Measure(remaining);

                var desired = element.DesiredSize;

                switch (GetDock(element))
                {
                    case Dock.Left:
                    case Dock.Right:
                        maximumHeight = Math.Max(maximumHeight, usedHeight + desired.Height);
                        usedWidth += desired.Width;
                        break;

                    case Dock.Top:
                    case Dock.Bottom:
                        maximumWidth = Math.Max(maximumWidth, usedWidth + desired.Width);
                        usedHeight += desired.Height;
                        break;
                }
            }

            maximumWidth = Math.Max(maximumWidth, usedWidth);
            maximumHeight = Math.Max(maximumHeight, usedHeight);

            return new Size(maximumWidth, maximumHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var left = 0.0D;
            var top = 0.0D;
            var right = 0.0D;
            var bottom = 0.0D;
            var dockedCount = Children.Count - (LastChildFill ? 1 : 0);

            var index = 0;

            foreach (var element in Children)
            {
                var remaining = new Rect(
                    left,
                    top,
                    Math.Max(0.0D, finalSize.Width - left - right),
                    Math.Max(0.0D, finalSize.Height - top - bottom)
                    );

                if (index < dockedCount)
                {
                    var desired = element.DesiredSize;

                    switch (GetDock(element))
                    {
                        case Dock.Left:
                            left += desired.Width;
                            remaining.Width = desired.Width;
                            break;

                        case Dock.Top:
                            top += desired.Height;
                            remaining.Height = desired.Height;
                            break;

                        case Dock.Right:
                            right += desired.Width;
                            remaining.X = Math.Max(0.0D, finalSize.Width - right);
                            remaining.Width = desired.Width;
                            break;

                        case Dock.Bottom:
                            bottom += desired.Height;
                            remaining.Y = Math.Max(0.0D, finalSize.Height - bottom);
                            remaining.Height = desired.Height;
                            break;
                    }
                }

                element.Arrange(remaining);
                index++;
            }

            return finalSize;
        }

        private static void OnDockPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement) source;
//            var value = (Dock) e.NewValue;
            (VisualTreeHelper.GetParent(element) as DockPanel)?.InvalidateMeasure();
        }

        private static void OnLastChildFillPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((DockPanel)source).InvalidateArrange();
        }
    }
}

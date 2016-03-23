using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    public sealed class SidePanel : Panel
    {
        internal SideDrawer Drawer
        {
            get;
            set;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var width = 0.0d;
            var height = 0.0d;

            foreach (var element in Children)
            {
                element.Measure(new Size(availableSize.Width, availableSize.Height));

                var size = element.DesiredSize;

                width = Math.Max(size.Height, width);
                height = Math.Max(size.Height, height);
            }

            if (Double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = height;
            }

            if (Double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = width;
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var element in Children)
            {
                element.Arrange(new Rect(new Point(), finalSize));
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}

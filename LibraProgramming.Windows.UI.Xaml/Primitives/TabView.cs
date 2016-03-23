using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using LibraProgramming.Windows.UI.Xaml.Core;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public enum TabOrientation
    {
        /// <summary>
        /// 
        /// </summary>
        Horizontal,

        /// <summary>
        /// 
        /// </summary>
        Vertical
    }

    /// <summary>
    /// 
    /// </summary>
    [ContentProperty(Name = "Tabs")]
    public sealed class TabView : ControlPrimitive
    {
        private const string LayoutGridPartName = "PART_Layout";
        private const string TabsPanelPartName = "PART_TabsPanel";
        private const string ContentPartName = "PART_Content";

        public static readonly DependencyProperty TabOrientationProperty;

        private Grid layout;
        private Panel headers;
        private FrameworkElement contentPresenter;
        private readonly ObservableVector<object> tabs;

        /// <summary>
        /// 
        /// </summary>
        public ICollection<object> Tabs => tabs;

        /// <summary>
        /// 
        /// </summary>
        public TabOrientation TabOrientation
        {
            get
            {
                return (TabOrientation)GetValue(TabOrientationProperty);
            }
            set
            {
                SetValue(TabOrientationProperty, value);
            }
        }

        public TabView()
        {
            DefaultStyleKey = typeof(TabView);
            tabs = new ObservableVector<object>();

            tabs.VectorChanged += OnTabsVectorChanged;
        }

        static TabView()
        {
            TabOrientationProperty = DependencyProperty
                .Register(
                    "TabOrientation",
                    typeof (TabOrientation),
                    typeof (TabView),
                    new PropertyMetadata(TabOrientation.Horizontal, OnTabOrientationPropertyChanged)
                );
        }

        protected override void OnApplyTemplate()
        {
            layout = GetTemplatePart<Grid>(LayoutGridPartName);

            headers = GetTemplatePart<Panel>(TabsPanelPartName);

            contentPresenter = GetTemplatePart<FrameworkElement>(ContentPartName);

            SynchronizeTabsPosition(TabOrientation);

            base.OnApplyTemplate();

            UpdateVisualState(true);

            BindTabsCollection(Tabs);
        }

        private void SynchronizeTabsPosition(TabOrientation orientation)
        {
            switch (orientation)
            {
                case TabOrientation.Horizontal:
                    Grid.SetColumnSpan(headers, 2);
                    headers.ClearValue(Grid.RowSpanProperty);

                    Grid.SetColumn(contentPresenter, 0);
                    Grid.SetColumnSpan(contentPresenter, 2);
                    Grid.SetRow(contentPresenter, 1);

                    headers.Height = 70;
                    break;

                case TabOrientation.Vertical:
                    Grid.SetRowSpan(headers, 2);
                    headers.ClearValue(Grid.ColumnSpanProperty);

                    Grid.SetRow(contentPresenter, 0);
                    Grid.SetRowSpan(contentPresenter, 2);
                    Grid.SetColumn(contentPresenter, 1);

                    headers.Width = 70;
                    break;
            }
        }

        private void BindTabsCollection(IEnumerable source)
        {
            headers.Children.Clear();
            foreach (UIElement tab in source)
            {
                headers.Children.Add(tab);
            }
        }

        private void InsertTabItem(int index, UIElement tab)
        {
            headers.Children.Insert(index, tab);
        }

        private void OnTabsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            if (IsTemplateApplied)
            {
                switch (e.CollectionChange)
                {
                    case CollectionChange.Reset:
                        BindTabsCollection(tabs);
                        break;

                    case CollectionChange.ItemInserted:
                        var element = (UIElement) tabs[(int) e.Index];
                        InsertTabItem((int) e.Index, element);
                        break;
                }
            }
        }

        private void OnTabOrientationChanged(TabOrientation current)
        {
            if (IsTemplateApplied)
            {
                SynchronizeTabsPosition(current);
            }
        }

        private static void OnTabOrientationPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((TabView) source).OnTabOrientationChanged((TabOrientation) e.NewValue);
        }
    }
}

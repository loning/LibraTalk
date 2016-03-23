using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using LibraTalk.Windows.Client.ViewModels.Interfaces;
using LibraTalk.Windows.Client.Views.Interop;

namespace LibraTalk.Windows.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HostPage
    {
        public static readonly DependencyProperty PageTitleProperty;
        public static readonly DependencyProperty HeaderContentProperty;
        public HostPage()
        {
            InitializeComponent();
        }

        static HostPage()
        {
            PageTitleProperty = DependencyProperty
                .RegisterAttached(
                    "PageTitle",
                    typeof (string),
                    typeof (HostPage),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            HeaderContentProperty = DependencyProperty
                .RegisterAttached(
                    "HeaderContent",
                    typeof (object),
                    typeof (HostPage),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        public static void SetPageTitle(DependencyObject element, string value)
        {
            element.SetValue(PageTitleProperty, value);
        }

        public static string GetPageTitle(DependencyObject element)
        {
            return (string) element.GetValue(PageTitleProperty);
        }

        public static void SetHeaderContent(DependencyObject element, object value)
        {
            element.SetValue(HeaderContentProperty, value);
        }

        public static object GetHeaderContent(DependencyObject element)
        {
            return element.GetValue(HeaderContentProperty);
        }

        private async void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var requestor = element?.DataContext as ISetupRequired;

            if (requestor != null)
            {
                await requestor.SetupAsync();
            }
        }

        private async void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var requestor = element?.DataContext as ICleanupRequired;

            if (null != requestor)
            {
                await requestor.CleanupAsync();
            }
        }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            foreach (var button in GetNavigationButtons(MenuHostPanel.Children))
            {
                var command = button.Command as NavigateToPageCommand;
                button.IsChecked = null != command && e.SourcePageType == command.Type;
            }

            var page = e.Content as ContentPage;

            if (null != page)
            {
                var title = GetPageTitle(page);
                var content = GetHeaderContent(page);
                var element = content as FrameworkElement;

                element?.SetBinding(
                    DataContextProperty,
                    new Binding
                    {
                        Source = page,
                        Path = new PropertyPath("DataContext"),
                        Mode = BindingMode.OneWay
                    });

                PageTitle.Text = title ?? String.Empty;
                PageHeaderCustomContent.Content = content;
            }

            MenuSplitView.IsPaneOpen = false;
        }

        private static IEnumerable<RadioButton> GetNavigationButtons(UIElementCollection elements)
        {
            return elements.OfType<RadioButton>();
        }
    }
}

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LibraTalk.Windows.Client.ViewModels.Interfaces;

namespace LibraTalk.Windows.Client.Views
{
    public class ContentPage : Page
    {
        protected ContentPage()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var requestor = DataContext as ISetupRequired;

            if (null != requestor)
            {
                await requestor.SetupAsync();
            }
        }

        protected async void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var requestor = DataContext as ICleanupRequired;

            if (null != requestor)
            {
                await requestor.CleanupAsync();
            }
        }
    }
}
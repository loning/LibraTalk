using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace LibraProgramming.Windows.UI.Xaml.Awaitable
{
    public static class ButtonExtension
    {
        public static async Task<RoutedEventArgs> WaitForClickAsync(this ButtonBase button)
        {
            return await EventAsync.FromRoutedEvent(
                handler => button.Click += handler,
                handler => button.Click -= handler
            );
        }
    }
}
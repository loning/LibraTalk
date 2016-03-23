using Windows.UI;
using Microsoft.Xaml.Interactivity;

namespace LibraProgramming.Windows.UI.Xaml.Interactivity
{
    public interface ITitleBarColorAction : IAction
    {
        Color BackgroundColor
        {
            get;
            set;
        }
    }
}
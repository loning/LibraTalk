using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    public interface IElementAdorner
    {
        /// <summary>
        /// 
        /// </summary>
        FrameworkElement Element
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        void Attach(FrameworkElement element);

        /// <summary>
        /// 
        /// </summary>
        void Release();
    }
}
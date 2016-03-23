using LibraProgramming.Windows.Locator;

namespace LibraTalk.Windows.Client.ViewModels
{
    public class ViewModelLocator
    {
        /// <summary>
        /// 
        /// </summary>
        public HostPageViewModel HostPageViewModel => ServiceLocator.Current.GetInstance<HostPageViewModel>();

        /// <summary>
        /// 
        /// </summary>
        public MainPageViewModel MainPageViewModel => ServiceLocator.Current.GetInstance<MainPageViewModel>();

        /// <summary>
        /// 
        /// </summary>
        public OptionsPageViewModel OptionsPageViewModel => ServiceLocator.Current.GetInstance<OptionsPageViewModel>();
    }
}
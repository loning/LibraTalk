using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace LibraTalk.Windows.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartupPage
    {
        private readonly SplashScreen splashScreen;
        private bool dismissed;

        public StartupPage(SplashScreen splashScreen)
        {
            InitializeComponent();

            Window.Current.SizeChanged += OnWindowSizeChanged;

            this.splashScreen = splashScreen;

            if (null != splashScreen)
            {
                splashScreen.Dismissed += OnSplashScreenDismissed;
                SetImageSize(new Size(splashScreen.ImageLocation.Width, splashScreen.ImageLocation.Height));
            }
        }

        private void SetImageSize(Size size)
        {
            LogoImage.Width = size.Width;
            LogoImage.Height = size.Height;
        }

        private void OnSplashScreenDismissed(SplashScreen sender, object args)
        {
            splashScreen.Dismissed -= OnSplashScreenDismissed;
            dismissed = true;
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (null != splashScreen && !dismissed)
            { 
                SetImageSize(e.Size);
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }
    }
}

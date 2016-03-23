using Windows.ApplicationModel.Resources.Core;
using LibraProgramming.Windows.UI.Xaml.Localization;

namespace LibraTalk.Windows.Client.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ApplicationLocalizationManager : LocalizationManager, IApplicationLocalization
    {
        public const string ApplicationNameResourceKey = "ApplicationName";

        public static readonly ApplicationLocalizationManager Current = new ApplicationLocalizationManager();

        public string ApplicationName => GetString(ApplicationNameResourceKey);

        private ApplicationLocalizationManager()
        {
            // Resources/LibraProgramming/Sample/UI/Xaml/TestEnum/Failed
            DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        }
    }
}
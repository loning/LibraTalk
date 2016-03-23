using Windows.ApplicationModel.Resources.Core;
using LibraProgramming.Windows.UI.Xaml.Localization;

namespace LibraProgramming.Windows.UI.Xaml.Primitives
{
    internal sealed class PrimitivesLocalizationManager : LocalizationManager
    {
        public const string BusyIndicatorContentKey = "BusyIndicatorContentKey";

        public static readonly PrimitivesLocalizationManager Current = new PrimitivesLocalizationManager();

        /// <summary>
        /// 
        /// </summary>
        public string BusyIndicatorContent => GetString(BusyIndicatorContentKey);

        private PrimitivesLocalizationManager()
        {
            DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LibraProgramming.Windows.UI.Xaml/Resources");
        }
    }
}

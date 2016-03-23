using Windows.Web.AtomPub;

namespace LibraProgramming.Windows.UI.Xaml.Core
{
    public interface IObservableCollectionView
    {
        bool CanFilter
        {
            get;
        }

        ICollectionFilter Filter
        {
            get;
        }
    }
}
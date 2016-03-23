namespace LibraProgramming.Windows.UI.Xaml.Core
{
    public interface IObjectBuilder<out TObject>
    {
        TObject Construct();
    }
}
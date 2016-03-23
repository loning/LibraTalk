namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    public interface IActionReference<in TPayload>
    {
        void Invoke(TPayload arg);
    }
}
namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    public interface IPredicateReference<in TPayload>
    {
        bool Invoke(TPayload arg);
    }
}
namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventMessenger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        TEvent GetEvent<TEvent>() where TEvent : EventBase, new();
    }
}
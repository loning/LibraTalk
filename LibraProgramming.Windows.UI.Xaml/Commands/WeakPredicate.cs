using System;

namespace LibraProgramming.Windows.UI.Xaml.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class WeakPredicate<TResult> :WeakDelegateBase, IEquatable<Predicate<TResult>>
    {
        public WeakPredicate(Predicate<TResult> predicate)
            : base(predicate)
        {
        }

        public Predicate<TResult> CreateDelegate()
        {
            return (Predicate<TResult>) CreateDelegate<Predicate<TResult>>();
        }

        public bool Equals(Predicate<TResult> other)
        {
            return base.Equals(other);
        }

        public bool Invoke(TResult value)
        {
            return (bool) CreateDelegate<Predicate<TResult>>().DynamicInvoke(value);
        }
    }
}

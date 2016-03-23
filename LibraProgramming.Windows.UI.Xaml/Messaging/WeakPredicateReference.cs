using System;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    internal class WeakPredicateReference<TPayload> : WeakPredicate<TPayload>, IPredicateReference<TPayload>
    {
        public WeakPredicateReference(Predicate<TPayload> predicate)
            : base(predicate)
        {
        }
    }
}
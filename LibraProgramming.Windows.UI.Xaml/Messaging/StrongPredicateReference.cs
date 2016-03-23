using System;

namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    public class StrongPredicateReference<TPayload> : IPredicateReference<TPayload>
    {
        private readonly Predicate<TPayload> predicate;

        public StrongPredicateReference(Predicate<TPayload> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke(TPayload arg)
        {
            return predicate.Invoke(arg);
        }
    }
}
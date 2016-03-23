using System;

namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {
        private Action<SubscriptionToken> release;

        public SubscriptionToken(Action<SubscriptionToken> release)
        {
            if (null == release)
            {
                throw new ArgumentNullException(nameof(release));
            }

            this.release = release;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SubscriptionToken);
        }

        public bool Equals(SubscriptionToken other)
        {
            if (null == other)
            {
                return false;
            }

            return ReferenceEquals(this, other);
        }

        public void Dispose()
        {
            if (null == release)
            {
                return;
            }

            release.Invoke(this);
            release = null;
        }
    }
}
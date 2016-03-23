using System;

namespace LibraProgramming.Windows.UI.Xaml.Commands
{
    public sealed class WeakDelegate<TDelegate> : WeakDelegateBase, IEquatable<TDelegate>
    {
        public WeakDelegate(Delegate @delegate)
            : base(@delegate)
        {
        }

        public TDelegate CreateDelegate()
        {
            return (TDelegate) (object) CreateDelegate<TDelegate>();
        }

        public bool Equals(TDelegate other)
        {
            return base.Equals(other);
        }

        public void Invoke(params object[] args)
        {
            CreateDelegate<TDelegate>().DynamicInvoke(args);
        }
    }
}

using System;
using System.Reflection;

namespace LibraProgramming.Windows.UI.Xaml.Commands
{
    public class WeakDelegateBase
    {
        private readonly WeakReference reference;
        private readonly MethodInfo method;

        public bool IsAlive => null != reference && reference.IsAlive;

        public WeakDelegateBase(Delegate @delegate)
        {
            if (null == @delegate)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            if (null != @delegate.Target)
            {
                reference = new WeakReference(@delegate.Target);
            }

            method = @delegate.GetMethodInfo();
        }

        public bool Equals(Delegate other)
        {
            return null != other && reference.Target == other.Target && method.Equals(other.GetMethodInfo());
        }

        protected Delegate CreateDelegate<TDelegate>()
        {
            /*if (null != reference)
            {
                return method.CreateDelegate(typeof(TDelegate), reference.Target);
            }

            return method.CreateDelegate(typeof(TDelegate));*/

            if (method.IsStatic)
            {
                return method.CreateDelegate(typeof(TDelegate));
            }

            if (null == reference)
            {
                throw new InvalidOperationException();
            }

            return method.CreateDelegate(typeof(TDelegate), reference.Target);
        }
    }
}
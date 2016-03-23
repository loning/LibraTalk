using System;

namespace LibraProgramming.Windows.UI.Xaml.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class WeakAction : WeakDelegateBase, IEquatable<Action>
    {
        public WeakAction(Action action)
            : base(action)
        {
        }

        public Action CreateDelegate()
        {
            return (Action) CreateDelegate<Action>();
        }

        public bool Equals(Action other)
        {
            return base.Equals(other);
        }

        public void Invoke()
        {
            CreateDelegate<Action>().DynamicInvoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WeakAction<T> : WeakDelegateBase, IEquatable<Action<T>>
    {
        public WeakAction(Action<T> action)
            : base(action)
        {
        }

        public Action<T> CreateDelegate()
        {
            return (Action<T>) CreateDelegate<Action>();
        }

        public bool Equals(Action<T> other)
        {
            return base.Equals(other);
        }

        public void Invoke(T arg)
        {
            CreateDelegate<Action>().DynamicInvoke(arg);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WeakAction<T1, T2> : WeakDelegateBase, IEquatable<Action<T1, T2>>
    {
        public WeakAction(Action<T1, T2> action)
            : base(action)
        {
        }

        public Action<T1, T2> CreateDelegate()
        {
            return (Action<T1, T2>) CreateDelegate<Action>();
        }

        public bool Equals(Action<T1, T2> other)
        {
            return base.Equals(other);
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            CreateDelegate<Action>().DynamicInvoke(arg1, arg2);
        }
    }
}
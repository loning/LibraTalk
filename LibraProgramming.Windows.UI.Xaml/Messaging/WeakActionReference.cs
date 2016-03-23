using System;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraProgramming.Windows.UI.Xaml.Messaging
{
    internal class WeakActionReference<TPayload> : WeakAction<TPayload>, IActionReference<TPayload>
    {
        public WeakActionReference(Action<TPayload> action)
            : base(action)
        {
        }
    }
}
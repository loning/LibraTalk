using System;
using System.Windows.Input;

namespace LibraProgramming.Windows.UI.Xaml.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly WeakDelegate<Action> action;
        private readonly WeakFunc<bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action action, Func<bool> canExecute = null)
        {
            this.action = new WeakDelegate<Action>(action);
            this.canExecute = null != canExecute ? new WeakFunc<bool>(canExecute) : null;
        }

        public bool CanExecute(object unused)
        {
            return null == canExecute || canExecute.Invoke();
        }

        public void Execute(object unused)
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public class RelayCommand<TParam> : ICommand
    {
        private readonly WeakDelegate<Action<TParam>> action;
        private readonly WeakPredicate<TParam> canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<TParam> action, Predicate<TParam> canExecute = null)
        {
            this.action = new WeakDelegate<Action<TParam>>(action);
            this.canExecute = null != canExecute ? new WeakPredicate<TParam>(canExecute) : null;
        }

        public bool CanExecute(object value)
        {
            return null == canExecute || canExecute.Invoke((TParam)value);
        }

        public void Execute(object value)
        {
            action?.Invoke(value);
        }
    }
}
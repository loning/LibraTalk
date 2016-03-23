using System.Windows.Input;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraProgramming.Windows.UI.Xaml.StateMachine
{
    public static class StateMachineExtension
    {
        public static ICommand CreateCommand<TState, TTrigger>(this StateMachine<TState, TTrigger> machine,
            TTrigger trigger)
            where TState : struct
        {
            return new RelayCommand(
                () => machine.Fire(trigger),
                () => machine.CanFire(trigger)
                );
        }
    }
}
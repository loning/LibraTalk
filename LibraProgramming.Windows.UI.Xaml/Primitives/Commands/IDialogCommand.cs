using System.Windows.Input;

namespace LibraProgramming.Windows.UI.Xaml.Primitives.Commands
{
    public interface IDialogCommand
    {
        ICommand Command
        {
            get;
            set;
        }

        DialogCommandDispatcher Dispatcher
        {
            get;
            set;
        }

        void Execute(DialogCommandDispatcher dispatcher);
    }
}
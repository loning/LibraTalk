using LibraProgramming.Windows.UI.Xaml.Primitives.Commands;

namespace LibraProgramming.Windows.UI.Xaml
{
    public class DialogCommandDispatcher
    {
        internal CustomDialog Dialog
        {
            get;
        }

        internal DialogCommandDispatcher(CustomDialog dialog)
        {
            Dialog = dialog;
        }

        public bool CanExecuteCommand(IDialogCommand command)
        {
            return true;
        }

        public void ExecuteCommand(IDialogCommand command)
        {
            command.Execute(this);
        }
    }
}
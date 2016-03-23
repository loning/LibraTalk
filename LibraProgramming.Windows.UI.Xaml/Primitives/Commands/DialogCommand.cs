using System;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.UI.Xaml.Primitives.Commands
{
    public enum DialogAction
    {
        None = -1,
        Close,
        Cancel,
        OK
    }

    public class DialogCommand : DependencyObject, IDialogCommand
    {
        public static readonly DependencyProperty ActionProperty;
        public static readonly DependencyProperty ContentProperty;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty CommandParameterProperty;

        public DialogAction Action
        {
            get
            {
                return (DialogAction) GetValue(ActionProperty);
            }
            set
            {
                SetValue(ActionProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        DialogCommandDispatcher IDialogCommand.Dispatcher
        {
            get;
            set;
        }

        static DialogCommand()
        {
            ActionProperty = DependencyProperty
                .Register(
                    "Action",
                    typeof (DialogAction),
                    typeof (DialogCommand),
                    new PropertyMetadata(DialogAction.None, OnActionPropertyChanged)
                );
            ContentProperty = DependencyProperty
                .Register(
                    "Content",
                    typeof (object),
                    typeof (DialogCommand),
                    new PropertyMetadata(DependencyProperty.UnsetValue/*, OnContentPropertyChanged*/)
                );
            CommandProperty = DependencyProperty
                .Register(
                    "Command",
                    typeof(ICommand),
                    typeof(DialogCommand),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            CommandParameterProperty = DependencyProperty
                .Register(
                    "CommandParameter",
                    typeof(object),
                    typeof(DialogCommand),
                    new PropertyMetadata(null)
                );
        }

        public void Execute(DialogCommandDispatcher dispatcher)
        {
            switch (Action)
            {
                case DialogAction.Cancel:
                case DialogAction.OK:
                    dispatcher.Dialog.Close();
                    break;
            }
        }

        protected virtual bool CanExecuteCommand()
        {
            return ((IDialogCommand)this).Dispatcher.CanExecuteCommand(this);
        }

        protected virtual void OnCommandExecute()
        {
            ((IDialogCommand)this).Dispatcher.ExecuteCommand(this);
        }

        private string GetTitleForAction(DialogAction action)
        {
            var suffix = Enum.GetName(typeof (DialogAction), action);
            var key = $"CustomDialog/DialogCommand/{suffix}";
            return PrimitivesLocalizationManager.Current.GetString(key);
        }

        private void OnActionChanged(DialogAction current, DialogAction previous)
        {
            var title = GetTitleForAction(previous);
            var content = Content as string;

            if (DependencyProperty.UnsetValue == ReadLocalValue(ContentProperty) ||
                (null != content && String.Equals(title, content)))
            {
                Content = GetTitleForAction(current);
            }
        }

        private static void OnActionPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((DialogCommand) source).OnActionChanged((DialogAction) e.NewValue, (DialogAction)e.OldValue);
        }
    }
}
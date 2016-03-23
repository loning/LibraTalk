using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraTalk.Windows.Client.Views.Interop
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigateToPageCommand : DependencyObject, ICommand
    {
        public static readonly DependencyProperty TypeProperty;
        public static readonly DependencyProperty ParameterProperty;
        public static readonly DependencyProperty FrameProperty;

        private readonly WeakEvent<EventHandler> executeChanged;

        public Type Type
        {
            get
            {
                return (Type) GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        public Frame Frame
        {
            get
            {
                return (Frame) GetValue(FrameProperty);
            }
            set
            {
                SetValue(FrameProperty, value);
            }
        }

        public object Parameter
        {
            get
            {
                return GetValue(ParameterProperty);
            }
            set
            {
                SetValue(ParameterProperty, value);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                executeChanged.AddHandler(value);
            }
            remove
            {
                executeChanged.RemoveHandler(value);
            }
        }

        /// <summary>
        /// Provides base class initialization behavior for DependencyObject derived classes.
        /// </summary>
        public NavigateToPageCommand()
        {
            executeChanged = new WeakEvent<EventHandler>();
        }

        static NavigateToPageCommand()
        {
            FrameProperty = DependencyProperty
                .Register(
                    "Frame",
                    typeof (Frame),
                    typeof (NavigateToPageCommand),
                    new PropertyMetadata(null, OnFramePropertyChanged)
                );
            TypeProperty = DependencyProperty
                .Register(
                    "Type",
                    typeof (Type),
                    typeof (NavigateToPageCommand),
                    new PropertyMetadata(null)
                );
            ParameterProperty = DependencyProperty
                .Register(
                    "Parameter",
                    typeof (object),
                    typeof (NavigateToPageCommand),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        /// <summary>
        /// Определяет метод, который определяет, может ли данная команда выполняться в ее текущем состоянии.
        /// </summary>
        /// <returns>
        /// Значение true, если команда может быть выполнена; в противном случае — значение false..
        /// </returns>
        /// <param name="parameter">Данные, используемые данной командой.Если для данной команды не требуется передача данных, можно присвоить этому объекту значение null.</param>
        public bool CanExecute(object parameter)
        {
            var frame = Frame;

            if (null == frame)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Определяет метод, вызываемый при вызове данной команды.
        /// </summary>
        /// <param name="parameter">Данные, используемые данной командой.Если для данной команды не требуется передача данных, можно присвоить этому объекту значение null.</param>
        public void Execute(object parameter)
        {
            var frame = Frame;

            if (null == frame)
            {
                return;
            }

            if (!CanExecute(parameter))
            {
                return;
            }

            frame.Navigate(Type, Parameter);
        }

        private bool CanExecute(Frame frame, object arg)
        {
            if (null == frame)
            {
                return false;
            }

            return true;
        }

        private void OnFrameChanged(Frame previous, Frame current)
        {
            var arg = Parameter;
            var value = CanExecute(previous, arg);

            if (value == CanExecute(current, arg))
            {
                return;
            }

            executeChanged.Invoke(this, EventArgs.Empty);
        }

        private static void OnFramePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((NavigateToPageCommand) source).OnFrameChanged((Frame) e.OldValue, (Frame) e.NewValue);
        }
    }
}
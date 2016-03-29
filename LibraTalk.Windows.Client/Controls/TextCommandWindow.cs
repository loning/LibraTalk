using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using LibraProgramming.Windows.UI.Xaml.Commands;
using LibraProgramming.Windows.UI.Xaml.Primitives;

namespace LibraTalk.Windows.Client.Controls
{
    public enum LogLevel
    {
        Information,
        Warning,
        Error
    }

    public interface IConsole
    {
        void WriteLine(string text);

        void WriteLine(string text, LogLevel level);
    }

    public interface IDeferral
    {
        void Complete();
    }

    public interface IDeferrable
    {
        IDeferral GetDeferral();
    }

    public class ConsoleCommand : DependencyObject
    {
        public static readonly DependencyProperty NameProperty;

        public string Name
        {
            get
            {
                return (string) GetValue(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// Provides base class initialization behavior for DependencyObject derived classes.
        /// </summary>
        static ConsoleCommand()
        {
            NameProperty = DependencyProperty
                .Register(
                    "Name",
                    typeof (string),
                    typeof (ConsoleCommand),
                    new PropertyMetadata(null, OnNamePropertyChanged)
                );
        }

        private static void OnNamePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public sealed class ConsoleCommandCollection : ObservableCollection<ConsoleCommand>
    {
    }

    [TemplatePart(Type = typeof(TextBlock), Name = HistoryTextBlockPartName)]
    [TemplatePart(Type = typeof(TextBox), Name = InputTextBoxPartName)]
    [ContentProperty(Name = "ConsoleCommands")]
    public sealed class TextCommandWindow : ControlPrimitive
    {
        private const string HistoryTextBlockPartName = "PART_HistoryTextBlock";
        private const string InputTextBoxPartName = "PART_InputTextBox";

        public static readonly DependencyProperty InformationTextForegroundProperty;
        public static readonly DependencyProperty ErrorTextForegroundProperty;
        public static readonly DependencyProperty WarningTextForegroundProperty;

        private readonly WeakEvent<EventHandler<ProcessCommandTextEventArgs>> processCommandText;
        private TextBlock history;
        private TextBox input;

        public ConsoleCommandCollection ConsoleCommands
        {
            get;
        }

        public Brush InformationTextForeground
        {
            get
            {
                return (Brush) GetValue(InformationTextForegroundProperty);
            }
            set
            {
                SetValue(InformationTextForegroundProperty, value);
            }
        }

        public Brush ErrorTextForeground
        {
            get
            {
                return (Brush) GetValue(ErrorTextForegroundProperty);
            }
            set
            {
                SetValue(ErrorTextForegroundProperty, value);
            }
        }

        public Brush WarningTextForeground
        {
            get
            {
                return (Brush) GetValue(WarningTextForegroundProperty);
            }
            set
            {
                SetValue(WarningTextForegroundProperty, value);
            }
        }

        public event EventHandler<ProcessCommandTextEventArgs> ProcessCommandText
        {
            add
            {
                processCommandText.AddHandler(value);
            }
            remove
            {
                processCommandText.RemoveHandler(value);
            }
        }

        public TextCommandWindow()
        {
            DefaultStyleKey = typeof (TextCommandWindow);
            processCommandText = new WeakEvent<EventHandler<ProcessCommandTextEventArgs>>();
            ConsoleCommands = new ConsoleCommandCollection();
        }

        static TextCommandWindow()
        {
            InformationTextForegroundProperty = DependencyProperty
                .Register(
                    "InformationTextForeground",
                    typeof (Brush),
                    typeof (TextCommandWindow),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            ErrorTextForegroundProperty = DependencyProperty
                .Register(
                    "ErrorTextForeground",
                    typeof (Brush),
                    typeof (TextCommandWindow),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            WarningTextForegroundProperty = DependencyProperty
                .Register(
                    "WarningTextForeground",
                    typeof (Brush),
                    typeof (TextCommandWindow),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        protected override void OnApplyTemplate()
        {
            if (null != history)
            {
                
            }

            if (null != input)
            {
                
            }

            history = GetTemplatePart<TextBlock>(HistoryTextBlockPartName);
            input = GetTemplatePart<TextBox>(InputTextBoxPartName);
            
            base.OnApplyTemplate();
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += OnCoreWindowKeyDown;

            base.OnLoaded(sender, e);
        }

        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= OnCoreWindowKeyDown;

            base.OnUnloaded(sender, e);
        }

        private void AddLineToConsole(string text, LogLevel level)
        {
            var brushes = new[]
            {
                InformationTextForeground,
                WarningTextForeground,
                ErrorTextForeground
            };

            var line = new Span
            {
                Foreground = brushes[(int) level]
            };

            line.Inlines.Add(new Run
            {
                Text = text
            });
            line.Inlines.Add(new LineBreak());

            history.Inlines.Add(line);
        }

        private void ClearInput()
        {
            input.ClearValue(TextBox.TextProperty);
        }

        private async void OnCoreWindowKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (VirtualKey.Enter == args.VirtualKey && IsFocused)
            {
                var text = input.Text;
                var promises = new Collection<Task>();

                if (!String.IsNullOrEmpty(text))
                {
                    var console = new CachedConsoleProxy();
                    var arg = new ProcessCommandTextEventArgs(console, promises, text);

                    processCommandText.Invoke(this, arg);

                    await Task.WhenAll(promises.ToArray());

                    await PerformConsoleWrite(console);

                    ClearInput();
                }

                args.Handled = true;
            }
        }

        private Task PerformConsoleWrite(CachedConsoleProxy console)
        {
            if (Dispatcher.HasThreadAccess)
            {
                console.WriteLines(this);
                return Task.CompletedTask;
            }

            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => console.WriteLines(this)).AsTask();
        }

        /// <summary>
        /// 
        /// </summary>
        private class Promise : IDeferral
        {
            private readonly ManualResetEventSlim evt;

            public Promise(ManualResetEventSlim evt)
            {
                this.evt = evt;
            }

            public void Complete()
            {
                evt.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class ProcessCommandTextEventArgs : EventArgs, IDeferrable
        {
            private readonly ICollection<Task> promises;

            public IConsole Console
            {
                get;
            }

            public string Text
            {
                get;
            }

            internal ProcessCommandTextEventArgs(IConsole console, ICollection<Task> promises, string text)
            {
                this.promises = promises;
                Console = console;
                Text = text;
            }

            public IDeferral GetDeferral()
            {
                var promise = new ManualResetEventSlim();

                promises.Add(Task.Run(() => promise.Wait()));

                return new Promise(promise);
            }
        }

        private class LinesCollection : Collection<Tuple<LogLevel, string>>
        {
        }

        private class CachedConsoleProxy : IConsole
        {
            private readonly LinesCollection lines;

            public CachedConsoleProxy()
            {
                lines = new LinesCollection();
            }

            public void WriteLines(TextCommandWindow window)
            {
                foreach (var tuple in lines)
                {
                    window.AddLineToConsole(tuple.Item2, tuple.Item1);
                }
            }

            void IConsole.WriteLine(string text)
            {
                lines.Add(new Tuple<LogLevel, string>(LogLevel.Information, text));
            }

            void IConsole.WriteLine(string text, LogLevel level)
            {
                lines.Add(new Tuple<LogLevel, string>(level, text));
            }
        }
    }
}

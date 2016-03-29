using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using LibraProgramming.Windows.UI.Xaml.Commands;
using LibraProgramming.Windows.UI.Xaml.Primitives;
using LibraTalk.Windows.Client.Controls;

namespace LibraTalk.Windows.Client.Controls
{
    public enum LogLevel
    {
        Information,
        Warning,
        Error
    }

    public interface IConsoleOutput
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

    [TemplatePart(Type = typeof (TextBlock), Name = HistoryTextBlockPartName)]
    [TemplatePart(Type = typeof (TextBox), Name = InputTextBoxPartName)]
    [ContentProperty(Name = "CommandProcessor")]
    public sealed class TextCommandWindow : ControlPrimitive
    {
        private const string HistoryTextBlockPartName = "PART_HistoryTextBlock";
        private const string InputTextBoxPartName = "PART_InputTextBox";

        public static readonly DependencyProperty CommandProcessorProperty;
        public static readonly DependencyProperty InformationTextForegroundProperty;
        public static readonly DependencyProperty ErrorTextForegroundProperty;
        public static readonly DependencyProperty WarningTextForegroundProperty;

        private TextBlock history;
        private TextBox input;

        public ConsoleCommandProcessor CommandProcessor
        {
            get
            {
                return (ConsoleCommandProcessor) GetValue(CommandProcessorProperty);
            }
            set
            {
                SetValue(CommandProcessorProperty, value);
            }
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

        public TextCommandWindow()
        {
            DefaultStyleKey = typeof (TextCommandWindow);
        }

        static TextCommandWindow()
        {
            CommandProcessorProperty = DependencyProperty
                .Register(
                    "CommandProcessor",
                    typeof (ConsoleCommandProcessor),
                    typeof (TextCommandWindow),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnCommandProcessorPropertyChanged)
                );
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

                if (String.IsNullOrEmpty(text))
                {
                    return;
                }

                var processor = CommandProcessor;

                if (null == processor)
                {
                    return;
                }

                var console = new CachedConsoleOutputProxy();

                try
                {
                    var result = await processor.TryExecuteAsync(text, console);

                    if (result)
                    {
                        
                    }
                    else
                    {
                        throw new Exception();
                    }

                    ClearInput();
                }
                catch (Exception exception)
                {
                    var output = (IConsoleOutput)console;

                    output.WriteLine("Error: " + text, LogLevel.Error);
                    input.SelectAll();
                }

                await PerformConsoleWrite(console);

                args.Handled = true;
            }
        }

        private Task PerformConsoleWrite(CachedConsoleOutputProxy console)
        {
            if (Dispatcher.HasThreadAccess)
            {
                console.WriteLines(this);
                return Task.CompletedTask;
            }

            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => console.WriteLines(this)).AsTask();
        }

        private static void OnCommandProcessorPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private class LinesCollection : Collection<Tuple<LogLevel, string>>
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private class CachedConsoleOutputProxy : IConsoleOutput
        {
            private readonly LinesCollection lines;

            public CachedConsoleOutputProxy()
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

            void IConsoleOutput.WriteLine(string text)
            {
                lines.Add(new Tuple<LogLevel, string>(LogLevel.Information, text));
            }

            void IConsoleOutput.WriteLine(string text, LogLevel level)
            {
                lines.Add(new Tuple<LogLevel, string>(level, text));
            }
        }
    }
}

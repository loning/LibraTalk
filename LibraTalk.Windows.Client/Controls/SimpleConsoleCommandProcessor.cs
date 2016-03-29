using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraTalk.Windows.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteConsoleCommandEventArgs : EventArgs, IDeferrable
    {
        private readonly IConsoleOutput console;
        private readonly ICollection<Task> promises;

        public ExecuteConsoleCommandEventArgs(IConsoleOutput console, ICollection<Task> promises)
        {
            this.console = console;
            this.promises = promises;
        }

        public IDeferral GetDeferral()
        {
            var promise = new ManualResetEventSlim();

            promises.Add(Task.Run(() => promise.Wait()));

            return new Promise(promise);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConsoleCommandOption : DependencyObject
    {
        public static readonly DependencyProperty OptionNameProperty;

        public string OptionName
        {
            get
            {
                return (string) GetValue(OptionNameProperty);
            }
            set
            {
                SetValue(OptionNameProperty, value);
            }
        }

        static ConsoleCommandOption()
        {
            OptionNameProperty = DependencyProperty
                .Register(
                    "OptionName",
                    typeof (string),
                    typeof (ConsoleCommandOption),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConsoleCommandOptionsCollection : ObservableCollection<ConsoleCommandOption>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [ContentProperty(Name = "Options")]
    public class ConsoleCommand : DependencyObject
    {
        public static readonly DependencyProperty CommandNameProperty;
        public static readonly DependencyProperty MaxArgumentsCountProperty;
        public static readonly DependencyProperty MinArgumentsCountProperty;

        private readonly WeakEvent<TypedEventHandler<ConsoleCommand, ExecuteConsoleCommandEventArgs>> executeCommand;

        public string CommandName
        {
            get
            {
                return (string)GetValue(CommandNameProperty);
            }
            set
            {
                SetValue(CommandNameProperty, value);
            }
        }

        public ConsoleCommandOptionsCollection Options
        {
            get;
        }

        public int MaxArgumentsCount
        {
            get
            {
                return (int) GetValue(MaxArgumentsCountProperty);
            }
            set
            {
                SetValue(MaxArgumentsCountProperty, value);
            }
        }

        public int MinArgumentsCount
        {
            get
            {
                return (int) GetValue(MinArgumentsCountProperty);
            }
            set
            {
                SetValue(MinArgumentsCountProperty, value);
            }
        }

        public event TypedEventHandler<ConsoleCommand, ExecuteConsoleCommandEventArgs> ExecuteCommand
        {
            add
            {
                executeCommand.AddHandler(value);
            }
            remove
            {
                executeCommand.RemoveHandler(value);
            }
        }

        public ConsoleCommand()
        {
            executeCommand = new WeakEvent<TypedEventHandler<ConsoleCommand, ExecuteConsoleCommandEventArgs>>();
            Options = new ConsoleCommandOptionsCollection();
        }

        /// <summary>
        /// Provides base class initialization behavior for DependencyObject derived classes.
        /// </summary>
        static ConsoleCommand()
        {
            CommandNameProperty = DependencyProperty
                .Register(
                    "CommandName",
                    typeof(string),
                    typeof(ConsoleCommand),
                    new PropertyMetadata(null)
                );
            MaxArgumentsCountProperty = DependencyProperty
                .Register(
                    "MaxArgumentsCount",
                    typeof (int),
                    typeof (ConsoleCommand),
                    new PropertyMetadata(0)
                );
            MinArgumentsCountProperty = DependencyProperty
                .Register(
                    "MinArgumentsCount",
                    typeof (int),
                    typeof (ConsoleCommand),
                    new PropertyMetadata(0)
                );
        }

        public async Task<bool> ExecuteAsync(IConsoleOutput output)
        {
            var promises = new Collection<Task>();
            var args = new ExecuteConsoleCommandEventArgs(output, promises);

            executeCommand.Invoke(this, args);

            await Task.WhenAll(promises.ToArray());

            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConsoleCommandCollection : ObservableCollection<ConsoleCommand>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [ContentProperty(Name = "Commands")]
    public class SimpleConsoleCommandProcessor : ConsoleCommandProcessor
    {
        private readonly StringComparer comparer;

        public ConsoleCommandCollection Commands
        {
            get;
        }

        public SimpleConsoleCommandProcessor()
        {
            comparer = StringComparer.OrdinalIgnoreCase;
            Commands = new ConsoleCommandCollection();
        }

        public override Task<bool> TryExecuteAsync(string text, IConsoleOutput output)
        {
            var parser = new CommandTextParser();

            try
            {
                string name;
                var options = new Collection<Tuple<string, string>>();
                var arguments = new Collection<string>();

                if (!parser.Parse(text, out name, options, arguments))
                {
                    throw new Exception();
                }

                foreach (var command in Commands)
                {
                    if (!comparer.Equals(command.CommandName, name))
                    {
                        continue;
                    }

                    if (ValidateCommand(command, options, arguments))
                    {
                        return command.ExecuteAsync(output);
                    }
                }

                return Task.FromResult(false);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        private bool ValidateCommand(ConsoleCommand command, ICollection<Tuple<string, string>> options, ICollection<string> arguments)
        {
            bool isArgumentsOk;
            var argumentsCount = arguments.Count;

            if (0 < argumentsCount)
            {
                isArgumentsOk = command.MaxArgumentsCount >= argumentsCount &&
                                argumentsCount >= command.MinArgumentsCount;
            }
            else
            {
                isArgumentsOk = 0 == command.MaxArgumentsCount && 0 == command.MinArgumentsCount;
            }

            return isArgumentsOk && options.All(candidate => command.Options.Any(option => option.OptionName == candidate.Item1));
        }

        /// <summary>
        /// 
        /// </summary>
        private class CommandTextParser
        {
            public bool Parse(string text, out string name, ICollection<Tuple<string, string>> options, ICollection<string> arguments)
            {
                if (null == text)
                {
                    throw new ArgumentNullException(nameof(text));
                }

                name = null;

                if (String.IsNullOrEmpty(text))
                {
                    return false;
                }

                var parsingstate = ParsingState.CommandNameBegin;
                var ok = true;

                using (var reader = new StringReader(text))
                {
                    var command = new StringBuilder();

                    while (ok)
                    {
                        switch (parsingstate)
                        {
                            case ParsingState.CommandNameBegin:
                            {
                                var current = reader.Read();

                                if (-1 == current)
                                {
                                    ok = false;
                                    continue;
                                }

                                if (Char.IsLetter((char) current) || '_' == current)
                                {
                                    command.Append((char) current);
                                    parsingstate = ParsingState.CommandNameContinues;

                                    continue;
                                }

                                if (Char.IsWhiteSpace((char) current))
                                {
                                    continue;
                                }

                                throw new Exception();
                            }

                            case ParsingState.CommandNameContinues:
                            {
                                var current = reader.Read();

                                if (-1 == current)
                                {
                                    parsingstate = ParsingState.CommandNameEnds;
                                    name = command.ToString();
                                    continue;
                                }

                                if (Char.IsLetterOrDigit((char) current) || '_' == current || '-' == current)
                                {
                                    command.Append((char) current);
                                    continue;
                                }

                                if (Char.IsWhiteSpace((char) current))
                                {
                                    parsingstate = ParsingState.CommandNameEnds;
                                    name = command.ToString();
                                    continue;
                                }

                                throw new Exception();
                            }

                            case ParsingState.CommandNameEnds:
                            {
                                var current = reader.Read();

                                if (-1 == current)
                                {
                                    ok = false;
                                    continue;
                                }

                                if ('-' == current || '/' == current)
                                {
                                    parsingstate = ParsingState.CommandOptionNameBegin;
                                    command.Clear();

                                    continue;
                                }

                                if (Char.IsWhiteSpace((char) current))
                                {
                                    continue;
                                }

                                parsingstate = ParsingState.CommandArguments;

                                continue;
                            }

                            case ParsingState.CommandArguments:
                            {
                                var current = reader.Read();

                                if (-1 == current)
                                {
                                    ok = false;
                                    continue;
                                }

                                command.Clear();

                                if ('\"' == current)
                                {
                                    parsingstate = ParsingState.CommandArgumentQuoted;
                                    continue;
                                }

                                parsingstate = ParsingState.CommandArgumentSingleWord;
                                command.Append((char) current);

                                continue;
                            }

                            case ParsingState.CommandArgumentQuoted:
                            {
                                var current = reader.Read();

                                if (-1 == current)
                                {
                                    ok = false;
                                    continue;
                                }

                                if ('\"' == current)
                                {
                                    parsingstate = ParsingState.CommandArgumentQuotedPossibleEnd;
                                    continue;
                                }

                                command.Append((char) current);

                                continue;
                            }

                            case ParsingState.CommandArgumentQuotedPossibleEnd:
                            {
                                var current = reader.Read();

                                if ('\"' == current)
                                {
                                    parsingstate = ParsingState.CommandArgumentQuoted;
                                    command.Append((char) current);

                                    continue;
                                }

                                arguments.Add(command.ToString());

                                if (-1 == current)
                                {
                                    ok = false;
                                    continue;
                                }

                                parsingstate = ParsingState.CommandArguments;

                                break;
                            }

                            case ParsingState.CommandArgumentSingleWord:
                            {
                                var current = reader.Read();

                                if (-1 == current)
                                {
                                    arguments.Add(command.ToString());
                                    ok = false;

                                    continue;
                                }

                                if (Char.IsWhiteSpace((char) current))
                                {
                                    parsingstate = ParsingState.CommandArguments;
                                    continue;
                                }

                                command.Append((char) current);

                                continue;
                            }
                        }
                    }
                }

                return false == String.IsNullOrEmpty(name);
            }

            /// <summary>
            /// 
            /// </summary>
            private enum ParsingState
            {
                CommandNameBegin,
                CommandNameContinues,
                CommandNameEnds,
                CommandOptionNameBegin,
                CommandArguments,
                CommandArgumentQuoted,
                CommandArgumentSingleWord,
                CommandArgumentQuotedPossibleEnd,
            }
        }
    }
}
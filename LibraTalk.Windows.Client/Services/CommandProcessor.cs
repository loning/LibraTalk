using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraTalk.Windows.Client.Services
{
    public class CommandDescription
    {
        public string Name
        {
            get;
            set;
        }

        public Func<string, object, Task> Action
        {
            get;
            set;
        }
    }

    public class CommandProcessor
    {
        private readonly StringComparer comparer;
        private readonly IList<CommandDescription> commands;

        public CommandProcessor()
        {
            comparer = StringComparer.OrdinalIgnoreCase;
            commands = new List<CommandDescription>();
        }

        public void Configure(IEnumerable<CommandDescription> value)
        {
            foreach (var description in value)
            {
                commands.Add(description);
            }
        }

        public async Task<bool> Execute(string text, object state)
        {
            if (null == text)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (String.IsNullOrEmpty(text))
            {
                return false;
            }

            var pasingstate = ParsingState.CommandNameBegin;
            var ok = true;

            using (var reader = new StringReader(text))
            {
                var command = new StringBuilder();

                while (ok)
                {
                    switch (pasingstate)
                    {
                        case ParsingState.CommandNameBegin:
                        {
                            var current = reader.Read();

                            if (-1 == current)
                            {
                                ok = false;
                                continue;
                            }

                            if (Char.IsLetter((char) current))
                            {
                                command.Append((char) current);
                                pasingstate = ParsingState.CommandNameContinues;

                                continue;
                            }

                            break;
                        }

                        case ParsingState.CommandNameContinues:
                        {
                            var current = reader.Read();

                            if (-1 == current)
                            {
                                pasingstate = ParsingState.CommandNameEnds;
                                continue;
                            }

                            if (Char.IsLetterOrDigit((char) current)||'_'==current||'-'==current)
                            {
                                command.Append((char) current);
                                continue;
                            }

                            pasingstate = ParsingState.CommandNameEnds;

                            break;
                        }

                        case ParsingState.CommandNameEnds:
                        {
                            var description = FindCommand(command.ToString());

                            if (null == description)
                            {
                                pasingstate = ParsingState.CommandNotFound;
                                continue;
                            }

                            var tail = reader.ReadToEnd();

                            await description.Action.Invoke(tail, state);

                            return true;
                        }

                        case ParsingState.CommandNotFound:
                        {
                            ok = false;

                            break;
                        }
                    }
                }
            }

            return false;
        }

        private CommandDescription FindCommand(string command)
        {
            return commands.FirstOrDefault(description => comparer.Equals(description.Name, command));
        }

        private enum ParsingState
        {
            CommandNotFound = -1,
            CommandNameBegin,
            CommandNameContinues,
            CommandNameEnds
        }
    }
}
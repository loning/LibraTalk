using System.Threading.Tasks;

namespace LibraTalk.Windows.Client.Controls
{
    public sealed class EchoConsoleCommandProcessor : ConsoleCommandProcessor
    {
        public override Task<bool> TryExecuteAsync(string text, IConsoleOutput output)
        {
            output.WriteLine(text);
            return Task.FromResult(true);
        }
    }
}
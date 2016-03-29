using System.Threading.Tasks;

namespace LibraTalk.Windows.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ConsoleCommandProcessor
    {
        public abstract Task<bool> TryExecuteAsync(string text, IConsoleOutput output);
    }
}
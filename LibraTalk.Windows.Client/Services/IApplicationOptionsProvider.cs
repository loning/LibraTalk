using System;
using System.Threading;
using System.Threading.Tasks;
using LibraTalk.Windows.Client.Models;

namespace LibraTalk.Windows.Client.Services
{
    public interface IApplicationOptionsProvider
    {
        Task<ApplicationOptions> GetOptionsAsync(TimeSpan timeout, CancellationToken token);
    }
}
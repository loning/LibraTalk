using System;
using System.Threading;
using System.Threading.Tasks;
using LibraTalk.Windows.Client.Models;

namespace LibraTalk.Windows.Client.Services
{
    public enum StorageLocation
    {
        Local,
        Roaming
    }

    public sealed class ApplicationOptionsProvider : IApplicationOptionsProvider
    {
        private readonly StorageLocation location;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        public ApplicationOptionsProvider(StorageLocation location)
        {
            this.location = location;
        }

        public Task<ApplicationOptions> GetOptionsAsync(TimeSpan timeout, CancellationToken token)
        {
            return Task.FromResult(new ApplicationOptions());
        }
    }
}
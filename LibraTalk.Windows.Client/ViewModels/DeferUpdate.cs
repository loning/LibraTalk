using System;
using LibraTalk.Windows.Client.Properties;
using LibraTalk.Windows.Client.ViewModels.Interfaces;

namespace LibraTalk.Windows.Client.ViewModels
{
    internal sealed class DeferUpdate : IDisposable
    {
        private readonly IUpdateIndicator indicator;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
        /// </summary>
        public DeferUpdate([NotNull] IUpdateIndicator indicator)
        {
            this.indicator = indicator;
            indicator.BeginUpdate();
        }

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            indicator.EndUpdate();
        }
    }
}
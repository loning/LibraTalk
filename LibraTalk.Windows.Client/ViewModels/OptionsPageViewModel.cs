using System;
using System.Threading;
using System.Threading.Tasks;
using LibraTalk.Windows.Client.Services;
using LibraTalk.Windows.Client.ViewModels.Interfaces;

namespace LibraTalk.Windows.Client.ViewModels
{
    public class OptionsPageViewModel : ObservableViewModel, ISetupRequired, IUpdateIndicator
    {
        private readonly IApplicationOptionsProvider provider;
        private readonly CancellationTokenSource cts;
        private bool isDataLoading;
        private bool invalidated;

        public bool IsDataLoading
        {
            get
            {
                return isDataLoading;
            }
            set
            {
                SetProperty(ref isDataLoading, value);
            }
        }

        public OptionsPageViewModel(IApplicationOptionsProvider provider)
        {
            this.provider = provider;
            cts = new CancellationTokenSource();
        }

        async Task ISetupRequired.SetupAsync()
        {
            using (new DeferUpdate(this))
            {
                var options = await provider.GetOptionsAsync(TimeSpan.Zero, cts.Token);
            }


        }

        public void Invalidate()
        {
            invalidated = true;
        }

        void IUpdateIndicator.BeginUpdate()
        {
            IsDataLoading = true;
        }

        void IUpdateIndicator.EndUpdate()
        {
            IsDataLoading = false;
        }
    }
}
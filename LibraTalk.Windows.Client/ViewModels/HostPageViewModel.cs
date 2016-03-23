using System.Threading.Tasks;
using LibraTalk.Windows.Client.ViewModels.Interfaces;

namespace LibraTalk.Windows.Client.ViewModels
{
    public class HostPageViewModel : ObservableViewModel, ISetupRequired, ICleanupRequired
    {
        Task ISetupRequired.SetupAsync()
        {
            return Task.CompletedTask;
        }

        Task ICleanupRequired.CleanupAsync()
        {
            return Task.CompletedTask;
        }
    }
}
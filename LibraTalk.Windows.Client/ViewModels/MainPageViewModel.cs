using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using LibraProgramming.Windows.UI.Xaml.Commands;
using LibraProgramming.Windows.UI.Xaml.Dependency.Tracking;
using LibraProgramming.Windows.UI.Xaml.StateMachine;
using LibraTalk.Windows.Client.Localization;
using LibraTalk.Windows.Client.Services;
using LibraTalk.Windows.Client.ViewModels.Interfaces;

namespace LibraTalk.Windows.Client.ViewModels
{
    public enum TalkActions
    {
        Send,
        Complete
    }

    public enum TalkStates
    {
        EnteringText,
        SendingText
    }

    public class MainPageViewModel : ObservableViewModel, ISetupRequired, ICleanupRequired, IUpdateIndicator
    {
        private readonly IUserProvider userProvider;
//        private static readonly IDependencyTracker<MainPageViewModel> tracker;

//        private readonly IDisposable subscription;
        private readonly IApplicationLocalization localization;
        private readonly StateMachine<TalkStates, TalkActions> machine;
        private CancellationTokenSource cts;
        private bool isDataLoading;
        private string message;
        private string userName;

        public ObservableCollection<string> Messages
        {
            get;
        }

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

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                SetProperty(ref userName, value);
            }
        }

        public AsynchronousCommand<string> ChangeUserName
        {
            get;
        } 
        
        public AsynchronousCommand<string> Send
        {
            get;
        }

        public MainPageViewModel(IUserProvider userProvider)
        {
            this.userProvider = userProvider;
//            subscription = tracker.Subscribe(this);
            machine = new StateMachine<TalkStates, TalkActions>(TalkStates.EnteringText);
            machine.Configure(TalkStates.EnteringText)
                .Permit(TalkActions.Send, TalkStates.SendingText);
            machine.Configure(TalkStates.SendingText)
                .Ignore(TalkActions.Send)
                .Permit(TalkActions.Complete, TalkStates.EnteringText);
            Messages = new ObservableCollection<string>();
        }

        static MainPageViewModel()
        {
            /*tracker = DependencyTracker.Create<MainPageViewModel>(builder =>
            {
                builder.RaiseProperty(target => target.TotalCost)
                    .CalculatedBy(model => (double)(model.Price * model.Count))
                    .WhenPropertyChanged(model => model.Price)
                    .WhenPropertyChanged(model => model.Count);
            });*/
        }

        async Task ISetupRequired.SetupAsync()
        {
            using (new DeferUpdate(this))
            {
                var id = GetUserId();

//                UserName = await UserProvider.GetUserNameAsync(id);

//                UserProvider.Receive();
            }
        }

        Task ICleanupRequired.CleanupAsync()
        {
//            subscription.Dispose();
            return Task.CompletedTask;
        }

        void IUpdateIndicator.BeginUpdate()
        {
            IsDataLoading = true;
        }

        void IUpdateIndicator.EndUpdate()
        {
            IsDataLoading = false;
        }

        private static Guid GetUserId()
        {
            const string key = "Chat.UserName";

            Guid id;

            var value = ApplicationData.Current.LocalSettings.Values[key];

            if (null == value)
            {
                id = Guid.NewGuid();
                ApplicationData.Current.LocalSettings.Values[key] = id;
            }
            else
            {
                id = (Guid) value;
            }

            return id;
        }

/*
        private async Task DoSendMessage(string text)
        {
            var message = new Dictionary<string, string>
            {
                {
                    "message", text
                }
            };

            await UserProvider.SendMessageAsync(message);

            machine.Fire(TalkActions.Complete);
        }
*/

/*
        private void OnMessageReceived(IUserProvider sender, ReceivingMessageEventArgs args)
        {
        }
*/
    }
}
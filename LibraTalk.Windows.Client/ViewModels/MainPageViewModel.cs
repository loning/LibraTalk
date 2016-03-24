using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private readonly IMessageSender messageSender;
//        private static readonly IDependencyTracker<MainPageViewModel> tracker;

//        private readonly IDisposable subscription;
        private readonly IApplicationLocalization localization;
        private readonly StateMachine<TalkStates, TalkActions> machine;
        private CancellationTokenSource cts;
        private bool isDataLoading;
        private string message;

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
        
        public AsynchronousCommand<string> Send
        {
            get;
        }

        public MainPageViewModel(IMessageSender messageSender)
        {
            this.messageSender = messageSender;
//            subscription = tracker.Subscribe(this);
            machine = new StateMachine<TalkStates, TalkActions>(TalkStates.EnteringText);
            machine.Configure(TalkStates.EnteringText)
                .Permit(TalkActions.Send, TalkStates.SendingText);
            machine.Configure(TalkStates.SendingText)
                .Ignore(TalkActions.Send)
                .Permit(TalkActions.Complete, TalkStates.EnteringText);
            Messages = new ObservableCollection<string>();
            Send = new AsynchronousCommand<string>(DoSendMessage, text => machine.CanFire(TalkActions.Send));
            messageSender.MessageReceived += OnMessageReceived;
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

        Task ISetupRequired.SetupAsync()
        {
            using (new DeferUpdate(this))
            {
                messageSender.Receive();
            }

            return Task.CompletedTask;
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

        private async Task DoSendMessage(string text)
        {
            var message = new Dictionary<string, string>
            {
                {
                    "message", text
                }
            };

            await messageSender.SendMessageAsync(message);

            machine.Fire(TalkActions.Complete);
        }

        private void OnMessageReceived(IMessageSender sender, ReceivingMessageEventArgs args)
        {
            messageSender.Receive();
        }
    }
}
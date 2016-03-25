using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using LibraProgramming.Windows.Locator;
using LibraTalk.Windows.Client.Controls;
using LibraTalk.Windows.Client.Services;
using LibraTalk.Windows.Client.ViewModels;

namespace LibraTalk.Windows.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly CommandProcessor processor;
        private readonly IMessageSender messagesender;

        public MainPage()
        {
            messagesender = ServiceLocator.Current.GetInstance<IMessageSender>();
            processor = new CommandProcessor();
            InitializeComponent();
        }

        private async void OnTextCommandWindowProcessCommandText(object sender, TextCommandWindow.ProcessCommandTextEventArgs e)
        {
            var deferral = e.GetDeferral();
            var model = DataContext as MainPageViewModel;

            try
            {
                if (null == model)
                {
                    return;
                }

                var success = await processor.Execute(e.Text, e.Console);

                if (!success)
                {
                    e.Console.WriteLine("Error", LogLevel.Error);
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            processor.Configure(new List<CommandDescription>
            {
                new CommandDescription
                {
                    Name = "get-name",
                    Action = GetUserName
                },
                new CommandDescription
                {
                    Name = "set-name",
                    Action = SetUserName
                }
            });
        }

        private async Task GetUserName(string arg, object state)
        {
            var id = GetUserId();
            var console = (IConsole) state;
            var username = await messagesender.GetUserNameAsync(id);

            console.WriteLine(String.Format("User: \"{0}\"", username), LogLevel.Information);
        }

        private async Task SetUserName(string arg, object state)
        {
            var id = GetUserId();
            var console = (IConsole)state;

            await messagesender.SetUserName(id, arg);

            console.WriteLine("Ok", LogLevel.Information);
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
                id = (Guid)value;
            }

            return id;
        }
    }
}

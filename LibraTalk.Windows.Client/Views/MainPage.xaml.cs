using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using LibraProgramming.Windows.Locator;
using LibraTalk.Windows.Client.Controls;
using LibraTalk.Windows.Client.Models;
using LibraTalk.Windows.Client.Services;
using LibraTalk.Windows.Client.ViewModels;

namespace LibraTalk.Windows.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly IUserProvider userProvider;
        private Profile profile;

        public MainPage()
        {
            userProvider = ServiceLocator.Current.GetInstance<IUserProvider>();
            InitializeComponent();
        }

/*
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
*/

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            /*processor.Configure(new List<CommandDescription>
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
            })*/;
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

        private async void OnGetUserProfile(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (null == profile || args.Options.Any(option => "force" == option.Item1))
            {
                profile = await userProvider.GetProfileAsync(GetUserId());
                args.Console.WriteLine("Profile retrieved", LogLevel.Information);
            }
            else
            {
                args.Console.WriteLine("Profile cached", LogLevel.Information);
            }

            args.Console.WriteLine(String.Format("Name: \"{0}\"", profile.Name), LogLevel.Information);
            args.Console.WriteLine(String.Format("Id: \"{0}\"", profile.Id), LogLevel.Information);

            deferral.Complete();
        }

        private async void OnWriteUserProfile(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

            await userProvider.SetProfileAsync(GetUserId(), profile);
            args.Console.WriteLine("Write-Profile: Ok");

            deferral.Complete();
        }

        private void OnSetUserProfile(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (null == profile)
            {
                args.Console.WriteLine("Error: Get-Profile sould be executed first.", LogLevel.Error);
            }
            else
            {
                var arg = args.Arguments.First();

                if (args.Options.Any(option => "name" == option.Item1))
                {
                    profile.Name = arg;
                    args.Console.WriteLine("Set-Profile: Name updated");
                }
                else if (args.Options.Any(option => "id" == option.Item1))
                {
                    profile.Id = Guid.Parse(arg);
                    args.Console.WriteLine("Set-Profile: Id updated");
                }
            }

            deferral.Complete();
        }
    }
}

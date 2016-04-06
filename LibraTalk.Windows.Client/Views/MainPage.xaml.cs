using System;
using System.Linq;
using System.Threading;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using LibraProgramming.Communication.Protocol.Packets;
using LibraTalk.Windows.Client.Controls;
using LibraTalk.Windows.Client.Models;
using LibraTalk.Windows.Client.Services;

namespace LibraTalk.Windows.Client.Views
{
    /// <summary>An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private CancellationTokenSource cts;
        private readonly SocketCommunicationService service;

        public MainPage()
        {
            service = new SocketCommunicationService(new Uri("ws://localhost:1607/api/nexus"));
            service.PacketReceived += OnCommunicationServicePacketReceived;
            InitializeComponent();
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

        private void OnGetUserProfile(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
//            var deferral = args.GetDeferral();

            service.WhoAmI(GetUserId());

//            deferral.Complete();
        }

        private async void OnWriteUserProfile(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

//            await userProvider.SetProfileAsync(profile);
            args.Console.WriteLine("Write-Profile: Ok", LogLevel.Success);

            deferral.Complete();
        }

        private void OnSetUserProfile(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            /*var deferral = args.GetDeferral();

            if (null == profile)
            {
                args.Console.WriteLine("Set-Profile: Get-Profile command sould be executed first.", LogLevel.Error);
            }
            else
            {
                var arg = args.Arguments.First();

                if (args.Options.Any(option => "name" == option.Item1))
                {
                    profile.Name = arg;
                    args.Console.WriteLine("Set-Profile: Name updated", LogLevel.Success);
                }
                else if (args.Options.Any(option => "id" == option.Item1))
                {
                    profile.Id = Guid.Parse(arg);
                    args.Console.WriteLine("Set-Profile: Id updated", LogLevel.Success);
                }
            }

            deferral.Complete();*/
        }

        private async void OnPublishMessage(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (0 <= args.Arguments.Count)
            {
//                await userProvider.PublishMessageAsync(args.Arguments.First());
                args.Console.WriteLine("Publish-Message: Ok", LogLevel.Success);
            }
            else
            {
                args.Console.WriteLine("Publish-Message: Unknown error.", LogLevel.Error);
            }

            deferral.Complete();
        }

        private async void OnJoinRoomMessage(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (0 <= args.Arguments.Count)
            {
//                await userProvider.JoinRoomAsync(args.Arguments.First());
                args.Console.WriteLine("Join-Room: Ok", LogLevel.Success);
            }
            else
            {
                args.Console.WriteLine("Join-Room: Unknown error.", LogLevel.Error);
            }

            deferral.Complete();
        }

        private void OnPollRoomMessage(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (args.Options.Any(option => "disable" == option.Item1))
            {
                if (null == cts)
                {
                    args.Console.WriteLine("Poll-Room: Polling not started.", LogLevel.Error);
                }
                else
                {
                    cts.Cancel();
                    args.Console.WriteLine("Poll-Room: Cancel requested.", LogLevel.Information);
                }
            }
            else if (args.Options.Any(option => "enable" == option.Item1))
            {
//                cts = new CancellationTokenSource();
//                userProvider.Poll(cts.Token);
                args.Console.WriteLine("Poll-Room: Polling started.", LogLevel.Success);
            }

            deferral.Complete();
        }

        private async void OnMessageReceived(UserProvider sender, ReceivingMessageEventArgs args)
        {
            var print = new DispatchedHandler(() =>
            {
                foreach (var message in args.Messages)
                {
                    CommandWindow.WriteLine(message.PublisherNick + " say:", LogLevel.Information);
                    CommandWindow.WriteLine(message.Text, LogLevel.Information);
                }
            });

            if (Dispatcher.HasThreadAccess)
            {
                print.Invoke();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, print);
            }
        }

        private async void OnPollingCancelled(UserProvider sender, PollingCancelledEventArgs args)
        {
            var print = new DispatchedHandler(() =>
            {
                    CommandWindow.WriteLine("Poll-Room: Room polling stopped", LogLevel.Information);
            });

            if (Dispatcher.HasThreadAccess)
            {
                print.Invoke();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, print);
            }
        }

        private void OnClearMessage(ConsoleCommand sender, ExecuteConsoleCommandEventArgs args)
        {
            var deferral = args.GetDeferral();

            args.Console.Clear();

            deferral.Complete();
        }

        private async void OnCommunicationServicePacketReceived(SocketCommunicationService sender, PacketReceivedEventArgs args)
        {
            var packet = args.Packet;

            if (PacketType.Profile == packet.PacketType)
            {
                var profile = (ProfileResponsePacket) args.Packet;

                var print = new DispatchedHandler(() =>
                {
                    CommandWindow
                        .WriteLine(
                            String.Format("Who-am-i: ({0:D})\"{1}\"", profile.UserId, profile.UserName),
                            LogLevel.Information
                        );
                });

                if (Dispatcher.HasThreadAccess)
                {
                    print.Invoke();
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, print);
                }
            }
        }

        private async void OnContentPageLoaded(object sender, RoutedEventArgs e)
        {
//            var temp = SocketActivityInformation.AllSockets;
            await service.ConnectAsync();
        }
    }
}

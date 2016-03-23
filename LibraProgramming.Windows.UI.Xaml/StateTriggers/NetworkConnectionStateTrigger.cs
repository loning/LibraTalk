using System;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Xaml;
using LibraProgramming.Windows.UI.Xaml.Commands;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
    public enum ConnectionState
    {
        Connected,
        Disconnected
    }

    public class NetworkConnectionStateTrigger : CustomStateTrigger
    {
        public static readonly DependencyProperty ConnectionStateProperty;

        /// <summary>
        /// Gets or sets required connection state for the trigger to activate.
        /// </summary>
        public ConnectionState ConnectionState
        {
            get
            {
                return (ConnectionState) GetValue(ConnectionStateProperty);
            }
            set
            {
                SetValue(ConnectionStateProperty, value);
            }
        }

        public NetworkConnectionStateTrigger()
        {
            WeakEventListener
                .AttachEvent<object>(
                    handler => NetworkInformation.NetworkStatusChanged += new NetworkStatusChangedEventHandler(handler),
                    handler => NetworkInformation.NetworkStatusChanged -= new NetworkStatusChangedEventHandler(handler),
                    OnNetworkStatusChanged
                );
        }

        static NetworkConnectionStateTrigger()
        {
            ConnectionStateProperty = DependencyProperty
                .Register(
                    "ConnectionState",
                    typeof (ConnectionState),
                    typeof (NetworkConnectionStateTrigger),
                    new PropertyMetadata(ConnectionState.Connected, OnConnectionStatePropertyChanged)
                );
        }

        private async void OnNetworkStatusChanged(object sender)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateTrigger);
        }

        private void UpdateTrigger()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            var hasInternet = null != profile && NetworkConnectivityLevel.InternetAccess == profile.GetNetworkConnectivityLevel();

            if (hasInternet)
            {
                IsActive = ConnectionState.Connected == ConnectionState;
            }
            else
            {
                IsActive = ConnectionState.Disconnected == ConnectionState;
            }
        }

        private static void OnConnectionStatePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((NetworkConnectionStateTrigger) source).UpdateTrigger();
        }
    }
}
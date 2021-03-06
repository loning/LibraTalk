﻿using System;
using LibraProgramming.Windows.Locator;
using LibraTalk.Windows.Client.Localization;
using LibraTalk.Windows.Client.Services;
using LibraTalk.Windows.Client.ViewModels;

namespace LibraTalk.Windows.Client.Bootstraps
{
    internal static class ServiceBootstrap
    {
        public static void Register(ServiceLocator services)
        {
            services.Register<IApplicationLocalization, ApplicationLocalizationManager>(lifetime: InstanceLifetime.Singleton);
            services.Register<IApplicationOptionsProvider>(() => new ApplicationOptionsProvider(StorageLocation.Local), lifetime: InstanceLifetime.Singleton);
            services.Register<OptionsPageViewModel>(lifetime: InstanceLifetime.CreateNew);
            services.Register<HostPageViewModel>(lifetime: InstanceLifetime.CreateNew);
        }
    }
}
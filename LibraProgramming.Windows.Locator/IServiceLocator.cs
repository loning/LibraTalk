using System;

namespace LibraProgramming.Windows.Locator
{
    public interface IServiceLocator : IServiceProvider
    {
        object GetInstance(Type serviceType, string key = null);

        TService GetInstance<TService>(string key = null);
    }
}
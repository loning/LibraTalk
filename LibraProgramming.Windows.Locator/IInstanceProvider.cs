using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Locator
{
    public interface IInstanceProvider
    {
        object GetInstance(Queue<ServiceTypeReference> queue, Type serviceType, string key);
    }
}
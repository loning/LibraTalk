using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Locator
{
    partial class InstanceLifetime
    {
        public static Func<Factory, InstanceLifetime> Singleton
        {
            get
            {
                return factory => new SingletonLifetime(factory);
            }
        }

        public class SingletonLifetime : InstanceLifetime
        {
            private object instance;

            public SingletonLifetime(Factory factory)
                : base(factory)
            {
            }

            public override object ResolveInstance(Queue<ServiceTypeReference> queue)
            {
                return instance ?? (instance = Factory.Create(queue));
            }
        }
    }
}
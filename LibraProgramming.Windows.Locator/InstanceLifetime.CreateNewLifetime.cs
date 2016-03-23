using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Locator
{
    partial class InstanceLifetime
    {
        public static Func<Factory, InstanceLifetime> CreateNew
        {
            get
            {
                return factory => new CreateNewLifetime(factory);
            }
        }

        public class CreateNewLifetime : InstanceLifetime
        {
            public CreateNewLifetime(Factory factory)
                : base(factory)
            {
            }

            public override object ResolveInstance(Queue<ServiceTypeReference> queue)
            {
                return Factory.Create(queue);
            }
        }
    }
}
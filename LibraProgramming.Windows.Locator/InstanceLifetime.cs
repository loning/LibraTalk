using System.Collections.Generic;

namespace LibraProgramming.Windows.Locator
{
    public abstract partial class InstanceLifetime
    {
        protected Factory Factory
        {
            get;
            private set;
        }

        public abstract object ResolveInstance(Queue<ServiceTypeReference> queue);

        protected InstanceLifetime(Factory factory)
        {
            Factory = factory;
        }
    }
}
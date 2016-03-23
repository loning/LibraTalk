using System;
using System.Collections.Generic;

namespace LibraProgramming.Windows.Locator
{
    internal class TypeFactory : Factory
    {
        private readonly Type type;

        public TypeFactory(IInstanceProvider provider, Type type)
            : base(provider)
        {
            this.type = type;
        }

        public override object Create(Queue<ServiceTypeReference> types)
        {
            return CreateInstance(types, type);
        }
    }
}
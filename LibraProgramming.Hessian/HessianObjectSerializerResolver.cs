using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LibraProgramming.Hessian
{
    public sealed partial class HessianObjectSerializerResolver : IObjectSerializerResolver
    {
        private ICollection<IObjectSerializer> cache;
        private readonly object synchroot;

        private HessianObjectSerializerResolver()
        {
            synchroot = new object();
        }

        public IObjectSerializer GetSerializer(Type target)
        {
            Contract.Ensures(null != Contract.Result<IObjectSerializer>(), "null != Contract.Result<IObjectSerializer>()");
            Contract.Assert(null != target);

            EnsureCache();

            lock (synchroot)
            {
                return cache.FirstOrDefault(item => item.CanHandle(target));
            }
        }

        private void EnsureCache()
        {
            if (null != cache)
            {
                return;
            }

            lock (synchroot)
            {
                if (null != cache)
                {
                    return;
                }

                cache = new Collection<IObjectSerializer>
                {
                    new BooleanSerializer(),
                    new Int32Serializer(),
                    new Int64Serializer(),
                    new DoubleSerializer(),
                    new StringSerializer(),
                    new DateTimeSerializer(),
                    new GuidSerializer()
                };
            }
        }
    }
}
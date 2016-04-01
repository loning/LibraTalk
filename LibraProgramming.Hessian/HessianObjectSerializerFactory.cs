using LibraProgramming.Hessian.Specification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraProgramming.Hessian
{
    internal sealed partial class HessianObjectSerializerFactory : IObjectSerializerFactory
    {
        private HashSet<Evidence> cache;

        public IObjectSerializer GetSerializer(Type target)
        {
            EnsureCache();
            return cache
                .Where(evidence => evidence.IsSatisfied(target))
                .Select(evidence => evidence.Serializer)
                .FirstOrDefault();
        }

        private void EnsureCache()
        {
            if (null != cache)
            {
                return;
            }

            cache = new HashSet<Evidence>
            {
                new TypedEvidence(typeof (bool), new BooleanSerializer()),
                new TypedEvidence(typeof (int), new Int32Serializer()),
                new TypedEvidence(typeof (long), new Int64Serializer()),
                new TypedEvidence(typeof (double), new DoubleSerializer()),
                new TypedEvidence(typeof (string), new StringSerializer()),
                new TypedEvidence(typeof (DateTime), new DateTimeSerializer()),
                /*[new FixedTypedListEvidence()] = new FixedTypedListSerializer()*/
            };
        }

        private abstract class Evidence : ISpecification<Type>
        {
            public IObjectSerializer Serializer
            {
                get;
            }

            protected Evidence(IObjectSerializer serializer)
            {
                Serializer = serializer;
            }

            public abstract bool IsSatisfied(Type arg);
        }

        private class TypedEvidence : Evidence
        {
            private readonly Type type;

            public TypedEvidence(Type type, IObjectSerializer serializer)
                :base(serializer)
            {
                this.type = type;
            }

            public override bool IsSatisfied(Type arg)
            {
                return type == arg;
            }
        }
    }
}
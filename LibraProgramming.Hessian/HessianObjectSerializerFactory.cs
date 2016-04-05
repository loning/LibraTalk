using LibraProgramming.Hessian.Specification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibraProgramming.Hessian
{
    internal sealed partial class HessianObjectSerializerFactory : IObjectSerializerFactory
    {
        private ICollection<Evidence> cache;

        public IObjectSerializer GetSerializer(Type target)
        {
            EnsureCache();

            var evidence = cache.FirstOrDefault(item => item.IsSatisfied(target));

            return evidence?.Serializer;
        }

        private void EnsureCache()
        {
            if (null != cache)
            {
                return;
            }

            var dict = new Collection<Evidence>
            {
                new TypedEvidence(typeof (bool), new BooleanSerializer()),
                new TypedEvidence(typeof (int), new Int32Serializer()),
                new TypedEvidence(typeof (long), new Int64Serializer()),
                new TypedEvidence(typeof (double), new DoubleSerializer()),
                new TypedEvidence(typeof (string), new StringSerializer()),
                new TypedEvidence(typeof (DateTime), new DateTimeSerializer())
                //[new FixedTypedListEvidence()] = new FixedTypedListSerializer()
            };
            
            cache = dict;
        }

        private abstract class Evidence : ISpecification<Type>
        {
            public IObjectSerializer Serializer
            {
                get;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
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
                : base(serializer)
            {
                this.type = type;
            }

            public override bool IsSatisfied(Type arg)
            {
                return type == arg;
            }
        }

/*
        private class FixedTypedListEvidence : ITypeProbe
        {
            bool ITypeProbe.Test(Type target)
            {

            }
        }
*/
    }
}
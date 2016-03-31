using LibraProgramming.Hessian.Specification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibraProgramming.Hessian
{
    internal sealed partial class HessianObjectSerializerFactory : IObjectSerializerFactory
    {
        private ICollection<KeyValuePair<ISpecification<Type>, IObjectSerializer>> cache;

        public IObjectSerializer GetSerializer(Type target)
        {
            EnsureCache();
            var serializer = cache
                .Where(pair => pair.Key.IsSatisfied(target))
                .Select(pair => pair.Value)
                .FirstOrDefault();
            return serializer;
        }

        private void EnsureCache()
        {
            if (null != cache)
            {
                return;
            }

            var dict = new Collection<KeyValuePair<ISpecification<Type>, IObjectSerializer>>
            {
                new KeyValuePair<ISpecification<Type>, IObjectSerializer>(new TypedEvidence(typeof(bool)), new BooleanSerializer()),
                /*[new TypedEvidence(typeof(int))] = new Int32Serializer(),
                [new TypedEvidence(typeof(long))] = new Int64Serializer(),
                [new TypedEvidence(typeof(double))] = new DoubleSerializer(),
                [new TypedEvidence(typeof(string))] = new StringSerializer(),
                [new TypedEvidence(typeof(DateTime))] = new DateTimeSerializer(),
                [new FixedTypedListEvidence()] = new FixedTypedListSerializer()*/
            };
            
            cache = dict;
        }

        private class TypedEvidence : ISpecification<Type>
        {
            private readonly Type type;

            public TypedEvidence(Type type)
            {
                this.type = type;
            }

            public bool IsSatisfied(Type arg)
            {
                return type == arg;
            }
        }

        /*private class FixedTypedListEvidence : ITypeProbe
        {
            bool ITypeProbe.Test(Type target)
            {

            }
        }*/
    }
}
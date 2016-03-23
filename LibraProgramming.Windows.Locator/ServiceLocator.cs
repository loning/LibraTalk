using System;
using System.Collections.Generic;
using System.Reflection;

namespace LibraProgramming.Windows.Locator
{
    /// <summary>
    /// Implementation of the IOC pattern.
    /// </summary>
    public sealed partial class ServiceLocator : IServiceLocator, IInstanceProvider
    {
        private static readonly Lazy<ServiceLocator> instance;
        private static readonly object sync;

        private readonly Dictionary<Type, InstanceCollection> registration;

        /// <summary>
        /// Gets instance of the <see cref="ServiceLocator" /> class.
        /// </summary>
        public static ServiceLocator Current => instance.Value;

        private ServiceLocator()
        {
            registration = new Dictionary<Type, InstanceCollection>();
        }

        static ServiceLocator()
        {
            sync = new object();
            instance = new Lazy<ServiceLocator>(() => new ServiceLocator());
        }

        #region Service Locator implementation

        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, string key = null)
        {
            if (null == serviceType)
            {
                Throw.ArgumentNull(nameof(serviceType));
            }

            var queue = new Queue<ServiceTypeReference>();

            return GetInstanceInternal(queue, serviceType, key);
        }

        private object GetInstanceInternal(Queue<ServiceTypeReference> queue, Type serviceType, string key = null)
        {
            lock (sync)
            {
                InstanceCollection collection;

                if (!registration.TryGetValue(serviceType, out collection))
                {
                    Throw.MissingServiceRegistration(serviceType, nameof(serviceType));
                }

                return collection[key].ResolveInstance(queue);
            }
        }

        object IInstanceProvider.GetInstance(Queue<ServiceTypeReference> queue, Type serviceType, string key)
        {
            return GetInstanceInternal(queue, serviceType, key);
        }

        public TService GetInstance<TService>(string key = null)
        {
            return (TService) GetInstance(typeof (TService), key);
        }

        #endregion

        #region Service Registration

        public void Register(Type service, Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
        {
            if (null == service)
            {
                Throw.ArgumentNull(nameof(service));
            }

            var ti = service.GetTypeInfo();

            if (ti.IsAbstract || ti.IsInterface)
            {
                Throw.UnsupportedServiceType(service);
            }

            RegisterService(service, new TypeFactory(this, service), lifetime, key, createimmediate);
        }

        public void Register(Type service, Type impl, Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
        {
            if (null == service)
            {
                Throw.ArgumentNull(nameof(service));
            }

            var ti = service.GetTypeInfo();

            if (!ti.IsAbstract && !ti.IsInterface)
            {
                Throw.UnsupportedServiceType(service);
            }

            if (null == impl)
            {
                Throw.ArgumentNull(nameof(impl));
            }

            ti = impl.GetTypeInfo();

            if (ti.IsAbstract || ti.IsInterface)
            {
                Throw.UnsupportedServiceType(impl);
            }

            RegisterService(service, new TypeFactory(this, impl), lifetime, key, createimmediate);
        }

        public void Register<TService>(Func<TService> factory, Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
        {
            RegisterService(typeof (TService), new CreatorFactory<TService>(this, factory), lifetime, key, createimmediate);
        }

        public void Register<TService>(Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false) where TService : class 
        {
            Register(typeof (TService), lifetime, key, createimmediate);
        }

        public void Register<TService, TConcrete>(Func<Factory, InstanceLifetime> lifetime = null, string key = null, bool createimmediate = false)
            where TConcrete : class, TService
        {
            Register(typeof (TService), typeof (TConcrete), lifetime, key, createimmediate);
        }

        #endregion

        private void RegisterService(Type service, Factory factory, Func<Factory, InstanceLifetime> lifetime, string key, bool createimmediate)
        {
            lock (sync)
            {
                InstanceCollection collection;

                if (!registration.TryGetValue(service, out collection))
                {
                    collection = new InstanceCollection();
                    registration.Add(service, collection);
                }
                else if (null == key)
                {
                    Throw.MissingServiceRegistration(service, nameof(service));
                }

                var manager = lifetime ?? InstanceLifetime.Singleton;

                collection[key] = manager(factory);

                if (createimmediate)
                {
                    var queue = new Queue<ServiceTypeReference>();
                    collection[key].ResolveInstance(queue);
                }
            }
        }
    }
}
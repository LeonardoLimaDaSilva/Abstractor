using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Adapters
{
    internal sealed class ContainerAdapter : IContainer
    {
        private readonly Container _container;

        public ContainerAdapter(Container container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.GetInstance(type);
        }

        public object GetCurrentLifetimeScope()
        {
            return _container.GetCurrentLifetimeScope();
        }

        public IDisposable BeginLifetimeScope()
        {
            return _container.BeginLifetimeScope();
        }

        public IEnumerable<IInstanceProducer> GetCurrentRegistrations()
        {
            return _container.GetCurrentRegistrations()
                .Select(i => new InstanceProducerAdapter(i.GetInstance())
                {
                    ServiceType = i.ServiceType
                });
        }
    }
}
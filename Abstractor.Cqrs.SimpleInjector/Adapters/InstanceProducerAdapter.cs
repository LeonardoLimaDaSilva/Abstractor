using System;
using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.SimpleInjector.Adapters
{
    internal sealed class InstanceProducerAdapter : IInstanceProducer
    {
        private readonly object _instance;

        public InstanceProducerAdapter(object instance, Type serviceType)
        {
            _instance = instance;
            ServiceType = serviceType;
        }

        public Type ServiceType { get; }

        public object GetInstance()
        {
            return _instance;
        }
    }
}
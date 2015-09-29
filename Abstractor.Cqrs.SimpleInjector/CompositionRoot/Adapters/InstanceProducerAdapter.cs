using System;
using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Adapters
{
    internal sealed class InstanceProducerAdapter : IInstanceProducer
    {
        private readonly object _instance;

        public InstanceProducerAdapter(object instance)
        {
            _instance = instance;
        }

        public Type ServiceType { get; set; }

        public object GetInstance()
        {
            return _instance;
        }
    }
}
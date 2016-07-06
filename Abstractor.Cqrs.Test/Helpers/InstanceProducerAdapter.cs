using System;
using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.Test.Helpers
{
    public class InstanceProducerAdapter : IInstanceProducer
    {
        private readonly object _instance;

        public InstanceProducerAdapter(object instance)
        {
            _instance = instance;
            ServiceType = _instance.GetType();
        }

        public Type ServiceType { get; }

        public object GetInstance()
        {
            return _instance;
        }
    }
}
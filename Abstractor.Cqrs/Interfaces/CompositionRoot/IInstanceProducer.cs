using System;

namespace Abstractor.Cqrs.Interfaces.CompositionRoot
{
    /// <summary>
    ///     Abstrai o produtor de instâncias do container.
    /// </summary>
    public interface IInstanceProducer
    {
        Type ServiceType { get; }

        object GetInstance();
    }
}
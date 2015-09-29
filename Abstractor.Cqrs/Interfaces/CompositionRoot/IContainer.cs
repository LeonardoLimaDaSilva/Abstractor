using System;
using System.Collections.Generic;

namespace Abstractor.Cqrs.Interfaces.CompositionRoot
{
    /// <summary>
    /// Abstrai o container de inversão de controle.
    /// </summary>
    public interface IContainer
    {
        object GetInstance(Type type);
        object GetCurrentLifetimeScope();
        IDisposable BeginLifetimeScope();
        IEnumerable<IInstanceProducer> GetCurrentRegistrations();
    }
}

using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Abstract class that marks an entity as an aggregate root to be materialized by the persistence mechanism.
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Abstractor.Cqrs.Interfaces.Domain;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Abstract class that marks an entity as an aggregate root to be materialized by the persistence mechanism.
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents;

        public IReadOnlyCollection<IDomainEvent> DomainEvents => new ReadOnlyCollection<IDomainEvent>(_domainEvents);

        protected AggregateRoot()
        {
            _domainEvents = new List<IDomainEvent>();
        }

        public void ApplyEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
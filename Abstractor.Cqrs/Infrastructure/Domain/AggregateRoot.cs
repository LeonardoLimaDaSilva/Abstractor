using System.Collections.Generic;
using System.Collections.ObjectModel;
using Abstractor.Cqrs.Interfaces.Domain;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Abstract class that marks an entity as an aggregate root and provides an internal collection for domain events.
    /// </summary>
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents;

        /// <summary>
        ///     Collection of events emitted from the current instance of the aggregate root.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> EmittedEvents => new ReadOnlyCollection<IDomainEvent>(_domainEvents);

        /// <summary>
        ///     Constructs the aggregate with an identifier.
        /// </summary>
        /// <param name="id">Aggregate identifier.</param>
        protected AggregateRoot(TId id)
            : base(id)
        {
            _domainEvents = new List<IDomainEvent>();
        }

        /// <summary>
        ///     Emits the event to be returned by the command handler.
        /// </summary>
        /// <param name="domainEvent">Domain event to be emitted.</param>
        public void Emit(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        ///     Private list of domain events.
        /// </summary>
        private readonly List<IDomainEvent> _domainEvents;

        /// <summary>
        ///     Initialize the collection of events.
        /// </summary>
        protected CommandHandler()
        {
            _domainEvents = new List<IDomainEvent>();
        }

        /// <summary>
        ///     Collection of events emitted from the current instance of the command handler.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> EmittedEvents => new ReadOnlyCollection<IDomainEvent>(_domainEvents);

        /// <summary>
        ///     Abstract handling method that each concrete handler should override;
        /// </summary>
        /// <param name="command"></param>
        public abstract void Handle(TCommand command);

        /// <summary>
        ///     Emits the event ocurred into the command handler.
        /// </summary>
        /// <param name="domainEvent">Domain event to be emitted.</param>
        public void Emit(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        ///     Emits all the events ocurred into the command handler.
        /// </summary>
        /// <param name="domainEvents">Domain events to be emitted.</param>
        public void Emit(IEnumerable<IDomainEvent> domainEvents)
        {
            _domainEvents.AddRange(domainEvents);
        }
    }
}
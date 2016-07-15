using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Gets the domain events returned from the command handler, if any, and dispatches them synchronous and sequentially.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class DomainEventDispatcherDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        public DomainEventDispatcherDecorator(
            IDomainEventDispatcher eventDispatcher,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _eventDispatcher = eventDispatcher;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Delegates the domain events returned from the command handler to the event dispatcher.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        /// <returns>List of domain events raised by the command, if any.</returns>
        public IEnumerable<IDomainEvent> Handle(TCommand command)
        {
            var domainEvents = _handlerFactory().Handle(command)?.ToList();
            if (domainEvents == null) return null;

            foreach (var domainEvent in domainEvents)
                _eventDispatcher.Dispatch(domainEvent);

            return domainEvents;
        }
    }
}
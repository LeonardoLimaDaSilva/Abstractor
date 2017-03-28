using System;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Gets the domain events returned from the command handler, if any, and dispatches them synchronous and sequentially.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class DomainEventDispatcherDecorator<TCommand> : CommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        /// <summary>
        ///     DomainEventDispatcherDecorator constructor.
        /// </summary>
        /// <param name="eventDispatcher"></param>
        /// <param name="handlerFactory"></param>
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
        public override void Handle(TCommand command)
        {
            var handler = _handlerFactory();

            handler.Handle(command);

            foreach (var domainEvent in handler.EmittedEvents)
                _eventDispatcher.Dispatch(domainEvent);
        }
    }
}
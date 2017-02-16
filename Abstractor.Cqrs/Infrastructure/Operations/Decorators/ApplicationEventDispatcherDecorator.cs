using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Delegates the command, marked as an event, to the application event dispatcher after the execution of
    ///     <see cref="ICommandHandler{TCommand}" />.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class ApplicationEventDispatcherDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand, IApplicationEvent
    {
        private readonly IApplicationEventDispatcher _eventDispatcher;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        /// <summary>
        ///     ApplicationEventDispatcherDecorator constructor.
        /// </summary>
        /// <param name="eventDispatcher"></param>
        /// <param name="handlerFactory"></param>
        public ApplicationEventDispatcherDecorator(
            IApplicationEventDispatcher eventDispatcher,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _eventDispatcher = eventDispatcher;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Delegates the command, marked as an event, to the event dispatcher.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        /// <returns>List of domain events raised by the command, if any.</returns>
        public IEnumerable<IDomainEvent> Handle(TCommand command)
        {
            try
            {
                var domainEvents = _handlerFactory().Handle(command)?.ToList();
                _eventDispatcher.Dispatch(command);
                return domainEvents;
            }
            catch (CommandException ex)
            {
                _eventDispatcher.Dispatch(ex);
                throw;
            }
        }
    }
}
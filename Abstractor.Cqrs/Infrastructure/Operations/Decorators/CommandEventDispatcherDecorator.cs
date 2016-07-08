using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Delegates the command, marked as an event listener, to the event dispatcher after the execution of
    ///     <see cref="ICommandHandler{TCommand}" />.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandEventDispatcherDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand, IEventListener
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        public CommandEventDispatcherDecorator(
            IEventDispatcher eventDispatcher,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _eventDispatcher = eventDispatcher;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Delegates the command, marked as an event listener, to the event dispatcher.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public void Handle(TCommand command)
        {
            _handlerFactory().Handle(command);
            _eventDispatcher.Dispatch(command);
        }
    }
}
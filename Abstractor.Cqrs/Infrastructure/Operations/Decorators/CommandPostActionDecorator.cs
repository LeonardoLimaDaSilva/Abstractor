using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Executes the action event registered in <see cref="ICommandPostAction.Execute" /> after the command is handled.
    /// </summary>
    /// <typeparam name="TCommand">Command to be executed.</typeparam>
    public sealed class CommandPostActionDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandPostAction _commandPostAction;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        public CommandPostActionDecorator(
            Func<ICommandHandler<TCommand>> handler,
            ICommandPostAction commandPostAction)
        {
            _handlerFactory = handler;
            _commandPostAction = commandPostAction;
        }

        /// <summary>
        ///     Executes an action after the command is handled.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        /// <returns>List of domain events raised by the command, if any.</returns>
        public IEnumerable<IDomainEvent> Handle(TCommand command)
        {
            try
            {
                var domainEvents = _handlerFactory().Handle(command)?.ToList();
                _commandPostAction.Act();
                return domainEvents;
            }
            finally
            {
                _commandPostAction.Reset();
            }
        }
    }
}
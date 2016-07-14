using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Ensures that there is a lifetime scope before the command execution.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class CommandLifetimeScopeDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IContainer _container;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        public CommandLifetimeScopeDecorator(
            IContainer container,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Ensures that there is a lifetime scope before the command execution.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        /// <returns>List of domain events raised by the command, if any.</returns>
        public IEnumerable<IDomainEvent> Handle(TCommand command)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().Handle(command)?.ToList();

            using (_container.BeginLifetimeScope())
                return _handlerFactory().Handle(command)?.ToList();
        }
    }
}
using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Ensures that there is a lifetime scope before the command execution.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    [DebuggerStepThrough]
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
        /// <param name="command">Command to be handled</param>
        public void Handle(TCommand command)
        {
            if (_container.GetCurrentLifetimeScope() != null)
            {
                _handlerFactory().Handle(command);
                return;
            }

            using (_container.BeginLifetimeScope())
                _handlerFactory().Handle(command);
        }
    }
}
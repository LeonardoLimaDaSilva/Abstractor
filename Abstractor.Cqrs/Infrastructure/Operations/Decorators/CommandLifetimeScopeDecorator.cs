using System;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Ensures that there is a lifetime scope before the command execution.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class CommandLifetimeScopeDecorator<TCommand> : CommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IContainer _container;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        /// <summary>
        ///     CommandLifetimeScopeDecorator constructor.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="handlerFactory"></param>
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
        public override void Handle(TCommand command)
        {
            if (_container.GetCurrentLifetimeScope() != null)
            {
                _handlerFactory().Handle(command);
                return;
            }

            using (_container.BeginLifetimeScope())
            {
                _handlerFactory().Handle(command);
            }
        }
    }
}
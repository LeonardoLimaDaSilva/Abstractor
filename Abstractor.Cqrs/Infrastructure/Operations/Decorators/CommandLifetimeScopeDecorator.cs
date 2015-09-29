using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    /// Inicia um novo escopo caso não exista nenhum.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será executado.</typeparam>
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

        [DebuggerStepThrough]
        public void Handle(TCommand command)
        {
            if (_container.GetCurrentLifetimeScope() != null)
            {
                _handlerFactory().Handle(command);
            }
            else
            {
                using (_container.BeginLifetimeScope())
                {
                    _handlerFactory().Handle(command);
                }
            }
        }
    }
}

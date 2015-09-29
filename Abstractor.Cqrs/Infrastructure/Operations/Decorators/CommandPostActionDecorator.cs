using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    /// Extende a funcionalidade do <see cref="ICommandHandler{TCommand}"/> garantindo que uma ação seja executada após a finalização do comando.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será executado.</typeparam>
    public sealed class CommandPostActionDecorator<TCommand> : ICommandHandler<TCommand> 
        where TCommand : ICommand
    {
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly CommandPostAction _commandPostAction;

        public CommandPostActionDecorator(
            Func<ICommandHandler<TCommand>> handlerFactory, 
            CommandPostAction commandPostAction)
        {
            _handlerFactory = handlerFactory;
            _commandPostAction = commandPostAction;
        }

        [DebuggerStepThrough]
        public void Handle(TCommand command)
        {
            var handler = _handlerFactory();
            try
            {
                handler.Handle(command);
                _commandPostAction.Act();
            }
            finally
            {
                _commandPostAction.Reset();
            }
        }
    }
}

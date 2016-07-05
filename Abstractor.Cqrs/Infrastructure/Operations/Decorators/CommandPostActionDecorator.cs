using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Extende a funcionalidade do <see cref="ICommandHandler{TCommand}" /> garantindo que uma ação seja executada após a
    ///     finalização do comando.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será executado.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandPostActionDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandPostAction _commandPostAction;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        public CommandPostActionDecorator(
            Func<ICommandHandler<TCommand>> handlerFactory,
            ICommandPostAction commandPostAction)
        {
            _handlerFactory = handlerFactory;
            _commandPostAction = commandPostAction;
        }

        /// <summary>
        ///     Executa uma ação após a execução do comando.
        /// </summary>
        /// <param name="command"></param>
        public void Handle(TCommand command)
        {
            try
            {
                _handlerFactory().Handle(command);
                _commandPostAction.Act();
            }
            finally
            {
                _commandPostAction.Reset();
            }
        }
    }
}
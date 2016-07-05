using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Extende a funcionalidade do <see cref="ICommandHandler{TCommand}" /> tratando o comando como um evento, caso o
    ///     mesmo esteja habilitado a executar eventos.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será executado.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandEventDispatcherDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommandEvent
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
        ///     Dispara o evento após a execução do comando.
        /// </summary>
        /// <param name="command"></param>
        public void Handle(TCommand command)
        {
            _handlerFactory().Handle(command);
            _eventDispatcher.Dispatch(command);
        }
    }
}
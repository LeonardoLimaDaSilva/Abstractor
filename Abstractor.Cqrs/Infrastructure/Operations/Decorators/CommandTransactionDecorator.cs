using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Executa o comando <see cref="TCommand" /> dentro de uma transação e salva as alterações no repositório de dados.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será executado.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandTransactionDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly IUnitOfWork _unitOfWork;

        public CommandTransactionDecorator(
            IUnitOfWork unitOfWork,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _unitOfWork = unitOfWork;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Confirma a transação após a execução do comando.
        /// </summary>
        /// <param name="command"></param>
        public void Handle(TCommand command)
        {
            _handlerFactory().Handle(command);
            _unitOfWork.Commit();
        }
    }
}
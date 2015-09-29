using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    /// Garante que o <see cref="ICommandHandler{TCommand}"/> receba apenas parâmetros não nulos.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será executado.</typeparam>
    public sealed class CommandNotNullDecorator<TCommand> : ICommandHandler<TCommand> 
        where TCommand : ICommand
    {
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        public CommandNotNullDecorator(Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public void Handle(TCommand command)
        {
            if (Equals(command, null)) throw new ArgumentNullException(nameof(command));
            
            _handlerFactory().Handle(command);
        }
    }
}
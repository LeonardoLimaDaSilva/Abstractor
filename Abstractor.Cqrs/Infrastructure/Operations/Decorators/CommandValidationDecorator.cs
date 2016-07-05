using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Valida o comando <see cref="TCommand" /> utilizando o validador <see cref="IValidator" />.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será executado.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandValidationDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly IValidator _validator;

        public CommandValidationDecorator(
            IValidator validator,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _validator = validator;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Valida o comando antes de executá-lo.
        /// </summary>
        /// <param name="command"></param>
        public void Handle(TCommand command)
        {
            _validator.Validate(command);
            _handlerFactory().Handle(command);
        }
    }
}
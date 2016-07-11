using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Validates the command <see cref="TCommand" /> using the <see cref="IValidator" />.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
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
        ///     Validates the command before his execution.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public void Handle(TCommand command)
        {
            _validator.Validate(command);
            _handlerFactory().Handle(command);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Validates the command <see cref="TCommand" /> using the <see cref="IValidator" />.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
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
        /// <returns>List of domain events raised by the command, if any.</returns>
        public IEnumerable<IDomainEvent> Handle(TCommand command)
        {
            _validator.Validate(command);
            return _handlerFactory().Handle(command)?.ToList();
        }
    }
}
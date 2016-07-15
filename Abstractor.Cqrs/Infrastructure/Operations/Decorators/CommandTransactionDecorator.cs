using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Commits the unit of work after the successful command execution.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class CommandTransactionDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CommandTransactionDecorator(
            IAttributeFinder attributeFinder,
            ILogger logger,
            IUnitOfWork unitOfWork,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _attributeFinder = attributeFinder;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Commits the unit of work after the successful command execution.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        /// <returns>List of domain events raised by the command, if any.</returns>
        public IEnumerable<IDomainEvent> Handle(TCommand command)
        {
            if (!_attributeFinder.Decorates(command.GetType(), typeof (TransactionalAttribute)))
                return _handlerFactory().Handle(command)?.ToList();

            var log = _attributeFinder.Decorates(command.GetType(), typeof (LogAttribute));

            if (log) _logger.Log("Starting transaction...");

            var domainEvents = _handlerFactory().Handle(command)?.ToList();
            _unitOfWork.Commit();

            if (log) _logger.Log("Transaction committed.");

            return domainEvents;
        }
    }
}
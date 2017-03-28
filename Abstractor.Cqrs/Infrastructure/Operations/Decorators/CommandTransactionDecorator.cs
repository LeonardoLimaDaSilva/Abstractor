using System;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Commits the unit of work after the successful command execution.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class CommandTransactionDecorator<TCommand> : CommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly Func<ILogger> _logger;
        private readonly GlobalSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     CommandTransactionDecorator constructor.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="attributeFinder"></param>
        /// <param name="logger"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="handlerFactory"></param>
        public CommandTransactionDecorator(
            GlobalSettings settings,
            IAttributeFinder attributeFinder,
            Func<ILogger> logger,
            IUnitOfWork unitOfWork,
            Func<ICommandHandler<TCommand>> handlerFactory)
        {
            _settings = settings;
            _attributeFinder = attributeFinder;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Commits the unit of work after the successful command execution.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public override void Handle(TCommand command)
        {
            if (!_attributeFinder.Decorates(command.GetType(), typeof(TransactionalAttribute)) &&
                !_settings.EnableTransactions)
            {
                _handlerFactory().Handle(command);
                return;
            }

            var log = _attributeFinder.Decorates(command.GetType(), typeof(LogAttribute)) || _settings.EnableLogging;

            if (log) _logger().Log("Starting transaction...");

            _unitOfWork.Clear();

            _handlerFactory().Handle(command);

            _unitOfWork.Commit();

            if (log) _logger().Log("Transaction committed.");
        }
    }
}
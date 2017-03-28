using System;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the command handler.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public sealed class CommandLoggerDecorator<TCommand> : CommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly Func<ILogger> _logger;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly GlobalSettings _settings;
        private readonly IStopwatch _stopwatch;

        /// <summary>
        ///     CommandLoggerDecorator constructor.
        /// </summary>
        /// <param name="handlerFactory"></param>
        /// <param name="attributeFinder"></param>
        /// <param name="stopwatch"></param>
        /// <param name="loggerSerializer"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        public CommandLoggerDecorator(
            Func<ICommandHandler<TCommand>> handlerFactory,
            IAttributeFinder attributeFinder,
            IStopwatch stopwatch,
            ILoggerSerializer loggerSerializer,
            Func<ILogger> logger,
            GlobalSettings settings)
        {
            _handlerFactory = handlerFactory;
            _attributeFinder = attributeFinder;
            _stopwatch = stopwatch;
            _loggerSerializer = loggerSerializer;
            _logger = logger;
            _settings = settings;
        }

        /// <summary>
        ///     Logs the execution of the command handler.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public override void Handle(TCommand command)
        {
            if (!_attributeFinder.Decorates(command.GetType(), typeof(LogAttribute)) && !_settings.EnableLogging)
            {
                _handlerFactory().Handle(command);
                return;
            }

            _stopwatch.Start();

            var logger = _logger();

            try
            {
                logger.Log($"Executing command \"{command.GetType().Name}\" with the parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(command);
                    logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                _handlerFactory().Handle(command);
            }
            catch (Exception ex)
            {
                logger.Log("Exception caught: " + ex.Message);

                if (ex.InnerException != null)
                    logger.Log("Inner exception caught: " + ex.InnerException.Message);

                throw;
            }
            finally
            {
                _stopwatch.Stop();

                logger.Log($"Command \"{command.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}
using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the command handler.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandLoggerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly IStopwatch _stopwatch;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly ILogger _logger;

        public CommandLoggerDecorator(
            Func<ICommandHandler<TCommand>> handlerFactory,
            IStopwatch stopwatch,
            ILoggerSerializer loggerSerializer,
            ILogger logger)
        {
            _handlerFactory = handlerFactory;
            _stopwatch = stopwatch;
            _loggerSerializer = loggerSerializer;
            _logger = logger;
        }

        /// <summary>
        ///     Logs the execution of the command handler.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public void Handle(TCommand command)
        {
            _stopwatch.Start();

            try
            {
                _logger.Log($"Executing command \"{command.GetType().Name}\" with the parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(command);
                    _logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                _handlerFactory().Handle(command);
            }
            catch (Exception ex)
            {
                _logger.Log("Exception caught: " + ex.Message);

                if (ex.InnerException != null)
                    _logger.Log("Inner exception caught: " + ex.InnerException.Message);

                throw;
            }
            finally
            {
                _stopwatch.Stop();

                _logger.Log($"Command \"{command.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}
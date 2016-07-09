using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the event handler and supresses any possible exception.
    /// </summary>
    /// <typeparam name="TEventListener">Listener that the event subscribes to.</typeparam>
    [DebuggerStepThrough]
    public sealed class EventLoggerDecorator<TEventListener> : IEventHandler<TEventListener>
        where TEventListener : IEventListener
    {
        private readonly Func<IEventHandler<TEventListener>> _handlerFactory;
        private readonly ILogger _logger;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly IStopwatch _stopwatch;

        public EventLoggerDecorator(
            Func<IEventHandler<TEventListener>> handlerFactory,
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
        ///     Logs the execution of the event handler and supresses any possible exception.
        /// </summary>
        /// <param name="eventListener">Listener that the event subscribes to.</param>
        public void Handle(TEventListener eventListener)
        {
            _stopwatch.Start();

            var handler = _handlerFactory();

            try
            {
                _logger.Log($"Executing event \"{handler.GetType().Name}\" with the listener parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(eventListener);
                    _logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Could not serialize the listener parameters: {ex.Message}");
                }

                _handlerFactory().Handle(eventListener);
            }
            catch (Exception ex)
            {
                _logger.Log("Exception caught: " + ex.Message);

                if (ex.InnerException != null)
                    _logger.Log("Inner exception caught: " + ex.InnerException.Message);
            }
            finally
            {
                _stopwatch.Stop();

                _logger.Log($"Event \"{handler.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}
using System;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the event handler.
    /// </summary>
    /// <typeparam name="TEvent">Domain event in which the handler subscribes to.</typeparam>
    public sealed class DomainEventLoggerDecorator<TEvent> : IDomainEventHandler<TEvent>
        where TEvent : IDomainEvent
    {
        private readonly Func<IDomainEventHandler<TEvent>> _handlerFactory;
        private readonly ILogger _logger;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly IStopwatch _stopwatch;

        public DomainEventLoggerDecorator(
            Func<IDomainEventHandler<TEvent>> handlerFactory,
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
        ///     Logs the execution of the event handler.
        /// </summary>
        /// <param name="domainEvent">Domain event in which the handler subscribes to.</param>
        public void Handle(TEvent domainEvent)
        {
            _stopwatch.Start();

            try
            {
                _logger.Log($"Executing domain event \"{domainEvent.GetType().Name}\" with the parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(domainEvent);
                    _logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                _handlerFactory().Handle(domainEvent);
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

                _logger.Log($"Domain event \"{domainEvent.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}
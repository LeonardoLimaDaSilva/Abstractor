using System;
using Abstractor.Cqrs.Infrastructure.CompositionRoot;
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
        private readonly IAttributeFinder _attributeFinder;
        private readonly Func<IDomainEventHandler<TEvent>> _handlerFactory;
        private readonly Func<ILogger> _logger;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly GlobalSettings _settings;
        private readonly IStopwatch _stopwatch;

        /// <summary>
        ///     DomainEventLoggerDecorator constructor.
        /// </summary>
        /// <param name="handlerFactory"></param>
        /// <param name="attributeFinder"></param>
        /// <param name="stopwatch"></param>
        /// <param name="loggerSerializer"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        public DomainEventLoggerDecorator(
            Func<IDomainEventHandler<TEvent>> handlerFactory,
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
        ///     Logs the execution of the event handler.
        /// </summary>
        /// <param name="domainEvent">Domain event in which the handler subscribes to.</param>
        public void Handle(TEvent domainEvent)
        {
            var handler = _handlerFactory();

            if (!_attributeFinder.Decorates(domainEvent.GetType(), typeof(LogAttribute)) && !_settings.EnableLogging)
            {
                handler.Handle(domainEvent);
                return;
            }

            _stopwatch.Start();

            var logger = _logger();

            try
            {
                logger.Log($"Executing domain event \"{domainEvent.GetType().Name}\" with the parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(domainEvent);
                    logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                handler.Handle(domainEvent);
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

                logger.Log($"Domain event \"{domainEvent.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}
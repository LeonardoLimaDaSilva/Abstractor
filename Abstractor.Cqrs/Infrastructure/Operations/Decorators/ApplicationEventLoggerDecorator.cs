using System;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the event handler.
    /// </summary>
    /// <typeparam name="TEvent">Application event in which the handler subscribes to.</typeparam>
    public sealed class ApplicationEventLoggerDecorator<TEvent> : IApplicationEventHandler<TEvent>
        where TEvent : IApplicationEvent
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly Func<IApplicationEventHandler<TEvent>> _handlerFactory;
        private readonly ILogger _logger;
        private readonly ILoggerSerializer _loggerSerializer;
        private readonly IStopwatch _stopwatch;

        public ApplicationEventLoggerDecorator(
            Func<IApplicationEventHandler<TEvent>> handlerFactory,
            IAttributeFinder attributeFinder,
            IStopwatch stopwatch,
            ILoggerSerializer loggerSerializer,
            ILogger logger)
        {
            _handlerFactory = handlerFactory;
            _attributeFinder = attributeFinder;
            _stopwatch = stopwatch;
            _loggerSerializer = loggerSerializer;
            _logger = logger;
        }

        /// <summary>
        ///     Logs the execution of the event handler.
        /// </summary>
        /// <param name="applicationEvent">Application event in which the handler subscribes to.</param>
        public void Handle(TEvent applicationEvent)
        {
            var handler = _handlerFactory();

            if (!_attributeFinder.Decorates(applicationEvent.GetType(), typeof (LogAttribute)))
            {
                handler.Handle(applicationEvent);
                return;
            }

            _stopwatch.Start();

            try
            {
                _logger.Log($"Executing application event \"{handler.GetType().Name}\" with the parameters:");

                try
                {
                    var parameters = _loggerSerializer.Serialize(applicationEvent);
                    _logger.Log(parameters);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                handler.Handle(applicationEvent);
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

                _logger.Log($"Application event \"{handler.GetType().Name}\" executed in {_stopwatch.GetElapsed()}.");
            }
        }
    }
}
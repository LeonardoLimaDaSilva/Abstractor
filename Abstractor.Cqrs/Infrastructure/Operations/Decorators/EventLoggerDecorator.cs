using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the event handler and supresses any possible exception.
    /// </summary>
    /// <typeparam name="TEventListener">Listener that the event subscribes to.</typeparam>
    //[DebuggerStepThrough]
    public sealed class EventLoggerDecorator<TEventListener> : IEventHandler<TEventListener>
        where TEventListener : IEventListener
    {
        private readonly Func<IEventHandler<TEventListener>> _handlerFactory;
        private readonly ILogger _logger;

        public EventLoggerDecorator(
            Func<IEventHandler<TEventListener>> handlerFactory,
            ILogger logger)
        {
            _handlerFactory = handlerFactory;
            _logger = logger;
        }

        /// <summary>
        ///     Logs the execution of the event handler and supresses any possible exception.
        /// </summary>
        /// <param name="eventListener">Listener that the event subscribes to.</param>
        public void Handle(TEventListener eventListener)
        {
            var sw = Stopwatch.StartNew();
            sw.Start();

            var handler = _handlerFactory();

            try
            {
                _logger.Log($"Executing event \"{handler.GetType().Name}\" with the listener parameters:");

                try
                {
                    _logger.Log(JsonConvert.SerializeObject(eventListener, Formatting.Indented));
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
                sw.Stop();

                _logger.Log($"Event \"{handler.GetType().Name}\" executed in {sw.Elapsed}.");
            }
        }
    }
}
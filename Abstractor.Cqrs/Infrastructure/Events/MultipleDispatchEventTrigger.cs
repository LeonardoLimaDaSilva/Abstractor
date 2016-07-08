using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    /// <summary>
    ///     Dispatcher for multiple event handlers.
    /// </summary>
    /// <typeparam name="TEvent">Listener for the event handlers.</typeparam>
    public sealed class MultipleDispatchEventTrigger<TEvent> : IEventTrigger<TEvent>
        where TEvent : IEvent
    {
        private readonly IContainer _container;
        private readonly ILogger _logger;

        public MultipleDispatchEventTrigger(
            IContainer container,
            ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        /// <summary>
        ///     Triggers all event handlers registered for the current event listener.
        /// </summary>
        /// <param name="event">Listener for the event handlers.</param>
        public void Trigger(TEvent @event)
        {
            var handlers = GetHandlers(_container);
            if (handlers == null || !handlers.Any()) return;

            foreach (var handler in handlers)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        if (_container.GetCurrentLifetimeScope() != null)
                        {
                            try
                            {
                                handler.Handle(@event);
                            }
                            catch (Exception ex)
                            {
                                _logger.Log(ex.Message);
                            }
                        }
                        else
                        {
                            using (_container.BeginLifetimeScope())
                            {
                                try
                                {
                                    handler.Handle(@event);
                                }
                                catch (Exception ex)
                                {
                                    _logger.Log(ex.Message);
                                }
                            }
                        }
                    });
            }
        }

        /// <summary>
        ///     Returns all the event handlers registered in container for the current event listeners, namely, the implementations
        ///     of <see cref="IEventHandler{TEvent}" />.
        /// </summary>
        /// <param name="container">Inversion of control container.</param>
        /// <returns>List of registered <see cref="IEventHandler{TEvent}" />.</returns>
        private static IList<IEventHandler<TEvent>> GetHandlers(IContainer container)
        {
            var handlersType = typeof(IEventHandler<TEvent>);

            return container.GetCurrentRegistrations()
                .Where(x => handlersType.IsAssignableFrom(x.ServiceType))
                .Select(x => x.GetInstance()).Cast<IEventHandler<TEvent>>()
                .ToList();
        }
    }
}
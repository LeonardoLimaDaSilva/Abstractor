using System;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    /// <summary>
    ///     Garante que uma nova thread será criada apenas se existirem handlers <see cref="IEventHandler{TEvent}" />
    ///     registrados no <see cref="IContainer" /> para este evento.
    /// </summary>
    /// <typeparam name="TEvent">Evento que será disparado.</typeparam>
    public sealed class TriggerRegisteredEventsDecorator<TEvent> : IEventTrigger<TEvent> where TEvent : IEvent
    {
        private readonly IContainer _container;
        private readonly Func<IEventTrigger<TEvent>> _factory;

        public TriggerRegisteredEventsDecorator(IContainer container, Func<IEventTrigger<TEvent>> factory)
        {
            _container = container;
            _factory = factory;
        }

        /// <summary>
        ///     Dispara um <see cref="IEventHandler{TEvent}" />.
        /// </summary>
        /// <param name="event"></param>
        public void Trigger(TEvent @event)
        {
            if (!MultipleDispatchEventTrigger<TEvent>.HasRegisteredHandlers(_container)) return;

            Task.Factory.StartNew(() =>
            {
                if (_container.GetCurrentLifetimeScope() != null)
                {
                    _factory().Trigger(@event);
                }
                else
                {
                    using (_container.BeginLifetimeScope())
                    {
                        _factory().Trigger(@event);
                    }
                }
            });
        }
    }
}
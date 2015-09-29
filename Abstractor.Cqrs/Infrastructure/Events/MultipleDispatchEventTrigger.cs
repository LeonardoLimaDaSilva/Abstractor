using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    /// <summary>
    /// Dispara múltiplos eventos.
    /// </summary>
    /// <typeparam name="TEvent">Evento que será disparado.</typeparam>
    public sealed class MultipleDispatchEventTrigger<TEvent> : IEventTrigger<TEvent> where TEvent : IEvent
    {
        private readonly IContainer _container;

        public MultipleDispatchEventTrigger(IContainer container)
        {
            _container = container;
        }

        public void Trigger(TEvent e)
        {
            var handlers = GetHandlers(_container);
            if (handlers == null || !handlers.Any()) return;

            foreach (var handler in handlers)
                handler.Handle(e);
        }

        /// <summary>
        /// Identifica e retorna apenas os <see cref="IEventHandler{TEvent}"/> registrados no container de inversão de controle.
        /// </summary>
        /// <param name="container">Abstração do container de inversão de controle.</param>
        /// <returns>Lista dos <see cref="IEventHandler{TEvent}"/> registrados.</returns>
        internal static IList<IEventHandler<TEvent>> GetHandlers(IContainer container)
        {
            var handlersType = typeof(IEventHandler<TEvent>);

            var handlers = container.GetCurrentRegistrations()
                .Where(x => handlersType.IsAssignableFrom(x.ServiceType))
                .Select(x => x.GetInstance()).Cast<IEventHandler<TEvent>>()
                .ToList();

            return handlers;
        }
    }
}

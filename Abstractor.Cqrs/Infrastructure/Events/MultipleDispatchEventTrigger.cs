using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    /// <summary>
    ///     Dispara múltiplos eventos.
    /// </summary>
    /// <typeparam name="TEvent">Evento que será disparado.</typeparam>
    public sealed class MultipleDispatchEventTrigger<TEvent> : IEventTrigger<TEvent> where TEvent : IEvent
    {
        private readonly IContainer _container;

        public MultipleDispatchEventTrigger(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Dispara um <see cref="IEventHandler{TEvent}" />.
        /// </summary>
        /// <param name="event">Evento que será disparado.</param>
        public void Trigger(TEvent @event)
        {
            var handlers = GetHandlers(_container);
            if (handlers == null || !handlers.Any()) return;

            foreach (var handler in handlers)
                handler.Handle(@event);
        }

        /// <summary>
        ///     Informa se o container possui um ou mais <see cref="IEventHandler{TEvent}" /> registrados no container.
        /// </summary>
        /// <param name="container">Abstração do container de inversão de controle.</param>
        /// <returns></returns>
        public static bool HasRegisteredHandlers(IContainer container)
        {
            var handlers = GetHandlers(container);
            return GetHandlers(container) != null && handlers.Any();
        }

        /// <summary>
        ///     Identifica e retorna apenas os <see cref="IEventHandler{TEvent}" /> registrados no container de inversão de
        ///     controle.
        /// </summary>
        /// <param name="container">Abstração do container de inversão de controle.</param>
        /// <returns>Lista dos <see cref="IEventHandler{TEvent}" /> registrados.</returns>
        private static IList<IEventHandler<TEvent>> GetHandlers(IContainer container)
        {
            var handlersType = typeof (IEventHandler<TEvent>);

            return container.GetCurrentRegistrations()
                            .Where(x => handlersType.IsAssignableFrom(x.ServiceType))
                            .Select(x => x.GetInstance()).Cast<IEventHandler<TEvent>>()
                            .ToList();
        }
    }
}
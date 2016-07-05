using System.Diagnostics;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    /// <summary>
    ///     Processador de eventos.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IContainer _container;

        public EventDispatcher(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Dispara um <see cref="IEventTrigger{IEvent}" /> registrado em <see cref="IContainer" />.
        /// </summary>
        /// <param name="event"></param>
        public void Dispatch(IEvent @event)
        {
            Guard.ArgumentIsNotNull(@event, nameof(@event));

            var triggerType = typeof (IEventTrigger<>).MakeGenericType(@event.GetType());
            dynamic trigger = _container.GetInstance(triggerType);

            trigger.Trigger((dynamic) @event);
        }
    }
}
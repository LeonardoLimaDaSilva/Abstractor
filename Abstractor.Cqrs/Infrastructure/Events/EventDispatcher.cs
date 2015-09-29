using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IContainer _container;

        public EventDispatcher(IContainer container)
        {
            _container = container;
        }

        public void Dispatch(IEvent @event)
        {
            var triggerType = typeof(IEventTrigger<>).MakeGenericType(@event.GetType());
            dynamic trigger = _container.GetInstance(triggerType);

            trigger.Trigger((dynamic)@event);
        }
    }
}

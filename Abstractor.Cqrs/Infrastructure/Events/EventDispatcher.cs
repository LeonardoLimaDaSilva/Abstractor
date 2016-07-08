using System.Diagnostics;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    /// <summary>
    ///     Dispatches the event listener.
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
        ///     Delegates the event listener to the multiple events trigger.
        /// </summary>
        /// <param name="eventListener"></param>
        public void Dispatch(IEventListener eventListener)
        {
            Guard.ArgumentIsNotNull(eventListener, nameof(eventListener));

            var triggerType = typeof (IEventTrigger<>).MakeGenericType(eventListener.GetType());
            dynamic trigger = _container.GetInstance(triggerType);

            trigger.Trigger((dynamic)eventListener);
        }
    }
}
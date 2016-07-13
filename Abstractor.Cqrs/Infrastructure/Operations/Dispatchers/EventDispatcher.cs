using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Dispatcher for all the events that subscribes to the <see cref="IEventListener" />.
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
        ///     Delegates the event listener and dispatches to all event handlers that subscribes to <see cref="IEventListener" />.
        /// </summary>
        /// <param name="eventListener"></param>
        public void Dispatch(IEventListener eventListener)
        {
            Guard.ArgumentIsNotNull(eventListener, nameof(eventListener));

            var triggerType = typeof (IEventHandler<>).MakeGenericType(eventListener.GetType());
            dynamic handlers = _container.GetAllInstances(triggerType);

            foreach (var handler in handlers)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        handler.Handle((dynamic) eventListener);
                    }
                    catch
                    {
                        // supression of event handler exceptions
                    }
                });
            }
        }
    }
}
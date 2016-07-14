using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Dispatcher for all the event handlers that subscribes to the <see cref="IApplicationEvent" />.
    /// </summary>
    public sealed class ApplicationEventDispatcher : IApplicationEventDispatcher
    {
        private readonly IContainer _container;

        public ApplicationEventDispatcher(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Delegates the event and dispatches to all event handlers that subscribes to <see cref="IApplicationEvent" />.
        /// </summary>
        /// <param name="applicationEvent">Event to be dispatched.</param>
        public void Dispatch(IApplicationEvent applicationEvent)
        {
            Guard.ArgumentIsNotNull(applicationEvent, nameof(applicationEvent));

            var handlerType = typeof (IApplicationEventHandler<>).MakeGenericType(applicationEvent.GetType());
            dynamic handlers = _container.GetAllInstances(handlerType);

            foreach (var handler in handlers)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        handler.Handle((dynamic) applicationEvent);
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
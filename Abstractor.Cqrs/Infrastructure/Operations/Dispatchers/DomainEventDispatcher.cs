using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Dispatcher for all the event handlers that subscribes to the <see cref="IDomainEvent" />.
    /// </summary>
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IContainer _container;

        public DomainEventDispatcher(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Delegates the event and dispatches to all event handlers that subscribes to <see cref="IDomainEvent" />.
        /// </summary>
        /// <param name="domainEvent">Event to be dispatched.</param>
        public void Dispatch(IDomainEvent domainEvent)
        {
            Guard.ArgumentIsNotNull(domainEvent, nameof(domainEvent));

            var handlerType = typeof (IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            dynamic handlers = _container.GetAllInstances(handlerType);

            foreach (var handler in handlers)
                handler.Handle((dynamic) domainEvent);
        }
    }
}
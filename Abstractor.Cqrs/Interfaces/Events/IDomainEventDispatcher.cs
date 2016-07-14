namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Dispatcher for all the event handlers that subscribes to the <see cref="IDomainEvent" />.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        ///     Delegates the event and dispatches to all event handlers that subscribes to <see cref="IDomainEvent" />.
        /// </summary>
        /// <param name="domainEvent">Event to be dispatched.</param>
        void Dispatch(IDomainEvent domainEvent);
    }
}
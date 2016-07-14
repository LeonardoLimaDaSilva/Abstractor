namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Event handler that subscribes to the <see cref="IDomainEvent" />.
    /// </summary>
    /// <typeparam name="TEvent">Domain event in which the handler subscribes to.</typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        ///     Handles the <see cref="IDomainEvent" />.
        /// </summary>
        /// <param name="domainEvent">Domain event to be handled.</param>
        void Handle(TEvent domainEvent);
    }
}
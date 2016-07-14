namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     A domain event is executed transactionally inside the scope of a command. The framework discovers and dispatches
    ///     all event handlers that implements <see cref="IDomainEventHandler{TEvent}" /> interface.
    /// </summary>
    public interface IDomainEvent
    {
    }
}
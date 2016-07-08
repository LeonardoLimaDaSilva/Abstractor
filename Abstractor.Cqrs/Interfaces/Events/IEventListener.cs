namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Marks a class as an event listener allowing the framework to discover and dispatch all event handlers
    ///     that implements <see cref="IEventHandler{TEvent}" /> interface.
    /// </summary>
    public interface IEventListener
    {
    }
}
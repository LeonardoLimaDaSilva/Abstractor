namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Marks a class as an event listener. The framework will discover and trigger all event handlers
    ///     that implements <see cref="IEventHandler{TEvent}" /> interface.
    /// </summary>
    public interface IEventListener
    {
    }
}
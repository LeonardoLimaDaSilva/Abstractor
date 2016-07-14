namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     An application event is executed asynchronously, on a dedicated thread. The framework discovers and dispatches
    ///     all event handlers that implements <see cref="IApplicationEventHandler{TEvent}" /> interface.
    /// </summary>
    public interface IApplicationEvent
    {
    }
}
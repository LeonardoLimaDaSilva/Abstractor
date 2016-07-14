namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Dispatcher for all the event handlers that subscribes to the <see cref="IApplicationEvent" />.
    /// </summary>
    public interface IApplicationEventDispatcher
    {
        /// <summary>
        ///     Delegates the event and dispatches to all event handlers that subscribes to <see cref="IApplicationEvent" />.
        /// </summary>
        /// <param name="applicationEvent">Event to be dispatched.</param>
        void Dispatch(IApplicationEvent applicationEvent);
    }
}
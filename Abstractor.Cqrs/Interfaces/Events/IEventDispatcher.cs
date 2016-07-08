namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Dispatcher for all the events that subscribes to the <see cref="IEventListener" />.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        ///     Delegates the event listener and dispatches to all event handlers that subscribes to <see cref="IEventListener" />.
        /// </summary>
        /// <param name="eventListener"></param>
        void Dispatch(IEventListener eventListener);
    }
}
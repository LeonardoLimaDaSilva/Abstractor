namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Dispatches the event listener.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        ///     Delegates the event listener to the multiple events trigger.
        /// </summary>
        /// <param name="eventListener"></param>
        void Dispatch(IEventListener eventListener);
    }
}
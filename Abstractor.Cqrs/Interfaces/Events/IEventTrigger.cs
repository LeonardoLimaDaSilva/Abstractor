namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Abstraction for the event trigger.
    /// </summary>
    /// <typeparam name="TEventListener"></typeparam>
    public interface IEventTrigger<in TEventListener> where TEventListener : IEventListener
    {
        /// <summary>
        ///     Triggers all event handlers registered for the current event listener.
        /// </summary>
        /// <param name="eventListener">Listener for the event handlers.</param>
        void Trigger(TEventListener eventListener);
    }
}
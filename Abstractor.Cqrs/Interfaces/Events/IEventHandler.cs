namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Handler for the event that subscribes to the <see cref="IEventListener" />.
    /// </summary>
    /// <typeparam name="TEventListener">Listener that the event subscribes to.</typeparam>
    public interface IEventHandler<in TEventListener> where TEventListener : IEventListener
    {
        /// <summary>
        ///     Handle the event subscribed to the <see cref="IEventListener" />.
        /// </summary>
        /// <param name="eventListener">Listener that the event subscribes to.</param>
        void Handle(TEventListener eventListener);
    }
}
namespace Abstractor.Cqrs.Interfaces.Events
{
    public interface IEventTrigger<in TEvent> where TEvent : IEvent
    {
        /// <summary>
        ///     Dispara um <see cref="IEventHandler{TEvent}" />.
        /// </summary>
        /// <param name="event">Evento que será disparado.</param>
        void Trigger(TEvent @event);
    }
}
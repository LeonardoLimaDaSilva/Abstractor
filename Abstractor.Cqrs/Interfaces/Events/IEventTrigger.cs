namespace Abstractor.Cqrs.Interfaces.Events
{
    public interface IEventTrigger<in TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// Dispara o <see cref="IEventHandler{TEvent}"/> de <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="event">Evento que será disparado.</param>
        void Trigger(TEvent @event);
    }
}

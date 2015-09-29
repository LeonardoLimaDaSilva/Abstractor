namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    /// Manipula o <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">Evento que será manipulado.</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// Manipula o <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="event"><typeparamref name="TEvent"/> que será manipulado.</param>
        /// <returns>Retorna uma task.</returns>
        void Handle(TEvent @event);
    }
}
